using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using ExcelDataReader;
using StellarisMapper.Models;

namespace StellarisMapper.Services
{
    /// <summary>
    /// Excel数据读取器实现
    /// </summary>
    public class ExcelDataReaderService : IDataReader
    {
        /// <summary>
        /// 从Excel文件读取星表数据
        /// </summary>
        /// <param name="filePath">Excel文件路径</param>
        /// <returns>星表记录列表</returns>
        public List<StarRecord> ReadStarData(string filePath)
        {
            var records = new List<StarRecord>();
            
            // 设置ExcelDataReader的配置
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    // 读取所有数据
                    var hasHeader = true;
                    var headers = new Dictionary<string, int>();
                    
                    // 读取标题行
                    if (hasHeader && reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            headers[reader.GetString(i)] = i;
                        }
                    }
                    
                    // 遍历数据行
                    while (reader.Read())
                    {
                        try
                        {
                            var record = new StarRecord();
                            
                            // 获取各列的值
                            if (headers.ContainsKey("NB_SYS"))
                                record.NB_SYS = Convert.ToInt32(reader.GetValue(headers["NB_SYS"]) ?? 0);
                            
                            if (headers.ContainsKey("OBJ_CAT"))
                                record.OBJ_CAT = reader.GetValue(headers["OBJ_CAT"])?.ToString() ?? string.Empty;
                            
                            if (headers.ContainsKey("OBJ_NAME"))
                                record.OBJ_NAME = reader.GetValue(headers["OBJ_NAME"])?.ToString() ?? string.Empty;
                            
                            if (headers.ContainsKey("SYSTEM_NAME"))
                                record.SYSTEM_NAME = reader.GetValue(headers["SYSTEM_NAME"])?.ToString() ?? string.Empty;
                            
                            if (headers.ContainsKey("RA"))
                                record.RA = Convert.ToDouble(reader.GetValue(headers["RA"]) ?? 0.0);
                            
                            if (headers.ContainsKey("DEC"))
                                record.DEC = Convert.ToDouble(reader.GetValue(headers["DEC"]) ?? 0.0);
                            
                            if (headers.ContainsKey("PARALLAX"))
                                record.PARALLAX = Convert.ToDouble(reader.GetValue(headers["PARALLAX"]) ?? 0.0);
                            
                            if (headers.ContainsKey("SP_TYPE"))
                                record.SP_TYPE = ExtractSpectralLetters(reader.GetValue(headers["SP_TYPE"])?.ToString() ?? string.Empty);
                            
                            records.Add(record);
                        }
                        catch (Exception ex)
                        {
                            // 跳过解析错误的行
                            Console.WriteLine($"Warning: Skipping row due to parsing error: {ex.Message}");
                            continue;
                        }
                    }
                }
            }
            
            return records;
        }
        
        /// <summary>
        /// 从光谱类型字符串中提取字母部分
        /// </summary>
        /// <param name="spType">光谱类型字符串</param>
        /// <returns>仅包含大写字母的部分</returns>
        private string ExtractSpectralLetters(string spType)
        {
            if (string.IsNullOrEmpty(spType))
                return string.Empty;
                
            // 使用正则表达式提取大写字母部分
            return Regex.Replace(spType, @"[^A-Z]", "");
        }
    }
}
