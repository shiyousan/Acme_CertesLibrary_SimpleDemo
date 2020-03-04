using Certes;
using Certes.Acme;
using Certes.Acme.Resource;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acme_CertesLibrary_SimpleDemo
{
    /// <summary>
    /// **********************************************************
    /// 作者：叶伟峰
    /// 创建时间：2020-02-10
    /// **********************************************************
    /// 相关术语表：https://letsencrypt.org/zh-cn/docs/glossary/
    /// **********************************************************
    /// Let's Encrypt 是免费、开放和自动化的证书颁发机构。
    /// **********************************************************
    /// ACME (自动证书管理环境 - Automatic Certificate Management Environment)：
    /// 由 Let’s Encrypt 实现的协议。与该协议兼容的软件可以用它与 Let’s Encrypt 通信以获取证书。    
    /// **********************************************************
    /// ACMEv2：v2为当前ACME协议使用的最新版本号，之前v1属于旧的API
    /// **********************************************************
    /// </summary>
    public class AcmeV2Helper
    {
        #region 配置文件操作

        /// <summary>
        /// 获取ACME配置文件
        /// </summary>
        /// <param name="acmeConfigFilePath">配置文件路径</param>
        /// <returns></returns>
        public static AcmeV2Config GetAcmeConfig(string acmeConfigFilePath = null)
        {
            AcmeV2Config acmeConfig = null;
            string acmeConfigJsonString;
            if (!string.IsNullOrEmpty(acmeConfigFilePath) && File.Exists(acmeConfigFilePath))
            {
                acmeConfigJsonString = File.ReadAllText(acmeConfigFilePath);
                acmeConfig = JsonConvert.DeserializeObject<AcmeV2Config>(acmeConfigJsonString);
            }
            return acmeConfig;
        }

        /// <summary>
        /// 保存配置文件
        /// </summary>
        /// <param name="acmeConfig"></param>
        /// <param name="acmeConfigName">文件路径名称</param>
        public static void SaveAcmeConfigJson(AcmeV2Config acmeConfig, string acmeConfigName)
        {
            string acmeConfigJsonString = JsonConvert.SerializeObject(acmeConfig);

            File.WriteAllText(acmeConfigName, acmeConfigJsonString);
        }
        #endregion

        #region 步骤一:创建或使用已有的ACME帐户

        /// <summary>
        /// 获取ACME上下文
        /// </summary>        
        /// <param name="accountPemKey">ACME帐户PEM格式密钥</param>
        /// <param name="apiUri">API 地址</param>
        /// <param name="isExist">是否已经存在ACME帐户</param>
        /// <returns></returns>
        public static AcmeContext GetAcmeContext(string accountPemKey, Uri apiUri, out bool isExist)
        {
            AcmeContext acme;
            if (!string.IsNullOrWhiteSpace(accountPemKey))
            {
                //使用已有的ACME帐户密钥获取ACME上下文
                var accountKey = KeyFactory.FromPem(accountPemKey);
                acme = new AcmeContext(apiUri, accountKey);
                isExist = true;
            }
            else
            {
                acme = new AcmeContext(apiUri);
                isExist = false;
            }
            return acme;
        }
        /// <summary>
        /// 获取ACME上下文
        /// </summary>
        /// <param name="accountPemKey">ACME帐户PEM格式密钥</param>
        /// <param name="isExist">是否已经存在ACME帐户</param>
        /// <returns></returns>
        public static AcmeContext GetAcmeContext(string accountPemKey, out bool isExist)
        {
            return GetAcmeContext(accountPemKey, WellKnownServers.LetsEncryptV2, out isExist);
        }

        /// <summary>
        /// 创建新的ACME帐户
        /// </summary>
        /// <param name="acmeContext">ACME上下文</param>
        /// <param name="email">邮件(用于注册ACME帐户)</param>
        /// <returns></returns>
        public static async Task<IAccountContext> GetAccountContextByNewAsync(AcmeContext acmeContext, string email)
        {
            IAccountContext account = null;
            if (!string.IsNullOrWhiteSpace(email))
            {
                account = await acmeContext.NewAccount(email, true);
            }
            return account;
        }

        /// <summary>
        /// 使用已有的ACME帐户获取ACME上下文
        /// </summary>
        /// <param name="accountPemKey">ACME帐户PEM格式密钥</param>
        /// <returns></returns>
        public static AcmeContext GetAcmeContextByPemKey(string accountPemKey)
        {
            AcmeContext acme = null;
            if (!string.IsNullOrWhiteSpace(accountPemKey))
            {
                var accountKey = KeyFactory.FromPem(accountPemKey);
                acme = new AcmeContext(WellKnownServers.LetsEncryptStagingV2, accountKey);
            }
            return acme;
        }
        #endregion

        #region 步骤二:域名认证,通过订单上下文向ACME服务器发送请求以验证域名所有权
        /// <summary>
        /// 获取订单上下文
        /// </summary>
        /// <param name="acmeContext"></param>
        /// <param name="acmeConfig"></param>
        /// <param name="orderUri"></param>
        /// <returns></returns>
        public static async Task<IOrderContext> GetOrderContextAsync(AcmeContext acmeContext, AcmeV2Config acmeConfig, Uri orderUri)
        {
            IOrderContext orderContext;
            if (orderUri != null)
            {
                orderContext = acmeContext.Order(orderUri);
            }
            else
            {
                orderContext = await acmeContext.NewOrder(acmeConfig.Identifiers.ToArray());
            }
            return orderContext;
        }

        /// <summary>
        /// 执行HTTP-01验证
        /// </summary>
        /// <param name="authzList">授权上下文集合</param>
        /// <param name="challengeTokenSavePath">http-01验证令牌文件存放路径</param>
        /// <returns></returns>
        public static async Task<bool> HttpChallengeValidateByListAsync(IList<IAuthorizationContext> authzList, string challengeTokenSavePath = "")
        {
            bool isValidate = true;

            foreach (IAuthorizationContext authz in authzList)
            {
                var status = await HttpChallengeValidateAsync(authz, challengeTokenSavePath);
                if (status == null || status != ChallengeStatus.Valid)
                {
                    isValidate = false;
                    break;
                }
            }
            return isValidate;
        }
        /// <summary>
        /// 执行HTTP-01验证
        /// </summary>
        /// <param name="authz">授权上下文</param>
        /// <param name="challengeTokenPath">http-01验证令牌文件存放路径</param>
        /// <returns></returns>
        public static async Task<ChallengeStatus?> HttpChallengeValidateAsync(IAuthorizationContext authz, string challengeTokenPath = "")
        {
            IChallengeContext httpChallenge = await authz.Http();
            var keyAuthz = httpChallenge.KeyAuthz;
            File.WriteAllText(Path.Combine(challengeTokenPath, httpChallenge.Token), keyAuthz);
            /*
             * 向ACME服务器发送申请/请求，以验证域名所有权
             * 注意，此处需要延迟轮询等等，具体可以参考GITHUB的几个issues
             * 需要延迟轮询：https://github.com/fszlin/certes/issues/194
             * 如何获取当前验证状态：https://github.com/fszlin/certes/issues/89             
             */
            Challenge challengeResult = await httpChallenge.Validate();

            //等待服务器验证结果，每隔3秒轮询一次，最多轮询10次
            var attempts = 10;
            while (attempts > 0 && (challengeResult.Status == ChallengeStatus.Pending || challengeResult.Status == ChallengeStatus.Processing))
            {
                await Task.Delay(3000);
                challengeResult = await httpChallenge.Resource();
                attempts--;
            }
            return challengeResult.Status;
        }

        #endregion

        #region 步骤三:证书颁发,证书下载到本地

        /// <summary>
        /// 导出证书文件
        /// </summary>
        /// <param name="orderCtx">订单上下文</param>
        /// <param name="acmeConfig">ACME配置文件实体</param>
        /// <returns></returns>
        public static async Task ExportCertificateAsync(IOrderContext orderCtx, AcmeV2Config acmeConfig)
        {
            var privateKey = KeyFactory.NewKey(KeyAlgorithm.ES256);
            var cert = await orderCtx.Generate(new CsrInfo
            {
                CommonName = acmeConfig.CommonName,
                CountryName = acmeConfig.CountryName,
                State = acmeConfig.State,
                Locality = acmeConfig.Locality,
                Organization = acmeConfig.Organization,
                OrganizationUnit = acmeConfig.OrganizationUnit
            }, privateKey);

            //导出整个证书链
            var certPem = cert.ToPem();
            File.WriteAllText(Path.Combine(acmeConfig.CertSavePath, acmeConfig.CertFullChainPemName), certPem);
            //导出PFX格式的证书
            var pfxBuilder = cert.ToPfx(privateKey);
            var pfx = pfxBuilder.Build(acmeConfig.ServerCertDisplayName, acmeConfig.ServerCertPwd);

            using (FileStream fs = new FileStream(Path.Combine(acmeConfig.CertSavePath, acmeConfig.CertPfxName), FileMode.Create))
            {
                fs.Write(pfx, 0, pfx.Length);
            }
        }

        #endregion
    }
}
