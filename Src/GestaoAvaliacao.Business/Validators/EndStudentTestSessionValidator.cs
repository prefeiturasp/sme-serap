﻿using FluentValidation;
using GestaoAvaliacao.Entities.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Business.Validators
{
    public class EndStudentTestSessionValidator : AbstractValidator<EndStudentTestSessionDto>
    {
        public EndStudentTestSessionValidator()
        {
            When(x => !x.ByLostConnection, () =>
            {
                RuleFor(x => x.AluId)
                    .NotEmpty()
                    .WithMessage("A identificação do aluno deve ser informada.");

                RuleFor(x => x.TestId)
                    .NotEmpty()
                    .WithMessage("A prova em andamento deve ser informada.");

                RuleFor(x => x.TurId)
                    .NotEmpty()
                    .WithMessage("A identificação da turma do aluno deve ser informada.");
            });

            RuleFor(x => x.ConnectionId)
                .NotEmpty()
                .When(x => x.ByLostConnection)
                .WithMessage("A identificação da conexão deve ser informada.");
        }
    }
}
