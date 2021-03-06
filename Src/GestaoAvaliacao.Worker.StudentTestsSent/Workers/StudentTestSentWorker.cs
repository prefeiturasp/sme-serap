﻿using GestaoAvaliacao.Worker.StudentTestsSent.Consumers;
using GestaoAvaliacao.Worker.StudentTestsSent.Logging;
using GestaoAvaliacao.Worker.StudentTestsSent.Requests.Commands;
using GestaoAvaliacao.Worker.StudentTestsSent.Workers.Scheduling;
using MediatR;
using Microsoft.Extensions.Configuration;
using Prometheus.DotNetRuntime;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.StudentTestsSent.Workers
{
    public class StudentTestSentWorker : BaseScheduledWorker
    {
        private readonly IMediator _mediator;
        private IDisposable _collector;

        public StudentTestSentWorker(IConfiguration configuration, ISentryLogger sentryLogger, IMediator mediator)
            : base(configuration, sentryLogger)
        {
            _mediator = mediator;
        }

        protected override string WorkerDescription => nameof(StudentTestSentWorker);

        protected override string CronWorkerParameter => $"{nameof(StudentTestSentWorker)}_CronParameter";

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _collector = DotNetRuntimeStatsBuilder.Default().StartCollecting();
            await _mediator.Send(new ProcessStudentTestSentCommand());
        }
    }
}