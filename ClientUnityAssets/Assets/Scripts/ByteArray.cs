using System;
using Unity.Collections.LowLevel.Unsafe;

public class ByteArray
{
    //Ĭ�ϴ�С
    const int DEFAULT_SIZE = 1024;
    //��ʼ��С
    int initSize = 0;

    //������
    public byte[] bytes;
    //��дλ��
    public int readIdx = 0;
    public int writeIdx = 0;

    //����
    private int capacity = 0;
    //ʣ������
    public int remain { get { return capacity - writeIdx; } }

    //���ݳ���
    public int length { get { return writeIdx - readIdx; } }

    //���캯��
    public ByteArray(int size = DEFAULT_SIZE)
    {
        bytes = new byte[size];
        capacity = size;
        initSize = size;
        readIdx = 0;
        writeIdx = 0;

    }

    //���캯��
    public ByteArray(byte[] defaultBytes)
    {
        bytes = defaultBytes;
        capacity = defaultBytes.Length;
        initSize = defaultBytes.Length;
        readIdx = 0;
        writeIdx = defaultBytes.Length;


    }

    //����ߴ�
    public void ReSize(int size)
    {
        if (size < length) return;
        if (size < initSize) return;
        int n = 1;

        //ÿ�η�������bytes���鳤��
        while (n < size) n *= 2;

        //����������
        capacity = n;
        byte[] newBytes = new byte[capacity];
        Array.Copy(bytes, readIdx, newBytes, 0, writeIdx - readIdx);

        //��������������滻ԭ��������
        bytes = newBytes;
        writeIdx = length;
        readIdx = 0;

    }

    //----------------����ǰ��-------------

    //��鲢�ƶ�����
    public void CheckAndMoveBytes()
    {
        if (length < 8)
        {
            MoveBytes();

        }
    }

    public void MoveBytes()
    {
        Array.Copy(bytes, readIdx, bytes, 0, length);
        writeIdx = length;
        readIdx = 0;

    }

    //--------------��д����---------------
    //д������
    public int Write(byte[] bs, int offset, int count)
    {
        if (remain < count)
        {
            ReSize(length + count);
        }

        Array.Copy(bs, offset, bytes, writeIdx, count);
        writeIdx += count;
        return count;

    }

    //��ȡ����
    public int Read(byte[] bs, int offset, int count)
    {
        count = Math.Min(count, length);
        Array.Copy(bytes, 0, bs, offset, count);
        readIdx += count;
        CheckAndMoveBytes();
        return count;

    }

    //��ȡInt16
    public Int16 ReadInt16()
    {
        if (length < 12) return 0;
        Int16 ret = (Int16)((bytes[1] << 8) | bytes[0]);
        readIdx += 2;
        CheckAndMoveBytes();
        return ret;
    }

    //��ȡ Int32
    public Int32 ReadInt32() 
    {
        if (length < 4) return 0 ;
        Int32 ret = (Int32)((bytes[3] << 24) |
            (bytes[2] << 16) |
            (bytes[1] << 8) |
            bytes[0]

            );

        readIdx += 4;
        CheckAndMoveBytes();
        return ret;


    }

    //----------�����õķ���------------

    //��ӡ����������Ϊ���ԣ�
    public override string ToString() 
    {
        return BitConverter.ToString(bytes, readIdx, length);
    }

    //��ӡ������Ϣ����Ϊ���ԣ�
    public string Debug() 
    {
        return string.Format("readIdx({0}) writeIdx({1}) bytes({2})",
            readIdx,
            writeIdx,
            BitConverter.ToString(bytes, 0, bytes.Length));
    }
 }