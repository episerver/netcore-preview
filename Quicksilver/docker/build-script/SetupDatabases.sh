#!/bin/bash
sleep 30s

export cms_db=netcore.qs.cms
export commerce_db=netcore.qs.commerce
export user=netcoreUser
export password=Episerver123!
export cms_core_path="../packages/episerver.cms.core/*"
export commerce_core_path="../packages/episerver.commerce.core/*"
export personalization_commerce_path="../packages/episerver.personalization.commerce/*"
for i in $(ls -d $cms_core_path); do
    if [[ $i =~ $cms_core_path ]]; then
        export cms_core=$i
    fi
done
for i in $(ls -d $commerce_core_path); do
    if [[ $i =~ $commerce_core_path ]]; then
        export commerce_core=$i
    fi
done
for i in $(ls -d $personalization_commerce_path); do
    if [[ $i =~ $personalization_commerce_path ]]; then
      export personalization_commerce=$i
    fi
done

if [[ $cms_core == "" ]]; then
    echo CMS Core package is missing. Please build the project before running the setup.
    exit
fi
if [[ $commerce_core == "" ]]; then
    echo Commerce Core package is missing. Please build the project before running the setup.
    exit
fi

if [[ $personalization_commerce == "" ]]; then
    echo Personalization Commerce package is missing. Please build the project before running the setup.
    exit
fi

export sql="/opt/mssql-tools/bin/sqlcmd -S . -U sa -P ${SA_PASSWORD}"
echo Dropping databases...
$sql -Q "EXEC msdb.dbo.sp_delete_database_backuphistory N'$cms_db'"
$sql -Q "if db_id('$cms_db') is not null ALTER DATABASE [$cms_db] SET SINGLE_USER WITH ROLLBACK IMMEDIATE"
$sql -Q "if db_id('$cms_db') is not null DROP DATABASE [$cms_db]"
$sql -Q "EXEC msdb.dbo.sp_delete_database_backuphistory N'$commerce_db'"
$sql -Q "if db_id('$commerce_db') is not null ALTER DATABASE [$commerce_db] SET SINGLE_USER WITH ROLLBACK IMMEDIATE"
$sql -Q "if db_id('$commerce_db') is not null DROP DATABASE [$commerce_db]"

echo Dropping user...
$sql -Q "if exists (select loginname from master.dbo.syslogins where name = '$user') EXEC sp_droplogin @loginame='$user'"

echo Creating databases...
$sql -Q "CREATE DATABASE [$cms_db] COLLATE SQL_Latin1_General_CP1_CI_AS"
$sql -Q "CREATE DATABASE [$commerce_db] COLLATE SQL_Latin1_General_CP1_CI_AS"

echo Creating user...
$sql -Q "EXEC sp_addlogin @loginame='$user', @passwd='$password', @defdb='$cms_db'"
$sql -d $cms_db -Q "EXEC sp_adduser @loginame='$user'"
$sql -d $cms_db -Q "EXEC sp_addrolemember N'db_owner', N'$user'"
$sql -d $commerce_db -Q "EXEC sp_adduser @loginame='$user'"
$sql -d $commerce_db -Q "EXEC sp_addrolemember N'db_owner', N'$user'"

echo Installing CMS database...
$sql -d $cms_db -b -i "$cms_core/tools/EPiServer.Cms.Core.sql" >SetupCmsDb.log

echo Installing Commerce database...
$sql -d $commerce_db -b -i "$commerce_core/tools/EPiServer.Commerce.Core.sql" >SetupCommerceDb.log

echo Installing Commerce database...
$sql -d $commerce_db -b -i "$personalization_commerce/tools/epiupdates_commerce/sql/1.0.0.sql" >SetupPersonalizationCommerce.log

#echo Installing ASP.NET Identity...
#$sql -d $commerce_db -b -i "/docker/aspnet_identity.sql" >SetupIdentity.log