using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResponseJoinEventArgs : ExtendedEventArgs
{
	public string name { get; set; }

	public ResponseJoinEventArgs()
	{
		event_id = Constants.SMSG_JOIN;
	}
}

public class ResponseJoin : NetworkResponse
{
	private string name;

	public ResponseJoin()
	{
	}

	public override void parse()
	{
		name = DataReader.ReadString(dataStream);
	}

	public override ExtendedEventArgs process()
	{
		ResponseJoinEventArgs args = new ResponseJoinEventArgs
		{
			name = name
		};

		return args;
	}
}
