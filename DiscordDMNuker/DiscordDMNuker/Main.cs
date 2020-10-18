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
using Discord.Media;
using Discord.Gateway;
using System.Net.Mime;
using System.Net;

namespace DiscordDMNuker
{
    public partial class Main : Form
    {
        private ToolStripStatusLabel Status;
        private ListBox Logs;
        private int Index;

        string currentPath = Directory.GetCurrentDirectory();
        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            if (!Directory.Exists(Path.Combine(currentPath, "SavedMedia")))
                Directory.CreateDirectory(Path.Combine(currentPath, "SavedMedia"));

            if (!Directory.Exists(Path.Combine(currentPath, "SavedConvos")))
                Directory.CreateDirectory(Path.Combine(currentPath, "SavedConvos"));
        }

        private async void Start(string Token, ulong UserId, int Delay, bool savepicsnvids, bool savemessages, bool delete)
        {
            await Task.Run(async () =>
           {

               try
               {
                   Random rnd = new Random();
                   Status.SafeChangeText("Starting");
                   DiscordClient client = new DiscordClient(Token);
                   Logs.SafeAddItem(string.Format("Logged In To: {0}", client.User.Username));
                   var User = await client.GetProfileAsync(UserId);
                   var channelid = await client.CreateDMAsync(UserId);
                   Logs.SafeAddItem(string.Format("Created Dms With: {0}", User.User.Username));
                   var msg = await client.GetChannelMessagesAsync(channelid.Id);
                   Status.SafeChangeText("In Progress....");
                   var Convo = "SavedConvos/" + User.User.Username + rnd.Next(1, 999999999) + ".txt";
                   foreach (DiscordMessage message in msg)
                   {
                       if (savemessages)
                       {
                           
                           using (StreamWriter writetext = new StreamWriter(Convo, true))
                           {
                               writetext.WriteLine(message.Author.User.Username + " || " + message.Content);
                           }
                       }
                       if (savepicsnvids)
                       {
                           if (message.Attachment != null)
                           {
                               if (message.Attachment.FileName.Contains(".mp4") || message.Attachment.FileName.Contains(".jpg") || message.Attachment.FileName.Contains(".png") || message.Attachment.FileName.Contains(".webm") || message.Attachment.FileName.Contains(".png") || message.Attachment.FileName.Contains(".gif"))
                               {
                                   var webClient = new WebClient();
                                   webClient.DownloadFileCompleted += (sender, e) => Logs.SafeAddItem(string.Format("Saved Image/Video: {0}", message.Attachment.FileName)); ;
                                   webClient.DownloadFileAsync(new Uri(message.Attachment.Url), "SavedMedia/" + message.Attachment.FileName);
                               }
                           }
                       }
                       if (delete)
                       {
                           if (message.Author.User.Id == client.User.Id)
                           {
                               if (message.Type != MessageType.Call)
                               {
                                   await message.DeleteAsync();
                                   Logs.SafeAddItem(string.Format("Deleted Message: {0}", message.Content));
                                   await Task.Delay(Delay);
                               }
                           }
                       }
                   }
                   Status.SafeChangeText("Completed");
               } catch (Exception ex) {
                   //debug stuff
                   Logs.SafeAddItem(string.Format("Error Message: {0}", ex.Message));
                   await Task.Delay(Delay);
               }
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

                    Start(FormStart.Token, FormStart.UserId, FormStart.Delay, FormStart.SavePicsNVids, FormStart.SaveMessages, FormStart.Delete);
                }
            }
        }
    }
}
