WMS related utils such as:
* WMS driver (see notes)
* Manifold WMS driver
* GDAL WMS driver
* SharpMap WMS driver
* TileCache

depends on:
* https://github.com/cartomatic/Utils
* https://github.com/cartomatic/OgcSchemas

project board: https://waffle.io/cartomatic/Wms

Notes:
WMS driver - driver term was borrowed from manifold, as most of the functionality was created in order to enable reasonable WMS endpoint for manifold projects;
manifold calls its IMS WMS functionality a WMS driver;
So a driver here is pretty much a box that takes in a request object or string url and responds according to the WMS spec;
In other words - WmsDriver exposes OGC WMS interface to a GIS engine working behind.
