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
    --sign-in-audience AzureADMyOrg \
    --public-client-redirect-uris http://localhost \
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
read=$(echo '{
    "adminConsentDescription": "Allow the API to read signed-in users data.",
    "adminConsentDisplayName": "Read data.",
    "id": "'$readId'",
    "isEnabled": true,
    "type": "User",
    "userConsentDescription": "Allow the API to read data on your behalf.",
    "userConsentDisplayName": "Read data as yourself.",
    "value": "Demo.Read"
}')

readWrite=$(echo '{
    "adminConsentDescription": "Allow the API to read and write signed-in users data.",
    "adminConsentDisplayName": "Read and Write data.",
    "id": "'$readWriteId'",
    "isEnabled": true,
    "type": "User",
    "userConsentDescription": "Allow the API to read and write data on your behalf.",
    "userConsentDisplayName": "Read and Write data as yourself.",
    "value": "Demo.ReadWrite"
}')

api=$(echo '"api": {
    "oauth2PermissionScopes": [
        '$read',
        '$readWrite'
    ]
}')

# Configure API App Roles
appRead=$(echo '{
    "allowedMemberTypes": [
        "Application"
    ],
    "description": "Allow this application to read every users data.",
    "displayName": "Demo.Read.All",
    "id": "'$appReadId'",
    "isEnabled": true,
    "origin": "Application",
    "value": "Demo.Read.All"
}')

appReadWrite=$(echo '{
    "allowedMemberTypes": [
        "Application"
    ],
    "description": "Allow this application to read and write every users data.",
    "displayName": "Demo.ReadWrite.All",
    "id": "'$appReadWriteId'",
    "isEnabled": true,
    "origin": "Application",
    "value": "Demo.ReadWrite.All"
}')

appRoles=$(echo '"appRoles": [
    '$appRead',
    '$appReadWrite'
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
        "Domain": "qualified.domain.name",
        "ClientId": "'$apiAppId'",
        "TenantId": "'$tenantId'"
    },
    "VaultName": "'$kv'"
}' ../src/$api1/appsettings.json > "tmp" && mv "tmp" ../src/$api1/appsettings.json

jq '. += {
    "AzureAd": {
        "Instance": "https://login.microsoftonline.com/",
        "Domain": "qualified.domain.name",
        "ClientId": "'$apiAppId'",
        "TenantId": "'$tenantId'"
    },
    "VaultName": "'$kv'"
}' ../src/$api2/appsettings.json > "tmp" && mv "tmp" ../src/$api2/appsettings.json

jq '.AuthSettings += {
    "ClientId": "'$apiAppId'",
    "TenantId": "'$tenantId'",
    "Scopes": [
        "api://'$apiAppId'/Demo.Read"
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
                "id": "'$appReadId'",
                "type": "Scope"
            },
            {
                "id": "'$appReadWriteId'",
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
    --image $api1 \
    ../src/$api1

az acr build \
    --registry $acr \
    --image $api2 \
    ../src/$api2

# App Plan
az appservice plan create \
    --name $appPlan \
    --resource-group $rg \
    --sku F1 \
    --is-linux

## App Services
az webapp create \
    --name $api1 \
    --plan $appPlan \
    --resource-group $rg \
    --docker-registry-server-user $ACR_ADMIN \
    --docker-registry-server-password $ACR_PW \
    --deployment-container-image-name $acr.azurecr.io/$api1:latest

az webapp create \
    --name $api2 \
    --plan $appPlan \
    --resource-group $rg \
    --docker-registry-server-user $ACR_ADMIN \
    --docker-registry-server-password $ACR_PW \
    --deployment-container-image-name $acr.azurecr.io/$api2:latest

# Configure Continuous Deployment
az webapp deployment container config \
    --resource-group $rg \
    --name $api1 \
    --enable-cd true

az webapp deployment container config \
    --resource-group $rg \
    --name $api2 \
    --enable-cd true

# Configure ACR CD Webhooks
PRIMARY_WEBHOOK=$(az webapp deployment container show-cd-url \
    --resource-group $rg \
    --name $api1 \
    --query "CI_CD_URL" \
    --output tsv \
 | tr -d '\r')

SECONDARY_WEBHOOK=$(az webapp deployment container show-cd-url \
    --resource-group $rg \
    --name $api2 \
    --query "CI_CD_URL" \
    --output tsv \
 | tr -d '\r')

az acr webhook create \
    --resource-group $rg \
    --registry $acr \
    --name $hook1 \
    --uri $PRIMARY_WEBHOOK \
    --actions push

az acr webhook create \
    --resource-group $rg \
    --registry $acr \
    --name $hook2 \
    --uri $SECONDARY_WEBHOOK \
    --actions push

# Add Key Vault to App Service settings

az webapp config appsettings set \
    --resource-group $rg \
    --name $api1 \
    --settings "VaultName=$kv"

az webapp config appsettings set \
    --resource-group $rg \
    --name $api2 \
    --settings "VaultName=$kv"

# Enable Web App Managed Identity
az webapp identity assign \
    --resource-group $rg \
    --name $api1

az webapp identity assign \
    --resource-group $rg \
    --name $api2

# Grant Key Vault Access
# tr -d '\r' required for WSL use
# results were coming back with a ghost \r
# the resulting set-policy calls would error
# with an invalid ObjectId message
PRIMARY_PRINCIPAL=$(az webapp identity show \
    --resource-group $rg \
    --name $api1 \
    --query principalId \
    --output tsv \
| tr -d '\r')

SECONDARY_PRINCIPAL=$(az webapp identity show \
    --resource-group $rg \
    --name $api2 \
    --query principalId \
    --output tsv \
| tr -d '\r')

az keyvault set-policy \
    --name $kv \
    --secret-permissions get list \
    --object-id $PRIMARY_PRINCIPAL

az keyvault set-policy \
    --name $kv \
    --secret-permissions get list \
    --object-id $SECONDARY_PRINCIPAL