@echo off
set Config=%1
if %1. == . set Config=Release

dotnet build --force -c %Config% shelltwit.sln
dotnet publish -c %Config% -o ..\bin shelltwit.sln 
