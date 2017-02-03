cp shelltwit/Properties/AssemblyInfo.cs shelltwitcore/Properties/ --verbose
cp ./CommonAssemblyInfo.cs ./shelltwitcore/Properties/ --verbose
cp ./shelltwit/*.cs ./shelltwitcore/ --verbose
rsync -avh --include="*.cs" --exclude="*.*" ./shelltwitlib/ ./shelltwitcore
cp ./BitLyHelper/Util.cs ./BitLyHelper/BitLyUtil.cs --verbose
cp ./BitLyHelper/BitLyUtil.cs ./shelltwitcore/ --verbose
rm ./BitLyHelper/BitLyUtil.cs --verbose
cd shelltwitcore
dotnet restore && dotnet build 