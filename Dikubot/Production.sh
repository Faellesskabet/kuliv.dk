sudo systemctl stop kestrel-bot.service
sudo rm -rf ./bin/Release/net6.0
dotnet publish --configuration Release
rm -rf ./bin/Release/net6.0/wwwroot
cp -r Webapp/wwwroot/ ./bin/Release/net6.0
sudo systemctl start kestrel-bot.service
