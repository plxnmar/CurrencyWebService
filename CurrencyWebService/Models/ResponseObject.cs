namespace CurrencyWebService.Models
{
	public class ResponseObject<T>
	{
		public bool Success { get; set; }
		public string? ErrorMessage { get; set; }
		public int StatusCode { get; set; }
		public T? Data { get; set; }
	}
}
