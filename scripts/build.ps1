param(
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"

dotnet restore ./HCL.CS.sln
dotnet build ./HCL.CS.sln -c $Configuration --no-restore
dotnet test ./tests/HCL.CS.IntegrationTests/IntegrationTests.csproj -c $Configuration --no-build
