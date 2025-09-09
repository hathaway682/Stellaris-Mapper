using System;
using System.Collections.Generic;
using StellarisMapper.Models;

namespace StellarisMapper.Services
{
    /// <summary>
    /// 坐标转换服务实现
    /// </summary>
    public class CoordinateConverter : ICoordinateConverter
    {
        /// <summary>
        /// 将天球坐标转换为直角坐标
        /// </summary>
        /// <param name="ra">赤经（度）</param>
        /// <param name="dec">赤纬（度）</param>
        /// <param name="parallax">视差（毫角秒）</param>
        /// <returns>直角坐标 (x, y, z)</returns>
        public (double x, double y, double z) ConvertToCartesian(double ra, double dec, double parallax)
        {
            // 将角度转换为弧度
            double raRad = DegreesToRadians(ra);
            double decRad = DegreesToRadians(dec);
            
            // 将视差（毫角秒）转换为距离（秒差距）
            // 距离（pc）= 1000 / 视差（mas）
            double distance = parallax > 0 ? 1000.0 / parallax : 0.0;
            
            // 转换为直角坐标系
            double x = distance * Math.Cos(decRad) * Math.Cos(raRad);
            double y = distance * Math.Cos(decRad) * Math.Sin(raRad);
            double z = distance * Math.Sin(decRad);
            
            return (x, y, z);
        }
        
        /// <summary>
        /// 将角度转换为弧度
        /// </summary>
        /// <param name="degrees">角度值</param>
        /// <returns>弧度值</returns>
        private double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }
    }
}
