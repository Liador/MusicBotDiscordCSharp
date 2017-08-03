using Discord;
using Discord.Audio;
using Discord.WebSocket;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using YoutubeExplode;

namespace MusicBot
{
    public class AudioService
    {
        private readonly ConcurrentDictionary<ulong, IAudioClient> ConnectedChannels = new ConcurrentDictionary<ulong, IAudioClient>();
        private AudioOutStream stream;
        private ConcurrentQueue<Music> playlist;
        private ulong BotLogChannelID;
        private SocketGuild sGuild;
        private YoutubeClient ytClient;
        //private YoutubeExplode.Models.VideoInfo videoInfos
        private bool isPlaying;

        public AudioService()
        {
            playlist = new ConcurrentQueue<Music>();
            BotLogChannelID = 340938722830712842;
            isPlaying = false;
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
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }          
        }

        public async Task LeaveAudio(IGuild guild)
        {
            IAudioClient client;
            if (ConnectedChannels.TryRemove(guild.Id, out client))
            {
                await client.StopAsync();
            }
        }

        internal async Task SendAudioAsync(SocketGuild guild, SocketChannel socketChannel, string song)
        {
            if (!File.Exists(song))
            {
                //await guild.TextChannels.SendMessageAsync("File does not exist.");
                Console.Write("File does not exist.");
                return;
            }

            IAudioClient client = IsConnected(guild);
            if (client != null)
            {
                var output = CreateStream(song).StandardOutput.BaseStream;
                stream = client.CreatePCMStream(AudioApplication.Music);
                await output.CopyToAsync(stream);
                await stream.FlushAsync().ConfigureAwait(false);
            }
            else
            {
                await JoinAudio(guild, socketChannel as IVoiceChannel);
            }
            Console.WriteLine(client);
        }

        internal async Task SendAudioAsyncYTdirect(SocketGuild guild, SocketChannel socketChannel, string song)
        {
            string videoID = ParseYoutubeID(song);
            ytClient = new YoutubeClient();
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
                    var output = CreateStreamYTDirect(song).StandardOutput.BaseStream;
                    stream = client.CreatePCMStream(AudioApplication.Music, 96*1024,2000,30);
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
            Console.WriteLine("stopped \n");
            await guild.GetTextChannel(BotLogChannelID).SendMessageAsync("Hey");
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
            playlist.Enqueue(new Music(song));
            Console.WriteLine(playlist.Count.ToString() + " songs in the list");
        }

        public bool IsPlaying()
        {
            return isPlaying;
        }

        public async Task StartPlaying(SocketGuild guild, SocketChannel socketChannel)
        {
            isPlaying = true;
            bool hasNext = true;
            Music mu = new Music("");
            hasNext=  playlist.TryDequeue(out mu);
            while(hasNext)
            {
                await SendAudioAsyncYTdirect(guild, socketChannel, mu.getPathFile());
                hasNext = playlist.TryDequeue(out mu);
                Console.WriteLine("Next song");
            }
            isPlaying = false;
        }
    }
}
