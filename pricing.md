# Pricing

### Pricing Scheme

| Action | Uses |
| :--- | :--- |
| Saving a server | 1 use |
| Saving the messages of a channel | 1 use per 1000 messages |
| Saving a server + messages of a channel | 1 use for the server + 1 use per 1000 messages saved |
| Loading a server | 1 use |
| Loading messages of a channel | 1 use per 1000 messages |

NOTE:

When saving and loading messages we round up to next thousand, so any amount from 1-1000 will charge a use. \(This applies to the total message count when doing a full server count, ie. if the rebate sample below saved 8,100 messages it would charge the same as if 9,000 messages were saved\)

### Rebates

Running `clone.save 100`would charge 1 use for the save + 1 use per 10 channels in your server \(since 10 channels with 100 messages saved is 1000 messages\). If a channel's messages are saved but the count is less than the specified amount \(ie. the channel only has 20 messages in it\) then you may be rebated uses depending on the total amount saved.

Example 1:

`clone.save 1000` in a server with 10 channels Initial charge: 1 + 10 \(1 for the save and 1 per 1000 messages ie. 10,000 messages total\) If some channels had less than 1000 messages there is a chance the bot may only saved 8,000 messages. In this case you would be restored 2 uses. so the total cost of the save would be 11 - 2 = 9 uses

Example 2:

`clone.savemessages 10000` will initially charge 10 uses \(1 per 1000 messages\), if the channel only has 3000 messages in it then the bot will save all the messages and restore the remaining 7 uses to your profile. So the message save will cost 10 - 7 = 3 uses

### Pricing Examples:

| Action | Uses |
| :--- | :--- |
| full server save, total 6,500 messages | 7\* |
| single channel save, total 200 messages | 1 |
| full server save, total 0 messages | 0\* |
| server load | 1 |
| channel messages load, 999 messages | 1 |
| channel messages load, 1,100 messages | 2 |
| channel messages load, 11,001 messages | 12 |

\*Not including the cost of saving the server itself \(1 use\)

