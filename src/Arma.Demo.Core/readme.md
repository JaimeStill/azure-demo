# Arma.Demo.Core

## NuGet

```bash
# pack
dotnet pack -c Release -o ./.nuget/

# push
dotnet nuget push \
    ./.nuget/Arma.Demo.Core.{version}.nupkg \
    --api-key {key} \
    --source https://api.nuget.org/v3/index.json
```

## GitHub Actions

