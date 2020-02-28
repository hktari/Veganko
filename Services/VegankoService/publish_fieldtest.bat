ssh rpi 'veganko/util/prepare_for_deploy.sh'
scp bin/FieldTest/netcoreapp2.1/linux-arm/publish/* rpi:veganko/publish
pause
