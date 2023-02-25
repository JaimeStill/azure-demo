# Azure Demo

Demonstrates the ability to quickly generate standardized infrastructure to facilitate development of microservice modules and shared packages.

## NuGet

```bash
# pack
dotnet pack -c Release -o ./.nuget/

# push
dotnet nuget push \
    ./.nuget/Arma.Demo.Core.{version}.nupkg \
    --api-key {key} \
    --source https://api.nuget.org/v3/index.json \
    --skip-duplicate
```

Deployment to NuGet is automated through the [arma-demo-core-release.yml](./.github/workflows/arma-demo-core-release.yml) workflow. To initiate an updated version, create a new [GitHub Tag](https://github.com/JaimeStill/azure-demo/tags) in the format of `v[0-9].[0-9].[0-9]`. This should be done after the PR is approved.

## Azure CLI Scripts

Make sure when writing `.sh` files, to use the *End of Line Sequence* **LF** vs. **CRLF**. Otherwise, weird things will happen trying to execute scripts in WSL.