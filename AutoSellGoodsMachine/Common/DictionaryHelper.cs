#region [ KIMMA Co.,Ltd. Copyright (C) 2013 ]
//-------------------------------------------------------------------------------------
// 开发单位：湖南金码智能设备制造有限公司
// 业务模块：iVend终端软件公共处理类
// 业务功能：iVend终端软件字典转换处理类
// 创建标识：2013-06-10		谷霖
//-------------------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Business.Enum;

namespace AutoSellGoodsMachine
{
    public class DictionaryHelper
    {
        /// <summary>
        /// 根据购买商品环境检测代码获取检测提示语
        /// </summary>
        /// <param name="saleEnvirCode">购买商品环境监测代码</param>
        /// <returns>检测提示语</returns>
        public static string Dictionary_SaleEnvirCode(BusinessEnum.ServerReason saleEnvirCode)
        {
            string strCodeName = string.Empty;

            switch (saleEnvirCode)
            {
                case BusinessEnum.ServerReason.Normal:// 正常
                    break;

                case BusinessEnum.ServerReason.Err_NoAsile:// 货道不存在
                    strCodeName = "Err_SellGoods_PaErr_NoExist";
                    break;
                case BusinessEnum.ServerReason.Err_AsilePause:// 货道暂停销售
                    strCodeName = "Err_SellGoods_PaErr_Kind";
                    break;
                case BusinessEnum.ServerReason.Err_NoStock:// 库存不足
                    strCodeName = "Err_SellGoods_PaErr_NoStock";
                    break;
                case BusinessEnum.ServerReason.Err_AsileStatus:// 货道故障
                    strCodeName = "Err_SellGoods_PaErr_Status";
                    break;
                case BusinessEnum.ServerReason.Err_Cash:// 纸币器/硬币器故障
                    strCodeName = "Err_PauseServer_Reason_Cash";
                    break;
                case BusinessEnum.ServerReason.Err_IC:// 刷卡器故障
                    strCodeName = "Err_PauseServer_Reason_IC";
                    break;
                #region 升降系统故障
                /// 01：升降机位置不在初始位
                /// 02：纵向电机卡塞
                /// 03：接货台不在初始位
                /// 04：横向电机卡塞
                /// 05：小门电机卡塞
                /// 06：接货台有货
                /// 07：接货台电机卡塞
                /// 08：取货口有货
                /// 09：其它故障 
                case BusinessEnum.ServerReason.Err_UpDown_Other:// 其它故障
                    strCodeName = "Err_UpDown_Status_Other";
                    break;
                case BusinessEnum.ServerReason.Err_UpDown_VertMotor:// 升降机位置不在初始位纵向电机卡塞
                    strCodeName = "Err_UpDown_Status_VertMotor";
                    break;
                case BusinessEnum.ServerReason.Err_UpDown_HoriMotor:// 接货台不在初始位横向电机卡塞
                    strCodeName = "Err_UpDown_Status_HoriMotor";
                    break;
                case BusinessEnum.ServerReason.Err_UpDown_DoorMotor:// 小门电机卡塞
                    strCodeName = "Err_UpDown_Status_DoorMotor";
                    break;
                case BusinessEnum.ServerReason.Err_UpDown_JieHuo:// 接货台有货
                    strCodeName = "Err_UpDown_Status_JieHuo";
                    break;
                case BusinessEnum.ServerReason.Err_UpDown_JieHuoMotor:// 接货台电机卡塞
                    strCodeName = "Err_UpDown_Status_JieHuoMotor";
                    break;

                #endregion
                case BusinessEnum.ServerReason.Err_GuangDian:// 光电管故障
                    strCodeName = "Err_UpDown_Status_GuangDian";
                    break;
                case BusinessEnum.ServerReason.Err_GoodsExist:// 取货仓有货
                    strCodeName = "Err_UpDown_Status_NoTake";
                    break;
                case BusinessEnum.ServerReason.Err_TmpLimit:// 温度超限
                    strCodeName = "Err_PauseServer_Reason_Tmp";
                    break;
                case BusinessEnum.ServerReason.Err_NoPayment:// 没有开通任何支付方式
                    strCodeName = "Err_PauseServer_NoPayment";
                    break;
                case BusinessEnum.ServerReason.Err_NoFeeCard:// 不能会员卡支付
                    strCodeName = "Err_PauseServer_Reason_NoFeeCard";
                    break;
                case BusinessEnum.ServerReason.Err_QrCode:// 不能二维码支付
                    strCodeName = "Err_PauseServer_Reason_QrCode";
                    break;
                case BusinessEnum.ServerReason.Err_DiskSpaceNoEnougth:// 磁盘空间不足
                    strCodeName = "Err_PauseServer_Reason_DiskSpaceNoEnougth";
                    break;
                case BusinessEnum.ServerReason.Err_IDCard:// 二代身份证设备故障
                    strCodeName = "Err_PauseServer_Reason_IDCard";
                    break;
                default:// 其它故障
                    strCodeName = "Err_SellGoods_PaErr_Status";
                    break;
            }

            strCodeName = PubHelper.p_LangOper.GetStringBundle(strCodeName);

            return strCodeName;
        }

        /// <summary>
        /// 根据储值卡刷卡扣款结果代码获取提示语
        /// </summary>
        /// <param name="postCardCode">刷卡扣款结果代码</param>
        /// <returns>提示语</returns>
        public static string Dictionary_PostCardCode(int postCardCode)
        {
            string strCodeName = string.Empty;

            switch (postCardCode)
            {
                case 0:
                    break;

                case 10:// 无卡
                    break;

                case 11:// 无效卡或黑名单卡
                    strCodeName = PubHelper.p_LangOper.GetStringBundle("PosCard_Err_BlackCard");
                    break;

                case 12:// 余额不足
                    strCodeName = PubHelper.p_LangOper.GetStringBundle("PosCard_Err_NoEnoughFee");
                    break;

                case 18:// 卡状态异常
                    strCodeName = PubHelper.p_LangOper.GetStringBundle("PosCard_Err_InvalidCard");
                    break;

                default:// 认为刷卡失败
                    strCodeName = PubHelper.p_LangOper.GetStringBundle("PosCard_Err_Pay");
                    break;
            }

            return strCodeName;
        }

        /// <summary>
        /// 根据会员卡（非储值卡）刷卡扣款结果代码获取提示语
        /// </summary>
        /// <param name="postCardCode">刷卡扣款结果代码</param>
        /// <returns>提示语</returns>
        public static string Dictionary_NoFeeCardCode(int cardCode)
        {
            string strCodeName = string.Empty;

            switch (cardCode)
            {
                case 0:
                    break;

                case 8:// 调用Web Service失败
                    strCodeName = PubHelper.p_LangOper.GetStringBundle("NoFeeCard_Err_Net");
                    break;

                case 10:// 不存在会员卡信息
                    strCodeName = PubHelper.p_LangOper.GetStringBundle("NoFeeCard_Err_NoCard");
                    break;

                case 11:// 黑名单会员
                    strCodeName = PubHelper.p_LangOper.GetStringBundle("NoFeeCard_Err_BlackCard");
                    break;

                case 13:// 会员已停用
                    strCodeName = PubHelper.p_LangOper.GetStringBundle("NoFeeCard_Err_BlackCard");
                    break;

                case 14:// 会员已挂失
                    strCodeName = PubHelper.p_LangOper.GetStringBundle("NoFeeCard_Err_BlackCard");
                    break;

                case 15:// 会员未激活
                    strCodeName = PubHelper.p_LangOper.GetStringBundle("NoFeeCard_Err_InvalidCard");
                    break;

                case 12:// 余额不足
                    strCodeName = PubHelper.p_LangOper.GetStringBundle("NoFeeCard_Err_NoEnoughFee");
                    break;
                case 17:// 该卡不能再售货机上消费
                    strCodeName = PubHelper.p_LangOper.GetStringBundle("NoFeeCard_Err_LimitVm");
                    break;

                default:// 认为刷卡失败
                    strCodeName = PubHelper.p_LangOper.GetStringBundle("NoFeeCard_Err_Pay");
                    break;
            }

            return strCodeName;
        }

        /// <summary>
        /// 根据支付宝付款码结果获取相关提示语
        /// </summary>
        /// <param name="resultCode"></param>
        /// <returns></returns>
        public static string Dictionary_AliPayCode(int resultCode)
        {
            string strCodeName = string.Empty;

            switch (resultCode)
            {
                case 3:// 等待支付宝付款结果超时
                    strCodeName = PubHelper.p_LangOper.GetStringBundle("Err_AliPay_TimeOut");
                    break;
                default:
                    strCodeName = PubHelper.p_LangOper.GetStringBundle("Err_AliPay_PayFail");
                    break;
            }

            return strCodeName;
        }

        /// <summary>
        /// 根据志愿者兑换结果获取相关提示语
        /// </summary>
        /// <param name="resultCode"></param>
        /// <returns></returns>
        public static string Dictionary_Volunteer(int resultCode)
        {
            string strCodeName = string.Empty;

            switch (resultCode)
            {
                case 1012:// 智能兑换3元以下商品
                    strCodeName = "只能兑换3元及以下价格商品";
                    break;
                case 1013:// 一天只能兑换两次
                    strCodeName = "一天只能兑换两次";
                    break;
                case 1002:// 没有达到兑换条件
                    strCodeName = "您的积分不足以兑换";
                    break;
                case 1001:// 志愿者信息不存在
                    strCodeName = "该志愿者信息不存在";
                    break;
                default:
                    strCodeName = PubHelper.p_LangOper.GetStringBundle("Err_AliPay_PayFail");
                    break;
            }

            return strCodeName;
        }

        /// <summary>
        /// 根据二代身份证结果代码获取提示语
        /// </summary>
        /// <param name="postCardCode">二代身份证结果代码</param>
        /// <returns>提示语</returns>
        public static string Dictionary_IDCardCode(int resultCode)
        {
            string strCodeName = string.Empty;

            switch (resultCode)
            {
                case 0:
                    break;

                case 3:// 调用Web Service失败
                    strCodeName = PubHelper.p_LangOper.GetStringBundle("FreeSell_IDCard_Err_Query");
                    break;

                case 10:// 领取已满
                    strCodeName = PubHelper.p_LangOper.GetStringBundle("FreeSell_IDCard_Err_Taked");
                    break;
                
                case 999:// 认为读身份证信息失败
                    strCodeName = PubHelper.p_LangOper.GetStringBundle("FreeSell_IDCard_Err_Read");
                    break;
                default:// 认为操作失败
                    strCodeName = PubHelper.p_LangOper.GetStringBundle("FreeSell_IDCard_Err_Query");
                    break;
            }

            return strCodeName;
        }

        /// <summary>
        /// 线下扫码取货—根据条形码订单结果代码获取提示语
        /// </summary>
        /// <param name="postCardCode">结果代码</param>
        /// <returns>提示语</returns>
        public static string Dictionary_TakeBarCode(int resultCode,string takeCodeName)
        {
            string strCodeName = string.Empty;

            switch (resultCode)
            {
                case 0:
                    break;

                case 3:// 调用Web Service失败
                    strCodeName = PubHelper.p_LangOper.GetStringBundle("TakeSell_BarCode_Err_Query");
                    break;

                case 5:// 订单信息不存在
                    strCodeName = PubHelper.p_LangOper.GetStringBundle("TakeSell_BarCode_Err_NoOrder");
                    break;

                case 7:// 订单没有商品
                    strCodeName = PubHelper.p_LangOper.GetStringBundle("TakeSell_BarCode_Err_NoGoods");
                    break;

                case 8:// 该订单已经出货
                case 10:// 已经领取过商品
                    strCodeName = PubHelper.p_LangOper.GetStringBundle("TakeSell_BarCode_Err_Taked");
                    break;

                case 6:// 该订单状态异常
                case 9:// 该订单无效
                    strCodeName = PubHelper.p_LangOper.GetStringBundle("TakeSell_BarCode_Err_InValid");
                    break;

                case 11:// 不能再该机器上取
                    strCodeName = PubHelper.p_LangOper.GetStringBundle("TakeSell_BarCode_Err_NoTakeInVm");
                    break;

                case 999:// 认为读取出来的订单信息和后台返回的不一致
                    strCodeName = PubHelper.p_LangOper.GetStringBundle("TakeSell_BarCode_Err_NoSameOrder");
                    break;
                default:// 认为操作失败
                    strCodeName = PubHelper.p_LangOper.GetStringBundle("TakeSell_BarCode_Err_Query");
                    break;
            }

            if (resultCode != 0)
            {
                strCodeName = strCodeName.Replace("{N}", takeCodeName);
            }

            return strCodeName;
        }

        /// <summary>
        /// 根据取货码结果代码获取提示语
        /// </summary>
        /// <param name="postCardCode">结果代码</param>
        /// <returns>提示语</returns>
        public static string Dictionary_WxTakeCode(int resultCode,string takeCodeName)
        {
            string strCodeName = string.Empty;

            switch (resultCode)
            {
                case 0:
                    break;

                case 3:// 调用Web Service失败
                    strCodeName = PubHelper.p_LangOper.GetStringBundle("TakeSell_BarCode_Err_Query");
                    break;

                case 5:// 订单信息不存在
                    strCodeName = PubHelper.p_LangOper.GetStringBundle("TakeSell_BarCode_Err_NoOrder");
                    break;

                case 7:// 订单没有商品
                    strCodeName = PubHelper.p_LangOper.GetStringBundle("TakeSell_BarCode_Err_NoGoods");
                    break;

                case 8:// 该订单已经出货
                    strCodeName = PubHelper.p_LangOper.GetStringBundle("TakeSell_BarCode_Err_Taked");
                    break;

                case 9:// 该订单无效
                    strCodeName = PubHelper.p_LangOper.GetStringBundle("TakeSell_BarCode_Err_InValid");
                    break;

                case 999:// 认为读取出来的订单信息和后台返回的不一致
                    strCodeName = PubHelper.p_LangOper.GetStringBundle("TakeSell_BarCode_Err_NoSameOrder");
                    break;
                default:// 认为操作失败
                    strCodeName = PubHelper.p_LangOper.GetStringBundle("TakeSell_BarCode_Err_Query");
                    break;
            }

            if (resultCode != 0)
            {
                strCodeName = strCodeName.Replace("{N}", takeCodeName);
            }

            return strCodeName;
        }

        /// <summary>
        /// 根据输入转换输入后的字符
        /// </summary>
        /// <param name="intPut">输入的字符</param>
        /// <returns>转换后的字符</returns>
        public static string Dictionary_Input(string intPut)
        {
            string strInput = string.Empty;

            if (string.IsNullOrEmpty(intPut))
            {
                return strInput;
            }

            strInput = intPut.Replace("System.Windows.Controls.Button: ","");

            switch (intPut)
            {
                case "":
                    break;
            }

            return strInput;
        }

        /// <summary>
        /// 门状态
        /// </summary>
        /// <returns></returns>
        public static string Dictionary_Door(int vendBoxIndex)
        {
            string strDoorStatus = string.Empty;

            switch (PubHelper.p_BusinOper.AsileOper.VendBoxList[vendBoxIndex].DoorStatus)
            {
                case "00":// 门关
                    strDoorStatus = PubHelper.p_LangOper.GetStringBundle("Pub_Door_Close");
                    break;
                case "01":// 门开
                    strDoorStatus = PubHelper.p_LangOper.GetStringBundle("Pub_Door_Open");
                    break;
                default:// 故障
                    strDoorStatus = PubHelper.p_LangOper.GetStringBundle("Pub_Door_Connect");
                    break;
            }

            return strDoorStatus;
        }

        /// <summary>
        /// 当前温度
        /// </summary>
        /// <param name="isShowStatus">温度检测不正常时，是否显示状态 True：显示 False：不显示</param>
        /// <returns></returns>
        public static string Dictionary_NowTmp(int vendBoxIndex,bool isShowStatus)
        {
            string strNowTmp = string.Empty;

            try
            {
                if (PubHelper.p_BusinOper.AsileOper.VendBoxList[vendBoxIndex].TmpStatus == "00")
                {
                    int intTmpValue = Convert.ToInt32(PubHelper.p_BusinOper.AsileOper.VendBoxList[vendBoxIndex].TmpValue);
                    BusinessEnum.TmpControlModel tmpControlModel = PubHelper.p_BusinOper.AsileOper.VendBoxList[vendBoxIndex].TmpControlModel;
                    bool blnIsShowValue = false;
                    // 温度检测正常
                    if (tmpControlModel == BusinessEnum.TmpControlModel.Refrigeration)
                    {
                        // 如果是制冷
                        if ((intTmpValue >= 0) && (intTmpValue <= 60))
                        {
                            blnIsShowValue = true;
                        }
                    }
                    if (tmpControlModel == BusinessEnum.TmpControlModel.Heating)
                    {
                        // 如果是加热
                        if ((intTmpValue >= 0) && (intTmpValue < 100))
                        {
                            blnIsShowValue = true;
                        }
                    }
                    if (blnIsShowValue)
                    {
                        strNowTmp = intTmpValue.ToString() + PubHelper.TMP_UNIT;
                    }
                }
                else
                {
                    if (isShowStatus)
                    {
                        switch (PubHelper.p_BusinOper.AsileOper.VendBoxList[vendBoxIndex].TmpStatus)
                        {
                            case "01"://
                                strNowTmp = PubHelper.p_LangOper.GetStringBundle("Pub_Device_Tmp_Out");// "检测超限";
                                break;

                            case "02"://
                                strNowTmp = PubHelper.p_LangOper.GetStringBundle("Pub_Device_Tmp_Connect");// "未连接";
                                break;
                        }
                    }
                }
            }
            catch
            {

            }

            return strNowTmp;
        }

        /// <summary>
        /// 根据货柜显示柜子的温控类型名称
        /// </summary>
        /// <param name="vendBoxIndex"></param>
        /// <returns></returns>
        public static string Dictionary_TmpType(int vendBoxIndex)
        {
            string strTmpType = string.Empty;
            try
            {
                switch (PubHelper.p_BusinOper.AsileOper.VendBoxList[vendBoxIndex].TmpControlModel)
                {
                    case BusinessEnum.TmpControlModel.Heating:// 加热
                        strTmpType = PubHelper.p_LangOper.GetStringBundle("Pub_TmpType_Heating");
                        break;
                    case BusinessEnum.TmpControlModel.Normal:// 普通
                        strTmpType = PubHelper.p_LangOper.GetStringBundle("Pub_TmpType_Normal");
                        break;
                    case BusinessEnum.TmpControlModel.Refrigeration:// 制冷
                        strTmpType = PubHelper.p_LangOper.GetStringBundle("Pub_TmpType_Refrigeration");
                        break;
                }
            }
            catch
            {

            }
            return strTmpType;
        }

        /// <summary>
        /// 网络连接状态
        /// </summary>
        /// <returns></returns>
        public static string Dictionary_NetStatus()
        {
            string strNetStatus = string.Empty;

            switch (PubHelper.p_BusinOper.DeviceInfo.NetStatus)
            {
                case BusinessEnum.NetStatus.OffLine:// 离线
                    strNetStatus = PubHelper.p_LangOper.GetStringBundle("Pub_Net_OffLine"); //"离线";
                    break;

                case BusinessEnum.NetStatus.OnLine:// 联机
                    strNetStatus = PubHelper.p_LangOper.GetStringBundle("Pub_Net_OnLine");//"联机";
                    break;

                default:// 不存在网络设备
                    strNetStatus = PubHelper.p_LangOper.GetStringBundle("Pub_Net_NoNet");//"无通讯功能";
                    break;
            }

            return strNetStatus;
        }

        /// <summary>
        /// 纸币器状态
        /// </summary>
        /// <returns></returns>
        public static string Dictionary_CashStatus()
        {
            string strStatusInfo = string.Empty;

            if (PubHelper.p_BusinOper.PaymentOper.PaymentList.Cash.ControlSwitch == Business.Enum.BusinessEnum.ControlSwitch.Run)
            {
                string strCashStatus = PubHelper.p_BusinOper.DeviceInfo.CashStatus.Status;
                switch (strCashStatus)
                {
                    case "00":
                        strStatusInfo = PubHelper.p_LangOper.GetStringBundle("Pub_Device_Close");
                        break;

                    case "02":// 正常
                        strStatusInfo = PubHelper.p_LangOper.GetStringBundle("Pub_Normal");
                        break;

                    case "FF":// 无连接
                        strStatusInfo = PubHelper.p_LangOper.GetStringBundle("Pub_FF");
                        break;
                    default:// 故障
                        strStatusInfo = PubHelper.p_LangOper.GetStringBundle("Pub_Error");
                        break;
                }
                if (strCashStatus != "02")
                {
                    strStatusInfo += "【" + strCashStatus + "】";
                }
            }
            else
            {
                // 纸币器被关闭
                strStatusInfo = PubHelper.p_LangOper.GetStringBundle("Pub_Device_Close");
            }

            return strStatusInfo;
        }

        /// <summary>
        /// 硬币器状态
        /// </summary>
        /// <returns></returns>
        public static string Dictionary_CoinStatus()
        {
            string strStatusInfo = string.Empty;

            if (PubHelper.p_BusinOper.PaymentOper.PaymentList.Cash.ControlSwitch == Business.Enum.BusinessEnum.ControlSwitch.Run)
            {
                string strCoinStatus = PubHelper.p_BusinOper.DeviceInfo.CoinStatus.Status;
                switch (strCoinStatus)
                {
                    case "00":
                        strStatusInfo = PubHelper.p_LangOper.GetStringBundle("Pub_Device_Close");
                        break;

                    case "02":// 正常
                        strStatusInfo = PubHelper.p_LangOper.GetStringBundle("Pub_Normal");
                        break;

                    case "FF":// 无连接
                        strStatusInfo = PubHelper.p_LangOper.GetStringBundle("Pub_FF");
                        break;
                    default:// 故障
                        strStatusInfo = PubHelper.p_LangOper.GetStringBundle("Pub_Error");
                        break;
                }
                if (strCoinStatus != "02")
                {
                    strStatusInfo += "【" + strCoinStatus + "】";
                }
            }
            else
            {
                // 硬币器被关闭
                strStatusInfo = PubHelper.p_LangOper.GetStringBundle("Pub_Device_Close");
            }

            return strStatusInfo;
        }

        /// <summary>
        /// 刷卡器状态
        /// </summary>
        /// <returns></returns>
        public static string Dictionary_ICStatus()
        {
            string strICStatus = string.Empty;

            if (PubHelper.p_BusinOper.PaymentOper.PaymentList.IC.ControlSwitch == Business.Enum.BusinessEnum.ControlSwitch.Run)
            {
                switch (PubHelper.p_BusinOper.DeviceInfo.ICStatus.Status)
                {
                    case "00":
                        strICStatus = PubHelper.p_LangOper.GetStringBundle("Pub_Device_Close");
                        break;

                    case "02":// 正常
                        strICStatus = PubHelper.p_LangOper.GetStringBundle("Pub_Normal");
                        break;

                    case "FF":// 无连接
                        strICStatus = PubHelper.p_LangOper.GetStringBundle("Pub_FF");
                        break;
                    default:// 故障
                        strICStatus = PubHelper.p_LangOper.GetStringBundle("Pub_Error");
                        break;
                }
            }
            else
            {
                // 刷卡器被关闭
                strICStatus = PubHelper.p_LangOper.GetStringBundle("Pub_Device_Close");
            }

            return strICStatus;
        }

        /// <summary>
        /// 磁条卡设备状态
        /// </summary>
        /// <returns></returns>
        public static string Dictionary_NoFeeCardStatus()
        {
            string strStatus = string.Empty;

            if (PubHelper.p_BusinOper.PaymentOper.PaymentList.NoFeeCard.ControlSwitch == Business.Enum.BusinessEnum.ControlSwitch.Run)
            {
                switch (PubHelper.p_BusinOper.DeviceInfo.NoFeeCardStatus.Status)
                {
                    case "00":
                        strStatus = PubHelper.p_LangOper.GetStringBundle("Pub_Device_Close");
                        break;

                    case "02":// 正常
                        strStatus = PubHelper.p_LangOper.GetStringBundle("Pub_Normal");
                        break;

                    case "FF":// 无连接
                        strStatus = PubHelper.p_LangOper.GetStringBundle("Pub_FF");
                        break;
                    default:// 故障
                        strStatus = PubHelper.p_LangOper.GetStringBundle("Pub_Error");
                        break;
                }
            }
            else
            {
                // 被关闭
                strStatus = PubHelper.p_LangOper.GetStringBundle("Pub_Device_Close");
            }

            return strStatus;
        }

        /// <summary>
        /// 银联卡设备状态
        /// </summary>
        /// <returns></returns>
        public static string Dictionary_UnionPayStatus()
        {
            string strStatus = string.Empty;

            if (PubHelper.p_BusinOper.PaymentOper.PaymentList.UnionPay.ControlSwitch == Business.Enum.BusinessEnum.ControlSwitch.Run)
            {
                switch (PubHelper.p_BusinOper.DeviceInfo.UnionPayStatus)
                {
                    case "00":
                        strStatus = PubHelper.p_LangOper.GetStringBundle("Pub_Device_Close");
                        break;

                    case "02":// 正常
                        strStatus = PubHelper.p_LangOper.GetStringBundle("Pub_Normal");
                        break;

                    case "FF":// 无连接
                        strStatus = PubHelper.p_LangOper.GetStringBundle("Pub_FF");
                        break;
                    default:// 故障
                        strStatus = PubHelper.p_LangOper.GetStringBundle("Pub_Error");
                        break;
                }
            }
            else
            {
                // 被关闭
                strStatus = PubHelper.p_LangOper.GetStringBundle("Pub_Device_Close");
            }

            return strStatus;
        }

        /// <summary>
        /// 二维码设备状态
        /// </summary>
        /// <returns></returns>
        public static string Dictionary_QRCodeStatus()
        {
            string strStatus = string.Empty;

            if (PubHelper.p_BusinOper.PaymentOper.PaymentList.QRCodeCard.ControlSwitch == Business.Enum.BusinessEnum.ControlSwitch.Run)
            {
                switch (PubHelper.p_BusinOper.DeviceInfo.QrDeviceStatus)
                {
                    case "00":
                        strStatus = PubHelper.p_LangOper.GetStringBundle("Pub_Device_Close");
                        break;

                    case "02":// 正常
                        strStatus = PubHelper.p_LangOper.GetStringBundle("Pub_Normal");
                        break;

                    case "FF":// 无连接
                        strStatus = PubHelper.p_LangOper.GetStringBundle("Pub_FF");
                        break;
                    default:// 故障
                        strStatus = PubHelper.p_LangOper.GetStringBundle("Pub_Error");
                        break;
                }
            }
            else
            {
                // 被关闭
                strStatus = PubHelper.p_LangOper.GetStringBundle("Pub_Device_Close");
            }

            return strStatus;
        }

        /// <summary>
        /// 压缩机/照明灯/广告灯/除雾设备等工作状态
        /// </summary>
        /// <param name="deviceStatus"></param>
        /// <returns></returns>
        public static string Dictionary_DeviceStatus(string deviceStatus)
        {
            string strDeviceStatus = string.Empty;

            switch (deviceStatus)
            {
                case "Close":// 关闭
                    strDeviceStatus = PubHelper.p_LangOper.GetStringBundle("Pub_Device_Close");// "关闭";
                    break;
                case "Open":// 开启
                    strDeviceStatus = PubHelper.p_LangOper.GetStringBundle("Pub_Device_Run");// "运行中";
                    break;
                default:// 不存在控制回路
                    strDeviceStatus = PubHelper.p_LangOper.GetStringBundle("Pub_Circle_NoExist");// "不存在控制回路";
                    break;
            }

            return strDeviceStatus;
        }

        /// <summary>
        /// 货道状态
        /// </summary>
        /// <param name="asileStatus"></param>
        /// <returns></returns>
        public static string Dictionary_AsileStatus(string asileStatus)
        {
            string strStatusInfo = string.Empty;

            switch (asileStatus)
            {
                case "02":// 正常
                    strStatusInfo = PubHelper.p_LangOper.GetStringBundle("Pub_Normal");
                    break;

                default:
                    strStatusInfo = PubHelper.p_LangOper.GetStringBundle("Pub_Error");
                    break;
            }

            return strStatusInfo;
        }

        /// <summary>
        /// 根据货柜编号获取该货柜的出货方式 弹簧 升降等
        /// </summary>
        /// <param name="vendBoxCode"></param>
        /// <returns></returns>
        public static string Dictionary_SellGoodsType(string vendBoxCode)
        {
            string strName = string.Empty;
            for (int i = 0; i < PubHelper.p_BusinOper.AsileOper.VendBoxList.Count; i++)
            {
                if (PubHelper.p_BusinOper.AsileOper.VendBoxList[i].VendBoxCode == vendBoxCode)
                {
                    switch (PubHelper.p_BusinOper.AsileOper.VendBoxList[i].SellGoodsType)
                    {
                        case BusinessEnum.SellGoodsType.Spring:// 弹簧
                            strName = PubHelper.p_LangOper.GetStringBundle("Pub_SellGoodsType_Spring");
                            break;
                        case BusinessEnum.SellGoodsType.Lifter_Comp:// 复杂型升降机
                            strName = PubHelper.p_LangOper.GetStringBundle("Pub_SellGoodsType_Lifter_Comp");
                            break;
                        case BusinessEnum.SellGoodsType.Lifter_Simple:// 简易型升降机
                            strName = PubHelper.p_LangOper.GetStringBundle("Pub_SellGoodsType_Lifter_Simple");
                            break;
                    }
                    break;
                }
            }

            return strName;
        }

        /// <summary>
        /// 转换货柜名称
        /// </summary>
        /// <param name="vendBoxCode"></param>
        /// <returns></returns>
        public static string Dictionary_VendBoxName(string vendBoxCode)
        {
            string strVendBoxName = string.Empty;
            string strVendBoxTitle = string.Empty;

            if (vendBoxCode == "1")
            {
                // 如果是第一个柜子，认为是主柜
                strVendBoxName = PubHelper.p_LangOper.GetStringBundle("Pub_VendBox_Main");
            }
            else
            {
                strVendBoxTitle = PubHelper.p_LangOper.GetStringBundle("Pub_VendBox_Fu");
                strVendBoxName = strVendBoxTitle + (Convert.ToInt32(vendBoxCode) - 1).ToString();
            }

            return strVendBoxName;
        }

        /// <summary>
        /// 转换广告更新策略
        /// </summary>
        /// <param name="advUploadType"></param>
        /// <returns></returns>
        public static string Dictionary_AdvUploadType(string advUploadType)
        {
            string strUploadType = string.Empty;

            // 0：只允许本地更新 1：只允许远程更新 2：本地更新和远程更新皆可
            switch (advUploadType)
            {
                case "0":// 只允许本地更新
                    strUploadType = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Advert_UploadType_Local");
                    break;
                case "1":// 只允许远程更新
                    strUploadType = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Advert_UploadType_Net");
                    break;
                default:// 2：本地更新和远程更新皆可
                    strUploadType = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Advert_UploadType_All");
                    break;
            }

            return strUploadType;
        }

        #region 不同语言下的转换 2016-07-07

        /// <summary>
        /// 不同语言下转换身份证扫描设备错误码释义
        /// </summary>
        /// <returns></returns>
        public static string Dictionary_Lang_IDCardErrCode()
        {
            string strLangValue = string.Empty;
            switch (PubHelper.p_BusinOper.ConfigInfo.Language)
            {
                case BusinessEnum.Language.Zh_CN:// 中文
                    strLangValue = @"0：成功
1：打开设备失败
2：没有打开设备
3：无身份证
4：读取信息失败
99：其它错误";
                    break;
                default:
                    strLangValue = @"0:Success
1:Failed to open device
2:don't open device
3:No IDCard
4:Failed to read
99:Other error";
                    break;
            }

            return strLangValue;
        }

        /// <summary>
        /// 不同语言下转换打印设备错误码释义
        /// </summary>
        /// <returns></returns>
        public static string Dictionary_Lang_PrinterErrCode()
        {
            string strLangValue = string.Empty;
            switch (PubHelper.p_BusinOper.ConfigInfo.Language)
            {
                case BusinessEnum.Language.Zh_CN:// 中文
                    strLangValue = @"0：正常（成功）
1：找不到打印机
2：数据线故障
3：电源线故障
4：打印机忙
5：超时
6：打印机被禁用
7：获取硬件信息失败
8：启动打印机失败
9：打印机上盖被打开
10：纸卷错误（如：缺纸）
11：卡纸
12：纸将尽
13：上一次打印未完成
99：其它错误";
                    break;
                default:
                    strLangValue = @"0:OK(Success)
1:Device isn't exist
2:Data lines error
3:Power lines error
4:Device busying
5:Out of time
6:Device disableed
7:Failed to read device data
8:Failed to run device
9:Device have been opened
99:Other error";
                    break;
            }

            return strLangValue;
        }

        #endregion
    }
}
