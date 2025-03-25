@echo off
setlocal

dotnet build src/Amethyst.Server.csproj -o bin/ -c Release
if %ERRORLEVEL% neq 0 exit /b %ERRORLEVEL%

dotnet bin/Amethyst.Server.dll %*