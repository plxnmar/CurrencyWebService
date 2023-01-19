using CurrencyWebService.Models;

namespace CurrencyWebService.Services.Interfaces
{
	public interface ICurrencyService
	{
		Task<ResponseObject<IEnumerable<Currency>>> GetPaginateCurrencies(int pageNumber, int pageSize);
		Task<ResponseObject<Currency>> GetCurrencyById(string id);
		Task<ResponseObject<IEnumerable<Currency>>> GetFromCacheCurrencies();
		Task<ResponseObject<IEnumerable<Currency>>> GetFromURICurrencies();
		void CacheCurrencies(IEnumerable<Currency> currentCurrencies);
	}
}
