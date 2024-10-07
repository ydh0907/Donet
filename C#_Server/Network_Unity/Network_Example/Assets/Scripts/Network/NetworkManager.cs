using Donet;
using Network_Example_Server.Packets;
using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance { get; private set; }

    private Connector connector;
    public ServerSession session;

    public Queue<Action> jobQueue = new();
    private object locker = new object();

    public Action OnConnected = null;
    public Action OnDisconnected = null;

    private void Awake()
    {
        Instance = this;

        PacketFactory.InitializePacket<PacketEnum>();
    }
    private void Start()
    {
        connector = new Connector();

        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3001);
        connector.Connect(endPoint, () => new ServerSession(), HandleServerConnected);
    }
    private void Update()
    {
        lock (locker)
            while (jobQueue.Count > 0)
                jobQueue.Dequeue()?.Invoke();
    }

    public void AddJob(Action job)
    {
        lock (locker)
            jobQueue.Enqueue(job);
    }
    private void HandleServerConnected(Session session)
    {
        this.session = session as ServerSession;
        OnConnected?.Invoke();
    }

    public void Disconnect()
    {
        if (session.Connected)
        {
            session.Disconnect();
        }

        OnDisconnected?.Invoke();
    }
}
