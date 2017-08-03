using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace MusicBot.Modules
{
    public class AudioModule : ModuleBase<SocketCommandContext>
    {
        private readonly AudioService _service;

        public AudioModule(AudioService service)
        {
            _service = service;
        }

        [Command("join", RunMode = RunMode.Async)]
        public async Task JoinCmd()
        {
            //ulong channelID = 317035706319110155;
            await _service.JoinAudio(Context.Guild, (Context.User as IVoiceState).VoiceChannel);
        }

        [Command("leave", RunMode = RunMode.Async)]
        public async Task LeaveCmd()
        {
            await _service.LeaveAudio(Context.Guild);
        }
        /** plays from local files
         * */
        [Command("playy", RunMode = RunMode.Async)]
        public async Task PlayCmd([Remainder] string song)
        {
            ulong channelID = 317035706319110155;
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
        /**plays from youtube url
         * */
        [Command("play", RunMode = RunMode.Async)]
        public async Task PlayyCmd([Remainder] string song)
        {
            //ulong channelID = 317035706319110155;
            if (_service.IsConnected(Context.Guild) != null)
            {
                
            }
            else
            {
                await _service.JoinAudio(Context.Guild, (Context.User as IVoiceState).VoiceChannel);
            }
            _service.AddToPlaylist(song);
            if(!_service.IsPlaying())
            {
                await _service.StartPlaying(Context.Guild, (Context.User as IVoiceState).VoiceChannel as SocketChannel);
            }
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

        /** Test to get the idd from a wyoutube link
         * */
        [Command("vidID", RunMode = RunMode.Async)]
        public async Task vidID(string ytUrl)
        {
            string videoID = ytUrl.Remove(0, "https://www.youtube.com/watch?v=".Length);
            videoID = videoID.Split('&')[0];
            await Context.Message.Channel.SendMessageAsync(videoID);
        }
    }
}
