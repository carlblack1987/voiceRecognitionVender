using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Business;

namespace AutoSellGoodsMachineBusiness.ReadConfig
{
    /// <summary>
    /// 读取配置文件
    /// </summary>
    public class ReadProductConfig
    {
        ///// <summary>
        ///// 获取配置
        ///// </summary>
        ///// <param name="fileName"></param>
        ///// <returns></returns>
        //public List<PaInfo> GetConfig(string fileName)
        //{
        //    var machineInfos = new List<PaInfo>();

        //    var doc = new XmlDocument();

        //    doc.Load(fileName);

        //    var nodeList = doc.SelectSingleNode("McdInfos").ChildNodes;

        //    foreach (XmlNode node in nodeList)
        //    {
        //        try
        //        {
        //            var machineInfo = new PaInfo()
        //            {
        //                RowIndex = Convert.ToInt32(node["RowIndex"].InnerText),

        //                ColumnIndex = Convert.ToInt32(node["ColumnIndex"].InnerText),

        //                IsNew = Convert.ToBoolean(node["IsNew"].InnerText),

        //                SurNum = Convert.ToInt32(node["McdCount"].InnerText),

        //                McdPicName = (node["McdPicName"].InnerText),

        //                PaCode = (node["PaCode"].InnerText),

        //                Unit = (node["Unit"].InnerText),

        //                SellPrice = node["SellPrice"].InnerText//(float)Convert.ToDecimal(node["SellPrice"].InnerText),
        //            };
        //            machineInfos.Add(machineInfo);
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new Exception("读取配置错误在第" + machineInfos.Count + "节点的时候;" + ex.Message);
        //        }
        //    }

        //    return machineInfos;
        //}
    }
}
