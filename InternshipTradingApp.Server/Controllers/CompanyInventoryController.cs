﻿using InternshipTradingApp.CompanyInventory.Features.Shared;
using Microsoft.AspNetCore.Mvc;
using InternshipTradingApp.CompanyInventory.Domain;
using InternshipTradingApp.ModuleIntegration.CompanyInventory;
using ExternalDataSynchronization.Infrastructure;
using InternshipTradingApp.CompanyInventory.Domain.MarketIndex;


namespace InternshipTradingApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyInventoryController : ControllerBase
    {
        private readonly ICompanyInventoryService companyInventoryService;
        private readonly ICompanyHistoryInventoryService companyHistoryInventoryService;
        private readonly IMarketIndexService _marketIndexService;

        public CompanyInventoryController(ICompanyInventoryService companyInventoryService, ICompanyHistoryInventoryService companyHistoryInventoryService, IMarketIndexService marketIndexService)
        {
            this.companyInventoryService = companyInventoryService;
            this.companyHistoryInventoryService = companyHistoryInventoryService;
            this._marketIndexService = marketIndexService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                var company = await companyHistoryInventoryService.GetCompanyWithHistoryDataAsync(value);
                if (company != null)
                {
                    return Ok(company);
                }
                return NotFound(value);
            }

            var allCompanies = await companyInventoryService.GetAllCompanies();
            return Ok(allCompanies);
        }

        [HttpGet("topXCompaniesByParameter")]
        public async Task<IEnumerable<CompanyWithHistoryGetDTO>> GetTopXCompanies([FromQuery] int? x, string? value, string orderToggle)
        {
            return await companyInventoryService.GetTopXCompanies(x, value, orderToggle);
        }

        [HttpGet("marketIndex")]
        public async Task<decimal> GetMarketIndex()
        {
            return await companyInventoryService.GetMarketIndex();
        }


        [HttpPost("history")]
        public async Task<IEnumerable<CompanyHistoryGetDTO>> Post([FromBody] IEnumerable<CompanyHistoryAddDTO> companyHistoryDtos)
        {

            if (companyHistoryDtos == null)
            {
                throw new ArgumentNullException(nameof(companyHistoryDtos));
            }

            return await this.companyHistoryInventoryService.RegisterCompaniesHistory(companyHistoryDtos);
        }
        [HttpPost("external-data")]
        public async Task<IEnumerable<CompanyGetDTO>> Post([FromBody] IEnumerable<CompanyAddDTO> companyDtos)
        {

            if (companyDtos == null)
            {
                throw new ArgumentNullException(nameof(companyDtos));
            }
            var importedCompanies =  await this.companyInventoryService.RegisterOrUpdateCompanies(companyDtos);
            var marketIndex = await _marketIndexService.CalculateAndSaveMarketIndex();

            return importedCompanies;
        }

        [HttpGet("marketIndexHistory")]
        public async Task<IActionResult> GetMarketIndexHistory()
        {
            var history = await _marketIndexService.GetMarketIndexHistory();
            return Ok(history);
        }
    }
}