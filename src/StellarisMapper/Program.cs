using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using StellarisMapper.Models;
using StellarisMapper.Services;
using StellarisMapper.Services.Visualization;

namespace StellarisMapper
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Stellaris Mapper - 星图生成工具");
            Console.WriteLine("================================");
            
            // 检查命令行参数
            if (args.Length < 2)
            {
                Console.WriteLine("使用方法: StellarisMapper <输入文件路径> <输出文件路径> [最大跳跃距离(秒差距, 默认10.0)] [选项]");
                Console.WriteLine("选项:");
                Console.WriteLine("  --include-lm    包含OBJ_CAT为LM/LM?的对象（低质量天体）");
                Console.WriteLine("  --include-wd    包含OBJ_CAT为WD/WD?的对象（白矮星）");
                Console.WriteLine("  --include-bd    包含OBJ_CAT为BD/BD?的对象（褐矮星）");
                Console.WriteLine("示例: StellarisMapper ./data/The10pcSample_v2.xlsx ./output/stellar_network.json 15.0 --include-lm --include-wd");
                return;
            }
            
            string inputFilePath = args[0];
            string outputFilePath = args[1];
            double maxJumpDistance = 10.0; // 默认最大跳跃距离为10秒差距
            
            // 解析命令行选项
            bool includeLM = false;
            bool includeWD = false;
            bool includeBD = false;
            
            // 如果提供了第三个参数，解析最大跳跃距离和选项
            if (args.Length >= 3)
            {
                // 检查第三个参数是否为数字（最大跳跃距离）
                if (double.TryParse(args[2], out double parsedJumpDistance))
                {
                    maxJumpDistance = parsedJumpDistance;
                    
                    // 检查后续参数中的选项
                    for (int i = 3; i < args.Length; i++)
                    {
                        switch (args[i].ToLower())
                        {
                            case "--include-lm":
                                includeLM = true;
                                break;
                            case "--include-wd":
                                includeWD = true;
                                break;
                            case "--include-bd":
                                includeBD = true;
                                break;
                        }
                    }
                }
                else
                {
                    // 第三个参数不是数字，可能是选项
                    for (int i = 2; i < args.Length; i++)
                    {
                        switch (args[i].ToLower())
                        {
                            case "--include-lm":
                                includeLM = true;
                                break;
                            case "--include-wd":
                                includeWD = true;
                                break;
                            case "--include-bd":
                                includeBD = true;
                                break;
                        }
                    }
                }
            }
            
            // 显示包含选项的信息
            Console.WriteLine($"包含选项: LM/LM?={includeLM}, WD/WD?={includeWD}, BD/BD?={includeBD}");
            
            // 检查输入文件是否存在
            if (!File.Exists(inputFilePath))
            {
                Console.WriteLine($"错误: 输入文件 '{inputFilePath}' 不存在");
                return;
            }
            
            try
            {
                // 创建服务实例
                IDataReader dataReader = new ExcelDataReaderService();
                ICoordinateConverter coordinateConverter = new CoordinateConverter();
                IStellarNetworkGenerator networkGenerator = new StellarNetworkGenerator(coordinateConverter);
                IFileWriter fileWriter = new FileWriter();
                
                Console.WriteLine($"正在读取星表数据: {inputFilePath}");
                var starRecords = dataReader.ReadStarData(inputFilePath);
                Console.WriteLine($"成功读取 {starRecords.Count} 条记录");
                
                Console.WriteLine($"正在生成星图网络，最大跳跃距离: {maxJumpDistance} 秒差距");
                var network = networkGenerator.GenerateNetwork(starRecords, maxJumpDistance, includeLM, includeWD, includeBD);
                Console.WriteLine($"网络生成完成，包含 {network.Systems.Count} 个恒星系统和 {network.Connections.Count} 个连接");
                
                // 检查网络连通性
                Console.WriteLine("正在检查网络连通性...");
                bool isConnected = networkGenerator.IsNetworkConnected(network);
                int componentCount = networkGenerator.CountConnectedComponents(network);
                Console.WriteLine($"网络连通性检查完成，网络{(isConnected ? "是" : "不")}连通");
                if (!isConnected)
                {
                    Console.WriteLine($"网络包含 {componentCount} 个独立的连通组件");
                }
                
                Console.WriteLine($"正在保存结果到: {outputFilePath}");
                fileWriter.SaveToJson(network, outputFilePath);
                
                // 生成可视化图片
                string imageOutputPath = Path.ChangeExtension(outputFilePath, ".png");
                Console.WriteLine($"正在生成可视化图片: {imageOutputPath}");
                INetworkVisualizer visualizer = new SimpleNetworkVisualizer();
                await visualizer.VisualizeNetworkAsync(network, imageOutputPath);
                
                // 生成GIF动画
                string gifOutputPath = Path.ChangeExtension(outputFilePath, ".gif");
                Console.WriteLine($"正在生成GIF动画: {gifOutputPath}");
                await visualizer.VisualizeNetworkAsGifAsync(network, gifOutputPath);
                Console.WriteLine("GIF动画生成完成!");
                Console.WriteLine($"正在生成GIF动画: {gifOutputPath}");
                await ((SimpleNetworkVisualizer)visualizer).VisualizeNetworkAsGifAsync(network, gifOutputPath);
                Console.WriteLine("GIF动画生成完成!");
                
                Console.WriteLine("处理完成!");
                
                // 显示一些统计信息
                ShowNetworkStatistics(network, isConnected, componentCount);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"处理过程中发生错误: {ex.Message}");
                Console.WriteLine($"详细信息: {ex.StackTrace}");
            }
        }
        
        /// <summary>
        /// 显示网络统计信息
        /// </summary>
        /// <param name="network">星图网络</param>
        /// <param name="isConnected">网络是否连通</param>
        /// <param name="componentCount">连通组件数量</param>
        static void ShowNetworkStatistics(StellarNetwork network, bool isConnected, int componentCount)
        {
            Console.WriteLine("\n网络统计信息:");
            Console.WriteLine($"  跳跃距离限制: {network.JumpDistance} 秒差距");
            Console.WriteLine($"  恒星系统总数: {network.Systems.Count}");
            Console.WriteLine($"  系统间连接数: {network.Connections.Count}");
            Console.WriteLine($"  网络是否连通: {(isConnected ? "是" : "否")}");
            if (!isConnected)
            {
                Console.WriteLine($"  连通组件数量: {componentCount}");
            }
            
            // 统计多星系统
            int multiStarSystems = network.Systems.Count(s => s.Stars.Count > 0);
            Console.WriteLine($"  多星系统数量: {multiStarSystems}");
            
            // 计算平均连接数
            if (network.Systems.Count > 0)
            {
                double avgConnections = (double)network.Connections.Count * 2 / network.Systems.Count;
                Console.WriteLine($"  平均每个系统连接数: {avgConnections:F2}");
            }
            
            // 显示距离统计
            if (network.Connections.Count > 0)
            {
                double minDistance = network.Connections.Min(c => c.Distance);
                double maxDistance = network.Connections.Max(c => c.Distance);
                double avgDistance = network.Connections.Average(c => c.Distance);
                Console.WriteLine($"  连接距离 - 最小: {minDistance:F2} pc, 最大: {maxDistance:F2} pc, 平均: {avgDistance:F2} pc");
            }
        }
    }
}
