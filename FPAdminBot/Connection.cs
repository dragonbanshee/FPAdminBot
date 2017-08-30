using Discord;
using Meebey.SmartIrc4net;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace FPAdminBot
{
    public class Connection
    {
        private DiscordClient DiscordClient;
        private IrcClient IrcClient;
        private Config Config;

        private static ulong DISCORD_ADMIN_CHAN = 306261888394854400;

        public Connection()
        {
            DiscordClient = new DiscordClient();
            IrcClient = new IrcClient();
            Config = File.ReadAllText("config.json").ParseJSON<Config>();
            Connect();
        }

        public void ConnectIRC()
        {
            IrcClient = new IrcClient();
            
            IrcClient.SendDelay = 200;
            IrcClient.Encoding = Encoding.UTF8;
            IrcClient.ActiveChannelSyncing = true;
            IrcClient.AutoRejoin = true;

            IrcClient.OnChannelMessage += IrcClient_OnChannelMessage;
            IrcClient.OnError += IrcClient_OnError;
            IrcClient.OnRawMessage += IrcClient_OnRawMessage;

            try
            {
                IrcClient.Connect("irc.rizon.net", 6667);
            }
            catch(ConnectionException ce)
            {
                Console.WriteLine("Unable to connect: " + ce.Message + ": " + ce.StackTrace);
                return;
            }

            try
            {
                IrcClient.Login("FPAlerts", "Firepowered Alert Bot");
            }
            catch(Exception e)
            {
                Console.WriteLine("Unable to login: " + e.Message + ": " + e.StackTrace);
                return;
            }
            IrcClient.RfcPrivmsg("NickServ", "IDENTIFY " + Config.NickServPass);
            IrcClient.RfcJoin("#firepowered");
            IrcClient.RfcJoin("#firepowered-admins");
            IrcClient.SendMessage(SendType.Action, "#firepowered", "connected!");
            IrcClient.Listen(true);
        }

        private void IrcClient_OnRawMessage(object sender, IrcEventArgs e)
        {
            Console.WriteLine("RECEIVED IRC MESSAGE: " + e.Data.Message);
            if (e.Data.From == "FirePowered!~core@FirePowered.bot.firepowered" && e.Data != null && e.Data.Message != null)
            {
                if (e.Data.Message != "" && e.Data.Message.Contains("ABUN1") || e.Data.Message.Contains("ABHT"))
                {
                    DiscordClient.GetChannel(DISCORD_ADMIN_CHAN).SendMessage(e.Data.Message);
                }
            }
        }

        private void IrcClient_OnError(object sender, Meebey.SmartIrc4net.ErrorEventArgs e)
        {
            Console.WriteLine("ERROR: " + e.ErrorMessage);
        }

        private void IrcClient_OnChannelMessage(object sender, IrcEventArgs e)
        {
            if(e.Data.Channel == "#firepowered-admins" && e.Data.From == "FirePowered!~core@FirePowered.bot.firepowered")
            {
                if (e.Data.Message.Contains("has reported"))
                {
                    DiscordClient.GetChannel(DISCORD_ADMIN_CHAN).SendMessage("<@&306863720364244992> " + e.Data.Message);
                }
                else
                {
                    DiscordClient.GetChannel(DISCORD_ADMIN_CHAN).SendMessage(e.Data.Message);
                }
            }
        }

        private async Task ConnectDiscord()
        {
            DiscordClient = new DiscordClient();
            await DiscordClient.Connect(Config.DiscordToken, TokenType.Bot);
            DiscordClient.MessageReceived += Client_MessageReceived;
        }

        private void Client_MessageReceived(object sender, MessageEventArgs e)
        {
            if(e.Channel.Name == "adminalerts" && !e.Message.IsAuthor)
            {
                IrcClient.SendMessage(SendType.Message, "#firepowered-admins", e.Message.Text);
            }
        }

        public async void Connect()
        {
            Console.WriteLine("Connecting to Discord...");
            ConnectDiscord();
            ConnectIRC();
            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }

    internal class Config
    {
        public string DiscordToken { get; set; }
        public string NickServPass { get; set; }
    }
}
