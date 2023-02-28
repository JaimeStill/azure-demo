#!/bin/bash

# initialize variables
. init-variables.sh

# resource group
az group create \
    --name $rg \
    --location eastus

# acr
az acr create \
    --resource-group $rg \
    --name $acr \
    --sku Standard \
    --admin-enabled true

# capture acr admin creds
ACR_ADMIN=$(az acr credential show \
    --name $acr \
    --query "username" \
    --output tsv \
| tr -d '\r')

ACR_PW=$(az acr credential show \
    --name $acr \
    --query "passwords[0].value" \
    --output tsv \
| tr -d '\r')

# build docker image
az acr build \
    --registry $acr \
    --image $api \
    ../$api

# app plan
az appservice plan create \
    --name $appPlan \
    --resource-group $rg \
    --sku F1 \
    --is-linux

# app service
az webapp create \
    --name $api \
    --plan $appPlan \
    --resource-group $rg \
    --docker-registry-server-user $ACR_ADMIN \
    --docker-registry-server-password $ACR_PW \
    --deployment-container-image-name $acr.azurecr.io/$api:latest

# configure continuous deployment
az webapp deployment container config \
    --resource-group $rg \
    --name $api \
    --enable-cd true

# configure acr cd webhook
WEBHOOK=$(az webapp deployment container show-cd-url \
    --resource-group $rg \
    --name $api \
    --query "CI_CD_URL" \
    --output tsv \
| tr -d '\r')

az acr webhook create \
    --resource-group $rg \
    --registry $acr \
    --name $hook \
    --uri $WEBHOOK \
    --actions push