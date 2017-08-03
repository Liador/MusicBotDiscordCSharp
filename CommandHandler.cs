using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace MusicBot
{
    class CommandHandler
    {
        private DiscordSocketClient _client;
        private CommandService _service;
        private readonly IServiceCollection _map = new ServiceCollection();
        private IServiceProvider _services = new ServiceCollection().BuildServiceProvider();
        private CommandService _commands = new CommandService();
        public AudioService audioService;

        public CommandHandler(DiscordSocketClient client)
        {
            _client = client;

            _service = new CommandService();
            _service.Log += Log;

            _service.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private async Task HandleCommandAsync(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            if (msg == null) return;
            /**string modulesNames = "";
            foreach (ModuleInfo m in _service.Modules)
            {
                modulesNames += m.Name;
            }*/
            
            //await s.Channel.SendMessageAsync("executing command");
            try
            {
                if(!s.Author.IsBot)
                {
                    //await s.Channel.SendMessageAsync(_service.Modules.Count().ToString());
                    var context = new SocketCommandContext(_client, msg);
                    

                    int argPos = 0;
                    if (msg.HasStringPrefix("$", ref argPos))
                    {
                        //await s.Channel.SendMessageAsync("." + context.Message + "");
                        //await context.Channel.SendFileAsync(new Uri("c://").ToString());

                        var result = await _service.ExecuteAsync(context, argPos, _services);

                        if (!result.IsSuccess /*&& result.Error != CommandError.UnknownCommand*/)
                        {
                            await context.Channel.SendMessageAsync(result.ErrorReason);
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Console.Write(e.Message);
            }
        }
        public async Task InitCommands()

        {

            // Repeat this for all the service classes

            // and other dependencies that your commands might need.
            //_map.AddSingleton(new Modules.AudioModule(audioService));
            //_map.AddSingleton(new Modules.Test());
            _map.AddSingleton(audioService = new AudioService());




            // When all your required services are in the collection, build the container.

            // Tip: There's an overload taking in a 'validateScopes' bool to make sure

            // you haven't made any mistakes in your dependency graph.

            _services = _map.BuildServiceProvider();



            // Either search the program and add all Module classes that can be found.

            // Module classes *must* be marked 'public' or they will be ignored.

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());

            // Or add Modules manually if you prefer to be a little more explicit:

            //await _commands.AddModuleAsync<SomeModule>();



            // Subscribe a handler to see if a message invokes a command.

            _client.MessageReceived += HandleCommandAsync;

        }
    }
}
