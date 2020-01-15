# Loading a Save

{% hint style="info" %}
Best results are achieved if you load a save in a new server with no channels or roles, this is recommended but optional
{% endhint %}

## Setting up

1. Invite the bot to your server [https://discordapp.com/oauth2/authorize?client\_id=500791607839162376&scope=bot&permissions=8](https://discordapp.com/oauth2/authorize?client_id=500791607839162376&scope=bot&permissions=8)
2. Ensure you and the bot both have administrator permissions
3. Find your `.clone` file from earlier, this contains all the save information for your server

## Loading the Save

1. attach your `.clone` file with the message `clone.load`

   ![](.gitbook/assets/image.png)  
   - Additionally you can specify `nobans` and/or `nomessages` to stop the bot from copying user bans and channel messages over. Do so by running the load command with those parameters ex. `clone.load nobans` or `clone.load nobans nomessages`

2. The bot will automatically create roles, channels, categories etc. and apply the correct settings and permissions to each
3. Enjoy your backup server

