using GestaoAvaliacao.Worker.IoC;
using GestaoAvaliacao.Worker.StudentTestsSent.Processing;
using GestaoAvaliacao.Worker.StudentTestsSent.Services.CorrectionResult;
using GestaoAvaliacao.Worker.StudentTestsSent.Services.Grades;
using GestaoAvaliacao.Worker.StudentTestsSent.Workers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using GestaoAvaliacao.Worker.StudentTestsSent.Logging;
using Prometheus;
using GestaoAvaliacao.Worker.StudentTestsSent.Consumers;

namespace GestaoAvaliacao.Worker.StudentTestsSent
{
    public class Program
    {
        private static readonly IIoCStartup _ioCStartup = new IoCStartup();

        public static void Main(string[] args)
        {
            var metricServer = new MetricServer(port: 1234);
            metricServer.Start();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    config.AddEnvironmentVariables();
                })
                .ConfigureLogging((context, logging) =>
                {
                    logging.AddConfiguration(context.Configuration);
                    logging.AddSentry();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<StudentTestSentWorker>();
                    _ioCStartup.Register(services, hostContext.Configuration);
                    services.AddSingleton<ISentryLogger, SentryLogger>();
                    services.AddTransient<IGenerateCorrectionResultsServices, GenerateCorrectionResultsServices>();
                    services.AddTransient<IProcessGradesServices, ProcessGradesServices>();
                    services.AddTransient<IStudentTestSentProcessingChain, StudentTestSentProcessingChain>();
                    services.AddMediatR(Assembly.GetExecutingAssembly());
                });

    }
}