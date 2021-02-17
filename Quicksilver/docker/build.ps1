Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass

#Install ssh client
if ((Get-WindowsCapability -Online | ? Name -like 'OpenSSH*')[0].State -ne "Installed") {
  Write-Host "Install openssh client"
  Add-WindowsCapability -Online -Name OpenSSH.Client~~~~0.0.1.0
}

$defaultSitePort = "8000"
$defaultTag = "latest"
$dbFileName = "quicksilver-db.tar"
$webFileName = "quicksilver-web.tar"
# At this moment, we only have 1 VM.
# vnadmin is the username; 10.120.18.240 is the VM address; /home/vnadmin/Downloads is the location of downloaded file
$vmAddress = "10.120.18.240"
$vmUser = "vnadmin"
#enter 'y' if you don't want to build sql image
$skipSql = Read-Host "Skip building new sql image (y/n)"
$enterSqlTagNameMessage = "Enter sql image tag (default is $defaultTag)"

if ($skipSql -eq 'y') { 
  $enterSqlTagNameMessage += ".If you skip building new sql image, enter the created tag that you want to re-use"
}

if (!($sitePort = Read-Host "Enter site port (default is $defaultSitePort)")) { $sitePort = $defaultSitePort }
Write-Host "Site port is" $sitePort
$env:site_port = $sitePort

#set tag for images
if (!($webTag = Read-Host "Enter web image tag (default is $defaultTag)")) { $webTag = $defaultTag }
Write-Host "Web tag is" $webTag
if (!($sqlTag = Read-Host $enterSqlTagNameMessage)) { $sqlTag = $defaultTag }
Write-Host "SQL tag is" $sqlTag
$env:web_tag = $webTag
$env:sql_tag = $sqlTag
#set password for sql server (you can change it here)
$env:sa_password = "Your_password123"

#build docker compose (change build command to up if you want to build then run images)
Write-Host "Building docker images "
if ($skipSql -eq 'y') { 
  docker-compose -f ./../docker-compose.yml build --force-rm web
}
else {
  dotnet restore "./../EPiServer.Reference.Commerce.Site/EPiServer.Reference.Commerce.Site.csproj"
  docker-compose -f ./../docker-compose.yml build --force-rm
}

Write-Host "Copy docker-compose file and replace parameters"
Copy-Item ..\docker-compose.yml -Destination .
(Get-Content -path docker-compose.yml).replace('${sa_password}', $env:sa_password).replace('${web_tag}', $env:web_tag).replace('${sql_tag}', $env:sql_tag).replace('${site_port}', $env:site_port) | Set-Content docker-compose.yml

$folderName = "$env:web_tag$env:sql_tag"
$zipFileName = "$folderName.zip"
if (!($confirmation = Read-Host "Do you want to upload $zipFileName file to VM (y/n)")) { $confirmation = 'y' }
if ($confirmation -eq 'y') {
  Write-Host "Export docker images to files"

  if (!($skipSql -eq 'y')) {
    docker save -o $dbFileName quicksilver/db:$env:sql_tag
  }
  docker save -o $webFileName quicksilver/web:$env:web_tag

  Write-Host "Zip files"
  $zipFileArray = "docker-compose.yml", "$webFileName", ".\run-script\*.*"
  if (!($skipSql -eq 'y')) {
    $zipFileArray += "$dbFileName"
  }

  $compress = @{
    Path             = $zipFileArray
    CompressionLevel = "Optimal"
    DestinationPath  = ".\$zipFileName"
  }
  Compress-Archive -Force @compress


  Write-Host "Upload to VM"
  scp ".\$zipFileName" ${vmUser}@${vmAddress}:/home/vnadmin/Downloads
  if ($skipSql -eq 'y') { 
    ssh ${vmUser}@${vmAddress} "cd Downloads/ && unzip $zipFileName -d ./$folderName  && cd $folderName && find . -name '*.sh' -type f | xargs dos2unix  && find . -name '*.sh' -type f | xargs chmod 777  && sudo -S ./build_web_only.sh && sudo -S ./stop_web_only.sh && sudo -S ./run.sh && exit"
  }
  else {
    ssh ${vmUser}@${vmAddress} "cd Downloads/ && unzip $zipFileName -d ./$folderName  && cd $folderName && find . -name '*.sh' -type f | xargs dos2unix  && find . -name '*.sh' -type f | xargs chmod 777  && sudo -S ./build.sh && sudo -S ./stop.sh && sudo -S ./run.sh && exit"
  }
  Write-Host "Clean up temp zip files"
  Remove-Item -Force $zipFileName
  if (!($skipSql -eq 'y')) {
    Remove-Item -Force $dbFileName
  }
  Remove-Item -Force $webFileName
}

Write-Host "Clean up temp files"
Remove-Item -Force "docker-compose.yml"

Write-Host "Successful"
Read-Host -Prompt "Press Enter to exit"