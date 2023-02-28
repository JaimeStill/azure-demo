# Notes

Issues:

* [Resolve failed local Docker runs](https://kevinle.medium.com/securely-store-and-access-secrets-in-azure-keyvault-from-docker-based-app-service-babe463fe57b)

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