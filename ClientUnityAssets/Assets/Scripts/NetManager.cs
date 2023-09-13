using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;
using System.Linq;
using System.Diagnostics.Tracing;


public class NetManager 
{
    //����socket
    static Socket socket;
    //���ջ�����
    static ByteArray readBuff;
    //д�����
    static Queue<ByteArray> writeQueue;

    //�Ƿ���������
    static bool isConnecting = false;

    //�Ƿ����ڹر�
    static bool isClosing = false;

    //��Ϣ�б�
    static List<MsgBase> msgList = new List<MsgBase>();
    //��Ϣ�б���
    static int msgCount = 0;
    //ÿһ��Update�������Ϣ����
    readonly static int MAX_MESSAGE_FIRE = 10;


    //�Ƿ���������
    public static bool isUsePing = true;
    //�������ʱ��
    public static int pingInterval = 30;
    //��һ�η���PING��ʱ��
    static float lastPingTime = 0;
    //�ϴ��յ�PONG��ʱ��
    static float lastPongTime = 0;



    //----------------------------------�����ṹ------------------------------------------

    //------------------------------1. �����¼�����----------------------------------------

    //�¼�ί������
    public delegate void EventListener(String err);
    //�¼������б�
    private static Dictionary<NetEvent, EventListener> eventListeners = new Dictionary<NetEvent, EventListener>();


    //����¼�����
    public static void AddEventListener(NetEvent netEvent, EventListener listener)
    {
        //����ֵ����Ѵ����¼���������¼�
        if (eventListeners.ContainsKey(netEvent))
        {
            eventListeners[netEvent] += listener;
        }
        //��������ڣ��������¼�
        else 
        {
            eventListeners[netEvent] = listener;
        }
    }

    //ɾ���¼�����
    public void RemoveEventListener(NetEvent netEvent, EventListener listener) 
    {
        if (eventListeners.ContainsKey(netEvent)) 
        {
            eventListeners[netEvent] -= listener;

            //ɾ��
            if (eventListeners[netEvent] == null) 
            {
                eventListeners.Remove(netEvent);
                
            }          
        }
    }

    //�ַ��¼�
    private static void FireEvent(NetEvent netEvent, String err) 
    {
        if (eventListeners.ContainsKey(netEvent)) 
        {
            eventListeners[netEvent](err);
        }
    }

    //-------------------------------2. ������Ϣ����---------------------------------------------

    //��Ϣί������
    public delegate void MsgListener(MsgBase msgBase);

    //��Ϣ�����б�
    private static Dictionary<string, MsgListener> msgListeners = new Dictionary<string, MsgListener>();

    //�����Ϣ����
    public static void AddMsgListener(string msgName, MsgListener listener) 
    {
        //���
        if (msgListeners.ContainsKey(msgName))
        {
            msgListeners[msgName] += listener;
        }
        //����
        else 
        {
            msgListeners[msgName] = listener;
        }
    }

    //ɾ����Ϣ����
    public static void RemoveMsgListener(string msgName, MsgListener listener) 
    {
        if (msgListeners.ContainsKey(msgName))
        {
            msgListeners[msgName] -= listener;

            //ɾ��
            if (msgListeners[msgName] == null) 
            {
                msgListeners.Remove(msgName);
            }
        }


    }



    //-------------------------------------------------------------------------------------------

    //----------------------------�ַ���Ϣ-------------------------------
    private static void FireMsg(string msgName, MsgBase msgBase) 
    {
        if (msgListeners.ContainsKey(msgName)) 
        {
            msgListeners[msgName](msgBase);
        }
    }
    

    //-------------------------------��ʼ��״̬-----------------------------------
    private static void InitState() 
    {
        //Socket
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        //���ջ�����
        readBuff = new ByteArray();

        //д�����
        writeQueue = new Queue<ByteArray>();

        //�Ƿ���������
        isConnecting = false;

        //�Ƿ����ڹر�
        isClosing = false;

        //��Ϣ�б�
        msgList = new List<MsgBase>();
        //��Ϣ�б���
        msgCount = 0;


        //��һ�η���PING��ʱ��
        lastPingTime = Time.time;
        //��һ���յ�PONG��ʱ��
        lastPongTime = Time.time;
        //��ͨPONGЭ�飨���ظ���ӣ�
        if (!msgListeners.ContainsKey("MsgPong")) 
        {
            AddMsgListener("MsgPong", OnMsgPong);
        }


    }

    //����PONG��������Ϣ
    private static void OnMsgPong(MsgBase msgBase) 
    {
        lastPongTime = Time.time;
    }

    //--------------------------------------����-----------------------------------
    public static void Connect(string ip, int port) 
    {
        //״̬�ж�
        //1.�Ѿ�����ʱ�������Ӽ�
        if (socket != null && socket.Connected) 
        {
            Debug.Log("Connect fail, already connected!");
            return;
        }
        //2. ��������ʱ�������Ӽ�
        if (isConnecting) 
        {
            Debug.Log("Connect fail, isConnecting");
            return;
        }

        //3. δ�����ǰ������Ӽ�
        //��ʼ����Ա
        InitState();
        //��������
        socket.NoDelay = true;
        //����
        isConnecting = true;
        socket.BeginConnect(ip, port, ConnectCallback, socket);
    }

    //Connect�ص�
    private static void ConnectCallback(IAsyncResult ar) 
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            socket.EndConnect(ar);
            Debug.Log("Socket Connect Succ");
            FireEvent(NetEvent.ConnectSucc, "");
            isConnecting = false;

            //��ʼ����
            socket.BeginReceive(readBuff.bytes, readBuff.writeIdx, readBuff.remain, 0, ReceiveCallback, socket);
        }
        catch (SocketException ex) 
        {
            Debug.Log("Socket Connect fail" + ex.ToString());
            FireEvent(NetEvent.ConnectFail, ex.ToString());
            isConnecting = false;
        }
    }

    //Receive�ص�
    public static void ReceiveCallback(IAsyncResult ar) 
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;

            //��ȡ�������ݳ���
            int count = socket.EndReceive(ar);
            if (count == 0)
            {
                Close();
                return;
            }
            readBuff.writeIdx += count;

            //������Ϣ������Э�飬����Э�������õ���Ϣ�б���
            OnReceiveData();

            //������������
            if (readBuff.remain < 8)
            {
                readBuff.MoveBytes();
                readBuff.ReSize(readBuff.length * 2);
            }
            socket.BeginReceive(readBuff.bytes, readBuff.writeIdx, readBuff.remain, 0, ReceiveCallback, socket);

        }
        catch (SocketException ex) 
        {
            Debug.Log("Socket Receive Fail" + ex.ToString());
            
        }
    }

    //���ݴ���
    public static void OnReceiveData() 
    {
        //��Ϣ����
        if (readBuff.length <= 2) 
        {
            return;
        }

        //��ȡ��Ϣ�峤��
        int readIdx = readBuff.readIdx;
        byte[] bytes = readBuff.bytes;
        Int16 bodyLength = (Int16)((bytes[readIdx + 1] << 8) | bytes[readIdx]);
        if (readBuff.length < bodyLength) 
        {
            return;
        }
        readBuff.readIdx += 2;

        //����Э����
        int nameCount = 0;
        string protoName = MsgBase.DecodeName(readBuff.bytes, readBuff.readIdx, out nameCount);

        if (protoName == "") 
        {
            Debug.Log("OnReceiveData MsgBase.DecodeName fail");
            return;
        }
        readBuff.readIdx += nameCount;

        //����Э����
        int bodyCount = bodyLength - nameCount;
        MsgBase msgBase = MsgBase.Decode(protoName, readBuff.bytes, readBuff.readIdx, bodyCount);
        readBuff.readIdx += bodyCount;
        readBuff.CheckAndMoveBytes();

        //��ӵ���Ϣ����
        lock (msgList) 
        {
            msgList.Add(msgBase);
        }

        msgCount++;

        //������ȡ��Ϣ
        if (readBuff.length > 2) 
        {
            OnReceiveData();
        }

    }

    //---------------------------�ر�����-----------------------------
    public static void Close() 
    {
        //״̬�ж�1:ֻ�д���socket �� �������Ӻ���ܹر�
        if (socket == null || !socket.Connected) 
        {
            return;
            
        }

        //״̬�ж�2����������ʱ���ܹر�
        if (isConnecting) 
        {
            return;
        }

        //״̬�ж�3�����������ڷ���ʱ��,�ȴ����������ٹر�
        if (writeQueue.Count > 0)
        {
            isClosing = true;
        }
        else//û�����ݷ��� 
        {
            socket.Close();
            FireEvent(NetEvent.Close, "");
        }
    }

    //------------------------------��������--------------------------------
    public static void Send(MsgBase msg) 
    {
        //״̬�ж�
        if (socket == null || !socket.Connected) 
        {
            return;
        }

        if (isConnecting) 
        {
            return;
        }

        if (isClosing) 
        {
            return;
        }

        //���ݱ���
        byte[] nameBytes = MsgBase.EncodeName(msg);
        byte[] bodyBytes = MsgBase.Encode(msg);

        int len = nameBytes.Length + bodyBytes.Length;
        byte[] sendBytes = new byte[ 2 + len ];

        //��װ����
        sendBytes[0] = (byte)(len % 256);
        sendBytes[1] = (byte)(len / 256);

        //��װ����
        Array.Copy(nameBytes, 0, sendBytes, 2, nameBytes.Length);

        //��װ��Ϣ��
        Array.Copy(bodyBytes, 0, sendBytes, 2 + nameBytes.Length, bodyBytes.Length);

        //д�����
        ByteArray ba = new ByteArray(sendBytes);
        int count = 0;  //writeQueue�ĳ���
        lock (writeQueue) 
        {
            writeQueue.Enqueue(ba);
            count = writeQueue.Count;
        }

        //send
        if (count == 1) 
        {
            socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, SendCallback, socket);
        }

    }

    //Send�ص�
    public static void SendCallback(IAsyncResult ar) 
    {
        //��ȡstate, EndSend �Ĵ���
        Socket socket = (Socket)ar.AsyncState;

        //״̬�ж�
        if (socket == null || !socket.Connected) 
        {
            return;
        }

        //��������
        int count = socket.EndSend(ar);

        //��ȡд����е�һ������
        ByteArray ba;
        lock (writeQueue) 
        {
            ba = writeQueue.First();
        }

        //��������
        ba.readIdx += count;
        if (ba.length == 0) 
        {
            lock (writeQueue) 
            {
                writeQueue.Dequeue();
                ba = writeQueue.First();

            }
        }

        //��������
        if (ba != null)
        {
            socket.BeginSend(ba.bytes, ba.readIdx, ba.length, 0, SendCallback, socket);

        }
        //���ڹر�
        else if (isClosing) 
        {
            socket.Close();
        }

    }

    //-------------------Update-------------------------------

    public static void Update() 
    {
        MsgUpdate();
        PingUpdate();
    }

    //������Ϣ
    public static void MsgUpdate() 
    {
        //�����жϣ�����Ч��
        if (msgCount == 0) 
        {
            return;
        }

        //�ظ�������Ϣ
        for (int i = 0; i < MAX_MESSAGE_FIRE; i++)
        {
            //��ȡ��һ����Ϣ
            MsgBase msgBase = null;
            lock (msgList) 
            {
                if (msgList.Count > 0) 
                {
                    msgBase = msgList[0];
                    msgList.RemoveAt(0);
                    msgCount--;
                }
            }

            //�ַ���Ϣ
            if (msgBase != null)
            {
                FireMsg(msgBase.protoName, msgBase);
            }
            //û����Ϣ��
            else 
            {
                break;
            }

        }
    }

    //���·���pingЭ��
    private static void PingUpdate() 
    {
        //�ж��Ƿ�����
        if (!isUsePing) 
        {
            return;
        }

        //�жϵ�ǰʱ������һ�η���MsgPingЭ���ʱ���������Ʒ���ping
        if (Time.time - lastPingTime > pingInterval) 
        {
            MsgPing msgPing = new MsgPing();
            Send(msgPing);
            lastPingTime = Time.time;
        }

        //�жϵ�ǰʱ������һ�ν���MsgPongЭ���ʱ���������ƹر�����
        if (Time.time - lastPongTime > pingInterval * 4) 
        {
            Close();
        }
    }




    //---------------------------ENUM--------------------------
    //�¼�
    public enum NetEvent
    {
        ConnectSucc = 1,
        ConnectFail = 2,
        Close = 3,

    }
    //---------------------------------------------------------



}


