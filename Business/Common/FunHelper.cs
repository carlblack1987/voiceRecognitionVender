#region [ KIMMA Co.,Ltd. Copyright (C) 2014 ]
//-------------------------------------------------------------------------------------
// 开发单位：湖南金码智能设备制造有限公司
// 业务模块：iVend终端软件业务处理类
// 业务功能：公共信息处理类
// 创建标识：2014-06-10		谷霖
//-------------------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Business.Enum;

namespace Business
{
    public class FunHelper
    {
        public static BusinessEnum.Language ConvertLanguage(string language)
        {
            BusinessEnum.Language convertLanguage = BusinessEnum.Language.Zh_CN;
            switch (language)
            {
                case "0":// 英文
                    convertLanguage = BusinessEnum.Language.English;
                    break;
                case "1":// 中文简体
                    convertLanguage = BusinessEnum.Language.Zh_CN;
                    break;
                case "2":// 俄文
                    convertLanguage = BusinessEnum.Language.Russian;
                    break;
                case "3":// 法文
                    convertLanguage = BusinessEnum.Language.French;
                    break;
                default:
                    convertLanguage = BusinessEnum.Language.Zh_CN;
                    break;
            }

            return convertLanguage;
        }

        public static BusinessEnum.ControlSwitch ChangeControlSwitch(string controlSwitch)
        {
            if (controlSwitch == "1")
            {
                // 1 启用
                return BusinessEnum.ControlSwitch.Run;
            }
            else
            {
                // 0 停止
                return BusinessEnum.ControlSwitch.Stop;
            }
        }

        public static BusinessEnum.DeviceControlStatus ChangeDeviceControlStatus(string deviceStatus)
        {
            if (deviceStatus == "00")
            {
                // 1 关闭
                return BusinessEnum.DeviceControlStatus.Close;
            }
            else if (deviceStatus == "01")
            {
                // 0 开启
                return BusinessEnum.DeviceControlStatus.Open;
            }
            else
            {
                // 没有回路
                return BusinessEnum.DeviceControlStatus.NoCircle;
            }
        }

        public static BusinessEnum.DeviceControlStatus ChangeDeviceControlStatus_Ref(string deviceStatus)
        {
            // Convert.ToInt32(fd,16);
            if (deviceStatus == "00")
            {
                // 1 关闭
                return BusinessEnum.DeviceControlStatus.Close;
            }
            else if (deviceStatus == "FF")
            {
                // 没有回路
                return BusinessEnum.DeviceControlStatus.NoCircle;
            }
            else
            {
                // 0 开启
                return BusinessEnum.DeviceControlStatus.Open;
            }
        }

        public static bool ConvertIsWriteLog(string isWriteLog)
        {
            if (isWriteLog == "1")
            {
                // 1 记录日志文件
                return true;
            }
            else
            {
                // 0 不记录日志文件
                return false;
            }
        }

        /// <summary>
        /// 把小时分钟时间格式转换为整数值
        /// </summary>
        /// <param name="time">要转换的时间</param>
        /// <returns>转换后的结果</returns>
        public static int ConvertHourMinTime(string time)
        {
            int strConvertTime = 0;

            try
            {
                if (time.Length == 4)
                {
                    strConvertTime = Convert.ToInt32(time.Substring(0, 2)) * 60 + Convert.ToInt32(time.Substring(2));
                }
            }
            catch
            {
            }

            return strConvertTime;
        }

        /// <summary>
        /// 转换托盘编码
        /// </summary>
        public static string ChangeTray(string trayNum,string columnType)
        {
            string strTray = string.Empty;

            if (columnType == "1")
            {
                // 数字型标签
                strTray = trayNum;
            }
            else
            {
                switch (trayNum)
                {
                    case "1"://
                        strTray = "A";
                        break;
                    case "2"://
                        strTray = "B";
                        break;
                    case "3"://
                        strTray = "C";
                        break;
                    case "4"://
                        strTray = "D";
                        break;
                    case "5"://
                        strTray = "E";
                        break;
                    case "6"://
                        strTray = "F";
                        break;
                    case "7"://
                        strTray = "G";
                        break;
                    case "8"://
                        strTray = "H";
                        break;
                    case "9"://
                        strTray = "I";
                        break;
                }
            }

            return strTray;
        }

        /// <summary>
        /// 将金额从转后为string,即将金额加小数点
        /// </summary>
        /// <param name="money">要转换的金额</param>
        /// <returns>转换后的金额</returns>
        public static string MoneyIntToString(string money, int decimalNum,
            string pointNum,string moneySymbol,
            BusinessEnum.ControlSwitch isShowMoneySymbol,bool isSet)
        {
            string strPointNum = decimalNum.ToString();//"100";d

            string strMoney = money;
            int length = strMoney.Length;
            int intPointLen = strPointNum.Length;
            if (length < intPointLen)
            {
                strMoney = strMoney.PadLeft(intPointLen, '0');
            }

            switch (pointNum)
            {
                case "0":// 不显示货币小数点
                    strMoney = money.Substring(0, strMoney.Length - intPointLen + 1);
                    break;

                case "1":// 显示货币小数点
                    strMoney = strMoney.Insert(strMoney.Length - intPointLen + 1, ".");
                    break;
            }

            if ((isShowMoneySymbol == BusinessEnum.ControlSwitch.Run) && (isSet))
            {
                strMoney = moneySymbol + strMoney;
            }

            return strMoney;
        }

        /// <summary>
        /// 备份数据文件
        /// </summary>
        /// <param name="dbFileName"></param>
        /// <returns></returns>
        public static bool BackUpDbFile(string dbFileName)
        {
            bool result = false;

            try
            {
                string strBackDire = AppDomain.CurrentDomain.BaseDirectory.ToString() + "BackUp\\";
                if (!Directory.Exists(strBackDire))
                {
                    // 不存在，创建
                    Directory.CreateDirectory(strBackDire);
                }
                strBackDire += "DB\\";
                if (!Directory.Exists(strBackDire))
                {
                    // 不存在，创建
                    Directory.CreateDirectory(strBackDire);
                }

                string strCopyFile = strBackDire + "BK_" + dbFileName + "_" + DateTime.Now.ToString("yyMMddHHmmss") + ".db";
                string strSourceDbFile = AppDomain.CurrentDomain.BaseDirectory.ToString() + "DB\\" + dbFileName + ".db";
                if (File.Exists(strSourceDbFile))
                {
                    File.Copy(strSourceDbFile, strCopyFile, true);
                }

                result = true;
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public static BusinessEnum.PosBusiType ChangePosBusiType(string icBusiType)
        {
            BusinessEnum.PosBusiType busiType = BusinessEnum.PosBusiType.Other;
            switch (icBusiType)
            {
                case "1":// 武汉一卡通
                    busiType = BusinessEnum.PosBusiType.WuHanTong;
                    break;
                case "2":// 长安通（不带网络）
                    busiType = BusinessEnum.PosBusiType.XiAnTong_NoNet;
                    break;
                default:
                    busiType = BusinessEnum.PosBusiType.Other;
                    break;
            }

            return busiType;
        }

        public static BusinessEnum.ScreenType ChangeScreenType(string screenType)
        {
            BusinessEnum.ScreenType scrype = BusinessEnum.ScreenType.ScreenType26;
            switch (screenType)
            {
                case "0":// 26寸屏
                    scrype = BusinessEnum.ScreenType.ScreenType26;
                    break;
                case "1":// 50寸屏
                    scrype = BusinessEnum.ScreenType.ScreenType50;
                    break;
                default:
                    scrype = BusinessEnum.ScreenType.ScreenType26;
                    break;
            }

            return scrype;
        }

        public static BusinessEnum.TmpControlModel ChangeTmpControlModel(string controlModel)
        {
            BusinessEnum.TmpControlModel model = BusinessEnum.TmpControlModel.Refrigeration;
            switch (controlModel)
            {
                case "1":// 加热
                    model = BusinessEnum.TmpControlModel.Heating;
                    break;
                default:
                    model = BusinessEnum.TmpControlModel.Refrigeration;
                    break;
            }

            return model;
        }

        public static BusinessEnum.GoodsShowModelType ChangeGoodsShowModelType(string goodsShowModel)
        {
            BusinessEnum.GoodsShowModelType model = BusinessEnum.GoodsShowModelType.GoodsToOnlyAsile;
            switch (goodsShowModel)
            {
                case "0":// 商品和货道一一对应模式
                    model = BusinessEnum.GoodsShowModelType.GoodsToOnlyAsile;
                    break;
                case "1":// 不同商品图片对应模式
                    model = BusinessEnum.GoodsShowModelType.GoodsToMultiAsile;
                    break;
                case "2":// 键盘输入货道编号模式
                    model = BusinessEnum.GoodsShowModelType.InputAsileCode;
                    break;
                case "3":// 商品分类显示模式
                    model = BusinessEnum.GoodsShowModelType.GoodsType;
                    break;
                default:// 商品图片对应货道
                    model = BusinessEnum.GoodsShowModelType.GoodsToOnlyAsile;
                    break;
            }

            return model;
        }

        public static BusinessEnum.Main_Lgs_TopType ConvertMainLgsTopType(string topType)
        {
            BusinessEnum.Main_Lgs_TopType model = BusinessEnum.Main_Lgs_TopType.Normal;
            switch (topType)
            {
                case "0":// 普通客户
                    model = BusinessEnum.Main_Lgs_TopType.Normal;
                    break;
                case "1":// 上海志愿者协会
                    model = BusinessEnum.Main_Lgs_TopType.ShangHai_ZhiYuanZhe;
                    break;
                default:
                    model = BusinessEnum.Main_Lgs_TopType.Normal;
                    break;
            }

            return model;
        }

        public static BusinessEnum.CoinDeviceType ConvertCoinDeviceType(string topType)
        {
            BusinessEnum.CoinDeviceType model = BusinessEnum.CoinDeviceType.CoinDevice;
            switch (topType)
            {
                case "0":// 硬币器
                    model = BusinessEnum.CoinDeviceType.CoinDevice;
                    break;
                case "1":// Hook找零器
                    model = BusinessEnum.CoinDeviceType.Hook;
                    break;
                default:
                    model = BusinessEnum.CoinDeviceType.CoinDevice;
                    break;
            }

            return model;
        }

        public static BusinessEnum.CashManagerModel ChangeCashManagerModel(string cashManagerModel)
        {
            if (cashManagerModel == "0")
            {
                // 0 简单模式
                return BusinessEnum.CashManagerModel.Singal;
            }
            else
            {
                // 1 高级模式
                return BusinessEnum.CashManagerModel.Advance;
            }
        }

        /// <summary>
        /// 转换商品销售类型
        /// </summary>
        /// <param name="saleType"></param>
        /// <returns></returns>
        public static BusinessEnum.McdSaleType ChangeMcdSaleType(string saleType)
        {
            BusinessEnum.McdSaleType mcdSaleType = BusinessEnum.McdSaleType.Normal;
            // 0：正常销售 1：热点商品 2：新品 3：打折商品 4：免费商品
            switch (saleType)
            {
                case "1":// 热点商品
                    mcdSaleType = BusinessEnum.McdSaleType.Hot;
                    break;
                case "2":// 新品
                    mcdSaleType = BusinessEnum.McdSaleType.New;
                    break;
                case "3":// 打折促销商品
                    mcdSaleType = BusinessEnum.McdSaleType.DaZhe;
                    break;
                case "4":// 免费商品
                    mcdSaleType = BusinessEnum.McdSaleType.Free;
                    break;
                default:
                    break;
            }
            return mcdSaleType;
        }
    }
}
