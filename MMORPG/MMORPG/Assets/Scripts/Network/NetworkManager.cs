using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

using Donet.Connection;
using Donet.Sessions;
using Donet.Utils;

using Server.Packets;

using UnityEngine;

using Random = UnityEngine.Random;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance { get; private set; }
    public ConcurrentQueue<Action> Jobs { get; private set; } = new ConcurrentQueue<Action>();

    public Session session = new Session();

    private void Awake()
    {
        DontDestroyOnLoad(this);

        MemoryPool.Initialize();
        PacketFactory.Initialize(
            new InitializePacket()
        );

        Instance = this;

        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("172.31.1.212"), 9977 + Random.Range(0, 3));
        Socket socket = Connector.Connect(endPoint);
        session.Initialize(0, socket);
    }

    private void OnDestroy()
    {
        session.Close();

        PacketFactory.Dispose();
        MemoryPool.Dispose();
    }

    private void Update()
    {
        while (Jobs.Count > 0)
        {
            if (Jobs.TryDequeue(out Action action))
                action?.Invoke();
        }
    }
}
