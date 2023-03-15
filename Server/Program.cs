
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using static System.Net.Mime.MediaTypeNames;

Socket server = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

IPAddress ip = IPAddress.Parse("127.0.0.1");
int port = 44;

IPEndPoint ep = new IPEndPoint(ip, port);
var remotep = new IPEndPoint(IPAddress.Any, 0); 

server.Bind(ep);
ushort size = ushort.MaxValue - 29;
byte[] buffer = new byte[size];


while (true)
{
    var result = await server.ReceiveFromAsync(buffer, SocketFlags.None, remotep);
    var image = Screenshot();
    var imageToBytes = ImageToByte(image);
    var fill = imageToBytes.Chunk(size);
    var imageArray = fill.ToArray();

    foreach (var item in imageArray)
    {
        await Task.Delay(200);
        await server.SendToAsync(item, SocketFlags.None, result.RemoteEndPoint);
    }
    Console.WriteLine("Complate");
}


System.Drawing.Image Screenshot()
{
    Bitmap bitmap = new Bitmap(1920, 1080);

    Graphics graphics = Graphics.FromImage(bitmap);
    graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);

    return bitmap;
}


byte[] ImageToByte(System.Drawing.Image image)
{
    using (var stream = new MemoryStream())
    {
        image.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);

        return stream.ToArray();
    }
}