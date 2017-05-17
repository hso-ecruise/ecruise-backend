FROM microsoft/dotnet:latest

COPY . /app

WORKDIR /app/ecruise.Api
 
RUN dotnet restore
RUN dotnet build -c Release

ENTRYPOINT ["dotnet", "bin/Release/netcoreapp1.1/ecruise.Api.dll"]