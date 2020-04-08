dotnet publish -c Release -r linux-arm
ssh rpi 'veganko/util/prepare_for_deploy.sh'
scp bin/Release/netcoreapp2.1/linux-arm/publish/* rpi:veganko/publish
ssh rpi 'veganko/util/finish_deploy.sh'
pause
