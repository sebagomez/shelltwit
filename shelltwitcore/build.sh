cp ../shelltwit/Properties/AssemblyInfo.cs ./Properties/ --verbose
cp ../CommonAssemblyInfo.cs ./Properties/ --verbose
cp ../shelltwit/*.cs . --verbose
cp ../shelltwit/*.data . --verbose
cp ../shelltwit/*.data ./bin/Debug/netcoreapp1.1/ --verbose
cp -r ../shelltwitlib/*/*.cs . --verbose
cp ../BitLyHelper/Util.cs ../BitLyHelper/BitLyUtil.cs --verbose
cp ../BitLyHelper/BitLyUtil.cs . --verbose
rm ../BitLyHelper/BitLyUtil.cs --verbose
