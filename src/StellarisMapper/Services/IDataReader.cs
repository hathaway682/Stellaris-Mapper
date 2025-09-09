using System.Collections.Generic;
using StellarisMapper.Models;

namespace StellarisMapper.Services
{
    /// <summary>
    /// 数据读取服务接口
    /// </summary>
    public interface IDataReader
    {
        /// <summary>
        /// 从文件读取星表数据
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>星表记录列表</returns>
        List<StarRecord> ReadStarData(string filePath);
    }
}
