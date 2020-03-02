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
    public class AcmeV2Config
    {
        /// <summary>
        /// 帐户名/邮件地址
        /// </summary>
        public string AccountName { get; set; }
        /// <summary>
        /// 帐户密钥,PEM格式
        /// </summary>
        public string AccountPemKey { get; set; }

        public IList<string> Identifiers { get; set; }

        /// <summary>
        /// pfx证书文件名称
        /// </summary>
        public string CertPfxName { get; set; }
        /// <summary>
        /// 证书存放目录路径
        /// </summary>
        public string CertSavePath { get; set; }
        /// <summary>
        /// 证书存放路径，包含文件名
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public string CertFullPath { get { return Path.Combine(CertSavePath, CertPfxName); } }
        /// <summary>
        /// PEM文件名称
        /// FullChain：包含服务器证书的全部证书链文件
        /// </summary>
        public string CertFullChainPemName { get; set; }
        /// <summary>
        /// 服务器证书显示名称
        /// </summary>
        public string ServerCertDisplayName { get; set; }
        /// <summary>
        /// 服务器证书密码
        /// </summary>
        public string ServerCertPwd { get; set; }
        /// <summary>
        /// http-01验证令牌文件存放路径
        /// </summary>
        public string ChallengeTokenSavePath { get; set; }
        /// <summary>
        /// 主域名/通用名称，一般是用一级域名
        /// 如果未设置，则将选择ACME订单的第一个标识符作为通用名称。
        /// </summary>
        public string CommonName { get; set; }
        /// <summary>
        /// 所在国家/地区的两个字母的ISO标准国家代码（ISO 3166-1）
        /// https://www.iso.org/iso-3166-country-codes.html
        /// </summary>
        public string CountryName { get; set; }
        /// <summary>
        /// 省份
        /// </summary>
        public string State { get; set; }
        /// <summary>
        /// 城市
        /// </summary>
        public string Locality { get; set; }
        /// <summary>
        /// 组织
        /// </summary>
        public string Organization { get; set; }
        /// <summary>
        /// 所在组织的职位
        /// </summary>
        public string OrganizationUnit { get; set; }
    }
}
