#!/bin/bash

# Initialize variables
. init-variables.sh

# Resource Group
az group create \
    --name $rg \
    --location eastus

# Key Vault
az keyvault create \
    --resource-group $rg \
    --location eastus \
    --name $kv

az keyvault secret set \
    --name DatabaseConnection \
    --value 'Server=neural.local;database=neural-db;UID=sa;PWD=P@$$Word1234!@#$;' \
    --vault-name $kv

# Configure AD API App Registration
apiAppId=$(az ad app create \
    --display-name $adApiApp \
    --public-client-redirect-uris https://login.microsoftonline.com/common/oauth2/nativeclient \
    --is-fallback-public-client true \
    --query appId \
    --output tsv \
| tr -d '\r')

apiObjectId=$(az ad app show \
    --id $apiAppId \
    --query id \
    --output tsv \
| tr -d '\r')

# Configure delegated OAuth permissions
access=$(echo '{
    "adminConsentDescription": "Allow the API to access signed-in users data.",
    "adminConsentDisplayName": "Access data.",
    "id": "'$accessId'",
    "isEnabled": true,
    "type": "User",
    "userConsentDescription": "Allow the API to access data on your behalf.",
    "userConsentDisplayName": "Access data as yourself.",
    "value": "demo_access"
}')

api=$(echo '"api": {
    "oauth2PermissionScopes": [
        '$access'
    ]
}')

# Configure API App Roles
appAccess=$(echo '{
    "allowedMemberTypes": [
        "Application"
    ],
    "description": "Allow this application to access every users data.",
    "displayName": "demo_app_access",
    "id": "'$appAccessId'",
    "isEnabled": true,
    "origin": "Application",
    "value": "demo_app_access"
}')

appRoles=$(echo '"appRoles": [
    '$appAccess'
]')

# Configure API App Identifier URI
identifierUris=$(echo '"identifierUris": [
    "api://'$apiAppId'"
]')

# Optional Claims
optionalClaims=$(echo '"optionalClaims": {
    "accessToken": [
        {
            "additionalProperties": [],
            "essential": false,
            "name": "idtyp",
            "source": null
        }
    ]
}')

# Compose API App Configuration and update App Registration
apiBody=$(echo '{
    '$api',
    '$appRoles',
    '$identifierUris',
    '$optionalClaims'
}' | jq .)

az rest \
    --method PATCH \
    --uri https://graph.microsoft.com/v1.0/applications/$apiObjectId/ \
    --headers 'Content-Type=application/json' \
    --body "$apiBody"

# Add appsettings in APIs and CLI
jq '. += {
    "AzureAd": {
        "Instance": "https://login.microsoftonline.com/",
        "Domain": "'$coreApi'.azurewebsites.net",
        "ClientId": "'$apiAppId'",
        "TenantId": "'$tenant'"
    },
    "Scopes": "demo_access",
    "VaultName": "'$kv'"
}' ../src/$coreApi/appsettings.json > "tmp" && mv "tmp" ../src/$coreApi/appsettings.json

jq '.AuthSettings += {
    "ClientId": "'$apiAppId'",
    "TenantId": "'$tenant'",
    "Scopes": [
        "api://'$apiAppId'/demo_access"
    ]
}' ../src/$cli/appsettings.json > "tmp" && mv "tmp" ../src/$cli/appsettings.json

# Configure AD SPA App Registration
spaAppId=$(az ad app create \
    --display-name $adSpaApp \
    --sign-in-audience AzureADMyOrg \
    --query appId \
    --output tsv \
| tr -d '\r')

spaObjectId=$(az ad app show \
    --id $spaAppId \
    --query id \
    --output tsv \
| tr -d '\r')

# Configure API Permissions
requiredResourceAccess=$(echo '"requiredResourceAccess": [
    {
        "resourceAccess": [
            {
                "id": "'$appAccessId'",
                "type": "Scope"
            }
        ],
        "resourceAppId": "'$apiAppId'"
    }
]')

# Configure Redirect URIs
spa=$(echo '"spa": {
    "redirectUris": [
        "http://localhost:4200/",
        "http://localhost:4200/auth"
    ]
}')

# Compose API App Configuration and update App Registration
spaBody=$(echo '{
    '$requiredResourceAccess',
    '$spa'
}' | jq .)

az rest \
    --method PATCH \
    --uri https://graph.microsoft.com/v1.0/applications/$spaObjectId/ \
    --headers 'Content-Type=application/json' \
    --body "$spaBody"

# ACR
az acr create \
    --resource-group $rg \
    --name $acr \
    --sku Standard \
    --admin-enabled true

# Admin Details in:
# Container Registries -> Registry -> Access keys

# Set ACR Admin
ACR_ADMIN=$(az acr credential show \
    --name $acr \
    --query "username" \
    --output tsv \
| tr -d '\r')

# Set ACR Password
ACR_PW=$(az acr credential show \
    --name $acr \
    --query "passwords[0].value" \
    --output tsv \
| tr -d '\r')

# Build Docker images with ACR
az acr build \
    --registry $acr \
    --image $sync \
    ../src/$sync

az acr build \
    --registry $acr \
    --image $processor \
    ../src/$processor

az acr build \
    --registry $acr \
    --image $coreApi \
    ../src/$coreApi

# App Plan
az appservice plan create \
    --name $appPlan \
    --resource-group $rg \
    --sku F1 \
    --is-linux

## App Services
az webapp create \
    --name $sync \
    --plan $appPlan \
    --resource-group $rg \
    --docker-registry-server-user $ACR_ADMIN \
    --docker-registry-server-password $ACR_PW \
    --deployment-container-image-name $acr.azurecr.io/$sync:latest

az webapp create \
    --name $processor \
    --plan $appPlan \
    --resource-group $rg \
    --docker-registry-server-user $ACR_ADMIN \
    --docker-registry-server-password $ACR_PW \
    --deployment-container-image-name $acr.azurecr.io/$processor:latest

az webapp create \
    --name $coreApi \
    --plan $appPlan \
    --resource-group $rg \
    --docker-registry-server-user $ACR_ADMIN \
    --docker-registry-server-password $ACR_PW \
    --deployment-container-image-name $acr.azurecr.io/$coreApi:latest

# Configure Continuous Deployment
az webapp deployment container config \
    --resource-group $rg \
    --name $sync \
    --enable-cd true

az webapp deployment container config \
    --resource-group $rg \
    --name $processor \
    --enable-cd true

az webapp deployment container config \
    --resource-group $rg \
    --name $coreApi \
    --enable-cd true

# Configure ACR CD Webhooks
SYNC_WEBHOOK=$(az webapp deployment container show-cd-url \
    --resource-group $rg \
    --name $sync \
    --query "CI_CD_URL" \
    --output tsv \
| tr -d '\r')

PROCESSOR_WEBHOOK=$(az webapp deployment container show-cd-url \
    --resource-group $rg \
    --name $processor \
    --query "CI_CD_URL" \
    --output tsv \
| tr -d '\r')

CORE_WEBHOOK=$(az webapp deployment container show-cd-url \
    --resource-group $rg \
    --name $coreApi \
    --query "CI_CD_URL" \
    --output tsv \
| tr -d '\r')

az acr webhook create \
    --resource-group $rg \
    --registry $acr \
    --name $syncHook \
    --uri $SYNC_WEBHOOK \
    --actions push

az acr webhook create \
    --resource-group $rg \
    --registry $acr \
    --name $processorHook \
    --uri $PROCESSOR_WEBHOOK \
    --actions push

az acr webhook create \
    --resource-group $rg \
    --registry $acr \
    --name $coreHook \
    --uri $CORE_WEBHOOK \
    --actions push

# Add Key Vault to App Service settings

az webapp config appsettings set \
    --resource-group $rg \
    --name $coreApi \
    --settings "VaultName=$kv"

# Enable Web App Managed Identity
az webapp identity assign \
    --resource-group $rg \
    --name $coreApi

# Grant Key Vault Access
# tr -d '\r' required for WSL use
# results were coming back with a ghost \r
# the resulting set-policy calls would error
# with an invalid ObjectId message
CORE_PRINCIPAL=$(az webapp identity show \
    --resource-group $rg \
    --name $coreApi \
    --query principalId \
    --output tsv \
| tr -d '\r')

az keyvault set-policy \
    --name $kv \
    --secret-permissions get list \
    --object-id $CORE_PRINCIPAL