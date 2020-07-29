﻿using GestaoAvaliacao.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoAvaliacao.IRepository
{
    public interface ITestTypeDeficiencyRepository
    {
        Task<IEnumerable<TestTypeDeficiency>> GetAsync(long testTypeId);
        IEnumerable<Guid> GetDeficienciesIds(long testTypeId);
    }
}