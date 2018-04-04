using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace DiscordServerCloner
{
    public class ServerPairs
    {
        public static List<serversused> PairList { get; set; }
        public class serversused
        {
            public string SavedServer { get; set; }
            public string LoadedServer { get; set; }
        }

    }

    public class ServerObject
    {
        public string ServerName { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<TextChannel> TextChannels { get; set; }
        public IEnumerable<AudioChannel> AudioChannels { get; set; }
        public IEnumerable<Role> Roles { get; set; }
        public IEnumerable<ulong> Bans { get; set; }
        public GuildPermissions EveryonePerms { get; set; }
        public IEnumerable<ulong> Users { get; set; }

        public static async Task LoadServer(SocketTextChannel thechannel, string ServerName = null)
        {
            var Guild = thechannel.Guild;
            if (ServerName == null)
            {
                var saves = Directory.GetFiles(AppContext.BaseDirectory).Where(x => x.Contains(".txt"));
                await thechannel.SendMessageAsync("Select a config\n" +
                                                  $"{string.Join("\n", saves)}");
            }
            else
            {
                var saves = Directory.GetFiles(AppContext.BaseDirectory).First(x => x.Contains($"{ServerName}.txt"));
                var ns = JsonConvert.DeserializeObject<ServerObject>(File.ReadAllText(saves));
                await thechannel.SendMessageAsync("Loaded Server Config!");
                await Guild.ModifyAsync(x => x.Name = ns.ServerName);
                await thechannel.SendMessageAsync(
                    "Adding Roles! (BOT Roles Not Included, invite bots back and do these manually)");
                if (ns.Roles.Any())
                    foreach (var role in ns.Roles.OrderBy(x => x.position))
                    {
                        var rol = await Guild.CreateRoleAsync(role.RoleName,
                            new GuildPermissions(role.GuildPermissions));
                        await rol.ModifyAsync(x => x.Position = role.position);
                        await rol.ModifyAsync(x => x.Color = new Optional<Color>(new Color(role.RawColour)));
                    }

                await Guild.EveryoneRole.ModifyAsync(x => x.Permissions = ns.EveryonePerms);

                await thechannel.SendMessageAsync("Adding Categories!");
                if (ns.Categories.Any())
                    foreach (var category in ns.Categories)
                    {
                        var cat = await Guild.CreateCategoryChannelAsync(category.CategoryName);
                        await cat.ModifyAsync(x => x.Position = category.Position);
                        foreach (var perm in category.CategoryPermissions)
                            await cat.AddPermissionOverwriteAsync(Guild.Roles.First(x => x.Name == perm.PRole),
                                new OverwritePermissions(perm.AChannelPermissions, perm.DChannelPermissions));
                    }

                await thechannel.SendMessageAsync("Adding Audio Channels!");
                if (ns.AudioChannels.Any())
                    foreach (var channel in ns.AudioChannels)
                    {
                        if (Guild.CategoryChannels.Any(x =>
                            x.Name == channel.ChannelName && x.Position == channel.Position)) continue;

                        var chan = await Guild.CreateVoiceChannelAsync(channel.ChannelName);

                        foreach (var permission in channel.ChannelPermissions)
                            await chan.AddPermissionOverwriteAsync(Guild.Roles.First(x => x.Name == permission.PRole),
                                new OverwritePermissions(permission.AChannelPermissions,
                                    permission.DChannelPermissions));
                        await chan.ModifyAsync(x =>
                            x.CategoryId = Guild.CategoryChannels
                                .FirstOrDefault(y => string.Equals(y.Name, channel.category,
                                    StringComparison.CurrentCultureIgnoreCase))?.Id);
                        await chan.ModifyAsync(x => x.UserLimit = channel.UserLimit);
                        await chan.ModifyAsync(x => x.Position = channel.Position);
                    }

                await thechannel.SendMessageAsync("Adding Text Channels!");
                if (ns.TextChannels.Any())
                    foreach (var channel in ns.TextChannels)
                    {
                        if (Guild.CategoryChannels.Any(x =>
                            x.Name == channel.ChannelName && x.Position == channel.Position)) continue;

                        var chan = await Guild.CreateTextChannelAsync(channel.ChannelName);

                        foreach (var permission in channel.ChannelPermissions)
                            await chan.AddPermissionOverwriteAsync(Guild.Roles.First(x => x.Name == permission.PRole),
                                new OverwritePermissions(permission.AChannelPermissions,
                                    permission.DChannelPermissions));
                        await chan.ModifyAsync(x => x.IsNsfw = channel.IsNSFW);
                        await chan.ModifyAsync(x =>
                            x.CategoryId = Guild.CategoryChannels
                                .FirstOrDefault(y => string.Equals(y.Name, channel.category,
                                    StringComparison.CurrentCultureIgnoreCase))?.Id);
                        await chan.ModifyAsync(x => x.Position = channel.Position);
                        await chan.ModifyAsync(x => x.Topic = channel.topic);
                        var embed = new EmbedBuilder();
                        foreach (var msg in channel.LastMessages)
                        {
                            embed.AddField(msg.author, $"{msg.text}\n\n" +
                                                       $"{msg.timestamp}");
                        }

                        await chan.SendMessageAsync("", false, embed.Build());
                    }

                await thechannel.SendMessageAsync("Complete!\n" +
                                                  "NOTE: Use LoadBans to load the previous server's bans!");
            }
        }

        public static async Task ClearServer(DiscordSocketClient client, SocketTextChannel thechannel, string clearKEY)
        {
            var Guild = thechannel.Guild;
            //to ensure that the user is actually paying attention, ask for a standard verification key before deleting all server content
            if (clearKEY != "o2fovbhwiuh")
            {
                await thechannel.SendMessageAsync("ERROR: The Key is `o2fovbhwiuh`\n" +
                                                  "This command will delete EVERYTHING in this server. You have been warned.");
                return;
            }

            try
            {
                //remove all channels in the guild
                foreach (var channel in Guild.Channels) await channel.DeleteAsync();

                var nchan = await Guild.CreateTextChannelAsync("default");
                //Only try to delete roles below the bot's role level
                //also ignore the @everyone role
                foreach (var role in Guild.Roles)
                    if (role.Position < Guild.Roles.Where(x => x.Members.Any(y => y.Id == client.CurrentUser.Id))
                            .Max(x => x.Position) && !role.IsEveryone)
                        await role.DeleteAsync();
                    else if (role.IsEveryone)
                    {

                    }
                    else
                    {
                        await nchan.SendMessageAsync($"Unable to Remove {role.Name}, insufficient permissions");
                    }


                await nchan.SendMessageAsync("Server Purge Complete!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static async Task LoadBans(SocketTextChannel channel, string ServerName = null)
        {
            //ensure that the user picks the correct server config
            if (ServerName == null)
            {
                var saves = Directory.GetFiles(AppContext.BaseDirectory).Where(x => x.Contains(".txt"));
                await channel.SendMessageAsync("Select a config\n" +
                                               $"{string.Join("\n", saves)}");
            }
            else
            {
                //deserialises the server file from json to ServerObject
                var saves = Directory.GetFiles(AppContext.BaseDirectory).First(x => x.Contains($"{ServerName}.txt"));
                var ns = JsonConvert.DeserializeObject<ServerObject>(File.ReadAllText(saves));
                await channel.SendMessageAsync("Loaded Server Config!");
                await channel.SendMessageAsync("Adding Bans!");
                //Check to see if there are any bans
                if (ns.Bans.Any())
                    foreach (var ban in ns.Bans)
                        await channel.Guild.AddBanAsync(ban);
                await channel.SendMessageAsync("Done!");
            }
        }

        public static ServerObject GetServer(string servername = null)
        {
            if (servername == null) return null;
            var saves = Directory.GetFiles(AppContext.BaseDirectory).First(x => x.Contains($"{servername}.txt"));
            var ns = JsonConvert.DeserializeObject<ServerObject>(File.ReadAllText(saves));
            return ns;

        }

        public static async Task NotifyUsers(DiscordSocketClient Client, SocketTextChannel channel, string ServerName = null, string Message = null)
        {
            //ensure that the user picks the correct server config
            if (ServerName == null)
            {
                var saves = Directory.GetFiles(AppContext.BaseDirectory).Where(x => x.Contains(".txt"));
                await channel.SendMessageAsync("Select a config\n" +
                                               $"{string.Join("\n", saves)}");
            }
            else
            {
                //deserialises the server file from json to ServerObject
                var saves = Directory.GetFiles(AppContext.BaseDirectory).First(x => x.Contains($"{ServerName}.txt"));
                var ns = JsonConvert.DeserializeObject<ServerObject>(File.ReadAllText(saves));
                await channel.SendMessageAsync("Loaded Server Config!");
                var embed = new EmbedBuilder();
                        embed.AddField($"Message From {channel.Guild.Name} Guild",
                            $"This Message is notifying all logged users of the server: {ns.ServerName}\n\n" +
                            $"{Message}");
                embed.Color = Color.Green;
                foreach (var uID in ns.Users)
                {
                    try
                    {
                        var user = Client.GetUser(uID);

                        await user.SendMessageAsync("", false, embed.Build());
                    }
                    catch
                    {
                        //
                    }

                    await Task.Delay(200);
                }
                await channel.SendMessageAsync("Done!");
            }
        }

        public static async Task LoadRoles(SocketTextChannel channel, string ServerName = null)
        {
            var Guild = channel.Guild;
            if (ServerName == null)
            {
                var saves = Directory.GetFiles(AppContext.BaseDirectory).Where(x => x.Contains(".txt"));
                await channel.SendMessageAsync("Select a config\n" +
                                               $"{string.Join("\n", saves)}");
            }
            else
            {
                //deserialises the server file from json to ServerObject
                var saves = Directory.GetFiles(AppContext.BaseDirectory).First(x => x.Contains($"{ServerName}.txt"));
                var ns = JsonConvert.DeserializeObject<ServerObject>(File.ReadAllText(saves));
                await channel.SendMessageAsync("Loaded Server Config!");
                //Rename the server to the saved one
                await Guild.ModifyAsync(x => x.Name = ns.ServerName);
                await channel.SendMessageAsync(
                    "Adding Roles! (BOT Roles Not Included, invite bots back and do these manually)");
                //check if there are any roles saved
                //it is important to order these by position so they get saved and loaded in the correct order
                if (ns.Roles.Any())
                    foreach (var role in ns.Roles.OrderBy(x => x.position))
                    {
                        //re-create the roles with the same name, permissions and colour as before
                        var rol = await Guild.CreateRoleAsync(role.RoleName,
                            new GuildPermissions(role.GuildPermissions), new Color(role.RawColour));
                        //to ensure the position, set it at the end.
                        await rol.ModifyAsync(x => x.Position = role.position);
                    }
                //modify the permissions of the @everyone role
                await Guild.EveryoneRole.ModifyAsync(x => x.Permissions = ns.EveryonePerms);
                await channel.SendMessageAsync("Done!");
            }
        }

        public static async Task<List<Message>> GetMessagesAsync(SocketTextChannel channel)
        {
            return (from item in await channel.GetMessagesAsync(10).FlattenAsync()
                select new Message
                {
                    author = item.Author.Username,
                    text = item.Content,
                    timestamp = item.Timestamp
                }).ToList();
        }

        public static async Task SaveServer(SocketTextChannel Channel)
        {
            var Guild = Channel.Guild;
            try
            {
                //normally you would use an object initialiser for this, however in a try catch loop we would miss any errors
                //so set each value indivisually for the object.
                var ns = new ServerObject();
                //save the guild name
                ns.ServerName = Guild.Name;
                //save the @everyone role permissions
                ns.EveryonePerms = Guild.EveryoneRole.Permissions;
                //save each category with its name, position and permissions for each role
                ns.Categories = Guild.CategoryChannels.Select(x =>
                    new Category
                    {
                        CategoryName = x.Name,
                        Position = x.Position,
                        CategoryPermissions = x.PermissionOverwrites.Where(y => y.TargetType == PermissionTarget.Role)
                            .Select(y => new Permissions
                            {
                                AChannelPermissions = y.Permissions.AllowValue,
                                DChannelPermissions = y.Permissions.DenyValue,
                                PRole = Guild.GetRole(y.TargetId).Name
                            })
                    });
                //save each text channel with name, permissions, NSFW Status, position, topic and category (if applicable)
                ns.TextChannels = Guild.TextChannels.Where(x => !Guild.CategoryChannels.Any(y =>
                    string.Equals(y.Name, x.Name, StringComparison.CurrentCultureIgnoreCase) &&
                    y.Position == x.Position)).Select(x => new TextChannel
                    {
                        ChannelName = x.Name,
                        ChannelPermissions = x.PermissionOverwrites.Where(z => z.TargetType == PermissionTarget.Role)
                        .Select(z => new Permissions
                        {
                            AChannelPermissions = z.Permissions.AllowValue,
                            DChannelPermissions = z.Permissions.DenyValue,
                            PRole = Guild.GetRole(z.TargetId).Name
                        }),
                        IsNSFW = x.Name.ToLower().Contains("nsfw"),
                        Position = x.Position,
                        category = x.Category?.Name,
                        topic = x.Topic,
                        LastMessages = GetMessagesAsync(x).Result
                    });
                //save each audio channel with its name, permissions, position and category (if applicable)
                ns.AudioChannels = Guild.VoiceChannels.Where(x => !Guild.CategoryChannels.Any(y =>
                    string.Equals(y.Name, x.Name, StringComparison.CurrentCultureIgnoreCase) &&
                    y.Position == x.Position)).Select(x => new AudioChannel
                    {
                        ChannelName = x.Name,
                        ChannelPermissions = x.PermissionOverwrites.Where(z => z.TargetType == PermissionTarget.Role)
                        .Select(z => new Permissions
                        {
                            AChannelPermissions = z.Permissions.AllowValue,
                            DChannelPermissions = z.Permissions.DenyValue,
                            PRole = Guild.GetRole(z.TargetId).Name
                        }),
                        Position = x.Position,
                        category = x.Category?.Name,
                        UserLimit = x.UserLimit
                    });

                //save each role with name, colour, members, permissions & position
                ns.Roles = Guild.Roles.Where(x => !x.IsEveryone && !x.IsManaged).Select(x => new Role
                {
                    RoleName = x.Name,
                    RawColour = x.Color.RawValue,
                    RoleMembers = x.Members.Select(y => y.Id).ToList(),
                    GuildPermissions = x.Permissions.RawValue,
                    position = x.Position
                });
                //save all bans for the server
                ns.Bans = Guild.GetBansAsync().Result.Select(x => x.User.Id);

                //save a list of all the user IDs from the last server
                ns.Users = Guild.Users.Select(x => x.Id);

                //serialise the server object to a json string and save to txt file for later use.
                var serverobj = JsonConvert.SerializeObject(ns, Formatting.Indented);
                File.WriteAllText(Path.Combine(AppContext.BaseDirectory, $"{Guild.Name}.txt"), serverobj);
                await Channel.SendMessageAsync("Server Saved.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public class Category
        {
            public string CategoryName { get; set; }
            public int Position { get; set; }
            public IEnumerable<Permissions> CategoryPermissions { get; set; }
        }


        public class Role
        {
            public string RoleName { get; set; }
            public List<ulong> RoleMembers { get; set; }
            public uint RawColour { get; set; }
            public ulong GuildPermissions { get; set; }
            public int position { get; set; }
        }


        public class TextChannel
        {
            public bool IsNSFW { get; set; }
            public string ChannelName { get; set; }
            public IEnumerable<Permissions> ChannelPermissions { get; set; }
            public int Position { get; set; }
            public string category { get; set; }
            public string topic { get; set; }
            public List<Message> LastMessages { get; set; }
        }

        public class Message
        {
            public string text { get; set; }
            public string author { get; set; }
            public DateTimeOffset timestamp { get; set; }
        }

        public class AudioChannel
        {
            public int? UserLimit { get; set; } = 0;
            public string ChannelName { get; set; }
            public IEnumerable<Permissions> ChannelPermissions { get; set; }
            public int Position { get; set; }
            public string category { get; set; }
        }

        public class Permissions
        {
            public string PRole { get; set; }
            public ulong AChannelPermissions { get; set; }
            public ulong DChannelPermissions { get; set; }
        }
    }
}