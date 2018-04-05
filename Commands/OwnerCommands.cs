using System;
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
        [Command("ClearServer", RunMode = RunMode.Async)]
        [Summary("ClearServer <key>")]
        [Remarks("Delete ALL Content from the current server")]
        public async Task ClearServer([Remainder]string clearKEY = null)
        {
            await ServerObject.ClearServer(Context.Client, (SocketTextChannel) Context.Channel, clearKEY);
        }

        [Command("GetOverview", RunMode = RunMode.Async)]
        [Summary("Geroverview <serverID>")]
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
            var embed = new EmbedBuilder();
            embed.Title = serv.ServerName;
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
        [Summary("NotifyUsersTest <servername> <message>")]
        [Remarks("Test a notification message before sending it")]
        public async Task NotifyUsersTest(string ServerName, [Remainder]string Message)
        {
            var embed = new EmbedBuilder();
            var saves = Directory.GetFiles(AppContext.BaseDirectory).First(x => x.Contains($"{ServerName}.txt"));
            var ns = JsonConvert.DeserializeObject<ServerObject>(File.ReadAllText(saves));
            embed.AddField($"Message From {Context.Guild.Name} Guild",
                $"This Message is notifying all logged users of the server: {ns.ServerName}\n\n" +
                $"{Message}");
            embed.Color = Color.Green;
            await ReplyAsync("", false, embed.Build());
        }

        [Command("NotifyUsers", RunMode = RunMode.Async)]
        [Summary("NotifyUsers <servername> <message>")]
        [Remarks("try to message all users from a given server log")]
        public async Task NotifyUsers(string ServerName, [Remainder]string Message)
        {
            await ServerObject.NotifyUsers(Context.Client, (SocketTextChannel)Context.Channel, ServerName, Message);
        }

        [Command("SaveServer")]
        [Summary("SaveServer")]
        [Remarks("Save The Current Server Configuration")]
        public async Task SaveServer()
        {
            await ServerObject.SaveServer((SocketTextChannel) Context.Channel);
        }

        [Command("LoadRoles", RunMode = RunMode.Async)]
        [Summary("LoadRoles")]
        [Remarks("Load The Given Server Roles Configuration")]
        public async Task LoadRoles([Remainder] string ServerName = null)
        {
            await ServerObject.LoadRoles((SocketTextChannel) Context.Channel, ServerName);
        }

        [Command("LoadBans", RunMode = RunMode.Async)]
        [Summary("LoadBans")]
        [Remarks("Load The Given Server Bans Configuration")]
        public async Task LoadBans([Remainder] string ServerName = null)
        {
            await ServerObject.LoadBans((SocketTextChannel) Context.Channel, ServerName);
        }

        [Command("LoadServer", RunMode = RunMode.Async)]
        [Summary("LoadServer")]
        [Remarks("Load The Given Server Configuration")]
        public async Task LoadServer(ulong GuildID = 0)
        {
            await ServerObject.LoadServer((SocketTextChannel) Context.Channel, GuildID);
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