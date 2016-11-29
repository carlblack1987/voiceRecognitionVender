#region [ KIMMA Co.,Ltd. Copyright (C) 2014 ]
//-------------------------------------------------------------------------------------
// 开发单位：湖南金码智能设备制造有限公司
// 业务模块：iVend终端软件业务处理类
// 业务功能：非储值会员卡处理类
// 创建标识：2014-10-20		谷霖
//-------------------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NoFeeCard;
using Business.Model;

namespace Business.Common
{
    public class NoFeeCardHelper
    {
        #region 变量

        /// <summary>
        /// 磁条卡处理类对象—非储值卡
        /// </summary>
        private CardBusi m_NoFeeCardBusi = new CardBusi();

        /// <summary>
        /// 会员信息对象
        /// </summary>
        public MemberUserInfoModel MemberUserInfo = new MemberUserInfoModel();

        /// <summary>
        /// 磁条卡处理参数
        /// </summary>
        public NoFeeCardCfg NoFeeCardConfig = new NoFeeCardCfg();

        #endregion

        #region 接口函数

        public string SoftName()
        {
            return m_NoFeeCardBusi.SoftName;
        }

        public void IsLogToFile(bool isLogToFile)
        {
            m_NoFeeCardBusi.IsLogToFile = isLogToFile;
        }

        public int CheckNoFeeCardDevice()
        {
            return m_NoFeeCardBusi.CheckNoFeeCardDevice();
        }

        /// <summary>
        /// 打开非储值卡设备
        /// </summary>
        /// <returns></returns>
        public int InitNoFeeCardDevice()
        {
            int intErrCode = 99;

            try
            {
                m_NoFeeCardBusi.ThirdWebUrl = NoFeeCardConfig.WebUrl;
                m_NoFeeCardBusi.TermCode = NoFeeCardConfig.TermCode;
                intErrCode = m_NoFeeCardBusi.Initialize(NoFeeCardConfig.Port);
            }
            catch
            {
                intErrCode = 99;
            }

            return intErrCode;
        }

        /// <summary>
        /// 查询非储值卡数据（磁条卡数据等）
        /// </summary>
        /// <param name="_cardData"></param>
        /// <returns></returns>
        public int QueryNoFeeCardData(out string _cardData)
        {
            _cardData = string.Empty;
            string _icCode = string.Empty;
            int intErrCode = m_NoFeeCardBusi.QueryCardNum(out _cardData, out _icCode);
            return intErrCode;
        }

        /// <summary>
        /// 清除磁条卡数据
        /// </summary>
        /// <returns></returns>
        public int ClearNoFeeCardData()
        {
            return m_NoFeeCardBusi.ClearCardNum();
        }

        /// <summary>
        /// 查询会员卡信息
        /// </summary>
        /// <param name="cardNum">会员卡号</param>
        /// <param name="money">要扣款金额</param>
        /// <returns>错误代码</returns>
        public int QueryCardInfo(string cardNum,int money)
        {
            int intErrCode = 0;

            string _cardData = string.Empty;
            try
            {
                #region 暂停磁条卡设备

                m_NoFeeCardBusi.StopNoFeeCard();

                #endregion

                intErrCode = m_NoFeeCardBusi.QueryCardInfo(cardNum, out _cardData);
                if (intErrCode == 0)
                {
                    // 解析会员卡用户数据
                    // 卡ID|外卡号|内卡号|余额|姓名|卡所属类型|积分
                    string[] hexUserInfo = _cardData.Split('|');
                    if (hexUserInfo.Length > 5)
                    {
                        MemberUserInfo.Card_Id = hexUserInfo[0];
                        MemberUserInfo.CardNum_Out = hexUserInfo[1];
                        MemberUserInfo.CardNum_In = hexUserInfo[2];
                        MemberUserInfo.BanFee = hexUserInfo[3];
                        MemberUserInfo.Name = hexUserInfo[4];
                        MemberUserInfo.CardType = hexUserInfo[5];
                        MemberUserInfo.Points = hexUserInfo[6];

                        // 检测余额是否足够
                        if (Convert.ToInt32(MemberUserInfo.BanFee) < money)
                        {
                            // 余额不足
                            intErrCode = 12;// 余额不足，无法消费
                        }
                    }
                    else
                    {
                        intErrCode = 99;
                    }
                }
            }
            catch
            {
                intErrCode = 99;
            }
            finally
            {
                #region 如果查询失败，则开启磁条卡设备

                if (intErrCode != 0)
                {
                    m_NoFeeCardBusi.BegingNoFeeCard();
                }

                #endregion
            }

            return intErrCode;
        }

        /// <summary>
        /// 提交非储值卡或虚拟会员卡在线支付
        /// </summary>
        /// <param name="cardNum"></param>
        /// <param name="money"></param>
        /// <param name="serNo"></param>
        /// <param name="isRebate"></param>
        /// <param name="factMoeny">实际扣费金额</param>
        /// <returns>错误代码</returns>
        public int DecuctCardMoney(string cardNum, int money, string serNo, string isRebate, out int factMoeny)
        {
            int intErrCode = 0;

            factMoeny = 0;
            string _cardData = string.Empty;

            try
            {
                #region 暂停磁条卡设备

                m_NoFeeCardBusi.StopNoFeeCard();

                #endregion

                intErrCode = m_NoFeeCardBusi.DecuctCardMoney(cardNum, money, serNo, isRebate, out _cardData);
                if (intErrCode == 0)
                {
                    // 解析数据
                    // 卡ID|外卡号|内卡号|实际扣费金额|余额|姓名|卡所属类型|积分
                    string[] hexUserInfo = _cardData.Split('|');
                    if (hexUserInfo.Length > 6)
                    {
                        MemberUserInfo.Card_Id = hexUserInfo[0];
                        MemberUserInfo.CardNum_Out = hexUserInfo[1];
                        MemberUserInfo.CardNum_In = hexUserInfo[2];
                        factMoeny = Convert.ToInt32(hexUserInfo[3]);
                        MemberUserInfo.BanFee = hexUserInfo[4];
                        MemberUserInfo.Name = hexUserInfo[5];
                        MemberUserInfo.CardType = hexUserInfo[6];
                        MemberUserInfo.Points = hexUserInfo[7];
                    }
                    else
                    {
                        intErrCode = 99;
                    }
                }
            }
            catch
            {
                intErrCode = 99;
            }
            finally
            {
                #region 扣款失败，开启磁条卡设备

                if (intErrCode != 0)
                {
                    m_NoFeeCardBusi.BegingNoFeeCard();
                }

                #endregion
            }
            return intErrCode;
        }

        #endregion
    }

    public class NoFeeCardCfg
    {
        private int m_Port = 1;
        public int Port
        {
            get { return m_Port; }
            set { m_Port = value; }
        }

        private string m_TermCode = string.Empty;
        public string TermCode
        {
            get { return m_TermCode; }
            set { m_TermCode = value; }
        }

        private string m_WebUrl = string.Empty;
        public string WebUrl
        {
            get { return m_WebUrl; }
            set { m_WebUrl = value; }
        }
    }
}
