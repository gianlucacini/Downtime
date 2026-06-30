using System;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;
using Serilog;
using SimpleInjector;
using Downtime.Service.BusinessLogic;
using Downtime.Service.DataAccess;
using Downtime.Service.Jobs;

namespace Downtime.Service
{
    static class Program
    {
        static readonly Container _container = new Container();

        /// <summary>
        /// Main Entry Point
        /// </summary>
        static void Main()
        {
            RegisterServices();

            var appBlockerService = _container.GetInstance<AppBlockerService>();
            var communicationServerService = _container.GetInstance<CommunicationServerService>();
            var registryMonitorService = _container.GetInstance<RegistryMonitorService>();

#if DEBUG
            appBlockerService.OnDebug();
            communicationServerService.OnDebug();
            registryMonitorService.OnDebug();

            Thread.Sleep(Timeout.Infinite);

#else
                ServiceBase[] ServicesToRun = new ServiceBase[]
                {
                    communicationServerService,
                    registryMonitorService,
                   appBlockerService,
                   
                };
                ServiceBase.Run(ServicesToRun);
#endif
        }

        private static void RegisterServices()
        {
            _container.RegisterInstance(ConfigureLogger());

            //data access
            _container.Register<ApplicationSettingsContext>(Lifestyle.Singleton);

            //managers
            _container.Register<CriticalProcessManager>(Lifestyle.Singleton);            
            _container.Register<RegistryMonitorJob>(Lifestyle.Singleton);
            _container.Register<SntpClient>(Lifestyle.Singleton);
            _container.Register<DateTimeSource>(Lifestyle.Singleton);

            //jobs
            _container.Register<RegistryMonitorJob>(Lifestyle.Singleton);
            _container.Register<AppBlockerJob>(Lifestyle.Singleton);

            //services
            _container.Register<AppBlockerService>(Lifestyle.Singleton);
            _container.Register<CommunicationServerService>(Lifestyle.Singleton);
            _container.Register<RegistryMonitorService>(Lifestyle.Singleton);


            _container.Verify();
        }

        private static ILogger ConfigureLogger()
        {

            var serilogConfiguration = new LoggerConfiguration()
                .WriteTo
                .File(LogPath())
                .CreateLogger();

            return serilogConfiguration;
        }

        private static string LogPath()
        {
            String localPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            return Path.Combine(localPath, $"UnplugService_{DateTime.Now.ToString("yyyy")}_{DateTime.Now.ToString("MM")}.log");
        }
    }
}