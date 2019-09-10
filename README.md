# MeshSimplificationComparer
Program for comparing various triangular mesh simplification implementations, i.a Blender, OpenFlipper, MeshLab, Progressive Meshes, CGAL Surface Mesh Simplification.

## Functionality

This program is capable of running up to 8 mesh simplification programs at five levels of detail - 50\%, 25\%, 5\%, 2\% i 1\% (running all 8 results in creating 40 simplified meshes). Each level of detail represents a percentage of triangles of the output mesh compared to the input mesh. As of now the steps are hard-coded.

Foreach of the simplified meshes it can also do the following:
- Register and save time of exectuing the simplification
- Calculate so-called one-sided Hausdorff distance in comparison to the original mesh (this is done by sampling vertices of the original mesh)  
- Render mesh in six orthographic views (top, bottom, left, right, front, back) and save them as bitmap files (.png).

All of the Hausdorff metrics are combined into tables and plots ready to be used in LaTeX.
It is also possible to create renders of the input mesh.

## Instalation and Setup

Download the binaries or compile MeshSimplificationComparer in Visual Studio.

To compare all implementations you will have to install the following (but to start with you only need the bold-fonted ones):
- **MeshLab 2016**: http://www.meshlab.net/
- QSlim 2.1: https://mgarland.org/software/qslim.html (compiled program can be found here: http://qipeng.me/software/bugfix-qslim-21.html)
- OpenFlipper 3.1: https://www.openflipper.org/
- **Blender 2.79**: https://www.blender.org/download/releases/2-79/ 
- MyQem: https://github.com/ShuTheWise/MyQem
- Fogleman: https://github.com/fogleman, this also requires Golang requires https://golang.org/
- SimplifyCGAL: https://github.com/ShuTheWise/SimplifyCGAL
- Mesh processing library: https://github.com/microsoft/Mesh-processing-library, library containing Progressive Meshes algorithm, if you wish to run this you will also need perl  https://www.perl.org/

It is absolutely crucial to install MeshLab as it handles all mesh preprocessing as well as generating Hausdorff distance metrics.
If you wish to generate renders (in .png) you will also need Blender.

You can either install (or place) the above list of program in locations provided in the `MeshSimplificationComparer.exe.config` file or you can change the values in this file to your preference, here is a part of this file:
```
  <appSettings>
    <add key="meshlab" value="C:\Program Files\VCG\MeshLab\meshlabserver.exe" />
    <add key="qslim" value="C:\Program Files\QSlim\QSlim.exe" />
    <add key="openflipper" value="C:\Program Files\OpenFlipper 3.1\OpenFlipper.exe" />
    <add key="blender" value="C:\Program Files\Blender Foundation\Blender\blender.exe" />
    <add key="myqem" value="qem" />
    <add key="foglemanqem" value="simplify" />
    <add key="cgal" value="C:\Program Files\SimplifyCGAL\SimplifyCGAL.exe" />
    <add key="perl" value="C:\Perl64\bin\perl.exe" />
    <add key="pm" value="C:\Mesh-processing-library\bin\Win32\debug" />
    <add key="verbose" value="false" />
    <add key="ClientSettingsProvider.ServiceUri" value="" />
  </appSettings>
```

## How to use

1. Navigate to the folder containing `MeshSimplificationComparer.exe` and create a folder called `Models` inside this directory if it's not there already. Make sure there is at least one triangular mesh inside the folder in a supported file format.
2. Run `MeshSimplificationComparer.exe` this should prompt a list of models like the following:
```
4 models found:
0: .\Models\armadillo.ply
1: .\Models\bunny.obj
2: .\Models\desertrose.obj
3: .\Models\ogr.obj
```

Choose one or more models by providing their indexes with separators, for example `0` - only armadillo, `1,2` - bunny and desert rose. You can also press enter to run all.

3. Prompt `Enter name:` makes you choose a working directory name. This will do some preprocessing on the mesh and convert it to file formats used by other algorithms. From now on everything the program does will be stored in `mc_output\<Your working directory name>`. This also acts as a caching mechanism so you don't have to preprocess the mesh every time you want to run the algorithms.
4. Choose mesh simplification algorithms you wish to run (you need to have installed those programs):

```
8 programs found:
0: MeshSimplificationComparer.MeshLabQem
1: MeshSimplificationComparer.FoglemanQem
2: MeshSimplificationComparer.MyQem
3: MeshSimplificationComparer.OpenFlipper
4: MeshSimplificationComparer.Blender
5: MeshSimplificationComparer.QSlim
6: MeshSimplificationComparer.SimplifyCGAL
7: MeshSimplificationComparer.ProgressiveMeshes
```

Enter id's just like you would previously, i.e. `0, 4, 6` runs MeshLabQem, Blender and SimplifyCGAL or press enter to run all.

5. Choose operations you wish to run:

```
4 operations found:
0: Run simplification algorithms to create simplified meshes
1: Run unification of output mesh formats (all to .obj)
2: Run metrics on the simplified meshes (Hausdorff Distance Calculation)
3: Render images of the simplified meshes (Blender render)
```
Same way of entering as before.

Executing operation 0 runs all the meshes you have chosen against algorithms chosen at five levels of detail - 50\%, 25\%, 5\%, 2\% i 1\%. Output meshes will be put in `mc_output\<Your working directory name>\meshes`.
Do not use operation 2 unless you are sure that all meshes in the `meshes` folder are in .obj file format. You can convert all meshes to this format using operation 1.
After running operation 2 you will find the results in `mc_output\<Your working directory name>\data`.
Running operation 3 outputs the renders in `mc_output\<Your working directory name>\renders`

You can run the application many selecting different algoritms and operations but you have to keep in mind the state of your data. For example, if you wish to solely run operation 3 or 4 there have to be simplified meshes cached in the `meshes` folder.

### Supported input mesh files formats
Program supports basically all file formats as MeshLab that is:
- .3ds, 3D Studio format;
- .collada, open XML standard for 3D data exchange;
- .obj, Wavefront object format;
- .off, GEOMVIEW Object File Format;
- .ply, the Stanford triangle mesh format;
- .stl, the stereolithography format, ASCII or binary;
