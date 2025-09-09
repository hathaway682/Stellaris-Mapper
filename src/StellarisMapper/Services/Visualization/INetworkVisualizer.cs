using System.Threading.Tasks;
using StellarisMapper.Models;

namespace StellarisMapper.Services.Visualization
{
    /// <summary>
    /// 网络可视化接口
    /// </summary>
    public interface INetworkVisualizer
    {
        /// <summary>
        /// 将星图网络可视化为图片
        /// </summary>
        /// <param name="network">星图网络</param>
        /// <param name="outputPath">输出图片路径</param>
        /// <returns>表示异步操作的任务</returns>
        Task VisualizeNetworkAsync(StellarNetwork network, string outputPath);
        
        /// <summary>
        /// 将星图网络可视化为GIF动画
        /// </summary>
        /// <param name="network">星图网络</param>
        /// <param name="outputPath">输出GIF路径</param>
        /// <returns>表示异步操作的任务</returns>
        Task VisualizeNetworkAsGifAsync(StellarNetwork network, string outputPath);
    }
}
