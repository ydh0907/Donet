using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Donet
{
    public enum NetworkSerializeMode
    {
        Serialize,
        Deserialize,
    }

    public interface INetworkSerializable
    {
        bool Serialize(Serializer serializer);
    }

    public static class LocalSerializer
    {
        private static readonly ThreadLocal<Serializer> serializer = new ThreadLocal<Serializer>(() => new Serializer());
        public static Serializer Serializer => serializer.Value;
        public static void Dispose() => serializer.Dispose();
    }

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
        public int Close() => offset;
        public bool SetOffset(ushort offset)
        {
            if (offset >= buffer.Count)
                return false;
            this.offset = offset;
            return true;
        }
        private void LogError<T>(T value, int size, string cause)
        {
            Console.WriteLine($"[Serializer] Convert Failed... Mode: {mode}, Type: {value?.GetType().Name}, Size: {size}, Value: {value}, Space: {buffer.Count - offset}, Cause: {cause}");
        }

        #region serializable
        public bool SerializeObject<T>(ref T value) where T : INetworkSerializable
        {
            try
            {
                return value.Serialize(this);
            }
            catch (Exception ex)
            {
                LogError(value, -1, ex.Message);
                return false;
            }
        }
        public bool SerializeObject<T>(ref T[] array) where T : INetworkSerializable
        {
            ushort count = 0;
            if (mode == NetworkSerializeMode.Serialize)
                count = (ushort)array.Length;

            if (!Serialize(ref count))
                return false;

            if (mode == NetworkSerializeMode.Deserialize)
                array = new T[count];

            for (int i = 0; i < count; i++)
            {
                if (!SerializeObject(ref array[i]))
                    return false;
            }
            return true;
        }
        public bool SerializeObject<T>(ref List<T> list) where T : INetworkSerializable, new()
        {
            ushort count = 0;
            if (mode == NetworkSerializeMode.Serialize)
                count = (ushort)list.Count;

            if (!Serialize(ref count))
                return false;

            if (mode == NetworkSerializeMode.Deserialize)
                list = new List<T>(count);

            for (int i = 0; i < count; i++)
            {
                if (mode == NetworkSerializeMode.Serialize)
                {
                    INetworkSerializable value = list[i];
                    if (!(value is T) || !SerializeObject(ref value))
                        return false;
                }
                else
                {
                    T value = new T();
                    if (!SerializeObject(ref value))
                        return false;
                    list.Add(value);
                }
            }
            return true;
        }
        #endregion

        #region value
        public unsafe bool Serialize<T>(ref T value) where T : unmanaged
        {
            ushort size = (ushort)sizeof(T);

            if (buffer.Count - offset < size)
            {
                LogError(value, size, "out of range...");
                return false;
            }

            try
            {
                if (mode == NetworkSerializeMode.Serialize)
                    MemoryMarshal.Write<T>(new Span<byte>(buffer.Array, buffer.Offset + offset, size), ref value);
                else if (mode == NetworkSerializeMode.Deserialize)
                    value = MemoryMarshal.Read<T>(new Span<byte>(buffer.Array, buffer.Offset + offset, size));
                offset += size;
                return true;
            }
            catch (Exception ex)
            {
                LogError(value, size, ex.Message);
                return false;
            }
        }
        public bool Serialize<T>(ref T[] array) where T : unmanaged
        {
            ushort count = 0;
            if (mode == NetworkSerializeMode.Serialize)
                count = (ushort)array.Length;

            if (!Serialize(ref count))
                return false;

            if (mode == NetworkSerializeMode.Deserialize)
                array = new T[count];

            for (int i = 0; i < count; i++)
            {
                if (!Serialize(ref array[i]))
                    return false;
            }
            return true;
        }
        public bool Serialize<T>(ref List<T> list) where T : unmanaged
        {
            ushort count = 0;
            if (mode == NetworkSerializeMode.Serialize)
                count = (ushort)list.Count;

            if (!Serialize(ref count))
                return false;

            if (mode == NetworkSerializeMode.Deserialize)
                list = new List<T>(count);

            for (int i = 0; i < count; i++)
            {
                if (mode == NetworkSerializeMode.Serialize)
                {
                    T value = list[i];
                    if (!Serialize(ref value))
                        return false;
                }
                else
                {
                    T value = default;
                    if (!Serialize(ref value))
                        return false;
                    list.Add(value);
                }
            }
            return true;
        }
        #endregion

        #region string
        public bool Serialize(ref string value)
        {
            value ??= string.Empty;

            ushort size = 0;
            try
            {
                if (mode == NetworkSerializeMode.Serialize)
                    size = (ushort)encoding.GetByteCount(value);

                if (!Serialize(ref size))
                    return false;

                if (buffer.Count - offset < size)
                {
                    LogError(value, size, "out of range...");
                    return false;
                }

                if (mode == NetworkSerializeMode.Serialize)
                {
                    int written = encoding.GetBytes(value, new Span<byte>(buffer.Array, buffer.Offset + offset, size));
                    if (written != size)
                    {
                        LogError(value, size, "convert failed...");
                        return false;
                    }
                }
                else
                {
                    value = encoding.GetString(buffer.Array, buffer.Offset + offset, size);
                }
                offset += size;
                return true;
            }
            catch (Exception ex)
            {
                LogError(value, size, ex.Message);
                return false;
            }
        }
        public bool Serialize(ref string[] array)
        {
            ushort count = 0;
            if (mode == NetworkSerializeMode.Serialize)
                count = (ushort)array.Length;

            if (!Serialize(ref count))
                return false;

            if (mode == NetworkSerializeMode.Deserialize)
                array = new string[count];

            for (int i = 0; i < count; i++)
            {
                if (!Serialize(ref array[i]))
                    return false;
            }
            return true;
        }
        public bool Serialize(ref List<string> list)
        {
            ushort count = 0;
            if (mode == NetworkSerializeMode.Serialize)
                count = (ushort)list.Count;

            if (!Serialize(ref count))
                return false;

            if (mode == NetworkSerializeMode.Deserialize)
                list = new List<string>(count);

            for (int i = 0; i < count; i++)
            {
                if (mode == NetworkSerializeMode.Serialize)
                {
                    string value = list[i];
                    if (!Serialize(ref value))
                        return false;
                }
                else
                {
                    string value = string.Empty;
                    if (!Serialize(ref value))
                        return false;
                    list.Add(value);
                }
            }
            return true;
        }
        #endregion

        public static bool WriteUShort(ushort value, ArraySegment<byte> buffer, int offset = 0)
        {
            return BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + offset, sizeof(ushort)), value);
        }
    }
}
