FROM microsoft/dotnet:2-runtime

LABEL com.sebagomez.name="SebaGomez Shelltwit client" \
      com.genexus.description="Run your own shelltwit from this image" \
      com.genexus.vcs-url="https://github.com/sebagomez/shelltwit" \
      com.genexus.vendor="@sebagomez" \
      com.genexus.schema-version="1.0" \
      com.genexus.maintainer="Seba Gómez <twitter.com/sebagomez>"

#For automated build purposes
ARG TAG=7.4 

RUN mkdir twit

WORKDIR twit

ADD https://github.com/sebagomez/shelltwit/releases/download/$TAG/shelltwit.tar.gz ./

RUN tar xvf shelltwit.tar.gz

ENTRYPOINT ["dotnet", "Sebagomez.Shelltwit.dll"] 

#ENTRYPOINT ["bash"] 
