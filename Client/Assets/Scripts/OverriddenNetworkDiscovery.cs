using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class OverriddenNetworkDiscovery : NetworkDiscovery
{

    public delegate void DiscoveredAction(string address);
    public event DiscoveredAction OnDiscovered;


    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        OnDiscovered(fromAddress);
    }

}
