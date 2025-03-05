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
        public abstract void Serialize(Serializer serializer);
    }

    public static class LocalSerializer
    {
        private static ThreadLocal<Serializer> serializer = new ThreadLocal<Serializer>(() => new Serializer());
        public static Serializer Serializer => serializer.Value;
        public static void Dispose() => serializer.Dispose();
    }

    public class Serializer
    {
        private NetworkSerializeMode mode;
        private ArraySegment<byte> buffer;
        private ushort offset;

        public void Open(NetworkSerializeMode mode, ArraySegment<byte> buffer, ushort offset = 0)
        {
            this.mode = mode;
            this.buffer = buffer;
            this.offset = offset;
        }

        public int Close()
        {
            return offset;
        }

        public bool SetOffset(ushort offset)
        {
            if (offset >= buffer.Count)
                return false;
            this.offset = offset;
            return true;
        }

        private void LogError<T>(T value, int size, string cause)
        {
            Console.WriteLine($"[Serializer] Convert Failed... Mode : {mode}, Type : {value?.GetType().Name}, Size : {size}, Value : {value}, Space : {buffer.Count - offset}, Cause : {cause}");
        }

        #region value
        public void SerializeValue(ref INetworkSerializable value)
        {
            if (value == null)
            {
                LogError(value, -1, "value is null...");
                return;
            }

            value.Serialize(this);
        }
        unsafe public void SerializeValue<T>(ref T value) where T : unmanaged
        {
            ushort size = (ushort)sizeof(T);

            if (buffer.Count - offset < size)
            {
                LogError(value, size, "out of range...");
                return;
            }

            try
            {
                if (mode == NetworkSerializeMode.Serialize)
                    MemoryMarshal.Write<T>(new Span<byte>(buffer.Array, buffer.Offset + offset, size), ref value);
                else if (mode == NetworkSerializeMode.Deserialize)
                    value = MemoryMarshal.Read<T>(new Span<byte>(buffer.Array, buffer.Offset + offset, size));
                offset += size;
            }
            catch (Exception ex)
            {
                LogError(value, size, ex.Message);
            }
        }
        public void SerializeValue(ref string value)
        {
            if (value == null)
                value = string.Empty;

            ushort size = 0;
            try
            {
                if (mode == NetworkSerializeMode.Serialize)
                    size = (ushort)Encoding.Unicode.GetByteCount(value);
                SerializeValue(ref size);

                if (buffer.Count - offset < size)
                {
                    LogError(value, size, "out of range...");
                    return;
                }

                if (mode == NetworkSerializeMode.Serialize)
                {
                    int written = Encoding.Unicode.GetBytes(value, new Span<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset));
                    if (written == size)
                        offset += size;
                    else
                        LogError(value, size, "convert failed...");
                }
                else if (mode == NetworkSerializeMode.Deserialize)
                {
                    value = Encoding.Unicode.GetString(buffer.Array, buffer.Offset + offset, size);
                    offset += size;
                }
            }
            catch (Exception ex)
            {
                LogError(value, size, ex.Message);
            }
        }
        #endregion value
        #region array
        public void SerializeArray<T>(ref T[] array) where T : unmanaged
        {
            ushort count = 0;
            if (mode == NetworkSerializeMode.Serialize)
                count = (ushort)array.Length;

            SerializeValue(ref count);

            if (mode == NetworkSerializeMode.Deserialize)
                array = new T[count];
            for (int i = 0; i < count; i++)
                SerializeValue(ref array[i]);
        }
        public void SerializeList<T>(ref List<T> list) where T : unmanaged
        {
            ushort count = 0;
            if (mode == NetworkSerializeMode.Serialize)
            {
                count = (ushort)list.Count;
                SerializeValue(ref count);
                for (int i = 0; i < count; i++)
                {
                    var value = list[i];
                    SerializeValue(ref value);
                }
            }
            else if (mode == NetworkSerializeMode.Deserialize)
            {
                SerializeValue(ref count);
                list = new List<T>(count);
                T value = new T();
                for (int i = 0; i < count; i++)
                {
                    SerializeValue(ref value);
                    list.Add(value);
                }
            }
        }
        public void SerializeArray(ref string[] array)
        {
            ushort count = 0;
            if (mode == NetworkSerializeMode.Serialize)
                count = (ushort)array.Length;

            SerializeValue(ref count);
            if (mode == NetworkSerializeMode.Deserialize)
                array = new string[count];
            for (int i = 0; i < count; i++)
                SerializeValue(ref array[i]);
        }
        public void SerializeList(ref List<string> list)
        {
            ushort count = 0;
            if (mode == NetworkSerializeMode.Serialize)
            {
                count = (ushort)list.Count;
                SerializeValue(ref count);
                for (int i = 0; i < count; i++)
                {
                    var value = list[i];
                    SerializeValue(ref value);
                }
            }
            else if (mode == NetworkSerializeMode.Deserialize)
            {
                SerializeValue(ref count);
                list = new List<string>(count);
                string value = string.Empty;
                for (int i = 0; i < count; i++)
                {
                    SerializeValue(ref value);
                    list.Add(value);
                }
            }
        }
        #endregion

        public static bool WriteUShort(ushort value, ArraySegment<byte> buffer, int offset = 0)
        {
            return BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + offset, sizeof(ushort)), value);
        }
    }
}
