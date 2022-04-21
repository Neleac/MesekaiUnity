public class GamePacket
{
	private GamePacketStream buffer;

	public GamePacket(short message_id)
	{
		buffer = new GamePacketStream(message_id);
	}

	public void addShort16(short val)
	{
		buffer.add(val);
	}

	public void addInt32(int val)
	{
		buffer.add(val);
	}

	public void addLong64(long val)
	{
		buffer.add(val);
	}

	public void addBool(bool val)
	{
		buffer.add(val);
	}

	public void addBytes(byte[] bytes)
	{
		buffer.add(bytes);
	}

	public void addString(string val)
	{
		buffer.add((short)val.Length);
		buffer.add(val);
	}

	public void addFloat32(float val)
	{
		buffer.add(val);
	}

	public int size()
	{
		return buffer.size();
	}

	public byte[] getBytes()
	{
		return buffer.toByteArray();
	}
}
