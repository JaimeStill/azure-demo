#!/bin/bash

export rg=arma-demo-rg
export kv=arma-demo-vault
export acr=armaregistry
export appPlan=demo-app-plan
export coreApi=jps-core-api
export processor=jps-processor
export sync=jps-sync
export coreSpa=jps-core-spa
export cli=cli
export apiHook=deploycore
export processorHook=deployprocessor
export syncHook=deploysync
export spaHook=deployspa
export adApiApp=demo-ad-api
export adSpaApp=demo-ad-spa
export tenant=common
export accessId=$(uuidgen)
export appAccessId=$(uuidgen)