using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using StellarisMapper.Models;
using StellarisMapper.Services.Visualization;

namespace StellarisMapper.ColorDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("星图生成工具 - 颜色编码演示");
            Console.WriteLine("========================");
            
            // 创建一个更复杂的测试网络，展示不同类型的恒星
            var testNetwork = new StellarNetwork
            {
                JumpDistance = 10.0,
                Systems = new List<StellarSystem>
                {
                    // 主序星
                    new StellarSystem { Id = 0, Name = "天狼星", ObjectType = "*", SpectralType = "A", X = 100, Y = 100, Z = 0 },
                    new StellarSystem { Id = 1, Name = "太阳", ObjectType = "*", SpectralType = "G", X = 200, Y = 150, Z = 0 },
                    new StellarSystem { Id = 2, Name = "阿尔法半人马座", ObjectType = "LM", SpectralType = "G", X = 300, Y = 120, Z = 0 },
                    new StellarSystem { Id = 3, Name = "比邻星", ObjectType = "LM", SpectralType = "M", X = 320, Y = 130, Z = 0 },
                    new StellarSystem { Id = 4, Name = "巴纳德星", ObjectType = "LM?", SpectralType = "M", X = 400, Y = 180, Z = 0 },
                    new StellarSystem { Id = 5, Name = "沃夫359", ObjectType = "*", SpectralType = "M", X = 500, Y = 110, Z = 0 },
                    
                    // 特殊恒星
                    new StellarSystem { Id = 6, Name = "天狼星B", ObjectType = "WD", SpectralType = "DA", X = 120, Y = 110, Z = 0 },
                    new StellarSystem { Id = 7, Name = "范马南星", ObjectType = "WD?", SpectralType = "DQ", X = 220, Y = 170, Z = 0 },
                    new StellarSystem { Id = 8, Name = "LHS 2924", ObjectType = "BD", SpectralType = "L", X = 320, Y = 160, Z = 0 },
                    new StellarSystem { Id = 9, Name = "2MASS J04151954+0935069", ObjectType = "BD?", SpectralType = "T", X = 420, Y = 190, Z = 0 },
                    
                    // 大质量恒星
                    new StellarSystem { Id = 10, Name = "参宿四", ObjectType = "*", SpectralType = "M", X = 150, Y = 250, Z = 0 },
                    new StellarSystem { Id = 11, Name = "参宿七", ObjectType = "*", SpectralType = "B", X = 250, Y = 280, Z = 0 },
                    new StellarSystem { Id = 12, Name = "心宿二", ObjectType = "*", SpectralType = "M", X = 350, Y = 260, Z = 0 },
                },
                Connections = new List<Connection>
                {
                    // 主序星之间的连接
                    new Connection { System1Id = 0, System2Id = 1, Distance = 8.6 },
                    new Connection { System1Id = 1, System2Id = 2, Distance = 4.3 },
                    new Connection { System1Id = 2, System2Id = 3, Distance = 0.2 },
                    new Connection { System1Id = 2, System2Id = 4, Distance = 6.0 },
                    new Connection { System1Id = 4, System2Id = 5, Distance = 7.8 },
                    
                    // 特殊恒星连接
                    new Connection { System1Id = 0, System2Id = 6, Distance = 0.01 },
                    new Connection { System1Id = 1, System2Id = 7, Distance = 15.2 },
                    new Connection { System1Id = 2, System2Id = 8, Distance = 3.1 },
                    new Connection { System1Id = 8, System2Id = 9, Distance = 2.4 },
                    
                    // 大质量恒星连接
                    new Connection { System1Id = 10, System2Id = 11, Distance = 9.5 },
                    new Connection { System1Id = 11, System2Id = 12, Distance = 7.2 },
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
            var outputPath = Path.Combine(outputDir, "stellaris_color_demo.png");
            
            Console.WriteLine($"正在生成演示图片: {outputPath}");
            await visualizer.VisualizeNetworkAsync(testNetwork, outputPath);
            Console.WriteLine("演示图片生成完成!");
            
            // 显示颜色编码说明
            Console.WriteLine("\n颜色编码说明:");
            Console.WriteLine("================================");
            Console.WriteLine("主序星 (OBJ_CAT: *, LM, LM?):");
            Console.WriteLine("  O型: 蓝色 (RGB: 155, 176, 255)");
            Console.WriteLine("  B型: 蓝白色 (RGB: 170, 191, 255)");
            Console.WriteLine("  A型: 青色 (RGB: 0, 255, 255)");
            Console.WriteLine("  F型: 白色 (RGB: 255, 255, 255)");
            Console.WriteLine("  G型: 黄色 (RGB: 255, 255, 0)");
            Console.WriteLine("  K型: 橙色 (RGB: 255, 127, 0)");
            Console.WriteLine("  M型: 红色 (RGB: 255, 0, 0)");
            Console.WriteLine("  L型: 紫色 (RGB: 255, 0, 255)");
            Console.WriteLine("\n特殊恒星:");
            Console.WriteLine("  白矮星 (WD/WD?): 青色 (RGB: 0, 255, 255)");
            Console.WriteLine("  褐矮星 (BD/BD?): 紫色 (RGB: 255, 0, 255)");
            Console.WriteLine("\n大小说明:");
            Console.WriteLine("  大质量恒星 (OBJ_CAT: *): 半径5px");
            Console.WriteLine("  其他恒星: 半径3px");
            
            Console.WriteLine("\n演示完成! 请查看输出目录中的stellaris_color_demo.png文件。");
        }
    }
}
