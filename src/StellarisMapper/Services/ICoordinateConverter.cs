using System;
using System.Collections.Generic;
using StellarisMapper.Models;

namespace StellarisMapper.Services
{
    /// <summary>
    /// 坐标转换服务接口
    /// </summary>
    public interface ICoordinateConverter
    {
        /// <summary>
        /// 将天球坐标转换为直角坐标
        /// </summary>
        /// <param name="ra">赤经（度）</param>
        /// <param name="dec">赤纬（度）</param>
        /// <param name="parallax">视差（毫角秒）</param>
        /// <returns>直角坐标 (x, y, z)</returns>
        (double x, double y, double z) ConvertToCartesian(double ra, double dec, double parallax);
    }
}
