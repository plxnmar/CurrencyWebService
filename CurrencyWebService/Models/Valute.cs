namespace CurrencyWebService.Models
{
	public class Currency
	{
		public string ID { get; set; }
		public int NumCode { get; set; }
		public string CharCode { get; set; }
		public int Nominal { get; set; }
		public string Name { get; set; }
		public double Value { get; set; }
		public double Previous { get; set; }
	}

	public class RootObject
	{
		public DateTime DateOnly { get; set; }
		public DateTime PreviousDate { get; set; }
		public string PreviousURL { get; set; }
		public DateTime Timestamp { get; set; }
		public Dictionary<string, Currency> Valute { get; set; }
	}
}
