#region [ KIMMA Co.,Ltd. Copyright (C) 2014 ]
//-------------------------------------------------------------------------------------
// 开发单位：湖南金码智能设备制造有限公司
// 业务模块：iVend终端软件业务处理类
// 业务功能：卡信息模板
// 创建标识：2014-09-04		谷霖
//-------------------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Business.Model
{
    public class CardInfoModel
    {
        /// <summary>
        /// 析构
        /// </summary>
        public CardInfoModel()
        {
            CardNum_Show = string.Empty;
            BanFee = "0";
            CardNum_Upload = string.Empty;
        }

        /// <summary>
        /// 要在界面上显示的卡号
        /// </summary>
        public string CardNum_Show { get; set; }

        /// <summary>
        /// 余额
        /// </summary>
        public string BanFee { get; set; }

        /// <summary>
        /// 要上传的卡号
        /// </summary>
        public string CardNum_Upload { get; set; }
    }
}
