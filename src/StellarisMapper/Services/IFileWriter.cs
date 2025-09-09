using StellarisMapper.Models;

namespace StellarisMapper.Services
{
    /// <summary>
    /// 文件写入服务接口
    /// </summary>
    public interface IFileWriter
    {
        /// <summary>
        /// 将星图网络保存为JSON文件
        /// </summary>
        /// <param name="network">星图网络</param>
        /// <param name="filePath">文件路径</param>
        void SaveToJson(StellarNetwork network, string filePath);
    }
}
