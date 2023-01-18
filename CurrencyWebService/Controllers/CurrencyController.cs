using CurrencyWebService.Models;
using CurrencyWebService.Services.Implementations;
using CurrencyWebService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyWebService.Controllers
{
	[ApiController]
	public class CurrencyController : ControllerBase
	{
		private readonly ICurrencyService _currencyService;
		public CurrencyController(ICurrencyService currencyService)
		{
			_currencyService = currencyService;
		}

		[HttpGet]
		[Route("currencies")]
		public async Task<List<Currency>> Get(int pageNumber = 1, int pageSize = 5)
		{
			return await _currencyService.GetPaginateCurrencies(pageNumber, pageSize);
		}

		[HttpGet]
		[Route("currency/{id}")]
		public async Task<Currency> Get(string id)
		{
			return await _currencyService.GetCurrencyById(id);
		}

	}
}
