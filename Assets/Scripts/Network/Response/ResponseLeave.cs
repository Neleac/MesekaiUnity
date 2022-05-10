using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResponseLeaveEventArgs : ExtendedEventArgs
{
	public int user_id { get; set; } // The user_id of whom who sent the request

	public ResponseLeaveEventArgs()
	{
		event_id = Constants.SMSG_LEAVE;
	}
}

public class ResponseLeave : NetworkResponse
{
	private int user_id;

	public ResponseLeave()
	{
	}

	public override void parse()
	{
		user_id = DataReader.ReadInt(dataStream);
	}

	public override ExtendedEventArgs process()
	{
		ResponseLeaveEventArgs args = new ResponseLeaveEventArgs
		{
			user_id = user_id
		};

		return args;
	}
}
