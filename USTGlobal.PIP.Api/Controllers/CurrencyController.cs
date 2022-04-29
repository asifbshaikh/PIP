using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.Api.Controllers
{
    /// <summary>
    /// CurrencyController
    /// </summary>
    [Route("api/currency")]
    public class CurrencyController : BaseController
    {
        private readonly IProjectHeaderService projectHeaderService;

        /// <summary>
        /// CurrencyController Constructor
        /// </summary>
        /// <param name="projectHeaderService"></param>
        public CurrencyController(IProjectHeaderService projectHeaderService)
        {
            this.projectHeaderService = projectHeaderService;
        }

        /// <summary>
        /// Get Currency Conversion data based on CountryId
        /// </summary>
        /// <param name="countryId"></param>
        /// <returns></returns>
        [HttpGet, Route("country/{countryId}")]
        public async Task<CurrencyDTO> GetCurrencyConversionData(int countryId)
        {
            return await this.projectHeaderService.GetCurrencyConversionData(countryId);
        }

    }
}
