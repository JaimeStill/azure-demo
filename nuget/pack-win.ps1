param(
    [string]
    [Parameter()]
    $Source = "$env:userprofile\packages\demo\",
    [string]
    [Parameter()]
    $SourceName = "Demo Packages"
)

$Path = "..\src\packages\"
$Package = "Arma.Demo.Core"
$Projects = @("..\src\jps-primary-api", "..\src\jps-secondary-api");

& dotnet nuget locals all --clear

if (!(Test-Path $Source)) {
    New-Item $Source -ItemType Directory
} else {
    if (Test-Path (Join-Path $Source "$Package.*.nupkg")) {
        Remove-Item (Join-Path $Source "$Package.*.nupkg") -Force -Recurse
    }
}

if (!(& dotnet nuget list source | Where-Object { $_ -like "*$SourceName*" })) {
    & dotnet nuget add source $Source -n $SourceName
}

& dotnet pack $Path -o $Source

foreach ($project in $Projects) {
    & dotnet restore $project -f
}