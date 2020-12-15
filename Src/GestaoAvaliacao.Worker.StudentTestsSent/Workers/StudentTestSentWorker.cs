﻿using GestaoAvaliacao.Worker.StudentTestsSent.Consumers;
using GestaoAvaliacao.Worker.StudentTestsSent.Logging;
using GestaoAvaliacao.Worker.StudentTestsSent.Workers.Scheduling;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.StudentTestsSent.Workers
{
    public class StudentTestSentWorker : BaseScheduledWorker
    {
        private readonly IStudentTestSentConsumer _studentTestSentConsummer;

        public StudentTestSentWorker(IConfiguration configuration, ISentryLogger sentryLogger, IStudentTestSentConsumer studentTestSentConsummer)
            : base(configuration, sentryLogger)
        {
            _studentTestSentConsummer = studentTestSentConsummer;
        }

        protected override string WorkerDescription => nameof(StudentTestSentWorker);

        protected override string CronWorkerParameter => $"{nameof(StudentTestSentWorker)}_CronParameter";

        protected override Task ExecuteAsync(CancellationToken cancellationToken) => _studentTestSentConsummer.ConsumeAsync(cancellationToken);
    }
}