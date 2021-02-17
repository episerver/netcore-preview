FROM mcr.microsoft.com/mssql/server:2019-latest AS base

USER root

ENV ACCEPT_EULA=Y
ENV MSSQL_TCP_PORT=1433
EXPOSE 1433

WORKDIR /src
COPY ./docker/build-script/SetupDatabases.sh /docker/SetupDatabases.sh
COPY ./EPiServer.Reference.Commerce.Site/packages/episerver.cms.core /packages/episerver.cms.core
COPY ./EPiServer.Reference.Commerce.Site/packages/episerver.commerce.core /packages/episerver.commerce.core
COPY ./EPiServer.Reference.Commerce.Site/packages/episerver.personalization.commerce /packages/episerver.personalization.commerce
RUN chmod -R 777 /docker/.

ENTRYPOINT /docker/SetupDatabases.sh & /opt/mssql/bin/sqlservr