Write-Output "Building and installing Nanopass# tool globally"

dotnet restore ./src/NanopassSharp.Cli
dotnet build ./src/NanopassSharp.Cli --no-restore
dotnet pack ./src/NanopassSharp.Cli --no-build
dotnet tool install --global --add-source ./src/NanopassSharp.Cli/bin/package NanopassSharp.Cli --prerelease
