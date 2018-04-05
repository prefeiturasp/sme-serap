﻿using GestaoAvaliacao.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.IRepository
{
    public interface IItemFileRepository
    {
        IEnumerable<ItemFile> GetVideosByItemId(long itemId);
        IEnumerable<ItemFile> GetVideosByLstItemId(List<long> itemId);
    }
}
