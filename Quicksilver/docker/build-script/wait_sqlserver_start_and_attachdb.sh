sleep 180s
    echo "CMS connection string:" $ConnectionStrings__EPiServerDB
    echo "Commerce connection string:" $ConnectionStrings__EcfSqlConnection
dotnet EPiServer.Reference.Commerce.Site.dll