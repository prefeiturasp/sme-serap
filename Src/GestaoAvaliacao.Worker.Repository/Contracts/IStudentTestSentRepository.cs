﻿using GestaoAvaliacao.Worker.Domain.Entities.Tests;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.Repository.Contracts
{
    // Interface alocada junto à implementação do repositório inicialmente visando facilitar uma possível migração
    public interface IStudentTestSentRepository
    {
        Task<StudentTestSentEntityWorker> GetFirstToProcessAsync(CancellationToken cancellationToken);

        Task UpdateAsync(StudentTestSentEntityWorker entity, CancellationToken cancellationToken);
    }
}