using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Audio;

namespace MusicBot
{
    class Playlist
    {
        ConcurrentDictionary<Music, AudioOutStream> streams;
        ConcurrentQueue<Music> listToPlay;
        ConcurrentQueue<Music> listPlaying;
        Music muTest;
        public bool addStream(Music mu, AudioOutStream str)
        {
            bool succeed=streams.TryAdd(mu, str);
            if(succeed)
            {
                listPlaying.Enqueue(mu);
                listToPlay.TryDequeue(out muTest);
                succeed = mu == muTest;
            }
                
            return succeed;
        }
        public AudioOutStream getStream (Music mu)
        {
            AudioOutStream retVal;
            streams.TryGetValue(mu, out retVal);
            return retVal;
        }
        public Music First()
        {
            return listPlaying.First();
        }
        public void finishedSong(Music mu)
        {
            AudioOutStream retVal;
            bool succeed = streams.TryRemove(mu, out retVal);
            if (succeed)
                listPlaying.TryDequeue(out muTest);
            succeed = mu == muTest;
        }
        public Music GetCurrentSong()
        {
            return listPlaying.First();
        }
        public void AddSong(Music mu)
        {
            listToPlay.Enqueue(mu);
        }
    }
}
