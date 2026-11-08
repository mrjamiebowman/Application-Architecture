# syntax=docker/dockerfile:1

# SQL Server 2025 (Developer edition) that creates the AppArch database and applies the
# schema scripts from MrJB.Sample.Data/Scripts on every start (all scripts are idempotent).
#
# Build through docker compose (repo root), which supplies the "scripts" build context:
#   docker compose up -d --build mssql
#
# Or standalone, from the repo root:
#   docker build -t apparch-mssql:2025 --build-context scripts=projects/api/src/MrJB.Sample.Data/Scripts .docker/mssql

FROM mcr.microsoft.com/mssql/server:2025-latest

USER root

# init scripts run in name order: 00 creates the database, the rest run inside it.
# add new schema scripts here with a number that puts them after the objects they depend on.
COPY init/ /opt/mssql-init/
COPY --from=scripts Customers.sql          /opt/mssql-init/10-Customers.sql
COPY --from=scripts uspCustomerGetById.sql /opt/mssql-init/20-uspCustomerGetById.sql

COPY entrypoint.sh /usr/local/bin/mssql-init-entrypoint.sh

# strip CRLF in case the repo was checked out on Windows with autocrlf
RUN sed -i 's/\r$//' /usr/local/bin/mssql-init-entrypoint.sh \
    && chmod 755 /usr/local/bin/mssql-init-entrypoint.sh \
    && chown -R mssql /opt/mssql-init

USER mssql

ENV ACCEPT_EULA=Y \
    MSSQL_PID=Developer \
    MSSQL_DATABASE=AppArch

EXPOSE 1433

# the base image's ENTRYPOINT (permissions_check.sh) still runs and execs this
CMD ["/usr/local/bin/mssql-init-entrypoint.sh"]
