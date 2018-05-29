using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FileServer
{
    class Program
    {
        private static int defaultPort = 3535;
        static void Main(string[] args)
        {
            TcpListener server = new TcpListener(IPAddress.Parse("127.0.0.1"), defaultPort);
            try
            {
                server.Start();
                Console.WriteLine("Сервер запущен. Ждет подключений...");
                TcpClient client = server.AcceptTcpClient();

                Console.WriteLine("Входящее соединение...");
                byte[] clientData = new byte[1024 * 5000];
                string receivedPath = "d:/";

                int receivedBytesLen = client.Client.Receive(clientData);
                int fileNameLen = BitConverter.ToInt32(clientData, 0);
                string fileName = Encoding.ASCII.GetString(clientData, 4, fileNameLen);

                BinaryWriter bWrite = new BinaryWriter(File.Open(receivedPath + fileName, FileMode.Append)); ;
                bWrite.Write(clientData, 4 + fileNameLen, receivedBytesLen - 4 - fileNameLen);

                Console.WriteLine("Файл: {0} получен & сохранен: {1}", fileName, receivedPath);
                bWrite.Close();
                client.Close();
                Console.ReadLine();
            }
            catch(SocketException ex)
            {
                Console.WriteLine("Error {0}" ,ex.Message);
            }
            finally
            {
                server.Stop();
            }
        }
    }
}
