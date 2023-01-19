using CurrencyWebService.Services.Interfaces;


namespace CurrencyWebService.Services.Implementations
{
	public class CurrencyHostedService : BackgroundService
	{
		private readonly ICurrencyService _currencyService;
		private readonly ILogger _logger;

		private const int INTERVAL_MINUTES = 30;
		public CurrencyHostedService(ICurrencyService currencyService, ILogger<CurrencyHostedService> logger)
		{
			_currencyService = currencyService;
			_logger = logger;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			// выполняем задачу пока не будет запрошена остановка приложения
			while (!stoppingToken.IsCancellationRequested)
			{
				try
				{
					//получаем и кэшируем актуальные данные
					await _currencyService.GetAndCacheCurrencies();

					_logger.LogInformation($"Курс валют обновлен в {DateTimeOffset.Now}");
				}
				catch (Exception ex)
				{
					throw new Exception(ex.ToString());
				}

				//вычисление времени до следующего обновления 
				//чтобы обновления происходили кратно 30 минутам
				//10:00 - 10:30 - 11:00 - 11:30 и тд

				var now = DateTime.Now.Minute;
				var nextInterval = INTERVAL_MINUTES  - now % INTERVAL_MINUTES;

				//выполняем задачу каждый интервал времени
				await Task.Delay(TimeSpan.FromMinutes(nextInterval), stoppingToken);
			}
		}
	}
}
