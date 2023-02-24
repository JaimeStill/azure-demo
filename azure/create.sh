# Initialized Variables
rg=arma-demo-rg
kv=arma-demo-vault
acr=armaregistry
ad-app=demo-api
scopeId=$(uuidgen)
api-1=primary-api
api-2=secondary-api


# Resource Group
az group create \
    --name $rg \
    --location eastus

# ACR
az acr create \
    --resource-group $rg \
    --name $acr \
    --sku Standard \
    --admin-enabled true

# Set ACR Admin
ACR_ADMIN=$(az acr credential show \
    --name $acr \
    --query "username" \
    --output tsv)

# Set ACR Password
ACR_PW=$(az acr credential show \
    --name $acr \
    --query "passwords[0].value" \
    --output tsv)

# Build Docker images with ACR

az acr build \
    --registry $acr \
    --image $api-1 \
    ../src/$api-1

az acr build \
    --registry $acr \
    --image $api-2 \
    ../src/$api-2

# App Service

# Key Vault
az keyvault create \
    --resource-group $rg \
    --location eastus \
    --name $kv

az keyvault secret set \
    --name DatabaseConnection \
    --value "Server=nerual.local;database=neural-db;UID=sa;PWD=P@$$Word1234!@#$;" \
    --vault-name $kv

# Configure AD App Registration
appId=$(az ad app create \
    --display-name $ad-app \
    --public-client-redirect-uris http://localhost \
    --is-fallback-public-client true \
    --query appId \
    --output tsv)

# Setup api object with OAuth Scopes
api=$(echo '{
    "acceptMappedClaims": null,
    "knownClientApplications": [],
    "oauth2PermissionScopes": [{
        "adminConsentDescription": "Access Demo API",
        "adminConsentDisplayName": "API Access",
        "id": "'$scopeId'",
        "isEnabled": true,
        "type": "User",
        "userConsentDescription": "Access Demo API",
        "userConsentDisplayName": "API Access",
        "value": "access_as_user"
    }],
    "preAuthorizedApplications": [],
    "requestedAccessTokenVersion": 2
}' | jq .)

# Update app registration
az ad app update \
    --id $appId \
    --identifier-uris api://$appId \
    --set api="$api"

# Add AzureAd config to appsettings in APIs
jq '.AzureAd += {
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "qualified.domain.name",
    "ClientId": "$appId",
    "TenantId": "common"
}' ../src/$api-1/appsettings.json > "tmp" && mv "tmp" ../src/$api-1/appsettings.json

jq '.AzureAd += {
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "qualified.domain.name",
    "ClientId": "$appId",
    "TenantId": "common"
}' ../src/$api-2/appsettings.json > "tmp" && mv "tmp" ../src/$api-2/appsettings.json