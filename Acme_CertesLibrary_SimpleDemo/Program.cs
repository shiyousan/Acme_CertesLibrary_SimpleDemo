using Certes;
using Certes.Acme;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Acme_CertesLibrary_SimpleDemo
{
    class Program
    {
        private static readonly string _acmeConfigFilePath = ConfigurationManager.AppSettings["AcemV2ConfigFilePath"];
        static async Task Main(string[] args)
        {
            try
            {
                ConsoleHelper.WriteLineByColor("Acme .NET Certes类库简单控制台应用程序演示", ConsoleColor.Green);
                ConsoleHelper.WriteLineByColor("详见：https://github.com/fszlin/certes", ConsoleColor.Green);
                ConsoleHelper.DrawHalfSplitLine();

                AcmeV2Config acmeConfig = AcmeV2Helper.GetAcmeConfig(_acmeConfigFilePath);
                ShowAcmeConfigInfo(acmeConfig);
                ShowCertInfo(acmeConfig);

                if (!Directory.Exists(acmeConfig.ChallengeTokenSavePath))
                {
                    ConsoleHelper.WriteLineByColor($"Error: 验证令牌路径不存在 ---{acmeConfig.ChallengeTokenSavePath}", ConsoleColor.Red);
                    return;
                }
                if (!Directory.Exists(acmeConfig.CertSavePath))
                {
                    ConsoleHelper.WriteLineByColor($"Error: 证书文件存放路径不存在 ---{acmeConfig.CertSavePath}", ConsoleColor.Red);
                    return;
                }

                #region 步骤一，创建或使用已有的ACME帐户
                ConsoleHelper.DrawHalfSplitLine();
                Console.WriteLine("步骤一，创建或使用已有的ACME帐户");
                /*
                 *  注意！！！
                 *  目前调用的API使用的是测试环境，
                 *  WellKnownServers.LetsEncryptStagingV2=https://acme-staging-v02.api.letsencrypt.org
                 *  生产环境应当使用：WellKnownServers.LetsEncryptV2
                 */
                //
                AcmeContext acmeCtx = AcmeV2Helper.GetAcmeContext(acmeConfig.AccountPemKey, WellKnownServers.LetsEncryptStagingV2, out bool isExist);
                if (acmeCtx == null)
                {
                    ConsoleHelper.WriteLineByColor($"Error: 无法获取正确的ACME上下文信息,检查Api地址和AccountPemKey是否正确", ConsoleColor.Red);
                    return;
                }

                IAccountContext accountCtx;
                //根据Account Pem Key判断是否需要创建新帐号，或者用已有帐户
                if (isExist)
                {
                    Console.WriteLine($"已存在Account Pem Key，当前帐户：[{acmeConfig.AccountName}]，获取帐户操作上下文...");
                    accountCtx = await acmeCtx.Account();
                }
                else
                {
                    Console.WriteLine($"未有Account Pem Key，创建新帐户[{acmeConfig.AccountName}]以获取帐户操作上下文...");
                    accountCtx = await AcmeV2Helper.GetAccountContextByNewAsync(acmeCtx, acmeConfig.AccountName);
                }

                if (accountCtx == null)
                {
                    ConsoleHelper.WriteLineByColor($"Error: 无法获取正确的ACME帐户操作上下文信息，请检查相关帐户密钥是否正确,{acmeConfig.AccountName}", ConsoleColor.Red);
                    return;
                }
                //保存ACME帐户密钥(pem key)到本地
                acmeConfig.AccountPemKey = acmeCtx.AccountKey.ToPem();
                AcmeV2Helper.SaveAcmeConfigJson(acmeConfig, _acmeConfigFilePath);
                Console.WriteLine("步骤一完成");
                #endregion

                #region 步骤二，通过订单上下文向ACME服务器发送请求以验证域名所有权
                //步骤二，域名认证，通过订单上下文向ACME服务器发送请求以验证域名所有权
                //此处使用HTTP-01验证方式轮询所有域名，适合非通配符证书。Ps：如果需要使用通配符域名则需要使用DNS-01验证方式
                ConsoleHelper.DrawHalfSplitLine();
                Console.WriteLine("步骤二，通过订单上下文向ACME服务器发送请求以验证域名所有权");
                IOrderContext orderCtx = await acmeCtx.NewOrder(acmeConfig.Identifiers.ToArray());
                IList<IAuthorizationContext> authzList = (await orderCtx.Authorizations()).ToList();

                bool isValidate = await AcmeV2Helper.HttpChallengeValidateByListAsync(authzList, acmeConfig.ChallengeTokenSavePath);
                if (!isValidate)
                {
                    ConsoleHelper.WriteLineByColor($"Error: HTTP-01验证未通过！！！", ConsoleColor.Red);
                    return;
                }
                Console.WriteLine("步骤二完成");
                #endregion

                #region 步骤三，验证通过后下载证书
                //步骤三，验证通过后下载证书
                ConsoleHelper.DrawHalfSplitLine();
                Console.WriteLine("步骤三，验证通过后下载证书");
                AcmeV2Helper.SaveAcmeConfigJson(acmeConfig, _acmeConfigFilePath);
                await AcmeV2Helper.ExportCertificateAsync(orderCtx, acmeConfig);
                Console.WriteLine("步骤三完成");
                #endregion
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteLineByColor(ex.Message, ConsoleColor.Red);
                ConsoleHelper.DrawHalfSplitLine();
                ConsoleHelper.WriteLineByColor(ex.StackTrace, ConsoleColor.Yellow);
            }
            Console.ReadKey();
        }
        /// <summary>
        /// 显示Acme配置文件信息
        /// </summary>
        /// <param name="acmeConfig"></param>
        private static void ShowAcmeConfigInfo(AcmeV2Config acmeConfig)
        {
            if (acmeConfig == null)
            {
                ConsoleHelper.WriteLineByColor($"Error：无法读取ACME配置文件！", ConsoleColor.Red);
                return;
            }
            Console.WriteLine("ACME配置文件：");
            Console.WriteLine(JsonConvert.SerializeObject(acmeConfig));
            ConsoleHelper.DrawHalfSplitLine();
        }

        /// <summary>
        /// 显示当前已存在的证书信息
        /// </summary>
        /// <param name="acmeConfig"></param>
        private static void ShowCertInfo(AcmeV2Config acmeConfig)
        {
            if (acmeConfig == null)
            {
                ConsoleHelper.WriteLineByColor($"Error: 无法读取ACME配置文件!", ConsoleColor.Red);
                return;
            }
            if (!File.Exists(acmeConfig.CertFullPath))
            {
                ConsoleHelper.WriteLineByColor($"Warning: 当前路径无法找到SSL证书文件，可能证书尚未下载，无法读取证书信息：{acmeConfig.CertFullPath}!", ConsoleColor.Yellow);
                return;
            }
            //判断距离证书到期还剩多少天
            X509Certificate2 existCertInfo = new X509Certificate2(acmeConfig.CertFullPath, acmeConfig.ServerCertPwd);
            TimeSpan dateDiffResult = existCertInfo.NotAfter - DateTime.Now;

            //显示证书相关信息
            Console.WriteLine($"证书别名：{existCertInfo.FriendlyName}，证书主体：{existCertInfo.Subject}，证书到期日期：{existCertInfo.NotAfter.ToString()}（倒计时{dateDiffResult.Days}天）");
            ConsoleHelper.DrawHalfSplitLine();
        }
    }
}
