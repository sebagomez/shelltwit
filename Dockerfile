FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /src

COPY ./src .

RUN dotnet publish shelltwit.csproj -c Release -o ./bin

FROM mcr.microsoft.com/dotnet/runtime:6.0

LABEL maintainer="Sebastián Gómez <@sebagomez>"

#For automated build purposes
ARG BUILD

ENV APPBUILD=$BUILD

WORKDIR /twit

COPY --from=build /src/bin . 

ENTRYPOINT ["dotnet", "Sebagomez.Shelltwit.dll"] 
