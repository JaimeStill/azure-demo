#!/bin/bash

# init variables
. init-variables.sh

# resource group
az group create \
    --name $rg \
    --location eastus

# ad api app registration
appId=$(az ad app create \
    --display-name $adApp \
    --public-client-redirect-uris https://login.microsoftonline.com/common/oauth2/nativeclient \
    --is-fallback-public-client true \
    --query appId \
    --output tsv \
| tr -d '\r')

objectId=$(az ad app show \
    --id $appId \
    --query id \
    --output tsv \
| tr -d '\r')

# configure delegated OAuth permissions
read=$(echo '{
    "adminConsentDescription": "Allow the API to read signed-in users data.",
    "adminConsentDisplayName": "Read data.",
    "id": "'$readId'",
    "isEnabled": true,
    "type": "User",
    "userConsentDescription": "Allow the API to read data on your behalf.",
    "userConsentDisplayName": "Read data as yourself.",
    "value": "app_read"
}')

api=$(echo '"api": {
    "oauth2PermissionScopes": [
        '$read'
    ]
}')

# configure identifier URI
identifierUris=$(echo '"identifierUris": [
    "api://'$appId'"
]')

# compose request body and update app registration
apiBody=$(echo '{
    '$api',
    '$identifierUris'
}' | jq .)

az rest \
    --method PATCH \
    --uri https://graph.microsoft.com/v1.0/applications/$objectId/ \
    --headers 'Content-Type=application/json' \
    --body "$apiBody"

# add appsettings in API and CLI

jq '.AzureAd += {
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "qualified.domain.name",
    "ClientId": "'$appId'",
    "TenantId": "'$tenant'"
}' ../$webApi/appsettings.json > "tmp" && mv "tmp" ../$webApi/appsettings.json

jq '.AuthSettings += {
    "ClientId": "'$appId'",
    "TenantId": "'$tenant'",
    "Scopes": [
        "api://'$appId'/app_read"
    ]
}' ../$cli/appsettings.json > "tmp" && mv "tmp" ../$cli/appsettings.json

# acr
az acr create \
    --resource-group $rg \
    --name $acr \
    --sku Standard \
    --admin-enabled true

# acr admin name
ACR_ADMIN=$(az acr credential show \
    --name $acr \
    --query "username" \
    --output tsv \
| tr -d '\r')

# acr admin pw
ACR_PW=$(az acr credential show \
    --name $acr \
    --query "passwords[0].value" \
    --output tsv \
| tr -d '\r')

# build image
az acr build \
    --registry $acr \
    --image $webApi \
    ../$webApi

# app plan
az appservice plan create \
    --name $appPlan \
    --resource-group $rg \
    --sku F1 \
    --is-linux

## app services
az webapp create \
    --name $webApi \
    --plan $appPlan \
    --resource-group $rg \
    --docker-registry-server-user $ACR_ADMIN \
    --docker-registry-server-password $ACR_PW \
    --deployment-container-image-name $acr.azurecr.io/$webApi:latest

# configure continuous deployment
az webapp deployment container config \
    --resource-group $rg \
    --name $webApi \
    --enable-cd true

# configure cd webhook
WEBHOOK=$(az webapp deployment container show-cd-url \
    --resource-group $rg \
    --name $webApi \
    --query "CI_CD_URL" \
    --output tsv \
 | tr -d '\r')

 az acr webhook create \
    --resource-group $rg \
    --registry $acr \
    --name $hook \
    --uri $WEBHOOK \
    --actions push