# Notes

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