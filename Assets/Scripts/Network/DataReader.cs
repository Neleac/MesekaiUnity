using System;
using System.IO;
using System.Text;

public class DataReader
{

	public static short ReadShort(MemoryStream dataStream)
	{
		byte[] buffer = new byte[2];
		dataStream.Read(buffer, 0, 2);
		return BitConverter.ToInt16(buffer, 0); ;
	}

	public static int ReadInt(MemoryStream dataStream)
	{
		byte[] buffer = new byte[4];
		dataStream.Read(buffer, 0, 4);
		return BitConverter.ToInt32(buffer, 0); ;
	}

	public static float ReadFloat(MemoryStream dataStream)
	{
		byte[] buffer = new byte[4];
		dataStream.Read(buffer, 0, 4);
		return BitConverter.ToSingle(buffer, 0); ;
	}

	public static bool ReadBool(MemoryStream dataStream)
	{
		byte[] buffer = new byte[1];
		dataStream.Read(buffer, 0, 1);
		return BitConverter.ToBoolean(buffer, 0); ;
	}

	public static string ReadString(MemoryStream dataStream)
	{
		short length = ReadShort(dataStream);

		byte[] buffer = new byte[length];
		dataStream.Read(buffer, 0, length);
		buffer = Encoding.Convert(Encoding.GetEncoding("iso-8859-1"), Encoding.UTF8, buffer);
		return Encoding.UTF8.GetString(buffer, 0, length);
	}
}