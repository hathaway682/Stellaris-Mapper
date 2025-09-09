using System.IO;
using Newtonsoft.Json;
using StellarisMapper.Models;

namespace StellarisMapper.Services
{
    /// <summary>
    /// 文件写入服务实现
    /// </summary>
    public class FileWriter : IFileWriter
    {
        /// <summary>
        /// 将星图网络保存为JSON文件
        /// </summary>
        /// <param name="network">星图网络</param>
        /// <param name="filePath">文件路径</param>
        public void SaveToJson(StellarNetwork network, string filePath)
        {
            // 使用Newtonsoft.Json序列化对象
            string json = JsonConvert.SerializeObject(network, Formatting.Indented);
            
            // 写入文件
            File.WriteAllText(filePath, json);
        }
    }
}
