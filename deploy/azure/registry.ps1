$registry = "crtshapess.azurecr.io"
$apps = ("basket.api", "catalog.api", "coupon.api", "identity.api", "mobileshoppingagg", "ordering.api", "ordering.backgroundtasks", "ordering.signalrhub", "payment.api", "webmvc", "webshoppingagg", "webspa", "webstatus", "webhooks.api", "webhooks.client")

if ($apps) {
    foreach ($app in $apps) {
        Write-Host "Tag and push: $app" -ForegroundColor Green
        docker tag eshop/${app}:linux-latest ${registry}/eshop/${app}:linux-latest   
        docker push ${registry}/eshop/${app}:linux-latest
    }
}
