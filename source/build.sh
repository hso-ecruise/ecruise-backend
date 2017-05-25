#!/usr/bin/env bash
cd ecruise.Api

dotnet restore && dotnet build -c Release
