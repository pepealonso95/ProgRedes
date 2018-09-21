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
            string header = CmdReqList.HEADER + CmdReqList.REGISTER;
            Console.WriteLine("Enter Player Nickname:");
            string nickname = EnterValidLengthString();
            Console.WriteLine("Enter Player Picture Path:");
            string picture = GetPictureFromPath(nickname + CmdReqList.NAMEPICSEPARATOR);
            string data = nickname + CmdReqList.NAMEPICSEPARATOR + picture;
            int length = System.Text.Encoding.UTF8.GetByteCount(data);
            string strLength = length.ToString().PadLeft(4, '0');
            return header+strLength + data;
        }

        private string GetPictureFromPath(string prevData)
        {
            string strImage = "";
            while (strImage.Equals(""))
            {
                string imgPath = Console.ReadLine();
                int imgByteLength = 0;
                if (File.Exists(imgPath))
                {
                    if (imgPath.EndsWith(".png") || imgPath.EndsWith(".jpg")) { 
                        byte[] file = File.ReadAllBytes(imgPath);
                        imgByteLength = file.Length;
                        strImage = System.Text.Encoding.Default.GetString(file);
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
                int prevDataLength = System.Text.ASCIIEncoding.Unicode.GetByteCount(prevData);
                if (prevDataLength+imgByteLength > CmdReqList.MAX_VAR_SIZE)
                {
                    Console.WriteLine("File too large, please select another file:");
                    strImage = "";

                }

            }
            return strImage;
        }

    }
}
