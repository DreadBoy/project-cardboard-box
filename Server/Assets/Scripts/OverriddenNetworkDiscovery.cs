using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class OverriddenNetworkDiscovery : NetworkDiscovery
{

    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        NetworkManager.singleton.networkAddress = fromAddress;
        NetworkManager.singleton.StartClient();
    }

}