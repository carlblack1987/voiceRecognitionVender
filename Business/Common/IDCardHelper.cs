#region [ KIMMA Co.,Ltd. Copyright (C) 2014 ]
//-------------------------------------------------------------------------------------
// 开发单位：湖南金码智能设备制造有限公司
// 业务模块：iVend终端软件业务处理类
// 业务功能：二代身份证处理类
// 创建标识：2015-03-16		谷霖
//-------------------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IDCard;

namespace Business.Common
{
    public class IDCardHelper
    {
        #region 变量声明

        private IDCardOper m_IDCardOper = new IDCardOper();

        #endregion

        #region 属性

        /// <summary>
        /// 设备端口
        /// </summary>
        public string ComPort { get; set; }

        private IDCardInfo_User m_IDCard_UserInfo = new IDCardInfo_User();
        public IDCardInfo_User IDCard_UserInfo
        {
            get { return m_IDCard_UserInfo; }
            set { m_IDCard_UserInfo = value; }
        }

        #endregion

        #region 业务函数

        public int OpenIDCard()
        {
            int intErrCode = 0;

            intErrCode = m_IDCardOper.OpenIDCard(Convert.ToInt32(ComPort));

            return intErrCode;
        }

        public int CloseIDCard()
        {
            int intErrCode = 0;

            intErrCode = m_IDCardOper.CloseIDCard();
            return intErrCode;
        }

        /// <summary>
        /// 寻二代身份证
        /// </summary>
        /// <returns></returns>
        public int SelectIDCard()
        {
            int intErrCode = 0;

            intErrCode = m_IDCardOper.SelectIDCard();

            return intErrCode;
        }

        public int ReadIDCardInfo(out string _IDCardNum)
        {
            int intErrCode = 0;

            _IDCardNum = string.Empty;

            intErrCode = m_IDCardOper.ReadIDCardDetailInfo();
            if (intErrCode == 0)
            {
                _IDCardNum = m_IDCardOper.IDCard_Info.IDC;
                m_IDCard_UserInfo.IDC = m_IDCardOper.IDCard_Info.IDC;
                m_IDCard_UserInfo.Name = m_IDCardOper.IDCard_Info.Name;
                m_IDCard_UserInfo.Sex_Code = m_IDCardOper.IDCard_Info.Sex_Code;
                m_IDCard_UserInfo.NATION_Code = m_IDCardOper.IDCard_Info.NATION_Code;
                m_IDCard_UserInfo.ADDRESS = m_IDCardOper.IDCard_Info.ADDRESS;
                m_IDCard_UserInfo.BIRTH = m_IDCardOper.IDCard_Info.BIRTH;
            }

            return intErrCode;
        }

        /// <summary>
        /// 读取身份证详细信息
        /// </summary>
        /// <param name="_IDCardNum"></param>
        /// <returns></returns>
        public int ReadIDCardDetailInfo()
        {
            int intErrCode = 0;

            intErrCode = m_IDCardOper.ReadIDCardDetailInfo();
            if (intErrCode == 0)
            {
                m_IDCard_UserInfo.IDC = m_IDCardOper.IDCard_Info.IDC;
                m_IDCard_UserInfo.Name = m_IDCardOper.IDCard_Info.Name;
                m_IDCard_UserInfo.Sex_Code = m_IDCardOper.IDCard_Info.Sex_Code;
                m_IDCard_UserInfo.NATION_Code = m_IDCardOper.IDCard_Info.NATION_Code;
                m_IDCard_UserInfo.ADDRESS = m_IDCardOper.IDCard_Info.ADDRESS;
                m_IDCard_UserInfo.BIRTH = m_IDCardOper.IDCard_Info.BIRTH;
                m_IDCard_UserInfo.REGORG = m_IDCardOper.IDCard_Info.REGORG;
            }

            return intErrCode;
        }

        #endregion
    }

    /// <summary>
    /// 身份证信息类
    /// </summary>
    public class IDCardInfo_User
    {
        private string _Name;   //姓名
        private string _Sex_Code;   //性别代码
        private string _Sex_CName;   //性别
        private string _IDC;      //身份证号码
        private string _NATION_Code;   //民族代码
        private string _NATION_CName;   //民族
        private string _BIRTH;     //出生日期
        private string _ADDRESS;    //住址
        private string _REGORG;     //签发机关
        private string _STARTDATE;    //身份证有效起始日期
        private string _ENDDATE;    //身份证有效截至日期
        private string _Period_Of_Validity_Code;   //有效期限代码，许多原来系统上面为了一代证考虑，常常存在这样的字段，二代证中已经没有了
        private string _Period_Of_Validity_CName;   //有效期限

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        /// <summary>
        /// 性别代码
        /// </summary>
        public string Sex_Code
        {
            get { return _Sex_Code; }
            set
            {
                _Sex_Code = value;
                switch (value)
                {
                    case "1":
                        Sex_CName = "男";
                        break;
                    case "2":
                        Sex_CName = "女";
                        break;
                }
            }
        }

        /// <summary>
        /// 性别
        /// </summary>
        public string Sex_CName
        {
            get { return _Sex_CName; }
            set { _Sex_CName = value; }
        }

        /// <summary>
        /// 身份证号码
        /// </summary>
        public string IDC
        {
            get { return _IDC; }
            set { _IDC = value; }
        }

        /// <summary>
        /// 民族代码
        /// </summary>
        public string NATION_Code
        {
            get { return _NATION_Code; }
            set
            {
                _NATION_Code = value;
            }
        }

        /// <summary>
        /// 民族
        /// </summary>
        public string NATION_CName
        {
            get { return _NATION_CName; }
            set { _NATION_CName = value; }
        }

        /// <summary>
        /// 出生日期
        /// </summary>
        public string BIRTH
        {
            get { return _BIRTH; }
            set { _BIRTH = value; }
        }

        /// <summary>
        /// 住址
        /// </summary>
        public string ADDRESS
        {
            get { return _ADDRESS; }
            set { _ADDRESS = value; }
        }

        /// <summary>
        /// 签发机关
        /// </summary>
        public string REGORG
        {
            get { return _REGORG; }
            set { _REGORG = value; }
        }

        /// <summary>
        /// 身份证有效起始日期
        /// </summary>
        public string STARTDATE
        {
            get { return _STARTDATE; }
            set { _STARTDATE = value; }
        }

        /// <summary>
        /// 身份证有效截至日期
        /// </summary>
        public string ENDDATE
        {
            get { return _ENDDATE; }
            set
            {
                _ENDDATE = value;
            }
        }

        /// <summary>
        /// 有效期限
        /// </summary>
        public string Period_Of_Validity_CName
        {
            get { return _Period_Of_Validity_CName; }
            set { _Period_Of_Validity_CName = value; }
        }
    }
}
