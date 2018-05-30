using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FileServerAsync
{
    public class SocketServer
    {
        private static TcpListener serverSocket;
        private static int defaultPort = 3535;
        public static void StartServer()
        {
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), defaultPort);
            serverSocket = new TcpListener(ipEndPoint);
            Console.WriteLine("Асинхронные сервер начал работу : " + ipEndPoint.Address.ToString());
            while (true)
            {
                serverSocket.Start();
                WaitForClients();
            }
        }
        private async void HandleConnectionAsync(TcpClient tcpClient)
        {
            TcpClient client = await serverSocket.AcceptTcpClientAsync();
        }
        private static void WaitForClients()
        {
            serverSocket.BeginAcceptTcpClient(new System.AsyncCallback(OnClientConnected), null);
        }
        private static void OnClientConnected(IAsyncResult asyncResult)
        {
            try
            {
                TcpClient clientSocket = serverSocket.EndAcceptTcpClient(asyncResult);
                if (clientSocket != null)
                    Console.WriteLine("Входящее подключение от: " + clientSocket.Client.RemoteEndPoint.ToString());
                HandleClientRequest(clientSocket);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error OnClientConnected : {0}",ex.Message);
            }
            WaitForClients();
        }

        private static void HandleClientRequest(TcpClient clientSocket)
        {
            byte[] clientData = new byte[1024 * 10000]; // размер файла
            string receivedPath = @"D:\";

            int receivedBytesLen = clientSocket.Client.Receive(clientData);
            int fileNameLen = BitConverter.ToInt32(clientData, 0);
            string fileName = Encoding.ASCII.GetString(clientData, 4, fileNameLen);

            BinaryWriter bWrite = new BinaryWriter(File.Open(receivedPath + fileName, FileMode.Append)); ;
            bWrite.Write(clientData, 4 + fileNameLen, receivedBytesLen - 4 - fileNameLen);

            Console.WriteLine("Файл: {0} получен и сохранен: {1}", fileName, receivedPath);
            bWrite.Close();
            clientSocket.Close();
            Console.ReadLine();
        }
    }
}
