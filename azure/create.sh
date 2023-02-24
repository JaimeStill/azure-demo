# Initialize variables
. ./init-variables.sh

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

# Admin Details in:
# Container Registries -> Registry -> Access keys

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
    --deployment-container-image-name $acr.azureacr.io/$api1:latest

az webapp create \
    --name $api2 \
    --plan $appPlan \
    --resource-group $rg \
    --docker-registry-server-user $ACR_ADMIN \
    --docker-registry-server-password $ACR_PW \
    --deployment-container-image-name $acr.azureacr.io/$api2:latest

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
    --output tsv
)

SECONDARY_WEBHOOK=$(az webapp deployment container show-cd-url \
    --resource-group $rg \
    --name $api2 \
    --query "CI_CD_URL" \
    --output tsv
)

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

# Key Vault
az keyvault create \
    --resource-group $rg \
    --location eastus \
    --name $kv

az keyvault secret set \
    --name DatabaseConnection \
    --value 'Server=neural.local;database=neural-db;UID=sa;PWD=P@$$Word1234!@#$;' \
    --vault-name $kv

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

PRIMARY_PRINCIPAL=$(az webapp identity show \
    --resource-group $rg \
    --name $api1 \
    --query principalId \
    --output tsv)

SECONDARY_PRINCIPAL=$(az webapp identity show \
    --resource-group $rg \
    --name $api2 \
    --query principalId \
    --output tsv)

az keyvault set-policy \
    --name $kv \
    --secret-permissions get list \
    --object-id $PRIMARY_PRINCIPAL

az keyvault set-policy \
    --name $kv \
    --secret-permissions get list \
    --object-id $SECONDARY_PRINCIPAL

# Add Key Vault config to appsettings in APIs

jq '. += {
    "VaultName": "'$kv'"
}' ../src/$api1/appsettings.json > "tmp" && mv "tmp" ../src/$api1/appsettings.json

jq '. += {
    "VaultName": "'$kv'"
}' ../src/$api2/appsettings.json > "tmp" && mv "tmp" ../src/$api2/appsettings.json

# Configure AD App Registration
appId=$(az ad app create \
    --display-name $adApp \
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
}' ../src/$api1/appsettings.json > "tmp" && mv "tmp" ../src/$api1/appsettings.json

jq '.AzureAd += {
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "qualified.domain.name",
    "ClientId": "$appId",
    "TenantId": "common"
}' ../src/$api2/appsettings.json > "tmp" && mv "tmp" ../src/$api2/appsettings.json