# sharper-dikubot
A complete rewrite of the DIKU discord bot in C#

## Getting started
The application is split into 3 three main components, there's the Discord bot, the website and a datalayer, which is how the two mostly communicate.

### Setting up Discord Bot
You need to set a Discord Token in order to run the bot,
1. Create a Discord bot and copy its in the [Discord Developer Portal](https://discord.com/developers/applications)
2. Navigate to `Dikubot/Properties/launchSettings.json`
3. Set `"DISCORD_TOKEN:"` to your bot's Discord Token

You've now connected your own Discord Bot.

### Setting up web
You need to have Sendgrid account and a Sendgrid API token with access to send emails.
1. Navigate to `Dikubot/Properties/launchSettings.json`
2. Set `"SENDGRID_API":` to your Sendgrid API token
3. Navigate to `Dikubot/DataLayer/Logic/WebDiscordBridge/SubDomainConnector.cs` and add the guilds your bot has access to, to the Subdomains enum. The name will be the subdomain of the website, and the ulong is the ID of your Discord guild.


### Setting up Datalayer
1. All you need is to [install MongoDB](https://www.mongodb.com/try/download/community)



