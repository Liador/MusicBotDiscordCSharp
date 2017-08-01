using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicBot
{
    class Music
    {
        string pathFile;

        public Music (string path)
        {
            pathFile = path;
        }
        public string getPathFile()
        {
            return pathFile;
        }
    }
}
