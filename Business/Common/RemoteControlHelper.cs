#region [ KIMMA Co.,Ltd. Copyright (C) 2014 ]
//-------------------------------------------------------------------------------------
// 开发单位：湖南金码智能设备制造有限公司
// 业务模块：iVend终端软件业务处理类
// 业务功能：远程指令接收类
// 创建标识：2016-07-10		谷霖
//-------------------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Business.Common
{
    public class RemoteControlHelper
    {
        private bool m_CloseListen = false;
        private int m_ListenPort = 6010;

        /// <summary>
        /// 远程控制指令数据队列
        /// </summary>
        private Queue<string> RemoteDataQueue = new Queue<string>();

        //private List<string> RemoteDataaList = new List<string>[];

        public int Init(int port)
        {
            int intErrCode = 0;
            m_ListenPort = port;

            Thread TrdListen = new Thread(new ThreadStart(ServerListen));
            TrdListen.IsBackground = true;
            TrdListen.Start();

            return intErrCode;
        }

        public void Displose()
        {
            m_CloseListen = true;
        }

        /// <summary>
        /// 获取远程控制指令队列数量
        /// </summary>
        /// <returns>远程控制指令队列数量</returns>
        public int GetRemoteCount()
        {
            return RemoteDataQueue.Count;
        }

        /// <summary>
        /// 获取远程控制指令队列
        /// </summary>
        /// <returns>远程控制指令队列的值</returns>
        public string ReadRemoteData()
        {
            string strData = "";

            if (RemoteDataQueue.Count > 0)
            {
                lock (RemoteDataQueue)
                {
                    strData = RemoteDataQueue.Dequeue();
                }
            }

            return strData;
        }

        /// <summary>
        /// 监听网络数据的工作线程
        /// </summary>
        private void ServerListen()
        {
            try
            {
                Socket Listener = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);
                Listener.Bind(new IPEndPoint(IPAddress.Any, m_ListenPort));
                Listener.Listen(10);

                while (!m_CloseListen)
                {
                    Socket socket = Listener.Accept();
                    string receivedValue = string.Empty;
                    StringBuilder m_Sb = new StringBuilder();

                    while (true)
                    {
                        byte[] receivedBytes = new byte[500];
                        int numBytes = socket.Receive(receivedBytes);

                        receivedValue += Encoding.UTF8.GetString(receivedBytes,
                            0, numBytes);

                        if (receivedValue.Length > 0)
                        {
                            if (receivedBytes[0] == 0x40)
                            {
                                byte[] tempByts = new byte[numBytes - 8];
                                for (int i = 4; i < numBytes - 4; i++)
                                {
                                    tempByts[i - 4] = receivedBytes[i];
                                }

                                string recData = Encoding.UTF8.GetString(tempByts,
                                     0, tempByts.Length);
                                // 加入队列中
                                lock (RemoteDataQueue)
                                {
                                    RemoteDataQueue.Enqueue(recData);
                                }
                            }
                            break;
                        }
                    }

                    string replyValue = "";
                    replyValue = "0";

                    byte[] replyMessage = PackSendNetData();

                    socket.Send(replyMessage);

                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
                Listener.Close();
            }
            catch
            {
                Thread.Sleep(2000);

                Thread TrdListen = new Thread(new ThreadStart(ServerListen));
                TrdListen.IsBackground = true;
                TrdListen.Start();
            }
        }

        private static string ByteArrayToHexString(byte[] data, int length)
        {
            try
            {
                StringBuilder sb = new StringBuilder(length * 3);
                for (int i = 0; i < length; i++)
                {
                    sb.Append(Convert.ToString(data[i], 16).PadLeft(2, '0').PadRight(3, ' '));
                }
                return sb.ToString().ToUpper();
            }
            catch
            {
                return "";
            }
        }

        private static byte[] PackSendNetData()
        {
            byte[] SendData = null;

            int intNetInfoLength = 0;

            try
            {
                // 包体数据
                string strBodyData = "0";
                intNetInfoLength = strBodyData.Length;// bodyData.Length;// StringLength(netInfo);

                SendData = new byte[8 + intNetInfoLength];

                // 通道类型
                SendData[0] = 0x40;

                // 通讯流水号
                SendData[1] = Convert.ToByte("1");

                // 数据包长度
                SendData[2] = Convert.ToByte((8 + intNetInfoLength).ToString());

                // 数据包子类型
                SendData[3] = 0x10;

                byte[] bytsBody = Encoding.UTF8.GetBytes(strBodyData);

                for (int i = 0; i < bytsBody.Length; i++)
                {
                    SendData[4 + i] = bytsBody[i];
                }

                // 结束符
                SendData[4 + intNetInfoLength] = 0x0A;

                // 检验和
                byte bytCheck = SendData[0];
                for (int i = 1; i < 5 + intNetInfoLength; i++)
                {
                    bytCheck = (byte)(bytCheck + SendData[i]);
                }

                SendData[5 + intNetInfoLength] = (byte)((byte)(bytCheck & 0xF0) + 0x0F);
                SendData[6 + intNetInfoLength] = (byte)((byte)(bytCheck << 4) + 0x0F);

                SendData[7 + intNetInfoLength] = 0x00;
            }
            catch (Exception ex)
            {
            }
            return SendData;
        }
    }
}
