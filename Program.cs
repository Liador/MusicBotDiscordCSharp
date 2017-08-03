using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

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
                using (StreamReader sr = new StreamReader("D:\\Token2.txt"))
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