namespace OpenQuant.Finam
{
	public class OpenBookData
	{
		public double Price { get; set; }

		public int Size { get; set; }

		public OpenBookData(double price, int size)
		{
			Price = price;
			Size = size;
		}
	}
}
