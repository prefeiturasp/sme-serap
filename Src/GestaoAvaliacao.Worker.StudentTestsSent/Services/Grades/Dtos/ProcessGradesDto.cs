﻿using GestaoAvaliacao.MongoEntities;
using GestaoAvaliacao.Worker.StudentTestsSent.Processing.Dtos.Base;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.Worker.StudentTestsSent.Services.Grades.Dtos
{
    internal class ProcessGradesDto : NotificableWorkerDto
    {
        public long TestId { get; set; }
        public long TurId { get; set; }
        public Guid EntId { get; set; }
        public Guid DreId { get; set; }
        public int EscId { get; set; }
        public int QuantidadeDeAlunos { get; set; }
        public TestTemplate TestTemplate { get; set; }
        public List<StudentCorrection> StudentCorrections { get; set; }
    }
}