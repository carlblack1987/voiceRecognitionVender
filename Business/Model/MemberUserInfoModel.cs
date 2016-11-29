#region [ KIMMA Co.,Ltd. Copyright (C) 2014 ]
//-------------------------------------------------------------------------------------
// 开发单位：湖南金码智能设备制造有限公司
// 业务模块：iVend终端软件业务处理类
// 业务功能：会员卡（磁条卡）信息模板
// 创建标识：2014-10-21		谷霖
//-------------------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Business.Model
{
    public class MemberUserInfoModel
    {
        //卡ID|外卡号|内卡号|余额|姓名|卡所属类型|积分
        /// <summary>
        /// 析构
        /// </summary>
        public MemberUserInfoModel()
        {
            Card_Id = string.Empty;
            CardNum_Out = string.Empty;
            CardNum_In = string.Empty;
            BanFee = "0";
            Name = string.Empty;
            CardType = string.Empty;
            Points = "0";
        }

        /// <summary>
        /// 卡ID
        /// </summary>
        public string Card_Id { get; set; }

        /// <summary>
        /// 外卡号
        /// </summary>
        public string CardNum_Out { get; set; }

        /// <summary>
        /// 内卡号
        /// </summary>
        public string CardNum_In { get; set; }

        /// <summary>
        /// 余额
        /// </summary>
        public string BanFee { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 卡所属类型
        /// </summary>
        public string CardType { get; set; }

        /// <summary>
        /// 积分
        /// </summary>
        public string Points { get; set; }
    }
}
