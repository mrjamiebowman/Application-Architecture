#!/bin/bash
# Starts SQL Server, waits until it accepts connections, creates $MSSQL_DATABASE and runs the
# init scripts, then keeps SQL Server in the foreground. /tmp/mssql-init-complete marks the
# container as ready for the compose health check.
set -euo pipefail

: "${MSSQL_SA_PASSWORD:?MSSQL_SA_PASSWORD must be set}"
: "${MSSQL_DATABASE:=AppArch}"

INIT_DIR=/opt/mssql-init
READY_MARKER=/tmp/mssql-init-complete
rm -f "$READY_MARKER"

# sqlcmd location differs between image versions
SQLCMD="$(command -v sqlcmd || true)"
for candidate in /opt/mssql-tools18/bin/sqlcmd /opt/mssql-tools/bin/sqlcmd; do
    if [ -z "$SQLCMD" ] && [ -x "$candidate" ]; then
        SQLCMD="$candidate"
    fi
done
if [ -z "$SQLCMD" ]; then
    echo "[init] sqlcmd not found in the image" >&2
    exit 1
fi

sql() {
    "$SQLCMD" -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C -b "$@"
}

/opt/mssql/bin/sqlservr &
SQL_PID=$!

# forward docker stop to SQL Server so it shuts down cleanly
trap 'kill -TERM "$SQL_PID"; wait "$SQL_PID"' TERM INT

echo "[init] waiting for SQL Server to accept connections..."
ready=false
for _ in $(seq 1 120); do
    if ! kill -0 "$SQL_PID" 2>/dev/null; then
        echo "[init] SQL Server exited during start-up" >&2
        exit 1
    fi
    if sql -l 2 -Q "SELECT 1" >/dev/null 2>&1; then
        ready=true
        break
    fi
    sleep 1
done
if [ "$ready" != true ]; then
    echo "[init] SQL Server did not become ready in time" >&2
    exit 1
fi

# 00-* scripts run against master (they create the database)
for script in "$INIT_DIR"/00-*.sql; do
    echo "[init] $(basename "$script") (master)"
    sql -d master -v DatabaseName="$MSSQL_DATABASE" -i "$script"
done

# on a restart the database already exists in the volume, but SQL Server accepts logins
# before it has finished recovering user databases, so wait for it to come ONLINE
echo "[init] waiting for $MSSQL_DATABASE to come online..."
online=false
for _ in $(seq 1 120); do
    state="$(sql -d master -h -1 -W -Q "SET NOCOUNT ON; SELECT CONVERT(varchar(60), DATABASEPROPERTYEX(N'$MSSQL_DATABASE', 'Status'));" 2>/dev/null | tr -d '[:space:]' || true)"
    if [ "$state" = "ONLINE" ]; then
        online=true
        break
    fi
    sleep 1
done
if [ "$online" != true ]; then
    echo "[init] $MSSQL_DATABASE did not come online (last status: ${state:-unknown})" >&2
    exit 1
fi

# everything else runs inside the database
for script in "$INIT_DIR"/*.sql; do
    name="$(basename "$script")"
    [[ "$name" == 00-* ]] && continue
    echo "[init] $name ($MSSQL_DATABASE)"
    sql -d "$MSSQL_DATABASE" -i "$script"
done

touch "$READY_MARKER"
echo "[init] database $MSSQL_DATABASE is ready"

wait "$SQL_PID"
