using System;
using System.IO;
using System.Threading.Tasks;
using StellarisMapper.Models;
using StellarisMapper.Services.Visualization;

namespace StellarisMapper.Tests
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("可视化功能测试");
            Console.WriteLine("=============");

            // 测试1: 基本网络可视化
            await TestBasicVisualization();

            // 测试2: 空网络可视化
            await TestEmptyNetworkVisualization();

            Console.WriteLine("所有可视化测试完成!");
        }

        static async Task TestBasicVisualization()
        {
            Console.WriteLine("\n测试1: 基本网络可视化");

            // 创建测试数据
            var network = new StellarNetwork();
            network.JumpDistance = 10.0;

            // 添加测试系统
            network.Systems.Add(new StellarSystem
            {
                Id = 0,
                Name = "Test System 1",
                SpectralType = "G",
                X = 0,
                Y = 0,
                Z = 0
            });

            network.Systems.Add(new StellarSystem
            {
                Id = 1,
                Name = "Test System 2",
                SpectralType = "M",
                X = 5,
                Y = 5,
                Z = 0
            });

            // 添加连接
            network.Connections.Add(new Connection
            {
                System1Id = 0,
                System2Id = 1,
                Distance = Math.Sqrt(50) // 约7.07
            });

            // 创建输出目录
            string outputDir = Path.Combine(Path.GetTempPath(), "stellaris_mapper_test");
            Directory.CreateDirectory(outputDir);

            // 生成可视化图片
            string outputPath = Path.Combine(outputDir, "test_visualization.png");
            INetworkVisualizer visualizer = new SimpleNetworkVisualizer();
            await visualizer.VisualizeNetworkAsync(network, outputPath);

            // 验证文件是否生成
            bool fileExists = File.Exists(outputPath);
            Console.WriteLine($"基本网络可视化文件生成: {(fileExists ? "成功" : "失败")}");

            if (fileExists)
            {
                // 验证文件大小
                FileInfo fileInfo = new FileInfo(outputPath);
                Console.WriteLine($"文件大小: {fileInfo.Length} 字节");
            }

            // 清理测试文件
            if (File.Exists(outputPath))
                File.Delete(outputPath);
            Directory.Delete(outputDir);

            Console.WriteLine("测试1完成");
        }

        static async Task TestEmptyNetworkVisualization()
        {
            Console.WriteLine("\n测试2: 空网络可视化");

            // 创建空网络
            var network = new StellarNetwork();
            network.JumpDistance = 10.0;

            // 创建输出目录
            string outputDir = Path.Combine(Path.GetTempPath(), "stellaris_mapper_test");
            Directory.CreateDirectory(outputDir);

            // 生成可视化图片
            string outputPath = Path.Combine(outputDir, "empty_visualization.png");
            INetworkVisualizer visualizer = new SimpleNetworkVisualizer();
            await visualizer.VisualizeNetworkAsync(network, outputPath);

            // 验证文件是否生成
            bool fileExists = File.Exists(outputPath);
            Console.WriteLine($"空网络可视化文件生成: {(fileExists ? "成功" : "失败")}");

            if (fileExists)
            {
                // 验证文件大小
                FileInfo fileInfo = new FileInfo(outputPath);
                Console.WriteLine($"文件大小: {fileInfo.Length} 字节");
            }

            // 清理测试文件
            if (File.Exists(outputPath))
                File.Delete(outputPath);
            Directory.Delete(outputDir);

            Console.WriteLine("测试2完成");
        }
    }
}
