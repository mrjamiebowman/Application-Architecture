Clear-Host

# dos2unix
dos2unix .docker/mssql/entrypoint.sh

# docker compose build
docker-compose build

# display built images
docker images | findstr appdev