using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using StellarisMapper.Models;
using StellarisMapper.Services.Visualization;

namespace StellarisMapper.MultiStarDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("多星系统可视化演示程序");
            Console.WriteLine("========================");
            
            // 创建一个包含半人马座阿尔法星系统的演示网络
            var network = new StellarNetwork
            {
                JumpDistance = 10.0,
                Systems = new List<StellarSystem>
                {
                    new StellarSystem
                    {
                        Id = 0,
                        Name = "alf Cen",
                        ObjectType = "*",
                        SpectralType = "G", // 主星的光谱类型
                        X = 0,
                        Y = 0,
                        Z = 0,
                        Stars = new List<Star>
                        {
                            new Star { Name = "Proxima Cen", SpectralType = "M" },
                            new Star { Name = "alf Cen A", SpectralType = "G" },
                            new Star { Name = "alf Cen B", SpectralType = "K" }
                        }
                    },
                    new StellarSystem
                    {
                        Id = 1,
                        Name = "Single Star",
                        ObjectType = "*",
                        SpectralType = "M",
                        X = 50,
                        Y = 50,
                        Z = 0,
                        Stars = new List<Star>() // 单星系统
                    }
                },
                Connections = new List<Connection>
                {
                    new Connection
                    {
                        System1Id = 0,
                        System2Id = 1,
                        Distance = 70.71
                    }
                }
            };
            
            // 创建可视化器并生成图片
            var visualizer = new SimpleNetworkVisualizer();
            string outputPath = Path.Combine("output", "multistar_demo.png");
            
            Console.WriteLine($"正在生成可视化图片: {outputPath}");
            await visualizer.VisualizeNetworkAsync(network, outputPath);
            Console.WriteLine("可视化图片生成完成!");
            
            // 检查文件是否存在
            if (File.Exists(outputPath))
            {
                var fileInfo = new FileInfo(outputPath);
                Console.WriteLine($"文件大小: {fileInfo.Length} 字节");
                Console.WriteLine($"文件路径: {Path.GetFullPath(outputPath)}");
            }
            else
            {
                Console.WriteLine("错误: 文件未生成");
            }
        }
    }
}
