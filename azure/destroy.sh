#!/bin/bash

rg=arma-demo-rg
kv=arma-demo-vault

az group delete -n $rg -y

az keyvault purge --name $kv