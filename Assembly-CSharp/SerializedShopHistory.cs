using System;
using System.Collections.Generic;

[Serializable]
public class SerializedShopHistory
{
	public string id;

	public int visits;

	public decimal totalTransactionValue;

	public List<int> visitDays = new List<int>();

	public List<int> transactionDays = new List<int>();
}
