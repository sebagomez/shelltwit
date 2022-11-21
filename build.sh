#!/bin/bash
config=Debug

dotnet build -c $config ./shelltwit.sln
dotnet publish -c $config -o ./bin shelltwit.sln 
