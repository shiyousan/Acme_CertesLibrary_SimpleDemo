using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acme_CertesLibrary_SimpleDemo
{
    public class AcmeV2Helper
    {
        #region 配置文件操作

        /// <summary>
        /// 获取ACME配置文件
        /// </summary>
        /// <param name="acmeConfigName">配置文件名称</param>
        /// <returns></returns>
        public static AcmeV2Config GetAcmeConfig(string acmeConfigName = null)
        {
            AcmeV2Config acmeConfig = null;
            string acmeConfigJsonString;
            if (!string.IsNullOrEmpty(acmeConfigName) && File.Exists(acmeConfigName))
            {
                acmeConfigJsonString = File.ReadAllText(acmeConfigName);
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
    }
}
