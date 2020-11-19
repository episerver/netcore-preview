FROM mcr.microsoft.com/dotnet/core/aspnet:5.0.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
FROM mcr.microsoft.com/dotnet/core/sdk:5.0-buster-slim AS build
WORKDIR /src

COPY . .
#RUN dotnet restore
#RUN dotnet build -c Release -o /app/build
COPY ./app/publish /app/publish
COPY ./docker/build-script/wait_sqlserver_start_and_attachdb.sh /app/publish/wait_sqlserver_start_and_attachdb.sh

#FROM build AS publish
#RUN dotnet publish "AlloyMvcTemplates.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
#wait sql server container start and attach alloy database then start web
ENTRYPOINT ./wait_sqlserver_start_and_attachdb.sh