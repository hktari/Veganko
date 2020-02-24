ssh rpi 'veganko/util/prepare-for-deploy.sh'
scp /bin/Release/netcoreapp2.1/linux-arm/publish/* pi@192.168.1.149:veganko/publish
pause
