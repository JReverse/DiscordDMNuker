using DiscordDMNuker.Extensions;
using Discord;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiscordDMNuker
{
    public partial class Main : Form
    {
        private ToolStripStatusLabel Status;
        private ListBox Logs;
        private int Index;
        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {

        }

        private async void Start(string Token, ulong UserId, int Delay)
        {
            await Task.Run(async () =>
           {
               Status.SafeChangeText("Starting");

               DiscordClient client = new DiscordClient(Token);
               Logs.SafeAddItem(string.Format("Logged In To: {0}", client.User.Username));
               var User = await client.GetProfileAsync(UserId);
               var channelid = await client.CreateDMAsync(UserId);
               Logs.SafeAddItem(string.Format("Created Dms With: {0}", User.User.Username));
               var msg = await client.GetChannelMessagesAsync(channelid.Id);
               Status.SafeChangeText("In Progress....");
               foreach (DiscordMessage message in msg)
               {
                   if (message.Author.User.Id == client.User.Id)
                   {
                       await message.DeleteAsync();
                       Logs.SafeAddItem(string.Format("Deleted Message: {0}", message.Content));
                       await Task.Delay(Delay);
                   }
               }

               Status.SafeChangeText("Completed");
           });
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            using (FormStart FormStart = new FormStart())
            {
                FormStart.ShowDialog();
                if (FormStart.Start)
                {
                    Status = toolStripStatusLabel1;
                    Logs = listBox1;
                    Index = 0;

                    Start(FormStart.Token, FormStart.UserId, FormStart.Delay);
                }
            }
        }
    }
}
