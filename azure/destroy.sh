#!/bin/bash

rg=arma-demo-rg
kv=arma-demo-vault
adApiApp=demo-ad-api
adSpaApp=demo-ad-spa

az group delete -n $rg -y

az keyvault purge --name $kv

azAdAppId=$(az ad app list \
    --query "[?displayName == \`$adApiApp\`].appId | [0]" \
    --output tsv \
| tr -d '\r')

azAdSpaId=$(az ad app list \
    --query "[?displayName == \`$adSpaApp\`].appId | [0]" \
    --output tsv \
| tr -d '\r')

az ad app delete --id $azAdAppId
az ad app delete --id $azAdSpaId