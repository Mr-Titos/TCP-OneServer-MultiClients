using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections;

namespace ServerTcp
{
    class Program
    {
        const string SERVER_IP = "127.0.0.1";
        byte[] bytes = new byte[1024];
        static List<Socket> readList = new List<Socket>();
        static Socket ServerSocket = new Socket(AddressFamily.InterNetwork,
                      SocketType.Stream,
                      ProtocolType.Tcp);

        static int port = 0;
        static IPAddress localAdd = IPAddress.Parse(SERVER_IP);

        static byte[] saucisse = new byte[99999];

        static void Main(string[] args)
        {
            Console.Write("Port : ");
            port = Convert.ToInt32(Console.ReadLine());

            Thread start;
            start = new Thread(Start);
            start.Start();

            Thread list;
            list = new Thread(Listen);
            list.Start();

        }

        public static void Listen()
        {
            while (Thread.CurrentThread.IsAlive)
            {
                List<Socket> temp = readList;
                Thread.Sleep(10);
                foreach (Socket client in temp)
                {
                    try
                    {
                        if (!client.Connected)
                            continue;

                        string txt = "";
                        while (client.Available > 0)
                        {
                            byte[] bytes = new byte[client.ReceiveBufferSize];
                            int byteRec = client.Receive(bytes);
                            saucisse = bytes;
                            if (byteRec > 0)
                                txt += Encoding.UTF8.GetString(bytes, 0, byteRec);

                            if (!string.IsNullOrEmpty(txt))
                            {
                                Console.WriteLine(client.RemoteEndPoint + " : " + txt);
                                byte[] salt = Encoding.UTF8.GetBytes(txt);
                                foreach(Socket so in readList)
                                    so.Send(salt, salt.Length, SocketFlags.None);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
        }


        public static void Start()
        {
            IPHostEntry ipHostEntry = Dns.Resolve(Dns.GetHostName());
            IPAddress ipAddress = ipHostEntry.AddressList[0];
            Console.WriteLine("Server IP = " + ipAddress.ToString());
            Socket CurrentClient = null;
            
            try
            {
                ServerSocket.Bind(new IPEndPoint(localAdd, port));
                //Le 10 veut dire 10 clients max
                ServerSocket.Listen(10);

                while (true)
                {
                    CurrentClient = ServerSocket.Accept();
                    Console.WriteLine("New client:" + CurrentClient.RemoteEndPoint);
                    readList.Add(CurrentClient);
                }
            }
            catch (SocketException E)
            {
                Console.WriteLine(E.Message);
            }
        }
    }
}
        
