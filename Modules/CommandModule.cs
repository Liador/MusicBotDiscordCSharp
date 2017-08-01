using Discord.Commands;
using System.Threading.Tasks;

namespace MusicBot.Modules
{
    public class CommandModule : ModuleBase<SocketCommandContext>
    {
        public CommandModule()
        {
        }

        [Command("help", RunMode = RunMode.Async)]
        public async Task join()
        {
            await Context.Channel.SendMessageAsync("Helping");
        }

        /**[Command("play", RunMode = RunMode.Async)]
        public async Task play(string url)
        {
                playlist.Enqueue(messages[1]);
                await message.Channel.SendMessageAsync("Song added to the playlist. " + playlist.Count() + " song(s) in the list");
                await Play(message);
            }
            IVoiceChannel channel = (Context.Message.Author as IGuildUser)?.VoiceChannel;
            await channel.ConnectAsync();
        }*/
    }
}
