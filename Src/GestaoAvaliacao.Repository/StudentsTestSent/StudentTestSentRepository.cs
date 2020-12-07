﻿using GestaoAvaliacao.Entities.StudentsTestSent;
using GestaoAvaliacao.IRepository.StudentsTestSent;
using GestaoAvaliacao.Repository.Context;
using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Repository.StudentsTestSent
{
    public class StudentTestSentRepository : IStudentTestSentRepository
    {
        private readonly GestaoAvaliacaoContext _gestaoAvaliacaoContext;

        public StudentTestSentRepository()
        {
            _gestaoAvaliacaoContext = new GestaoAvaliacaoContext();
        }

        public async Task AddAsync(StudentTestSent entity, CancellationToken cancellationToken)
        {
            if (entity is null || !entity.Validate.IsValid) return;
            _gestaoAvaliacaoContext.StudentTestsSent.Add(entity);
            await _gestaoAvaliacaoContext.SaveChangesAsync(cancellationToken);
        }

        public Task<bool> AnyAsync(long testId, long turId, long aluId, CancellationToken cancellationToken)
            => _gestaoAvaliacaoContext
                .StudentTestsSent
                .AnyAsync(x => x.TestId == testId && x.TurId == turId && x.AluId == aluId, cancellationToken);

        public Task<StudentTestSent> GetFirstOrDefaultAsync(long testId, long turId, long aluId, CancellationToken cancellationToken)
            => _gestaoAvaliacaoContext
                .StudentTestsSent
                .FirstOrDefaultAsync(x => x.TestId == testId && x.TurId == turId && x.AluId == aluId, cancellationToken);

        public Task<StudentTestSent> GetFirstBySituationAsync(StudentTestSentSituation situation, CancellationToken cancellationToken) 
            => _gestaoAvaliacaoContext
                .StudentTestsSent
                .FirstOrDefaultAsync(x => x.Situation == situation);

        public async Task RemoveAsync(StudentTestSent entity, CancellationToken cancellationToken)
        {
            if (entity is null || !entity.Validate.IsValid) return;
            _gestaoAvaliacaoContext.StudentTestsSent.Remove(entity);
            await _gestaoAvaliacaoContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(StudentTestSent entity, CancellationToken cancellationToken)
        {
            if (entity is null || !entity.Validate.IsValid) return;
            entity.UpdateDate = DateTime.Now;
            _gestaoAvaliacaoContext.Entry(entity).State = EntityState.Modified;
            await _gestaoAvaliacaoContext.SaveChangesAsync(cancellationToken);
        }
    }
}