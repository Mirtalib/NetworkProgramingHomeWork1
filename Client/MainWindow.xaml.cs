using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        private Socket client;
        private IPEndPoint ep;
        private IPEndPoint remotep;

        private ushort size = ushort.MaxValue - 29;
        private byte[] buffer;

        public MainWindow()
        {
            InitializeComponent();
            client = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            IPAddress ip = IPAddress.Parse("127.0.0.1");
            int port = 43;
            ep = new IPEndPoint(ip, port);
            remotep = new IPEndPoint(ip,44);

            
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (client == null)
                return;

            buffer = new byte[size];
            await client.SendToAsync(buffer, SocketFlags.None, remotep);
            List<byte> data = new List<byte>();
            var len = 0;


            do
            {
                try
                {

                    var result = await client.ReceiveFromAsync(buffer, SocketFlags.None, remotep);
                    len = result.ReceivedBytes;
                    data.AddRange(buffer.Take(len));
                    var image = ByteToImage(data.ToArray());
                    Image1.Source = image;

                }
                catch (Exception)
                {

                }
            } while (len == buffer.Length);

            

        }



        private static BitmapImage ByteToImage(byte[] imageInfo)
        {
            var image = new BitmapImage();

            using (var memoryStream = new MemoryStream(imageInfo))
            {
                memoryStream.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = memoryStream;
                image.EndInit();
            }

            image.Freeze();

            return image;
        }
    }
}
