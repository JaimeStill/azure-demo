#!/bin/bash

# example:
# . build-acr \
#   -i jps-core-api \
#   -r armaregistry \
#   -s ../src/jps-core-api

while getopts i:r:s: option
do
    case "${option}"
        in
        i)image=${OPTARG};;
        r)registry=${OPTARG};;
        s)source=${OPTARG};;
    esac
done

echo "Building ACR Image"
echo "image: $image"
echo "registry: $registry"
echo "source: $source"

az acr build \
    --registry $registry \
    --image $image \
    $source