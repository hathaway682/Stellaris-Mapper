using GraphX.Common.Models;

namespace StellarisMapper.Services.Visualization
{
    /// <summary>
    /// 数据顶点类
    /// </summary>
    public class DataVertex : VertexBase
    {
        public string Name { get; set; }
        public string SpectralType { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        
        public DataVertex(string name)
        {
            Name = name;
        }
        
        public override string ToString()
        {
            return Name;
        }
    }
    
    /// <summary>
    /// 数据边类
    /// </summary>
    public class DataEdge : EdgeBase<DataVertex>
    {
        public new double Weight { get; set; }
        
        public DataEdge(DataVertex source, DataVertex target, double weight = 1)
            : base(source, target, weight)
        {
            Weight = weight;
        }
        
        public override string ToString()
        {
            return $"Edge: {Source} -> {Target}, Weight: {Weight:F2}";
        }
    }
}
