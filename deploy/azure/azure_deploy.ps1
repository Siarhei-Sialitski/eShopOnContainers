$rg = "rgtshapessdev"
$acr = "acrtshapessdev"
$acrUser = ""
$acrPasswprd = ""
$aks = "akstshapessdev"
$spnName = "spntshapessdev" # see below if you have an existing SPN

# create RG
az group create -n $rg --location southcentralus

# create ACR
az acr create -g $rg -n $acr --sku Basic --admin-enabled true
$acrId = $(az acr show -g $rg -n $acr --query "id" -o tsv)

# assign push/pull role to SPN
spnPassword=$(az ad sp create-for-rbac --name http://$spnName --scopes $acrId --role acrpush --query password --output tsv)
spnId=$(az ad sp list --display-name http://$spnName --query [0].appId  --output tsv) # Ref : https://github.com/Azure/azure-cli/issues/19179

# for an existing SPN
# export spnId="<id of an existing service principle>"
# az role assignment create --assignee $spnId --role acrpush --scope $acrId

# create AKS cluster
az aks create -g $rg -n $aks --node-count 1 --enable-addons monitoring, http_application_routing --enable-azure-rbac --enable-aad --generate-ssh-keys --attach-acr $acr

# set the k8s context locally
az aks get-credentials -g $rg -n $aks
$AKS_ID = $(az aks show -g $rg -n $aks --query id -o tsv)
az role assignment create --role "Azure Kubernetes Service RBAC Cluster Admin" --assignee siarhei_sialitski@epam.com --scope $AKS_ID 
# deploy nginx controller
cd ../deploy/k8s/nginx-ingress
kubectl apply -f mandatory.yaml
kubectl apply -f local-cm.yaml
kubectl apply -f local-svc.yaml

# update nginx controller to allow large heaeders for login
cd -
cd ../deploy/k8s/helm
kubectl apply -f aks-httpaddon-cfg.yaml
kubectl delete pod $(kubectl get pod -l app=addon-http-application-routing-nginx-ingress -n kube-system -o jsonpath="{.items[0].metadata.name}") -n kube-system

cd -

# deploy all from public repos
cd ../deploy/k8s/helm
kubectl create ns eshop
#./deploy-all.sh --dns aks --aks-name $aks --aks-rg $rg -t linux-latest
#.\deploy-all.ps1 -externalDns aks -aksName $aks -aksRg $rg -imageTag linux-dev -useMesh $false
.\deploy-all.ps1 -externalDns aks -aksName $aks -aksRg $rg -imageTag linux-latest -registry "{$acr}.azurecr.io" -dockerUser $acrUser -dockerPassword $acrPassword -useMesh $false

# fix versions for apigwms (envoy)
$domain = "$(az aks show -n $aks -g $rg --query addonProfiles.httpApplicationRouting.config.HTTPApplicationRoutingZoneName -o tsv)"
$dns = "$aks.$domain"

helm uninstall eshop-apigwms -n eshop
helm install "eshop-apigwms" --namespace eshop --set "ingress.hosts={$dns}" --values app.yaml --values inf.yaml --values ingress_values.yaml --set app.name=eshop --set inf.k8s.dns=$dns --set image.pullPolicy=Always apigwms

helm uninstall eshop-apigwws -n eshop
helm install "eshop-apigwws" --namespace eshop --set "ingress.hosts={$dns}" --values app.yaml --values inf.yaml --values ingress_values.yaml --set app.name=eshop --set inf.k8s.dns=$dns --set image.pullPolicy=Always apigwws
