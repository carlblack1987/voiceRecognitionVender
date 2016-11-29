#region [ KIMMA Co.,Ltd. Copyright (C) 2014 ]
//-------------------------------------------------------------------------------------
// 开发单位：湖南金码智能设备制造有限公司
// 业务模块：iVend终端软件业务处理类
// 业务功能：升降机上下移动码盘参数类
// 创建标识：2015-06-02		谷霖
//-------------------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Business.Model
{
    public class UpDownNumsModel
    {
        /// <summary>
        /// 析构
        /// </summary>
        public UpDownNumsModel()
        {
            VendBoxCode = "1";
            TrayNum = 0;
            UpDownCodeNums = 0;
        }

        /// <summary>
        /// 所属机柜，默认机柜1
        /// </summary>
        public string VendBoxCode { get; set; }

        /// <summary>
        /// 托盘编号
        /// </summary>
        public int TrayNum { get; set; }

        /// <summary>
        /// 上下移动码盘数值
        /// </summary>
        public int UpDownCodeNums { get; set; }
    }
}
