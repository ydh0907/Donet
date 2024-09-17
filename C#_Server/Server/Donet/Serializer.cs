using System;
using System.Collections.Generic;
using System.Text;

namespace Donet
{
    public enum NetworkSerializeMode
    {
        Serialize,
        Deserialize,
    }

    public interface INetworkSerializable
    {
        public abstract void Serialize(Serializer serializer); // return write count
    }

    public class Serializer
    {
        private NetworkSerializeMode mode = NetworkSerializeMode.Serialize;
        private ArraySegment<byte> buffer;
        private ushort offset;
        private bool error;
        public bool Success => !error;

        public void Open(NetworkSerializeMode mode, ArraySegment<byte> buffer, ushort offset = 4)
        {
            this.mode = mode;
            this.buffer = buffer;
            this.offset = offset;
            error = false;
        }

        public int Close()
        {
            return offset;
        }

        private void LogError<T>(T value, int size, string cause)
        {
            Console.WriteLine($"[Serializer] Convert Failed... Mode : {mode}, Type : {value.GetType().Name}, Size : 1, Value : {value}, Space : {buffer.Count - offset}, Cause : {cause}");
        }

        public void SerializeValue<T>(ref T value) where T : INetworkSerializable
        {
            value.Serialize(this);
        }
        public void SerializeValue(ref byte value)
        {
            if (!Success) return;

            bool convertible = buffer.Count - offset >= sizeof(byte);
            if (Success && convertible)
            {
                try
                {
                    if (mode == NetworkSerializeMode.Serialize)
                        buffer.Array[buffer.Offset + offset] = value;
                    else if (mode == NetworkSerializeMode.Deserialize)
                        value = buffer.Array[buffer.Offset + offset];
                    ++offset;
                }
                catch (Exception ex)
                {
                    error = true;
                    LogError(value, 1, ex.Message);
                }
            }
            else
            {
                error = true;
                LogError(value, 1, "out of range");
            }
        }
        public void SerializeValue(ref bool value)
        {
            if (!Success) return;

            ushort size = sizeof(bool);
            bool convertible = buffer.Count - offset >= size;
            if (Success && convertible)
            {
                try
                {
                    bool success = true;
                    if (mode == NetworkSerializeMode.Serialize)
                        success = BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset), value);
                    else if (mode == NetworkSerializeMode.Deserialize)
                        value = BitConverter.ToBoolean(new ReadOnlySpan<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset));
                    if (!success)
                        throw new Exception("convert failed...");
                    offset += size;
                }
                catch (Exception ex)
                {
                    error = true;
                    LogError(value, size, ex.Message);
                }
            }
            else
            {
                error = true;
                LogError(value, size, "out of range");
            }
        }
        public void SerializeValue(ref char value)
        {
            if (!Success) return;

            ushort size = sizeof(char);
            bool convertible = buffer.Count - offset >= size;
            if (Success && convertible)
            {
                try
                {
                    bool success = true;
                    if (mode == NetworkSerializeMode.Serialize)
                        success = BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset), value);
                    else if (mode == NetworkSerializeMode.Deserialize)
                        value = BitConverter.ToChar(new ReadOnlySpan<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset));
                    if (!success)
                        throw new Exception("convert failed...");
                    offset += size;
                }
                catch (Exception ex)
                {
                    error = true;
                    LogError(value, size, ex.Message);
                }
            }
            else
            {
                error = true;
                LogError(value, size, "out of range");
            }
        }
        public void SerializeValue(ref short value)
        {
            if (!Success) return;

            ushort size = sizeof(short);
            bool convertible = buffer.Count - offset >= size;
            if (Success && convertible)
            {
                try
                {
                    bool success = true;
                    if (mode == NetworkSerializeMode.Serialize)
                        success = BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset), value);
                    else if (mode == NetworkSerializeMode.Deserialize)
                        value = BitConverter.ToInt16(new ReadOnlySpan<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset));
                    if (!success)
                        throw new Exception("convert failed...");
                    offset += size;
                }
                catch (Exception ex)
                {
                    error = true;
                    LogError(value, size, ex.Message);
                }
            }
            else
            {
                error = true;
                LogError(value, size, "out of range");
            }
        }
        public void SerializeValue(ref ushort value)
        {
            if (!Success) return;

            ushort size = sizeof(ushort);
            bool convertible = buffer.Count - offset >= size;
            if (Success && convertible)
            {
                try
                {
                    bool success = true;
                    if (mode == NetworkSerializeMode.Serialize)
                        success = BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset), value);
                    else if (mode == NetworkSerializeMode.Deserialize)
                        value = BitConverter.ToUInt16(new ReadOnlySpan<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset));
                    if (!success)
                        throw new Exception("convert failed...");
                    offset += size;
                }
                catch (Exception ex)
                {
                    error = true;
                    LogError(value, size, ex.Message);
                }
            }
            else
            {
                error = true;
                LogError(value, size, "out of range");
            }
        }
        public void SerializeValue(ref int value)
        {
            if (!Success) return;

            ushort size = sizeof(int);
            bool convertible = buffer.Count - offset >= size;
            if (Success && convertible)
            {
                try
                {
                    bool success = true;
                    if (mode == NetworkSerializeMode.Serialize)
                        success = BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset), value);
                    else if (mode == NetworkSerializeMode.Deserialize)
                        value = BitConverter.ToInt32(new ReadOnlySpan<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset));
                    if (!success)
                        throw new Exception("convert failed...");
                    offset += size;
                }
                catch (Exception ex)
                {
                    error = true;
                    LogError(value, size, ex.Message);
                }
            }
            else
            {
                error = true;
                LogError(value, size, "out of range");
            }
        }
        public void SerializeValue(ref uint value)
        {
            if (!Success) return;

            ushort size = sizeof(uint);
            bool convertible = buffer.Count - offset >= size;
            if (Success && convertible)
            {
                try
                {
                    bool success = true;
                    if (mode == NetworkSerializeMode.Serialize)
                        success = BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset), value);
                    else if (mode == NetworkSerializeMode.Deserialize)
                        value = BitConverter.ToUInt32(new ReadOnlySpan<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset));
                    if (!success)
                        throw new Exception("convert failed...");
                    offset += size;
                }
                catch (Exception ex)
                {
                    error = true;
                    LogError(value, size, ex.Message);
                }
            }
            else
            {
                error = true;
                LogError(value, size, "out of range");
            }
        }
        public void SerializeValue(ref long value)
        {
            if (!Success) return;

            ushort size = sizeof(long);
            bool convertible = buffer.Count - offset >= size;
            if (Success && convertible)
            {
                try
                {
                    bool success = true;
                    if (mode == NetworkSerializeMode.Serialize)
                        success = BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset), value);
                    else if (mode == NetworkSerializeMode.Deserialize)
                        value = BitConverter.ToInt64(new ReadOnlySpan<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset));
                    if (!success)
                        throw new Exception("convert failed...");
                    offset += size;
                }
                catch (Exception ex)
                {
                    error = true;
                    LogError(value, size, ex.Message);
                }
            }
            else
            {
                error = true;
                LogError(value, size, "out of range");
            }
        }
        public void SerializeValue(ref ulong value)
        {
            if (!Success) return;

            ushort size = sizeof(ulong);
            bool convertible = buffer.Count - offset >= size;
            if (Success && convertible)
            {
                try
                {
                    bool success = true;
                    if (mode == NetworkSerializeMode.Serialize)
                        success = BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset), value);
                    else if (mode == NetworkSerializeMode.Deserialize)
                        value = BitConverter.ToUInt64(new ReadOnlySpan<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset));
                    if (!success)
                        throw new Exception("convert failed...");
                    offset += size;
                }
                catch (Exception ex)
                {
                    error = true;
                    LogError(value, size, ex.Message);
                }
            }
            else
            {
                error = true;
                LogError(value, size, "out of range");
            }
        }
        public void SerializeValue(ref float value)
        {
            if (!Success) return;

            ushort size = sizeof(float);
            bool convertible = buffer.Count - offset >= size;
            if (Success && convertible)
            {
                try
                {
                    bool success = true;
                    if (mode == NetworkSerializeMode.Serialize)
                        success = BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset), value);
                    else if (mode == NetworkSerializeMode.Deserialize)
                        value = BitConverter.ToSingle(new ReadOnlySpan<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset));
                    if (!success)
                        throw new Exception("convert failed...");
                    offset += size;
                }
                catch (Exception ex)
                {
                    error = true;
                    LogError(value, size, ex.Message);
                }
            }
            else
            {
                error = true;
                LogError(value, size, "out of range");
            }
        }
        public void SerializeValue(ref double value)
        {
            if (!Success) return;

            ushort size = sizeof(double);
            bool convertible = buffer.Count - offset >= size;
            if (Success && convertible)
            {
                try
                {
                    bool success = true;
                    if (mode == NetworkSerializeMode.Serialize)
                        success = BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset), value);
                    else if (mode == NetworkSerializeMode.Deserialize)
                        value = BitConverter.ToDouble(new ReadOnlySpan<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset));
                    if (!success)
                        throw new Exception("convert failed...");
                    offset += size;
                }
                catch (Exception ex)
                {
                    error = true;
                    LogError(value, size, ex.Message);
                }
            }
            else
            {
                error = true;
                LogError(value, size, "out of range");
            }
        }
        public void SerializeValue(ref string value)
        {
            if (!Success) return;

            ushort size = 0;
            try
            {
                size = (ushort)(mode == NetworkSerializeMode.Serialize ? Encoding.Unicode.GetByteCount(value) : 0);
                SerializeValue(ref size);

                bool convertible = buffer.Count - offset >= size;
                if (convertible)
                {
                    if (mode == NetworkSerializeMode.Serialize)
                    {
                        int written = Encoding.Unicode.GetBytes(value, new Span<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset));
                        if (written == size)
                            offset += size;
                        else
                        {
                            error = true;
                            LogError(value, size, "convert failed...");
                        }
                    }
                    else if (mode == NetworkSerializeMode.Deserialize)
                    {
                        value = Encoding.Unicode.GetString(buffer.Array, buffer.Offset + offset, size);
                        offset += size;
                    }
                }
            }
            catch (Exception ex)
            {
                error = true;
                LogError(value, size, ex.Message);
            }
        }

        public void SerializeArray<T>(ref T[] array) where T : INetworkSerializable, new()
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
        public void SerializeList<T>(ref List<T> list) where T : INetworkSerializable, new()
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
        public void SerializeArray(ref byte[] array)
        {
            ushort count = 0;
            if (mode == NetworkSerializeMode.Serialize)
                count = (ushort)array.Length;

            SerializeValue(ref count);
            if (mode == NetworkSerializeMode.Deserialize)
                array = new byte[count];
            for (int i = 0; i < count; i++)
                SerializeValue(ref array[i]);
        }
        public void SerializeList(ref List<byte> list)
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
                list = new List<byte>(count);
                byte value = default;
                for (int i = 0; i < count; i++)
                {
                    SerializeValue(ref value);
                    list.Add(value);
                }
            }
        }
        public void SerializeArray(ref bool[] array)
        {
            ushort count = 0;
            if (mode == NetworkSerializeMode.Serialize)
                count = (ushort)array.Length;

            SerializeValue(ref count);
            if (mode == NetworkSerializeMode.Deserialize)
                array = new bool[count];
            for (int i = 0; i < count; i++)
                SerializeValue(ref array[i]);
        }
        public void SerializeList(ref List<bool> list)
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
                list = new List<bool>(count);
                bool value = default;
                for (int i = 0; i < count; i++)
                {
                    SerializeValue(ref value);
                    list.Add(value);
                }
            }
        }
        public void SerializeArray(ref char[] array)
        {
            ushort count = 0;

            if (mode == NetworkSerializeMode.Serialize)
                count = (ushort)array.Length;

            SerializeValue(ref count);

            if (mode == NetworkSerializeMode.Deserialize)
                array = new char[count];
            for (int i = 0; i < count; i++)
                SerializeValue(ref array[i]);
        }
        public void SerializeList(ref List<char> list)
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
                list = new List<char>(count);
                char value = default;
                for (int i = 0; i < count; i++)
                {
                    SerializeValue(ref value);
                    list.Add(value);
                }
            }
        }
        public void SerializeArray(ref short[] array)
        {
            ushort count = 0;
            if (mode == NetworkSerializeMode.Serialize)
                count = (ushort)array.Length;

            SerializeValue(ref count);
            if (mode == NetworkSerializeMode.Deserialize)
                array = new short[count];
            for (int i = 0; i < count; i++)
                SerializeValue(ref array[i]);
        }
        public void SerializeList(ref List<short> list)
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
                list = new List<short>(count);
                short value = default;
                for (int i = 0; i < count; i++)
                {
                    SerializeValue(ref value);
                    list.Add(value);
                }
            }
        }
        public void SerializeArray(ref ushort[] array)
        {
            ushort count = 0;
            if (mode == NetworkSerializeMode.Serialize)
                count = (ushort)array.Length;

            SerializeValue(ref count);
            if (mode == NetworkSerializeMode.Deserialize)
                array = new ushort[count];
            for (int i = 0; i < count; i++)
                SerializeValue(ref array[i]);
        }
        public void SerializeList(ref List<ushort> list)
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
                list = new List<ushort>(count);
                ushort value = default;
                for (int i = 0; i < count; i++)
                {
                    SerializeValue(ref value);
                    list.Add(value);
                }
            }
        }
        public void SerializeArray(ref int[] array)
        {
            ushort count = 0;
            if (mode == NetworkSerializeMode.Serialize)
                count = (ushort)array.Length;

            SerializeValue(ref count);
            if (mode == NetworkSerializeMode.Deserialize)
                array = new int[count];
            for (int i = 0; i < count; i++)
                SerializeValue(ref array[i]);
        }
        public void SerializeList(ref List<int> list)
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
                list = new List<int>(count);
                int value = default;
                for (int i = 0; i < count; i++)
                {
                    SerializeValue(ref value);
                    list.Add(value);
                }
            }
        }
        public void SerializeArray(ref uint[] array)
        {
            ushort count = 0;
            if (mode == NetworkSerializeMode.Serialize)
                count = (ushort)array.Length;

            SerializeValue(ref count);
            if (mode == NetworkSerializeMode.Deserialize)
                array = new uint[count];
            for (int i = 0; i < count; i++)
                SerializeValue(ref array[i]);
        }
        public void SerializeList(ref List<uint> list)
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
                list = new List<uint>(count);
                uint value = default;
                for (int i = 0; i < count; i++)
                {
                    SerializeValue(ref value);
                    list.Add(value);
                }
            }
        }
        public void SerializeArray(ref long[] array)
        {
            ushort count = 0;
            if (mode == NetworkSerializeMode.Serialize)
                count = (ushort)array.Length;

            SerializeValue(ref count);
            if (mode == NetworkSerializeMode.Deserialize)
                array = new long[count];
            for (int i = 0; i < count; i++)
                SerializeValue(ref array[i]);
        }
        public void SerializeList(ref List<long> list)
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
                list = new List<long>(count);
                long value = default;
                for (int i = 0; i < count; i++)
                {
                    SerializeValue(ref value);
                    list.Add(value);
                }
            }
        }
        public void SerializeArray(ref ulong[] array)
        {
            ushort count = 0;
            if (mode == NetworkSerializeMode.Serialize)
                count = (ushort)array.Length;

            SerializeValue(ref count);
            if (mode == NetworkSerializeMode.Deserialize)
                array = new ulong[count];
            for (int i = 0; i < count; i++)
                SerializeValue(ref array[i]);
        }
        public void SerializeList(ref List<ulong> list)
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
                list = new List<ulong>(count);
                ulong value = default;
                for (int i = 0; i < count; i++)
                {
                    SerializeValue(ref value);
                    list.Add(value);
                }
            }
        }
        public void SerializeArray(ref float[] array)
        {
            ushort count = 0;
            if (mode == NetworkSerializeMode.Serialize)
                count = (ushort)array.Length;

            SerializeValue(ref count);
            if (mode == NetworkSerializeMode.Deserialize)
                array = new float[count];
            for (int i = 0; i < count; i++)
                SerializeValue(ref array[i]);
        }
        public void SerializeList(ref List<float> list)
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
                list = new List<float>(count);
                float value = default;
                for (int i = 0; i < count; i++)
                {
                    SerializeValue(ref value);
                    list.Add(value);
                }
            }
        }
        public void SerializeArray(ref double[] array)
        {
            ushort count = 0;
            if (mode == NetworkSerializeMode.Serialize)
                count = (ushort)array.Length;

            SerializeValue(ref count);
            if (mode == NetworkSerializeMode.Deserialize)
                array = new double[count];
            for (int i = 0; i < count; i++)
                SerializeValue(ref array[i]);
        }
        public void SerializeList(ref List<double> list)
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
                list = new List<double>(count);
                double value = default;
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
                string value = default;
                for (int i = 0; i < count; i++)
                {
                    SerializeValue(ref value);
                    list.Add(value);
                }
            }
        }

        public void WriteSize(ArraySegment<byte> buffer)
        {
            bool success = BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset, sizeof(ushort)), offset);
            if (!success)
                error = true;
        }
        public void WriteID(ushort id, ArraySegment<byte> buffer)
        {
            bool success = BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + sizeof(ushort), sizeof(ushort)), id);
            if (!success)
                error = true;
        }
    }
}
