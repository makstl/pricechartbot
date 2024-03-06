using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PriceChartOptionsList
{
	public PriceChartOptions Bitcoin { get; set; }
	public PriceChartOptions Tezos { get; set; }
	public PriceChartOptions Ethereum { get; set; }
	public PriceChartOptions Toncoin { get; set; }
	public PriceChartOptions Mina { get; set; }
}
public class PriceChartOptions
{
	public long ChatId { get; set; }
	public int Interval { get; set; }
	public string Symbol { get; set; }
	public int MessageId { get; set; }
	public string? ApiKey { get; set; }
}
