using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

namespace Cardboard
{
    public interface INetworkConnection
    {
        SmartEvent<CommandArgs> CommandReceived { get; set; }
    }

    public class SmartConnection : INetworkConnection
    {
        public NetworkConnection conn;

        public SmartConnection(NetworkConnection connection)
        {
            conn = connection;
            conn.RegisterHandler(49, OnCommandReceived);
        }

        public SmartEvent<CommandArgs> CommandReceived { get; set; }

        void OnCommandReceived(NetworkMessage netMsg)
        {
            var message = netMsg.ReadMessage<StringMessage>();
            Debug.Log(message.value);
        }
    }

    public class MockConnection : INetworkConnection
    {
        public SmartEvent<CommandArgs> CommandReceived { get; set; }
    }
}
