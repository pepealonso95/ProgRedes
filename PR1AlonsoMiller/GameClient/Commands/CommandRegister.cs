using GameComm;
using System;
using System.IO;

namespace GameClient.Commands
{
    public class CommandRegister:Command
    {
        public CommandRegister() : base(RequestCmd.REGISTER)
        {
        }
        public override string Run()
        {
            Console.WriteLine("Enter Player Nickname:");
            string nickname = EnterValidLengthString();
            Console.WriteLine("Enter Player Picture Path:");
            byte[] picture = GetPictureFromPath();
            string allMessages = GetSplitSendPicture(nickname, picture);
            return allMessages;
        }

        private string GetSplitSendPicture(string nickname, byte[] picture)
        {
            string sendMessage = "";
            int amountOfMessages = picture.Length / CmdReqList.MAX_VAR_SIZE;
            int finalMsgLength = picture.Length % CmdReqList.MAX_VAR_SIZE;
            string composedMessage = nickname + CmdReqList.NAMEPICSEPARATOR + (amountOfMessages+1);
            int length = System.Text.Encoding.UTF8.GetByteCount(composedMessage);
            string strLength = length.ToString().PadLeft(4, '0');
            sendMessage = CmdReqList.HEADER + CmdReqList.REGISTER + strLength+composedMessage;
            for (int i = 0; i < amountOfMessages; i++)
            {
                byte[] subarray = new byte[CmdReqList.MAX_VAR_SIZE];
                Buffer.BlockCopy(picture, i * CmdReqList.MAX_VAR_SIZE, subarray, 0, CmdReqList.MAX_VAR_SIZE);
                string picturePart = System.Text.Encoding.UTF8.GetString(subarray);
                sendMessage += CmdReqList.HEADER + CmdReqList.PICTURE + CmdReqList.MAX_VAR_SIZE + picturePart;
            }
            byte[] finalSubarray = new byte[finalMsgLength];
            Buffer.BlockCopy(picture, amountOfMessages * CmdReqList.MAX_VAR_SIZE, finalSubarray, 0, finalMsgLength);
            string finalPicturePart = System.Text.Encoding.UTF8.GetString(finalSubarray);
            string finalMsgStrLength = finalMsgLength.ToString().PadLeft(4, '0');
            sendMessage += CmdReqList.HEADER + CmdReqList.PICTURE + finalMsgStrLength + finalPicturePart;
            return sendMessage;
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
