using Donet.Sessions;
using Donet.Utils;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Server.Packets
{
    public struct InitializePacket : IPacket
    {
        public ulong id;

        public IPacket CreateInstance()
        {
            return new InitializePacket();
        }

        public void OnReceived(Session session)
        {
            ulong id = this.id;
            NetworkManager.Instance.Jobs.Enqueue(() =>
            {
                NetworkManager.Instance.session.Id = id;
                Debug.Log($"[Session] session id is set to {id}");
                SceneManager.LoadScene(1);
            });
        }

        public void Serialize(Serializer serializer)
        {
            serializer.Serialize(ref id);
        }
    }
}
