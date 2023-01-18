using CurrencyWebService.Models;

namespace CurrencyWebService.Services.Interfaces
{
	public interface ICurrencyService
	{
		Task<List<Currency>> GetPaginateCurrencies(int pageNumber, int pageSize);
		Task<Currency> GetCurrencyById(string id);
		Task<IEnumerable<Currency>> GetAllCurrencies();
		Task<List<Currency>> GetAndCacheCurrencies();

	}
}
