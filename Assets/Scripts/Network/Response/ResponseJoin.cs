using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResponseJoinEventArgs : ExtendedEventArgs
{
	public string playerName { get; set; }

	public ResponseJoinEventArgs()
	{
		event_id = Constants.SMSG_JOIN;
	}
}

public class ResponseJoin : NetworkResponse
{
	private string playerName;

	public ResponseJoin()
	{
	}

	public override void parse()
	{
		playerName = DataReader.ReadString(dataStream);
	}

	public override ExtendedEventArgs process()
	{
		ResponseJoinEventArgs args = new ResponseJoinEventArgs
		{
			playerName = playerName
		};

		return args;
	}
}
