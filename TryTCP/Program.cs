﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

    namespace Client
    {
        class Program
        {
            static Socket ClientSocket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);

            static void Main(string[] args)
            {
                Console.WriteLine("Veiller a ce que le serveur soit allumé avant d'entrer le port");
                String SERVER_IP = "127.0.0.1";
                Console.Write("Port : ");
                int port = Convert.ToInt32(Console.ReadLine());

                Thread Listen;
                Listen = new Thread(Receive);
                Listen.Start();
      
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = IPAddress.Parse(SERVER_IP);

                ClientSocket.Connect(ipAddress,port);

                while (true)
                {
                    Console.Write("Send What ?  -- ");
                    string textToSend = Console.ReadLine();

                    byte[] bytesToSend = Encoding.ASCII.GetBytes(textToSend);

                    Console.WriteLine("Sending : " + textToSend);
                    ClientSocket.Send(bytesToSend,bytesToSend.Length,SocketFlags.None);
                    Thread.Sleep(100);
                    
                }
            }

            static void Receive()
            {
                while (Thread.CurrentThread.IsAlive)
                {
                    if (!ClientSocket.Connected)
                        continue;

                    byte[] bytesToRead = new byte[ClientSocket.ReceiveBufferSize];

                    byte[] msg = new Byte[ClientSocket.Available];
                    ClientSocket.Receive(msg, 0, ClientSocket.Available, SocketFlags.None);

                    String messageReceived = Encoding.UTF8.GetString(msg);
                    if (messageReceived.Length > 0)
                        Console.WriteLine("Receive : " + messageReceived );
                }
            }

        }
    }


