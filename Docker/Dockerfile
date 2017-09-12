FROM microsoft/dotnet:2-runtime

LABEL com.sebagomez.name="SebaGomez Shelltwit client" \
      com.genexus.description="Run your own shelltwit from this image" \
      com.genexus.vcs-url="https://github.com/sebagomez/shelltwit" \
      com.genexus.vendor="@sebagomez" \
      com.genexus.schema-version="1.0" \
      com.genexus.maintainer="Seba Gómez <twitter.com/sebagomez>"

RUN mkdir twit

WORKDIR twit

COPY https://github.com/sebagomez/shelltwit/archive/s7.zip .

ENTRYPOINT ["dotnet", "Sebagomez.Shelltwit.dll"] 

