using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acme_CertesLibrary_SimpleDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ConsoleHelper.WriteLineByColor("Acme控制台应用程序客户端-简易用于申请/更新SSL证书工具-shiyousan.com", ConsoleColor.Green);
                ConsoleHelper.DrawHalfSplitLine();
                string acmeConfigFilePath = ConfigurationManager.AppSettings["AcemV2Config"];
                AcmeV2Config acmeConfig = AcmeV2Helper.GetAcmeConfig(acmeConfigFilePath);
                if (acmeConfig == null)
                {
                    ConsoleHelper.WriteLineByColor($"请检查ACME配置文件，当前存放路径：[{acmeConfigFilePath}]", ConsoleColor.Red);
                    return;
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteLineByColor(ex.Message, ConsoleColor.Red);
                ConsoleHelper.DrawHalfSplitLine();
                ConsoleHelper.WriteLineByColor(ex.StackTrace, ConsoleColor.Yellow);
            }
        }
    }
}
