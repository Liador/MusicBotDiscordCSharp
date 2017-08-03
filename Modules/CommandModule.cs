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

        [Command("servid", RunMode = RunMode.Async)]
        public async Task servid()
        {
            await Context.Channel.SendMessageAsync("Server ID: " + 94773780315910144.ToString());
            await Context.Channel.SendMessageAsync(Context.Guild.Id.ToString());
            
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
