using System.Text;

namespace ServerCore
{
    public enum SerializeMode
    {
        Serialize,
        Deserialize,
    }

    public interface ISerializable
    {
        public abstract void Serialize(Serializer serializer); // return write count
    }

    public class Serializer
    {
        private SerializeMode mode = SerializeMode.Serialize;
        private ArraySegment<byte> buffer;
        private ushort offset;
        private bool error;
        public bool Success => !error;

        public void Open(SerializeMode mode, ArraySegment<byte> buffer, ushort offset = 4)
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

        public void SerializeValue<T>(ref T value) where T : ISerializable
        {
            value.Serialize(this);
        }
        public void SerializeValue(ref byte value)
        {
            if (mode == SerializeMode.Serialize && Success)
            {
                bool convertible = buffer.Count - offset > sizeof(byte);
                if (convertible)
                {
                    buffer.Array[buffer.Offset + offset] = value;
                    ++offset;
                }
                else
                {
                    error = true;
                    Console.WriteLine
                        ($"[Serializer] Convert Failed... Mode : {mode}, Type : {value.GetType().Name}, Value : {value}, Space : {buffer.Count - offset}");
                }
            }
            else if (mode == SerializeMode.Deserialize && Success)
            {
                bool convertible = buffer.Count - offset > sizeof(byte);
                if (convertible)
                {
                    try
                    {
                        value = buffer.Array[buffer.Offset + offset];
                        ++offset;
                    }
                    catch (Exception ex)
                    {
                        error = true;
                        Console.WriteLine($"[Serializer] {ex}");
                    }
                }
            }
        }
        public void SerializeValue(ref bool value)
        {
            if (mode == SerializeMode.Serialize && Success)
            {
                bool success = BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset), value);
                if (success)
                    offset += sizeof(bool);
                else
                {
                    error = true;
                    Console.WriteLine
                        ($"[Serializer] Convert Failed... Mode : {mode}, Type : {value.GetType().Name}, Value : {value}, Space : {buffer.Count - offset}");
                }
            }
            else if (mode == SerializeMode.Deserialize && Success)
            {
                bool convertible = buffer.Count - offset > sizeof(bool);
                if (convertible)
                {
                    try
                    {
                        value = BitConverter.ToBoolean(new ReadOnlySpan<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset));
                        offset += sizeof(bool);
                    }
                    catch (Exception ex)
                    {
                        error = true;
                        Console.WriteLine($"[Serializer] {ex}");
                    }
                }
            }
        }
        public void SerializeValue(ref char value)
        {
            if (mode == SerializeMode.Serialize && Success)
            {
                bool success = BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset), value);
                if (success)
                    offset += sizeof(char);
                else
                {
                    error = true;
                    Console.WriteLine
                        ($"[Serializer] Convert Failed... Mode : {mode}, Type : {value.GetType().Name}, Value : {value}, Space : {buffer.Count - offset}");
                }
            }
            else if (mode == SerializeMode.Deserialize && Success)
            {
                try
                {
                    value = BitConverter.ToChar(new ReadOnlySpan<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset));
                    offset += sizeof(char);
                }
                catch (Exception ex)
                {
                    error = true;
                    Console.WriteLine($"[Serializer] {ex}");
                }
            }
        }
        public void SerializeValue(ref short value)
        {
            if (mode == SerializeMode.Serialize && Success)
            {
                bool success = BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset), value);
                if (success)
                    offset += sizeof(short);
                else
                {
                    error = true;
                    Console.WriteLine
                        ($"[Serializer] Convert Failed... Mode : {mode}, Type : {value.GetType().Name}, Value : {value}, Space : {buffer.Count - offset}");
                }
            }
            else if (mode == SerializeMode.Deserialize && Success)
            {
                try
                {
                    value = BitConverter.ToInt16(new ReadOnlySpan<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset));
                    offset += sizeof(short);
                }
                catch (Exception ex)
                {
                    error = true;
                    Console.WriteLine($"[Serializer] {ex}");
                }
            }
        }
        public void SerializeValue(ref ushort value)
        {
            if (mode == SerializeMode.Serialize && Success)
            {
                bool success = BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset), value);
                if (success)
                    offset += sizeof(ushort);
                else
                {
                    error = true;
                    Console.WriteLine
                        ($"[Serializer] Convert Failed... Mode : {mode}, Type : {value.GetType().Name}, Value : {value}, Space : {buffer.Count - offset}");
                }
            }
            else if (mode == SerializeMode.Deserialize && Success)
            {
                try
                {
                    value = BitConverter.ToUInt16(new ReadOnlySpan<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset));
                    offset += sizeof(ushort);
                }
                catch (Exception ex)
                {
                    error = true;
                    Console.WriteLine($"[Serializer] {ex}");
                }
            }
        }
        public void SerializeValue(ref int value)
        {
            if (mode == SerializeMode.Serialize && Success)
            {
                bool success = BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset), value);
                if (success)
                    offset += sizeof(int);
                else
                {
                    error = true;
                    Console.WriteLine
                        ($"[Serializer] Convert Failed... Mode : {mode}, Type : {value.GetType().Name}, Value : {value}, Space : {buffer.Count - offset}");
                }
            }
            else if (mode == SerializeMode.Deserialize && Success)
            {
                try
                {
                    value = BitConverter.ToInt32(new ReadOnlySpan<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset));
                    offset += sizeof(int);
                }
                catch (Exception ex)
                {
                    error = true;
                    Console.WriteLine($"[Serializer] {ex}");
                }
            }
        }
        public void SerializeValue(ref uint value)
        {
            if (mode == SerializeMode.Serialize && Success)
            {
                bool success = BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset), value);
                if (success)
                    offset += sizeof(uint);
                else
                {
                    error = true;
                    Console.WriteLine
                        ($"[Serializer] Convert Failed... Mode : {mode}, Type : {value.GetType().Name}, Value : {value}, Space : {buffer.Count - offset}");
                }
            }
            else if (mode == SerializeMode.Deserialize && Success)
            {
                try
                {
                    value = BitConverter.ToUInt32(new ReadOnlySpan<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset));
                    offset += sizeof(uint);
                }
                catch (Exception ex)
                {
                    error = true;
                    Console.WriteLine($"[Serializer] {ex}");
                }
            }
        }
        public void SerializeValue(ref long value)
        {
            if (mode == SerializeMode.Serialize && Success)
            {
                bool success = BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset), value);
                if (success)
                    offset += sizeof(long);
                else
                {
                    error = true;
                    Console.WriteLine
                        ($"[Serializer] Convert Failed... Mode : {mode}, Type : {value.GetType().Name}, Value : {value}, Space : {buffer.Count - offset}");
                }
            }
            else if (mode == SerializeMode.Deserialize && Success)
            {
                try
                {
                    value = BitConverter.ToInt64(new ReadOnlySpan<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset));
                    offset += sizeof(long);
                }
                catch (Exception ex)
                {
                    error = true;
                    Console.WriteLine($"[Serializer] {ex}");
                }
            }
        }
        public void SerializeValue(ref ulong value)
        {
            if (mode == SerializeMode.Serialize && Success)
            {
                bool success = BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset), value);
                if (success)
                    offset += sizeof(ulong);
                else
                {
                    error = true;
                    Console.WriteLine
                        ($"[Serializer] Convert Failed... Mode : {mode}, Type : {value.GetType().Name}, Value : {value}, Space : {buffer.Count - offset}");
                }
            }
            else if (mode == SerializeMode.Deserialize && Success)
            {
                try
                {
                    value = BitConverter.ToUInt64(new ReadOnlySpan<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset));
                    offset += sizeof(ulong);
                }
                catch (Exception ex)
                {
                    error = true;
                    Console.WriteLine($"[Serializer] {ex}");
                }
            }
        }
        public void SerializeValue(ref float value)
        {
            if (mode == SerializeMode.Serialize && Success)
            {
                bool success = BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset), value);
                if (success)
                    offset += sizeof(float);
                else
                {
                    error = true;
                    Console.WriteLine
                        ($"[Serializer] Convert Failed... Mode : {mode}, Type : {value.GetType().Name}, Value : {value}, Space : {buffer.Count - offset}");
                }
            }
            else if (mode == SerializeMode.Deserialize && Success)
            {
                try
                {
                    value = BitConverter.ToSingle(new ReadOnlySpan<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset));
                    offset += sizeof(float);
                }
                catch (Exception ex)
                {
                    error = true;
                    Console.WriteLine($"[Serializer] {ex}");
                }
            }
        }
        public void SerializeValue(ref double value)
        {
            if (mode == SerializeMode.Serialize && Success)
            {
                bool success = BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset), value);
                if (success)
                    offset += sizeof(double);
                else
                {
                    error = true;
                    Console.WriteLine
                        ($"[Serializer] Convert Failed... Mode : {mode}, Type : {value.GetType().Name}, Value : {value}, Space : {buffer.Count - offset}");
                }
            }
            else if (mode == SerializeMode.Deserialize && Success)
            {
                try
                {
                    value = BitConverter.ToDouble(new ReadOnlySpan<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset));
                    offset += sizeof(double);
                }
                catch (Exception ex)
                {
                    error = true;
                    Console.WriteLine($"[Serializer] {ex}");
                }
            }
        }
        public void SerializeValue(ref string value)
        {
            if (!Success)
                return;
            try
            {
                if (mode == SerializeMode.Serialize)
                {
                    ushort size = (ushort)Encoding.Unicode.GetByteCount(value);
                    SerializeValue(ref size);
                    bool success = Encoding.Unicode.TryGetBytes
                        (
                        value,
                        new Span<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset),
                        out int written
                        );
                    if (success)
                        offset += (ushort)written;
                    else
                    {
                        error = true;
                        Console.WriteLine
                            ($"[Serializer] Convert Failed... Mode : {mode}, Type : {value.GetType().Name}, Value : {value}, Space : {buffer.Count - offset}");
                    }
                }
                else if (mode == SerializeMode.Deserialize)
                {
                    ushort size = 0;
                    SerializeValue(ref size);
                    if (buffer.Count - offset >= size)
                    {
                        value = Encoding.Unicode.GetString(buffer.Array, buffer.Offset + offset, size);
                        offset += size;
                    }
                    else
                    {
                        error = true;
                        Console.WriteLine
                            ($"[Serializer] Convert Failed... Mode : {mode}, Type : {value.GetType().Name}, Value : {value}, Space : {buffer.Count - offset}");
                    }
                }
                else
                    error = true;
            }
            catch (Exception ex)
            {
                error = true;
                Console.WriteLine($"[Serializer] {ex}");
            }
        }

        public void SerializeArray<T>(T[] array) where T : ISerializable
        {
            ushort count = (ushort)array.Length;

            SerializeValue(ref count);
            for (int i = 0; i < count; i++)
                SerializeValue(ref array[i]);
        }
        public void SerializeList<T>(List<T> list) where T : ISerializable
        {
            ushort count = (ushort)list.Count;

            SerializeValue(ref count);
            for (int i = 0; i < count; i++)
            {
                T value = list[i];
                SerializeValue(ref value);
            }
        }
        public void SerializeArray(byte[] array)
        {
            ushort count = (ushort)array.Length;

            SerializeValue(ref count);
            for (int i = 0; i < count; i++)
                SerializeValue(ref array[i]);
        }
        public void SerializeList(List<byte> list)
        {
            ushort count = (ushort)list.Count;

            SerializeValue(ref count);
            for (int i = 0; i < count; i++)
            {
                var value = list[i];
                SerializeValue(ref value);
            }
        }
        public void SerializeArray(bool[] array)
        {
            ushort count = (ushort)array.Length;

            SerializeValue(ref count);
            for (int i = 0; i < count; i++)
                SerializeValue(ref array[i]);
        }
        public void SerializeList(List<bool> list)
        {
            ushort count = (ushort)list.Count;

            SerializeValue(ref count);
            for (int i = 0; i < count; i++)
            {
                var value = list[i];
                SerializeValue(ref value);
            }
        }
        public void SerializeArray(char[] array)
        {
            ushort count = (ushort)array.Length;

            SerializeValue(ref count);
            for (int i = 0; i < count; i++)
                SerializeValue(ref array[i]);
        }
        public void SerializeList(List<char> list)
        {
            ushort count = (ushort)list.Count;

            SerializeValue(ref count);
            for (int i = 0; i < count; i++)
            {
                var value = list[i];
                SerializeValue(ref value);
            }
        }
        public void SerializeArray(short[] array)
        {
            ushort count = (ushort)array.Length;

            SerializeValue(ref count);
            for (int i = 0; i < count; i++)
                SerializeValue(ref array[i]);
        }
        public void SerializeList(List<short> list)
        {
            ushort count = (ushort)list.Count;

            SerializeValue(ref count);
            for (int i = 0; i < count; i++)
            {
                var value = list[i];
                SerializeValue(ref value);
            }
        }
        public void SerializeArray(ushort[] array)
        {
            ushort count = (ushort)array.Length;

            SerializeValue(ref count);
            for (int i = 0; i < count; i++)
                SerializeValue(ref array[i]);
        }
        public void SerializeList(List<ushort> list)
        {
            ushort count = (ushort)list.Count;

            SerializeValue(ref count);
            for (int i = 0; i < count; i++)
            {
                var value = list[i];
                SerializeValue(ref value);
            }
        }
        public void SerializeArray(int[] array)
        {
            ushort count = (ushort)array.Length;

            SerializeValue(ref count);
            for (int i = 0; i < count; i++)
                SerializeValue(ref array[i]);
        }
        public void SerializeList(List<int> list)
        {
            ushort count = (ushort)list.Count;

            SerializeValue(ref count);
            for (int i = 0; i < count; i++)
            {
                var value = list[i];
                SerializeValue(ref value);
            }
        }
        public void SerializeArray(uint[] array)
        {
            ushort count = (ushort)array.Length;

            SerializeValue(ref count);
            for (int i = 0; i < count; i++)
                SerializeValue(ref array[i]);
        }
        public void SerializeList(List<uint> list)
        {
            ushort count = (ushort)list.Count;

            SerializeValue(ref count);
            for (int i = 0; i < count; i++)
            {
                var value = list[i];
                SerializeValue(ref value);
            }
        }
        public void SerializeArray(long[] array)
        {
            ushort count = (ushort)array.Length;

            SerializeValue(ref count);
            for (int i = 0; i < count; i++)
                SerializeValue(ref array[i]);
        }
        public void SerializeList(List<long> list)
        {
            ushort count = (ushort)list.Count;

            SerializeValue(ref count);
            for (int i = 0; i < count; i++)
            {
                var value = list[i];
                SerializeValue(ref value);
            }
        }
        public void SerializeArray(ulong[] array)
        {
            ushort count = (ushort)array.Length;

            SerializeValue(ref count);
            for (int i = 0; i < count; i++)
                SerializeValue(ref array[i]);
        }
        public void SerializeList(List<ulong> list)
        {
            ushort count = (ushort)list.Count;

            SerializeValue(ref count);
            for (int i = 0; i < count; i++)
            {
                var value = list[i];
                SerializeValue(ref value);
            }
        }
        public void SerializeArray(float[] array)
        {
            ushort count = (ushort)array.Length;

            SerializeValue(ref count);
            for (int i = 0; i < count; i++)
                SerializeValue(ref array[i]);
        }
        public void SerializeList(List<float> list)
        {
            ushort count = (ushort)list.Count;

            SerializeValue(ref count);
            for (int i = 0; i < count; i++)
            {
                var value = list[i];
                SerializeValue(ref value);
            }
        }
        public void SerializeArray(double[] array)
        {
            ushort count = (ushort)array.Length;

            SerializeValue(ref count);
            for (int i = 0; i < count; i++)
                SerializeValue(ref array[i]);
        }
        public void SerializeList(List<double> list)
        {
            ushort count = (ushort)list.Count;

            SerializeValue(ref count);
            for (int i = 0; i < count; i++)
            {
                var value = list[i];
                SerializeValue(ref value);
            }
        }
        public void SerializeArray(string[] array)
        {
            ushort count = (ushort)array.Length;

            SerializeValue(ref count);
            for (int i = 0; i < count; i++)
                SerializeValue(ref array[i]);
        }
        public void SerializeList(List<string> list)
        {
            ushort count = (ushort)list.Count;

            SerializeValue(ref count);
            for (int i = 0; i < count; i++)
            {
                var value = list[i];
                SerializeValue(ref value);
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
