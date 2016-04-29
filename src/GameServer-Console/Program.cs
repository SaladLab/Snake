namespace GameServer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var serviceMain = new ServiceMain();
            serviceMain.Run(args);
        }
    }
}
