# KULiv
KULiv is a student driven organization at the Univeristy of Copenhagen, which seeks to connect students through the use of Discord.

## Contributing
### Find a bug or have a suggestion?
Create a new issue at https://github.com/Faellesskabet/Discord-Botten/issues with your suggestion or bug!


### Creating new features
You are welcome to add whatever you want! Checkout from the develop branch and make whatever you feel like. If you want to help, but are not sure with what, then you can take a look at some existing issues :D

## Setting up
### Prerequisites
1. .NET 6.0
2. .NET Core 3.1
3. [MongoDB](https://www.mongodb.com/try/download/community)

### Setting up Discord Bot
You need to set a Discord Token in order to run the bot,
1. Create a Discord bot and copy its in the [Discord Developer Portal](https://discord.com/developers/applications)
2. Navigate to `Dikubot/Properties/launchSettings.json`
3. Set all the things

You've now connected your own Discord Bot.

### Setting up web
You need to have Sendgrid account and a Sendgrid API token with access to send emails.
1. Navigate to `Dikubot/Properties/launchSettings.json`
2. Set `"SENDGRID_API":` to your Sendgrid API token
3. Navigate to `Dikubot/DataLayer/Logic/WebDiscordBridge/SubDomainConnector.cs` and add the guilds your bot has access to, to the Subdomains enum. The name will be the subdomain of the website, and the ulong is the ID of your Discord guild.


### Setting up Datalayer
1. All you need is to [install MongoDB](https://www.mongodb.com/try/download/community) if you haven't already
2. Make sure it is running

