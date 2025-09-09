using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace StellarisMapper.Models
{
    /// <summary>
    /// 表示一个恒星系统
    /// </summary>
    public class StellarSystem
    {
        /// <summary>
        /// 系统ID
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// 系统名称
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 对象类型
        /// </summary>
        [JsonProperty("obj_type")]
        public string ObjectType { get; set; } = string.Empty;

        /// <summary>
        /// 光谱类型
        /// </summary>
        [JsonProperty("sp_type")]
        public string SpectralType { get; set; } = string.Empty;

        /// <summary>
        /// X坐标（直角坐标系）
        /// </summary>
        [JsonProperty("x")]
        public double X { get; set; }

        /// <summary>
        /// Y坐标（直角坐标系）
        /// </summary>
        [JsonProperty("y")]
        public double Y { get; set; }

        /// <summary>
        /// Z坐标（直角坐标系）
        /// </summary>
        [JsonProperty("z")]
        public double Z { get; set; }

        /// <summary>
        /// 系统中的恒星列表（用于多星系统）
        /// </summary>
        [JsonProperty("stars")]
        public List<Star> Stars { get; set; } = new List<Star>();
    }

    /// <summary>
    /// 表示一颗恒星
    /// </summary>
    public class Star
    {
        /// <summary>
        /// 恒星名称
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 光谱类型
        /// </summary>
        [JsonProperty("sp_type")]
        public string SpectralType { get; set; } = string.Empty;
    }

    /// <summary>
    /// 表示两个恒星系统之间的连接
    /// </summary>
    public class Connection
    {
        /// <summary>
        /// 第一个系统ID
        /// </summary>
        [JsonProperty("system1_id")]
        public int System1Id { get; set; }

        /// <summary>
        /// 第二个系统ID
        /// </summary>
        [JsonProperty("system2_id")]
        public int System2Id { get; set; }

        /// <summary>
        /// 两个系统之间的距离（单位：秒差距）
        /// </summary>
        [JsonProperty("distance")]
        public double Distance { get; set; }
    }

    /// <summary>
    /// 表示完整的星图网络
    /// </summary>
    public class StellarNetwork
    {
        /// <summary>
        /// 跳跃距离限制（单位：秒差距）
        /// </summary>
        [JsonProperty("jump_distance")]
        public double JumpDistance { get; set; }

        /// <summary>
        /// 恒星系统列表
        /// </summary>
        [JsonProperty("systems")]
        public List<StellarSystem> Systems { get; set; } = new List<StellarSystem>();

        /// <summary>
        /// 系统间连接列表
        /// </summary>
        [JsonProperty("connections")]
        public List<Connection> Connections { get; set; } = new List<Connection>();
    }

    /// <summary>
    /// 表示星表中的原始数据行
    /// </summary>
    public class StarRecord
    {
        public int NB_SYS { get; set; }
        public string OBJ_CAT { get; set; } = string.Empty;
        public string OBJ_NAME { get; set; } = string.Empty;
        public string SYSTEM_NAME { get; set; } = string.Empty;
        public double RA { get; set; }
        public double DEC { get; set; }
        public double PARALLAX { get; set; }
        public string SP_TYPE { get; set; } = string.Empty;
    }
}
