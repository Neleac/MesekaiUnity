using System;
using System.IO;
using System.Text;

public class GamePacketStream
{
	private MemoryStream stream;

	public GamePacketStream(short message_id)
	{
		stream = new MemoryStream();

		stream.WriteByte(0xff);
		stream.WriteByte(0xff);

		add(message_id);
	}

	public void add(byte[] bytes)
	{
		stream.Write(bytes, 0, bytes.Length);
	}

	public void add(short val)
	{
		add(BitConverter.GetBytes(val));
	}

	public void add(int val)
	{
		add(BitConverter.GetBytes(val));
	}

	public void add(long val)
	{
		add(BitConverter.GetBytes(val));
	}

	public void add(float val)
	{
		add(BitConverter.GetBytes(val));
	}

	public void add(bool val)
	{
		add(BitConverter.GetBytes(val));
	}

	public void add(string val)
	{
		add(Encoding.UTF8.GetBytes(val));
	}

	public byte[] toByteArray()
	{
		byte[] bytes = stream.ToArray();

		bytes[0] = (byte)((stream.Length - 2) & 0xff);
		bytes[1] = (byte)((stream.Length - 2) >> 8);

		return bytes;
	}

	public int size()
	{
		return (int)stream.Length;
	}
}
