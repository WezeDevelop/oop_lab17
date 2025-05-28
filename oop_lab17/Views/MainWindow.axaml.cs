using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace oop_lab17.Views
{
    public partial class MainWindow : Window
    {
        private TcpClient client;
        private NetworkStream stream;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public MainWindow()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        {
            InitializeComponent();
            ConnectToServer();
        }

        private void ConnectToServer()
        {
            try
            {
                client = new TcpClient("127.0.0.1", 8888);
                stream = client.GetStream();
                Thread receiveThread = new Thread(ReceiveMessages);
                receiveThread.Start();
            }
            catch (Exception ex)
            {
                ChatBox.Text += $"Помилка підключення: {ex.Message}\n";
            }
        }

        private void ReceiveMessages()
        {
            byte[] buffer = new byte[1024];
            int byteCount;
            try
            {
                while ((byteCount = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, byteCount);
                    Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        ChatBox.Text += message + "\n";
                    });
                }
            }
            catch
            {
                // Обробка помилок
            }
        }

        private void MessageBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string message = MessageBox.Text;
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                stream.Write(buffer, 0, buffer.Length);
                MessageBox.Text = string.Empty;
            }
        }
    }
}
