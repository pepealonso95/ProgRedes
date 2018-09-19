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
        private Match match;
        private Player player;
        public bool isConnected;
        private TcpClient client;
        private NetworkStream stream;
        private bool isPlaying;
        public ClientHandler(TcpClient clientToHandle)
        {
            client = clientToHandle;
            isConnected = true;
            stream = client.GetStream();
            isPlaying = false;
            match = Match.Instance();
        }

        public void Start()
        {
            try
            {
                while (isConnected)
                {
                    byte[] buffer = new byte[CmdResList.FIXED_LENGTH];
                    RecieveStream(buffer);
                    string strBuffer = Encoding.UTF8.GetString(buffer);
                    string header = strBuffer.Substring(0, 3);
                    if (header.Equals("REQ"))
                    {
                        HandleRequest(strBuffer);
                    }
                }
            }catch(DisconnectedException e)
            {
                stream.Close();
                client.Close();
                isConnected = false;
                ServerMain.count--;
            }
            if (player != null)
            {
                PlayerList.PlayerLogout(player);
            }
        }
        
        private void HandleRequest(string buffer)
        {
            string strCmd = buffer.Substring(3, 2);
            if(strCmd == CmdReqList.EXIT)
            {
                throw new DisconnectedException("Close");
            }
            else if (strCmd == CmdReqList.REGISTER)
            {
                RegisterPlayer(buffer);
            }
            else if(strCmd == CmdReqList.LOGIN)
            {
                LoginPlayer(buffer);
            }
            else if (strCmd == CmdReqList.LOGOUT)
            {
                LogoutPlayer();
            }
            else if (strCmd == CmdReqList.JOINMATCH)
            {
                JoinMatch();
            }
            else if (strCmd == CmdReqList.SELECTCHARACTER)
            {
                AddCharacter(buffer);
            }

        }

        private void LoginPlayer(string buffer)
        {
            int length = Int32.Parse(buffer.Substring(5, 4));
            byte[] data = new byte[length];
            RecieveStream(data);
            string strData = Encoding.UTF8.GetString(data);
            if (this.player != null)
            {
                ReturnError(CmdResList.ALREADY_LOGGED);
                return;
            }
            Player logplayer = PlayerList.PlayerLogin(strData);
            if (logplayer != null)
            {
                this.player = logplayer;
                ReturnOk();
            }
            else
            {
                ReturnError(CmdResList.LOGIN_INVALID);
            }
        }

        //Si esta jugando el jugador que se desloguea, matar a su jugador
         private void LogoutPlayer()
        {
            if (player != null)
            {
                PlayerList.PlayerLogout(this.player);
                this.player = null;
                ReturnOk();
            }
            else
            {
                ReturnError(CmdResList.NOTLOGGED);
            }
        }

        private void ReturnExit()
        {
            string response = CmdResList.HEADER + CmdResList.EXIT + CmdResList.NO_VAR;
            byte[] send = Encoding.UTF8.GetBytes(response);
            stream.Write(send, 0, CmdResList.FIXED_LENGTH);
        }

        private void RegisterPlayer(string buffer)
        {
            int length = Int32.Parse(buffer.Substring(5, 4));
            byte[] data = new byte[length];
            RecieveStream(data);
            string strData = Encoding.UTF8.GetString(data);
            string[] splitData = strData.Split(CmdReqList.NAMEPICSEPARATOR);
            Player player = new Player(splitData[0], Encoding.UTF8.GetBytes(splitData[1]));
            if (PlayerList.PlayerRegister(player))
            {
                ReturnOk();
            }
            else
            {
                ReturnError(CmdResList.REGISTER_INVALID);
            }
        }
        
        private void ReturnOk()
        {
            string response = CmdResList.HEADER+CmdResList.OK+CmdResList.NO_VAR;
            byte[] send = Encoding.UTF8.GetBytes(response);
            stream.Write(send, 0, CmdResList.FIXED_LENGTH);
        }
        private void ReturnOkWithMessage(string message)
        {
            string header = CmdResList.HEADER + CmdResList.OK;
            int length = System.Text.Encoding.UTF8.GetByteCount(message);
            if (length > CmdReqList.MAX_VAR_SIZE)
            {
                ReturnError(CmdResList.UNKNOWN);
                return;
            }
            string strLength = length.ToString().PadLeft(4, '0');
            byte[] send = Encoding.UTF8.GetBytes(header + strLength + message);
            stream.Write(send, 0, send.Length);
        }

        private void ReturnError(string error)
        {
            string response = CmdResList.HEADER + error + CmdResList.NO_VAR;
            byte[] send = Encoding.UTF8.GetBytes(response);
            stream.Write(send, 0, CmdResList.FIXED_LENGTH);
        }

        private void RecieveStream(byte[] buffer)
        {
            var recieved = 0;
            while (recieved < buffer.Length && isConnected)
            {
                try
                {
                    var pos = stream.Read(buffer, 0, buffer.Length);
                    if (pos == 0)
                    {
                        client.Close();
                        isConnected = false;
                    }
                    recieved += pos;
                }
                catch (System.IO.IOException se)
                {
                    throw new DisconnectedException("Lost Connection");

                }
            }
        }

        private void JoinMatch()
        {
            if (player == null)
            {
                ReturnError(CmdResList.NOTLOGGED);
                return;
            }
            else if(match.Finished)
            {
                ReturnError(CmdResList.MATCHFINISHED);
                return;
            }
            else
            {
                string addResult = match.AddPlayer(player);
                if (addResult == CmdResList.MATCHFULL)
                {
                    ReturnError(CmdResList.MATCHFULL);
                }
                else if(addResult == CmdResList.INMATCH)
                {
                    ReturnError(CmdResList.INMATCH);
                }
                else if (addResult == "")
                {
                    ReturnError(CmdResList.UNKNOWN);
                }
                else
                {
                    ReturnOkWithMessage(addResult);
                }
            }

        }

        private void AddCharacter(string buffer)
        {
            if (player == null)
            {
                ReturnError(CmdResList.NOTLOGGED);
                return;
            }
            else if (match.Finished)
            {
                ReturnError(CmdResList.MATCHFINISHED);
                return;
            }
            else
            {
                Character toAdd = GetCharacter(buffer);
                string addResult = match.AddCharacter(toAdd);
                if(addResult == CmdResList.NOTINMATCH)
                {
                    ReturnError(CmdResList.NOTINMATCH);
                }
                else if (addResult == CmdResList.PLAYERDEAD)
                {
                    ReturnError(CmdResList.PLAYERDEAD);
                }
                else if (addResult == CmdResList.INMATCH)
                {
                    ReturnError(CmdResList.INMATCH);
                }
                else if(addResult == "")
                {
                    ReturnError(CmdResList.UNKNOWN);
                }
                else
                {
                    ReturnOkWithMessage(addResult);
                }
            }

        }

        private Character GetCharacter(string buffer)
        {
            int length = Int32.Parse(buffer.Substring(5, 4));
            byte[] data = new byte[length];
            RecieveStream(data);
            string strData = Encoding.UTF8.GetString(data);
            if(strData.Equals(TextCommands.MONSTER, StringComparison.InvariantCultureIgnoreCase) )
            {
                return new Monster(player);
            }
            else if (strData.Equals(TextCommands.SURVIVOR, StringComparison.InvariantCultureIgnoreCase))
            {
                return new Survivor(player);
            }
            else
            {
                return null;
            }
        }
    }
}
