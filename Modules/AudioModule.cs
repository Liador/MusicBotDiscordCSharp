using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace MusicBot.Modules
{
    public class AudioModule : ModuleBase<SocketCommandContext>
    {
        // Scroll down further for the AudioService.
        // Like, way down.
        // Hit 'End' on your keyboard if you still can't find it.
        private readonly AudioService _service;

        public AudioModule(AudioService service)
        {
            //Console.Write("Heyo");
            _service = service;
        }

        // You MUST mark these commands with 'RunMode.Async'
        // otherwise the bot will not respond until the Task times out.
        [Command("join", RunMode = RunMode.Async)]
        public async Task JoinCmd()
        {
            ulong channelID = 317035706319110155;
            //Console.Write("join\n");
            await _service.JoinAudio(Context.Guild, (Context.User as IVoiceState).VoiceChannel);
            Console.Write("I'm connected\n");
            //await _service.JoinAudio(Context.Guild, Context.Guild.GetVoiceChannel(channelID));
        }

        // Remember to add preconditions to your commands,
        // this is merely the minimal amount necessary.
        // Adding more commands of your own is also encouraged.
        [Command("leave", RunMode = RunMode.Async)]
        public async Task LeaveCmd()
        {
            //Console.Write("leave\n");
            await _service.LeaveAudio(Context.Guild);
        }

        [Command("play", RunMode = RunMode.Async)]
        public async Task PlayCmd([Remainder] string song)
        {
            //Console.Write("play\n");
            ulong channelID = 317035706319110155;
            //if (Context.Client.ConnectionState == ConnectionState.Connected)
            if (_service.IsConnected(Context.Guild) != null)
            {

            }
            else
            {
                await _service.JoinAudio(Context.Guild, (Context.User as IVoiceState).VoiceChannel);
            }
            await Context.Channel.SendMessageAsync(song);
            await _service.SendAudioAsync(Context.Guild, Context.Guild.GetChannel(channelID), song);

        }

        [Command("playy", RunMode = RunMode.Async)]
        public async Task PlayyCmd([Remainder] string song)
        {
            //Console.Write("play\n");
            ulong channelID = 317035706319110155;
            //if (Context.Client.ConnectionState == ConnectionState.Connected)
            if (_service.IsConnected(Context.Guild) != null)
            {

            }
            else
            {
                await _service.JoinAudio(Context.Guild, (Context.User as IVoiceState).VoiceChannel);
            }
            await Context.Channel.SendMessageAsync(song);
            await _service.SendAudioAsyncYTdirect(Context.Guild, Context.Guild.GetChannel(channelID), song);

        }

        [Command("stop", RunMode = RunMode.Async)]
        public async Task stop()
        {
             await _service.StopAudioAsync(Context.Guild);
        }

        [Command("test", RunMode = RunMode.Async)]
        public async Task test()
        {
            await Context.Channel.SendMessageAsync("Is it working?\n");
        }

        [Command("vidID", RunMode = RunMode.Async)]
        public async Task vidID(string ytUrl)
        {
            string videoID = ytUrl.Remove(0, "https://www.youtube.com/watch?v=".Length);
            videoID = videoID.Split('&')[0];
            await Context.Message.Channel.SendMessageAsync(videoID);
        }
    }
}
