FROM microsoft/dotnet:latest

COPY . /app

WORKDIR /app/ecruise.Api
 
RUN ["dotnet", "restore"]
RUN ["dotnet", "build"]

ENV ASPNETCORE_URLS ASPNETCORE_URLS=http://+:5000
 
ENTRYPOINT ["dotnet", "run"]