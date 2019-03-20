using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KomunikatorSerwer.Model
{
    class Client
    {
        public Client(string ipAdress, StreamWriter streamWriter)
        {
            this.ipAdress = ipAdress;
            this.writer = streamWriter;
        }

        public string ipAdress { get; }
        public StreamWriter writer { get; }

    }
}
