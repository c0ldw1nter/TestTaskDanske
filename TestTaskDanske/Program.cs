namespace TestTaskDanske
{
    internal class Program
    {
        private const int MaxSimultaneousRequests = 10;

        static void Main(string[] args)
        {
            Server server = new Server(MaxSimultaneousRequests);
            server.StartServer();
        }
    }
}