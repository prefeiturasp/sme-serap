﻿using GestaoAvaliacao.Dtos.StudentTestAccoplishments;
using GestaoAvaliacao.Entities.DTO;
using GestaoAvaliacao.Entities.DTO.StudentTestAccoplishments;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoAvaliacao.IBusiness
{
    public interface IStudentTestAccoplishmentBusiness
    {
        Task StartSessionAsync(StartStudentTestSessionDto dto);

        Task EndSessionAsync(EndStudentTestSessionDto dto);

        Task EndStudentTestAccoplishmentAsync(EndStudentTestAccoplishmentDto dto);

        Task<StudentTestTimeResultDto> GetStudenteResultAsync(List<ElectronicTestDTO> electronicTests);

        Task<StudentTestTimeDto> GetAsyncByAluIdTurIdTestId(long aluId, long turId, long testId);

        Task<List<StudentTestTimeDto>> GetAsyncByAluIdTestId(long aluId, List<long> testId);
    }
}