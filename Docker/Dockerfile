FROM microsoft/dotnet:2.1-runtime

LABEL com.sebagomez.name="SebaGomez Shelltwit client" \
      com.sebagomez.description="Run your own shelltwit from this image" \
      com.sebagomez.vcs-url="https://github.com/sebagomez/shelltwit" \
      com.sebagomez.vendor="@sebagomez" \
      com.sebagomez.schema-version="1.0" \
      com.sebagomez.maintainer="Seba Gómez <twitter.com/sebagomez>"

#For automated build purposes
ARG TAG=7.8.2 

RUN mkdir twit

WORKDIR twit

ADD https://github.com/sebagomez/shelltwit/releases/download/$TAG/shelltwit.tar.gz ./

RUN tar xvf shelltwit.tar.gz

ENTRYPOINT ["dotnet", "Sebagomez.Shelltwit.dll"] 

#ENTRYPOINT ["bash"] 
