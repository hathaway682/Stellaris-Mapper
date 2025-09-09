using System;
using System.IO;
using System.Threading.Tasks;
using StellarisMapper.Models;
using StellarisMapper.Services;
using StellarisMapper.Services.Visualization;

namespace StellarisMapper.VisualizationDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("星图生成工具可视化演示");
            Console.WriteLine("==================");

            // 读取生成的JSON文件
            string jsonPath = "/home/admin/workspace/stellaris_mapper/output/final_stellar_network.json";
            if (!File.Exists(jsonPath))
            {
                Console.WriteLine($"错误: 找不到JSON文件 {jsonPath}");
                return;
            }

            // 读取JSON数据
            IFileWriter fileWriter = new FileWriter();
            string jsonContent = File.ReadAllText(jsonPath);
            
            // 这里我们简化处理，直接使用已有的网络数据
            // 在实际应用中，我们会反序列化JSON数据
            var dataReader = new ExcelDataReaderService();
            var starRecords = dataReader.ReadStarData("/home/admin/workspace/stellaris_mapper/The10pcSample_v2.xlsx");
            var coordinateConverter = new CoordinateConverter();
            var networkGenerator = new StellarNetworkGenerator(coordinateConverter);
            var network = networkGenerator.GenerateNetwork(starRecords, 10.0);

            // 生成可视化图片
            string imagePath = "/home/admin/workspace/stellaris_mapper/output/demo_visualization.png";
            INetworkVisualizer visualizer = new SimpleNetworkVisualizer();
            await visualizer.VisualizeNetworkAsync(network, imagePath);

            Console.WriteLine($"可视化演示完成!");
            Console.WriteLine($"生成的图片路径: {imagePath}");
            
            // 显示文件信息
            if (File.Exists(imagePath))
            {
                FileInfo fileInfo = new FileInfo(imagePath);
                Console.WriteLine($"图片文件大小: {fileInfo.Length} 字节 ({fileInfo.Length / 1024.0 / 1024.0:F2} MB)");
            }
        }
    }
}
