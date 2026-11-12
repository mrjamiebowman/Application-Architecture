Clear-Host

# api
helm upgrade --install mrjb-api charts/mrjb-app-api `
  --namespace appdev `
  --set image.tag=latest
