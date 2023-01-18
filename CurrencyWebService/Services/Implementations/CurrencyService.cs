using CurrencyWebService.Models;
using CurrencyWebService.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text.Json;

namespace CurrencyWebService.Services.Implementations
{
	public class CurrencyService : ICurrencyService
	{
		private readonly string _url = "https://www.cbr-xml-daily.ru/daily_json.js";

		private readonly IMemoryCache _cache;
		private readonly IHttpClientFactory _httpClientFactory;

		public CurrencyService(IMemoryCache memoryCache, IHttpClientFactory httpClientFactory)
		{
			_cache = memoryCache;
			_httpClientFactory = httpClientFactory;
		}

		public async Task<Currency> GetCurrencyById(string id)
		{
			var list = await GetAllCurrencies();
			var cur = list.FirstOrDefault(x => x.ID == id);
			return cur;
		}

		public async Task<List<Currency>> GetPaginateCurrencies(int pageNumber, int pageSize)
		{
			var currencies = await GetAllCurrencies();

			currencies = currencies
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize);

			return currencies.ToList();
		}

		public async Task<IEnumerable<Currency>> GetAllCurrencies()
		{	
			// try-catch
			//обработка ошибок и в контроллеры

			_cache.TryGetValue("currenciesKey", out List<Currency> currencies);

			if (currencies != null)
			{
				return currencies;
			}
			else
			{
				currencies = await GetAndCacheCurrencies();
				return currencies;
			}

		}


		public async Task<List<Currency>> GetAndCacheCurrencies()
		{
			var client = _httpClientFactory.CreateClient();
			var rootObject = await client.GetFromJsonAsync<RootObject>(_url);

			if (rootObject != null)
			{
				var currentCurrencies = rootObject.Valute.Values.ToList();

				_cache.Set("currenciesKey", currentCurrencies,
					new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(1)));

				return currentCurrencies;
			}
			else
			{
				//TODO
				return new List<Currency>();
			}

		}
	}
}
