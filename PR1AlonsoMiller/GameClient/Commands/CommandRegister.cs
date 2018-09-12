using GameComm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

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
            string nickname = Console.ReadLine();
            Console.WriteLine("Enter Player Picture Path:");
            string picture = GetPictureFromPath();
            string data = nickname + CmdReqList.NAMEPICSEPARATOR + picture;
            int length = System.Text.Encoding.UTF8.GetByteCount(data);
            string strLength = length.ToString().PadLeft(4, '0');
            return header+strLength + data;
        }

        private string GetPictureFromPath()
        {
            string strImage = "";
            while (strImage.Equals(""))
            {
                string imgPath = Console.ReadLine();
                if (File.Exists(imgPath))
                {
                    if (imgPath.EndsWith(".png") || imgPath.EndsWith(".jpg")) { 
                        byte[] file = File.ReadAllBytes(imgPath);
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
                if (strImage.Length > 9999)
                {
                    Console.WriteLine("File too large, please select another file:");
                    strImage = "";

                }

            }
            return strImage;
        }

    }
}
