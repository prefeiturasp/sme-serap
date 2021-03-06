﻿using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.DTO;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.MongoEntities;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Business
{
    public class TestSectionStatusCorrectionBusiness : ITestSectionStatusCorrectionBusiness
    {
        private readonly ITestSectionStatusCorrectionRepository testSectionStatusCorrectionRepository;
        private readonly ITempCorrectionResultRepository tempCorrectionResultRepository;

        public TestSectionStatusCorrectionBusiness(ITestSectionStatusCorrectionRepository testSectionStatusCorrectionRepository, ITempCorrectionResultRepository tempCorrectionResultRepository)
        {
            this.testSectionStatusCorrectionRepository = testSectionStatusCorrectionRepository;
            this.tempCorrectionResultRepository = tempCorrectionResultRepository;
        }

        #region Write

        public TestSectionStatusCorrection Save(TestSectionStatusCorrection entity)
        {
            return testSectionStatusCorrectionRepository.Save(entity);
        }

        public TestSectionStatusCorrection Update(TestSectionStatusCorrection entity)
        {
            return testSectionStatusCorrectionRepository.Update(entity);
        }

        public TestSectionStatusCorrection SetStatusCorrection(long test_id, long tur_id, EnumStatusCorrection status)
        {
            return testSectionStatusCorrectionRepository.SetStatusCorrectionUpdate(new TestSectionStatusCorrection()
            {
                Test_Id = test_id,
                tur_id = tur_id,
                StatusCorrection = status
            });
        }

        public async Task SetCorrectionStatusAsync(long test_id, long tur_id, EnumStatusCorrection status)
        {
            var statusCorrection = await GetAsync(test_id, tur_id);
            if(statusCorrection is null)
            {
                statusCorrection = new TestSectionStatusCorrection()
                {
                    StatusCorrection = EnumStatusCorrection.Processing,
                    Test_Id = test_id,
                    tur_id = tur_id
                };
                Save(statusCorrection);
            }

            if (statusCorrection.StatusCorrection != EnumStatusCorrection.Pending) return;
            statusCorrection.StatusCorrection = status;
            await testSectionStatusCorrectionRepository.UpdateAsync(statusCorrection);
        }

        #endregion Write

        #region Read

        public TestSectionStatusCorrection Get(long test_id, long tur_id)
        {
            return testSectionStatusCorrectionRepository.Get(test_id, tur_id);
        }

        public async Task<TestSectionStatusCorrection> GetAsync(long test_Id, long tur_id)
            => await testSectionStatusCorrectionRepository.GetAsync(test_Id, tur_id);

        public IEnumerable<TestSectionStatusCorrection> GetByTest(long test_id)
        {
            return testSectionStatusCorrectionRepository.GetByTest(test_id);
        }

        public IEnumerable<TestSectionStatusCorrection> GetBySchool(long Test_Id, int esc_id)
        {
            return testSectionStatusCorrectionRepository.GetBySchool(Test_Id, esc_id);
        }

        public IEnumerable<TestStatsEntitiesDTO> GetFinalizedEntities(long? Test_Id = null, string Year = null, Guid? uad_id = null, int? esc_id = null, long? tur_id = null,
            DateTime? FinalizationDate = null)
        {
            return testSectionStatusCorrectionRepository.GetFinalizedEntities(Test_Id, Year, uad_id, esc_id, tur_id, FinalizationDate);
        }

        public IEnumerable<FinalizedTestYearDTO> GetSectionsToCalculate(DateTime? PartialDate)
        {
            return testSectionStatusCorrectionRepository.GetSectionsToCalculate(PartialDate);
        }

        #region tempCorrectionResult

        public async Task<List<TempCorrectionResult>> TempCorrectionResults(Guid ent_id)
        {
            long count = await tempCorrectionResultRepository._GetCount();

            if (count <= 0)
                Generate(ent_id);

            return await tempCorrectionResultRepository._GetNotProcessed();
        }

        public void Generate(Guid ent_id)
        {
            var lstTest = testSectionStatusCorrectionRepository.GetAll();

            var entity = (from l in lstTest
                          orderby l.Test_Id, l.tur_id
                          select new TempCorrectionResult(ent_id, l.Test_Id, l.tur_id)
                          {
                              Test_id = l.Test_Id,
                              Tur_id = l.tur_id,
                              Processed = false,
                              CreateDate = DateTime.Now,
                              UpdateDate = DateTime.Now,
                              State = 1
                          }).ToList();

            tempCorrectionResultRepository.InsertMany(entity);
        }

        public Task<TempCorrectionResult> UpdateTempCorrection(TempCorrectionResult entity)
        {
            return tempCorrectionResultRepository.Replace(entity);
        }

        public async Task<TempCorrectionResult> GetTempCorrection(Guid ent_id, long test_id, long tur_id)
        {
            var tempCorrectionResult = await tempCorrectionResultRepository.FindOneAsync(new TempCorrectionResult(ent_id, test_id, tur_id));
            if (tempCorrectionResult is null)
                tempCorrectionResult = new TempCorrectionResult(ent_id, test_id, tur_id)
                {
                    CreateDate = DateTime.Now,
                    Test_id = test_id,
                    Tur_id = tur_id,
                    Processed = false
                };

            tempCorrectionResult.UpdateDate = DateTime.Now;
            await tempCorrectionResultRepository.InsertOrReplaceAsync(tempCorrectionResult);
            return tempCorrectionResult;
        }

        #endregion tempCorrectionResult

        #endregion Read
    }
}