FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 8000

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY EPiServer.Reference.Commerce.Shared EPiServer.Reference.Commerce.Shared
COPY EPiServer.Reference.Commerce.Site EPiServer.Reference.Commerce.Site
RUN dotnet restore "EPiServer.Reference.Commerce.Shared/EPiServer.Reference.Commerce.Shared.csproj"
RUN dotnet restore "EPiServer.Reference.Commerce.Site/EPiServer.Reference.Commerce.Site.csproj"
RUN dotnet build "EPiServer.Reference.Commerce.Site/EPiServer.Reference.Commerce.Site.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EPiServer.Reference.Commerce.Site/EPiServer.Reference.Commerce.Site.csproj" -c Release -o /app/publish
COPY ./docker/build-script/wait_sqlserver_start_and_attachdb.sh /app/publish/wait_sqlserver_start_and_attachdb.sh
COPY ./EPiServer.Reference.Commerce.Site/App_Data /app/publish/App_Data

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
#wait sql server container start and attach alloy database then start web
ENTRYPOINT ./wait_sqlserver_start_and_attachdb.sh