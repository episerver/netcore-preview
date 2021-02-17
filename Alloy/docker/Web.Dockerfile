FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 8000

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY . .
RUN dotnet restore "AlloyMvcTemplates.csproj"

RUN dotnet build "AlloyMvcTemplates.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AlloyMvcTemplates.csproj" -c Release -o /app/publish
COPY ./docker/build-script/wait_sqlserver_start_and_attachdb.sh /app/publish/wait_sqlserver_start_and_attachdb.sh
COPY ./App_Data/DefaultSiteContent.episerverdata /app/publish/App_Data/DefaultSiteContent.episerverdata

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
#wait sql server container start and attach alloy database then start web
ENTRYPOINT ./wait_sqlserver_start_and_attachdb.sh
