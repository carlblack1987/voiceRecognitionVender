#region [ KIMMA Co.,Ltd. Copyright (C) 2014 ]
//-------------------------------------------------------------------------------------
// 开发单位：湖南金码智能设备制造有限公司
// 业务模块：iVend终端软件业务处理类
// 业务功能：系统参数模板
// 创建标识：2014-06-10		谷霖
//-------------------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Business.Enum;

namespace Business.Model
{
    public class ConfigModel
    {
        /// <summary>
        /// 析构
        /// </summary>
        public ConfigModel()
        {
            VmId = "9999999999";
            Language = BusinessEnum.Language.Zh_CN;
            ColumnType = "0";
            SaleModel = "0";
            ChangeModel = "0";
            TunOutTime = 0;
            QueryTmpDelay = 10;
            IsRunStock = BusinessEnum.ControlSwitch.Stop;
            DecimalNum = 100;
            PointNum = "1";
            IcQuerySwitch = BusinessEnum.ControlSwitch.Stop;
            ShowCardInfoTime = 3;
            SellOperOutTime = 0;
            GoodsShowModel = BusinessEnum.GoodsShowModelType.GoodsToOnlyAsile;
            GoodsShowContent = "1";
            GoodsOpacity = 10;
            NoStockClickGoods = "0";
            AllowPaymentList = "cash,";
            ClearLogIntervalDay = "10";
            GoodsNameShowType = "3";
            MinMoneyValue = 50;
            IcBusiModel = "0";
            IcPayShowSwitch = BusinessEnum.ControlSwitch.Stop;
            NoFeeCardPayShow = BusinessEnum.ControlSwitch.Stop;
            IcCardNumHide = BusinessEnum.ControlSwitch.Stop;
            NoFeeCardNumHide = BusinessEnum.ControlSwitch.Stop;
            PosBusiType = BusinessEnum.PosBusiType.Other;
            ScreenType = BusinessEnum.ScreenType.ScreenType26;
            NoFeeCardIsRebate = "0";
            ////TmpControlModel = BusinessEnum.TmpControlModel.Refrigeration;
            AdvertPlaySwitch = BusinessEnum.ControlSwitch.Stop;
            AdvertImgShowTime = 5;
            AdvertPlayOutTime = 1;
            EachPageMaxRowNum = 4;
            EachRowMaxColuNum = 3;
            MoneySymbol = "¥";
            IsShowMoneySymbol = BusinessEnum.ControlSwitch.Stop;
            ////UpDownSellModel = "0";
            ////UpDownDelayTimeNums = 150;
            ////UpDownSendGoodsTimes = 30;
            ////UpDownIsQueryElectStatus = BusinessEnum.ControlSwitch.Run;
            ////UpDownVmLifterType = "0";
            IsPrintConsumeBill = BusinessEnum.ControlSwitch.Stop;
            PrintPort = "1";
            PrintTmepContent = string.Empty;
            PrintTmepTitle = string.Empty;
            GoodsPropShowType = "1";
            IsShowGoodsDetailContent = BusinessEnum.ControlSwitch.Stop;
            GoodsDetailFontSize = 17;
            IDCardPort = "0";
            IsReturnBill = BusinessEnum.ControlSwitch.Stop;
            ReturnBillMoney = 1000;
            IsShowChoiceKeyBoard = BusinessEnum.ControlSwitch.Stop;
            KeyBoardType = "0";
            IDCardFreeTake_Switch = BusinessEnum.ControlSwitch.Stop;
            O2OTake_Switch = BusinessEnum.ControlSwitch.Stop;
            WxTake_Switch = BusinessEnum.ControlSwitch.Stop;
            IsFreeSellNoPay = BusinessEnum.ControlSwitch.Stop;
            IsShowMainLgsBottom = BusinessEnum.ControlSwitch.Stop;
            IsShowMainLgsTop = BusinessEnum.ControlSwitch.Stop;
            MainLgsTop_Type = BusinessEnum.Main_Lgs_TopType.Normal;
            CoinDeviceType = BusinessEnum.CoinDeviceType.CoinDevice;
            CashManagerModel = BusinessEnum.CashManagerModel.Singal;
            UpDownLeftRightNum_Left = 140;
            UpDownLeftRightNum_Center = 101;
            UpDownLeftRightNum_Right = 62;
            AddedServiceSwitch = BusinessEnum.ControlSwitch.Stop;
            IsShowVmDiagnose = BusinessEnum.ControlSwitch.Stop;
            BillStackSwitch = BusinessEnum.ControlSwitch.Stop;
            RemoteControlSwitch = BusinessEnum.ControlSwitch.Stop;
            NowAdvertPlayID = string.Empty;
            UpdateAdvertListID = string.Empty;
            VmSoftApiUrl = string.Empty;
            VmSoftApiUserKey = string.Empty;
            AdvertUploadType = "0";
            GoodsUploadType = "0";
            PriceUploadType = "0";
        }

        /// <summary>
        /// 机器出厂编号
        /// </summary>
        public string VmId { get; set; }

        /// <summary>
        /// 语言版本 0：英文 1：中文简体 2：俄文（Russian） 3：法文（French）
        /// </summary>
        public BusinessEnum.Language Language { get; set; }

        /// <summary>
        /// 货道外部标签类型 0：字符型标签 1：数字型标签
        /// </summary>
        public string ColumnType { get; set; }

        /// <summary>
        /// 购买方式 0：连续购买 1：单次购买
        /// </summary>
        public string SaleModel { get; set; }

        /// <summary>
        /// 兑零模式 0：关闭兑零 1：开启兑零
        /// </summary>
        public string ChangeModel { get; set; }

        /// <summary>
        /// 吞币超时时间，以分钟为单位，默认5分钟
        /// </summary>
        public int TunOutTime { get; set; }

        /// <summary>
        /// 温度采集时间间隔，以分钟为单位
        /// </summary>
        public int QueryTmpDelay { get; set; }

        /// <summary>
        /// 是否启用库存 0：不启用 1：启用
        /// </summary>
        public BusinessEnum.ControlSwitch IsRunStock { get; set; }

        /// <summary>
        /// 小数点位数
        /// </summary>
        public int DecimalNum { get; set; }

        /// <summary>
        /// 是否显示货币小数点位数 0：不显示 1：显示
        /// </summary>
        public string PointNum { get; set; }

        /// <summary>
        /// 是否在主界面时显示卡查询信息 0：不显示（默认） 1：显示
        /// </summary>
        public BusinessEnum.ControlSwitch IcQuerySwitch { get; set; }

        /// <summary>
        /// 在主界面时显示卡查询信息展现时间 以秒为单位
        /// </summary>
        public int ShowCardInfoTime { get; set; }

        /// <summary>
        /// 购物无操作超时时间，以秒为单位
        /// </summary>
        public int SellOperOutTime { get; set; }

        /// <summary>
        /// 商品展示模式 0：商品对应货道（默认） 1：不同商品对应模式 2：键盘输入货道编号模式
        /// </summary>
        public BusinessEnum.GoodsShowModelType GoodsShowModel { get; set; }

        /// <summary>
        /// 商品列表显示内容 0：不显示任何内容 1：只显示商品价格（默认） 2：只显示商品所在货道编号 3：显示商品价格和所在货道编号
        /// </summary>
        public string GoodsShowContent { get; set; }

        /// <summary>
        /// 商品无库存时透明度
        /// </summary>
        public int GoodsOpacity { get; set; }
        
        /// <summary>
        /// 商品无库存时是否允许点击
        /// </summary>
        public string NoStockClickGoods { get; set; }

        /// <summary>
        /// 允许支付方式列表
        /// </summary>
        public string AllowPaymentList { get; set; }

        /// <summary>
        /// 日志清除间隔天数
        /// </summary>
        public string ClearLogIntervalDay { get; set; }

        /// <summary>
        /// 商品展示页面中商品名称显示类型 0：不显示任何内容 1：只显示商品名称 2：只显示货道编号 3：显示商品名称及货道编号
        /// </summary>
        public string GoodsNameShowType { get; set; }

        /// <summary>
        /// 支持的最先面值的金额，乘以小数点
        /// </summary>
        public int MinMoneyValue { get; set; }

        /// <summary>
        /// 储值卡业务流程模式 0：先查询后扣款模式 1：直接扣款模式
        /// </summary>
        public string IcBusiModel { get; set; }

        /// <summary>
        /// 储值卡用户信息是否显示 0：不显示 1：显示
        /// </summary>
        public BusinessEnum.ControlSwitch IcPayShowSwitch { get; set; }

        /// <summary>
        /// 会员卡用户信息是否显示 0：不显示1：显示
        /// </summary>
        public BusinessEnum.ControlSwitch NoFeeCardPayShow { get; set; }

        /// <summary>
        /// 会员卡卡号信息*字显示 0：不显示1：显示
        /// </summary>
        public BusinessEnum.ControlSwitch IcCardNumHide { get; set; }

        /// <summary>
        /// 会员卡卡号信息*字显示 0：不显示1：显示
        /// </summary>
        public BusinessEnum.ControlSwitch NoFeeCardNumHide { get; set; }

        /// <summary>
        /// 一卡通业务类型
        /// </summary>
        public BusinessEnum.PosBusiType PosBusiType { get; set; }

        /// <summary>
        /// 屏幕类型 0:26寸屏 1:50寸屏
        /// </summary>
        public BusinessEnum.ScreenType ScreenType { get; set; }

        /// <summary>
        /// 会员卡是否打折 0：不打折 1：打折
        /// </summary>
        public string NoFeeCardIsRebate { get; set; }

        /////// <summary>
        /////// 温控模式 制冷或加热
        /////// </summary>
        ////public BusinessEnum.TmpControlModel TmpControlModel { get; set; }

        /// <summary>
        /// 是否播放广告
        /// </summary>
        public BusinessEnum.ControlSwitch AdvertPlaySwitch { get; set; }

        /// <summary>
        /// 图片广告显示时间，以秒为单位
        /// </summary>
        public int AdvertImgShowTime { get; set; }

        /// <summary>
        /// 无人操作播放广告时间，以分钟为单位
        /// </summary>
        public int AdvertPlayOutTime { get; set; }

        /// <summary>
        /// 商品列表最大行数
        /// </summary>
        public int EachPageMaxRowNum { get; set; }

        /// <summary>
        /// 商品列表每行最大列数
        /// </summary>
        public int EachRowMaxColuNum { get; set; }

        /// <summary>
        /// 国家货币符号
        /// </summary>
        public string MoneySymbol { get; set; }

        /// <summary>
        /// 是否显示货币符号
        /// </summary>
        public BusinessEnum.ControlSwitch IsShowMoneySymbol { get; set; }

        /////// <summary>
        /////// 升降机出货形式参数 0：直接升降 1：按参数升降
        /////// </summary>
        ////public string UpDownSellModel { get; set; }

        /////// <summary>
        /////// 升降机出货延时
        /////// </summary>
        ////public int UpDownDelayTimeNums { get; set; }

        /////// <summary>
        /////// 云台至出货口后延时时长，以毫秒为单位，最大延时255毫秒，默认30毫秒
        /////// </summary>
        ////public int UpDownSendGoodsTimes { get; set; }

        /////// <summary>
        /////// 是否检测升降机光电管状态 
        /////// </summary>
        ////public BusinessEnum.ControlSwitch UpDownIsQueryElectStatus { get; set; }

        /////// <summary>
        /////// 升降机类型 0：复杂升降机 1：简易升降机
        /////// </summary>
        ////public string UpDownVmLifterType { get; set; }

        /// <summary>
        /// 升降机横向点击移动码盘数—最左边
        /// </summary>
        public int UpDownLeftRightNum_Left { get; set; }

        /// <summary>
        /// 升降机横向点击移动码盘数—中间
        /// </summary>
        public int UpDownLeftRightNum_Center { get; set; }

        /// <summary>
        /// 升降机横向点击移动码盘数—最右边
        /// </summary>
        public int UpDownLeftRightNum_Right { get; set; }

        
        /// <summary>
        /// 是否打印购物单据
        /// </summary>
        public BusinessEnum.ControlSwitch IsPrintConsumeBill { get; set; }

        /// <summary>
        /// 打印串口
        /// </summary>
        public string PrintPort { get; set; }

        /// <summary>
        /// 热敏打印票据内容
        /// </summary>
        public string PrintTmepContent { get; set; }

        /// <summary>
        /// 热敏打印票据表头
        /// </summary>
        public string PrintTmepTitle { get; set; }

        /// <summary>
        /// 商品展示页面中商品属性显示类型 0：不显示任何内容 1：只显示商品介绍
        /// 2：只显示商品规格
       /// 3：只显示生产厂家
       /// 4：显示商品规格和生产厂家
        /// </summary>
        public string GoodsPropShowType { get; set; }

        /// <summary>
        /// 是否显示商品内容详细介绍信息，主要针对商品介绍比较长的情况，如：医药、成人用品等，如果显示，则在商品详细展示页面下部分显示
        /// </summary>
        public BusinessEnum.ControlSwitch IsShowGoodsDetailContent { get; set; }

        /// <summary>
        /// 商品详细介绍字体大小
        /// </summary>
        public int GoodsDetailFontSize { get; set; }

        /// <summary>
        /// 二代身份证设备端口
        /// </summary>
        public string IDCardPort { get; set; }

        /// <summary>
        /// 是否有纸币找零功能
        /// </summary>
        public BusinessEnum.ControlSwitch IsReturnBill { get; set; }

        /// <summary>
        /// 纸币找零面值
        /// </summary>
        public int ReturnBillMoney { get; set; }

        /// <summary>
        /// 是否显示选货键盘
        /// </summary>
        public BusinessEnum.ControlSwitch IsShowChoiceKeyBoard { get; set; }

        /// <summary>
        /// 输入键盘类型 0：小键盘 1：大键盘
        /// </summary>
        public string KeyBoardType { get; set; }

        /// <summary>
        /// 身份证免费领是否开启
        /// </summary>
        public BusinessEnum.ControlSwitch IDCardFreeTake_Switch { get; set; }

        /// <summary>
        /// 线下取货服务是否开启
        /// </summary>
        public BusinessEnum.ControlSwitch O2OTake_Switch { get; set; }

        /// <summary>
        /// 微信关注取货码服务是否开启
        /// </summary>
        public BusinessEnum.ControlSwitch WxTake_Switch { get; set; }

        /// <summary>
        /// 是否无需支付免费出货
        /// </summary>
        public BusinessEnum.ControlSwitch IsFreeSellNoPay { get; set; }

        /// <summary>
        /// 是否显示主界面底部的运营商信息显示区域
        /// </summary>
        public BusinessEnum.ControlSwitch IsShowMainLgsBottom { get; set; }

        /// <summary>
        /// 是否显示主界面顶部的运营商信息显示区域
        /// </summary>
        public BusinessEnum.ControlSwitch IsShowMainLgsTop { get; set; }

        /// <summary>
        /// 主界面的运营商顶部信息类型 0：普通客户 1：上海志愿者协会
        /// </summary>
        public BusinessEnum.Main_Lgs_TopType MainLgsTop_Type { get; set; }

        /// <summary>
        /// 硬币设备类型 0：硬币器 1：Hook找零器
        /// </summary>
        public BusinessEnum.CoinDeviceType CoinDeviceType { get; set; }

        /// <summary>
        /// 货币管理模式 0：简单模式 1：高级模式，主要针对以前的硬币器为简单模式（货币库存查询等属于高级模式）
        /// </summary>
        public BusinessEnum.CashManagerModel CashManagerModel { get; set; }

        /// <summary>
        /// 增值服务开关 0：关闭【不具有】 1：打开【具有】
        /// </summary>
        public BusinessEnum.ControlSwitch AddedServiceSwitch { get; set; }

        /// <summary>
        /// 门关时是否显示机器诊断 0：不显示 1：显示
        /// </summary>
        public BusinessEnum.ControlSwitch IsShowVmDiagnose { get; set; }

        /// <summary>
        /// 纸币器缓存开关 0：关闭缓存 1：打开缓存
        /// </summary>
        public BusinessEnum.ControlSwitch BillStackSwitch { get; set; }

        /// <summary>
        /// 是否接收远程控制指令 0：不接收 1：接收
        /// </summary>
        public BusinessEnum.ControlSwitch RemoteControlSwitch { get; set; }

        /// <summary>
        /// 当前播放的广告节目单编号
        /// </summary>
        public string NowAdvertPlayID { get; set; }

        /// <summary>
        /// 正在更新的广告节目单编号
        /// </summary>
        public string UpdateAdvertListID { get; set; }

        /// <summary>
        /// 广告更新策略 0：只允许本地更新 1：只允许远程更新 2：本地更新和远程更新皆可
        /// </summary>
        public string AdvertUploadType { get; set; }

        /// <summary>
        /// 终端远程更新API网址
        /// </summary>
        public string VmSoftApiUrl { get; set; }

        /// <summary>
        /// 终端远程更新API的UserKey
        /// </summary>
        public string VmSoftApiUserKey { get; set; }

        /// <summary>
        /// 商品更新策略 0：只允许本地更新 1：只允许远程更新 2：本地更新和远程更新皆可
        /// </summary>
        public string GoodsUploadType { get; set; }

        /// <summary>
        /// 价格设置策略 0：本地设置 1：远程更新
        /// </summary>
        public string PriceUploadType { get; set; }
    }
}
