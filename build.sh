cp shelltwit/Properties/AssemblyInfo.cs shelltwitcore/Properties/ --verbose
cp ./CommonAssemblyInfo.cs ./shelltwitcore/Properties/ --verbose
cp ./shelltwit/*.cs ./shelltwitcore/ --verbose
cp ./shelltwit/*.data ./shelltwitcore/ --verbose
cp ./shelltwit/*.data ./shelltwitcore/bin/Debug/netcoreapp1.1/ --verbose
cp -r ./shelltwitlib/*/*.cs ./shelltwitcore/ --verbose
cp ./BitLyHelper/Util.cs ./BitLyHelper/BitLyUtil.cs --verbose
cp ./BitLyHelper/BitLyUtil.cs ./shelltwitcore/ --verbose
rm ./BitLyHelper/BitLyUtil.cs --verbose
dotnet restore -s ./shelltwitcore && dotnet build ./shelltwitcore