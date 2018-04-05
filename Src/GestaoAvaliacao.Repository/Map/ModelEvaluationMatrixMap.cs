﻿using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class ModelEvaluationMatrixMap : EntityBaseMap<ModelEvaluationMatrix>
    {
        public ModelEvaluationMatrixMap()
        {
            ToTable("ModelEvaluationMatrix");

            Property(p => p.Description)
               .IsRequired()
               .HasMaxLength(500)
               .HasColumnType("varchar");

            Property(p => p.EntityId)
               .IsRequired();
        }
    }
}
