#!/bin/bash

if ! dotnet build src/Amethyst.Server.csproj -o bin/ -c Release; then
    exit 1
fi

exec dotnet bin/Amethyst.Server.dll "$@"