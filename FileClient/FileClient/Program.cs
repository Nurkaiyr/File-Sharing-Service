using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FileClient
{
    class Program
    {
        private static int defaultPort = 3535;
        static void Main(string[] args)
        {
            TcpClient client = new TcpClient();
            client.Connect(IPAddress.Parse("127.0.0.1"), defaultPort);
            try
            {
                Console.WriteLine("Клиент запущен...");

                try
                {
                    string fileName = "2.zip";
                    string filePath = @"C:\Users\едиген\Downloads\";

                    byte[] fileNameByte = Encoding.ASCII.GetBytes(fileName);
                    byte[] fileData = File.ReadAllBytes(filePath + fileName);
                    byte[] clientData = new byte[4 + fileNameByte.Length + fileData.Length];
                    byte[] fileNameLen = BitConverter.GetBytes(fileNameByte.Length);

                    fileNameLen.CopyTo(clientData, 0);
                    fileNameByte.CopyTo(clientData, 4);
                    fileData.CopyTo(clientData, 4 + fileNameByte.Length);

                    client.Client.Send(clientData);

                    Console.WriteLine("Файл отправлен: {0}",fileName);
                    client.Client.Close();
                    Console.ReadLine();
                    //client.Client.SendFile(path);

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Загрузка не удалась, {0}", ex.Message);
                }
                Console.ReadLine();
                client.Close();
            }
            catch(SocketException ex)
            {
                Console.WriteLine("Error: {0}",ex.Message);
            }
            finally
            {
                client.Close();
            }
            Console.WriteLine("Завершение работы...");
            Console.ReadLine();
        }
    }
}
