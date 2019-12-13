using System;
using System.IO;
using System.Text;

namespace Task5
{
	public class ListRand
	{
		public ListNode Head;
		public ListNode Tail;
		public Int32 Count;

		public void Serialize(FileStream s)
		{
			Byte[] inputBytes = Encoding.Default.GetBytes(this.SerializeToString());
			s.Write(inputBytes, 0, inputBytes.Length);
        }

		public void Deserialize(FileStream s)
		{
			Byte[] bytes = new Byte[s.Length];
			Int32 numBytesToRead = (Int32)s.Length;
			Int32 numBytesRead = 0;
			while (numBytesToRead > 0)
			{
				// Read may return anything from 0 to numBytesToRead.
                Int32 n = s.Read(bytes, numBytesRead, numBytesToRead);

				// Break when the end of the file is reached.
				if (n == 0)
					break;

				numBytesRead += n;
				numBytesToRead -= n;
			}
			String serializedList = Encoding.Default.GetString(bytes);
			this.DeserializeToString(serializedList);
        }
	}
}