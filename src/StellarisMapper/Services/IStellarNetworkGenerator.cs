using System.Collections.Generic;
using StellarisMapper.Models;

namespace StellarisMapper.Services
{
    /// <summary>
    /// 星图网络生成器接口
    /// </summary>
    public interface IStellarNetworkGenerator
    {
        /// <summary>
        /// 生成星图网络
        /// </summary>
        /// <param name="starRecords">星表记录</param>
        /// <param name="maxJumpDistance">最大跳跃距离（秒差距）</param>
        /// <param name="includeLM">是否包含LM/LM?对象</param>
        /// <param name="includeWD">是否包含WD/WD?对象</param>
        /// <param name="includeBD">是否包含BD/BD?对象</param>
        /// <returns>星图网络</returns>
        StellarNetwork GenerateNetwork(List<StarRecord> starRecords, double maxJumpDistance, 
            bool includeLM = false, bool includeWD = false, bool includeBD = false);
        
        /// <summary>
        /// 检查网络是否连通
        /// </summary>
        /// <param name="network">星图网络</param>
        /// <returns>如果网络连通返回true，否则返回false</returns>
        bool IsNetworkConnected(StellarNetwork network);
        
        /// <summary>
        /// 计算网络中连通组件的数量
        /// </summary>
        /// <param name="network">星图网络</param>
        /// <returns>连通组件的数量</returns>
        int CountConnectedComponents(StellarNetwork network);
    }
}
