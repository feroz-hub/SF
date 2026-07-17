param(
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"

dotnet restore ./Zentra.sln
dotnet build ./Zentra.sln -c $Configuration --no-restore
dotnet test ./tests/Zentra.IntegrationTests/IntegrationTests.csproj -c $Configuration --no-build
