sleep 30s
/opt/mssql-tools/bin/sqlcmd -S . -U sa -P ${SA_PASSWORD} \
-Q "CREATE DATABASE [${DATABASE_NAME}] ON (FILENAME ='/docker/Alloy.mdf') FOR ATTACH"