#!/bin/bash

rg=arma-demo-rg
kv=arma-demo-vault

az keyvault purge --name $kv

az group delete -n $rg -y