using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResponseAnimateEventArgs : ExtendedEventArgs
{
	public string playerName { get; set; }
    public Dictionary<string, bool> animStateDict { get; set; }
	public Dictionary<string, float> animValDict { get; set; }

	public ResponseAnimateEventArgs()
	{
		event_id = Constants.SMSG_ANIMATE;
	}
}

public class ResponseAnimate : NetworkResponse
{
	private string playerName;
    private Dictionary<string, bool> animStateDict;
	private Dictionary<string, float> animValDict;

	public ResponseAnimate()
	{
        animStateDict = new Dictionary<string, bool>();
        animStateDict.Add("grounded", true);
        animStateDict.Add("jump", false);
        animStateDict.Add("freeFall", false);

        animValDict = new Dictionary<string, float>();
        animValDict.Add("animationBlend", 0);
		animValDict.Add("inputMagnitude", 0);
	}

	public override void parse()
	{
		playerName = DataReader.ReadString(dataStream);
        animStateDict["grounded"] = DataReader.ReadBool(dataStream);
        animStateDict["jump"] = DataReader.ReadBool(dataStream);
        animStateDict["freeFall"] = DataReader.ReadBool(dataStream);
        animValDict["animationBlend"] = DataReader.ReadFloat(dataStream);
        animValDict["inputMagnitude"] = DataReader.ReadFloat(dataStream);
	}

	public override ExtendedEventArgs process()
	{
		ResponseAnimateEventArgs args = new ResponseAnimateEventArgs
		{
			playerName = playerName,
            animStateDict = animStateDict,
            animValDict = animValDict
		};

		return args;
	}
}