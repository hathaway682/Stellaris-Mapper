using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.Fonts;
using SixLabors.ImageSharp.Formats.Gif;
using StellarisMapper.Models;

namespace StellarisMapper.Services.Visualization
{
    /// <summary>
    /// 简单的网络可视化器实现
    /// </summary>
    public class SimpleNetworkVisualizer : INetworkVisualizer
    {
        /// <summary>
        /// 将星图网络可视化为PNG图片文件
        /// </summary>
        /// <param name="network">星图网络</param>
        /// <param name="outputPath">输出文件路径</param>
        /// <returns>任务</returns>
        public async Task VisualizeNetworkAsync(StellarNetwork network, string outputPath)
        {
            const int width = 2000;
            const int height = 2000;
            
            // 计算坐标范围
            var allSystems = network.Systems;
            if (!allSystems.Any()) return;
            
            float minX = (float)allSystems.Min(s => s.X);
            float maxX = (float)allSystems.Max(s => s.X);
            float minY = (float)allSystems.Min(s => s.Y);
            float maxY = (float)allSystems.Max(s => s.Y);
            float minZ = (float)allSystems.Min(s => s.Z);
            float maxZ = (float)allSystems.Max(s => s.Z);
            
            float rangeX = maxX - minX;
            float rangeY = maxY - minY;
            float rangeZ = maxZ - minZ;
            
            // 添加边距
            float margin = 0.1f;
            minX -= rangeX * margin;
            maxX += rangeX * margin;
            minY -= rangeY * margin;
            maxY += rangeY * margin;
            minZ -= rangeZ * margin;
            maxZ += rangeZ * margin;
            
            rangeX = maxX - minX;
            rangeY = maxY - minY;
            rangeZ = maxZ - minZ;
            
            // 如果范围为0，设置默认值
            if (rangeX == 0) rangeX = 1;
            if (rangeY == 0) rangeY = 1;
            if (rangeZ == 0) rangeZ = 1;
            
            // 缩放因子
            float scaleX = (float)(width / rangeX);
            float scaleY = (float)(height / rangeY);
            
            // 创建图像
            using (var image = new Image<Rgba32>(width, height))
            {
                // 设置背景色为黑色
                image.Mutate(ctx => ctx.Fill(Color.Black));
                
                // 绘制连接线
                foreach (var connection in network.Connections)
                {
                    var system1 = network.Systems.FirstOrDefault(s => s.Id == connection.System1Id);
                    var system2 = network.Systems.FirstOrDefault(s => s.Id == connection.System2Id);
                    
                    if (system1 != null && system2 != null)
                    {
                        // 转换为屏幕坐标
                        float screenX1 = (float)((system1.X - minX) * scaleX);
                        float screenY1 = (float)((system1.Y - minY) * scaleY);
                        float screenX2 = (float)((system2.X - minX) * scaleX);
                        float screenY2 = (float)((system2.Y - minY) * scaleY);
                        
                        // 绘制连接线
                        image.Mutate(ctx => ctx.DrawLine(Color.LightBlue, 1, new PointF[] { 
                            new PointF(screenX1, screenY1), 
                            new PointF(screenX2, screenY2) 
                        }));
                    }
                }
                
                // 绘制恒星系统
                foreach (var system in network.Systems)
                {
                    // 转换为屏幕坐标
                    float screenX = (float)((system.X - minX) * scaleX);
                    float screenY = (float)((system.Y - minY) * scaleY);
                    
                    // 获取恒星颜色和大小
                    var (starColor, starSize) = GetStarColorAndSize(system);
                    
                    // 对于多星系统，绘制环形标记
                    if (system.Stars != null && system.Stars.Count > 1)
                    {
                        // 绘制环形标记表示多星系统
                        var center = new PointF(screenX, screenY);
                        image.Mutate(ctx => ctx.Draw(starColor, 1, new SixLabors.ImageSharp.Drawing.EllipsePolygon(center, starSize)));
                        
                        // 绘制每个恒星组件
                        float componentRadius = starSize;
                        float systemRadius = starSize * 1.5f;
                        
                        for (int i = 0; i < system.Stars.Count; i++)
                        {
                            double componentAngle = 2 * Math.PI * i / system.Stars.Count;
                            float componentX = screenX + (float)(systemRadius * Math.Cos(componentAngle));
                            float componentY = screenY + (float)(systemRadius * Math.Sin(componentAngle));
                            
                            // 根据恒星组件的光谱类型获取颜色
                            var componentColor = GetStarColorBySpectralType(system.Stars[i].SpectralType);
                            
                            var componentCenter = new PointF(componentX, componentY);
                            image.Mutate(ctx => ctx.Fill(componentColor, new SixLabors.ImageSharp.Drawing.EllipsePolygon(componentCenter, componentRadius)));
                        }
                    }
                    else
                    {
                        // 单星系统
                        var center = new PointF(screenX, screenY);
                        image.Mutate(ctx => ctx.Fill(starColor, new SixLabors.ImageSharp.Drawing.EllipsePolygon(center, starSize)));
                    }
                }
                
                // 确保输出目录存在
                var directory = Path.GetDirectoryName(outputPath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                
                // 保存图像
                await image.SaveAsPngAsync(outputPath);
            }
        }
        
        /// <summary>
        /// 将星图网络可视化为GIF动画文件
        /// </summary>
        /// <param name="network">星图网络</param>
        /// <param name="outputPath">输出文件路径</param>
        /// <returns>任务</returns>
        public async Task VisualizeNetworkAsGifAsync(StellarNetwork network, string outputPath)
        {
            const int width = 2000;
            const int height = 2000;
            const int frames = 360; // 生成360帧，每帧1度
            
            // 计算坐标范围
            var allSystems = network.Systems;
            if (!allSystems.Any()) return;
            
            float minX = (float)allSystems.Min(s => s.X);
            float maxX = (float)allSystems.Max(s => s.X);
            float minY = (float)allSystems.Min(s => s.Y);
            float maxY = (float)allSystems.Max(s => s.Y);
            float minZ = (float)allSystems.Min(s => s.Z);
            float maxZ = (float)allSystems.Max(s => s.Z);
            
            float rangeX = maxX - minX;
            float rangeY = maxY - minY;
            float rangeZ = maxZ - minZ;
            
            // 添加边距
            float margin = 0.1f;
            minX -= rangeX * margin;
            maxX += rangeX * margin;
            minY -= rangeY * margin;
            maxY += rangeY * margin;
            minZ -= rangeZ * margin;
            maxZ += rangeZ * margin;
            
            rangeX = maxX - minX;
            rangeY = maxY - minY;
            rangeZ = maxZ - minZ;
            
            // 缩放因子
            float scaleX = (float)(width / rangeX);
            float scaleY = (float)(height / rangeY);
            
            // 创建帧列表
            var frameImages = new List<Image<Rgba32>>();
            
            try
            {
                // 为每个帧创建旋转视角
                for (int frame = 0; frame < frames; frame++)
                {
                    // 计算当前帧的旋转角度（绕Y轴旋转）
                    double angle = 2 * Math.PI * frame / frames;
                    float cosAngle = (float)Math.Cos(angle);
                    float sinAngle = (float)Math.Sin(angle);
                    
                    // 创建新帧
                    var frameImage = new Image<Rgba32>(width, height);
                    frameImages.Add(frameImage);
                    
                    // 设置背景色为黑色
                    frameImage.Mutate(ctx => ctx.Fill(Color.Black));
                    
                    // 绘制连接线
                    foreach (var connection in network.Connections)
                    {
                        var system1 = network.Systems.FirstOrDefault(s => s.Id == connection.System1Id);
                        var system2 = network.Systems.FirstOrDefault(s => s.Id == connection.System2Id);
                        
                        if (system1 != null && system2 != null)
                        {
                            // 应用Y轴旋转矩阵
                            float x1 = (float)(system1.X * cosAngle - system1.Z * sinAngle);
                            float z1 = (float)(system1.X * sinAngle + system1.Z * cosAngle);
                            float x2 = (float)(system2.X * cosAngle - system2.Z * sinAngle);
                            float z2 = (float)(system2.X * sinAngle + system2.Z * cosAngle);
                            
                            // 转换为屏幕坐标
                            float screenX1 = (float)((x1 - minX) * scaleX);
                            float screenY1 = (float)((system1.Y - minY) * scaleY);
                            float screenX2 = (float)((x2 - minX) * scaleX);
                            float screenY2 = (float)((system2.Y - minY) * scaleY);
                            
                            // 绘制连接线
                            frameImage.Mutate(ctx => ctx.DrawLine(Color.LightBlue, 1, new PointF[] { 
                                new PointF(screenX1, screenY1), 
                                new PointF(screenX2, screenY2) 
                            }));
                        }
                    }
                    
                    // 绘制恒星系统
                    foreach (var system in network.Systems)
                    {
                        // 应用Y轴旋转矩阵
                        float x = (float)(system.X * cosAngle - system.Z * sinAngle);
                        float z = (float)(system.X * sinAngle + system.Z * cosAngle);
                        
                        // 转换为屏幕坐标
                        float screenX = (float)((x - minX) * scaleX);
                        float screenY = (float)((system.Y - minY) * scaleY);
                        
                        // 获取恒星颜色和大小
                        var (starColor, starSize) = GetStarColorAndSize(system);
                        
                        // 对于多星系统，绘制环形标记
                        if (system.Stars != null && system.Stars.Count > 1)
                        {
                            // 绘制环形标记表示多星系统
                            var center = new PointF(screenX, screenY);
                            frameImage.Mutate(ctx => ctx.Draw(starColor, 1, new SixLabors.ImageSharp.Drawing.EllipsePolygon(center, starSize)));
                            
                            // 绘制每个恒星组件
                            float componentRadius = starSize;
                            float systemRadius = starSize * 1.5f;
                            
                            for (int i = 0; i < system.Stars.Count; i++)
                            {
                                double componentAngle = 2 * Math.PI * i / system.Stars.Count;
                                float componentX = screenX + (float)(systemRadius * Math.Cos(componentAngle));
                                float componentY = screenY + (float)(systemRadius * Math.Sin(componentAngle));
                                
                                // 根据恒星组件的光谱类型获取颜色
                                var componentColor = GetStarColorBySpectralType(system.Stars[i].SpectralType);
                                
                                var componentCenter = new PointF(componentX, componentY);
                                frameImage.Mutate(ctx => ctx.Fill(componentColor, new SixLabors.ImageSharp.Drawing.EllipsePolygon(componentCenter, componentRadius)));
                            }
                        }
                        else
                        {
                            // 单星系统
                            var center = new PointF(screenX, screenY);
                            frameImage.Mutate(ctx => ctx.Fill(starColor, new SixLabors.ImageSharp.Drawing.EllipsePolygon(center, starSize)));
                        }
                    }
                }
                
                // 创建GIF图像
                using (var gif = new Image<Rgba32>(width, height))
                {
                    // 添加所有帧到GIF
                    for (int i = 0; i < frameImages.Count; i++)
                    {
                        gif.Frames.AddFrame(frameImages[i].Frames.RootFrame);
                    }
                    
                    // 设置帧延迟（10 = 1/10秒）
                    for (int i = 0; i < gif.Frames.Count; i++)
                    {
                        var metadata = gif.Frames[i].Metadata.GetFormatMetadata(GifFormat.Instance);
                        ((GifFrameMetadata)metadata).FrameDelay = 10;
                    }
                    
                    // 确保输出目录存在
                    var directory = Path.GetDirectoryName(outputPath);
                    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    
                    // 保存GIF
                    await gif.SaveAsGifAsync(outputPath);
                }
            }
            finally
            {
                // 释放所有帧图像
                foreach (var frameImage in frameImages)
                {
                    frameImage.Dispose();
                }
            }
        }
        
        /// <summary>
        /// 根据恒星对象类型和光谱类型获取颜色和大小
        /// </summary>
        /// <param name="system">恒星系统</param>
        /// <returns>颜色和大小的元组</returns>
        private (Color color, float size) GetStarColorAndSize(StellarSystem system)
        {
            // 默认大小
            float size = 3f;
          
            // 根据对象类型调整大小
            if (system.ObjectType == "*" && system.Stars == null)
            {
                size = 5f; // 大质量恒星绘制得更大
            }
            
            // 根据对象类型和光谱类型获取颜色
            var color = GetStarColorByObjectTypeAndSpectralType(system.ObjectType, system.SpectralType);
            
            return (color, size);
        }
        
        /// <summary>
        /// 根据对象类型和光谱类型获取恒星颜色
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="spectralType">光谱类型</param>
        /// <returns>颜色</returns>
        private Color GetStarColorByObjectTypeAndSpectralType(string objectType, string spectralType)
        {
            // 特殊对象类型处理
            switch (objectType)
            {
                case "WD":
                case "WD?":
                    return Color.FromRgb(0, 255, 0); // 绿色（白矮星）
                case "BD":
                case "BD?":
                    return Color.FromRgb(255, 0, 255); // 紫色（褐矮星）
                case "LM":
                case "LM?":
                    // 对于低质量天体，根据光谱类型确定颜色
                    return GetStarColorBySpectralType(spectralType);
                case "*":
                    // 对于大质量恒星，根据光谱类型确定颜色
                    return GetStarColorBySpectralType(spectralType);
                default:
                    // 默认根据光谱类型确定颜色
                    return GetStarColorBySpectralType(spectralType);
            }
        }
        
        /// <summary>
        /// 根据光谱类型获取恒星颜色
        /// </summary>
        /// <param name="spectralType">光谱类型</param>
        /// <returns>颜色</returns>
        private Color GetStarColorBySpectralType(string spectralType)
        {
            if (string.IsNullOrEmpty(spectralType))
                return Color.White; // 默认白色
                
            // 只取第一个字符作为光谱类型
            char firstChar = spectralType.ToUpper()[0];
            
            switch (firstChar)
            {
                case 'O':
                    return Color.FromRgb(155, 176, 255); // 蓝色
                case 'B':
                    return Color.FromRgb(170, 191, 255); // 蓝白色
                case 'A':
                    return Color.FromRgb(0, 255, 255); // 青色
                case 'F':
                    return Color.FromRgb(255, 255, 255); // 白色
                case 'G':
                    return Color.FromRgb(255, 255, 0); // 黄色
                case 'K':
                    return Color.FromRgb(255, 127, 0); // 橙色
                case 'M':
                    return Color.FromRgb(255, 0, 0); // 红色
                case 'L':
                    return Color.FromRgb(255, 0, 255); // 紫色（褐矮星）
                default:
                    return Color.White; // 默认白色
            }
        }
    }
}
