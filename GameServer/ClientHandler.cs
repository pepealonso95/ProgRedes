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
        public bool isConnected;
        private TcpClient client;
        private NetworkStream stream;

        private Match match;
        private Player player;
        private bool isPlaying;

        private int expectedImgFiles = 0;
        private Player registering;
        private string registeringImg;

        public ClientHandler(TcpClient clientToHandle)
        {
            client = clientToHandle;
            isConnected = true;
            stream = client.GetStream();
            isPlaying = false;
            match = Match.Instance();
            expectedImgFiles = 0;
            registeringImg = "";
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
                    if (Int32.Parse(header)<CmdReqList.REQLIMIT)
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
                match.PlayerKill(player);
                PlayerList.PlayerLogout(player);
            }
        }
        
        private void HandleRequest(string buffer)
        {
            string strCmd = buffer.Substring(0, 3);
            if(expectedImgFiles!=0&& strCmd != CmdReqList.PICTURE)
            {
                ReturnError(CmdResList.EXPECTING_IMG);
            }
             if(strCmd == CmdReqList.EXIT)
            {
                throw new DisconnectedException("Close");
            }
            else if(strCmd == CmdReqList.PICTURE)
            {
                AddDataToPicture(buffer);
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
            else if (strCmd == CmdReqList.MOVECHARACTER)
            {
                MoveCharacter(buffer);
            }
            else if (strCmd == CmdReqList.ATTACKCHARACTER)
            {
                AttackCharacter();
            }
            else
            {
                ReturnError(CmdResList.UNKNOWN);
            }

        }

        private void AddDataToPicture(string buffer)
        {
            if (expectedImgFiles > 0)
            {
                int length = Int32.Parse(buffer.Substring(3, 5));
                byte[] data = new byte[length];
                RecieveStream(data);
                registeringImg += Encoding.UTF8.GetString(data);
                expectedImgFiles--;
                if (expectedImgFiles == 0)
                {
                    PlayerList.SetImage(registering, registeringImg);
                    registeringImg = "";
                    registering = null;
                    ReturnOk();
                }
            }
            else
            {
                ReturnError(CmdResList.NOT_EXPECTING_IMG);
            }
        }

        private void LoginPlayer(string buffer)
        {
            int length = Int32.Parse(buffer.Substring(3, 5));
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
        
         private void LogoutPlayer()
        {
            if (player != null)
            {
                match.PlayerKill(player);
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
            string response = CmdResList.EXIT + CmdResList.NO_VAR;
            byte[] send = Encoding.UTF8.GetBytes(response);
            stream.Write(send, 0, CmdResList.FIXED_LENGTH);
        }

        private void RegisterPlayer(string buffer)
        {
            if (this.player != null) {
                ReturnError(CmdResList.INVALID_WHILE_PLAYING);
                return;
            }
            int length = Int32.Parse(buffer.Substring(3, 5));
            byte[] data = new byte[length];
            RecieveStream(data);
            string strData = Encoding.UTF8.GetString(data);
            string[] splitData = strData.Split(CmdReqList.NAMEPICSEPARATOR);
            Player player = new Player(splitData[0]);
            if (!PlayerList.PlayerRegister(player))
            {
                ReturnError(CmdResList.REGISTER_INVALID);
            }
            registering = new Player(player.Nickname);
            expectedImgFiles = Int32.Parse(splitData[1]);
        }
        
        private void ReturnOk()
        {
            string response = CmdResList.OK+CmdResList.NO_VAR;
            byte[] send = Encoding.UTF8.GetBytes(response);
            stream.Write(send, 0, CmdResList.FIXED_LENGTH);
        }
        private void ReturnOkWithMessage(string message)
        {
            string header =  CmdResList.OK;
            int length = System.Text.Encoding.UTF8.GetByteCount(message);
            if (length > CmdReqList.MAX_VAR_SIZE)
            {
                ReturnError(CmdResList.UNKNOWN);
                return;
            }
            string strLength = length.ToString().PadLeft(5, '0');
            byte[] send = Encoding.UTF8.GetBytes(header + strLength + message);
            stream.Write(send, 0, send.Length);
        }

        private void ReturnError(string error)
        {
            string response =  error + CmdResList.NO_VAR;
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
                        throw new DisconnectedException("Lost Connection");
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
            int length = Int32.Parse(buffer.Substring(3, 5));
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


        private void AttackCharacter()
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
                string addResult = match.Attack(player);
                if (addResult == CmdResList.NOTINMATCH)
                {
                    ReturnError(CmdResList.NOTINMATCH);
                }
                else if (addResult == CmdResList.PLAYERDEAD)
                {
                    ReturnError(CmdResList.PLAYERDEAD);
                }
                else if (addResult == CmdResList.DIDNT_SELECT)
                {
                    ReturnError(CmdResList.DIDNT_SELECT);
                }
                else if (addResult == "")
                {
                    ReturnError(CmdResList.UNKNOWN);
                }
                else if (match.Finished)
                {
                    ReturnError(CmdResList.MATCHFINISHED);
                    return;
                }
                else
                {
                    ReturnOkWithMessage(addResult);
                }
            }
        }

        private void MoveCharacter(string buffer)
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
                int length = Int32.Parse(buffer.Substring(3, 5));
                byte[] data = new byte[length];
                RecieveStream(data);
                string directions = Encoding.UTF8.GetString(data);
                string addResult = match.Move(player,directions);
                if (addResult == CmdResList.NOTINMATCH)
                {
                    ReturnError(CmdResList.NOTINMATCH);
                }
                else if (addResult == CmdResList.PLAYERDEAD)
                {
                    ReturnError(CmdResList.PLAYERDEAD);
                }
                else if (addResult == "")
                {
                    ReturnError(CmdResList.UNKNOWN);
                }
                else if (addResult == CmdResList.OUT_OF_BOUNDS)
                {
                    ReturnError(CmdResList.OUT_OF_BOUNDS);
                }
                else if (addResult == CmdResList.DIDNT_SELECT)
                {
                    ReturnError(CmdResList.DIDNT_SELECT);
                }
                else if (addResult == CmdResList.OCCUPIED)
                {
                    ReturnError(CmdResList.OCCUPIED);
                }
                else if (addResult == CmdResList.MATCHFINISHED)
                {
                    ReturnError(CmdResList.MATCHFINISHED);
                    return;
                }
                else
                {
                    ReturnOkWithMessage(addResult);
                }
            }
        }
    }
}
