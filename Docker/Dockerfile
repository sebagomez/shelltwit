FROM microsoft/dotnet:2.1-runtime

LABEL name="SebaGomez Shelltwit client" \
      description="Run your own shelltwit from this image" \
      vcs-url="https://github.com/sebagomez/shelltwit" \
      vendor="@sebagomez" \
      schema-version="1.0" \
      maintainer="Seba Gómez <twitter.com/sebagomez>"

#For automated build purposes
ARG TAG=8.2

RUN mkdir twit

WORKDIR twit

ADD https://github.com/sebagomez/shelltwit/releases/download/$TAG/shelltwit.tar.gz ./

RUN tar xvf shelltwit.tar.gz

ENTRYPOINT ["dotnet", "Sebagomez.Shelltwit.dll"] 

#ENTRYPOINT ["bash"] 
