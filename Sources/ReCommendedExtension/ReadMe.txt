﻿-----------PREREQUISITES

Visual Studio is installed in

	C:\Program Files (x86)\Microsoft Visual Studio

Resharper Platform is installed into the experimental hive "ReSharper"

1. start the Visual Studio using the experimental hive "ReSharper":

	"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\devenv.exe" /rootSuffix ReSharper

2. install the package into the experimental hive "ReSharper"

3. make suse the HostFullIdentifier property in the .csproj file is initialized (use the value suggested in the build output)

	<HostFullIdentifier>ReSharperPlatformVs16ReSharper</HostFullIdentifier>

-----------DEBUGGING

To debug your plug-in, simply make sure it is selected as the start-up
project and press F5.

Debugging Settings:

	Start external program: "C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\devenv.exe"

	Command line arguments: /ReSharper.Internal /rootSuffix ReSharper

If Visual Studio still claims that PDB files are missing check the loaded assemblies (Debug/Windows/Modules) and search for "ReCommendedExtension".

-----------IMPORTANT LINKS

https://www.jetbrains.com/resharper/devguide
https://www.jetbrains.com/resharper/devguide/Extensions/Deployment/LocalInstallation/CopyOnBuild.html