# SharpTracer
C# implementation of Ray Tracing in One Weekend by Peter Shirley

## To use
`dotnet run [height] [width] [samples per pixel] (filename)`

Runs in parallel thanks to plinq. Still quite slow, next step would be some kind of spatial indexing on the spheres in the scene.

## Sample Image
![Array of spheres on a plane](sample.png)
