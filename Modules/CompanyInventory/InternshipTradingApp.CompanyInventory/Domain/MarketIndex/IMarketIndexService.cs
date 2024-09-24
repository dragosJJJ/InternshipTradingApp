﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTradingApp.CompanyInventory.Domain.MarketIndex
{
    public interface IMarketIndexService
    {
        Task<decimal> CalculateAndSaveMarketIndex();
        Task<IEnumerable<MarketIndexHistory>> GetMarketIndexHistory();
    }
}
