using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cartomatic.Wms;
using Cartomatic.Utils.Data;

using M = Manifold.Interop;

namespace Cartomatic.Wms
{
    public partial class ManifoldWmsDriver
    {
        /// <summary>
        /// A locker used when performin AOI based data refreshes
        /// </summary>
        protected object _aoiRefreshLocker = new object();

        /// <summary>
        /// Performs an AOI based data refresh of linked components; only drawing components get refreshed
        /// </summary>
        /// <param name="bbox"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private void AutoAOI(WmsBoundingBox bbox, int? width, int? height)
        {

            //served component should be a map. the driver will fail if it sis not so when getting here, it should be ok
            var map = MapServer.Component as M.Map;

            //get the app
            var app = map.Application;

            //get the current pixel size so aoi can be readjusted in order to include a collar - this will pull more data,
            //although should improve rendering in a case objects are labelled and the label resolve is on
            var mapPixSize = (bbox.MaxX - bbox.MinX) / width;

            //by default only refresh data exactly within the required bounds
            //unless it has been explicitly set to be refreshed within larger tile to improve rendering quality (labels placement ;)
            double collar = 0;
            if (MSettings.AoiCollar.HasValue)
            {
                collar = MSettings.AoiCollar.Value;
            }

            //express the collar in map units
            collar *= mapPixSize.Value;

            //extend the bbox edges to include a collar for the aoi-ed data
            var bl = app.NewPoint(bbox.MinX - collar, bbox.MinY - collar);
            var tr = app.NewPoint(bbox.MaxX + collar, bbox.MaxY + collar);

            var refreshedComps = new List<string>();

            //iterate through the visible layers and check if the parent comp is a drawing and is linked
            foreach (M.Layer l in map.LayerSet)
            {
                var comp = l.Component;

                if(!MapServer._GetLayerShown(comp.Name))
                    continue;

                M.Drawing d = null;

                if (comp.Type == M.ComponentType.ComponentDrawing)
                {
                    d = comp as M.Drawing;
                }
                else if (comp.Type == M.ComponentType.ComponentTheme)
                {
                    d = (comp as M.Theme).OwnerDrawing as M.Drawing;
                }

                //if layer component is not drawing or theme skip it
                if (d == null) continue;


                //skip the drawings that already have been refreshed
                if (refreshedComps.Exists(dname => dname == d.Name)) continue;

                var aois = GetAoiSettings(d.Name);

                //take care of the linked drawings - this is the default behavior and usage
                //but sometimes causes the IIS / worker process to die...
                if (d.IsLinked())
                {
                    RefreshLinkedDrawing(d, bl, tr);
                    refreshedComps.Add(d.Name);
                }

                //refresh a fake linked drawing - customised stuff for working with large vector datasets
                if (aois != null)
                {
                    RefreshFakeLinkedDrawing(d, aois, bl, tr);
                    refreshedComps.Add(d.Name);
                }

            }
        }

        /// <summary>
        /// Refreshes a linked drawing
        /// </summary>
        /// <param name="d"></param>
        /// <param name="bl"></param>
        /// <param name="tr"></param>
        protected void RefreshLinkedDrawing(M.Drawing d, M.Point bl, M.Point tr)
        {

            var app = d.Application;
            var map = MapServer.Component as M.Map;

            //usually drawing will be in the same projection as the map for better performance
            //if it is in a different projection though, the wms request bounds must be projected to drawing's cs in order to properly refresh it
            if (!map.CoordinateSystem.IsEqualTo(d.CoordinateSystem, true, true))
            {
                var cc = app.NewCoordinateConverter();
                cc.Prepare((M.Base)map.CoordinateSystem, (M.Base)d.CoordinateSystem);
                cc.Convert((M.Base)bl, null);
                cc.Convert((M.Base)tr, null);
            }


            //clone the cs of the drawing prior to updating the data
            //in a case manifold does not recognise the srid properly, cs of the linked drawing
            //will be reset to lonlat
            M.CoordinateSystem cs = app.DefaultCoordinateSystem;
            cs.Copy((M.Base)d.CoordinateSystem);

            //the aoi edges should now be ok, so can do the aoi refresh
            //Note: layers that are not visible in scale are turned off by mapserver when checking out what should be printed out
            d.RefreshWithArea(app.NewRect(bl.X, bl.Y, tr.X, tr.Y), false);


            //restore the cs if needed
            if (!cs.IsEqualTo(d.CoordinateSystem, true, true))
            {
                d.CoordinateSystem = cs;
                d.CoordinateSystemVerified = true;
            }
        }

        /// <summary>
        /// Refreshes data of the fake linked drawing;
        /// </summary>
        /// <remarks>
        /// This is customised logic for the 'linked' data retrieval. Data is located in the database and a drawing is only a holder for the data.
        /// in order to render it the logic is to:
        /// 1. wipe out the drawing data
        /// 2. read data from a db
        /// 3. create sql insert based on the retrieved data to populate the drawing objectset
        /// 
        /// In general this approach should be slower than the direct connection manifold can make but:
        /// 1. on some occassions manifold just dies trying to render the data
        /// 2. manifold is very ineffective when talking to postgresql / postgis due to the way it querries the data - it makes pgsql work harder than it is needed and therefore the pgsql's speed advantage over ms sqlserver for example is not utilised
        /// 
        /// so there are chances that for reasonable amounts of data transferred this approach can actually yield better results. even though the geom data is actually transferred as wkt...
        /// Note: geom can be extracted as binary from both - sql server and postgres!!!!
        /// 
        /// The main problem with this is that this is not the standard approach to linking the data and the project component must be configured properly, otherwise it will either not work or cause further errors
        /// 
        /// This assumes that the actual data model of the drawing being fake linked is extremally simplistic (to improve performance) and is only made of one column called label (variable text).
        /// data returned by the query is: l text, g text
        /// 
        /// connection string to the data source as well as the actual select query is defined within the wms.settings component.
        /// 
        /// More details on using this feature in the WmsDriverAoiSettings documentation
        /// </remarks>
        /// <param name="d"></param>
        /// <param name="aois"></param>
        /// <param name="bl"></param>
        /// <param name="tr"></param>
        protected void RefreshFakeLinkedDrawing(M.Drawing d, ManifoldWmsDriverAoiSettings aois, M.Point bl, M.Point tr)
        {

            var app = d.Application;
            var map = MapServer.Component as M.Map;

            //get the data
            var t = GetAoiData(aois, bl, tr);


            //check if the geom comes as binary or wkt
            if (aois.UseBinaryGeom)
            {
                //binary load is slow as M8 .NET accessors are slow.
                LoadFakeDrawingBinaryGeoms(d, t);
            }
            else
            {
                //Loading data as wkt is quicker than as binary
                //manifold8 query engine parsing wkt gives better results...
                LoadFakeDrawingWktGeoms(d, app, map, t);
            }
        }

        /// <summary>
        /// Loads wkt geoms to a fake drawing. In M8 parsing wkt within a query is quicker than using object accessors
        /// </summary>
        /// <param name="d"></param>
        /// <param name="app"></param>
        /// <param name="map"></param>
        /// <param name="t"></param>
        private static void LoadFakeDrawingWktGeoms(M.Drawing d, M.Application app, M.Map map, DataTable t)
        {
            //Note:
            //geom comes as wkt


            //fake table comp with one rec so can properly execute manifold query
            var mcs = app.NewColumnSet();
            var mc = mcs.NewColumn();
            mc.Name = "name";
            mc.set_Type(M.ColumnType.ColumnTypeInt8U);
            mcs.Add(mc);
            var mt = map.Document.NewTable("t", mcs, false);
            mt.RecordSet.AddNew();


            //prepare data inserts in batches
            int batchSize = 50;
            int counter = 0;

            List<List<string>> querries = new List<List<string>>();

            while (counter < t.Rows.Count)
            {
                List<string> data = new List<string>();

                for (int i = 0; i < batchSize; i++)
                {
                    //make sure there is something left in the table
                    if (counter >= t.Rows.Count) break;

                    var r = t.Rows[counter];

                    data.Add("SELECT AssignCoordSys(CGeomWKB(\"" + r["g"] + "\"), coordsys(\"" + d.Name +
                             "\" as COMPONENT)), \"" + r["l"] + "\" from [" + mt.Name + "]");

                    //update counter
                    counter++;
                }

                //if there is data gathered
                if (data.Count > 0)
                {
                    querries.Add(data);
                }
            }


            //query comp - used to manipulate the data
            var q = map.Document.NewQuery("q", false);


            //delete data
            q.Text = "DELETE FROM [" + d.Name + "];";
            q.Run();


            //and load new stuff
            foreach (var qry in querries)
            {
                q.Text =
                    "INSERT INTO [" + d.Name + "] ([Geom (I)], [l]) " +
                    string.Join(" UNION ALL ", qry);
                q.Run();
            }

            //finally remove the temp comps
            map.Document.ComponentSet.Remove(q);
            map.Document.ComponentSet.Remove(mt);
        }


        /// <summary>
        /// Loads binary geoms onto the specified drawing.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="t"></param>
        private static void LoadFakeDrawingBinaryGeoms(M.Drawing d, DataTable t)
        {
            //Note:
            //manifold accesors are not very efficient so the code below will be very slow.

            var app = d.Application;

            //remove the previous data
            d.ObjectSet.RemoveAll();

            //geom comes as binary data
            foreach (DataRow r in t.Rows)
            {
                d.ObjectSet.Add(app.NewGeomFromBinaryWKB(r["g"]));

                //This assumes that the input type is exactly the same as the declared one in the drawing model!!!!
                d.ObjectSet[d.ObjectSet.Count - 1].Record._PutData("l", r["l"]);
            }
        }

        /// <summary>
        /// Reads data for the fake linked drawing
        /// </summary>
        /// <param name="aois"></param>
        /// <param name="bl"></param>
        /// <param name="tr"></param>
        /// <returns></returns>
        protected DataTable GetAoiData(ManifoldWmsDriverAoiSettings aois, M.Point bl, M.Point tr)
        {
            var dbc = aois.DataSourceCredentials;

            //prepare the required db stuff
            IDbConnection conn = dbc.GetDbConnectionObject();
            var cmd = dbc.GetDbCommandObject();
            cmd.Connection = conn;
            var da = dbc.GetDbDataAdapterObject();
            da.SelectCommand = cmd;

            var t = new DataTable();

            //adjust spatial filtering
            cmd.CommandText =
                aois.Query
                    .Replace("{l}", bl.X.ToString(System.Globalization.CultureInfo.InvariantCulture))
                    .Replace("{b}", bl.Y.ToString(System.Globalization.CultureInfo.InvariantCulture))
                    .Replace("{r}", tr.X.ToString(System.Globalization.CultureInfo.InvariantCulture))
                    .Replace("{t}", tr.Y.ToString(System.Globalization.CultureInfo.InvariantCulture));


            //get the data
            try
            {
                conn.Open();

                //IDataAdapter does not have the Fill def for a table, hence cast...
                ((dynamic)da).Fill(t);
            }
            catch (Exception ex)
            {
                //Cartomatic.Utils.Exceptions.ExceptionLogger.logException(ex, "aoi_refresh");

                //rethrow exception, otherwise a blank image will get cached which is not good!
                throw ex;
            }
            finally
            {
                conn.CloseConnection();
            }

            return t;
        }

        /// <summary>
        /// Returns AOI settings for specified component name
        /// </summary>
        /// <param name="cname"></param>
        /// <returns></returns>
        private ManifoldWmsDriverAoiSettings GetAoiSettings(string cname)
        {
            return MSettings.AoiSettings.FirstOrDefault(aois => aois.Comp == cname); ;
        }

    }
}
