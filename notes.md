# Notes

Demonstrate:

* [Containerized App Service](https://github.com/JaimeStill/learning-azure/tree/main/exercises/app-service/02-app-service-containerized)
* [Key Vault](https://github.com/JaimeStill/learning-azure/tree/main/exercises/key-vault/01-manage-secrets)
* [CLI to API](https://github.com/JaimeStill/learning-azure/tree/main/exercises/azure-ad/02-msal-protect-web-api)
* [SPA to API](https://github.com/JaimeStill/learning-azure/tree/main/exercises/azure-ad/04-spa-to-api)

```bash
# build the image
docker build -t {tag} .

# list images
docker images

# run an image on port 8080
docker run -d -p 8080:80 {image}

# show all containers
docker ps -a

# stop a container
docker stop {container}

# remove a container
docker rm {container}

# remove image
docker rmi {image}
```

## References

* [Microsoft Identity Web APIs Wiki](https://github.com/AzureAD/microsoft-identity-web/wiki/web-apis)
* [Microsoft.Identity.Web ClaimsPrincipalExtensions](https://learn.microsoft.com/en-us/dotnet/api/microsoft.identity.web.claimsprincipalextensions?view=msal-model-dotnet-latest)