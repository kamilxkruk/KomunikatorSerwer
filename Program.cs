using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KomunikatorSerwer.Model;

namespace KomunikatorSerwer
{
    class Program
    {

        static List<Model.Client> onlinePeople = new List<Model.Client>();

        static void Main(string[] args)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, 8080);
            listener.Start();

            Console.WriteLine("Server started...");

            while (true)
            {
                try
                {
                    Console.WriteLine("Waiting for incoming client connections...");
                    TcpClient client = listener.AcceptTcpClient();
                    Console.WriteLine("Accepted new client connection...");

                    Thread t = new Thread(ProcessClientRequests);
                    t.Start(client);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

        }

        private static void ProcessClientRequests(object obj)
        {
            TcpClient client = (TcpClient) obj;
            string clientsIp = client.Client.RemoteEndPoint.ToString();
            clientsIp = clientsIp.Substring(0, clientsIp.LastIndexOf(":"));

            Console.WriteLine($"Connection created for ip {clientsIp}");
            StreamReader reader = new StreamReader(client.GetStream());
            StreamWriter writer = new StreamWriter(client.GetStream());

            onlinePeople.Add(new Client(clientsIp, writer));

            try
            {
                while (true)
                {
                    var rawMessage = reader.ReadLine();
                    if (!rawMessage.Contains("#"))
                    {
                        writer.WriteLine("Wrong message format");
                        continue;
                    }
                    var recipientIp = rawMessage.Substring(0, rawMessage.IndexOf("#"));
                    var message = rawMessage.Substring(rawMessage.IndexOf("#") + 1);


                    var recipient = onlinePeople.FirstOrDefault(c => c.ipAdress == recipientIp);
                    if (recipient == null)
                    {
                        writer.WriteLine("Wrong ip address or your colleague is offline, sorry");
                        writer.Flush();
                        continue;
                    }
                    var recipientWriter = recipient.writer;
                    recipientWriter.WriteLine($"Message from {clientsIp}: {message}");
                    recipientWriter.Flush();
                }
            }
            catch (Exception e)
            {
                var clientToRemove = onlinePeople.FirstOrDefault(c => c.ipAdress == clientsIp);
                onlinePeople.Remove(clientToRemove);
                reader.Close();
                writer.Close();
                client.Close();
            }
        }
    }
}
