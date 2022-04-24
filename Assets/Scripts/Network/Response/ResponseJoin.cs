using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResponseJoinEventArgs : ExtendedEventArgs
{
	public bool success { get; set; }
	public string playerName { get; set; }
	public string[] otherPlayers { get; set; }

	public ResponseJoinEventArgs()
	{
		event_id = Constants.SMSG_JOIN;
	}
}

public class ResponseJoin : NetworkResponse
{
	private bool success;
	private string playerName;
	private string[] otherPlayers;

	public ResponseJoin()
	{
	}

	public override void parse()
	{
		success = DataReader.ReadBool(dataStream);
		playerName = DataReader.ReadString(dataStream);
		
		int nOtherPlayers = DataReader.ReadInt(dataStream);
		otherPlayers = new string[nOtherPlayers];
		for (int i = 0; i < otherPlayers.Length; i++)
		{
			otherPlayers[i] = DataReader.ReadString(dataStream);
		}
	}

	public override ExtendedEventArgs process()
	{
		ResponseJoinEventArgs args = new ResponseJoinEventArgs
		{
			success = success,
			playerName = playerName,
			otherPlayers = otherPlayers
		};

		return args;
	}
}
