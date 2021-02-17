FROM mcr.microsoft.com/mssql/server:2019-latest AS base

USER root

ENV ACCEPT_EULA=Y
ENV MSSQL_TCP_PORT=1433
EXPOSE 1433

WORKDIR /src
COPY ./docker/build-script/attach_db.sh /docker/attach_db.sh
COPY ./App_Data/Alloy.mdf /docker/Alloy.mdf

RUN chmod -R 777 /docker/.

ENTRYPOINT /docker/attach_db.sh & /opt/mssql/bin/sqlservr
