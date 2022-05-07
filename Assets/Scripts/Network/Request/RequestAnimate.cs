using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestAnimate : NetworkRequest
{
	public RequestAnimate()
	{
		request_id = Constants.CMSG_ANIMATE;
	}

	public void send(string playerName, Dictionary<string, bool> animStateDict, Dictionary<string, float> animValDict)
	{
		packet = new GamePacket(request_id);
        packet.addString(playerName);
        packet.addBool(animStateDict["grounded"]);
        packet.addBool(animStateDict["jump"]);
        packet.addBool(animStateDict["freeFall"]);
        packet.addFloat32(animValDict["animationBlend"]);
        packet.addFloat32(animValDict["inputMagnitude"]);
	}
}