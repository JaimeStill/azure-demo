#!/bin/bash

export rg=arma-demo-rg
export kv=arma-demo-vault
export acr=armaregistry
export appPlan=demo-app-plan
export api1=jps-primary-api
export api2=jps-secondary-api
export hook1=deployprimary
export hook2=deploysecondary
export adApiApp=demo-ad-api
export adSpaApp=demo-ad-spa
export readId=$(uuidgen)
export readWriteId=$(uuidgen)
export appReadId=$(uuidgen)
export appReadWriteId=$(uuidgen)

export tenantId=$(az account show \
    --query "tenantId" \
    --output tsv)