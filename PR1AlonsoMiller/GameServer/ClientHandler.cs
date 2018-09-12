using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Linq;
using Domain;
using GameComm;

namespace GameServer
{
    public class ClientHandler
    {
        private Player player;
        public bool isConnected;
        private TcpClient client;
        public ClientHandler(TcpClient clientToHandle)
        {
            client = clientToHandle;
            isConnected = true;
        }

        public void Start()
        {
            while (isConnected)
            {
                byte[] buffer = new byte[ResponseCmd.FIXED_LENGTH];
                RecieveStream(buffer);
                string strBuffer = Encoding.UTF8.GetString(buffer);
                string header = strBuffer.Substring(0, 3);
                if (header.Equals("REQ"))
                {
                    HandleRequest(strBuffer);
                }
            }
        }

       

        private void HandleRequest(string buffer)
        {
            string strCmd = buffer.Substring(3, 2);
            if(strCmd == CmdReqList.EXIT)
            {
                ReturnExit();
                client.GetStream().Close();
                client.Close();
                isConnected = false;
                ServerMain.count--;
                return;
            }
            else if (strCmd == CmdReqList.REGISTER)
            {
                RegisterPlayer(buffer);
            }else if(strCmd == CmdReqList.LOGIN)
            {
                int length = Int32.Parse(buffer.Substring(5, 4));
                byte[] data = new byte[length];
                RecieveStream(data);
                string strData = Encoding.UTF8.GetString(data);
                Player player = PlayerList.GetInstance().Find(p=> p.Nickname == strData);
                if (player != null)
                {
                    this.player = player;
                    ReturnOk();
                }
                else
                {
                    ReturnError(ResponseCmd.LOGIN_INVALID);
                }
            }

        }

        private void ReturnExit()
        {
            string response = ResponseCmd.HEADER + ResponseCmd.EXIT + ResponseCmd.NO_VAR;
            byte[] send = Encoding.UTF8.GetBytes(response);
            client.GetStream().Write(send, 0, ResponseCmd.FIXED_LENGTH);
        }

        private void RegisterPlayer(string buffer)
        {
            int length = Int32.Parse(buffer.Substring(5, 4));
            byte[] data = new byte[length];
            client.GetStream().Read(data, 0, length);
            string strData = Encoding.UTF8.GetString(data);
            string[] splitData = strData.Split(CmdReqList.NAMEPICSEPARATOR);
            Player player = new Player(splitData[0], Encoding.UTF8.GetBytes(splitData[1]));
            PlayerList.GetInstance().Add(player);
            ReturnOk();
        }

        private void ReturnOk()
        {
            string response = ResponseCmd.HEADER+ResponseCmd.OK+ResponseCmd.NO_VAR;
            byte[] send = Encoding.UTF8.GetBytes(response);
            client.GetStream().Write(send, 0, ResponseCmd.FIXED_LENGTH);
        }
        
        private void ReturnError(string error)
        {
            string response = ResponseCmd.HEADER + error + ResponseCmd.NO_VAR;
            byte[] send = Encoding.UTF8.GetBytes(response);
            client.GetStream().Write(send, 0, ResponseCmd.FIXED_LENGTH);
        }

        private void RecieveStream(byte[] buffer)
        {
            var recieved = 0;
            while (recieved < buffer.Length)
            {
                var pos = client.GetStream().Read(buffer, 0, buffer.Length);
                if (pos == 0)
                {
                    //hay que hacer close?
                    throw new SocketException();
                }
                recieved += pos;
            }
        }
    }
}
