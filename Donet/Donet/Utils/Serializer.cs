using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Donet.Utils
{
    public enum NetworkSerializeMode
    {
        Serialize,
        Deserialize,
    }

    public interface INetworkSerializable
    {
        public void Serialize(Serializer serializer);
    }

    [Serializable]
    public sealed class Serializer
    {
        private NetworkSerializeMode mode;
        private ArraySegment<byte> buffer;
        private ushort offset;
        private static readonly Encoding encoding = Encoding.Unicode;


        public void Open(NetworkSerializeMode mode, ArraySegment<byte> buffer, ushort offset = 0)
        {
            this.mode = mode;
            this.buffer = buffer;
            this.offset = offset;
        }
        public ushort GetOffset() => offset;
        public void SetOffset(ushort offset)
        {
            if (offset >= buffer.Count)
                throw new ArgumentOutOfRangeException(nameof(offset));
            this.offset = offset;
        }

        #region serializable
        public void SerializeObject<T>(ref T value) where T : INetworkSerializable
        {
            value.Serialize(this);
        }
        public void SerializeObject<T>(ref T[] array) where T : INetworkSerializable
        {
            ushort count = 0;
            if (mode == NetworkSerializeMode.Serialize)
                count = (ushort)array.Length;
            Serialize(ref count);

            if (mode == NetworkSerializeMode.Deserialize)
                array = new T[count];
            for (int i = 0; i < count; i++)
                SerializeObject(ref array[i]);
        }
        public void SerializeObject<T>(ref List<T> list) where T : INetworkSerializable, new()
        {
            ushort count = 0;
            if (mode == NetworkSerializeMode.Serialize)
                count = (ushort)list.Count;
            Serialize(ref count);

            if (mode == NetworkSerializeMode.Deserialize)
                list = new List<T>(count);

            for (int i = 0; i < count; i++)
            {
                if (mode == NetworkSerializeMode.Serialize)
                {
                    INetworkSerializable value = list[i];
                    SerializeObject(ref value);
                }
                else
                {
                    T value = new T();
                    SerializeObject(ref value);
                    list.Add(value);
                }
            }
        }
        #endregion

        #region value
        public unsafe void Serialize<T>(ref T value) where T : unmanaged
        {
            ushort size = (ushort)sizeof(T);

            if (buffer.Count - offset < size)
                throw new Exception("value bigger then buffer");

            if (mode == NetworkSerializeMode.Serialize)
                MemoryMarshal.Write(new Span<byte>(buffer.Array, buffer.Offset + offset, size), ref value);
            else if (mode == NetworkSerializeMode.Deserialize)
                value = MemoryMarshal.Read<T>(new Span<byte>(buffer.Array, buffer.Offset + offset, size));
            offset += size;
        }
        public void Serialize<T>(ref T[] array) where T : unmanaged
        {
            ushort count = 0;
            if (mode == NetworkSerializeMode.Serialize)
                count = (ushort)array.Length;
            Serialize(ref count);

            if (mode == NetworkSerializeMode.Deserialize)
                array = new T[count];
            for (int i = 0; i < count; i++)
                Serialize(ref array[i]);
        }
        public void Serialize<T>(ref List<T> list) where T : unmanaged
        {
            ushort count = 0;
            if (mode == NetworkSerializeMode.Serialize)
                count = (ushort)list.Count;
            Serialize(ref count);

            if (mode == NetworkSerializeMode.Deserialize)
                list = new List<T>(count);

            for (int i = 0; i < count; i++)
            {
                if (mode == NetworkSerializeMode.Serialize)
                {
                    T value = list[i];
                    Serialize(ref value);
                }
                else
                {
                    T value = default;
                    Serialize(ref value);
                    list.Add(value);
                }
            }
        }
        #endregion

        #region string
        public void Serialize(ref string value)
        {
            value ??= string.Empty;

            ushort size = 0;
            if (mode == NetworkSerializeMode.Serialize)
                size = (ushort)encoding.GetByteCount(value);
            Serialize(ref size);

            if (buffer.Count - offset < size)
                throw new Exception("string bigger then buffer");

            if (mode == NetworkSerializeMode.Serialize)
            {
                int written = encoding.GetBytes(value, new Span<byte>(buffer.Array, buffer.Offset + offset, size));
                if (written != size)
                    throw new Exception("invalid write on buffer");
            }
            else
            {
                value = encoding.GetString(buffer.Array, buffer.Offset + offset, size);
            }

            offset += size;
        }
        public void Serialize(ref string[] array)
        {
            ushort count = 0;
            if (mode == NetworkSerializeMode.Serialize)
                count = (ushort)array.Length;
            Serialize(ref count);

            if (mode == NetworkSerializeMode.Deserialize)
                array = new string[count];
            for (int i = 0; i < count; i++)
                Serialize(ref array[i]);
        }
        public void Serialize(ref List<string> list)
        {
            ushort count = 0;
            if (mode == NetworkSerializeMode.Serialize)
                count = (ushort)list.Count;
            Serialize(ref count);

            if (mode == NetworkSerializeMode.Deserialize)
                list = new List<string>(count);

            for (int i = 0; i < count; i++)
            {
                if (mode == NetworkSerializeMode.Serialize)
                {
                    string value = list[i];
                    Serialize(ref value);
                }
                else
                {
                    string value = string.Empty;
                    Serialize(ref value);
                    list.Add(value);
                }
            }
        }
        #endregion
    }
}
