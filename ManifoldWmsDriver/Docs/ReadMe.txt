Custom Manifold WMS
Manifold WMS is a customized library that is responsible for exposing the manifold project map component through the WMS interface.
Only map components can be exposed as the WMS. Any number of maps can be served, each map can contain totally independent data and be in any projection
(at this stage wms driver DOES NOT do any reprojecting on the fly due to performance reasons; it is not likely so far that the reprojection will be implemented – 
likely though to implement multiple map comps that will ‘pretend’ one data source is exposed in different projections).

In order to properly inform the driver what are the particular map component settings it is necessary to provide a comments component named wms.settings.

Wms settings
Wms.settings is a comments component containing JSON configuration for the served map components. If the data held by it is not present or invalid, wms driver will throw errors.
The data is an array of configuration objects applicable to the served map components
For the details on the configuration see WmsDriverSettingsExample.txt


Refreshing linked data with AOI
WmsDriverManifold originated as a custom WMS interface that was require to serve large amounts of db linked vector data. While manifold can do this of course, loading such data
when the project file loads may be extremaly time consuming (from a perspective of a web app).
Therefore some mechanisms to allow dynamic data refreshes per WMS request were developed. See the WmsDriverSettingsExample.txt for details

Enabling automatic AOI-ing for big data projects:
1.	Disable global automatic refresh of the linked components so the data is not pulled by manifold upon project file open.
	To do so: Tools/Options/Miscellaneous/Refresh linked components after opening the file == false (unchecked)
 
2.	Configure WMS driver to perform auto AOI-ing for the linked components:
	Add AutoAOI: true to the driver settings for given map component; also see the WmsDriverSettingsExample.txt for the configuration details

3.	Ensure proper visibility settings are applied to a linked component, so the amount of data refreshed at each viewport is limited; display the layer only at largest zooms
	IMPORTANT – if the scales are not set properly, it may result in blocking the wms while it tries to pull all the data for a big viewport
	This will likely result in wms app become unresponsive and crash eventually

Notes on linked data:
•	Connection for the data source must include the proper user name / pass otherwise when runned as a iis service,
	the windows credentials will fail (well of course if iis does not have its own db credentials)
•	Legend must be flattened for the dynamic aoi linked components otherwise it will not render properly; this makes the legend static of course!
•	Manifold must be set to not refresh the linked drawings on load
•	Proper visibility scales must be set in order to avoid retrieving too much data for the AOI!!!
•	AOI get caps will lie so far (unless some static info is placed in the wms settings for the linked component - not implemented yet)
•	It is possible to use a fake linked component – in such case a full select query is required in order to retrieve data from db!