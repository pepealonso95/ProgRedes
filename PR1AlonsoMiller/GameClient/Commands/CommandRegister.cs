using GameComm;
using System;
using System.Collections;
using System.IO;
using System.Text;

namespace GameClient.Commands
{
    public class CommandRegister:Command
    {
        public CommandRegister() : base(RequestCmd.REGISTER)
        {
        }
        public override byte[] Run()
        {
            Console.WriteLine("Enter Player Nickname:");
            string nickname = EnterValidLengthString();
            Console.WriteLine("Enter Player Picture Path:");
            byte[] picture = GetPictureFromPath();
            byte[] allMessages = GetSplitSendPicture(nickname, picture);
            return allMessages;
        }

        private byte[] GetSplitSendPicture(string nickname, byte[] picture)
        {
            ArrayList sendMessage = new ArrayList();
            int amountOfMessages = picture.Length / CmdReqList.MAX_VAR_SIZE;
            int finalMsgLength = picture.Length % CmdReqList.MAX_VAR_SIZE;
            string composedMessage = nickname + CmdReqList.NAMEPICSEPARATOR + (amountOfMessages+1);
            int length = Encoding.UTF8.GetByteCount(composedMessage);
            string strLength = length.ToString().PadLeft(5, '0');
            sendMessage.AddRange(Encoding.UTF8.GetBytes(CmdReqList.HEADER + CmdReqList.REGISTER + strLength + composedMessage));
            for (int i = 0; i < amountOfMessages; i++)
            {
                byte[] subarray = new byte[CmdReqList.MAX_VAR_SIZE];
                Buffer.BlockCopy(picture, i * CmdReqList.MAX_VAR_SIZE, subarray, 0, CmdReqList.MAX_VAR_SIZE);
                string msgStrLength = CmdReqList.MAX_VAR_SIZE.ToString().PadLeft(5, '0');
                sendMessage.AddRange(Encoding.UTF8.GetBytes(CmdReqList.HEADER + CmdReqList.PICTURE + msgStrLength));
                sendMessage.AddRange(subarray);
            }
            byte[] finalSubarray = new byte[finalMsgLength];
            Buffer.BlockCopy(picture, amountOfMessages * CmdReqList.MAX_VAR_SIZE, finalSubarray, 0, finalMsgLength);
            string finalMsgStrLength = finalMsgLength.ToString().PadLeft(5, '0');
            sendMessage.AddRange(Encoding.UTF8.GetBytes(CmdReqList.HEADER + CmdReqList.PICTURE + finalMsgStrLength));
            sendMessage.AddRange(finalSubarray);
            return (byte[])sendMessage.ToArray(typeof(byte));
        }

        private byte[] GetPictureFromPath()
        {
            byte[] myFile = new byte[0];
            while (myFile.Length==0)
            {
                string imgPath = Console.ReadLine();
                if (File.Exists(imgPath))
                {
                    if (imgPath.EndsWith(".png") || imgPath.EndsWith(".jpg")) {
                        myFile = File.ReadAllBytes(imgPath);
                    }
                    else
                    {
                        Console.WriteLine("File extention must be png or jpg, please re-enter:");
                    }
                }
                else
                {
                    Console.WriteLine("Inexistent or Invalid File Path, please re-enter:");
                }
                /*if (prevDataLength+imgByteLength > CmdReqList.MAX_VAR_SIZE)
                {
                    Console.WriteLine("File too large, please select another file:");
                    strImage = "";

                }*/

            }
            return myFile;
        }

    }
}
