#!/usr/bin/env pwsh

Push-Location $PSScriptRoot
try 
{
    # Build Prototest.
    dotnet build -c Release
    if ($LASTEXITCODE -ne 0) 
    {
        exit $LASTEXITCODE
    }

    # Test running the example directly.
    dotnet .\Prototest.Example\bin\Release\netcoreapp2.1\Prototest.Example.dll -c Functional
    if ($LASTEXITCODE -ne 0) 
    {
        exit $LASTEXITCODE
    }

    # Test running the example via dotnet test.
    dotnet test -c Release .\Prototest.Example\Prototest.Example.csproj
    if ($LASTEXITCODE -ne 0) 
    {
        exit $LASTEXITCODE
    }

    # The package version to use.
    $PackageVersion=$env:APPVEYOR_BUILD_VERSION
    if ("$PackageVersion" -eq "") 
    {
        $PackageVersion="1.6.3-DEV"
    }

    # Create the NuGet packages.
    dotnet pack -c Release --no-build --no-restore /p:NuspecProperties=version=$PackageVersion /p:NuspecFile=..\Prototest.nuspec .\Prototest.Library\Prototest.Library.csproj
    if ($LASTEXITCODE -ne 0) 
    {
        exit $LASTEXITCODE
    }
    dotnet pack -c Release --no-build --no-restore /p:NuspecProperties=version=$PackageVersion /p:NuspecFile=..\Prototest.Runtime.nuspec .\Prototest.Library\Prototest.Library.csproj
    if ($LASTEXITCODE -ne 0) 
    {
        exit $LASTEXITCODE
    }
    dotnet pack -c Release --no-build --no-restore /p:NuspecProperties=version=$PackageVersion /p:NuspecFile=..\Prototest.EntryPoint.nuspec .\Prototest.Library\Prototest.Library.csproj
    if ($LASTEXITCODE -ne 0) 
    {
        exit $LASTEXITCODE
    }
    Move-Item -Force .\Prototest.Library\bin\Release\Prototest.*.nupkg .\
} 
finally 
{
    Pop-Location
}