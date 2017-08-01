using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using Discord.WebSocket;
using System.Collections.Generic;
using YoutubeExplode;
using System.Linq;
using YoutubeExplode.Models;

namespace MusicBot
{
    public class AudioService
    {
        private readonly ConcurrentDictionary<ulong, IAudioClient> ConnectedChannels = new ConcurrentDictionary<ulong, IAudioClient>();
        private AudioOutStream stream;
        private List<string> playlist;
        private ulong BotLogChannelID;
        private SocketGuild sGuild;
        private YoutubeClient ytClient;
        private YoutubeExplode.Models.VideoInfo videoInfos;

        public AudioService()
        {
            
        }
        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
        public async Task JoinAudio(IGuild guild, IVoiceChannel target)
        {
            IAudioClient client;
            sGuild = guild as SocketGuild;
            if (ConnectedChannels.TryGetValue(guild.Id, out client))
            {
                return;
            }
            if (target.Guild.Id != guild.Id)
            {
                return;
            }
            try
            {
                var audioClient = await target.ConnectAsync();
                if (ConnectedChannels.TryAdd(guild.Id, audioClient))
                {
                    Console.WriteLine($"Connected to voice on {guild.Name}.");
                    //await Log(LogSeverity.Info, $"Connected to voice on {guild.Name}.");
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                //Console.WriteLine(e.Data);
            }
            
        }

        public async Task LeaveAudio(IGuild guild)
        {
            IAudioClient client;
            if (ConnectedChannels.TryRemove(guild.Id, out client))
            {
                await client.StopAsync();
                //await Log(LogSeverity.Info, $"Disconnected from voice on {guild.Name}.");
            }
        }
        /**public async Task SendAudioAsync(IGuild guild, IMessageChannel channel, string path)
        {
            // Your task: Get a full path to the file if the value of 'path' is only a filename.
            //Console.Write("Async public SendAudioAsync");
            if (!File.Exists(path))
            {
                await channel.SendMessageAsync("File does not exist.");
                return;
            }
            IAudioClient client;
            if (ConnectedChannels.TryGetValue(guild.Id, out client))
            {
                //await Log(LogSeverity.Debug, $"Starting playback of {path} in {guild.Name}");
                var output = CreateStream(path).StandardOutput.BaseStream;

                // You can change the bitrate of the outgoing stream with an additional argument to CreatePCMStream().
                // If not specified, the default bitrate is 96*1024.
                stream = client.CreatePCMStream(AudioApplication.Music);
                await output.CopyToAsync(stream);
                await stream.FlushAsync().ConfigureAwait(false);
            }
            ytClient = new YoutubeClient();
            
            videoInfos = await ytClient.GetVideoInfoAsync(path);
            if (videoInfos!=null)
            {

            }
        }*/

        internal async Task SendAudioAsync(SocketGuild guild, SocketChannel socketChannel, string song)
        {
            //Console.Write("Playing music");
            // Your task: Get a full path to the file if the value of 'path' is only a filename.
            if (!File.Exists(song))
            {
                //await guild.TextChannels.SendMessageAsync("File does not exist.");
                Console.Write("File does not exist.");
                return;
            }

            IAudioClient client = IsConnected(guild);
            if (client != null)
            {
                //Console.WriteLine("If in SendAudioAsync");
                //await Log(LogSeverity.Debug, $"Starting playback of {path} in {guild.Name}");
                var output = CreateStream(song).StandardOutput.BaseStream;
                //var output = await YoutubeExplode.Services.Extensions.GetStreamAsync(, song);

                // You can change the bitrate of the outgoing stream with an additional argument to CreatePCMStream().
                // If not specified, the default bitrate is 96*1024.
                stream = client.CreatePCMStream(AudioApplication.Music);
                await output.CopyToAsync(stream);
                await stream.FlushAsync().ConfigureAwait(false);
            }
            else
            {
                await JoinAudio(guild, socketChannel as IVoiceChannel);
            }
            //Console.WriteLine("end of sendAUdioAsync");
            Console.WriteLine(client);
            //return guild.DefaultChannel.SendMessageAsync("Playing music");
            //throw new NotImplementedException();
        }

        internal async Task SendAudioAsyncYTdirect(SocketGuild guild, SocketChannel socketChannel, string song)
        {
            string videoID = ParseYoutubeID(song);
            ytClient = new YoutubeClient();


            //Console.Write("Playing music");
            // Your task: Get a full path to the file if the value of 'path' is only a filename.
            /**if (!File.Exists(song))
            {
                //await guild.TextChannels.SendMessageAsync("File does not exist.");
                Console.Write("File does not exist.");
                return;
            }*/
            bool exists = await ytClient.CheckVideoExistsAsync(videoID);
            if (!exists)
            {
                Console.WriteLine("video doesn't exist");
                return;
            }
            try
            {
                IAudioClient client = IsConnected(guild);
                if (client != null)
                {
                    Console.WriteLine("If in SendAudioAsync");
                    //await Log(LogSeverity.Debug, $"Starting playback of {path} in {guild.Name}");
                    var output = CreateStreamYTDirect(song).StandardOutput.BaseStream;
                    //var output = await YoutubeExplode.Services.Extensions.GetStreamAsync(, song);

                    // You can change the bitrate of the outgoing stream with an additional argument to CreatePCMStream().
                    // If not specified, the default bitrate is 96*1024.
                    stream = client.CreatePCMStream(AudioApplication.Music);
                    await output.CopyToAsync(stream);
                    await stream.FlushAsync().ConfigureAwait(true);
                }
                else
                {
                    Console.WriteLine("joining in audioservice");
                    await JoinAudio(guild, socketChannel as IVoiceChannel);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            

            /**videoInfos = await ytClient.GetVideoInfoAsync(videoID);
            var streamInfos = videoInfos.AudioStreams.OrderBy(s => s.AudioEncoding).Last();
            IAudioClient aClient;
            if (ConnectedChannels.TryGetValue(guild.Id, out aClient))
            {
                string fileExtension = streamInfos.Container.GetFileExtension();
                string fileName = $"{videoInfos.Id}.{streamInfos.AudioEncoding}.{fileExtension}";
                using (var input = await ytClient.GetMediaStreamAsync(streamInfos))
                using (var output = File.Create(fileName))
                {
                    await input.CopyToAsync(output);
                    stream = aClient.CreatePCMStream(AudioApplication.Music);
                    await output.FlushAsync().ConfigureAwait(false);
                }

            }


            /**IAudioClient client;
            if (ConnectedChannels.TryGetValue(guild.Id, out client))
            {
                //Console.WriteLine("If in SendAudioAsync");
                //await Log(LogSeverity.Debug, $"Starting playback of {path} in {guild.Name}");
                var output = CreateStream(song).StandardOutput.BaseStream;
                //var output = await YoutubeExplode.Services.Extensions.GetStreamAsync(, song);

                // You can change the bitrate of the outgoing stream with an additional argument to CreatePCMStream().
                // If not specified, the default bitrate is 96*1024.
                stream = client.CreatePCMStream(AudioApplication.Music);
                await output.CopyToAsync(stream);
                await stream.FlushAsync().ConfigureAwait(false);
            }
            else
            {
                await JoinAudio(guild, socketChannel as IVoiceChannel);
            }
            //Console.WriteLine("end of sendAUdioAsync");
            Console.WriteLine(client);
            //return guild.DefaultChannel.SendMessageAsync("Playing music");
            //throw new NotImplementedException();*/
        }

        internal async Task StopAudioAsync(SocketGuild guild)
        {
            
            if (stream != null)
            {
                stream.Close();
                stream = null;
            }
            else
            {
                
            }
            
            //Console.WriteLine("end of sendAUdioAsync");
            Console.WriteLine("stopped \n");
            await guild.GetTextChannel(BotLogChannelID).SendMessageAsync("Hey");
            //throw new NotImplementedException();
        }

        private Process CreateStreamYTDirect(string path)
        {
            Process currentsong = new Process();

            currentsong.StartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/C youtube-dl.exe -o - {path} | ffmpeg -i pipe:0 -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            currentsong.Start();
            return currentsong;
        }

        private Process CreateStream(string path)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg.exe",
                Arguments = $"-hide_banner -loglevel panic -i \"{ path} \" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true
            });
            
        }

        private string ParseYoutubeID(string ytUrl)
        {
            string videoID = ytUrl.Remove(0, "https://www.youtube.com/watch?v=".Length);
            videoID = videoID.Split('&')[0];
            return videoID;
        }
        internal IAudioClient IsConnected(SocketGuild guild)
        {
            IAudioClient client;
            if (ConnectedChannels.TryGetValue(guild.Id, out client))
            {
                return client;
            }
            else
            {
                return null;
            }
        }
        public void AddToPlaylist(string song)
        {
            playlist.Add(song);
            Console.WriteLine(playlist.Count.ToString() + " songs in the list\n");
        }
    }
}
