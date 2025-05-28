using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;

class Oop_lab17
{
    private TcpListener listener;
    private List<TcpClient> clients = new List<TcpClient>();

    public void Start()
    {
        listener = new TcpListener(IPAddress.Any, 8888);
        listener.Start();
        Console.WriteLine("Сервер запущено. Очікування підключень...");

        while (true)
        {
            TcpClient client = listener.AcceptTcpClient();
            clients.Add(client);
            Thread clientThread = new Thread(() => HandleClient(client));
            clientThread.Start();
        }
    }

    private void HandleClient(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[1024];
        int byteCount;
        try
        {
            while ((byteCount = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                string message = Encoding.UTF8.GetString(buffer, 0, byteCount);
                BroadcastMessage(message, client);
                Console.WriteLine(message);
            }
        }
        catch
        {
            // Обробка помилок
        }
        finally
        {
            clients.Remove(client);
            client.Close();
        }
    }

    private void BroadcastMessage(string message, TcpClient sender)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(message);
        foreach (var client in clients)
        {
            if (client != sender)
            {
                NetworkStream stream = client.GetStream();
                stream.Write(buffer, 0, buffer.Length);
            }
        }
    }
}
