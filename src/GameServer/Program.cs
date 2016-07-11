using System;
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

                x.SetServiceName("GameServer");
                x.SetDisplayName("GameServer for Snake");
                x.SetDescription("GameServer for Snake using Akka.NET and Akka.Interfaced.");

                x.UseAssemblyInfoForServiceInfo();
                x.RunAsLocalSystem();
                x.StartAutomatically();
                x.Service(() => new GameService(runner));
                x.EnableServiceRecovery(r => r.RestartService(1));
            });
        }
    }
}
