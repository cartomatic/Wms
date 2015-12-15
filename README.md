WMS related utils such as:
* WMS driver 
* Manifold WMS driver
* GDAL WMS driver
* SharpMap WMS driver
* TileCache

Note:
WMS driver - driver term was borrowed from manifold, as most of the functionality was created in order to enable reasonable WMS endpoint for manifold projects;
manifold calls its IMS WMS functionality a WMS driver; so a driver here is pretty much a box that takes in a request object or url and responds according to the WMS spec.