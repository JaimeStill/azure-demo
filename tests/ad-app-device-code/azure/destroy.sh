#!/bin/bash

rg=dev-code-rg
adApp=devcode-ad-api

az group delete -n $rg -y

adAppId=$(az ad app list \
    --query "[?displayName == \`$adApp\`].appId | [0]" \
    --output tsv \
| tr -d '\r')

az ad app delete --id $adAppId