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

		public async Task<ResponseObject<Currency>> GetCurrencyById(string id)
		{
			var response = await GetAllCurrencies();

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
			var response = await GetAllCurrencies();

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
				return new ResponseObject<IEnumerable<Currency>>
				{
					Success = response.Success,
					StatusCode = response.StatusCode,
					ErrorMessage = response.ErrorMessage
				};
			}
		}

		public async Task<ResponseObject<IEnumerable<Currency>>> GetAllCurrencies()
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
				var response = await GetAndCacheCurrencies();

				if (response.Success == true)
				{
					return new ResponseObject<IEnumerable<Currency>>
					{
						StatusCode = 200,
						Success = true,
						Data = response.Data,
					};
				}
				else
				{
					return response;
				}
			}
		}


		public async Task<ResponseObject<IEnumerable<Currency>>> GetAndCacheCurrencies()
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

						_cache.Set("currenciesKey", currentCurrencies,
							new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(1)));

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
