﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Wms;

namespace Cartomatic.Manifold
{
    public partial class WmsDriver
    {
        protected internal void CreateMapServer()
        {
            if (string.IsNullOrEmpty(MapFile) || !System.IO.File.Exists(MapFile))
            {
                throw new WmsDriverException("SETUP ERROR: Specified map file must exist.");
            }
        }

        protected string GetMapServerOpts(bool antialias = true)
        {
            string config;

            config =
                "file=" + MapFile + Environment.NewLine +
                "component=" + MapComp + Environment.NewLine +
                "cx=256" + Environment.NewLine + //map width
                "cy=256" + Environment.NewLine + //map height
                "smoothLargeVectorObjects=true" + Environment.NewLine + //turn on smoothing vectors so performance increases at a slight cost of quality

                "antialiasLines=" + antialias.ToString() + Environment.NewLine +
                "antialiasText=" + antialias.ToString() + Environment.NewLine +

                //use a default render quality initally. it gets adjusted later if provided through the map file
                "renderQuality=" + DEFAULT_RENDER_QUALITY.ToString() + Environment.NewLine +

                "logo=false";

            return config;
        }
    }
}