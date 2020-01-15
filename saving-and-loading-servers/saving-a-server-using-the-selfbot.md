# Saving a Server using the SelfBot



{% hint style="danger" %}
Using a self bot is against discords TOS and may result in action being taken against your discord account.
{% endhint %}

## Getting your user token <a id="getting-your-user-token"></a>

In order to automate saving the server you'll need to find your discord user token, this was previously easy however has become more complicated in recent updates to discord.

1. Open discord desktop
2. Open the developer tools \(CTRL+SHIFT+I on windows\)
3. Navigate to the network tab in the developer tools
4. In discord select a channel that you haven't yet opened, the network tab should populate with many requests
5. Look in the requests for `messages?limit=50` select it
6. Open the Headers tab of the request
7. Navigate to Request headers and look for `Authorization:`
8. Your token will be the long string of characters that follows this, copy it

## Getting the server ID <a id="getting-the-server-id"></a>

You will need to get the id of the server you wish to save

1. Ensure you are a member of the server you wish to save
2. Open discord settings
3. Navigate to the Appearance tab
4. Scroll down to Advanced
5. Check the Developer Mode toggle to on
6. Navigate to the server you are saving
7. Right click the server
8. Click Copy ID from the context menu

## Downloading the self bot <a id="downloading-the-self-bot"></a>

1. Go to [https://github.com/PassiveModding/ServerSaverSelf/releases](https://github.com/PassiveModding/ServerSaverSelf/releases)
2. Download the latest release for your platform \(mac, ubuntu linux, windows\) if your platform is not in the releases you will have to compile it yourself using the .NET Core 3.0 SDK

{% hint style="warning" %}
Versions 1.2.2 of the self bot and below will not work with the Cloner anymore
{% endhint %}

## Saving the server <a id="saving-the-server"></a>

1. Run the program
2. Ensure you have your user token and the id of the server you are saving handy \(you must be in the server you are saving in order for it to work\)
3. Enter your token into the program
4. Enter the server id \(Guild ID\) into the program
5. The program will verify your token and begin saving the server
6. Once complete it will generate a `.clone` file in the same directory as the program's files - The file will be named after the server and the time it was saved ex. `MyServer-12-30-30 01-01-20.clone`

{% hint style="info" %}
Store this file in a safe place so you can restore the backup as you please
{% endhint %}

{% page-ref page="../loading-a-save.md" %}



