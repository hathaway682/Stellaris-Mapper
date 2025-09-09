using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using StellarisMapper.Models;
using StellarisMapper.Services.Visualization;

namespace StellarisMapper.ColorTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("星图可视化颜色测试");
            
            // 创建测试网络
            var testNetwork = new StellarNetwork
            {
                JumpDistance = 10.0,
                Systems = new List<StellarSystem>
                {
                    // 测试不同光谱类型的恒星
                    new StellarSystem { Id = 0, Name = "O型恒星", ObjectType = "*", SpectralType = "O", X = 100, Y = 100, Z = 0 },
                    new StellarSystem { Id = 1, Name = "B型恒星", ObjectType = "*", SpectralType = "B", X = 200, Y = 100, Z = 0 },
                    new StellarSystem { Id = 2, Name = "A型恒星", ObjectType = "*", SpectralType = "A", X = 300, Y = 100, Z = 0 },
                    new StellarSystem { Id = 3, Name = "F型恒星", ObjectType = "*", SpectralType = "F", X = 400, Y = 100, Z = 0 },
                    new StellarSystem { Id = 4, Name = "G型恒星", ObjectType = "*", SpectralType = "G", X = 500, Y = 100, Z = 0 },
                    new StellarSystem { Id = 5, Name = "K型恒星", ObjectType = "*", SpectralType = "K", X = 600, Y = 100, Z = 0 },
                    new StellarSystem { Id = 6, Name = "M型恒星", ObjectType = "*", SpectralType = "M", X = 700, Y = 100, Z = 0 },
                    new StellarSystem { Id = 7, Name = "L型恒星", ObjectType = "*", SpectralType = "L", X = 800, Y = 100, Z = 0 },
                    
                    // 测试特殊类型的恒星
                    new StellarSystem { Id = 8, Name = "白矮星", ObjectType = "WD", SpectralType = "DA", X = 100, Y = 200, Z = 0 },
                    new StellarSystem { Id = 9, Name = "白矮星?", ObjectType = "WD?", SpectralType = "DB", X = 200, Y = 200, Z = 0 },
                    new StellarSystem { Id = 10, Name = "褐矮星", ObjectType = "BD", SpectralType = "L", X = 300, Y = 200, Z = 0 },
                    new StellarSystem { Id = 11, Name = "褐矮星?", ObjectType = "BD?", SpectralType = "T", X = 400, Y = 200, Z = 0 },
                    
                    // 测试大质量恒星
                    new StellarSystem { Id = 12, Name = "大质量恒星", ObjectType = "*", SpectralType = "O", X = 100, Y = 300, Z = 0 },
                    
                    // 测试LM类型恒星
                    new StellarSystem { Id = 13, Name = "LM恒星", ObjectType = "LM", SpectralType = "M", X = 200, Y = 300, Z = 0 },
                    new StellarSystem { Id = 14, Name = "LM?恒星", ObjectType = "LM?", SpectralType = "M", X = 300, Y = 300, Z = 0 }
                },
                Connections = new List<Connection>
                {
                    // 添加一些连接用于测试
                    new Connection { System1Id = 0, System2Id = 1, Distance = 5.0 },
                    new Connection { System1Id = 1, System2Id = 2, Distance = 5.0 },
                    new Connection { System1Id = 2, System2Id = 3, Distance = 5.0 },
                    new Connection { System1Id = 3, System2Id = 4, Distance = 5.0 },
                    new Connection { System1Id = 4, System2Id = 5, Distance = 5.0 },
                    new Connection { System1Id = 5, System2Id = 6, Distance = 5.0 },
                    new Connection { System1Id = 6, System2Id = 7, Distance = 5.0 },
                }
            };
            
            // 确保输出目录存在
            var outputDir = Path.Combine(Environment.CurrentDirectory, "output");
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }
            
            // 生成可视化图片
            var visualizer = new SimpleNetworkVisualizer();
            var outputPath = Path.Combine(outputDir, "color_test.png");
            
            Console.WriteLine($"正在生成颜色测试图片: {outputPath}");
            await visualizer.VisualizeNetworkAsync(testNetwork, outputPath);
            Console.WriteLine("颜色测试图片生成完成!");
            
            // 显示颜色映射说明
            Console.WriteLine("\n颜色映射说明:");
            Console.WriteLine("O型恒星: 蓝色 (RGB: 155, 176, 255)");
            Console.WriteLine("B型恒星: 蓝白色 (RGB: 170, 191, 255)");
            Console.WriteLine("A型恒星: 青色 (RGB: 0, 255, 255)");
            Console.WriteLine("F型恒星: 白色 (RGB: 255, 255, 255)");
            Console.WriteLine("G型恒星: 黄色 (RGB: 255, 255, 0)");
            Console.WriteLine("K型恒星: 橙色 (RGB: 255, 127, 0)");
            Console.WriteLine("M型恒星: 红色 (RGB: 255, 0, 0)");
            Console.WriteLine("L型恒星: 紫色 (RGB: 255, 0, 255)");
            Console.WriteLine("白矮星 (WD/WD?): 青色 (RGB: 0, 255, 255)");
            Console.WriteLine("褐矮星 (BD/BD?): 紫色 (RGB: 255, 0, 255)");
            Console.WriteLine("大质量恒星 (*): 比普通恒星大 (半径5px vs 3px)");
            Console.WriteLine("LM/LM?恒星: 与普通恒星相同颜色编码");
        }
    }
}
