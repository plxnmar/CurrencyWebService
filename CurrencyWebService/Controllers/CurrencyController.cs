using CurrencyWebService.Models;
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
		public async Task<IActionResult> Get(int pageNumber = 1, int pageSize = 5)
		{
			var response =  await _currencyService.GetPaginateCurrencies(pageNumber, pageSize);
			return GetStatusCode(response);
		}

		[HttpGet]
		[Route("currency/{id}")]
		public async Task<IActionResult> Get(string id)
		{
			var response = await _currencyService.GetCurrencyById(id);
			return GetStatusCode(response);
		}

		private IActionResult GetStatusCode<T>(ResponseObject<T> response)
		{
			if (response.Success)
			{
				return StatusCode(response.StatusCode, response.Data);
			}
			else
			{
				return StatusCode(response.StatusCode, response.ErrorMessage);
			}
		}
	}
}
