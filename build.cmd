SET PROJ=%~dp0src\Problem\Problem.csproj 
dotnet restore
dotnet build 
dotnet pack %PROJ% -c release -o %~dp0artifacts