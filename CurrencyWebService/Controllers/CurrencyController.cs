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
		public IEnumerable<string> Get()
		{
			return new string[] { "value1", "value2" };
		}

		[HttpGet("{id}")]
		[Route("currency/{id}")]
		public string Get(int id)
		{
			return "value";
		}

	}
}
