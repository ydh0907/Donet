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
        public abstract bool Serialize(Serializer serializer);
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


        #region serializable
        public bool SerializeSerializable<T>(ref T value) where T : INetworkSerializable
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
        public bool SerializeSerializable<T>(ref T[] array) where T : INetworkSerializable
        {
            ushort count = 0;
            if (mode == NetworkSerializeMode.Serialize)
                count = (ushort)array.Length;
            bool result = SerializeValue(ref count);

            if (mode == NetworkSerializeMode.Deserialize)
                array = new T[count];
            for (int i = 0; i < count && result; i++)
                result = SerializeSerializable(ref array[i]);
            return result;
        }
        public bool SerializeSerializable<T>(ref List<T> list) where T : INetworkSerializable, new()
        {
            ushort count = 0;
            if (mode == NetworkSerializeMode.Serialize)
            {
                count = (ushort)list.Count;
                bool result = SerializeValue(ref count);
                for (int i = 0; i < count && result; i++)
                {
                    T value = list[i];
                    result = SerializeSerializable<T>(ref value);
                }
                return result;
            }
            else // Deserialize Mode
            {
                bool result = SerializeValue(ref count);
                list = new List<T>(count);
                for (int i = 0; i < count && result; i++)
                {
                    T value = new T();
                    result = SerializeSerializable<T>(ref value);
                    list[i] = value;
                }
                return result;
            }
        }
        #endregion
        #region value
        unsafe public bool SerializeValue<T>(ref T value) where T : unmanaged
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
        public bool SerializeArray<T>(ref T[] array) where T : unmanaged
        {
            ushort count = 0;
            if (mode == NetworkSerializeMode.Serialize)
                count = (ushort)array.Length;

            bool result = SerializeValue(ref count);

            if (mode == NetworkSerializeMode.Deserialize)
                array = new T[count];
            for (int i = 0; i < count && result; i++)
                result = SerializeValue(ref array[i]);
            return result;
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
        #endregion value
        #region string
        public bool SerializeValue(ref string value)
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
                    return false;
                }

                if (mode == NetworkSerializeMode.Serialize)
                {
                    int written = Encoding.Unicode.GetBytes(value, new Span<byte>(buffer.Array, buffer.Offset + offset, size));
                    if (written != size)
                    {
                        LogError(value, size, "convert failed...");
                        return false;
                    }
                }
                else if (mode == NetworkSerializeMode.Deserialize)
                {
                    value = Encoding.Unicode.GetString(buffer.Array, buffer.Offset + offset, size);
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
        public bool SerializeArray(ref string[] array)
        {
            ushort count = 0;
            if (mode == NetworkSerializeMode.Serialize)
                count = (ushort)array.Length;

            bool result = SerializeValue(ref count);

            if (mode == NetworkSerializeMode.Deserialize)
                array = new string[count];
            for (int i = 0; i < count && result; i++)
                result = SerializeValue(ref array[i]);
            return result;
        }
        public void SerializeArray(ref List<string> list)
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
