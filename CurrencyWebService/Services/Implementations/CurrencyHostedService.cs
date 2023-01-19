using CurrencyWebService.Models;
using CurrencyWebService.Services.Interfaces;
using System.Threading;

namespace CurrencyWebService.Services.Implementations
{
	public class CurrencyHostedService : BackgroundService
	{
		private readonly ICurrencyService _currencyService;
		private readonly ILogger _logger;

		public CurrencyHostedService(ICurrencyService currencyService, ILogger<CurrencyHostedService> logger)
		{
			_currencyService = currencyService;
			_logger = logger;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			// Выполняем задачу пока не будет запрошена остановка приложения
			while (!stoppingToken.IsCancellationRequested)
			{
				try
				{
					var currencies = await _currencyService.GetAndCacheCurrencies();
					_logger.LogInformation($"Курс валют обновлен в {DateTimeOffset.Now}");
				}
				catch (Exception ex)
				{
					// обработка ошибки однократного неуспешного выполнения фоновой задачи
				}

				//получаем курсы валют каждые 30 минут
				await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
			}

			//TODO 
			//Прописать дату чтобы 00 30 00 30


			//// Если нужно дождаться завершения очистки, но контролировать время, то стоит предусмотреть в контракте использование CancellationToken
			//await someService.DoSomeCleanupAsync(cancellationToken);
		}
	}
}
