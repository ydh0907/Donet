namespace ServerCore
{
    public enum SerializeMode
    {
        Read,
        Write
    }

    public abstract class ISerializable
    {
        public abstract int Serialize(ArraySegment<byte> buffer, int offset); // return write count
        public abstract int Deserialize(ArraySegment<byte> buffer, int offset); // return read count
    }

    public class Serializer
    {
        private SerializeMode mode = SerializeMode.Read;
        private ArraySegment<byte> buffer;
        private int offset;
        private bool error;
        public bool Success => !error;

        internal void Open(ArraySegment<byte> buffer, SerializeMode mode)
        {
            this.mode = mode;
            this.buffer = buffer;
            offset = 4;
            error = false;
        }

        internal int Close()
        {
            return offset;
        }

        public void SerializeValue<T>(ref T value) where T : ISerializable
        {
            int size = value.Serialize(buffer, offset);
            offset += size;
        }
        public void SerializeValue(ref byte value)
        {
            if (offset < buffer.Count)
            {
                buffer.Array[buffer.Offset + offset] = value;
                ++offset;
            }
            else error = true;
        }
        public void SerializeValue(ref bool value)
        {
            if (mode == SerializeMode.Read && Success)
            {
                bool success = BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset), value);
                if (success)
                    offset += sizeof(bool);
                else error = true;
            }
            else if (mode == SerializeMode.Write && Success)
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
                    }
                }
            }
        }
        public void SerializeValue(ref char value)
        {
            if (mode == SerializeMode.Read && Success)
            {
                bool success = BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset), value);
                if (success)
                    offset += sizeof(char);
                else error = true;
            }
            else if (mode == SerializeMode.Write && Success)
            {
                try
                {
                    value = BitConverter.ToChar(new ReadOnlySpan<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset));
                    offset += sizeof(char);
                }
                catch (Exception ex)
                {
                    error = true;
                }
            }
        }
        public void SerializeValue(ref short value)
        {
            if (mode == SerializeMode.Read && Success)
            {
                bool success = BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset), value);
                if (success)
                    offset += sizeof(short);
                else error = true;
            }
            else if (mode == SerializeMode.Write && Success)
            {
                try
                {
                    value = BitConverter.ToInt16(new ReadOnlySpan<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset));
                    offset += sizeof(short);
                }
                catch (Exception ex)
                {
                    error = true;
                }
            }
        }
        public void SerializeValue(ref ushort value)
        {
            if (mode == SerializeMode.Read && Success)
            {
                bool success = BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset), value);
                if (success)
                    offset += sizeof(ushort);
                else error = true;
            }
            else if (mode == SerializeMode.Write && Success)
            {
                try
                {
                    value = BitConverter.ToUInt16(new ReadOnlySpan<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset));
                    offset += sizeof(ushort);
                }
                catch (Exception ex)
                {
                    error = true;
                }
            }
        }
        public void SerializeValue(ref int value)
        {
            if (mode == SerializeMode.Read && Success)
            {
                bool success = BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset), value);
                if (success)
                    offset += sizeof(int);
                else error = true;
            }
            else if (mode == SerializeMode.Write && Success)
            {
                try
                {
                    value = BitConverter.ToInt32(new ReadOnlySpan<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset));
                    offset += sizeof(int);
                }
                catch (Exception ex)
                {
                    error = true;
                }
            }
        }
        public void SerializeValue(ref uint value)
        {
            if (mode == SerializeMode.Read && Success)
            {
                bool success = BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset), value);
                if (success)
                    offset += sizeof(uint);
                else error = true;
            }
            else if (mode == SerializeMode.Write && Success)
            {
                try
                {
                    value = BitConverter.ToUInt32(new ReadOnlySpan<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset));
                    offset += sizeof(uint);
                }
                catch (Exception ex)
                {
                    error = true;
                }
            }
        }
        public void SerializeValue(ref long value)
        {
            if (mode == SerializeMode.Read && Success)
            {
                bool success = BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset), value);
                if (success)
                    offset += sizeof(long);
                else error = true;
            }
            else if (mode == SerializeMode.Write && Success)
            {
                try
                {
                    value = BitConverter.ToInt64(new ReadOnlySpan<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset));
                    offset += sizeof(long);
                }
                catch (Exception ex)
                {
                    error = true;
                }
            }
        }
        public void SerializeValue(ref ulong value)
        {
            if (mode == SerializeMode.Read && Success)
            {
                bool success = BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset), value);
                if (success)
                    offset += sizeof(ulong);
                else error = true;
            }
            else if (mode == SerializeMode.Write && Success)
            {
                try
                {
                    value = BitConverter.ToUInt64(new ReadOnlySpan<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset));
                    offset += sizeof(ulong);
                }
                catch (Exception ex)
                {
                    error = true;
                }
            }
        }
        public void SerializeValue(ref float value)
        {
            if (mode == SerializeMode.Read && Success)
            {
                bool success = BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset), value);
                if (success)
                    offset += sizeof(float);
                else error = true;
            }
            else if (mode == SerializeMode.Write && Success)
            {
                try
                {
                    value = BitConverter.ToSingle(new ReadOnlySpan<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset));
                    offset += sizeof(float);
                }
                catch (Exception ex)
                {
                    error = true;
                }
            }
        }
        public void SerializeValue(ref double value)
        {
            if (mode == SerializeMode.Read && Success)
            {
                bool success = BitConverter.TryWriteBytes(new Span<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset), value);
                if (success)
                    offset += sizeof(double);
                else error = true;
            }
            else if (mode == SerializeMode.Write && Success)
            {
                try
                {
                    value = BitConverter.ToDouble(new ReadOnlySpan<byte>(buffer.Array, buffer.Offset + offset, buffer.Count - offset));
                    offset += sizeof(double);
                }
                catch (Exception ex)
                {
                    error = true;
                }
            }
        }
        public void SerializeValue(ref string value)
        {
            ushort count = (ushort)value.Length;
            char[] copy;

            if (mode == SerializeMode.Read)
                copy = value.ToCharArray();
            else if (mode == SerializeMode.Write)
                copy = new char[count];
            else return;

            SerializeValue(ref count);
            for (int i = 0; i < count; i++)
                SerializeValue(ref copy[i]);

            value = new(copy);
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

        }

        public void WriteID(ushort id, ArraySegment<byte> buffer)
        {

        }
    }
}
