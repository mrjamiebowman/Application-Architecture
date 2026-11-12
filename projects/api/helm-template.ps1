Clear-Host

# template
helm template mrjb-api charts/mrjb-app-api `
  --namespace appdev `
  --set image.tag=latest

  