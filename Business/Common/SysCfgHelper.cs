#region [ KIMMA Co.,Ltd. Copyright (C) 2014 ]
//-------------------------------------------------------------------------------------
// 开发单位：湖南金码智能设备制造有限公司
// 业务模块：iVend终端软件业务处理类
// 业务功能：配置参数处理类
// 创建标识：2014-06-10		谷霖
//-------------------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using KdbPlug;
using Business.Model;

namespace Business.Common
{
    public class SysCfgHelper
    {
        #region 变量声明

        private string _m_DbFileName = "TermInfo.db";

        /// <summary>
        /// 第三方配置参数数据库文件名称
        /// </summary>
        private string m_DbFileName_Third = "ThirdConfigInfo.db";

        /// <summary>
        /// 系统参数配置链式表
        /// </summary>
        public List<SysCfgInfoModel> SysCfgList = new List<SysCfgInfoModel>();

        /// <summary>
        /// 第三方参数配置链式表—上海志愿者
        /// </summary>
        public List<SysCfgInfoModel> SysCfgList_Third = new List<SysCfgInfoModel>();

        /// <summary>
        /// 国家信息链式表
        /// </summary>
        public List<CountryModel> CountryList = new List<CountryModel>();

        /// <summary>
        /// 国家信息对象
        /// </summary>
        public CountryModel CountryInfo = new CountryModel();

        #endregion

        #region 公有函数

        /// <summary>
        /// 加载系统配置参数
        /// </summary>
        /// <returns></returns>
        public bool LoadSysCfg()
        {
            bool result = false;

            SysCfgList.Clear();

            #region 添加新的系统配置参数

            #region 已注释

            //////result = AddNewSysCfg("0", "007", "NetEquKind", "0", "0", "S", "网络设备连接方式 0：上位机连接网络设备 1：控制主板连接网络设备");
            //////result = AddNewSysCfg("1", "008", "IsTestVer", "0", "0", "S", "是否是测试版本 0：不是 1：是");
            //////result = AddNewSysCfg("1", "009", "SellFailTryNum", "0", "0", "S", "出货失败重试次数 0：不重试 1：重试1次等");
            //////result = AddNewSysCfg("0", "010", "DriveConnType", "1", "1", "S", "驱动板是否连接到控制主板 0：关闭（不连接） 1：启用（连接） 默认为1");
            //////result = AddNewSysCfg("1", "011", "RefOpenMaxTime", "50", "3", "S", "制冷压缩机最长打开时间，以分钟为单位");
            //////result = AddNewSysCfg("1", "231", "TmpRunDelay", "7", "3", "S", "制冷压缩机关闭后再次打开的延长时间，以分钟为单位");
            //////result = AddNewSysCfg("1", "013", "SkinStyle", "0", "0", "S", "皮肤样式 0：蓝天白云 1：星空");

            //////result = AddNewSysCfg("0", "014", "ClearLogIntervalDay", "10", "0", "S", "日志清理间隔天数，以天为单位，默认10天，清除该参数之前的日志文件");

            //////#region 日志配置参数 2014-12-07添加

            //////result = AddNewSysCfg("0", "015", "IsWriteLog_Busi", "1", "0", "S", "是否记录业务日志 0：不记录 1：记录（默认）");
            //////result = AddNewSysCfg("0", "016", "IsWriteLog_GateWay", "1", "0", "S", "是否记录网关日志 0：不记录 1：记录（默认）");
            //////result = AddNewSysCfg("0", "017", "IsWriteLog_Kmb", "1", "0", "S", "是否记录主板日志 0：不记录 1：记录（默认）");
            //////result = AddNewSysCfg("0", "018", "IsWriteLog_Card", "1", "0", "S", "是否记录刷卡日志 0：不记录 1：记录（默认）");
            //////result = AddNewSysCfg("0", "019", "IsWriteLog_ID", "1", "0", "S", "是否记录会员卡日志 0：不记录 1：记录（默认）");

            //////#endregion

            //////result = AddNewSysCfg("0", "020", "CountryCode", "CHN", "0", "S", "客户所属国家代码");

            //////result = AddNewSysCfg("1", "303", "IcQuerySwitch", "0", "3", "S", "刷卡信息查询功能开关 0：禁用（默认） 1：启用");
            //////result = AddNewSysCfg("1", "304", "ShowCardInfoTime", "3", "3", "S", "在主界面时显示卡查询信息展现时间，以秒为单位");

            //////#region 消毒灯（紫外线灯）参数

            //////result = AddNewSysCfg("1", "360", "SterilampModel", "2", "3", "U", "消毒紫外灯控制模式 1：全时段开启（默认）2：全时段关闭 0：定时开启");
            //////result = AddNewSysCfg("1", "361", "SterilampBeginTime1", "0800", "3", "U", "消毒时间段1开始时间");
            //////result = AddNewSysCfg("1", "362", "SterilampEndTime1", "2000", "3", "U", "消毒时间段1结束时间");
            //////result = AddNewSysCfg("1", "363", "SterilampBeginTime2", "0800", "3", "U", "消毒时间段2开始时间");
            //////result = AddNewSysCfg("1", "364", "SterilampEndTime2", "2000", "3", "U", "消毒时间段2结束时间");

            //////#endregion

            //////#region 支付宝声波支付参数

            //////result = AddNewSysCfg("1", "500", "AliPayControlSwitch", "0", "3", "U", "支付宝声波支付开关 0：关闭 1：开启");
            //////result = AddNewSysCfg("0", "501", "AliPayWebUrl", "http://alipay.kivend.net/AliPayWaveService.aspx", "3", "S", "支付宝声波支付WebUrl");
            //////result = AddNewSysCfg("0", "502", "AliPayWaveTimeOut", "60", "3", "S", "支付宝声波支付监听超时，以秒为单位，默认20秒");

            //////#endregion

            //////#region 非储值会员卡支付参数

            //////result = AddNewSysCfg("1", "503", "NoFeeCardSwitch", "0", "3", "U", "非储值会员卡支付开关 0：关闭 1：开启");
            //////result = AddNewSysCfg("1", "504", "QRCodeCardSwitch", "0", "3", "U", "虚拟会员卡（二维码）支付开关 0：关闭 1：开启");
            //////result = AddNewSysCfg("0", "505", "NoFeeCardWebUrl", "0", "3", "S", "非储值会员卡支付WebUrl");
            //////result = AddNewSysCfg("0", "506", "NoFeeCardPort", "3", "3", "S", "非储值会员卡设备串口");
            //////result = AddNewSysCfg("0", "507", "QrCodePort", "4", "3", "S", "二维码扫描设备串口");
            //////result = AddNewSysCfg("0", "510", "QRWebUrl", "0", "3", "S", "二维码支付WebUrl");

            //////#endregion

            //////#region 银联支付参数

            //////result = AddNewSysCfg("1", "508", "UnionPaySwitch", "0", "3", "U", "银联支付开关 0：关闭 1：开启");
            //////result = AddNewSysCfg("0", "509", "UnionPayPort", "4", "3", "S", "银联支付设备串口");

            //////result = AddNewSysCfg("0", "511", "AllowPaymentList", "cash,", "3", "S", "允许支付方式列表，默认只有现金支付，允许的各支付方式以，隔开");

            //////result = AddNewSysCfg("1", "512", "AliPayCodeSwitch", "0", "3", "U", "支付宝扫码支付开关 0：关闭 1：开启");
            //////result = AddNewSysCfg("0", "513", "AliPayCodeWebUrl", "http://alipay.kivend.net/AliPayWaveService.aspx", "3", "S", "支付宝扫码支付WebUrl");

            //////#endregion

            //////#region 2014-11-03

            //////result = AddNewSysCfg("0", "190", "GoodsShowModel", "0", "4", "S", "商品展示模式 0：商品图片对应货道 1：不同商品图片对应模式 2：键盘输入货道编号模式，默认为0");
            //////result = AddNewSysCfg("1", "191", "GoodsShowContent", "1", "4", "U", "商品列表显示内容 0：不显示任何内容 1：只显示商品价格 2：只显示商品所在货道编号 3：显示商品价格和所在货道编号");

            //////result = AddNewSysCfg("0", "192", "GoodsOpacity", "10", "4", "S", "商品无库存时透明度 1-10,10表示不变虚，1-9便是0.1-0.9 该参数只有在商品启用库存时才起作用");
            //////result = AddNewSysCfg("0", "193", "NoStockClickGoods", "0", "4", "S", "商品无库存时是否允许点击 0：不允许 1：允许（默认）");

            //////result = AddNewSysCfg("0", "201", "GoodsNameShowType", "3", "4", "S", "商品展示页面中商品名称显示类型 0：不显示任何内容 1：只显示商品名称 2：只显示货道编号 3：显示商品名称及货道编号");

            //////#region 声音设备控制参数

            //////result = AddNewSysCfg("1", "420", "SoundModel", "1", "16", "U", "声音设备控制模式 1：全时段开启（默认）2：全时段关闭 0：定时开启");
            //////result = AddNewSysCfg("1", "421", "SoundBeginTime1", "0800", "16", "U", "时间段1开始时间");
            //////result = AddNewSysCfg("1", "422", "SoundEndTime1", "2000", "16", "U", "时间段1结束时间");
            //////result = AddNewSysCfg("1", "423", "SoundBeginTime2", "0800", "16", "U", "时间段2开始时间");
            //////result = AddNewSysCfg("1", "424", "SoundEndTime2", "2000", "16", "U", "时间段2结束时间");

            //////#endregion

            //////#endregion

            //////#region 卡相关参数 2014-12-18添加

            //////result = AddNewSysCfg("0", "307", "IcBusiModel", "0", "0", "S", "储值卡业务流程模式 0：先查询后扣款模式 1：直接扣款模式");
            //////result = AddNewSysCfg("0", "308", "IcPayShowSwitch", "0", "0", "S", "储值卡用户信息是否显示 0：不显示 1：显示，默认为0");
            //////result = AddNewSysCfg("0", "514", "NoFeeCardPayShow", "0", "0", "S", "会员卡用户信息是否显示 0：不显示1：显示，默认为0");

            //////#endregion

            //////#region 卡号显示相关参数 2014-12-20添加

            //////result = AddNewSysCfg("0", "309", "IcCardNumHide", "0", "0", "S", "卡号*字显示 0：不显示1：显示，默认为0");
            //////result = AddNewSysCfg("0", "515", "NoFeeCardNumHide", "0", "0", "S", "会员卡卡号信息是否显示 0：不显示1：显示，默认为0");

            //////#endregion

            //////#region 微信扫码支付相关参数 2014-12-21添加

            //////result = AddNewSysCfg("1", "516", "WeChatCodeSwitch", "0", "3", "U", "微信扫码支付开关 0：关闭 1：开启");

            //////#endregion

            #endregion

            #region 一卡通业务类型参数 2015-01-07添加

            result = AddNewSysCfg("0", "310", "IcBusiType", "0", "3", "U", "一卡通业务类型 0：无 1：武汉通 2：长安通（不带网络）等");
            result = AddNewSysCfg("0", "311", "IcWebUrl", "http://vmterm.kivend.net/", "3", "U", "一卡通服务WebUrl");

            #endregion

            #region 条形码设备参数及实时网关设置参数 2015-01-15添加

            result = AddNewSysCfg("0", "148", "RTimeServerIp", "gate.kivend.net", "3", "U", "实时网关IP");
            result = AddNewSysCfg("0", "149", "RTimeServerPort", "5100", "3", "U", "实时网关端口");

            result = AddNewSysCfg("0", "517", "BarCodeScanPort", "1", "3", "U", "条形码扫描设备串口");

            #endregion

            #region 2015-01-24添加 （屏幕类型）

            result = AddNewSysCfg("0", "021", "ScreenType", "0", "0", "S", "屏幕类型 0:26寸屏 1:50寸屏");

            #endregion

            #region 2015-01-28添加（会员卡打折，目前米旗专用）

            result = AddNewSysCfg("1", "518", "NoFeeCardIsRebate", "0", "0", "S", "会员卡是否打折 0：不打折 1：打折");

            #endregion

            #region 2015-02-14添加 （参数上传日期）

            result = AddNewSysCfg("0", "022", "UploadParaDate", "", "0", "S", "参数上传的最后日期");

            result = AddNewSysCfg("0", "232", "Tmp1ControlModel", "0", "0", "S", "主箱温控工作模式 0：制冷 1：加热");

            #endregion

            #region 2015-03-05添加（终端与后台通信Web Url）

            result = AddNewSysCfg("0", "023", "VmTermApiUrl", "http://vmterm.kivend.net/VmApi.asmx", "0", "S", "终端获取数据API接口URL");

            #endregion

            #region 2015-03-16添加 （广告播放参数）

            result = AddNewSysCfg("0", "600", "AdvertPlaySwitch", "0", "6", "U", "是否播放广告 0：不播放 1：播放");
            result = AddNewSysCfg("0", "601", "AdvertImgShowTime", "5", "6", "U", "图片广告显示时间 以秒为单位，默认5秒");
            result = AddNewSysCfg("0", "602", "AdvertPlayOutTime", "1", "6", "U", "无人操作播放广告时间 以分钟为单位，默认1分钟");
            result = AddNewSysCfg("0", "603", "AdvertVolume", "60", "6", "U", "视频广告播放音量");

            // 2016-09-09添加
            result = AddNewSysCfg("0", "604", "NowAdvertPlayID", "", "6", "U", "当前播放广告节目单编号");
            result = AddNewSysCfg("0", "605", "UpdateAdvertListID", "", "6", "U", "正在更新的广告节目单编号");
            result = AddNewSysCfg("0", "606", "AdvertUploadType", "0", "6", "U", "广告更新策略 0：只允许本地更新 1：只允许远程更新 2：本地更新和远程更新皆可");

            #endregion

            #region 2015-05-23添加

            result = AddNewSysCfg("0", "202", "EachPageMaxRowNum", "4", "6", "U", "商品列表最大行数");
            result = AddNewSysCfg("0", "203", "EachRowMaxColuNum", "3", "6", "U", "商品列表每行最大列数");

            #endregion

            #region 2015-05-27添加

            result = AddNewSysCfg("0", "024", "IsShowMoneySymbol", "0", "6", "U", "是否显示货币符号 0：不显示 1：显示");

            #endregion

            #region 2015-06-02添加

            result = AddNewSysCfg("0", "325", "UpDownSellModel", "0", "6", "U", "升降机出货形式参数 0：直接升降 1：按参数升降");
            result = AddNewSysCfg("0", "326", "UpDownDelayTimeNums", "150", "6", "U", "升降机出货延时 以5毫秒为单位，最大500ms");
            result = AddNewSysCfg("0", "327", "UpDownSendGoodsTimes", "30", "6", "U", "云台至出货口后延时时长，以毫秒为单位，最大延时255毫秒，默认30毫秒，0表示不延时");

            result = AddNewSysCfg("0", "328", "UpDownIsQueryElectStatus", "1", "6", "U", "是否检测升降机光电管状态 0：不检测 1：检测（默认）");
            result = AddNewSysCfg("0", "329", "UpDownVmLifterType", "0", "6", "U", "升降机类型 0：复杂升降机 1：简易升降机");
            
            #endregion

            #region 2015-06-08添加（打印机参数）

            result = AddNewSysCfg("1", "700", "PrintPort", "3", "7", "U", "热敏打印机串口");
            result = AddNewSysCfg("1", "701", "IsPrintConsumeBill", "0", "7", "U", "是否打印购物单据 0：不打印 1：打印");
            string strPrintContent = @"
交易序号：{SerNo}

商品名称：{GoodsName}

生产批次：{GoodsPiCi}

生产厂商：{Manufacturer}

商品规格：{GoodsSpec}

商品价格：{GoodsPrice}

消费金额：{Money}

购买时间：{BuyTime}

购买地点：{TermCode}

支付方式：{PayMent}

消费卡号：{CardNum}
----------------------------------
感谢您的光临

服务电话：021-98897890
";
            result = AddNewSysCfg("0", "702", "PrintTmepContent", strPrintContent, "7", "U", "购物单据打印内容");
            result = AddNewSysCfg("0", "703", "PrintTmepTitle", "售货机购物小票", "7", "U", "购物单据打印表头");

            #endregion

            #region 2015-06-11添加（身份证参数及O2O有关参数）

            result = AddNewSysCfg("0", "720", "IDCardPort", "0", "7", "U", "身份证设备端口 0：USB口 其它：串口");
            result = AddNewSysCfg("0", "721", "IDCardWebUrl", "http://", "7", "U", "二代身份证Web Url");

            #region 2015-08-07添加

            result = AddNewSysCfg("0", "722", "IDCardFreeTake_Switch", "0", "7", "U", "身份证免费领取是否开启 0：关闭 1：开启");
            result = AddNewSysCfg("0", "723", "IDCardFreeTake_TopMv", "IDCard_Oper.mp4", "7", "U", "身份证免费领取界面头部文件");
            result = AddNewSysCfg("0", "724", "IDCardFreeTake_BottomMv", "IDCard_Rule.png", "7", "U", "身份证免费领取界面下部文件");
            result = AddNewSysCfg("0", "725", "O2OServerUrl", "http://123.57.212.86/ybmall/open", "7", "U", "O2O服务的Web Url");

            result = AddNewSysCfg("0", "726", "O2OTake_Switch", "0", "7", "U", "线下扫码取货是否开启 0：关闭 1：开启");
            result = AddNewSysCfg("0", "727", "O2OTake_TopMv", "BarCode_Oper.mp4", "7", "U", "线下扫码取货界面头部文件");
            result = AddNewSysCfg("0", "728", "O2OTake_BottomMv", "BarCode_Rule.png", "7", "U", "线下扫码取货界面下部文件");

            result = AddNewSysCfg("0", "729", "WxTake_Switch", "0", "7", "U", "微信取货码是否开启 0：关闭 1：开启");
            result = AddNewSysCfg("0", "730", "WxTake_TopMv", "WxTake_Oper.mp4", "7", "U", "微信取货码界面头部文件");
            result = AddNewSysCfg("0", "731", "WxTake_BottomMv", "WxTake_Rule.png", "7", "U", "微信取货码界面下部文件");
            result = AddNewSysCfg("0", "732", "WxTake_ServerUrl", "http://mqmzapi.kivend.net", "7", "U", "微信取货服务的Web Url");
            result = AddNewSysCfg("0", "733", "WxTake_UserKey", "76fc91ec3340af9b3fb928b67ab5059b", "7", "U", "微信取货码API接入Key");
    
            result = AddNewSysCfg("0", "734", "IDCardFreeTake_Name", "免费领", "7", "U", "身份证免费领取的服务按钮名称");
            result = AddNewSysCfg("0", "735", "O2OTake_Name", "提货", "7", "U", "线下扫码取货的服务按钮名称");
            result = AddNewSysCfg("0", "736", "WxTake_Name", "关注有礼", "7", "U", "微信关注有礼的服务按钮名称");

            result = AddNewSysCfg("0", "737", "WxTake_CodeLen", "6", "7", "U", "微信关注有礼的取货码长度，为0表示不限制长度");
            result = AddNewSysCfg("0", "738", "WxTake_Name_CodeNum", "取货码", "7", "U", "微信取货码的名称");

            result = AddNewSysCfg("0", "739", "O2OServer_UserKey", "76fc91ec3340af9b3fb928b67ab5059b", "7", "U", "O2O服务的接入Key");
            result = AddNewSysCfg("0", "740", "O2OTake_CodeLen", "21", "7", "U", "线下扫码的码长度，为0表示不限制长度");
            result = AddNewSysCfg("0", "741", "O2OTake_Name_CodeNum", "取货订单号", "7", "U", "线下扫码的码名称");

            #endregion

            #endregion

            #region 2015-06-12添加（商品展示页面参数）

            result = AddNewSysCfg("0", "204", "GoodsPropShowType", "1", "2", "U", "商品展示页面中商品属性显示类型 0：不显示任何内容 1：只显示商品介绍");

            // 是否显示商品内容详细介绍信息，主要针对商品介绍比较长的情况，如：医药、成人用品等，如果显示，则在商品详细展示页面下部分显示
            result = AddNewSysCfg("0", "205", "IsShowGoodsDetailContent", "0", "2", "U", "是否显示商品详细介绍信息 0：不显示 1：显示");

            // 商品内容详细介绍字体大小
            result = AddNewSysCfg("0", "206", "GoodsDetailFontSize", "17", "2", "U", "商品内容详细介绍字体大小");

            result = AddNewSysCfg("0", "207", "RedOTCTipInfo", "红色OTC药品，请在医生指导下使用", "2", "U", "红色OTC药品提示语");

            result = AddNewSysCfg("0", "208", "FreeTakeTipInfo", @"1、凭身份证每人每月限免费领取两盒；

2、未成年人请勿领取。", "2", "U", "免费领取活动规则说明");

            #endregion

            #region 2015-07-07添加

            result = AddNewSysCfg("0", "025", "IsTenAsileNum", "0", "6", "U", "托盘是否有10个货道 0：没有 1：有");

            #endregion

            #region 2015-07-23添加

            result = AddNewSysCfg("0", "026", "IsReturnBill", "0", "6", "U", "是否有纸币找零功能 0：没有 1：有");
            result = AddNewSysCfg("0", "027", "ReturnBillMoney", "1000", "6", "U", "找零纸币面值");

            result = AddNewSysCfg("0", "028", "IsShowChoiceKeyBoard", "0", "6", "U", "是否显示选货键盘");

            #endregion

            #region 2015-08-15添加

            // 临时测试，需要更改默认参数值
            result = AddNewSysCfg("0", "029", "IsShowGoodsTypeName", "0", "6", "U", "是否显示商品类型名称 0：不显示 1：显示");
            result = AddNewSysCfg("0", "030", "KeyBoardType", "1", "6", "U", "输入键盘类型 0：小键盘 1：大键盘");

            result = AddNewSysCfg("0", "031", "IsFreeSellNoPay", "0", "6", "U", "是否无需支付，直接出货 0：否 1：是");

            //result = AddNewSysCfg("0", "032", "PriceSetModel", "0", "6", "U", "价格设置模式 0：终端本地设置 1：远程更新价格");

            result = AddNewSysCfg("0", "033", "WebUrlOutTime", "3", "6", "U", "查看Web网页的超时时间，以分钟为单位");

            #endregion

            #region 2015-11-16添加

            result = AddNewSysCfg("0", "034", "IsShowMainLgsBottom", "0", "6", "U", "是否显示主界面的运营商底部信息区域 0：不显示 1：显示");
            result = AddNewSysCfg("0", "035", "IsShowMainLgsTop", "0", "6", "U", "是否显示主界面的运营商头部信息区域 0：不显示 1：显示");
            result = AddNewSysCfg("0", "036", "MainLgsBottom_Height", "200", "6", "U", "主界面的运营商底部信息区域高度");
            result = AddNewSysCfg("0", "037", "MainLgsTop_Height", "400", "6", "U", "主界面的运营商顶部信息区域高度");

            result = AddNewSysCfg("0", "038", "IsShowMainServerArea", "0", "6", "U", "是否显示主界面的增值服务区域 0：不显示 1：显示");
            result = AddNewSysCfg("0", "039", "MainLgsTop_Type", "0", "6", "U", "主界面的运营商顶部信息类型 0：普通客户 1：上海志愿者协会");

            #endregion

            #region 2015-11-25添加

            result = AddNewSysCfg("0", "040", "CoinDeviceType", "0", "6", "U", "硬币设备类型 0：硬币器 1：Hook找零器");
            result = AddNewSysCfg("0", "041", "OtherBrowseOutTime", "3", "6", "U", "其它内容浏览超时时间，以分钟为单位");

            result = AddNewSysCfg("0", "042", "CashManagerModel", "0", "6", "U", "货币管理模式 0：简单模式 1：高级模式");

            #endregion

            #region 2016-05-31添加

            result = AddNewSysCfg("1", "519", "BestPayCodeSwitch", "0", "3", "U", "翼支付付款码支付开关 0：关闭 1：开启");

            #endregion

            #region 2016-07-06添加

            result = AddNewSysCfg("0", "330", "UpDownLeftRightNum_Left", "140", "6", "U", "升降机横向点击移动码盘数—最左边，针对复杂型升降机");
            result = AddNewSysCfg("0", "331", "UpDownLeftRightNum_Center", "101", "6", "U", "升降机横向点击移动码盘数—中间，针对复杂型升降机");
            result = AddNewSysCfg("0", "332", "UpDownLeftRightNum_Right", "62", "6", "U", "升降机横向点击移动码盘数—最右边，针对复杂型升降机");

            result = AddNewSysCfg("0", "043", "AddedServiceSwitch", "0", "6", "U", "增值服务开关 0：关闭【不具有】 1：打开【具有】");

            result = AddNewSysCfg("0", "044", "VolunteerPay_Switch", "1", "6", "U", "志愿者兑换支付开关 0：关闭 1：开启");

            result = AddNewSysCfg("0", "045", "IsShowVmDiagnose", "1", "6", "U", "门关时是否显示机器诊断 0：不显示 1：显示");

            result = AddNewSysCfg("0", "046", "BillStackSwitch", "0", "6", "U", "纸币器缓存开关 0：关闭缓存 1：打开缓存");

            result = AddNewSysCfg("0", "047", "RemoteControlSwitch", "0", "6", "U", "是否接收远程控制指令 0：不接收 1：接收");
            result = AddNewSysCfg("0", "048", "RemoteListenPort", "6010", "6", "U", "远程控制指令端口");

            result = AddNewSysCfg("0", "049", "VmSoftApiUrl", "http://soft.kivend.net/krcs/", "6", "U", "终端远程更新API网址");
            result = AddNewSysCfg("0", "050", "VmSoftApiUserKey", "42ee413f51908b6f9c9bdfd801237756", "6", "U", "终端远程更新API的UserKey");

            result = AddNewSysCfg("0", "051", "OnlinePayUrl", "http://ngypay.kivend.net/AndriodAPI/", "6", "U", "电子支付网关API网址");
            result = AddNewSysCfg("0", "052", "OnlinePayToken", "KimmaAndriod201609", "6", "U", "电子支付网关API的UserKey");

            result = AddNewSysCfg("0", "053", "GoodsUploadType", "0", "6", "U", "商品更新策略 0：只允许本地更新 1：只允许远程更新 2：本地更新和远程更新皆可");

            result = AddNewSysCfg("0", "053", "PriceUploadType", "0", "6", "U", "价格设置策略 0：本地设置 1：远程更新");

            #endregion

            #endregion

            // 添加货道的临时测试
            //////AddAsileInfo();

            #region 添加新表

            // 添加金额销售数据表
            AddNewTable("T_VM_SALESTAT", "CREATE TABLE T_VM_SALESTAT(SaleDate NUMERIC,Cash_TotalMoney NUMERIC,Cash_SaleMoney NUMERIC,Cash_SaleNum NUMERIC,Other_TotalMoney NUMERIC,Other_SaleMoney NUMERIC,Other_SaleNum NUMERIC)");

            #region 添加国家信息配置表

            AddNewTable("T_SYS_Country", "CREATE TABLE T_SYS_Country(Code TEXT,Name_CN TEXT,Name_EN TEXT,DecimalNum NUMERIC,MinMoneyValue NUMERIC,MoneySymbol TEXT)");

            UpdateCountryInfoTable();

            AddNewCountryInfo("CHN", "中国", "Chinese", "100", "50", "¥");// 国家代码 国家名称中文 国家名称英文 小数点位数 最小面值货币 货币符号
            AddNewCountryInfo("SGP", "新加坡", "Singapore", "100", "10", "$");
            AddNewCountryInfo("MAL", "马来西亚", "Malaysia", "100", "10", "$");
            AddNewCountryInfo("FRA", "法国", "France", "100", "5000", "F");

            #endregion

            #region 添加支付方式信息表（主要是一卡通以及会员卡等可能不同运营商不同信息的支付方式）

            AddNewTable("T_SYS_PAYMENT", "CREATE TABLE T_SYS_PAYMENT(PayMentCode TEXT,Name_CN TEXT,Name_EN TEXT,Name_RUS TEXT)");
            AddNewPaymentInfo("IcCard", "一卡通", "POS", "POS");
            AddNewPaymentInfo("NoFeeCard", "会员卡", "Card", "Card");

            #endregion

            #region 添加升降机上下码盘配置表（2015-06-02）

            AddNewTable("T_VM_UPDOWN_CODENUM", "CREATE TABLE T_VM_UPDOWN_CODENUM(VendBoxCode TEXT,TrayNum TEXT,CodeNum TEXT)");
            //目前从上到下的格数分别为：267  234  201  168  135  101  68  35
            // 添加相关数据
            string strVendBoxCode = "1";
            for (int i = 1; i < 5; i++)
            {
                strVendBoxCode = i.ToString();
                AddNewUpDownCodeNumInfo(strVendBoxCode, 1, 267);
                AddNewUpDownCodeNumInfo(strVendBoxCode, 2, 234);
                AddNewUpDownCodeNumInfo(strVendBoxCode, 3, 201);
                AddNewUpDownCodeNumInfo(strVendBoxCode, 4, 168);
                AddNewUpDownCodeNumInfo(strVendBoxCode, 5, 135);
                AddNewUpDownCodeNumInfo(strVendBoxCode, 6, 101);
                AddNewUpDownCodeNumInfo(strVendBoxCode, 7, 68);
                AddNewUpDownCodeNumInfo(strVendBoxCode, 8, 35);
            }

            #endregion

            #region 添加商品类型信息表（2015-06-11）

            AddNewTable("T_MCD_TYPE", "CREATE TABLE T_MCD_TYPE(TypeCode TEXT,TypeName TEXT)");

            #endregion

            #region 添加货币库存信息表（2015-07-23）

            ////AddNewTable("T_VM_CASHSTOCK", "CREATE TABLE T_VM_CASHSTOCK(CashValue TEXT,StockNum TEXT,CashType TEXT)");
            ////AddCashStockInfo(100,0,"0");// 增加1元硬币库存
            ////AddCashStockInfo(50, 0, "0");// 增加5角硬币库存
            ////AddCashStockInfo(1000, 0, "1");// 增加10元纸币库存

            #endregion

            #region 添加货币信息表（2015-11-25）

            AddNewTable("T_VM_CASHINFO", "CREATE TABLE T_VM_CASHINFO(CashValue TEXT,StockNum TEXT,CashType TEXT,Channel TEXT,Status TEXT)");
            AddCashInfo(50, 0, "0", "2", "1");// 增加5角硬币信息
            AddCashInfo(100, 0, "0", "1", "1");// 增加1元硬币信息
            AddCashInfo(100, 0, "1", "1", "1");// 增加1元纸币信息
            AddCashInfo(500, 0, "1", "2", "1");// 增加5元纸币信息
            AddCashInfo(1000, 0, "1", "3", "1");// 增加10元纸币信息
            AddCashInfo(2000, 0, "1", "4", "1");// 增加20元纸币信息
            AddCashInfo(5000, 0, "1", "5", "0");// 增加50元纸币信息
            AddCashInfo(10000, 0, "1", "6", "0");// 增加100元纸币信息
            
            #endregion

            #region 添加广告信息表（2016-09-09）

            // 广告节目单基本信息表
            AddNewTable("T_ADV_ADVLIST", @"CREATE TABLE T_ADV_ADVLIST(AdvListID TEXT,UpdateSign TEXT,PlaySign TEXT,
                                            TotalNum TEXT,BeginDate TEXT,EndDate TEXT,Content TEXT,ImportType TEXT)");
            // 广告文件信息表
            AddNewTable("T_ADV_ADVFILE", @"CREATE TABLE T_ADV_ADVFILE(AdvListID TEXT,FileCode TEXT,FileName TEXT,
                                            FileFormat TEXT,FileType TEXT,FileSize TEXT,PlayNum TEXT,DelayTime TEXT,
                                            DownUrl TEXT,BeginTime TEXT,Status TEXT,ImportType TEXT,IsPlayTime TEXT,
                                            PlayTime1 TEXT,PlayTime2 TEXT)");

            // 修改广告文件信息表
            UpdateAdvFileTable("IsPlayTime");
            UpdateAdvFileTable("PlayTime1");
            UpdateAdvFileTable("PlayTime2");

            #endregion

            #region 添加商品图片更新中间表
            // T_MCD_PICTEMP
            // 商品编码、商品图片名称、下载地址、更新状态
            AddNewTable("T_MCD_PICTEMP", @"CREATE TABLE T_MCD_PICTEMP(McdCode TEXT,McdPic TEXT,Url TEXT,
                                            Status TEXT)");

            #endregion

            #endregion

            #region 修改表（2015-05-21 货道信息表）

            UpdatePaInfoTable("PaPostion");
            UpdatePaInfoTable("PiCi");
            UpdatePaInfoTable("ProductDate");
            UpdatePaInfoTable("MaxValidDate");// 有效期至（最后有效期）
            UpdatePaInfoTable("SaleNum");// 销售次数
            UpdatePaInfoTable("SaleMoney");// 销售金额
            UpdatePaInfoTable("SellModel");// 销售模式 0：正常销售 1：赠品
            UpdatePaInfoTable("McdSaleType");// 商品销售类型 0：正常销售 1：热点商品 2：新品 3：打折商品 4：免费商品

            #endregion

            #region 修改表（2015-06-09 商品信息表）

            UpdateGoodsInfoTable("IsFree");// 是否免费
            UpdateGoodsInfoTable("Manufacturer");// 生产厂商
            UpdateGoodsInfoTable("GoodsSpec");// 商品规格
            UpdateGoodsInfoTable("Unit");// 商品单位
            UpdateGoodsInfoTable("DrugType");// 商品标示
            UpdateGoodsInfoTable("DetailInfo");// 商品详细说明
            UpdateGoodsInfoTable("McdSaleType");// 商品销售类型

            #endregion

            #region 添加新表（2015-10-29 货柜信息表）

            AddNewTable("T_VM_VENDBOX", @"CREATE TABLE T_VM_VENDBOX(VendBoxCode TEXT,SellGoodsType TEXT,TmpControlModel TEXT,
                        TargetTmp TEXT,OutTmpWarnValue TEXT,ShippPort TEXT,IsControlCircle TEXT,UpDownSellModel TEXT,
                        UpDownDelayTimeNums TEXT,UpDownSendGoodsTimes TEXT,UpDownIsQueryElectStatus TEXT)");

            for (int i = 1; i < 5; i++)
            {
                AddNewVendBoxInfo(i.ToString());
            }

            UpdateVendBoxTable("TmpRunModel");// 温控运行模式 全开、全关等
            UpdateVendBoxTable("TmpBeginTime1");// 开启时间段1的开始时间
            UpdateVendBoxTable("TmpEndTime1");// 开启时间段1的结束时间
            UpdateVendBoxTable("TmpBeginTime2");// 开启时间段2的开始时间
            UpdateVendBoxTable("TmpEndTime2");// 开启时间段2的结束时间

            #endregion

            #region 修改表（2016-07-08 货币现金信息表）

            UpdateCashInfoTable("BoxStockNum");

            #endregion

            DbOper dbOper = new DbOper();
            dbOper.DbFileName = _m_DbFileName;

            try
            {
                string strSql = @"select CfgId,CfgName,CfgFactValue,CfgFactoryValue,CfgType,CfgLevel,IsReset 
                from T_SYS_CONFIG";
                DataSet dataSet = dbOper.dataSet(strSql);
                if (dataSet.Tables.Count > 0)
                {
                    int recordCount = dataSet.Tables[0].Rows.Count;
                    for (int i = 0; i < recordCount; i++)
                    {
                        SysCfgList.Add(new SysCfgInfoModel()
                        {
                            CfgId = dataSet.Tables[0].Rows[i]["CfgId"].ToString(),
                            CfgName = dataSet.Tables[0].Rows[i]["CfgName"].ToString(),
                            CfgFactValue = dataSet.Tables[0].Rows[i]["CfgFactValue"].ToString(),
                            CfgFactoryValue = dataSet.Tables[0].Rows[i]["CfgFactoryValue"].ToString(),
                            CfgType = dataSet.Tables[0].Rows[i]["CfgType"].ToString(),
                            CfgLevel = dataSet.Tables[0].Rows[i]["CfgLevel"].ToString(),
                            IsReset = dataSet.Tables[0].Rows[i]["IsReset"].ToString()
                        });
                    }
                }

                #region 获取国家信息

                strSql = @"select Code,Name_CN,Name_EN,DecimalNum,MinMoneyValue,MoneySymbol from T_SYS_Country ";
                dataSet = dbOper.dataSet(strSql);
                string strDecimalNum = "100";
                string strMinMoneyValue = "50";
                string strMoneySymbol = "¥";
                string strCountryCode = GetSysCfgValue("CountryCode");
                if (dataSet.Tables.Count > 0)
                {
                    int countryCount = dataSet.Tables[0].Rows.Count;
                    if (countryCount > 0)
                    {
                        string strCode = string.Empty;
                        for (int i = 0; i < countryCount; i++)
                        {
                            if (strCode == strCountryCode)
                            {
                                strDecimalNum = dataSet.Tables[0].Rows[i]["DecimalNum"].ToString();
                                strMinMoneyValue = dataSet.Tables[0].Rows[i]["MinMoneyValue"].ToString();
                                strMoneySymbol = dataSet.Tables[0].Rows[i]["MoneySymbol"].ToString();
                            }
                            CountryList.Add(new CountryModel()
                            {
                                CountryCode = dataSet.Tables[0].Rows[i]["Code"].ToString(),
                                CountryName_ZN = dataSet.Tables[0].Rows[i]["Name_CN"].ToString(),
                                CountryName_EN = dataSet.Tables[0].Rows[i]["Name_EN"].ToString(),
                                DecimalNum = dataSet.Tables[0].Rows[i]["DecimalNum"].ToString(),
                                MinMoneyValue = dataSet.Tables[0].Rows[i]["MinMoneyValue"].ToString(),
                                MoneySymbol = dataSet.Tables[0].Rows[i]["MoneySymbol"].ToString(),
                            });
                        }
                    }
                }
                SysCfgList.Add(new SysCfgInfoModel()
                {
                    CfgId = "DecimalNum",
                    CfgName = "DecimalNum",
                    CfgFactValue = strDecimalNum,
                    CfgFactoryValue = strDecimalNum,
                    CfgType = "0",
                    CfgLevel = "S"
                });
                SysCfgList.Add(new SysCfgInfoModel()
                {
                    CfgId = "MinMoneyValue",
                    CfgName = "MinMoneyValue",
                    CfgFactValue = strMinMoneyValue,
                    CfgFactoryValue = strMinMoneyValue,
                    CfgType = "0",
                    CfgLevel = "S"
                });
                SysCfgList.Add(new SysCfgInfoModel()
                {
                    CfgId = "MoneySymbol",
                    CfgName = "MoneySymbol",
                    CfgFactValue = strMoneySymbol,
                    CfgFactoryValue = strMoneySymbol,
                    CfgType = "0",
                    CfgLevel = "S"
                });

                #endregion

                LoadSysCfg_Third();

                result = true;
            }
            catch
            {
                result = false;
            }
            finally
            {
                dbOper.closeConnection();

                if (SysCfgList.Count < 1)
                {
                    result = false;
                }
            }
            return result;
        }

        /// <summary>
        /// 获取某系统配置参数的实际值
        /// </summary>
        /// <param name="sysCfgName">系统配置参数名称</param>
        /// <returns>系统配置参数实际值</returns>
        public string GetSysCfgValue(string sysCfgName)
        {
            string strCfgValue = "0";// string.Empty;

            int intSysCfgCount = SysCfgList.Count;
            for (int i = 0; i < intSysCfgCount; i++)
            {
                if (SysCfgList[i].CfgName == sysCfgName)
                {
                    strCfgValue = SysCfgList[i].CfgFactValue;
                    break;
                }
            }

            return strCfgValue;
        }

        /// <summary>
        /// 更新系统配置参数值
        /// </summary>
        /// <param name="cfgName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool UpdateSysCfg(string cfgName,string value)
        {
            bool result = false;
            try
            {
                string strSql = "update T_SYS_CONFIG set [CfgFactValue] = '" + value + "' where [CfgName]='" + cfgName + "'";

                DbOper dbOper = new DbOper();
                dbOper.DbFileName = _m_DbFileName;
                dbOper.ConnType = ConnectType.CloseConn;
                result = dbOper.excuteSql(strSql);
                dbOper.closeConnection();
            }
            catch
            {
                result = false;
            }

            return result;
        }      

        public DataSet GetCountryList()
        {
            DbOper dbOper = new DbOper();
            dbOper.DbFileName = _m_DbFileName;

            DataSet dataSet = new DataSet();
            try
            {
                string strSql = @"select Code,Name_CN,Name_EN,DecimalNum,MinMoneyValue from T_SYS_Country ";
                dataSet = dbOper.dataSet(strSql);
            }
            catch
            {

            }
            return dataSet;
        }

        public void GetCountryInfo(string countryCode)
        {
            CountryInfo.DecimalNum = "100";
            CountryInfo.MinMoneyValue = "50";
            for (int i = 0; i < CountryList.Count; i++)
            {
                if (CountryList[i].CountryCode == countryCode)
                {
                    CountryInfo.CountryCode = countryCode;
                    CountryInfo.DecimalNum = CountryList[i].DecimalNum;
                    CountryInfo.MinMoneyValue = CountryList[i].MinMoneyValue;
                    CountryInfo.MoneySymbol = CountryList[i].MoneySymbol;
                }
            }
        }

        #region 第三方配置参数处理

        private bool LoadSysCfg_Third()
        {
            bool result = false;

            SysCfgList_Third.Clear();

            DbOper dbOper = new DbOper();
            dbOper.DbFileName = m_DbFileName_Third;

            try
            {
                string strSql = @"select CfgId,CfgName,CfgFactValue,CfgFactoryValue,CfgType,CfgLevel,IsReset 
                    from T_SYS_CONFIG";
                DataSet dataSet = dbOper.dataSet(strSql);
                if (dataSet.Tables.Count > 0)
                {
                    int recordCount = dataSet.Tables[0].Rows.Count;
                    for (int i = 0; i < recordCount; i++)
                    {
                        SysCfgList_Third.Add(new SysCfgInfoModel()
                        {
                            CfgId = dataSet.Tables[0].Rows[i]["CfgId"].ToString(),
                            CfgName = dataSet.Tables[0].Rows[i]["CfgName"].ToString(),
                            CfgFactValue = dataSet.Tables[0].Rows[i]["CfgFactValue"].ToString(),
                            CfgFactoryValue = dataSet.Tables[0].Rows[i]["CfgFactoryValue"].ToString(),
                            CfgType = dataSet.Tables[0].Rows[i]["CfgType"].ToString(),
                            CfgLevel = dataSet.Tables[0].Rows[i]["CfgLevel"].ToString(),
                            IsReset = dataSet.Tables[0].Rows[i]["IsReset"].ToString()
                        });
                    }
                }

                result = true;
            }
            catch
            {
                result = false;
            }
            finally
            {
                dbOper.closeConnection();

                if (SysCfgList_Third.Count < 1)
                {
                    result = false;
                }
            }
            return result;
        }

        /// <summary>
        /// 获取第三方系统配置参数的实际值
        /// </summary>
        /// <param name="sysCfgName">系统配置参数名称</param>
        /// <returns>系统配置参数实际值</returns>
        public string GetSysCfgValue_Third(string sysCfgName)
        {
            string strCfgValue = "0";// string.Empty;

            int intSysCfgCount = SysCfgList_Third.Count;
            for (int i = 0; i < intSysCfgCount; i++)
            {
                if (SysCfgList_Third[i].CfgName == sysCfgName)
                {
                    strCfgValue = SysCfgList_Third[i].CfgFactValue;
                    break;
                }
            }

            return strCfgValue;
        }

        /// <summary>
        /// 更新第三方系统配置参数值
        /// </summary>
        /// <param name="cfgName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool UpdateSysCfg_Third(string cfgName, string value)
        {
            bool result = false;
            try
            {
                string strSql = "update T_SYS_CONFIG set [CfgFactValue] = '" + value + "' where [CfgName]='" + cfgName + "'";

                DbOper dbOper = new DbOper();
                dbOper.DbFileName = m_DbFileName_Third;
                dbOper.ConnType = ConnectType.CloseConn;
                result = dbOper.excuteSql(strSql);
                dbOper.closeConnection();

                if (result)
                {
                    for (int i = 0; i < SysCfgList_Third.Count; i++)
                    {
                        if (SysCfgList_Third[i].CfgName == cfgName)
                        {
                            SysCfgList_Third[i].CfgFactValue = value;
                            break;
                        }
                    }
                }
            }
            catch
            {
                result = false;
            }

            return result;
        }      

        #endregion

        #endregion

        #region 私有函数

        /// <summary>
        /// 添加货道—测试
        /// </summary>
        /// <returns></returns>
        private bool AddAsileInfo()
        {
            DbOper dbOper = new DbOper();
            dbOper.DbFileName = _m_DbFileName;
            dbOper.ConnType = ConnectType.KeepConn;

            string strSql = string.Empty;
            bool result = false;
            string strfdsa = string.Empty;

            for(int i = 1;i < 7;i++)
            {
                for(int j = 1;j < 10;j++)
                {
                    strfdsa = i.ToString() + j.ToString();
                    strSql = @"insert into t_vm_painfo(traynum,Isnew,Pacode_num,pacode_str,springnum,
                                vendboxcode,paid,pastatus,pakind,surnum,pastacknum,sellprice,pacode,papostion,salenum,salemoney)
                        values(" + i + ",'0','" + strfdsa + "','" + strfdsa + "',5,'2','" + strfdsa + "','02','0',5,0,100,'" + strfdsa + "'," + strfdsa + ",0,0)";
                    result = dbOper.excuteSql(strSql);
                }
            }
            dbOper.closeConnection();

            return true;
        }

        /// <summary>
        /// 添加新的系统配置参数
        /// </summary>
        /// <param name="isReset"></param>
        /// <param name="cfgId"></param>
        /// <param name="cfgName"></param>
        /// <param name="cfgValue"></param>
        /// <param name="cfgType"></param>
        /// <param name="cfgLevel"></param>
        /// <param name="cfgBase"></param>
        /// <returns></returns>
        private bool AddNewSysCfg(string isReset, string cfgId, string cfgName, string cfgValue,
            string cfgType,string cfgLevel,string cfgBase)
        {
            bool result = false;

            string strSql = string.Empty;

            try
            {
                DbOper dbOper = new DbOper();
                dbOper.DbFileName = _m_DbFileName;
                dbOper.ConnType = ConnectType.CloseConn;

                bool blnIsExist = false;
                strSql = @"select CfgId
                from T_SYS_CONFIG where CfgId = '" + cfgId + "' and CfgName = '" + cfgName + "'";
                DataSet dataSet = dbOper.dataSet(strSql);
                if (dataSet.Tables.Count > 0)
                {
                    if (dataSet.Tables[0].Rows.Count > 0)
                    {
                        // 该参数已经存在
                        blnIsExist = true;
                        result = true;
                    }
                }
                if (!blnIsExist)
                {
                    strSql = @"insert into T_SYS_CONFIG(IsReset,CfgId,CfgName,CfgFactValue,CfgFactoryValue,CfgType,CfgLevel,Base) 
            values('" + isReset + "','" + cfgId + "','" + cfgName + "','" + cfgValue +
                              "','" + cfgValue + "','" + cfgType + "','" + cfgLevel + "','" + cfgBase + "')";
                    result = dbOper.excuteSql(strSql);
                }

                dbOper.closeConnection();
            }
            catch
            {
                result = false;
            }

            return result;
        }

        private bool AddNewCountryInfo(string code, string nameCN, string nameEN, string decimalNum,string minMoneyValue,string moneySymbol)
        {
            bool result = false;

            string strSql = string.Empty;

            try
            {
                DbOper dbOper = new DbOper();
                dbOper.DbFileName = _m_DbFileName;
                dbOper.ConnType = ConnectType.CloseConn;

                bool blnIsExist = false;
                strSql = @"select Code
                from T_SYS_Country where Code = '" + code + "'";
                DataSet dataSet = dbOper.dataSet(strSql);
                if (dataSet.Tables.Count > 0)
                {
                    if (dataSet.Tables[0].Rows.Count > 0)
                    {
                        // 该参数已经存在
                        blnIsExist = true;
                        result = true;

                        // 更新金额符号的值
                        strSql = "update T_SYS_Country set MoneySymbol = '" + moneySymbol + "' where Code = '" + code + "'";
                        result = dbOper.excuteSql(strSql);
                    }
                }
                if (!blnIsExist)
                {
                    strSql = @"insert into T_SYS_Country(Code,Name_CN,Name_EN,DecimalNum,MinMoneyValue,MoneySymbol) 
            values('" + code + "','" + nameCN + "','" + nameEN + "','" + decimalNum +
                              "','" + minMoneyValue + "','" + moneySymbol + "')";
                    result = dbOper.excuteSql(strSql);
                }

                dbOper.closeConnection();
            }
            catch
            {
                result = false;
            }

            return result;
        }

        private bool AddNewPaymentInfo(string code, string nameCN, string nameEN, string nameRUS)
        {
            bool result = false;

            string strSql = string.Empty;

            try
            {
                DbOper dbOper = new DbOper();
                dbOper.DbFileName = _m_DbFileName;
                dbOper.ConnType = ConnectType.CloseConn;

                // AddNewTable("T_SYS_PAYMENT", "CREATE TABLE T_SYS_PAYMENT(PayMentCode TEXT,Name_CN TEXT,Name_EN TEXT,Name_RUS TEXT)");
                bool blnIsExist = false;
                strSql = @"select PayMentCode
                from T_SYS_PAYMENT where PayMentCode = '" + code + "'";
                DataSet dataSet = dbOper.dataSet(strSql);
                if (dataSet.Tables.Count > 0)
                {
                    if (dataSet.Tables[0].Rows.Count > 0)
                    {
                        // 该参数已经存在
                        blnIsExist = true;
                        result = true;
                    }
                }
                if (!blnIsExist)
                {
                    strSql = @"insert into T_SYS_PAYMENT(PayMentCode,Name_CN,Name_EN,Name_RUS) 
            values('" + code + "','" + nameCN + "','" + nameEN + "','" + nameRUS + "')";
                    result = dbOper.excuteSql(strSql);
                }

                dbOper.closeConnection();
            }
            catch
            {
                result = false;
            }

            return result;
        }

        private bool AddCashInfo(int cashValue, int stockNum, string cashType, string channel, string status)
        {
            bool result = false;

            string strSql = string.Empty;

            try
            {
                DbOper dbOper = new DbOper();
                dbOper.DbFileName = _m_DbFileName;
                dbOper.ConnType = ConnectType.CloseConn;

                bool blnIsExist = false;
                strSql = @"select CashValue
                from T_VM_CASHINFO where CashValue = '" + cashValue + "' and CashType = '" + cashType + "'";
                DataSet dataSet = dbOper.dataSet(strSql);
                if (dataSet.Tables.Count > 0)
                {
                    if (dataSet.Tables[0].Rows.Count > 0)
                    {
                        // 该参数已经存在
                        blnIsExist = true;
                        result = true;
                    }
                }
                if (!blnIsExist)
                {
                    strSql = @"insert into T_VM_CASHINFO(CashValue,StockNum,CashType,Channel,Status,BoxStockNum) 
            values('" + cashValue + "','" + stockNum + "','" + cashType + "','" + channel + "','" + status + "','0')";
                    result = dbOper.excuteSql(strSql);
                }

                dbOper.closeConnection();
            }
            catch
            {
                result = false;
            }

            return result;
        }

        private bool AddNewVendBoxInfo(string vendBoxCode)
        {
            bool result = false;

            string strSql = string.Empty;

            try
            {
                DbOper dbOper = new DbOper();
                dbOper.DbFileName = _m_DbFileName;
                dbOper.ConnType = ConnectType.CloseConn;

                bool blnIsExist = false;
                strSql = @"select VendBoxCode from T_VM_VENDBOX where VendBoxCode = '" + vendBoxCode + "'";
                DataSet dataSet = dbOper.dataSet(strSql);
                if (dataSet.Tables.Count > 0)
                {
                    if (dataSet.Tables[0].Rows.Count > 0)
                    {
                        // 该参数已经存在
                        blnIsExist = true;
                        result = true;
                    }
                }
                if (!blnIsExist)
                {
                    strSql = @"insert into T_VM_VENDBOX(VendBoxCode,SellGoodsType,TmpControlModel,TargetTmp,
                    OutTmpWarnValue,ShippPort,IsControlCircle,UpDownSellModel,
                    UpDownDelayTimeNums,UpDownSendGoodsTimes,UpDownIsQueryElectStatus) 
                            values('" + vendBoxCode + "','0','0','6','18','3','0','1','150','30','1')";
                    result = dbOper.excuteSql(strSql);
                }

                dbOper.closeConnection();
            }
            catch
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// 添加升降机上下移动码盘配置数据
        /// </summary>
        /// <param name="vendBoxCode"></param>
        /// <param name="trayNum"></param>
        /// <param name="codeNum"></param>
        /// <returns></returns>
        private bool AddNewUpDownCodeNumInfo(string vendBoxCode, int trayNum, int codeNum)
        {
            bool result = false;

            string strSql = string.Empty;

            try
            {
                DbOper dbOper = new DbOper();
                dbOper.DbFileName = _m_DbFileName;
                dbOper.ConnType = ConnectType.CloseConn;

                bool blnIsExist = false;
                strSql = @"select VendBoxCode
                from T_VM_UPDOWN_CODENUM where VendBoxCode = '" + vendBoxCode + "' and TrayNum = " + trayNum;
                DataSet dataSet = dbOper.dataSet(strSql);
                if (dataSet.Tables.Count > 0)
                {
                    if (dataSet.Tables[0].Rows.Count > 0)
                    {
                        // 该参数已经存在
                        blnIsExist = true;
                        result = true;
                    }
                }
                if (!blnIsExist)
                {
                    strSql = @"insert into T_VM_UPDOWN_CODENUM(VendBoxCode,TrayNum,CodeNum) 
                        values('" + vendBoxCode + "'," + trayNum + "," + codeNum + ")";
                    result = dbOper.excuteSql(strSql);
                }

                dbOper.closeConnection();
            }
            catch
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// 添加新表
        /// </summary>
        /// <param name="createSql"></param>
        /// <returns></returns>
        private bool AddNewTable(string tableName,string createSql)
        {
            bool result = false;

            string strSql = string.Empty;

            try
            {
                DbOper dbOper = new DbOper();
                dbOper.DbFileName = _m_DbFileName;
                dbOper.ConnType = ConnectType.CloseConn;

                bool blnIsExist = false;
                strSql = @"select name from sqlite_master where type = 'table' and name= '" + tableName + "'";
                DataSet dataSet = dbOper.dataSet(strSql);
                if (dataSet.Tables.Count > 0)
                {
                    if (dataSet.Tables[0].Rows.Count > 0)
                    {
                        // 该表已经存在
                        blnIsExist = true;
                        result = true;
                    }
                }
                if (!blnIsExist)
                {
                    result = dbOper.excuteSql(createSql);
                }

                dbOper.closeConnection();
            }
            catch
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// 修改货道信息表
        /// </summary>
        /// <returns></returns>
        private bool UpdatePaInfoTable(string columnName)
        {
            bool result = false;
            string strSql = string.Empty;

            try
            {
                DbOper dbOper = new DbOper();
                dbOper.DbFileName = _m_DbFileName;
                dbOper.ConnType = ConnectType.CloseConn;

                bool blnIsExist = false;

                #region 检测是否存在字段

                try
                {
                    switch (columnName)
                    {
                        case "PaPostion":// 货道位置编号
                            strSql = "Update t_vm_painfo set PaPostion = 11 where pacode = 1";
                            break;
                        case "PiCi":// 生产批次
                            strSql = "Update t_vm_painfo set PiCi = '' where pacode = 1";
                            break;
                        case "ProductDate":// 生产日期
                            strSql = "Update t_vm_painfo set ProductDate = '' where pacode = 1";
                            break;
                        case "MaxValidDate":// 最大有效期
                            strSql = "Update t_vm_painfo set MaxValidDate = '' where pacode = 1";
                            break;
                        case "SaleNum":// 销售次数
                            strSql = "Update t_vm_painfo set SaleNum = 0 where pacode = 1";
                            break;
                        case "SaleMoney":// 销售金额
                            strSql = "Update t_vm_painfo set SaleMoney = 0 where pacode = 1";
                            break;
                        case "SellModel":// 销售模式
                            strSql = "Update t_vm_painfo set SellModel = 0 where pacode = 1";
                            break;
                        case "McdSaleType":// 商品销售类型
                            strSql = "Update t_vm_painfo set McdSaleType = 0 where pacode = 1";
                            break;
                    }

                    result = dbOper.excuteSql(strSql);

                    blnIsExist = result;
                }
                catch
                {
                    blnIsExist = false;
                }

                #endregion

                if (!blnIsExist)
                {
                    // 不存在字段
                    switch (columnName)
                    {
                        case "PaPostion":// 货道位置编号
                            strSql = "ALTER TABLE `t_vm_painfo` ADD `PaPostion` VARCHAR( 10 ) ;";
                            break;
                        case "PiCi":// 生产批次
                            strSql = "ALTER TABLE `t_vm_painfo` ADD `PiCi` VARCHAR( 20 ) ;";
                            break;
                        case "ProductDate":// 生产日期
                            strSql = "ALTER TABLE `t_vm_painfo` ADD `ProductDate` VARCHAR( 20 ) ;";
                            break;
                        case "MaxValidDate":// 最大有效期
                            strSql = "ALTER TABLE `t_vm_painfo` ADD `MaxValidDate` VARCHAR( 20 ) ;";
                            break;
                        case "SaleNum":// 销售次数
                            strSql = "ALTER TABLE `t_vm_painfo` ADD `SaleNum` NUMERIC( 8 ) ;";
                            break;
                        case "SaleMoney":// 销售金额
                            strSql = "ALTER TABLE `t_vm_painfo` ADD `SaleMoney` NUMERIC( 10 ) ;";
                            break;
                        case "SellModel":// 销售模式
                            strSql = "ALTER TABLE `t_vm_painfo` ADD `SellModel` VARCHAR( 3 ) ;";
                            break;
                        case "McdSaleType":// 商品销售类型
                            strSql = "ALTER TABLE `t_vm_painfo` ADD `McdSaleType` VARCHAR( 3 ) ;";
                            break;
                    }
                    
                    result = dbOper.excuteSql(strSql);
                    if (result)
                    {
                        // 添加字段成功，更新该字段的值
                        switch (columnName)
                        {
                            case "PaPostion":// 货道位置编号
                                strSql = "Update t_vm_painfo set PaPostion = PaId";
                                break;
                            case "PiCi":// 生产批次
                                strSql = "Update t_vm_painfo set PiCi = ''";
                                break;
                            case "ProductDate":// 生产日期
                                strSql = "Update t_vm_painfo set ProductDate = ''";
                                break;
                            case "MaxValidDate":// 最大有效期
                                strSql = "Update t_vm_painfo set MaxValidDate = ''";
                                break;
                            case "SaleNum":// 销售次数
                                strSql = "Update t_vm_painfo set SaleNum = 0";
                                break;
                            case "SaleMoney":// 销售金额
                                strSql = "Update t_vm_painfo set SaleMoney = 0";
                                break;
                            case "SellModel":// 销售模式
                                strSql = "Update t_vm_painfo set SellModel = '0'";
                                break;
                            case "McdSaleType":// 商品销售类型
                                strSql = "Update t_vm_painfo set McdSaleType = '0'";
                                break;
                        }
                        
                        result = dbOper.excuteSql(strSql);
                    }
                }

                dbOper.closeConnection();
            }
            catch
            {

            }

            return result;
        }

        /// <summary>
        /// 修改国家信息表
        /// </summary>
        /// <returns></returns>
        private bool UpdateCountryInfoTable()
        {
            bool result = false;
            string strSql = string.Empty;

            try
            {
                DbOper dbOper = new DbOper();
                dbOper.DbFileName = _m_DbFileName;
                dbOper.ConnType = ConnectType.CloseConn;

                bool blnIsExist = false;

                #region 检测是否存在字段MoneySymbol（国家货币符号）

                try
                {
                    strSql = "Update T_SYS_Country set MoneySymbol = '' where Code = '1'";

                    result = dbOper.excuteSql(strSql);

                    blnIsExist = result;
                }
                catch
                {
                    blnIsExist = false;
                }

                #endregion

                if (!blnIsExist)
                {
                    // 不存在字段MoneySymbol
                    strSql = "ALTER TABLE `T_SYS_Country` ADD `MoneySymbol` VARCHAR( 10 ) ;";
                    result = dbOper.excuteSql(strSql);
                }

                dbOper.closeConnection();
            }
            catch
            {

            }

            return result;
        }

        /// <summary>
        /// 修改商品信息表
        /// </summary>
        /// <returns></returns>
        private bool UpdateGoodsInfoTable(string columnName)
        {
            bool result = false;
            string strSql = string.Empty;

            try
            {
                DbOper dbOper = new DbOper();
                dbOper.DbFileName = _m_DbFileName;
                dbOper.ConnType = ConnectType.CloseConn;

                bool blnIsExist = false;

                #region 检测是否存在字段IsFree（是否免费）

                try
                {
                    switch (columnName)
                    {
                        case "IsFree":// 是否免费 0：不免费 1：免费
                            strSql = "Update t_mcd_baseinfo set IsFree = 1 where mcdcode = ''";
                            break;
                        case "Manufacturer":// 生产厂商
                            strSql = "Update t_mcd_baseinfo set Manufacturer = '' where mcdcode = ''";
                            break;
                        case "GoodsSpec":// 商品规格
                            strSql = "Update t_mcd_baseinfo set GoodsSpec = '' where mcdcode = ''";
                            break;
                        case "Unit":// 商品单位
                            strSql = "Update t_mcd_baseinfo set Unit = '' where mcdcode = ''";
                            break;
                        case "DrugType":// 商品标示
                            strSql = "Update t_mcd_baseinfo set DrugType = '0' where mcdcode = ''";
                            break;
                        case "DetailInfo":// 商品详细说明
                            strSql = "Update t_mcd_baseinfo set DetailInfo = '' where mcdcode = ''";
                            break;
                        case "McdSaleType":// 商品销售类型
                            strSql = "Update t_mcd_baseinfo set McdSaleType = '' where mcdcode = ''";
                            break;
                    }

                    result = dbOper.excuteSql(strSql);

                    blnIsExist = result;
                }
                catch
                {
                    blnIsExist = false;
                }

                #endregion

                if (!blnIsExist)
                {
                    // 不存在字段
                    switch (columnName)
                    {
                        case "IsFree":// 是否免费 0：不免费 1：免费
                            strSql = "ALTER TABLE `t_mcd_baseinfo` ADD `IsFree` VARCHAR( 2 ) ;";
                            break;
                        case "Manufacturer":// 生产厂商
                            strSql = "ALTER TABLE `t_mcd_baseinfo` ADD `Manufacturer` VARCHAR( 30 ) ;";
                            break;
                        case "GoodsSpec":// 商品规格
                            strSql = "ALTER TABLE `t_mcd_baseinfo` ADD `GoodsSpec` VARCHAR( 30 ) ;";
                            break;
                        case "Unit":// 商品单位
                            strSql = "ALTER TABLE `t_mcd_baseinfo` ADD `Unit` VARCHAR( 30 ) ;";
                            break;
                        case "DrugType":// 商品标示
                            strSql = "ALTER TABLE `t_mcd_baseinfo` ADD `DrugType` VARCHAR( 3 ) ;";
                            break;
                        case "DetailInfo":// 商品详细说明
                            strSql = "ALTER TABLE `t_mcd_baseinfo` ADD `DetailInfo` VARCHAR( 1200 ) ;";
                            break;
                        case "McdSaleType":// 商品销售类型
                            strSql = "ALTER TABLE `t_mcd_baseinfo` ADD `McdSaleType` VARCHAR( 2 ) ;";
                            break;
                    }
                    
                    result = dbOper.excuteSql(strSql);
                    if (result)
                    {
                        // 添加字段成功，更新该字段的值
                        switch (columnName)
                        {
                            case "IsFree":// 是否免费 0：不免费 1：免费
                                strSql = "Update t_mcd_baseinfo set IsFree = '0'";
                                break;
                            case "Manufacturer":// 生产厂商
                                strSql = "Update t_mcd_baseinfo set Manufacturer = ''";
                                break;
                            case "GoodsSpec":// 商品规格
                                strSql = "Update t_mcd_baseinfo set GoodsSpec = ''";
                                break;
                            case "Unit":// 商品单位
                                strSql = "Update t_mcd_baseinfo set Unit = ''";
                                break;
                            case "DrugType":// 商品单位
                                strSql = "Update t_mcd_baseinfo set DrugType = '0'";
                                break;
                            case "DetailInfo":// 商品详细说明
                                strSql = "Update t_mcd_baseinfo set DetailInfo = ''";
                                break;
                            case "McdSaleType":// 商品销售类型
                                strSql = "Update t_mcd_baseinfo set McdSaleType = '0'";
                                break;
                        }
                        
                        result = dbOper.excuteSql(strSql);
                    }
                }

                dbOper.closeConnection();
            }
            catch
            {

            }

            return result;
        }

        /// <summary>
        /// 修改售货机货柜信息表
        /// </summary>
        /// <returns></returns>
        private bool UpdateVendBoxTable(string columnName)
        {
            bool result = false;
            string strSql = string.Empty;

            try
            {
                DbOper dbOper = new DbOper();
                dbOper.DbFileName = _m_DbFileName;
                dbOper.ConnType = ConnectType.CloseConn;

                bool blnIsExist = false;

                #region 检测是否存在字段

                try
                {
                    switch (columnName)
                    {
                        case "TmpRunModel":// 温控方式 全开、全关等
                            strSql = "Update T_VM_VENDBOX set TmpRunModel = '2' where VendBoxCode = ''";
                            break;
                        case "TargetTmp":// 目标温度
                            strSql = "Update T_VM_VENDBOX set TargetTmp = '6' where VendBoxCode = ''";
                            break;
                        case "TmpBeginTime1":// 温控开启时间段1的开始时间
                            strSql = "Update T_VM_VENDBOX set TmpBeginTime1 = '0800' where VendBoxCode = ''";
                            break;
                        case "TmpEndTime1":// 温控开启时间段1的结束时间
                            strSql = "Update T_VM_VENDBOX set TmpEndTime1 = '2000' where VendBoxCode = ''";
                            break;
                        case "TmpBeginTime2":// 开启时间段2的开始时间
                            strSql = "Update T_VM_VENDBOX set TmpBeginTime2 = '' where VendBoxCode = ''";
                            break;
                        case "TmpEndTime2":// 开启时间段2的结束时间
                            strSql = "Update T_VM_VENDBOX set TmpEndTime2 = '' where VendBoxCode = ''";
                            break;
                    }

                    result = dbOper.excuteSql(strSql);

                    blnIsExist = result;
                }
                catch
                {
                    blnIsExist = false;
                }

                #endregion

                if (!blnIsExist)
                {
                    // 不存在字段
                    switch (columnName)
                    {
                        case "TmpRunModel":// 温控方式 全开、全关等
                            strSql = "ALTER TABLE `T_VM_VENDBOX` ADD `TmpRunModel` VARCHAR( 2 ) ;";
                            break;
                        case "TargetTmp":// 目标温度
                            strSql = "ALTER TABLE `T_VM_VENDBOX` ADD `TargetTmp` VARCHAR( 10 ) ;";
                            break;
                        case "TmpBeginTime1":// 温控开启时间段1的开始时间
                            strSql = "ALTER TABLE `T_VM_VENDBOX` ADD `TmpBeginTime1` VARCHAR( 10 ) ;";
                            break;
                        case "TmpEndTime1":// 温控开启时间段1的结束时间
                            strSql = "ALTER TABLE `T_VM_VENDBOX` ADD `TmpEndTime1` VARCHAR( 10 ) ;";
                            break;
                        case "TmpBeginTime2":// 温控开启时间段2的开始时间
                            strSql = "ALTER TABLE `T_VM_VENDBOX` ADD `TmpBeginTime2` VARCHAR( 10 ) ;";
                            break;
                        case "TmpEndTime2":// 温控开启时间段2的结束时间
                            strSql = "ALTER TABLE `T_VM_VENDBOX` ADD `TmpEndTime2` VARCHAR( 10 ) ;";
                            break;
                    }

                    result = dbOper.excuteSql(strSql);
                    if (result)
                    {
                        // 添加字段成功，更新该字段的值
                        switch (columnName)
                        {
                            case "TmpRunModel":// 温控方式 全开、全关等
                                strSql = "Update T_VM_VENDBOX set TmpRunModel = '2'";
                                break;
                            case "TargetTmp":// 目标温度
                                strSql = "Update T_VM_VENDBOX set TargetTmp = '8'";
                                break;
                            case "TmpBeginTime1":// 温控开启时间段1的开始时间
                                strSql = "Update T_VM_VENDBOX set TmpBeginTime1 = '0800'";
                                break;
                            case "TmpEndTime1":// 温控开启时间段1的结束时间
                                strSql = "Update T_VM_VENDBOX set TmpEndTime1 = '2000'";
                                break;
                            case "TmpBeginTime2":// 温控开启时间段2的开始时间
                                strSql = "Update T_VM_VENDBOX set TmpBeginTime2 = ''";
                                break;
                            case "TmpEndTime2":// 温控开启时间段2的结束时间
                                strSql = "Update T_VM_VENDBOX set TmpEndTime2 = ''";
                                break;
                        }

                        result = dbOper.excuteSql(strSql);
                    }
                }

                dbOper.closeConnection();
            }
            catch
            {

            }

            return result;
        }

        /// <summary>
        /// 修改货币现金信息表
        /// </summary>
        /// <returns></returns>
        private bool UpdateCashInfoTable(string columnName)
        {
            bool result = false;
            string strSql = string.Empty;

            try
            {
                DbOper dbOper = new DbOper();
                dbOper.DbFileName = _m_DbFileName;
                dbOper.ConnType = ConnectType.CloseConn;

                bool blnIsExist = false;

                #region 检测是否存在字段

                try
                {
                    switch (columnName)
                    {
                        case "BoxStockNum":// 溢币盒库存量
                            strSql = "Update T_VM_CASHINFO set BoxStockNum = '0' where BoxStockNum = '0'";
                            break;
                    }

                    result = dbOper.excuteSql(strSql);

                    blnIsExist = result;
                }
                catch
                {
                    blnIsExist = false;
                }

                #endregion

                if (!blnIsExist)
                {
                    // 不存在字段
                    switch (columnName)
                    {
                        case "BoxStockNum":// 溢币盒库存量
                            strSql = "ALTER TABLE `T_VM_CASHINFO` ADD `BoxStockNum` VARCHAR( 20 ) ;";
                            break;
                    }

                    result = dbOper.excuteSql(strSql);
                    if (result)
                    {
                        // 添加字段成功，更新该字段的值
                        switch (columnName)
                        {
                            case "BoxStockNum":// 溢币盒库存量
                                strSql = "Update T_VM_CASHINFO set BoxStockNum = '0'";
                                break;
                        }

                        result = dbOper.excuteSql(strSql);
                    }
                }

                dbOper.closeConnection();
            }
            catch
            {

            }

            return result;
        }

        /// <summary>
        /// 修改广告文件信息表
        /// </summary>
        /// <returns></returns>
        private bool UpdateAdvFileTable(string columnName)
        {
            bool result = false;
            string strSql = string.Empty;

            // 广告文件信息表
            AddNewTable("T_ADV_ADVFILE", @"CREATE TABLE T_ADV_ADVFILE(AdvListID TEXT,FileCode TEXT,FileName TEXT,
                                            FileFormat TEXT,FileType TEXT,FileSize TEXT,PlayNum TEXT,DelayTime TEXT,
                                            DownUrl TEXT,BeginTime TEXT,Status TEXT,ImportType TEXT)");

            try
            {
                DbOper dbOper = new DbOper();
                dbOper.DbFileName = _m_DbFileName;
                dbOper.ConnType = ConnectType.CloseConn;

                bool blnIsExist = false;

                #region 检测是否存在字段

                try
                {
                    switch (columnName)
                    {
                        case "IsPlayTime":// 
                            strSql = "Update T_ADV_ADVFILE set IsPlayTime = '0' where IsPlayTime = '0'";
                            break;
                        case "PlayTime1":// 
                            strSql = "Update T_ADV_ADVFILE set PlayTime1 = '0' where PlayTime1 = '0'";
                            break;
                        case "PlayTime2":// 
                            strSql = "Update T_ADV_ADVFILE set PlayTime2 = '0' where PlayTime2 = '0'";
                            break;
                    }

                    result = dbOper.excuteSql(strSql);

                    blnIsExist = result;
                }
                catch
                {
                    blnIsExist = false;
                }

                #endregion

                if (!blnIsExist)
                {
                    // 不存在字段
                    switch (columnName)
                    {
                        case "IsPlayTime":// 
                            strSql = "ALTER TABLE `T_ADV_ADVFILE` ADD `IsPlayTime` VARCHAR( 2 ) ;";
                            break;
                        case "PlayTime1":// 
                            strSql = "ALTER TABLE `T_ADV_ADVFILE` ADD `PlayTime1` VARCHAR( 10 ) ;";
                            break;
                        case "PlayTime2":// 
                            strSql = "ALTER TABLE `T_ADV_ADVFILE` ADD `PlayTime2` VARCHAR( 10 ) ;";
                            break;
                    }

                    result = dbOper.excuteSql(strSql);
                    if (result)
                    {
                        // 添加字段成功，更新该字段的值
                        switch (columnName)
                        {
                            case "IsPlayTime":// 
                                strSql = "Update T_ADV_ADVFILE set IsPlayTime = '0'";
                                break;
                            case "PlayTime1":// 
                                strSql = "Update T_ADV_ADVFILE set PlayTime1 = '0'";
                                break;
                            case "PlayTime2":// 
                                strSql = "Update T_ADV_ADVFILE set PlayTime2 = '0'";
                                break;
                        }

                        result = dbOper.excuteSql(strSql);
                    }
                }

                dbOper.closeConnection();
            }
            catch
            {

            }

            return result;
        }

        #endregion
    }
}
