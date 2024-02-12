using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

//API
using System.Net.Http;
using System.Linq;

namespace BGLeopold
{
    class Program
    {
        private DiscordSocketClient client;
        private CommandService commands;
        private const string botToken = "MTAxNzc3OTE2ODQ1NDM5Nzk2Mw.G0a08D.AAH4bcqGd1p_c1FnPWL1pD2qWW7YNScssyxbFg";//enter ther token here
        private const ulong channelId = 886903917076688916;
        private const ulong privateChannelId = 1071852578054357033;
        private IMessageChannel edtChanel;
        private IMessageChannel privateChannel;
        private IServiceProvider services;
        private EDTWeekData lastEDTSend;
        private static HttpClient httpClient;

        //Param
        private float hourBeforeSendindEDT = 48f;

        static void Main(string[] args)
        {
            new Program().RunBotAsync().GetAwaiter().GetResult();
        }

        public void Print(object msg) => Console.WriteLine(msg);

        public async Task RunBotAsync()
        {
            httpClient = new HttpClient();

            client = new DiscordSocketClient(new DiscordSocketConfig
            {
                //LogLevel = LogSeverity.Debug
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
            });
            commands = new CommandService();
            //services = new ServiceCollection().AddSingleton(client).AddSingleton(commands).BuildServiceProvider();

            client.Log += ClientLog;

            //1er chose que va appeler le client
            client.Ready += () =>
            {
                Print("Ready to go!");
                return Task.CompletedTask;
            };

            //On installe les commandes
            await InstallCommandsAsync();

            await client.LoginAsync(TokenType.Bot, botToken);
            await client.StartAsync();

            await Task.Delay(-1);
        }

        public async Task InstallCommandsAsync()
        {
            client.MessageReceived += HandleChannelMessageAsync;
            client.Ready += ClientReady;
            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);//replace null per services
        }

        private int CalculateWeekOffset()
        {
            TimeSpan delta = DateTime.Now - new DateTime(2022, 9, 12, 0, 0, 0);//première semaine de l'année
            return (int)Math.Floor((delta.TotalDays + hourBeforeSendindEDT / 24f) / 7f);
        }

        private async Task ClientReady()
        {
            edtChanel = client.GetChannel(channelId) as IMessageChannel;
            privateChannel = client.GetChannel(privateChannelId) as IMessageChannel;
            if (edtChanel == null)
            {
                Console.WriteLine("The EDT channel wasn't found");
            }

            //SUPER TEST
            for (int i = 0; i < URLData.urlsPerWeek.Length; i++)
            {
                break;//comment to proceed the test
                string s = await GetEDTDataWeekAsync(URLData.urlsPerWeek[i]);
                lastEDTSend = EDTWeekData.ParseICSStringToEDTWeekData(s);
                string path = lastEDTSend.GenerateImage();
                await SendImage(path);
            }

            int weekOffset, oldWeekOffset = CalculateWeekOffset();
            string edtData = await GetEDTDataWeekAsync(URLData.urlsPerWeek[oldWeekOffset]);
            lastEDTSend = EDTWeekData.ParseICSStringToEDTWeekData(edtData);
            EDTWeekData edt = EDTWeekData.Load("lastEDT.json");
            string imagePath = string.Empty;

            if (ReferenceEquals(edt, null) || edt != lastEDTSend)
            {
                imagePath = lastEDTSend.GenerateImage();
                await SendImage(imagePath);
                lastEDTSend.Save("lastEDT.json");
            }

            while (true)
            {
                await Task.Delay(300000);//wait 5 min

                weekOffset = CalculateWeekOffset();
                edtData = await GetEDTDataWeekAsync(URLData.urlsPerWeek[weekOffset]);//.GetAwaiter().GetResult();//NE PAS OUBLIER LE @!!!!!
                edt = EDTWeekData.ParseICSStringToEDTWeekData(edtData);
                Console.WriteLine("New request to the university ! " + DateTime.Now.ToString());

                //si on change de semaine
                if(weekOffset > oldWeekOffset)
                {
                    //Generate image and send EDT to the disord's server
                    imagePath = edt.GenerateImage();
                    await SendImage(imagePath);
                    edt.Save("lastEDT.json");
                    lastEDTSend = edt;
                }
                else
                {
                    //Si un changement d'EDT s'est produit
                    if(edt != lastEDTSend)
                    {
                        //TODO : Generate image and send EDT to the disord's server
                        imagePath = edt.GenerateImage();
                        await SendImage(imagePath);
                        edt.Save("lastEDT.json");
                        lastEDTSend = edt;
                    }
                }

                oldWeekOffset = weekOffset;
            }
        }

        private async Task SendImage(string path)
        {
            FileAttachment file = new FileAttachment(path);
            await edtChanel.SendFileAsync(file);
            //await privateChannel.SendFileAsync(file);

            await Task.Delay(100);
        }

        private static async Task<string> GetEDTDataWeekAsync(string url)
        {
            string data = string.Empty;
            HttpResponseMessage response = await httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }
            return @"" + data;
        }

        private async Task SendEDT(int weekOffset)
        {
            string edtData = await GetEDTDataWeekAsync(URLData.urlsPerWeek[weekOffset]);//.GetAwaiter().GetResult();//NE PAS OUBLIER LE @!!!!!
            EDTWeekData edt = EDTWeekData.ParseICSStringToEDTWeekData(edtData);
            string imagePath = edt.GenerateImage();
            await SendImage(imagePath);
        }

        private async Task HandleChannelMessageAsync(SocketMessage arg)
        {
            SocketUserMessage message = (SocketUserMessage)arg;
            if (message == null || message.Author.IsBot || (message.Channel.Id != channelId && message.Channel.Id != privateChannelId))
                return;

            string textMessage = message.ToString();
            if(textMessage.StartsWith("!getEDT"))
            {
                int offset = CalculateWeekOffset();
                string tmp = textMessage.GetSubstringsBetweenPatterns("week:", ";").FirstOrDefault();
                if(tmp != default(string))
                {
                    if (tmp.Contains(".."))
                    {
                        int start, end;
                        string startStr = tmp[0].ToString() + tmp.GetSubstringsBetweenPatterns(tmp[0].ToString(), "..").FirstOrDefault();
                        string endStr = tmp.GetSubstringsBetweenPatterns("..", tmp[tmp.Length -1].ToString()).FirstOrDefault() + tmp[tmp.Length - 1].ToString();
                        if(startStr == default(string) || endStr == default(string))
                        {
                            start = end = 0;
                        }
                        else
                        {
                            start = startStr.ToInt();
                            end = endStr.ToInt();
                            if(start > end)
                            {
                                start = end = 0;
                            }
                        }

                        for (int i = start; i <= end; i++)
                        {
                            await SendEDT(i + offset);
                        }
                    }
                    else
                    {
                        int week = tmp.ToInt();
                        await SendEDT(week + offset);
                    }
                }
            }

            /*
            var context = new SocketCommandContext(client, message);

            Print("Message : " + message.ToString());
            Print("CONTENT : " + message.Content + "AUTOR : " + message.Author);

            int argPos = 0;
            if (!message.HasStringPrefix("!", ref argPos))
                return;

            var result = await commands.ExecuteAsync(context, argPos, null);//replace null per services

            //Error
            if (!result.IsSuccess)
                await context.Channel.SendMessageAsync("Error : " + result.ErrorReason);
            */
        }

        private Task ClientLog(LogMessage arg)
        {
            Print(arg);
            return Task.CompletedTask;
        }
    }
}
