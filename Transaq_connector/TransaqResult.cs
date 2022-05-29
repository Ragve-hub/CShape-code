namespace OpenQuant.Finam
{
	public sealed class TransaqResult
	{
		public bool Success { get; private set; }

		public string TransactionId { get; private set; }

		public string Message { get; private set; }

		public string Error { get; private set; }

		public TransaqResult()
		{
		}

		public TransaqResult(string data)
		{
			TransactionId = (Message = (Error = string.Empty));
			string[] array = data.Split(';');
			for (int i = 0; i < array.Length; i += 2)
			{
				switch (array[i])
				{
				case "success":
					Success = bool.Parse(array[i + 1]);
					break;
				case "transactionid":
					TransactionId = array[i + 1];
					break;
				case "message":
					Message = array[i + 1];
					break;
				case "error":
					Error = array[i + 1];
					break;
				}
			}
		}
	}
}
