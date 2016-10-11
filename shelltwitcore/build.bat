xcopy ..\shelltwit\Properties\AssemblyInfo.cs .\Properties\ /D /Y
xcopy ..\CommonAssemblyInfo.cs .\Properties\ /D /Y
xcopy ..\shelltwit\*.cs . /D /Y /EXCLUDE:exclude.txt
xcopy ..\shelltwit\*.data . /Y
xcopy ..\shelltwit\*.data .\bin\Debug\netcoreapp1.0 /Y
xcopy ..\shelltwitlib\*.cs . /D /Y /S /EXCLUDE:exclude.txt
copy ..\BitLyHelper\Util.cs ..\BitLyHelper\BitLyUtil.cs
xcopy ..\BitLyHelper\BitLyUtil.cs . /D /Y
del ..\BitLyHelper\BitLyUtil.cs