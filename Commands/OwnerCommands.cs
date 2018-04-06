using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace DiscordServerCloner.Commands
{
    [RequireOwner]
    public class OwnerCommands : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _service;

        public OwnerCommands(CommandService service)
        {
            _service = service;
        }


        [Command("ClearServer", RunMode = RunMode.Async)]
        [Summary("ClearServer <ClearKEY>")]
        [Remarks("Delete ALL Content from the current server")]
        public async Task ClearServer([Remainder]string clearKEY = null)
        {
            await ServerObject.ClearServer(Context.Client, (SocketTextChannel) Context.Channel, clearKEY);
        }

        [Command("ShowList", RunMode = RunMode.Async)]
        [Summary("ShowList")]
        [Remarks("Show all saved servers")]
        public async Task ShowList()
        {
            try
            {
                var saves = Directory.GetFiles(Path.Combine(AppContext.BaseDirectory, "setup/")).Where(x => x.Contains(".txt"));
                var embed = new EmbedBuilder
                {
                    Description = "<ServerName> - <Guild ID> - <Last Save Time>\n" +
                                  "```\n"
                };
                foreach (var file in saves)
                {
                    try
                    {
                        var ns = JsonConvert.DeserializeObject<ServerObject>(File.ReadAllText(file));
                        embed.Description += $"{ns.ServerName} - {Path.GetFileNameWithoutExtension(file)} - {ns.LastSave}\n" +
                                             $"Users: {ns.Users.Count()}\n" +
                                             $"Roles: {ns.Roles.Count()}\n" +
                                             $"Channels: {ns.TextChannels.Count() + ns.AudioChannels.Count()}\n" +
                                             $"Bans: {ns.Bans.Count()}\n";
                    }
                    catch
                    {
                        //
                    }
                    
                }

                embed.Description += "```";
                await ReplyAsync("", false, embed.Build());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        [Command("GetOverview", RunMode = RunMode.Async)]
        [Summary("GetOverview <GuildID>")]
        [Remarks("Get a brief overview of a saved server's data")]
        public async Task GetOverView(ulong GuildID = 0)
        {
            if (GuildID == 0)
            {
                GuildID = Context.Guild.Id;
            }
            var serv = ServerObject.GetSave(GuildID);
            if (serv == null)
            {
                await ReplyAsync("Server Not Saved!");
                return;
            }

            var embed = new EmbedBuilder {Title = serv.ServerName};
            if (serv.TextChannels.Any())
            {
                embed.AddField("Text Channels", string.Join("\n", serv.TextChannels.Select(x => x.ChannelName)));
            }

            if (serv.AudioChannels.Any())
            {
                embed.AddField("Audio Channels", string.Join("\n", serv.AudioChannels.Select(x => x.ChannelName)));
            }

            if (serv.Roles.Any())
            {
                embed.AddField("Roles", string.Join("\n", serv.Roles.Select(x => x.RoleName)));
            }

            if (serv.Categories.Any())
            {
            embed.AddField("Categories", string.Join("\n", serv.Categories.Select(x => x.CategoryName)));
            }

            await ReplyAsync("", false, embed.Build());
        }

        [Command("NotifyUsersTest", RunMode = RunMode.Async)]
        [Summary("NotifyUsersTest <OriginalGuildID> <message>")]
        [Remarks("Test a notification message before sending it")]
        public async Task NotifyUsersTest(ulong GuildId, [Remainder]string Message)
        {
            var embed = new EmbedBuilder();
            var ns = ServerObject.GetSave(GuildId);
            embed.AddField($"Message From {Context.Guild.Name} Guild",
                $"This Message is notifying all logged users of the server: {ns.ServerName}\n\n" +
                $"{Message}");
            embed.Color = Color.Green;
            await ReplyAsync("", false, embed.Build());
        }

        [Command("NotifyUsers", RunMode = RunMode.Async)]
        [Summary("NotifyUsers <OriginalGuildID> <message>")]
        [Remarks("try to message all users from a given server log")]
        public async Task NotifyUsers(ulong GuildId, [Remainder]string Message)
        {
            await ServerObject.NotifyUsers(Context.Client, (SocketTextChannel)Context.Channel, GuildId, Message);
        }

        [Command("SaveServer")]
        [Summary("SaveServer")]
        [Remarks("Save The Current Server Configuration")]
        public async Task SaveServer()
        {
            await ServerObject.SaveServer((SocketTextChannel) Context.Channel, true);
        }

        [Command("LoadRoles", RunMode = RunMode.Async)]
        [Summary("LoadRoles <OriginalGuildID>")]
        [Remarks("Load The Given Server Roles Configuration")]
        public async Task LoadRoles([Remainder] ulong guildid = 0)
        {
            await ServerObject.LoadRoles((SocketTextChannel) Context.Channel, guildid);
        }

        [Command("LoadMessages", RunMode = RunMode.Async)]
        [Summary("LoadMessages <OriginalGuildID>")]
        [Remarks("Load The last 10 messages from each channel in the old server Configuration")]
        public async Task LoadMessages([Remainder] ulong guildid = 0)
        {
            var server = ServerObject.GetSave(guildid);
            foreach (var channel in Context.Guild.TextChannels)
            {
                try
                {
                    if (server.TextChannels.All(x => x.ChannelName != channel.Name)) continue;
                    {
                        var servchannel = server.TextChannels.First(x => x.ChannelName == channel.Name);

                        if (servchannel.LastMessages.Count <= 0) continue;
                        var embed = new EmbedBuilder();
                        foreach (var msg in servchannel.LastMessages)
                        {
                            embed.AddField(msg.author, $"{msg.text}\n\n" +
                                                       $"{msg.timestamp}");
                        }

                        await channel.SendMessageAsync("", false, embed.Build());

                    }
                }
                catch
                {
                    //
                }

            }

            await ReplyAsync("MessageChannel Updates Complete");
        }

        [Command("ReassignRoles", RunMode = RunMode.Async)]
        [Summary("ReassignRoles <OriginalGuildID>")]
        [Remarks("Give all users their roles from the old server")]
        public async Task ReassignRoles([Remainder] ulong guildid = 0)
        {
            var server =  ServerObject.GetSave(guildid);

                foreach (var role in server.Roles)
                {
                    try
                    {
                        if (!Context.Guild.Users.Any(x => role.RoleMembers.Contains(x.Id)) ||
                            Context.Guild.Roles.All(x => x.Name != role.RoleName)) continue;
                        {
                            var rol = Context.Guild.Roles.First(x => x.Name == role.RoleName);
                            foreach (var user in Context.Guild.Users.Where(x => role.RoleMembers.Contains(x.Id)))
                            {
                                await user.AddRoleAsync(rol);
                            }
                        }
                    }
                    catch
                    {
                        //
                    }

                }

            await ReplyAsync("User Roles have been reassigned!");
        }

        [Command("LoadBans", RunMode = RunMode.Async)]
        [Summary("LoadBans <OriginalGuildID>")]
        [Remarks("Load The Given Server Bans Configuration")]
        public async Task LoadBans([Remainder] ulong GuildId = 0)
        {
            await ServerObject.LoadBans((SocketTextChannel) Context.Channel, GuildId);
        }

        [Command("LoadServer", RunMode = RunMode.Async)]
        [Summary("LoadServer <GuildID>")]
        [Remarks("Load The Given Server Configuration")]
        public async Task LoadServer(ulong GuildID = 0)
        {
            await ServerObject.LoadServer((SocketTextChannel) Context.Channel, GuildID);
            var embed = new EmbedBuilder();
            embed.WithColor(Color.Blue);
            embed.AddField("Thankyou for Using DiscordServerCopier By PassiveModding", "Store: https://rocketr.net/sellers/passivemodding");
            await ReplyAsync("", false, embed.Build());
        }

        [Command("help")]
        [Summary("help")]
        [Remarks("all help commands")]
        public async Task HelpAsync()
        {
            var prefix = Config.Load().Prefix;
            var embed = new EmbedBuilder
            {
                Color = new Color(114, 137, 218),
                Title = $"{Config.Load().BotName} | Commands | Prefix: {prefix}"
            };

            foreach (var module in _service.Modules)
            {
                    var list = new List<string>();
                    foreach (var command in module.Commands)
                    {
                        list.Add(
                            $"`{prefix}{command.Summary}` - {command.Remarks}");
                        if (string.Join("\n", list).Length <= 800) continue;
                        embed.AddField(module.Name, string.Join("\n", list));
                        list = new List<string>();
                    }

                    embed.AddField(module.Name, string.Join("\n", list));
            }


            await ReplyAsync("", false, embed.Build());
        }

        [Command("Stats")]
        [Summary("Stats")]
        [Remarks("Display Bot Statistics")]
        public async Task BotStats()
        {
            var embed = new EmbedBuilder();

            var heap = Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2)
                .ToString(CultureInfo.InvariantCulture);
            var uptime = (DateTime.Now - Process.GetCurrentProcess().StartTime).ToString(@"dd\.hh\:mm\:ss");

            embed.AddField($"{Context.Client.CurrentUser.Username} Statistics",
                $"Servers: {Context.Client.Guilds.Count}\n" +
                $"Users: {Context.Client.Guilds.Select(x => x.Users.Count).Sum()}\n" +
                $"Unique Users: {Context.Client.Guilds.SelectMany(x => x.Users.Select(y => y.Id)).Distinct().Count()}\n" +
                $"Server Channels: {Context.Client.Guilds.Select(x => x.Channels.Count).Sum()}\n" +
                $"DM Channels: {Context.Client.DMChannels.Count}\n\n" +
                $"Uptime: {uptime}\n" +
                $"Heap Size: {heap}\n" +
                $"Discord Version: {DiscordConfig.Version}");

            await ReplyAsync("", false, embed.Build());
        }
    }
}