using CurrencyWebService.Models;

namespace CurrencyWebService.Services.Interfaces
{
	public interface ICurrencyService
	{
		Task<ResponseObject<IEnumerable<Currency>>> GetPaginateCurrencies(int pageNumber, int pageSize);
		Task<ResponseObject<Currency>> GetCurrencyById(string id);
		Task<ResponseObject<IEnumerable<Currency>>> GetAllCurrencies();
		Task<ResponseObject<IEnumerable<Currency>>> GetAndCacheCurrencies();

	}
}
