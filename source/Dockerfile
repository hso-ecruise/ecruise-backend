FROM microsoft/dotnet:latest

COPY . /app

WORKDIR /app/ecruise.Api
 
RUN ["dotnet", "restore"]
RUN ["dotnet", "build"]

ENTRYPOINT ["dotnet", "run"]