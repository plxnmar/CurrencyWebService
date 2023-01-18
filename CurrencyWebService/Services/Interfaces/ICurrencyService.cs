namespace CurrencyWebService.Services.Interfaces
{
	public interface CurrencyInterface
	{
		IEnumerable<string> GetAll();
		string Get(int id);

	}
}
