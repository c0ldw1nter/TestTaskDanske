using System.Net.Sockets;
using System.Net;
using System.Text;

namespace TestTaskDanske
{
    public class Server
    {
        private readonly int _maxSimultaneousRequests;
        private readonly ManualResetEvent _doneEvent = new ManualResetEvent(false);

        public Server(int maxSimulataneousRequests)
        {
            this._maxSimultaneousRequests = maxSimulataneousRequests;
        }

        public void StartServer()
        {
            IPHostEntry host = Dns.GetHostEntry("localhost");
            IPAddress address = host.AddressList[1];
            IPEndPoint endpoint = new IPEndPoint(address, 11000);

            try
            {
                Socket listener = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(endpoint);
                listener.Listen(this._maxSimultaneousRequests);

                while (true)
                {
                    _doneEvent.Reset();
                    Console.WriteLine($"Waiting for requests on {endpoint.Address}:{endpoint.Port}");
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
                    _doneEvent.WaitOne();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\n Press any key to continue.");
            Console.ReadKey();
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            this._doneEvent.Set();

            Console.WriteLine($"Request received");

            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            Console.WriteLine($"Sending response");

            StringBuilder builder = new StringBuilder();
            builder.AppendLine(@"HTTP/1.1 200 OK");
            builder.AppendLine(@"Content-Type: text/html");
            builder.AppendLine(@"");
            builder.AppendLine(@"<html><head><title>Index</title></head><body>No content yet.</body></html>");

            byte[] bytes = Encoding.UTF8.GetBytes(builder.ToString());

            handler.Send(bytes);
            handler.Disconnect(true);
            Console.WriteLine("Connection end");
        }
    }
}
