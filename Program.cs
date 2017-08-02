using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Diagnostics;
using Discord.Audio;
using YoutubeExplode;
using System.IO;
using Microsoft.Extensions.DependencyInjection;

namespace MusicBot
{
    class Program
    {
        public DiscordSocketClient _client;
        public string commandPrefix = "/";
        private Queue<string> playlist = new Queue<string>();
        /**private IAudioClient _audioClient;
        private AudioOutStream stream;
        private Stream output;*/
        private CommandHandler handler;



        public static void Main(string[] args)
           => new Program().MainAsync().GetAwaiter().GetResult();



        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            handler = new CommandHandler(_client);
            
            _client.Log += Log;


            await handler.InitCommands();
            string token = ""; // Remember to keep this private!
            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader("D:\\Token.txt"))
                {
                    // Read the stream to a string, and write the string to the console.
                    String line = sr.ReadToEnd();
                    //Console.WriteLine(line);
                    token = line;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            

            //_client.MessageReceived += MessageReceived;

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }
    }
}



        

        /**private async Task MessageReceived(SocketMessage message)
        {
            
            //if (message.Channel.Id == 323947206044286977)
            //{
                //await message.Channel.SendMessageAsync(message.Channel.Id.ToString());
                if (message.Author.Id != 327068085087371265)
                {
                    if (message.Content.StartsWith(commandPrefix))
                    {
                        string contentTemp = message.Content;
                        foreach (char c in commandPrefix)
                        {
                            contentTemp = contentTemp.TrimStart(c);
                            await message.Channel.SendMessageAsync(".");
                        }
                        
                        string[] messages = new string[contentTemp.Split(' ').Count<string>()];
                        /**await message.Channel.SendMessageAsync(contentTemp.Split(' ').Count<string>().ToString());
                        await message.Channel.SendMessageAsync(contentTemp);
                        messages = contentTemp.Split(' ');
                        await message.Channel.SendMessageAsync("0");
                        if (messages[0] == "play")
                        {
                            if (messages.Length <= 1)
                            {
                                await message.Channel.SendMessageAsync("You must provide an Url for a youtube song");
                            }
                            else
                            {
                                playlist.Enqueue(messages[1]);
                                await message.Channel.SendMessageAsync("Song added to the playlist. " + playlist.Count() + " song(s) in the list");
                                await Play(message);
                            }
                        }
                        if (messages[0] == "ping")
                        {
                            await message.Channel.SendMessageAsync("Pong!");
                        }
                        await message.Channel.SendMessageAsync("1");
                        if (messages[0] == "join")
                        {
                            await JoinChannel(message);
                        }
                        await message.Channel.SendMessageAsync("2");
                        /**if (message.Content == "leave")
                        {
                            await LeaveChannel();
                        }
                        if (messages[0] == "kill")
                        {
                           
                        }
                    }
            }
            //}

        }

        public async Task JoinChannel(SocketMessage message, IVoiceChannel channel = null)
        {
            // Get the audio channel
            channel = channel ?? (message.Author as IGuildUser)?.VoiceChannel;
            if (channel == null) { await message.Channel.SendMessageAsync("User must be in a voice channel, or a voice channel must be passed as an argument."); return; }

            // For the next step with transmitting audio, you would want to pass this Audio Client in to a service.
            await message.Channel.SendMessageAsync("connecting");
            try
            {
                _audioClient = await channel.ConnectAsync();
            }
            catch(Exception e)
            {
                await message.Channel.SendMessageAsync(e.Message);
            }
            await message.Channel.SendMessageAsync("connected");
        }

        public async Task Play(SocketMessage message, IVoiceChannel channel = null)
        {
            await message.Channel.SendMessageAsync("Play1");
            if (_audioClient == null)
            {
                await message.Channel.SendMessageAsync("Play10");
                await JoinChannel(message);
                await message.Channel.SendMessageAsync("Play101");
            }
            await message.Channel.SendMessageAsync("Play11");
            Music song = new Music(playlist.Dequeue());
            await message.Channel.SendMessageAsync(song.getPathFile());
            if (song == null)
            {
                await message.Channel.SendMessageAsync("The playlist is empty.");
            }
            else
            {
                await message.Channel.SendMessageAsync("Play2");
                if (stream == null)
                {
                    await message.Channel.SendMessageAsync("Play3");
                    output = CreateStream(song.getPathFile()).StandardOutput.BaseStream;
                    stream = _audioClient.CreatePCMStream(AudioApplication.Music, 128 * 1024);
                    await output.CopyToAsync(stream);
                    await stream.FlushAsync().ConfigureAwait(false);
                }

            }
        }
        private Process CreateStream(string url)
        {
            Process currentsong = new Process();

            currentsong.StartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/C youtube-dl.exe -o - {url} | ffmpeg -i pipe:0 -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            currentsong.Start();
            return currentsong;
        }
    }
}
*/