using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResponseMoveEventArgs : ExtendedEventArgs
{
	public string playerName { get; set; }
    public Vector3 transform { get; set; }

	public ResponseMoveEventArgs()
	{
		event_id = Constants.SMSG_MOVE;
	}
}

public class ResponseMove : NetworkResponse
{
	private string playerName;
    private float x, y, z;

	public ResponseMove()
	{
	}

	public override void parse()
	{
		playerName = DataReader.ReadString(dataStream);
        x = DataReader.ReadFloat(dataStream);
        y = DataReader.ReadFloat(dataStream);
        z = DataReader.ReadFloat(dataStream);
	}

	public override ExtendedEventArgs process()
	{
		ResponseMoveEventArgs args = new ResponseMoveEventArgs
		{
			playerName = playerName,
            transform = new Vector3(x, y, z),
		};

		return args;
	}
}