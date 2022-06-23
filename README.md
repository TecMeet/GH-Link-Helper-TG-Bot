## Link Helper for GitHub issues

When user in a Telegram chat sends an issue number (e.g. #325) then this
bot will send a message with a link to that issue number on GitHub.

### Setup
1. Install ASP.NET Core Runtime 6.0
2. Set all values in the BotConfiguration section in `appsettings.json`
- **BotToken**: The token Telegram BotFather gave you for your bot
- **BotHostAddress**: The _https_ address where this app runs on.
It must have SSL certificate since Telegram requires that (LetsEncrypt works great)
- **GitHubRepoOwner**: Your GitHub username, or the name of your organization
- **GitHubRepoName**: The name of your GitHub repo
- **DropPendingUpdates**: When setting Telegram's webhook to call your app,
drop all existing messages that have not been handled yet (i.e. don't handle past messages)
