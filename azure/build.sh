# Initialized Variables
rg=arma-demo-rg
kv=arma-demo-vault
acr=armaregistry
ad-app=demo-api
scopeId=$(uuidgen)

# Resource Group
az group create \
    --name $rg \
    --location eastus

# Key Vault
az keyvault create \
    --resource-group $rg \
    --location eastus \
    --name $kv

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

# Add AzureAd config to appsettings
jq '.AzureAd += {
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "qualified.domain.name",
    "ClientId": "$appId",
    "TenantId": "common"
}' ../src/demo/demo-api/appsettings.json > "tmp" && mv "tmp" ../src/demo/demo-api/appsettings.json