# All Commands

## Saving and Loading <a id="saving-and-loading"></a>

<table>
  <thead>
    <tr>
      <th style="text-align:left">Command</th>
      <th style="text-align:left">Parameters</th>
      <th style="text-align:left">Summary</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td style="text-align:left"><b>clone.save</b>
      </td>
      <td style="text-align:left">&#x200B;</td>
      <td style="text-align:left">Saves the current server, requires both the bot and user have administrator
        permissions in the server</td>
    </tr>
    <tr>
      <td style="text-align:left"><b>clone.load</b>
      </td>
      <td style="text-align:left">
        <p><code>clone.load</code>
        </p>
        <p><code>clone.load nobans</code>
        </p>
      </td>
      <td style="text-align:left">Loads the attached <code>.clone</code> file into the current server, requires
        both the bot and user have administrator permissions in the server</td>
    </tr>
    <tr>
      <td style="text-align:left"><b>clone.loadmessages</b>
      </td>
      <td style="text-align:left"></td>
      <td style="text-align:left">Loads the attached <code>.clone</code> file and finds the current channel
        by name. Creates a webhook and sends the saved messages to that channel.</td>
    </tr>
    <tr>
      <td style="text-align:left"><b>clone.loadroles</b>
      </td>
      <td style="text-align:left"></td>
      <td style="text-align:left">Loads 50 roles from the attached <code>.clone</code> file, this command
        may need to be run multiple times if the backup has more than 50 roles
        contained</td>
    </tr>
    <tr>
      <td style="text-align:left"><b>clone.clear</b>
      </td>
      <td style="text-align:left"><code>clone.clear confirmation_key</code>
      </td>
      <td style="text-align:left">Deletes ALL roles and channels in the current server</td>
    </tr>
    <tr>
      <td style="text-align:left"><b>clone.clearbans</b>
      </td>
      <td style="text-align:left"><code>clone.clearbans confirmation_key</code>
      </td>
      <td style="text-align:left">Unbans all banned users from the server</td>
    </tr>
  </tbody>
</table>

## Profile <a id="profile"></a>

| Command | Parameters | Summary |
| :--- | :--- | :--- |
| **clone.profile** | ​ | Displays current user info such as remaining balance, total uses used, total uses redeemed and if the user has generated a backup key |
| **clone.redeem** | `clone.redeem key` | Redeems the provided license key or prompts for purchase |
| **clone.history** | ​ | Displays the bot's history logs for the current user |
| **clone.purchase** | ​ | Displays the store url and some useful information |

## Information <a id="information"></a>

| Command | Parameters | Summary |
| :--- | :--- | :--- |
| **clone.stats** | ​ | Displays useful information about the bot's server counts, users, shards, uptime etc. |
| **clone.ping** | ​ | Displays the current websocket latency, heartbeat and round trip speeds of the bot |
| **clone.shards** | ​ | Displays shard information that can help determine outages for different groups of servers |
| **clone.invite** | ​ | Displays the invite url for the bot with admin permissions |
| **clone.tutorial** | ​ | Shows a simple tutorial for saving and loading a backup |
| **clone.transfertutorial** | ​ | Shows a simple tutorial for transfering uses to another discord account |
| **clone.consumetutorial** | ​ | Shows how uses are consumed for different actions with the bot |
| **clone.help** | ​ | Displays an interactive help command with only commands that you can access currently |
| **clone.fullhelp** | ​ | Displays all commands interactively |

