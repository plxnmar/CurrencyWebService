using Microsoft.AspNetCore.Mvc;

namespace CurrencyWebService.Controllers
{
	[ApiController]
	public class CurrencyController : ControllerBase
	{
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
