using System;
using System.Collections.Generic;
using System.Linq;
using MIConvexHull;
using StellarisMapper.Models;

namespace StellarisMapper.Services
{
    // 定义顶点类
    public class StellarVertex : IVertex
    {
        public double[] Position { get; set; } = new double[3];
        public int SystemId { get; set; }
    }
    
    // 定义单元类
    public class StellarCell : TriangulationCell<StellarVertex, StellarCell>
    {
    }
    
    /// <summary>
    /// 星图网络生成器实现
    /// </summary>
    public class StellarNetworkGenerator : IStellarNetworkGenerator
    {
        private readonly ICoordinateConverter _coordinateConverter;
        
        public StellarNetworkGenerator(ICoordinateConverter coordinateConverter)
        {
            _coordinateConverter = coordinateConverter;
        }
        
        /// <summary>
        /// 生成星图网络
        /// </summary>
        /// <param name="starRecords">星表记录</param>
        /// <param name="maxJumpDistance">最大跳跃距离（秒差距）</param>
        /// <param name="includeLM">是否包含LM/LM?对象</param>
        /// <param name="includeWD">是否包含WD/WD?对象</param>
        /// <param name="includeBD">是否包含BD/BD?对象</param>
        /// <returns>星图网络</returns>
        public StellarNetwork GenerateNetwork(List<StarRecord> starRecords, double maxJumpDistance, 
            bool includeLM = false, bool includeWD = false, bool includeBD = false)
        {
            // 创建星图网络对象
            var network = new StellarNetwork
            {
                JumpDistance = maxJumpDistance
            };
            
            // 1. 过滤掉行星对象，仅保留恒星
            var starData = starRecords.Where(r => r.OBJ_CAT != "Planet").ToList();
            
            // 2. 根据选项过滤掉LM/LM?、WD/WD?、BD/BD?对象
            if (!includeLM)
            {
                starData = starData.Where(r => r.OBJ_CAT != "LM" && r.OBJ_CAT != "LM?").ToList();
            }
            
            if (!includeWD)
            {
                starData = starData.Where(r => r.OBJ_CAT != "WD" && r.OBJ_CAT != "WD?").ToList();
            }
            
            if (!includeBD)
            {
                starData = starData.Where(r => r.OBJ_CAT != "BD" && r.OBJ_CAT != "BD?").ToList();
            }
            
            // 2. 自动识别多星系统（拥有相同的星系ID），并合并为一点
            var systemGroups = starData.GroupBy(r => r.NB_SYS).ToList();
            
            // 3. 将天球赤道坐标系转换为游戏常用的三维直角坐标系
            var systems = new List<StellarSystem>();
            int systemId = 0;
            
            foreach (var group in systemGroups)
            {
                // 系统名称使用SYSTEM_NAME或第一个星体的OBJ_NAME
                string systemName = group.First().SYSTEM_NAME;
                if (string.IsNullOrEmpty(systemName))
                    systemName = group.First().OBJ_NAME;
                
                // 计算系统的平均坐标
                double avgRa = group.Average(r => r.RA);
                double avgDec = group.Average(r => r.DEC);
                double avgParallax = group.Average(r => r.PARALLAX);
                
                // 获取主星的光谱类型（第一个星体）
                string primarySpectralType = group.First().SP_TYPE;
                
                // 获取对象类型（第一个星体）
                string objectType = group.First().OBJ_CAT;
                
                // 转换为直角坐标
                var (x, y, z) = _coordinateConverter.ConvertToCartesian(avgRa, avgDec, avgParallax);
                
                // 创建恒星系统对象
                var system = new StellarSystem
                {
                    Id = systemId++,
                    Name = systemName,
                    ObjectType = objectType,
                    SpectralType = primarySpectralType,
                    X = x,
                    Y = y,
                    Z = z
                };
                
                // 如果是多星系统，添加各个恒星的信息
                if (group.Count() > 1)
                {
                    foreach (var starRecord in group)
                    {
                        system.Stars.Add(new Star
                        {
                            Name = starRecord.OBJ_NAME,
                            SpectralType = starRecord.SP_TYPE
                        });
                    }
                }
                
                systems.Add(system);
            }
            
            network.Systems = systems;
            
            // 4. 将星表数据处理为点集后，进行Delaunay四面体剖分
            var vertices = systems.Select((s, index) => new StellarVertex 
            { 
                Position = new[] { s.X, s.Y, s.Z },
                SystemId = index
            }).ToArray();
            
            // 使用默认的平面距离容差值
            var delaunay = DelaunayTriangulation<StellarVertex, StellarCell>.Create(vertices, 1e-10);
            
            // 5. 对四面体剖分生成的连接网络进行优化，删除过远的连接
            var connections = new List<Connection>();
            
            if (delaunay != null && delaunay.Cells != null)
            {
                foreach (var cell in delaunay.Cells)
                {
                    // 获取四面体的四个顶点
                    var verticesInCell = cell.Vertices;
                    
                    // 检查每一对顶点之间的连接
                    for (int i = 0; i < verticesInCell.Length; i++)
                    {
                        for (int j = i + 1; j < verticesInCell.Length; j++)
                        {
                            var vertex1 = verticesInCell[i];
                            var vertex2 = verticesInCell[j];
                            
                            // 获取系统ID
                            int system1Id = vertex1.SystemId;
                            int system2Id = vertex2.SystemId;
                            
                            if (system1Id != system2Id)
                            {
                                // 计算两个系统之间的距离
                                double distance = CalculateDistance(vertex1.Position, vertex2.Position);
                                
                                // 只添加在跳跃距离内的连接
                                if (distance <= maxJumpDistance)
                                {
                                    // 检查是否已存在相同的连接
                                    bool connectionExists = connections.Any(c => 
                                        (c.System1Id == system1Id && c.System2Id == system2Id) ||
                                        (c.System1Id == system2Id && c.System2Id == system1Id));
                                    
                                    if (!connectionExists)
                                    {
                                        connections.Add(new Connection
                                        {
                                            System1Id = system1Id,
                                            System2Id = system2Id,
                                            Distance = distance
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }
            
            network.Connections = connections;
            
            return network;
        }
        
        /// <summary>
        /// 检查网络是否连通
        /// </summary>
        /// <param name="network">星图网络</param>
        /// <returns>如果网络连通返回true，否则返回false</returns>
        public bool IsNetworkConnected(StellarNetwork network)
        {
            return CountConnectedComponents(network) == 1;
        }
        
        /// <summary>
        /// 计算网络中连通组件的数量
        /// </summary>
        /// <param name="network">星图网络</param>
        /// <returns>连通组件的数量</returns>
        public int CountConnectedComponents(StellarNetwork network)
        {
            // 如果没有系统，返回0
            if (network.Systems.Count == 0)
                return 0;
            
            // 如果只有一个系统或没有连接，返回系统数量
            if (network.Systems.Count == 1 || network.Connections.Count == 0)
                return network.Systems.Count;
            
            // 使用广度优先搜索(BFS)计算连通组件数量
            var visited = new HashSet<int>();
            int componentCount = 0;
            
            // 构建邻接表
            var adjacencyList = new Dictionary<int, List<int>>();
            foreach (var connection in network.Connections)
            {
                if (!adjacencyList.ContainsKey(connection.System1Id))
                    adjacencyList[connection.System1Id] = new List<int>();
                
                if (!adjacencyList.ContainsKey(connection.System2Id))
                    adjacencyList[connection.System2Id] = new List<int>();
                
                adjacencyList[connection.System1Id].Add(connection.System2Id);
                adjacencyList[connection.System2Id].Add(connection.System1Id);
            }
            
            // 遍历所有系统
            foreach (var system in network.Systems)
            {
                // 如果系统未被访问过，开始新的连通组件
                if (!visited.Contains(system.Id))
                {
                    componentCount++;
                    var queue = new Queue<int>();
                    queue.Enqueue(system.Id);
                    visited.Add(system.Id);
                    
                    // BFS遍历当前连通组件
                    while (queue.Count > 0)
                    {
                        int currentSystemId = queue.Dequeue();
                        
                        // 检查当前系统是否有邻接系统
                        if (adjacencyList.ContainsKey(currentSystemId))
                        {
                            foreach (int neighborId in adjacencyList[currentSystemId])
                            {
                                if (!visited.Contains(neighborId))
                                {
                                    visited.Add(neighborId);
                                    queue.Enqueue(neighborId);
                                }
                            }
                        }
                    }
                }
            }
            
            return componentCount;
        }
        
        /// <summary>
        /// 计算两个点之间的欧几里得距离
        /// </summary>
        /// <param name="point1">第一个点</param>
        /// <param name="point2">第二个点</param>
        /// <returns>两点之间的距离</returns>
        private double CalculateDistance(double[] point1, double[] point2)
        {
            double sum = 0;
            for (int i = 0; i < point1.Length; i++)
            {
                double diff = point1[i] - point2[i];
                sum += diff * diff;
            }
            return Math.Sqrt(sum);
        }
    }
}
