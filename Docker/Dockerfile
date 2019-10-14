FROM microsoft/dotnet:3.0-runtime

LABEL maintainer="seba gomez <sebagomezcorrea@outlook.com>"

#For automated build purposes
ARG BUILD

ENV APPBUILD=$BUILD

RUN mkdir twit

WORKDIR twit

COPY ["bin", "/twit"]

ENTRYPOINT ["dotnet", "Sebagomez.Shelltwit.dll"] 
