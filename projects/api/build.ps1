Clear-Host

$TAG = "latest"

# --no-cache
docker build -f ./src/MrJB.API.Sample/Dockerfile -t appdev/mrjb-api:$TAG .
