namespace MyFramework
{
    public class BufferManager
    {
        byte[] buffer = new byte[65536];

        //public ArraySegment<byte> TakeBuffer(int count)
        //{
        //    ArraySegment<byte> a = new ArraySegment<byte>(buffer, 3, 5);
        //    a.Array.
        //}

        public void ReturnBuffer(ArraySegment<byte> buffer)
        {

        }
    }
}

/*
    하는 일

1. 버퍼를 가진다
2. take와 return으로 버퍼를 할당, 회수한다
3. 할당된 버퍼를 리턴 전까지 추적한다
4. 최대치가 정해져있고 최대치를 넘기면 추가로 할당한다
 */