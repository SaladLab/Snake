using Topshelf;

namespace GameServer
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            return (int)HostFactory.Run(x =>
            {
                string runner = null;
                x.AddCommandLineDefinition("runner", val => runner = val);

                x.SetServiceName("Snake");
                x.SetDisplayName("Snake Service");
                x.SetDescription("Snake Service using Akka.NET and Akka.Interfaced. (https://github.com/SaladLab/Snake)");

                x.UseAssemblyInfoForServiceInfo();
                x.RunAsLocalSystem();
                x.StartAutomatically();
                x.Service(() => new GameService(runner));
                x.EnableServiceRecovery(r => r.RestartService(1));
            });
        }
    }
}
