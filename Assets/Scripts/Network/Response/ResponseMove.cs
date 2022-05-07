using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResponseMoveEventArgs : ExtendedEventArgs
{
	public string playerName { get; set; }
    public Vector3 position { get; set; }
	public Vector3 rotation { get; set; }
	public float[] jointAngles { get; set; }

	public ResponseMoveEventArgs()
	{
		event_id = Constants.SMSG_MOVE;
	}
}

public class ResponseMove : NetworkResponse
{
	private string playerName;
    private float xPos, yPos, zPos;	// avatar root position
	private float xRot, yRot, zRot;	// avatar root rotation
	private float[] jointAngles;	// armature joints angles

	public ResponseMove()
	{
	}

	public override void parse()
	{
		playerName = DataReader.ReadString(dataStream);
        xPos = DataReader.ReadFloat(dataStream);
        yPos = DataReader.ReadFloat(dataStream);
        zPos = DataReader.ReadFloat(dataStream);
		xRot = DataReader.ReadFloat(dataStream);
        yRot = DataReader.ReadFloat(dataStream);
        zRot = DataReader.ReadFloat(dataStream);

		// int nAngles = DataReader.ReadInt(dataStream);
		// jointAngles = new float[nAngles];
		// for (int i = 0; i < jointAngles.Length; i++)
		// {
		// 	jointAngles[i] = DataReader.ReadFloat(dataStream);
		// }
	}

	public override ExtendedEventArgs process()
	{
		ResponseMoveEventArgs args = new ResponseMoveEventArgs
		{
			playerName = playerName,
            position = new Vector3(xPos, yPos, zPos),
			rotation = new Vector3(xRot, yRot, zRot),
			//jointAngles = jointAngles
		};

		return args;
	}
}