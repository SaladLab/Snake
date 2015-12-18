namespace GameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceMain = new ServiceMain();
            serviceMain.Run(args);
        }
    }
}
