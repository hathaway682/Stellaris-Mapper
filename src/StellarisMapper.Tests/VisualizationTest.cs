using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StellarisMapper.Models;
using StellarisMapper.Services.Visualization;

namespace StellarisMapper.Tests
{
    [TestClass]
    public class VisualizationTest
    {
        [TestMethod]
        public async Task TestVisualizationGeneration()
        {
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
            Assert.IsTrue(File.Exists(outputPath), "可视化图片文件未生成");
            
            // 验证文件大小
            FileInfo fileInfo = new FileInfo(outputPath);
            Assert.IsTrue(fileInfo.Length > 0, "可视化图片文件为空");
            
            // 清理测试文件
            File.Delete(outputPath);
            Directory.Delete(outputDir);
        }
        
        [TestMethod]
        public async Task TestEmptyNetworkVisualization()
        {
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
            Assert.IsTrue(File.Exists(outputPath), "空网络可视化图片文件未生成");
            
            // 验证文件大小
            FileInfo fileInfo = new FileInfo(outputPath);
            Assert.IsTrue(fileInfo.Length > 0, "空网络可视化图片文件为空");
            
            // 清理测试文件
            File.Delete(outputPath);
            Directory.Delete(outputDir);
        }
    }
}
