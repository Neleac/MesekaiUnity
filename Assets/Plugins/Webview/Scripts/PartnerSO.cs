using UnityEngine;

public class PartnerSO : ScriptableObject
{
    public string Subdomain = null;

    public string GetUrl()
    {
        return $"https://{GetSubdomain()}.readyplayer.me/avatar";
    }

    public string GetSubdomain()
    {
        return Subdomain;
    }
}
