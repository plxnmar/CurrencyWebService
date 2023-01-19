using CurrencyWebService.Models;
using CurrencyWebService.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace CurrencyWebService.Services.Implementations
{
	public class CurrencyService : ICurrencyService
	{
		private readonly string _url = "https://www.cbr-xml-daily.ru/daily_json.js";

		private readonly IMemoryCache _cache;
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly ILogger _logger;

		public CurrencyService(IMemoryCache memoryCache, IHttpClientFactory httpClientFactory,
			ILogger<ICurrencyService> logger)
		{
			_cache = memoryCache;
			_httpClientFactory = httpClientFactory;
			_logger = logger;
		}

		public async Task<ResponseObject<Currency>> GetCurrencyById(string id)
		{
			var response = await GetFromCacheCurrencies();

			if (response.Success == true)
			{
				var currencies = response.Data;
				var currencyById = currencies.FirstOrDefault(x => x.ID == id);

				if (currencyById == null)
				{
					return new ResponseObject<Currency>
					{
						StatusCode = 404,
						Success = false,
						ErrorMessage = "Валюта с данным идентификатором не найдена"
					};
				}
				else
				{
					return new ResponseObject<Currency>
					{
						StatusCode = response.StatusCode,
						Success = true,
						Data = currencyById
					};
				}
			}
			else
			{
				return new ResponseObject<Currency>
				{
					Success = response.Success,
					StatusCode = response.StatusCode,
					ErrorMessage = response.ErrorMessage
				};
			}
		}

		public async Task<ResponseObject<IEnumerable<Currency>>> GetPaginateCurrencies(int pageNumber, int pageSize)
		{
			var response = await GetFromCacheCurrencies();

			if (response.Success == true)
			{
				var currencies = response.Data;

				currencies = currencies?
								.Skip((pageNumber - 1) * pageSize)
								.Take(pageSize);

				return new ResponseObject<IEnumerable<Currency>>
				{
					Success = true,
					StatusCode = response.StatusCode,
					Data = currencies,
				};
			}
			else
			{
				return response;
			}
		}

		public async Task<ResponseObject<IEnumerable<Currency>>> GetFromCacheCurrencies()
		{
			_cache.TryGetValue("currenciesKey", out IEnumerable<Currency>? currencies);

			if (currencies != null)
			{
				return new ResponseObject<IEnumerable<Currency>>
				{
					StatusCode = 200,
					Success = true,
					Data = currencies,
				};
			}
			else
			{
				//если в кэше нет данных получаем данные из json
				var response = await GetFromURICurrencies();
				return response;
			}
		}


		public void CacheCurrencies(IEnumerable<Currency> currentCurrencies)
		{
			_cache.Set("currenciesKey", currentCurrencies,
				new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(1)));

			_logger.LogInformation("Данные валют обновлены в кэше");
		}

		public async Task<ResponseObject<IEnumerable<Currency>>> GetFromURICurrencies()
		{
			try
			{
				var client = _httpClientFactory.CreateClient();

				var response = await client.GetAsync(_url);

				if (!response.IsSuccessStatusCode)
				{
					return new ResponseObject<IEnumerable<Currency>>
					{
						StatusCode = (int)response.StatusCode,
						Success = false,
						ErrorMessage = $"Не удалось выполнить запрос к {_url}",
					};
				}
				else
				{
					var rootObject = await response.Content.ReadFromJsonAsync<RootObject>();

					if (rootObject != null)
					{
						var currentCurrencies = rootObject.Valute.Values.Cast<Currency>();

						return new ResponseObject<IEnumerable<Currency>>
						{
							StatusCode = 200,
							Success = true,
							Data = currentCurrencies,
						};
					}
					else
					{
						return new ResponseObject<IEnumerable<Currency>>
						{
							StatusCode = 204,
							Success = true,
						};
					}
				}
			}
			catch (Exception ex)
			{
				return new ResponseObject<IEnumerable<Currency>>
				{
					StatusCode = 500,
					Success = false,
					ErrorMessage = ex.Message,
				};
			}
		}
	}
}
