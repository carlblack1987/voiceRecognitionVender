#region [ KIMMA Co.,Ltd. Copyright (C) 2014 ]
//-------------------------------------------------------------------------------------
// 开发单位：湖南金码智能设备制造有限公司
// 业务模块：iVend终端软件业务处理类
// 业务功能：iVend终端软件业务处理类
// 创建标识：2014-06-10		谷霖
// 修改标识：
//           1、2014-09-20		谷霖
//           增加支付宝声波功能
//-------------------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Windows.Forms;
using System.Threading;

using KMB11;
using KdbPlug;
using GateServer1;
using Business.Model;
using Business.Enum;
using Business.Common;

using PosCard;
using SellEquUp;
using RealTimeGateServer;
////using BarCodeScan;
using WebServicePlug;
using JsonPlug;
using LitJson;
using ThermalPrint;

using HttpPlug;

using OnlinePay;

namespace Business
{
    public class BusinessOper
    {
        #region 常量声明

        private const int ERR_SYS = -1;// 系统内的错误

        #endregion

        #region 私有变量声明

        private string _m_DbFileName = "TermInfo.db";

        /// <summary>
        /// 控制主板类对象
        /// </summary>
        private KmbOper m_KmbOper = new KmbOper();

        /// <summary>
        /// 网络通信类对象 
        /// </summary>
        private GateSocket m_GateSocket = new GateSocket();

        /// <summary>
        /// 升降机终端处理类对象
        /// </summary>
        private SellEquOper m_SellEquUp = new SellEquOper();

        /// <summary>
        /// 刷卡处理类对象—储值卡
        /// </summary>
        private CardOper m_CardOper = new CardOper();

        /// <summary>
        /// 磁条卡处理类对象—非储值卡
        /// </summary>
        private NoFeeCardHelper m_NoFeeCardOper = new NoFeeCardHelper();

        /// <summary>
        /// 实时网关处理类对象
        /// </summary>
        private GateServer m_RealTimeGateServer = new GateServer();

        /// <summary>
        /// 电子支付网关处理类对象
        /// </summary>
        private OnlinePayOper m_OnlinePayOper = new OnlinePayOper();

        /// <summary>
        /// 机器开始销售时间
        /// </summary>
        private string m_BeginSaleDate = string.Empty;

        /// <summary>
        /// 机器段销售开始时间
        /// </summary>
        private string m_CycleSaleDate = string.Empty;

        /// <summary>
        /// 机器销售的当前日期
        /// </summary>
        private string m_TodaySaleDate = string.Empty;

        /// <summary>
        /// 机器开始货币处理时间
        /// </summary>
        private string m_BeginMoneyDate = string.Empty;

        /// <summary>
        /// 机器段货币处理开始时间
        /// </summary>
        private string m_CycleMoneyDate = string.Empty;

        /// <summary>
        /// 机器货币处理的当前日期
        /// </summary>
        private string m_TodayMoneyDate = string.Empty;

        /// <summary>
        /// 当前交易号
        /// </summary>
        private long m_BusId = 0;

        /// <summary>
        /// 查询金额的回复数据
        /// </summary>
        private string m_QueryMoneyValue = string.Empty;

        /// <summary>
        /// 查询现金支付设备状态的回复数据
        /// </summary>
        private string m_CheckPayMentStatusValue = string.Empty;

        /// <summary>
        /// 查询门控等设备状态的回复数据
        /// </summary>
        private string m_QueryEquStatusValue = string.Empty;

        /// <summary>
        /// 查询制冷压缩机/除雾/照明/广告灯等设备状态的回复数据
        /// </summary>
        private string m_QueryEqu1StatusValue = string.Empty;

        /// <summary>
        /// 查询用户卡信息的回复数据
        /// </summary>
        private string m_QueryCardInfo = string.Empty;

        /// <summary>
        /// 当前支付方式 销售方式 0：现金支付 1：非现金支付
        /// </summary>
        private string m_SaleType = "0";

        /// <summary>
        /// 历史可以支持的支付类型
        /// </summary>
        private BusinessEnum.PayShowStatus m_PayShow_History = BusinessEnum.PayShowStatus.Pause;

        /// <summary>
        /// 是否成功加载了货道信息 False：否 True：是
        /// </summary>
        private bool m_IsLoadAsileInfo = false;

        /// <summary>
        /// 是否成功发送系统配置参数 False：否 True：是
        /// </summary>
        private bool m_IsSendParameter = false;

        /// <summary>
        /// 是否成功发送货道弹簧圈数 False：否 True：是
        /// </summary>
        private bool m_IsSendSprintNum = false;

        /////// <summary>
        /////// 是否成功发送货道设置商品 False：否 True：是
        /////// </summary>
        ////private bool m_IsSendAsileGoods = false;

        #endregion

        #region 设备监控相关私有变量声明

        /// <summary>
        /// 吞币开始计算的时间
        /// </summary>
        private DateTime m_LastTunMoneyTime = DateTime.Now;

        /// <summary>
        /// 工控机/显示器/机箱风扇控制模式
        /// </summary>
        public DeviceControlModel ComputerControl = new DeviceControlModel();

        /// <summary>
        /// 声音设备控制模式
        /// </summary>
        public DeviceControlModel SoundControl = new DeviceControlModel();

        #endregion

        #region 公有变量声明

        public string DllPath = string.Empty;
        public string ErrMsg = string.Empty;

        /// <summary>
        /// 条形码处理类对象
        /// </summary>
        public BarCodeHelper BarCodeOper = new BarCodeHelper();

        /// <summary>
        /// 二代身份证业务处理对象类
        /// </summary>
        public IDCardHelper IDCardOper = new IDCardHelper();

        /// <summary>
        /// O2O业务处理对象类 2015-08-06添加
        /// </summary>
        public O2OHelper O2OServerOper = new O2OHelper();

        /// <summary>
        /// 微信取货码业务处理对象类 2015-08-31添加
        /// </summary>
        public WxTakeCodeHelper WxTakeCodeOper = new WxTakeCodeHelper();

        /// <summary>
        /// 当前业务操作步骤标记
        /// </summary>
        public BusinessEnum.OperStep OperStep = BusinessEnum.OperStep.Loading;

        /// <summary>
        /// 货道数据类对象  2014--07-09增加
        /// </summary>
        public AsileHelper AsileOper = new AsileHelper();

        /// <summary>
        /// 系统参数类对象  2014-07-09增加
        /// </summary>
        public SysCfgHelper SysCfgOper = new SysCfgHelper();

        /// <summary>
        /// 系统配置参数类对象 2014-09-03增加
        /// </summary>
        public ConfigModel ConfigInfo = new ConfigModel();

        /// <summary>
        /// 设备状态类对象
        /// </summary>
        public DeviceInfoModel DeviceInfo = new DeviceInfoModel();

        /// <summary>
        /// 商品类对象 2014-08-14增加
        /// </summary>
        public GoodsHelper GoodsOper = new GoodsHelper();

        /// <summary>
        /// 用户卡信息 2014-09-04增加
        /// </summary>
        public CardInfoModel UserCardInfo = new CardInfoModel();

        /// <summary>
        /// 会员用户信息对象
        /// </summary>
        public MemberUserInfoModel MemberUserInfo = new MemberUserInfoModel();

        /// <summary>
        /// 支付方式对象 2014-10-21增加
        /// </summary>
        public PayMentHelper PaymentOper = new PayMentHelper();

        /// <summary>
        /// 当前可以支持的支付类型
        /// </summary>
        public BusinessEnum.PayShowStatus PayShow = BusinessEnum.PayShowStatus.Pause;

        /// <summary>
        /// 条形码扫描在线支付信息对象 2015-01-16增加
        /// </summary>
        public BarCodeNetPayModel BarCode_Net_PayInfo = new BarCodeNetPayModel();

        /// <summary>
        /// 广告信息类对象  2015--03-16增加
        /// </summary>
        public AdvertHelper AdvertOper = new AdvertHelper();

        /// <summary>
        /// 数据验证处理类对象 2015-04-25从主模块移到业务模块
        /// </summary>
        public CheckDataHelper CheckDataOper = new CheckDataHelper();

        /// <summary>
        /// 货币信息处理类对象 2015-07-23添加
        /// </summary>
        public CashInfoHelper CashInfoOper = new CashInfoHelper();

        /// <summary>
        /// 购买选择方式 货道点击 商品点击
        /// </summary>
        public BusinessEnum.GoodsShowModelType GoodsShowModelType = BusinessEnum.GoodsShowModelType.GoodsToOnlyAsile;

        /// <summary>
        /// 远程控制处理类对象 2016-07-23添加
        /// </summary>
        public RemoteControlHelper RemoteControlOper = new RemoteControlHelper();

        #endregion

        #region 属性

        private string m_IVendSoftVer = "MV_160920";
        /// <summary>
        /// 终端软件版本
        /// </summary>
        public string IVendSoftVer
        {
            //get { return m_IVendSoftVer + "_" + m_CardOper.PosType; }
            get { return m_IVendSoftVer; }
        }

        private int m_SellGoodsNum = 0;
        /// <summary>
        /// 当前投币后的购买次数
        /// </summary>
        public int SellGoodsNum
        {
            get { return m_SellGoodsNum; }
        }

        private int m_TotalPayMoney = 0;
        /// <summary>
        /// 当前现金投币金额【总金额】
        /// </summary>
        public int TotalPayMoney
        {
            get { return m_TotalPayMoney; }
            set { m_TotalPayMoney = value; }// 临时测试
        }

        private int m_TotalPayMoney_NoStack = 0;
        /// <summary>
        /// 当前现金纸币缓存金额【】
        /// </summary>
        public int TotalPayMoney_NoStack
        {
            get { return m_TotalPayMoney_NoStack; }
            set { m_TotalPayMoney_NoStack = value; }// 临时测试
        }

        private bool m_IsDisplose = false;
        /// <summary>
        /// 组件是否已释放 False：未释放 True：已释放
        /// </summary>
        public bool IsDisplose
        {
            get { return m_IsDisplose; }
        }

        private BusinessEnum.UserType m_UserType = BusinessEnum.UserType.Unknow;
        /// <summary>
        /// 管理员类型
        /// </summary>
        public BusinessEnum.UserType UserType
        {
            get { return m_UserType; }
        }

        /// <summary>
        /// 控制主板版本
        /// </summary>
        public string KmbVer
        {
            get { return m_KmbOper.Version; }
        }

        #endregion

        #region 初始化函数

        /// <summary>
        /// 初始化组件
        /// </summary>
        /// <returns>返回码 0：成功 1：数据文件不存在 2：初始化系统参数失败 3：加载货道信息失败</returns>
        public int InitPlug()
        {
            int intErrCode = 0;

            m_IsDisplose = false;

            string strInfo = string.Empty;

            #region 检查系统数据文件是否存在

            AddBusLog("===========================================");
            AddBusLog("Checking Db...");
            if (!File.Exists(Application.StartupPath + "\\db\\TermInfo.db"))
            {
                // 不存在系统数据文件
                strInfo = "Db file isn't exist,System will exit,Please check";
                AddBusLog(strInfo);
                return 1;
            }

            #endregion

            #region 初始化系统配置参数

            AddBusLog("Loading Config...");
            bool result = SysCfgOper.LoadSysCfg();
            if (!result)
            {
                // 加载系统配置参数失败
                strInfo = "Load config fail,System will exit,Please check";
                AddBusLog(strInfo);
                return 2;
            }

            ConfigInfo.VmId = SysCfgOper.GetSysCfgValue("VmId");
            ConfigInfo.Language = FunHelper.ConvertLanguage(SysCfgOper.GetSysCfgValue("Language"));
            ConfigInfo.PointNum = SysCfgOper.GetSysCfgValue("IsShowPoint");
            ConfigInfo.ColumnType = SysCfgOper.GetSysCfgValue("ColumnType");
            ConfigInfo.VmSoftApiUrl = SysCfgOper.GetSysCfgValue("VmSoftApiUrl");
            ConfigInfo.VmSoftApiUserKey = SysCfgOper.GetSysCfgValue("VmSoftApiUserKey");

            #region 更新终端配置参数

            AdvertOper.VmSoftUrl = ConfigInfo.VmSoftApiUrl;
            AdvertOper.UserKey = ConfigInfo.VmSoftApiUserKey;
            AdvertOper.VmID = ConfigInfo.VmId;

            AddBusLog("Uploading Config...");
            string strErrCode = string.Empty;
            UploadSysCfg_Net(out strErrCode);

            #endregion

            // 获取国家相关信息，包括小数点位数、最小货币面值
            SysCfgOper.GetCountryInfo(SysCfgOper.GetSysCfgValue("CountryCode"));
            ConfigInfo.DecimalNum = Convert.ToInt32(SysCfgOper.CountryInfo.DecimalNum);
            ConfigInfo.MinMoneyValue = Convert.ToInt32(SysCfgOper.CountryInfo.MinMoneyValue);
            ConfigInfo.MoneySymbol = SysCfgOper.CountryInfo.MoneySymbol;
            ConfigInfo.IsShowMoneySymbol = FunHelper.ChangeControlSwitch(SysCfgOper.GetSysCfgValue("IsShowMoneySymbol"));

            ConfigInfo.PosBusiType = FunHelper.ChangePosBusiType(SysCfgOper.GetSysCfgValue("IcBusiType"));
            ConfigInfo.ScreenType = FunHelper.ChangeScreenType(SysCfgOper.GetSysCfgValue("ScreenType"));
            ConfigInfo.NoFeeCardIsRebate = SysCfgOper.GetSysCfgValue("NoFeeCardIsRebate");

            ConfigInfo.GoodsShowModel = FunHelper.ChangeGoodsShowModelType(SysCfgOper.GetSysCfgValue("GoodsShowModel"));// 商品展示模式
            ConfigInfo.EachPageMaxRowNum = Convert.ToInt32(SysCfgOper.GetSysCfgValue("EachPageMaxRowNum"));
            ConfigInfo.EachRowMaxColuNum = Convert.ToInt32(SysCfgOper.GetSysCfgValue("EachRowMaxColuNum"));

            ConfigInfo.IsShowMainLgsBottom = FunHelper.ChangeControlSwitch(SysCfgOper.GetSysCfgValue("IsShowMainLgsBottom"));
            ConfigInfo.IsShowMainLgsTop = FunHelper.ChangeControlSwitch(SysCfgOper.GetSysCfgValue("IsShowMainLgsTop"));
            ConfigInfo.MainLgsTop_Type = FunHelper.ConvertMainLgsTopType(SysCfgOper.GetSysCfgValue("MainLgsTop_Type"));

            ConfigInfo.CoinDeviceType = FunHelper.ConvertCoinDeviceType(SysCfgOper.GetSysCfgValue("CoinDeviceType"));

            ConfigInfo.IsShowVmDiagnose = FunHelper.ChangeControlSwitch(SysCfgOper.GetSysCfgValue("IsShowVmDiagnose"));
            ConfigInfo.BillStackSwitch = FunHelper.ChangeControlSwitch(SysCfgOper.GetSysCfgValue("BillStackSwitch"));

            ConfigInfo.RemoteControlSwitch = FunHelper.ChangeControlSwitch(SysCfgOper.GetSysCfgValue("RemoteControlSwitch"));

            ConfigInfo.NowAdvertPlayID = SysCfgOper.GetSysCfgValue("NowAdvertPlayID");
            ConfigInfo.UpdateAdvertListID = SysCfgOper.GetSysCfgValue("UpdateAdvertListID");
            ConfigInfo.AdvertUploadType = SysCfgOper.GetSysCfgValue("AdvertUploadType");

            ConfigInfo.GoodsUploadType = SysCfgOper.GetSysCfgValue("GoodsUploadType");
            ConfigInfo.PriceUploadType = SysCfgOper.GetSysCfgValue("PriceUploadType");
            
            #endregion

            #region 获取终端货道列表信息

            AddBusLog("Loading Column...");
            result = AsileOper.LoadAsileInfo_Total(ConfigInfo.ColumnType, SysCfgOper.GetSysCfgValue("IsTenAsileNum"));
            if (!result)
            {
                // 加载货道信息列表失败
                strInfo = "Load column fail,System will exit,Please check";
                AddBusLog(strInfo);
                return 3;
            }

            result = AsileOper.UpDown_LoadUpDownCodeNum();// 加载升降机上下码盘配置信息

            #endregion

            #region 加载商品信息列表

            if (SysCfgOper.GetSysCfgValue("IsTestVer") == "1")
            {
                // 如果是测试版本
                LoadMcdInfo();
            }

            #endregion

            InitParameter();

            InitDll();

            #region 初始化广告

            AdvertOper.NowAdvertPlayID = ConfigInfo.NowAdvertPlayID;
            AdvertOper.UpdateAdvertListID = ConfigInfo.UpdateAdvertListID;
            
            AdvertOper.AdvertUploadType = ConfigInfo.AdvertUploadType;

            AdvertOper.InitAdvert();

            #endregion

            #region 加载货币库存信息表

            result = CashInfoOper.LoadCashInfoList();

            #endregion

            return intErrCode;
        }

        /// <summary>
        /// 释放组件
        /// </summary>
        public void Displose()
        {
            m_CardOper.Displose();
            m_GateSocket.Displose();

            m_IsDisplose = true;
        }

        /// <summary>
        /// 初始化系统部分配置参数
        /// </summary>
        public void InitParameter()
        {
            #region 获取相关内存参数

            ConfigInfo.ChangeModel = SysCfgOper.GetSysCfgValue("ChangeModel");
            ConfigInfo.SaleModel = SysCfgOper.GetSysCfgValue("SaleModel");

            ConfigInfo.TunOutTime = Convert.ToInt32(SysCfgOper.GetSysCfgValue("TunOutTime"));// 吞币超时时间
            ConfigInfo.QueryTmpDelay = Convert.ToInt32(SysCfgOper.GetSysCfgValue("QueryTmpDelay"));// 温度采集间隔时间
            ConfigInfo.IsRunStock = FunHelper.ChangeControlSwitch(SysCfgOper.GetSysCfgValue("IsRunStock"));// 是否启用库存
            ConfigInfo.IcQuerySwitch = FunHelper.ChangeControlSwitch(SysCfgOper.GetSysCfgValue("IcQuerySwitch"));// 是否在主界面时显示卡查询信息
            ConfigInfo.ShowCardInfoTime = Convert.ToInt32(SysCfgOper.GetSysCfgValue("ShowCardInfoTime"));// 在主界面时显示卡查询信息展现时间
            ConfigInfo.SellOperOutTime = Convert.ToInt32(SysCfgOper.GetSysCfgValue("SellOperOutTime"));// 购物无操作超时时间
            
            ConfigInfo.GoodsShowContent = SysCfgOper.GetSysCfgValue("GoodsShowContent");// 商品列表显示内容
            ConfigInfo.GoodsOpacity = Convert.ToInt32(SysCfgOper.GetSysCfgValue("GoodsOpacity"));// 商品无库存时透明度
            ConfigInfo.NoStockClickGoods = SysCfgOper.GetSysCfgValue("NoStockClickGoods");// 商品无库存时是否允许点击
            ConfigInfo.AllowPaymentList = SysCfgOper.GetSysCfgValue("AllowPaymentList");// 允许支付方式列表
            PaymentOper.AllowPaymentList = ConfigInfo.AllowPaymentList;
            ConfigInfo.ClearLogIntervalDay = SysCfgOper.GetSysCfgValue("ClearLogIntervalDay");// 日志清除间隔天数

            ConfigInfo.GoodsNameShowType = SysCfgOper.GetSysCfgValue("GoodsNameShowType");// 商品展示页面中商品名称显示类型

            ConfigInfo.IcBusiModel = SysCfgOper.GetSysCfgValue("IcBusiModel");// 储值卡业务流程模式
            ConfigInfo.IcPayShowSwitch = FunHelper.ChangeControlSwitch(SysCfgOper.GetSysCfgValue("IcPayShowSwitch"));// 储值卡用户信息是否显示
            ConfigInfo.NoFeeCardPayShow = FunHelper.ChangeControlSwitch(SysCfgOper.GetSysCfgValue("NoFeeCardPayShow"));// 会员卡用户信息是否显示
            ConfigInfo.IcCardNumHide = FunHelper.ChangeControlSwitch(SysCfgOper.GetSysCfgValue("IcCardNumHide"));// 储值卡卡号信息*字显示
            ConfigInfo.NoFeeCardNumHide = FunHelper.ChangeControlSwitch(SysCfgOper.GetSysCfgValue("NoFeeCardNumHide"));// 会员卡卡号信息*字显示

            // 打印机相关参数
            ConfigInfo.IsPrintConsumeBill = FunHelper.ChangeControlSwitch(SysCfgOper.GetSysCfgValue("IsPrintConsumeBill"));//是否打印购物单据
            ConfigInfo.PrintPort = SysCfgOper.GetSysCfgValue("PrintPort");
            ConfigInfo.PrintTmepContent = SysCfgOper.GetSysCfgValue("PrintTmepContent");// 打印内容
            ConfigInfo.PrintTmepTitle = SysCfgOper.GetSysCfgValue("PrintTmepTitle");// 打印内容

            ConfigInfo.GoodsPropShowType = SysCfgOper.GetSysCfgValue("GoodsPropShowType");
            ConfigInfo.IsShowGoodsDetailContent = FunHelper.ChangeControlSwitch(SysCfgOper.GetSysCfgValue("IsShowGoodsDetailContent"));

            ConfigInfo.GoodsDetailFontSize = Convert.ToInt32(SysCfgOper.GetSysCfgValue("GoodsDetailFontSize"));

            ConfigInfo.IsReturnBill = FunHelper.ChangeControlSwitch(SysCfgOper.GetSysCfgValue("IsReturnBill"));// 是否有纸币找零功能
            ConfigInfo.ReturnBillMoney = Convert.ToInt32(SysCfgOper.GetSysCfgValue("ReturnBillMoney"));// 纸币找零面值

            ConfigInfo.IsShowChoiceKeyBoard = FunHelper.ChangeControlSwitch(SysCfgOper.GetSysCfgValue("IsShowChoiceKeyBoard"));// 是否显示选货键盘
            ConfigInfo.KeyBoardType = SysCfgOper.GetSysCfgValue("KeyBoardType");// 输入键盘类型

            // 增值服务相关参数
            ConfigInfo.IDCardFreeTake_Switch = FunHelper.ChangeControlSwitch(SysCfgOper.GetSysCfgValue("IDCardFreeTake_Switch"));
            ConfigInfo.O2OTake_Switch = FunHelper.ChangeControlSwitch(SysCfgOper.GetSysCfgValue("O2OTake_Switch"));
            ConfigInfo.WxTake_Switch = FunHelper.ChangeControlSwitch(SysCfgOper.GetSysCfgValue("WxTake_Switch"));

            ConfigInfo.IsFreeSellNoPay = FunHelper.ChangeControlSwitch(SysCfgOper.GetSysCfgValue("IsFreeSellNoPay"));

            ConfigInfo.CashManagerModel = FunHelper.ChangeCashManagerModel(SysCfgOper.GetSysCfgValue("CashManagerModel"));

            // 升降机相关参数 2016-07-06
            ConfigInfo.UpDownLeftRightNum_Left = Convert.ToInt32(SysCfgOper.GetSysCfgValue("UpDownLeftRightNum_Left"));
            ConfigInfo.UpDownLeftRightNum_Center = Convert.ToInt32(SysCfgOper.GetSysCfgValue("UpDownLeftRightNum_Center"));
            ConfigInfo.UpDownLeftRightNum_Right = Convert.ToInt32(SysCfgOper.GetSysCfgValue("UpDownLeftRightNum_Right"));

            ConfigInfo.AddedServiceSwitch = FunHelper.ChangeControlSwitch(SysCfgOper.GetSysCfgValue("AddedServiceSwitch"));

            #endregion

            #region 获取支付方式开关及相关信息

            PaymentOper.PaymentList.IC.ControlSwitch = PaymentOper.ChangePaymentControl(BusinessEnum.PayMent.IcCard, SysCfgOper.GetSysCfgValue("IcControlSwitch"));// 储值卡支付开关
            PaymentOper.PaymentList.WeChatCode.ControlSwitch = PaymentOper.ChangePaymentControl(BusinessEnum.PayMent.WeChatCode, SysCfgOper.GetSysCfgValue("WeChatCodeSwitch"));// 微信扫码支付开关
            PaymentOper.PaymentList.AliPay_Code.ControlSwitch = PaymentOper.ChangePaymentControl(BusinessEnum.PayMent.AliPay_Code, SysCfgOper.GetSysCfgValue("AliPayCodeSwitch"));// 支付宝扫码支付开关
            PaymentOper.PaymentList.Cash.ControlSwitch = PaymentOper.ChangePaymentControl(BusinessEnum.PayMent.Cash, SysCfgOper.GetSysCfgValue("CashControlSwitch"));// 现金支付开关
            PaymentOper.PaymentList.UnionPay.ControlSwitch = PaymentOper.ChangePaymentControl(BusinessEnum.PayMent.QuickPass, SysCfgOper.GetSysCfgValue("UnionPaySwitch"));// 银联闪付开关
            PaymentOper.PaymentList.NoFeeCard.ControlSwitch = PaymentOper.ChangePaymentControl(BusinessEnum.PayMent.NoFeeCard, SysCfgOper.GetSysCfgValue("NoFeeCardSwitch"));// 会员卡（非储值卡）支付开关
            PaymentOper.PaymentList.QRCodeCard.ControlSwitch = PaymentOper.ChangePaymentControl(BusinessEnum.PayMent.QRCodeCard, SysCfgOper.GetSysCfgValue("QRCodeCardSwitch"));// 虚拟会员卡（二维码会员）支付开关
            PaymentOper.PaymentList.BestPay_Code.ControlSwitch = PaymentOper.ChangePaymentControl(BusinessEnum.PayMent.BestPay_Code, SysCfgOper.GetSysCfgValue("BestPayCodeSwitch"));// 翼支付付款码支付开关

            PaymentOper.PaymentList.Volunteer_Code.ControlSwitch = PaymentOper.ChangePaymentControl(BusinessEnum.PayMent.Volunteer, SysCfgOper.GetSysCfgValue("VolunteerPay_Switch"));// 志愿者兑换支付开关

            PaymentOper.LoadPaymentName();

            #endregion

            #region 获取广告播放参数（2015-03-16添加）

            ConfigInfo.AdvertPlaySwitch = FunHelper.ChangeControlSwitch(SysCfgOper.GetSysCfgValue("AdvertPlaySwitch"));
            ConfigInfo.AdvertImgShowTime = Convert.ToInt32(SysCfgOper.GetSysCfgValue("AdvertImgShowTime"));
            ConfigInfo.AdvertPlayOutTime = Convert.ToInt32(Convert.ToDouble(SysCfgOper.GetSysCfgValue("AdvertPlayOutTime")) * 60);

            #endregion

            DeviceInfo.KmbControlSwitch = FunHelper.ChangeControlSwitch(SysCfgOper.GetSysCfgValue("KmbControlSwitch"));

            // 获取数据库中的相关参数
            string strSql = @"select BeginDate,CycleDate,TodayDate from T_SALE_STAT";
            DbOper dbOper = new DbOper();
            dbOper.DbFileName = _m_DbFileName;

            DataSet dataSet = dbOper.dataSet(strSql);
            if (dataSet.Tables.Count > 0)
            {
                if (dataSet.Tables[0].Rows.Count > 0)
                {
                    m_BeginSaleDate = dataSet.Tables[0].Rows[0]["BeginDate"].ToString();
                    m_CycleSaleDate = dataSet.Tables[0].Rows[0]["CycleDate"].ToString();
                    m_TodaySaleDate = dataSet.Tables[0].Rows[0]["TodayDate"].ToString();
                }
            }

            strSql = "select TodayDate from T_MONEY_STAT";
            dataSet = dbOper.dataSet(strSql);
            if (dataSet.Tables.Count > 0)
            {
                if (dataSet.Tables[0].Rows.Count > 0)
                {
                    m_BeginMoneyDate = string.Empty;// dataSet.Tables[0].Rows[0]["BeginSaleDate"].ToString();
                    m_CycleMoneyDate = string.Empty;// dataSet.Tables[0].Rows[0]["CycleBeginDate"].ToString();
                    m_TodayMoneyDate = dataSet.Tables[0].Rows[0]["TodayDate"].ToString();
                }
            }

            dbOper.closeConnection();

            // 读取当前交易号
            m_BusId = Convert.ToInt32(SysCfgOper.GetSysCfgValue("BusinessId"));

            #region 初始化设备控制参数

            for (int i = 0; i < AsileOper.VendBoxList.Count; i++)
            {
                AsileOper.VendBoxList[i].RefControl.LastCloseTime = DateTime.Now;// 记录制冷压缩机最后一次关闭时间
                AsileOper.VendBoxList[i].RefControl.OpenMaxTime = Convert.ToInt32(SysCfgOper.GetSysCfgValue("RefOpenMaxTime"));// 打开最长时间，默认50分钟
                AsileOper.VendBoxList[i].RefControl.CloseDelayTime = Convert.ToInt32(SysCfgOper.GetSysCfgValue("TmpRunDelay"));// 关闭后再次打开延长时间，默认7分钟

                AsileOper.VendBoxList[i].RefControl.ControlStatus = BusinessEnum.DeviceControlStatus.Close;
                AsileOper.VendBoxList[i].RefControl.IsRefreshCfg = true;

                AsileOper.VendBoxList[i].LightControl.ControlStatus = BusinessEnum.DeviceControlStatus.Close;
                AsileOper.VendBoxList[i].LightControl.IsRefreshCfg = true;

                AsileOper.VendBoxList[i].AdvertLightControl.ControlStatus = BusinessEnum.DeviceControlStatus.Close;
                AsileOper.VendBoxList[i].AdvertLightControl.IsRefreshCfg = true;

                AsileOper.VendBoxList[i].DemisterControl.ControlStatus = BusinessEnum.DeviceControlStatus.Close;
                AsileOper.VendBoxList[i].DemisterControl.IsRefreshCfg = true;

                AsileOper.VendBoxList[i].UltravioletLampControl.ControlStatus = BusinessEnum.DeviceControlStatus.Close;
                AsileOper.VendBoxList[i].UltravioletLampControl.IsRefreshCfg = true;

                AsileOper.VendBoxList[i].DraughtFanControl.ControlStatus = BusinessEnum.DeviceControlStatus.Close;
                AsileOper.VendBoxList[i].DraughtFanControl.IsRefreshCfg = true;
            }
            
            ComputerControl.ControlStatus = BusinessEnum.DeviceControlStatus.Close;
            ComputerControl.IsRefreshCfg = true;

            SoundControl.ControlStatus = BusinessEnum.DeviceControlStatus.Close;
            SoundControl.IsRefreshCfg = true;

            #endregion
        }

        /// <summary>
        /// 加载商品信息
        /// </summary>
        public void LoadMcdInfo()
        {
            bool result = GoodsOper.LoadGoodsInfo_Total();

            result = GoodsOper.LoadGoodsInfo_Show(ConfigInfo.EachPageMaxRowNum, ConfigInfo.EachRowMaxColuNum);

            result = GoodsOper.LoadGoodsTypeList();// 加载商品类型信息
        }

        /// <summary>
        /// 初始化各组件
        /// </summary>
        private void InitDll()
        {
            string strVmId = ConfigInfo.VmId;

            LogHelper.IsLogToFile = FunHelper.ConvertIsWriteLog(SysCfgOper.GetSysCfgValue("IsWriteLog_Busi"));

            #region 初始化网络通信组件

            AddBusLog("Initializing Net...");

            string strNetSwitch = SysCfgOper.GetSysCfgValue("NetSwitch");
            m_GateSocket.IsEnable = false;
            if ((strNetSwitch == "1") && (SysCfgOper.GetSysCfgValue("NetEquKind") == "0"))
            {
                // 如果是上位机软件连接网络设备，且网络通信启用
                m_GateSocket.IsEnable = true;
            }

            DeviceInfo.NetSoftVer = m_GateSocket.SoftName;
            m_GateSocket.ServerIp = SysCfgOper.GetSysCfgValue("NetIp");
            m_GateSocket.ServerPort = Convert.ToInt32(SysCfgOper.GetSysCfgValue("NetPort"));
            m_GateSocket.VmCode = strVmId;
            m_GateSocket.VmPwd = SysCfgOper.GetSysCfgValue("NetPwd");
            m_GateSocket.IsLogToFile = FunHelper.ConvertIsWriteLog(SysCfgOper.GetSysCfgValue("IsWriteLog_GateWay"));

            m_GateSocket.Initialize();

            #endregion

            #region 初始化控制主板组件

            m_KmbOper.IsLogToFile = FunHelper.ConvertIsWriteLog(SysCfgOper.GetSysCfgValue("IsWriteLog_Kmb"));

            #endregion

            #region 初始化刷卡支付

            DeviceInfo.ICSoftVer = m_CardOper.SoftName;
            m_CardOper.TermCode = strVmId;
            m_CardOper.IsLogToFile = FunHelper.ConvertIsWriteLog(SysCfgOper.GetSysCfgValue("IsWriteLog_Card"));
            m_CardOper.ThirdWebUrl = SysCfgOper.GetSysCfgValue("IcWebUrl");

            #endregion

            #region 初始化会员卡支付

            DeviceInfo.NoFeeCardSoftVer = m_NoFeeCardOper.SoftName();
            m_NoFeeCardOper.IsLogToFile(FunHelper.ConvertIsWriteLog(SysCfgOper.GetSysCfgValue("IsWriteLog_ID")));

            string strPort = SysCfgOper.GetSysCfgValue("NoFeeCardPort");

            m_NoFeeCardOper.NoFeeCardConfig.Port = Convert.ToInt32(strPort);
            m_NoFeeCardOper.NoFeeCardConfig.WebUrl = SysCfgOper.GetSysCfgValue("NoFeeCardWebUrl");
            m_NoFeeCardOper.NoFeeCardConfig.TermCode = strVmId;

            #endregion

            #region 初始化支付宝声波支付

            ////AliPayWavePayment.VmId = strVmId;
            ////AliPayWavePayment.AliPayWebUrl = SysCfgOper.GetSysCfgValue("AliPayWebUrl");
            ////AliPayWavePayment.WaveTimeOut = Convert.ToInt32(SysCfgOper.GetSysCfgValue("AliPayWaveTimeOut"));

            #endregion

            #region 初始化实时通讯网关

            m_RealTimeGateServer.ServerIp = SysCfgOper.GetSysCfgValue("RTimeServerIp");
            m_RealTimeGateServer.ServerPort = Convert.ToInt32(SysCfgOper.GetSysCfgValue("RTimeServerPort"));
            m_RealTimeGateServer.VmCode = ConfigInfo.VmId;
            m_RealTimeGateServer.VmPwd = SysCfgOper.GetSysCfgValue("NetPwd");

            #endregion

            #region 初始化条形码扫描设备

            BarCodeOper.ComPort = Convert.ToInt32(SysCfgOper.GetSysCfgValue("BarCodeScanPort"));

            #endregion

            #region 初始化二代身份证设备（2015-06-10）

            InitIDCard();

            #endregion

            #region 初始化O2O组件（2015-09-07）

            O2OServerOper.IsSendWaitData = false;
            if (ConfigInfo.O2OTake_Switch == BusinessEnum.ControlSwitch.Run)
            {
                // 如果线下取货功能开启
                O2OServerOper.IsSendWaitData = true;
            }
            O2OServerOper.VmID = ConfigInfo.VmId;
            O2OServerOper.ServerURL = SysCfgOper.GetSysCfgValue("O2OServerUrl");
            O2OServerOper.UserKey = SysCfgOper.GetSysCfgValue("O2OServer_UserKey");
            O2OServerOper.Init();

            #endregion

            #region 初始化电子支付网关组件（2016-09-13）

            InitOnlinePayDll();

            #endregion

            #region 初始化远程控制组件（2016-07-23）

            if (ConfigInfo.RemoteControlSwitch == BusinessEnum.ControlSwitch.Run)
            {
                int intRemoteListenPort = Convert.ToInt32(SysCfgOper.GetSysCfgValue("RemoteListenPort"));
                RemoteControlOper.Init(intRemoteListenPort);
            }

            #endregion
        }

        #region 详细的初始化各组件

        /// <summary>
        /// 初始化电子支付网关组件
        /// </summary>
        private void InitOnlinePayDll()
        {
            m_OnlinePayOper.VmID = ConfigInfo.VmId;
            m_OnlinePayOper.VmPwd = SysCfgOper.GetSysCfgValue("NetPwd");
            m_OnlinePayOper.OnlinePayUrl = SysCfgOper.GetSysCfgValue("OnlinePayUrl");
            m_OnlinePayOper.Token = SysCfgOper.GetSysCfgValue("OnlinePayToken");
        }

        #endregion

        /// <summary>
        /// 获取网络连接状态
        /// </summary>
        /// <returns>网络连接状态 0：离线 1：联机</returns>
        public BusinessEnum.NetStatus GetNetStatus()
        {
            bool blnNetStatus = m_GateSocket.SocketStatus;

            BusinessEnum.NetStatus strNetStatus = BusinessEnum.NetStatus.OffLine;

            switch (blnNetStatus)
            {
                case true:
                    strNetStatus = BusinessEnum.NetStatus.OnLine;
                    break;

                default:
                    strNetStatus = BusinessEnum.NetStatus.OffLine;
                    break;
            }

            DeviceInfo.NetStatus = strNetStatus;

            return strNetStatus;
        }

        /// <summary>
        /// 获取待发数据数量
        /// </summary>
        /// <returns>待发数据数量</returns>
        public int GetWaitNetDataNum()
        {
            int dataCount = 0;
            dataCount = m_GateSocket.GetWaitDataNum();
            DeviceInfo.NetNoSendNum = dataCount;
            return dataCount;
        }

        #endregion

        #region 私有函数（硬件设备相关控制）

        #region 初始化相关操作

        /// <summary>
        /// 初始化控制主板并联机
        /// </summary>
        /// <returns>结果代码</returns>
        private int InitKmb()
        {
            int intErrCode = 0;

            string strLogType = string.Empty;

            string strPort = string.Empty;

            AddBusLog("===========================================");

            if (DeviceInfo.KmbControlSwitch == BusinessEnum.ControlSwitch.Run)
            {
                // 如果启动控制主板
                strLogType = "InitKmb";

                strPort = SysCfgOper.GetSysCfgValue("KmbPort");
                intErrCode = m_KmbOper.InitKmb(Convert.ToInt32(strPort));

                AddBusLog_Code(strLogType, intErrCode.ToString(), "");

                if (intErrCode == 0)
                {
                    strLogType = "ConnectKmb";

                    intErrCode = m_KmbOper.ConnectKmb();

                    AddBusLog_Code(strLogType, intErrCode.ToString(), "");
                }

                CheckKmbConnect(intErrCode);
            }

            // 升降机方式
            intErrCode = m_SellEquUp.Initialize();

            AddBusLog_Code("InitUpDown", intErrCode.ToString(), "");

            return intErrCode;
        }

        /// <summary>
        /// 检测控制主板通讯连接是否正常
        /// </summary>
        /// <param name="errCode">错误代码</param>
        private void CheckKmbConnect(int errCode)
        {
            switch (errCode)
            {
                case 1:// 没有初始化类
                case 3:// 打开串口失败
                case 4:// 通信超时
                case 5:// 控制主板正在维护
                case 8:// 向控制主板发送的数据错误
                case 11:// 从控制主板接收到的数据为空或数据错误
                    DeviceInfo.KmbConnectStatus = false;
                    break;

                default:
                    DeviceInfo.KmbConnectStatus = true;
                    break;
            }
        }

        /// <summary>
        /// 查询控制主板信息
        /// </summary>
        /// <returns>错误代码</returns>
        private int QueryKmbInfo()
        {
            int intErrCode = 0;

            string strValue = string.Empty;

            intErrCode = m_KmbOper.QueryKmbInfo(out strValue);

            if (intErrCode == 0)
            {
                string[] hexValue = strValue.Split('|');
                // 控制主板协议版本|控制主板时间|控制主板序列号|控制主板软件版本号
                if (hexValue.Length > 3)
                {
                    DeviceInfo.KmbSoftVer = hexValue[3];
                }
                else
                {
                    intErrCode = ERR_SYS;
                }
            }

            AddBusLog_Code("QueryKmbInfo", intErrCode.ToString(), strValue);

            return intErrCode;
        }

        /// <summary>
        /// 设置控制主板序列号
        /// </summary>
        /// <returns>错误代码</returns>
        private int SetKmbId()
        {
            int intErrCode = 0;

            string strValue = string.Empty;
            intErrCode = m_KmbOper.SetKmbId(ConfigInfo.VmId, out strValue);

            AddBusLog_Code("SetKmbId", intErrCode.ToString(), strValue);

            CheckKmbConnect(intErrCode);

            return intErrCode;
        }

        /// <summary>
        /// 设置控制主板通讯功能
        /// </summary>
        /// <returns>错误代码</returns>
        private int SetKmbNetSwitch()
        {
            string strNetSwitch = SysCfgOper.GetSysCfgValue("NetSwitch");

            string strNetPwd = SysCfgOper.GetSysCfgValue("NetPwd");

            int intErrCode = ERR_SYS;

            string strValue = string.Empty;

            intErrCode = m_KmbOper.SetKmbNetSwitch(strNetSwitch, strNetPwd, out strValue);

            AddBusLog_Code("SetKmbNetSwitch", intErrCode.ToString(), strValue);

            return intErrCode;
        }

        /// <summary>
        /// 使相关硬件设备恢复到初始状态
        /// </summary>
        /// <returns>结果代码</returns>
        public int InitDevice()
        {
            int intErrCode = 0;

            // 禁能硬币器
            intErrCode = ControlCoin("0",true);
            Thread.Sleep(100);

            if (intErrCode == 0)
            {
                // 禁能纸币器
                intErrCode = ControlCash("0",true);
                Thread.Sleep(100);
            }

            return intErrCode;
        }

        #endregion

        #region 货道相关控制

        /// <summary>
        /// 根据货道索引查询货道状态
        /// </summary>
        /// <param name="paCode">货道索引</param>
        /// <param name="isSend">是否需要向网关传送</param>
        /// <returns>结果代码</returns>
        private int QueryPaStatus(int paIndex, bool isSend)
        {
            int intErrCode = 0;
            int intTrayNum = 0;
            int intColumnNum = 0;
            string strValue = string.Empty;

            string strLogType = "QueryPaStatus";

            // 如何检测出某个货道是否暂停服务

            try
            {
                string strPaId = string.Empty;
                string strPaCode = string.Empty;
                string strVendBoxCode = string.Empty; 

                strPaId = AsileOper.AsileList[paIndex].PaId;
                strPaCode = AsileOper.AsileList[paIndex].PaCode;
                strVendBoxCode = AsileOper.AsileList[paIndex].VendBoxCode;

                AddBusLog(strLogType + "  Box:" + strVendBoxCode + "  PaCode:" + strPaCode + "  PaId:" + strPaId);

                intTrayNum = Convert.ToInt32(strPaId.Substring(0, 1));
                intColumnNum = Convert.ToInt32(strPaId.Substring(1, 1));

                string strPaStatus = string.Empty;// 货道状态

                int intVendBoxIndex = AsileOper.GetVendBoxIndex(strVendBoxCode);

                switch (AsileOper.VendBoxList[intVendBoxIndex].SellGoodsType)
                {
                    case BusinessEnum.SellGoodsType.Spring:// 弹簧
                        intErrCode = m_KmbOper.QueryAisleInfo(Convert.ToInt32(strVendBoxCode), intTrayNum, intColumnNum, out strValue);
                        if (intErrCode == 0)
                        {
                            string[] hexValue = strValue.Split('|');
                            if (hexValue.Length > 3)
                            {
                                strPaStatus = hexValue[2];
                            }
                            else
                            {
                                intErrCode = ERR_SYS;
                            }
                        }
                        break;

                    case BusinessEnum.SellGoodsType.Lifter_Comp:// 复杂型升降机方式
                    case BusinessEnum.SellGoodsType.Lifter_Simple:// 简易型升降机方式
                        // 重新设置升降机串口
                        m_SellEquUp.Reset(Convert.ToInt32(AsileOper.VendBoxList[intVendBoxIndex].ShippPort));
                        intErrCode = m_SellEquUp.QueryAisleInfo(intTrayNum, intColumnNum, out strValue);
                        if (intErrCode == 0)
                        {
                            strPaStatus = strValue;
                        }
                        break;
                }

                if (intErrCode == 0)
                {
                    AsileOper.AsileList[paIndex].IsQueryStatus = true;
                    if ((strPaStatus == "03") || (strPaStatus == "04"))
                    {
                        // 没有安装电机
                        AsileOper.AsileList[paIndex].PaKind = "2";
                    }
                    if (AsileOper.AsileList[paIndex].PaStatus != strPaStatus)
                    {
                        AsileOper.AsileList[paIndex].PaStatus = strPaStatus;

                        if (isSend)
                        {
                            // 向网关汇报信息
                            int intNetCode = m_GateSocket.UpdatePaStatus(strPaStatus, strPaCode);
                        }
                    }
                }

                AddBusLog_Code(strLogType, intErrCode.ToString(), strValue);
            }
            catch (Exception ex)
            {
                intErrCode = ERR_SYS;
                AddErrLog(strLogType, intErrCode.ToString(),ex.Message);
            }
            return intErrCode;
        }

        /// <summary>
        /// 根据货道编号查询货道状态及升降机状态
        /// </summary>
        /// <param name="paCode">货道编号</param>
        /// <param name="isSend"></param>
        /// <returns>结果代码</returns>
        private int QueryPaStatus(string paCode,bool isSend)
        {
            int intErrCode = 0;
            int intTrayNum = 0;
            int intColumnNum = 0;
            string strValue = string.Empty;

            string strLogType = "QueryPaStatus";

            // 如何检测出某个货道是否暂停服务

            try
            {
                int paIndex = 0;

                string strPaCode = AsileOper.CurrentMcdInfo.PaCode;

                string strPaId = AsileOper.CurrentMcdInfo.PaId;

                string strVendBoxCode = AsileOper.CurrentMcdInfo.VendBoxCode;

                AddBusLog(strLogType + "  Box:" + strVendBoxCode + "  PaCode:" + strPaCode + "  PaId:" + strPaId);

                intErrCode = AsileOper.GetPaIndex(strPaCode, out paIndex);
                if (intErrCode != 0)
                {
                    // 没有该货道
                    return intErrCode;
                }

                intTrayNum = Convert.ToInt32(strPaId.Substring(0, 1));
                intColumnNum = Convert.ToInt32(strPaId.Substring(1, 1));

                int intVendBoxIndex = AsileOper.GetVendBoxIndex(strVendBoxCode);

                string strPaStatus = string.Empty;// 货道状态

                switch (AsileOper.VendBoxList[intVendBoxIndex].SellGoodsType)
                {
                    #region 弹簧
                    case BusinessEnum.SellGoodsType.Spring:// 弹簧
                        intErrCode = m_KmbOper.QueryAisleInfo(Convert.ToInt32(strVendBoxCode), intTrayNum, intColumnNum, out strValue);
                        if (intErrCode == 0)
                        {
                            string[] hexValue = strValue.Split('|');
                            if (hexValue.Length > 3)
                            {
                                strPaStatus = hexValue[2];
                            }
                            else
                            {
                                intErrCode = ERR_SYS;
                            }
                        }
                        break;
                    #endregion
                    #region 升降机
                    case BusinessEnum.SellGoodsType.Lifter_Comp:// 复杂型升降机方式
                    case BusinessEnum.SellGoodsType.Lifter_Simple:// 简易型升降机方式
                        // 重新设置升降机串口
                        m_SellEquUp.Reset(Convert.ToInt32(AsileOper.VendBoxList[intVendBoxIndex].ShippPort));

                        intErrCode = m_SellEquUp.QueryAisleInfo(intTrayNum, intColumnNum, out strValue);
                        if (intErrCode == 0)
                        {
                            strPaStatus = strValue;
                        }
                        break;
                    #endregion
                }

                AddBusLog_Code(strLogType, intErrCode.ToString(), strValue);

                if (intErrCode == 0)
                {
                    AsileOper.CurrentMcdInfo.PaStatus = strPaStatus;
                    if ((strPaStatus == "03") || (strPaStatus == "04"))
                    {
                        // 没有安装电机
                        AsileOper.AsileList[paIndex].PaKind = "2";
                    }
                    if (AsileOper.AsileList[paIndex].PaStatus != strPaStatus)
                    {
                        AsileOper.AsileList[paIndex].PaStatus = strPaStatus;

                        if (isSend)
                        {
                            // 向网关汇报信息
                            int intNetCode = m_GateSocket.UpdatePaStatus(strPaStatus, strPaCode);
                        }
                    }

                    // 检查升降机状态
                }
            }
            catch (Exception ex)
            {
                intErrCode = ERR_SYS;
                AddErrLog(strLogType, intErrCode.ToString(), ex.Message);
            }
            return intErrCode;
        }

        /// <summary>
        /// 查询升降机设备状态
        /// </summary>
        /// <returns></returns>
        private int QueryLifterStatus(int vendBoxIndex)
        {
            int intErrCode = 0;
            string strLogType = "QueryLifterStatus";

            try
            {
                #region 如果是升降机终端，需要继续查询升降机设备状态

                if ((AsileOper.VendBoxList[vendBoxIndex].SellGoodsType == BusinessEnum.SellGoodsType.Lifter_Comp) ||
                    (AsileOper.VendBoxList[vendBoxIndex].SellGoodsType == BusinessEnum.SellGoodsType.Lifter_Simple))
                {
                    // 重新设置升降机串口
                    m_SellEquUp.Reset(Convert.ToInt32(AsileOper.VendBoxList[vendBoxIndex].ShippPort));

                    // 继续查询升降机状态，只有货道电机状态、升降机状态、光电管状态全部正常，才认为正常
                    string strUpDownStatus = string.Empty;
                    intErrCode = m_SellEquUp.QueryUpDownStatus(out strUpDownStatus);
                    AddBusLog(strLogType + "  Lifter  Box:" + AsileOper.VendBoxList[vendBoxIndex].VendBoxCode + 
                        "  Code:" + intErrCode.ToString() + "  " + strUpDownStatus);
                    if (intErrCode == 0)
                    {
                        /* 升降机状态说明
                        /// 01：故障
    /// 02：正常
    /// 03：升降机位置不在初始位
    /// 04：纵向电机卡塞
    /// 05：接货台不在初始位
    /// 06：横向电机卡塞
    /// 07：小门电机卡塞
    /// 08：接货台有货
    /// 09：接货台电机卡塞
    /// 10：取货口有货
                        */

                        switch (strUpDownStatus)
                        {
                            case "02":// 正常

                                if (AsileOper.VendBoxList[vendBoxIndex].UpDownIsQueryElectStatus == BusinessEnum.ControlSwitch.Run)
                                {
                                    #region 升降机系统状态正常，检测光电管状态

                                    string strGuangValue = string.Empty;
                                    intErrCode = m_SellEquUp.QueryElectDoorStatus(out strGuangValue);
                                    AddBusLog(strLogType + "  GuangDian  Code:" + intErrCode.ToString() + "  " + strGuangValue);
                                    if (intErrCode == 0)
                                    {
                                        string[] hexValuesSplit = strGuangValue.Split('|');
                                        if (hexValuesSplit.Length > 10)
                                        {
                                            if ((hexValuesSplit[2] == "00") &&
                                                (hexValuesSplit[4] == "00") &&
                                                (hexValuesSplit[7] == "00") &&
                                                (hexValuesSplit[8] == "00") &&
                                                (hexValuesSplit[9] == "00"))
                                            {

                                            }
                                            else
                                            {
                                                strUpDownStatus = "11";// 其它的认为光电管故障
                                            }
                                        }
                                    }

                                    #endregion
                                }

                                break;

                            default:
                                break;
                        }

                        #region 如果升降机状态发生改变，发送网关数据

                        #endregion

                        AsileOper.VendBoxList[vendBoxIndex].LifterStatus = strUpDownStatus;

                        ////CurrentMcdInfo.PaStatus = strUpDownStatus;
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                intErrCode = ERR_SYS;
                AddErrLog(strLogType, intErrCode.ToString(), ex.Message);
            }

            return intErrCode;
        }

        #endregion

        #region 纸币器/硬币器相关控制

        /// <summary>
        /// 使能/禁能纸币器
        /// </summary>
        /// <param name="controlType">控制类型 0：禁能 1：使能</param>
        /// <param name="isQueryResult">是否需要查询操作结果 False：否 True：是</param>
        /// <returns>结果代码</returns>
        public int ControlCash(string controlType,bool isQueryResult)
        {
            string statusCode = "FF";
            int intErrCode = 0;

            string strLogType = "ControlCash";

            DeviceInfo.CashEnableKind = BusinessEnum.EnableKind.Unknown;

            try
            {
                ////if ((PaymentOper.PaymentList.Cash.ControlSwitch == BusinessEnum.ControlSwitch.Run) &&
                ////    (DeviceInfo.KmbControlSwitch == BusinessEnum.ControlSwitch.Run))
                if (DeviceInfo.KmbControlSwitch == BusinessEnum.ControlSwitch.Run)
                {
                    // 如果安装了纸币器
                    string strValue = string.Empty;
                    intErrCode = m_KmbOper.ControlCash(controlType, out strValue);
                    AddBusLog_Code(strLogType + "  Type:" + controlType, intErrCode.ToString(), strValue);
                    if (intErrCode == 0)
                    {
                        string[] hexValue = strValue.Split('|');
                        if (hexValue.Length > 1)
                        {
                            statusCode = hexValue[1];

                            if (isQueryResult)
                            {
                                #region 2015-01-21 使能或禁能纸币器后，查询是否成功

                                if ((statusCode == "00") || (statusCode == "02"))
                                {
                                    // 如果此时返回的纸币器状态为00（纸币器禁能）或02（纸币器正常），则发送查询金额指令
                                    // 获取纸币器禁能或使能的结果

                                    #region 发送查询纸币器、硬币器状态指令

                                    string _returnValue = string.Empty;
                                    int intCode = 0;

                                    for (int i = 0; i < 5; i++)
                                    {
                                        Thread.Sleep(100);
                                        intCode = m_KmbOper.QueryCashCoinStatus(out _returnValue);
                                        ////intCode = m_KmbOper.QueryMoney(out _returnValue);

                                        AddBusLog_Code("Query ControlCash Result", intCode.ToString(), _returnValue);
                                        if (intCode == 0)
                                        {
                                            // 纸币器状态|硬币器状态|等
                                            string[] hexCashValue = _returnValue.Split('|');
                                            if (hexCashValue.Length > 1)
                                            {
                                                statusCode = hexCashValue[0];
                                                if ((statusCode != "00") && (statusCode != "02"))
                                                {
                                                    // 如果纸币器状态即不是禁能也不是正常，则退出
                                                    break;
                                                }
                                                if ((controlType == "0") && (statusCode == "00"))
                                                {
                                                    // 如果是让纸币器禁能，且此时纸币器状态为禁能时，则退出查询指令
                                                    break;
                                                }
                                                if ((controlType == "1") && (statusCode != "00"))
                                                {
                                                    // 如果是让纸币器使能，且此时纸币器状态为正常，则退出查询指令
                                                    break;
                                                }
                                            }
                                        }
                                    }

                                    #endregion
                                }

                                #endregion
                            }

                            if ((statusCode == "01") || (statusCode == "FF"))
                            {
                                statusCode = "01";
                            }

                            CheckIsSendDeviceSataus(BusinessEnum.DeviceList.BillValidator, statusCode, "");
                        }

                        switch (controlType)
                        {
                            case "0":// 禁能
                                DeviceInfo.CashEnableKind = BusinessEnum.EnableKind.Disable;
                                break;
                            default:
                                DeviceInfo.CashEnableKind = BusinessEnum.EnableKind.Enable;
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                intErrCode = ERR_SYS;
                AddErrLog(strLogType + "  Type:" + controlType, intErrCode.ToString(), ex.Message);
            }

            return intErrCode;
        }

        /// <summary>
        /// 使能/禁能硬币器
        /// </summary>
        /// <param name="controlType">控制类型 0：禁能 1：使能</param>
        /// <param name="isQueryResult">是否查询操作结果 False：否 True：是</param>
        /// <returns>结果代码</returns>
        public int ControlCoin(string controlType, bool isQueryResult)
        {
            string statusCode = "FF";

            string strValue = string.Empty;

            int intErrCode = 0;

            string strLogType = "ControlCoin";

            DeviceInfo.CoinEnableKind = BusinessEnum.EnableKind.Unknown;

            try
            {
                ////if ((PaymentOper.PaymentList.Cash.ControlSwitch == BusinessEnum.ControlSwitch.Run) &&
                ////    (DeviceInfo.KmbControlSwitch == BusinessEnum.ControlSwitch.Run))
                if (DeviceInfo.KmbControlSwitch == BusinessEnum.ControlSwitch.Run)
                {
                    intErrCode = m_KmbOper.ControlCoin(controlType, out strValue);
                    AddBusLog_Code(strLogType + "  Type:" + controlType, intErrCode.ToString(), strValue);
                    if (intErrCode == 0)
                    {
                        string[] hexValue = strValue.Split('|');
                        if (hexValue.Length > 1)
                        {
                            statusCode = hexValue[1];

                            #region 2015-01-21 使能或禁能硬币器后，查询是否成功

                            if ((statusCode == "00") || (statusCode == "02"))
                            {
                                // 如果此时返回的硬币器状态为00（硬币器禁能）或02（硬币器正常），则发送查询当前可找硬币金额指令
                                // 获取硬币器禁能或使能的结果

                                #region 发送查询当前可找硬币金额指令

                                string _coinValue = string.Empty;
                                int intCode = 0;

                                for (int i = 0; i < 2; i++)
                                {
                                    Thread.Sleep(100);
                                    intCode = m_KmbOper.QueryCoinMoney(out _coinValue);

                                    AddBusLog_Code("Query ControlCoin Result", intCode.ToString(), _coinValue);
                                    if (intCode == 0)
                                    {
                                        // 执行结果|硬币器状态|可找硬币金额
                                        string[] hexCoinValue = _coinValue.Split('|');
                                        if (hexCoinValue.Length > 2)
                                        {
                                            if (hexCoinValue[0] == "00")
                                            {
                                                #region
                                                statusCode = hexCoinValue[1];
                                                if ((statusCode != "00") && (statusCode != "02"))
                                                {
                                                    // 如果硬币器状态即不是禁能也不是正常，则退出
                                                    break;
                                                }
                                                if ((controlType == "0") && (statusCode == "00"))
                                                {
                                                    // 如果是让硬币器禁能，且此时硬币器状态为禁能时，则退出查询指令
                                                    break;
                                                }
                                                if ((controlType == "1") && (statusCode != "00"))
                                                {
                                                    // 如果是让硬币器使能，且此时硬币器状态不为禁能，则退出查询指令
                                                    break;
                                                }
                                                #endregion
                                            }
                                            else
                                            {
                                                #region
                                                statusCode = hexCoinValue[1];
                                                #endregion
                                            }
                                        }
                                    }
                                }

                                #endregion
                            }

                            #endregion

                            if ((statusCode == "01") || (statusCode == "FF"))
                            {
                                statusCode = "01";
                            }
                            CheckIsSendDeviceSataus(BusinessEnum.DeviceList.CoinChanger, statusCode, "");
                        }

                        switch (controlType)
                        {
                            case "0":// 禁能
                                DeviceInfo.CoinEnableKind = BusinessEnum.EnableKind.Disable;
                                break;
                            default:
                                DeviceInfo.CoinEnableKind = BusinessEnum.EnableKind.Enable;
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                intErrCode = ERR_SYS;
                AddErrLog(strLogType + "  Type:" + controlType, intErrCode.ToString(), ex.Message);
            }

            return intErrCode;
        }

        /// <summary>
        /// 检测纸币器/硬币器状态
        /// </summary>
        /// <returns>结果代码</returns>
        private int CheckPayMentStatus(bool isMonitor)
        {
            string strValue = string.Empty;
            string strCashStatus = "01";
            string strCoinStatus = "01";

            string strLogType = "CheckPayMentStatus";

            int intErrCode = 0;

            try
            {
                if (PaymentOper.PaymentList.Cash.ControlSwitch == BusinessEnum.ControlSwitch.Stop)
                {
                    strCashStatus = "00";
                    strCoinStatus = "00";
                }
                else
                {
                    intErrCode = m_KmbOper.QueryCashCoinStatus(out strValue);
                    ////intErrCode = m_KmbOper.QueryMoney(out strValue);

                    if (intErrCode == 0)
                    {
                        // 获取纸币器/硬币器状态
                        string[] hexValue = strValue.Split('|');
                        if (hexValue.Length > 2)
                        {
                            strCashStatus = hexValue[0];
                            strCoinStatus = hexValue[1];
                            DeviceInfo.MoneyRecType = hexValue[2];

                            if (strCashStatus == "00")
                            {
                                // 纸币器被禁能
                                DeviceInfo.CashEnableKind = BusinessEnum.EnableKind.Disable;
                                strCashStatus = "02";
                            }
                            if (strCoinStatus == "00")
                            {
                                // 硬币器被禁能
                                DeviceInfo.CoinEnableKind = BusinessEnum.EnableKind.Disable;
                                strCoinStatus = "02";
                            }
                            
                            if (isMonitor)
                            {
                                if ((hexValue[0] != "00") && (hexValue[0] != "FF"))
                                {
                                    // 纸币器可能没有被禁能
                                    DeviceInfo.CashEnableKind = BusinessEnum.EnableKind.Enable;
                                }
                                if ((hexValue[1] != "00") && (hexValue[1] != "FF"))
                                {
                                    // 硬币器可能没有被禁能
                                    DeviceInfo.CoinEnableKind = BusinessEnum.EnableKind.Enable;
                                }
                            }
                        }
                    }

                    ////bool blnIsWriteLog = false;
                    ////if ((isMonitor) && (m_CheckPayMentStatusValue != strValue))
                    ////{
                    ////    m_CheckPayMentStatusValue = strValue;
                    ////    blnIsWriteLog = true;
                    ////}
                    ////if (!isMonitor)
                    ////{
                    ////    blnIsWriteLog = true;
                    ////}
                    ////if (blnIsWriteLog)
                    ////{
                        AddBusLog_Code(strLogType, intErrCode.ToString(), strValue);
                    ////}
                }

                if ((strCashStatus == "01") || (strCashStatus == "FF"))
                {
                    strCashStatus = "01";
                }
                if ((strCoinStatus == "01") || (strCoinStatus == "FF"))
                {
                    strCoinStatus = "01";
                }

                CheckIsSendDeviceSataus(BusinessEnum.DeviceList.BillValidator, strCashStatus,"");
                CheckIsSendDeviceSataus(BusinessEnum.DeviceList.CoinChanger, strCoinStatus,"");
            }
            catch (Exception ex)
            {
                intErrCode = ERR_SYS;
                AddErrLog(strLogType, intErrCode.ToString(), ex.Message);
            }

            return intErrCode;
        }

        /// <summary>
        /// 退币
        /// </summary>
        /// <param name="money">要退币的金额</param>
        /// <param name="money1">退币后的控制主板当前余额</param>
        /// <returns>结果代码</returns>
        private int ReturnMoney(int money, string returnSource,out int money1)
        {
            int intErrCode = ERR_SYS;

            string strValue = string.Empty;

            string strLogType = "ReturnMoney";

            money1 = 0;

            try
            {
                AddBusLog(strLogType + "  Money:" + money.ToString() + "  Source:" + returnSource);

                intErrCode = m_KmbOper.ReturnMoney(money, out strValue);

                string strNewCoinStatus = "";// 当前最新硬币器状态
                bool blnIsCoinStatus = false;// 是否更新硬币器状态

                if (intErrCode == 0)
                {
                    // 退币指令发送成功
                    string[] hexValue = strValue.Split('|');
                    if (hexValue.Length > 2)
                    {
                        if (hexValue[0] == "00")
                        {
                            // 退币成功 执行结果|硬币器状态|退币后余额（00|02|0 ）
                            // 2015-07-27 退币成功后增加相关数据 执行结果|硬币器状态|退币后余额|退纸币金额|退硬币金额|1元硬币数量（00|02|0 ）
                            money1 = Convert.ToInt32(hexValue[2]);// 获取退币后余额
                            int intFactMoney = money - money1;

                            // 记录退币数据记录 
                            AddMoneyData("0", "1", intFactMoney, 1);

                            #region 2015-07-27 计算找零的各货币的面值

                            ////////if (hexValue.Length > 4)
                            ////////{
                            ////////    int intBillMoney = Convert.ToInt32(hexValue[3]);// 退纸币金额
                            ////////    int intCoinMoney = Convert.ToInt32(hexValue[4]);// 退硬币金额
                            ////////    int intCoinNum = Convert.ToInt32(hexValue[5]);// 1元硬币数量

                            ////////    if (ConfigInfo.IsReturnBill == BusinessEnum.ControlSwitch.Run)
                            ////////    {
                            ////////        // 如果纸币找零功能开启
                            ////////        if ((ConfigInfo.ReturnBillMoney > 0) && (intBillMoney > 0))
                            ////////        {
                            ////////            int intBillNum = intBillMoney / ConfigInfo.ReturnBillMoney;
                            ////////            // 记录纸币找零库存，减少
                            ////////            CashInfoOper.UpdateCashStockNum(ConfigInfo.ReturnBillMoney, intBillNum, "1", "1");s
                            ////////        }
                            ////////    }

                            ////////    #region 计算硬币找零

                            ////////    int intCoinTotalMoney = intFactMoney - intBillMoney;// 获取硬币实际找零总金额

                            ////////    if (intCoinNum > 0)
                            ////////    {
                            ////////        // 记录1元硬币库存，减少
                            ////////        CashInfoOper.UpdateCashStockNum(100, intCoinNum, "0", "1");
                            ////////    }

                            ////////    ////if (intBillMoney > 0)
                            ////////    ////{
                            ////////    ////    // 有纸币找零
                                    
                            ////////    ////}

                            ////////    #endregion
                            ////////}

                            #endregion

                            #region 计算找零的各货币面值

                            ////int intMaxCashValue = 0;
                            ////int intCashValue = 0;
                            ////// 获取找零最大面值
                            ////for (int i = 0; i < CashStockOper.CashStockList.Count; i++)
                            ////{
                            ////    intCashValue = CashStockOper.CashStockList[i].CashValue;
                            ////    if (intCashValue > intMaxCashValue)
                            ////    {
                            ////        intMaxCashValue = intCashValue;
                            ////    }
                            ////}

                            ////// 获取各找零面值的找零个数


                            #endregion

                            // 向网关汇报信息
                            m_GateSocket.OperMoney(m_BusId.ToString(), "0", "1", intFactMoney, 1);
                        }
                        else
                        {
                            // 退币失败，硬币器故障
                            intErrCode = 18;
                        }
                        strNewCoinStatus = hexValue[1];
                        blnIsCoinStatus = true;
                    }
                }
                else
                {
                    if (intErrCode == 14)
                    {
                        // 硬币器无币可找
                        strNewCoinStatus = "02";// strNewCoinStatus = "FD";//认为硬币器正常
                        blnIsCoinStatus = true;
                    }
                }

                AddBusLog_Code(strLogType, intErrCode.ToString(), strValue);

                ClearTunMoneyTime();// 更新退币最新开始计算时间

                if (blnIsCoinStatus)
                {
                    // 检测是否需要更新硬币器状态
                    CheckIsSendDeviceSataus(BusinessEnum.DeviceList.CoinChanger, strNewCoinStatus,"");
                }
            }
            catch (Exception ex)
            {
                intErrCode = ERR_SYS;
                AddErrLog(strLogType, intErrCode.ToString(), ex.Message);
            }

            return intErrCode;
        }

        /// <summary>
        /// 吞币
        /// </summary>
        /// <param name="money">要吞币的金额</param>
        /// <returns>结果代码</returns>
        private int TunMoney(int money)
        {
            int intErrCode = 0;
            string strLogType = "TunMoney";

            intErrCode = ClearUsableMoney(false);

            if (intErrCode == 0)
            {
                // 保存吞币数据 // 向网关汇报信息
                m_GateSocket.OperMoney(m_BusId.ToString(), "0", "2", money, 1);
            }

            AddBusLog(strLogType + "  " + money.ToString() + "  " + intErrCode.ToString());

            return intErrCode;
        }

        /// <summary>
        /// 清除当前可用金额
        /// </summary>
        /// <returns></returns>
        public int ClearUsableMoney(bool isUsable)
        {
            int intErrCode = 0;

            int strValue = 0;

            string strLogType = "ClearUsableMoney";

            //if ((DeviceInfo.CashControlSwitch == BusinessEnum.ControlSwitch.Run) &&
            //    (DeviceInfo.CoinControlSwitch == BusinessEnum.ControlSwitch.Run))
            if (DeviceInfo.KmbControlSwitch == BusinessEnum.ControlSwitch.Run)
            {
                intErrCode = m_KmbOper.SynMoney("1", out strValue);
                AddBusLog_Code(strLogType, intErrCode.ToString() + "  TotalMoney:" + m_TotalPayMoney.ToString(),
                    strValue.ToString());
                if (isUsable)
                {
                    if (intErrCode == 0)
                    {
                        m_TotalPayMoney = 0;
                    }
                }
                else
                {
                    m_TotalPayMoney = 0;

                    intErrCode = 0;
                }
            }

            return intErrCode;
        }

        /// <summary>
        /// 获取当前可用金额
        /// </summary>
        /// <param name="usableMoney">当前可用金额</param>
        /// <returns>错误代码</returns>
        private int GetUsableMoney(out int usableMoney)
        {
            int intErrCode = 0;

            usableMoney = 0;

            string strLogType = "GetUsableMoney";

            if (DeviceInfo.KmbControlSwitch == BusinessEnum.ControlSwitch.Run)
            {
                intErrCode = ERR_SYS;
                intErrCode = m_KmbOper.SynMoney("0", out usableMoney);

                AddBusLog_Code(strLogType, intErrCode.ToString(), usableMoney.ToString());
            }
            return intErrCode;
        }

        /// <summary>
        /// 查询当前可找硬币金额
        /// </summary>
        /// <param name="isEnoughCoin">硬币是否不足 False：充足 True：不足</param>
        /// <param name="value">执行结果|硬币器状态|可找硬币金额</param>
        /// <returns>错误代码</returns>
        public int QueryCoinMoney(out bool isEnoughCoin, out string value)
        {
            int intErrCode = 0;

            string strValue = string.Empty;

            isEnoughCoin = false;

            value = string.Empty;

            string strLogType = "QueryCoinMoney";

            if (DeviceInfo.KmbControlSwitch == BusinessEnum.ControlSwitch.Run)
            {
                intErrCode = m_KmbOper.QueryCoinMoney(out strValue);

                value = strValue;

                if (intErrCode == 0)
                {
                    // 执行结果|硬币器状态|可找硬币金额
                    string[] hexValue = strValue.Split('|');
                    if (hexValue.Length > 2)
                    {
                        if (hexValue[0] == "00")
                        {
                            strValue = hexValue[1];

                            #region 2016-07-21 临时注释
                            ////if (strValue == "FD")
                            ////{
                            ////    // 零钱不足
                            ////    isEnoughCoin = true;
                            ////}
                            ////else
                            ////{
                            ////    #region 2015-07-23 如果有纸币找零，则获取当前可找纸币金额（临时测试）

                            ////    //////if (ConfigInfo.IsReturnBill == BusinessEnum.ControlSwitch.Run)
                            ////    //////{
                            ////    //////    int int10Money = Convert.ToInt32(hexValue[2]);
                            ////    //////    CashStockOper.UpdateCashStockNum(1000, int10Money / 1000, "1","0");
                            ////    //////}

                            ////    #endregion
                            ////}
                            #endregion
                        }
                        else
                        {
                            strValue = string.Empty;
                            isEnoughCoin = true;
                        }
                    }
                    else
                    {
                        intErrCode = ERR_SYS;
                    }
                }

                AddBusLog_Code(strLogType, intErrCode.ToString(), value);
            }
            else
            {
                isEnoughCoin = true;
            }

            return intErrCode;
        }

        /// <summary>
        /// 根据零钱充足或不充足来控制纸币器
        /// </summary>
        /// <param name="isActual">是否实时刷新纸币器控制策略</param>
        /// <param name="isRefresh">零钱充足或不充足状态是否发生了改变 False：没有改变 True：改变</param>
        /// <param name="outValue"></param>
        public void ControlCashEnoughCoin(bool isActual, out bool isRefresh, out string outValue)
        {
            bool blnIsEnouthCoin = false;
            string strOutValue = string.Empty;
            bool blnIsRefreshCash = false;// 是否刷新纸币器的控制策略
            outValue = string.Empty;

            isRefresh = false;

            int intCode = QueryCoinMoney(out blnIsEnouthCoin, out outValue);
            if (intCode == 0)
            {
                if (DeviceInfo.IsEnougthCoin != blnIsEnouthCoin)
                {
                    isRefresh = true;

                    DeviceInfo.IsEnougthCoin = blnIsEnouthCoin;

                    blnIsRefreshCash = true;
                }

                if (isActual)
                {
                    // 需要实时刷新纸币器控制策略
                    blnIsRefreshCash = true;
                }

                if (blnIsRefreshCash)
                {
                    #region 2017-07-21 临时测试【注释】

                    ////////// 需要刷新纸币器的控制策略，根据零钱是否充足来控制纸币器
                    ////////if (blnIsEnouthCoin)
                    ////////{
                    ////////    // 零钱不足，禁能纸币器
                    ////////    intCode = ControlCash("0",true);
                    ////////}
                    ////////else
                    ////////{
                    ////////    // 零钱充足，使能纸币器
                    ////////    intCode = ControlCash("1",true);
                    ////////}

                    #endregion
                }
            }
        }

        /// <summary>
        /// 纸币币种使能/禁能
        /// </summary>
        /// <param name="operType">控制类型，0：禁能 1：使能</param>
        /// <param name="channelCode">纸币通道号</param>
        /// <returns></returns>
        public int ControlBillType(int cashValue,string operType, string channelCode)
        {
            int intErrCode = 0;
            if (ConfigInfo.CashManagerModel == BusinessEnum.CashManagerModel.Advance)
            {
                intErrCode = m_KmbOper.ControlBillType(operType, channelCode);
                if (intErrCode == 0)
                {
                    // 保存币种使能/禁能状态
                    bool result = CashInfoOper.UpdateCashStatus_All(cashValue, "1", operType);
                }
            }
            return intErrCode;
        }

        /// <summary>
        /// 查询纸币币种接收使能/禁能状态
        /// </summary>
        /// <param name="cashValue"></param>
        /// <param name="channelCode"></param>
        /// <returns></returns>
        public int QueryBillType(int cashValue,string channelCode,out string _status)
        {
            int intErrCode = ERR_SYS;
            string _value = string.Empty;
            _status = string.Empty;
            string strLogType = "QueryBillType";
            if (ConfigInfo.CashManagerModel == BusinessEnum.CashManagerModel.Advance)
            {
                intErrCode = m_KmbOper.QueryBillType(channelCode, out _value);
                if (intErrCode == 0)
                {
                    string[] hexValue = _value.Split('|');
                    if (hexValue.Length > 1)
                    {
                        intErrCode = 0;
                        // 保存币种使能/禁能状态
                        string strNewStatus = Convert.ToInt32(hexValue[0]).ToString();
                        _status = strNewStatus;
                        bool result = CashInfoOper.UpdateCashStatus_All(cashValue, "1", strNewStatus);
                    }
                    else
                    {
                        intErrCode = ERR_SYS;
                    }
                }
            }
            else
            {
                strLogType += "  No";
                intErrCode = 0;
            }

            AddBusLog_Code(strLogType, intErrCode.ToString() + "  CashValue:" + cashValue.ToString() + "  Channel:" + channelCode, _value);

            return intErrCode;
        }

        /// <summary>
        /// 查询各面值货币库存余量
        /// </summary>
        /// <param name="cashType">货币类型 0：硬币 1：纸币</param>
        /// <param name="cashValue">货币面值</param>
        /// <param name="stockNum">库存余量</param>
        /// <param name="source">来源途径</param>
        /// <param name="isPush">是否在投币业务流程中 False：不在 True：在</param>
        /// <returns></returns>
        public int QueryDenomChangeNum(string cashType, int cashValue,string source,bool isPush,out int stockNum)
        {
            int intErrCode = ERR_SYS;
            stockNum = 0;
            string _value = string.Empty;
            string strLogType = "QueryDenomChangeNum";
            string strSourceType = source;
            bool blnIsRefreshStock = true;// 是否覆盖刷新货币库存，False：不覆盖，正常处理 True：覆盖刷新
            try
            {
                if (ConfigInfo.CashManagerModel == BusinessEnum.CashManagerModel.Advance)
                {
                    if (isPush)
                    {
                        #region 如果是用户实际投币
                        switch (source)
                        {
                            case "00":// 硬币（进硬币币筒或进Hopper找零箱）
                            case "01":// 纸币（进纸币钞箱）
                                strSourceType = "1";
                                break;
                            default:
                                strSourceType = "2";
                                break;
                        }

                        if (cashType == "0")
                        {
                            // 如果是硬币
                            if (ConfigInfo.CoinDeviceType == BusinessEnum.CoinDeviceType.CoinDevice)
                            {
                                // 如果是普通硬币器
                                blnIsRefreshStock = false;
                            }
                            else
                            {
                                // Hook投币器，可以刷新覆盖
                                blnIsRefreshStock = true;
                            }
                        }
                        if (cashType == "1")
                        {
                            // 如果是纸币
                            switch (strSourceType)
                            {
                                case "2":// 纸币进入找零器
                                    // 需要刷新覆盖纸币器找零库存余量
                                    blnIsRefreshStock = true;
                                    break;
                                default:// 认为纸币进入钞箱
                                    blnIsRefreshStock = false;
                                    break;
                            }
                        }
                        #endregion
                    }

                    if (!blnIsRefreshStock)
                    {
                        // 不需覆盖刷新货币库存，按正常增减流程处理
                        CashInfoOper.UpdateCashStockNum(cashValue, 1, cashType, strSourceType, "0");// 增加库存
                        intErrCode = 0;
                    }
                    else
                    {
                        // 如果覆盖刷新货币库存
                        intErrCode = m_KmbOper.QueryDenomChangeNum(cashType, cashValue, out _value);
                        if (intErrCode == 0)
                        {
                            intErrCode = ERR_SYS;
                            string[] hexValue = _value.Split('|');
                            if (hexValue.Length > 1)
                            {
                                if (hexValue[0] == "00")
                                {
                                    intErrCode = 0;
                                    stockNum = Convert.ToInt32(hexValue[1]);
                                    CashInfoOper.UpdateCashStockNum(cashValue, stockNum, cashType,"0", "2");// 覆盖库存
                                }
                            }
                        }
                        AddBusLog_Code(strLogType, intErrCode.ToString() + "  CashType:" + cashType + ",CashValue:" + cashValue.ToString(), _value);
                    }
                }
                else
                {
                    intErrCode = 0;
                }
                
            }
            catch(Exception ex)
            {
                intErrCode = ERR_SYS;
                AddErrLog(strLogType, intErrCode.ToString(), ex.Message);
            }

            return intErrCode;
        }

        /// <summary>
        /// 清点指定面值的货币
        /// </summary>
        /// <param name="cashType">货币类型 0：硬币 1：纸币</param>
        /// <param name="cashValue">货币面值</param>
        /// <param name="stockNum">货币余量</param>
        /// <returns></returns>
        public int QingDianCashStock(string cashType,int cashValue,out int stockNum)
        {
            int intErrCode = 0;
            stockNum = 0;

            string _value = string.Empty;
            string strLogType = "QingDianCashStock";

            // 执行结果|货币面值|货币余量
            intErrCode = m_KmbOper.CheckCoinNum(cashType, cashValue, out _value);
            if (intErrCode == 0)
            {
                string[] hexValue = _value.Split('|');
                if (hexValue.Length > 2)
                {
                    if (hexValue[0] != "00")
                    {
                        intErrCode = Convert.ToInt32(hexValue[0]);
                    }
                    else
                    {
                        stockNum = Convert.ToInt32(hexValue[2]);
                        string strSource = "0";
                        if (cashType == "0")
                        {
                            strSource = "1";// 进入Hopper
                        }
                        if (cashType == "1")
                        {
                            strSource = "2";// 进入纸币找零箱
                        }
                        // 更新该货币当前库存余量
                        CashInfoOper.UpdateCashStockNum(cashValue, stockNum, cashType, strSource, "2");// 覆盖库存
                    }
                }
                else
                {
                    intErrCode = 9;
                }
            }

            AddBusLog_Code(strLogType, intErrCode.ToString() + "  CashType:" + cashType + ",CashValue:" + cashValue.ToString(), _value);

            return intErrCode;
        }

        /// <summary>
        /// 检测货币管理模式
        /// </summary>
        private void CheckCashManagerModel()
        {
            int intErrCode = ERR_SYS;
            string _value = string.Empty;
            for (int i = 0; i < CashInfoOper.CashInfoList.Count; i++)
            {
                intErrCode = m_KmbOper.QueryDenomChangeNum(CashInfoOper.CashInfoList[i].CashType, CashInfoOper.CashInfoList[i].CashValue, out _value);
                if (intErrCode == 0)
                {
                    ConfigInfo.CashManagerModel = BusinessEnum.CashManagerModel.Advance;
                }
                else
                {
                    ConfigInfo.CashManagerModel = BusinessEnum.CashManagerModel.Singal;
                }
                if (i == 0)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 查询所有硬币及可找零纸币的当前可找余量
        /// </summary>
        private void QueryDenomChangeNum_AllCoin()
        {
            int intErrCode = 0;
            int intStockNum = 0;
            bool blnQuery = false;

            if (ConfigInfo.CoinDeviceType == BusinessEnum.CoinDeviceType.CoinDevice)
            {
                // 如果是普通硬币器
                string _value = string.Empty;
                
                for (int i = 0; i < 20; i++)
                {
                    intErrCode = m_KmbOper.QueryReturnDetail(out _value);
                    if (intErrCode != 0)
                    {
                        break;
                    }
                    else
                    {
                        string[] hexValue = _value.Split('|');
                        if (hexValue.Length > 4)
                        {
                            // 获取执行结果
                            if (hexValue[0] != "00")
                            {
                                // 失败
                                break;
                            }
                            else
                            {
                                // 获取是否结束
                                if (hexValue[1] == "01")
                                {
                                    // 已经结束
                                    break;
                                }
                                else
                                {
                                    // 获取货币类型

                                    // 获取货币面值

                                    // 获取退币数量
                                    // 更新硬币库存
                                    CashInfoOper.UpdateCashStockNum(Convert.ToInt32(hexValue[3]), Convert.ToInt32(hexValue[4]),
                                        Convert.ToInt32(hexValue[2]).ToString(), "0", "1");
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < CashInfoOper.CashInfoList.Count; i++)
                {
                    blnQuery = false;
                    if (CashInfoOper.CashInfoList[i].CashType == "0")
                    {
                        if (ConfigInfo.CoinDeviceType == BusinessEnum.CoinDeviceType.Hook)
                        {
                            // 如果是Hook找零设备，则查询该面值硬币的库存余量
                            blnQuery = true;
                        }
                    }
                    else
                    {
                        // 如果是现金，且现金找零打开，且现金面值为找零的现金面值
                        if ((ConfigInfo.IsReturnBill == BusinessEnum.ControlSwitch.Run) &&
                            (ConfigInfo.ReturnBillMoney == CashInfoOper.CashInfoList[i].CashValue))
                        {
                            blnQuery = true;
                        }
                    }
                    if (blnQuery)
                    {
                        intErrCode = QueryDenomChangeNum(CashInfoOper.CashInfoList[i].CashType, CashInfoOper.CashInfoList[i].CashValue,
                            "00", false, out intStockNum);
                        Thread.Sleep(20);
                    }
                }
            }
        }

        /// <summary>
        /// 查询各面值纸币接收使能或禁能状态
        /// </summary>
        private void QueryBillStatus_All()
        {
            int intErrCode = ERR_SYS;
            string _status = string.Empty;
            for (int i = 0; i < CashInfoOper.CashInfoList.Count; i++)
            {
                if (CashInfoOper.CashInfoList[i].CashType == "1")
                {
                    intErrCode = QueryBillType(CashInfoOper.CashInfoList[i].CashValue, CashInfoOper.CashInfoList[i].Channel.ToString(), out _status);
                }
            }
        }

        #endregion

        #region 机器设备相关控制

        /// <summary>
        /// 查询机器制冷/风机/照明/除雾设备状态
        /// </summary>
        /// <param name="vendBoxCode">机柜编号索引</param>
        /// <returns>结果代码</returns>
        private int QueryEqu1Status(int vendBoxIndex)
        {
            int intErrCode = 0;

            string strValue = string.Empty;

            string strLogType = "QueryEqu1Status";

            try
            {
                int intVendBoxCode = Convert.ToInt32(AsileOper.VendBoxList[vendBoxIndex].VendBoxCode);
                switch (AsileOper.VendBoxList[vendBoxIndex].SellGoodsType)
                {
                    case BusinessEnum.SellGoodsType.Spring:// 该货柜属于弹簧出货

                        #region 货柜出货方式属于弹簧出货

                        intErrCode = m_KmbOper.QueryControlStatus(intVendBoxCode, out strValue);
                        
                        if (intErrCode == 0)
                        {
                            AsileOper.VendBoxList[vendBoxIndex].VendBoxStatus = true;

                            // 解析各设备状态，主要有：制冷设备状态|制冷风机状态|照明灯状态|除雾器状态|广告灯状态
                            string[] hexValue = strValue.Split('|');
                            if (hexValue.Length > 3)
                            {
                                AsileOper.VendBoxList[vendBoxIndex].RefControl.ControlStatus = FunHelper.ChangeDeviceControlStatus_Ref(hexValue[0]);// 制冷设备状态
                                AsileOper.VendBoxList[vendBoxIndex].DraughtFanControl.ControlStatus = FunHelper.ChangeDeviceControlStatus(hexValue[1]);// 风机状态
                                AsileOper.VendBoxList[vendBoxIndex].LightControl.ControlStatus = FunHelper.ChangeDeviceControlStatus(hexValue[2]);// 照明灯状态
                                AsileOper.VendBoxList[vendBoxIndex].DemisterControl.ControlStatus = FunHelper.ChangeDeviceControlStatus(hexValue[3]);// 除雾器状态
                                AsileOper.VendBoxList[vendBoxIndex].AdvertLightControl.ControlStatus = FunHelper.ChangeDeviceControlStatus(hexValue[4]);// 广告灯状态
                            }
                        }
                        else
                        {
                            if (intErrCode == 10)
                            {
                                // 不存在机柜编号
                                AsileOper.VendBoxList[vendBoxIndex].VendBoxStatus = false;
                            }
                        }
                        //strQueryValue = strLogType + "  " + "Code：" + "  " + intErrCode.ToString() + "  " + "Data：" + strValue;

                        #endregion

                        CheckKmbConnect(intErrCode);

                        break;

                    case BusinessEnum.SellGoodsType.Lifter_Comp:// 该货柜属于复杂型升降机出货
                    case BusinessEnum.SellGoodsType.Lifter_Simple:// 简易型升降机

                        #region 货柜出货方式属于升降机出货

                        // 重新设置升降机串口
                        m_SellEquUp.Reset(Convert.ToInt32(AsileOper.VendBoxList[vendBoxIndex].ShippPort));
                        intErrCode = m_SellEquUp.QueryControlStatus(out strValue);

                        string strDriverBoard = "01";// 驱动板状态
                        if (intErrCode == 0)
                        {
                            // 解析各设备状态，主要有：加热器|风机|紫外灯|照明灯1|照明灯2
                            string[] hexValue = strValue.Split('|');
                            if (hexValue.Length > 3)
                            {
                                strDriverBoard = "02";
                                AsileOper.VendBoxList[vendBoxIndex].RefControl.ControlStatus = FunHelper.ChangeDeviceControlStatus_Ref(hexValue[0]);// 制冷设备状态
                                AsileOper.VendBoxList[vendBoxIndex].DraughtFanControl.ControlStatus = FunHelper.ChangeDeviceControlStatus(hexValue[1]);// 风机状态
                                AsileOper.VendBoxList[vendBoxIndex].UltravioletLampControl.ControlStatus = FunHelper.ChangeDeviceControlStatus(hexValue[2]);// 紫外灯状态
                                AsileOper.VendBoxList[vendBoxIndex].LightControl.ControlStatus = FunHelper.ChangeDeviceControlStatus(hexValue[3]);// 照明灯状态
                                AsileOper.VendBoxList[vendBoxIndex].AdvertLightControl.ControlStatus = FunHelper.ChangeDeviceControlStatus(hexValue[4]);// 广告灯状态
                            }
                        }
                        CheckIsSendDeviceSataus_Box(vendBoxIndex,BusinessEnum.DeviceList.DriverBoard, strDriverBoard, "");// 驱动板状态

                        #endregion

                        break;
                }

                ////if (m_QueryEqu1StatusValue != (intErrCode.ToString() + strValue))
                ////{
                ////    m_QueryEqu1StatusValue = intErrCode.ToString() + strValue;

                AddBusLog_Code(strLogType, intErrCode.ToString(), intVendBoxCode.ToString() + "  " + strValue);
                ////}
            }
            catch (Exception ex)
            {
                intErrCode = ERR_SYS;
                AddErrLog(strLogType, intErrCode.ToString(), ex.Message);
            }

            return intErrCode;
        }

        /// <summary>
        /// 设置压缩机控制策略
        /// </summary>
        /// <param name="vendBoxCode">机柜编号索引</param>
        /// <returns>结果代码</returns>
        private int SetRefrigerationTactics(int vendboxIndex,BusinessEnum.DeviceControlStatus deviceControlStatus)
        {
            int intErrCode = ERR_SYS;
            bool result = false;

            // 货柜出货方式为弹簧出货，则只在控制参数需要刷新改变时，才发送控制指令；
            // 货柜出货方式为升降机，则每次都要监控处理控制策略
            switch (AsileOper.VendBoxList[vendboxIndex].SellGoodsType)
            {
                case BusinessEnum.SellGoodsType.Spring:// 弹簧方式出货
                    if (AsileOper.VendBoxList[vendboxIndex].RefControl.IsRefreshCfg)
                    {
                        result = RefreshDeviceControlCfg(vendboxIndex, BusinessEnum.DeviceList.Refrigeration);
                        if (result)
                        {
                            intErrCode = SetEquControlTactics(Convert.ToInt32(AsileOper.VendBoxList[vendboxIndex].VendBoxCode), "0",
                                AsileOper.VendBoxList[vendboxIndex].RefControl.ControlModel,
                                AsileOper.VendBoxList[vendboxIndex].RefControl.BeginTime1 + 
                                AsileOper.VendBoxList[vendboxIndex].RefControl.EndTime1,
                                AsileOper.VendBoxList[vendboxIndex].RefControl.BeginTime2 +
                                AsileOper.VendBoxList[vendboxIndex].RefControl.EndTime2,
                                Convert.ToInt32(AsileOper.VendBoxList[vendboxIndex].TargetTmp));
                            if (intErrCode == 0)
                            {
                                AsileOper.VendBoxList[vendboxIndex].RefControl.IsRefreshCfg = false;
                            }
                        }
                    }
                    break;
                case BusinessEnum.SellGoodsType.Lifter_Comp:// 升降机方式出货
                case BusinessEnum.SellGoodsType.Lifter_Simple:// 
                    intErrCode = SetEquControlTactics_NoLine(vendboxIndex,BusinessEnum.DeviceList.Refrigeration,
                            DeviceControlRole.CheckRefControlRole(AsileOper.VendBoxList[vendboxIndex].RefControl,
                            AsileOper.VendBoxList[vendboxIndex].TmpControlModel,
                            AsileOper.VendBoxList[0].DoorStatus,
                            AsileOper.VendBoxList[vendboxIndex].TmpStatus, AsileOper.VendBoxList[vendboxIndex].TmpValue,
                            AsileOper.VendBoxList[vendboxIndex].TargetTmp));
                    break;
                default:
                    intErrCode = 0;
                    break;
            }

            Thread.Sleep(30);

            return intErrCode;
        }

        /// <summary>
        /// 设置照明设备控制策略
        /// </summary>
        /// <param name="vendBoxCode">机柜编号 1：第一个机柜 2：第二个机柜</param>
        /// <returns>结果代码</returns>
        private int SetJacklightTactics(int vendboxIndex, BusinessEnum.DeviceControlStatus deviceControlStatus)
        {
            int intErrCode = ERR_SYS;
            bool result = false;

            if (AsileOper.VendBoxList[vendboxIndex].IsControlCircle == "0")
            {
                // 该货柜不具有回路控制板，不能控制设备
                return 0;
            }

            // 货柜出货方式为弹簧出货，则只在控制参数需要刷新改变时，才发送控制指令；
            // 货柜出货方式为升降机，则每次都要监控处理控制策略
            switch (AsileOper.VendBoxList[vendboxIndex].SellGoodsType)
            {
                case BusinessEnum.SellGoodsType.Spring:// 弹簧方式出货
                    if (AsileOper.VendBoxList[vendboxIndex].LightControl.IsRefreshCfg)
                    {
                        result = RefreshDeviceControlCfg(vendboxIndex, BusinessEnum.DeviceList.Light);
                        if (result)
                        {
                            intErrCode = SetEquControlTactics(Convert.ToInt32(AsileOper.VendBoxList[vendboxIndex].VendBoxCode), "2",
                                AsileOper.VendBoxList[vendboxIndex].LightControl.ControlModel,
                                AsileOper.VendBoxList[vendboxIndex].LightControl.BeginTime1 +
                                AsileOper.VendBoxList[vendboxIndex].LightControl.EndTime1,
                                AsileOper.VendBoxList[vendboxIndex].LightControl.BeginTime2 +
                                AsileOper.VendBoxList[vendboxIndex].LightControl.EndTime2, 0);
                            if (intErrCode == 0)
                            {
                                AsileOper.VendBoxList[vendboxIndex].LightControl.IsRefreshCfg = false;
                            }
                        }
                    }
                    break;
                case BusinessEnum.SellGoodsType.Lifter_Comp:// 升降机方式出货
                case BusinessEnum.SellGoodsType.Lifter_Simple:// 
                    intErrCode = SetEquControlTactics_NoLine(vendboxIndex, BusinessEnum.DeviceList.Light,
                            DeviceControlRole.CheckDeviceControlRole(AsileOper.VendBoxList[vendboxIndex].LightControl));
                    break;
                default:
                    intErrCode = 0;
                    break;
            }

            Thread.Sleep(30);

            return intErrCode;
        }

        /// <summary>
        /// 设置广告灯控制策略
        /// </summary>
        /// <param name="vendBoxCode">机柜编号 1：第一个机柜 2：第二个机柜</param>
        /// <returns>结果代码</returns>
        private int SetAdvertLightTactics(int vendboxIndex, BusinessEnum.DeviceControlStatus deviceControlStatus)
        {
            int intErrCode = ERR_SYS;
            bool result = false;

            if (AsileOper.VendBoxList[vendboxIndex].IsControlCircle == "0")
            {
                // 该货柜不具有回路控制板，不能控制设备
                return 0;
            }

            // 货柜出货方式为弹簧出货，则只在控制参数需要刷新改变时，才发送控制指令；
            // 货柜出货方式为升降机，则每次都要监控处理控制策略
            switch (AsileOper.VendBoxList[vendboxIndex].SellGoodsType)
            {
                case BusinessEnum.SellGoodsType.Spring:// 弹簧方式出货
                    if (AsileOper.VendBoxList[vendboxIndex].AdvertLightControl.IsRefreshCfg)
                    {
                        result = RefreshDeviceControlCfg(vendboxIndex, BusinessEnum.DeviceList.AdvertLight);
                        if (result)
                        {
                            intErrCode = SetEquControlTactics(Convert.ToInt32(AsileOper.VendBoxList[vendboxIndex].VendBoxCode), "4",
                                AsileOper.VendBoxList[vendboxIndex].AdvertLightControl.ControlModel,
                                AsileOper.VendBoxList[vendboxIndex].AdvertLightControl.BeginTime1 +
                                AsileOper.VendBoxList[vendboxIndex].AdvertLightControl.EndTime1,
                                AsileOper.VendBoxList[vendboxIndex].AdvertLightControl.BeginTime2 +
                                AsileOper.VendBoxList[vendboxIndex].AdvertLightControl.EndTime2, 0);
                            if (intErrCode == 0)
                            {
                                AsileOper.VendBoxList[vendboxIndex].AdvertLightControl.IsRefreshCfg = false;
                            }
                        }
                    }
                    break;
                case BusinessEnum.SellGoodsType.Lifter_Comp:// 升降机方式出货
                case BusinessEnum.SellGoodsType.Lifter_Simple:// 
                    intErrCode = SetEquControlTactics_NoLine(vendboxIndex, BusinessEnum.DeviceList.AdvertLight,
                            DeviceControlRole.CheckDeviceControlRole(AsileOper.VendBoxList[vendboxIndex].AdvertLightControl));
                    break;
                default:
                    intErrCode = 0;
                    break;
            }

            Thread.Sleep(30);

            return intErrCode;
        }

        /// <summary>
        /// 设置紫外灯设备控制策略
        /// </summary>
        /// <param name="vendBoxCode">机柜编号 1：第一个机柜 2：第二个机柜</param>
        /// <returns>结果代码</returns>
        private int SetUltravioletLampTactics(int vendboxIndex, BusinessEnum.DeviceControlStatus deviceControlStatus)
        {
            int intErrCode = ERR_SYS;
            bool result = false;

            if (AsileOper.VendBoxList[vendboxIndex].IsControlCircle == "0")
            {
                // 该货柜不具有回路控制板，不能控制设备
                return 0;
            }

            // 货柜出货方式为弹簧出货，则只在控制参数需要刷新改变时，才发送控制指令；
            // 货柜出货方式为升降机，则每次都要监控处理控制策略
            switch (AsileOper.VendBoxList[vendboxIndex].SellGoodsType)
            {
                case BusinessEnum.SellGoodsType.Spring:// 弹簧方式出货
                    if (AsileOper.VendBoxList[vendboxIndex].UltravioletLampControl.IsRefreshCfg)
                    {
                        result = RefreshDeviceControlCfg(vendboxIndex, BusinessEnum.DeviceList.UltravioletLamp);
                        if (result)
                        {
                            ////intErrCode = SetEquControlTactics(Convert.ToInt32(AsileOper.VendBoxList[vendboxIndex].VendBoxCode), "1",
                            ////    AsileOper.VendBoxList[vendboxIndex].UltravioletLampControl.ControlModel,
                            ////    AsileOper.VendBoxList[vendboxIndex].UltravioletLampControl.BeginTime1 +
                            ////    AsileOper.VendBoxList[vendboxIndex].UltravioletLampControl.EndTime1,
                            ////    AsileOper.VendBoxList[vendboxIndex].UltravioletLampControl.BeginTime2 +
                            ////    AsileOper.VendBoxList[vendboxIndex].UltravioletLampControl.EndTime2, 0);
                            intErrCode = 0;
                            if (intErrCode == 0)
                            {
                                AsileOper.VendBoxList[vendboxIndex].UltravioletLampControl.IsRefreshCfg = false;
                            }
                        }
                    }
                    break;
                case BusinessEnum.SellGoodsType.Lifter_Comp:// 升降机方式出货
                case BusinessEnum.SellGoodsType.Lifter_Simple:// 
                    intErrCode = SetEquControlTactics_NoLine(vendboxIndex, BusinessEnum.DeviceList.UltravioletLamp,
                            DeviceControlRole.CheckDeviceControlRole(AsileOper.VendBoxList[vendboxIndex].UltravioletLampControl));
                    break;
                default:
                    intErrCode = 0;
                    break;
            }

            Thread.Sleep(30);

            return intErrCode;
        }

        /// <summary>
        /// 设置除雾设备控制策略
        /// </summary>
        /// <param name="vendBoxCode">机柜编号 1：第一个机柜 2：第二个机柜</param>
        /// <returns>结果代码</returns>
        private int SetDemisterTactics(int vendboxIndex, BusinessEnum.DeviceControlStatus deviceControlStatus)
        {
            int intErrCode = ERR_SYS;
            bool result = false;

            if (AsileOper.VendBoxList[vendboxIndex].IsControlCircle == "0")
            {
                // 该货柜不具有回路控制板，不能控制设备
                return 0;
            }

            // 货柜出货方式为弹簧出货，则只在控制参数需要刷新改变时，才发送控制指令；
            // 货柜出货方式为升降机，则每次都要监控处理控制策略
            switch (AsileOper.VendBoxList[vendboxIndex].SellGoodsType)
            {
                case BusinessEnum.SellGoodsType.Spring:// 弹簧方式出货
                    if (AsileOper.VendBoxList[vendboxIndex].DemisterControl.IsRefreshCfg)
                    {
                        result = RefreshDeviceControlCfg(vendboxIndex, BusinessEnum.DeviceList.Demister);
                        if (result)
                        {
                            intErrCode = SetEquControlTactics(Convert.ToInt32(AsileOper.VendBoxList[vendboxIndex].VendBoxCode), "3",
                                AsileOper.VendBoxList[vendboxIndex].DemisterControl.ControlModel,
                                AsileOper.VendBoxList[vendboxIndex].DemisterControl.BeginTime1 +
                                AsileOper.VendBoxList[vendboxIndex].DemisterControl.EndTime1,
                                AsileOper.VendBoxList[vendboxIndex].DemisterControl.BeginTime2 +
                                AsileOper.VendBoxList[vendboxIndex].DemisterControl.EndTime2, 0);
                            if (intErrCode == 0)
                            {
                                AsileOper.VendBoxList[vendboxIndex].DemisterControl.IsRefreshCfg = false;
                            }
                        }
                    }
                    break;
                case BusinessEnum.SellGoodsType.Lifter_Comp:// 升降机方式出货
                case BusinessEnum.SellGoodsType.Lifter_Simple:// 
                    intErrCode = 0;
                    break;
                default:
                    intErrCode = 0;
                    break;
            }

            Thread.Sleep(30);

            return intErrCode;
        }

        /// <summary>
        /// 设置工控机/显示器/机箱风扇控制策略
        /// </summary>
        /// <returns>错误代码</returns>
        private int SetComputerTactics()
        {
            int intErrCode = ERR_SYS;

            #region 临时测试

            ////////switch (DeviceInfo.DriveConnType)
            ////////{
            ////////    case BusinessEnum.ControlSwitch.Run:// 驱动板连接到控制主板上
            ////////        ////ComputerControl.ControlModel = SysCfgOper.GetSysCfgValue("PowerModel");

            ////////        ////ComputerControl.BeginTime1 = SysCfgOper.GetSysCfgValue("PowerBeginTime1");
            ////////        ////ComputerControl.EndTime1 = SysCfgOper.GetSysCfgValue("PowerEndTime1");
            ////////        ////ComputerControl.BeginTime2 = SysCfgOper.GetSysCfgValue("PowerBeginTime2");
            ////////        ////ComputerControl.EndTime2 = SysCfgOper.GetSysCfgValue("PowerEndTime1");

            ////////        intErrCode = SetEquControlTactics(1, "5",
            ////////            ComputerControl.ControlModel,
            ////////            ComputerControl.BeginTime1 + ComputerControl.EndTime1,
            ////////            ComputerControl.BeginTime2 + ComputerControl.EndTime2, 0);
            ////////        break;

            ////////    case BusinessEnum.ControlSwitch.Stop:// 驱动板没有连接到控制主板上
            ////////        break;
            ////////}

            #endregion

            return intErrCode;
        }

        /// <summary>
        /// 设置制冷压缩机/风机/照明/除雾设备/广告灯控制策略—弹簧方式出货
        /// </summary>
        /// <param name="vendBoxCode">机柜编号</param>
        /// <param name="equType">设备类型，0：制冷压缩机 2：照明设备 3：除雾设备 4：广告灯 5：工控机/显示器/机箱风扇</param>
        /// <param name="workModel">控制模式 0：定时开启 1：全时段开启 2：全时段关闭</param>
        /// <param name="time1">定时时间段1</param>
        /// <param name="time2">定时时间段2</param>
        /// <param name="targetTmp">目标温度，只对设备类型为0时启作用，3-15之间</param>
        /// <returns>错误代码</returns>
        private int SetEquControlTactics(int vendBoxCode, string equType,
            string workModel, string time1, string time2, int targetTmp)
        {
            int intErrCode = ERR_SYS;

            string strLogType = "SetEquControlTactics";

            intErrCode = m_KmbOper.SetEquControlTactics(vendBoxCode, equType, workModel,
                time1, time2, targetTmp);

            if (intErrCode == 10)
            {
                // 不存在机柜编号 临时注释
                ////////DeviceInfo.VendBoxStatus = false;
            }

            strLogType = strLogType + "  EquType:" + equType + "  Model:" + workModel;

            AddBusLog_Code(strLogType, intErrCode.ToString(), "");

            CheckKmbConnect(intErrCode);

            return intErrCode;
        }

        /// <summary>
        /// 设置制冷压缩机/风机/照明/紫外灯/广告灯控制策略—升降机方式出货
        /// </summary>
        /// <param name="deviceName"></param>
        /// <param name="controlStatus"></param>
        /// <returns></returns>
        private int SetEquControlTactics_NoLine(int vendBoxIndex,BusinessEnum.DeviceList deviceName,BusinessEnum.DeviceControlStatus controlStatus)
        {
            int intErrCode = 0;

            string strLogType = "SetEquControlTactics_NoLine";

            #region 临时注释

            if ((controlStatus == BusinessEnum.DeviceControlStatus.Close) ||
                (controlStatus == BusinessEnum.DeviceControlStatus.Open))
            {
                string strControlKind = "0";
                if (controlStatus == BusinessEnum.DeviceControlStatus.Open)
                {
                    strControlKind = "1";
                }

                switch (deviceName)
                {
                    case BusinessEnum.DeviceList.Refrigeration:// 制冷压缩机
                        intErrCode = m_SellEquUp.ControlAllHeatBooster(strControlKind);
                        if (intErrCode == 0)
                        {
                            AsileOper.VendBoxList[vendBoxIndex].RefControl.ControlStatus = controlStatus;
                            switch (controlStatus)
                            {
                                case BusinessEnum.DeviceControlStatus.Close:
                                    AsileOper.VendBoxList[vendBoxIndex].RefControl.LastCloseTime = DateTime.Now;// 记录最后一次关闭时间
                                    break;
                                case BusinessEnum.DeviceControlStatus.Open://
                                    AsileOper.VendBoxList[vendBoxIndex].RefControl.LastOpenTime = DateTime.Now;// 记录最后一次打开时间
                                    break;
                            }
                            // 同时操作风机
                            intErrCode = m_SellEquUp.ControlAllDraughtFan("1");
                            if (intErrCode == 0)
                            {
                                AsileOper.VendBoxList[vendBoxIndex].DraughtFanControl.ControlStatus = BusinessEnum.DeviceControlStatus.Open;
                            }
                        }
                        break;

                    case BusinessEnum.DeviceList.AdvertLight:// 广告灯
                        intErrCode = m_SellEquUp.ControlAdvertLamp2(strControlKind);
                        if (intErrCode == 0)
                        {
                            AsileOper.VendBoxList[vendBoxIndex].AdvertLightControl.ControlStatus = controlStatus;
                        }
                        break;

                    case BusinessEnum.DeviceList.Light:// 照明灯
                        intErrCode = m_SellEquUp.ControlAdvertLamp1(strControlKind);
                        if (intErrCode == 0)
                        {
                            AsileOper.VendBoxList[vendBoxIndex].LightControl.ControlStatus = controlStatus;
                        }
                        break;

                    case BusinessEnum.DeviceList.UltravioletLamp:// 紫外灯
                        intErrCode = m_SellEquUp.ControlUltravioletLamp(strControlKind);
                        if (intErrCode == 0)
                        {
                            AsileOper.VendBoxList[vendBoxIndex].UltravioletLampControl.ControlStatus = controlStatus;
                        }
                        break;

                    case BusinessEnum.DeviceList.Sound:// 声音设备
                        intErrCode = m_SellEquUp.ControlUltravioletLamp(strControlKind);
                        if (intErrCode == 0)
                        {
                            SoundControl.ControlStatus = controlStatus;
                        }
                        break;
                }

                AddBusLog_Code(strLogType + "  " + deviceName.ToString() + "  " + strControlKind, intErrCode.ToString(), "");
            }

            #endregion

            return intErrCode;
        }

        #endregion

        #region 刷卡终端相关控制

        #region 储值卡

        /// <summary>
        /// 初始化刷卡终端设备
        /// </summary>
        /// <returns></returns>
        private int InitICCard()
        {
            string strPort = SysCfgOper.GetSysCfgValue("IcPort");

            string strLogType = "InitICCard";
            AddBusLog(strLogType + "  " + "Port:" + strPort);

            int intErrCode = m_CardOper.Initialize(Convert.ToInt32(strPort));

            string strIcStatus = string.Empty;
            if (intErrCode != 0)
            {
                strIcStatus = "01";
            }
            else
            {
                strIcStatus = "02";
            }
            DeviceInfo.ICStatus.Status = strIcStatus;

            AddBusLog_Code(strLogType, intErrCode.ToString(), strIcStatus);

            return intErrCode;
        }

        /// <summary>
        /// 查询刷卡终端设备状态
        /// </summary>
        /// <returns>设备状态 True：正常 False：故障</returns>
        private bool QueryIcStatus()
        {
            bool result = false;

            string strIcStatus = string.Empty;
            if (PaymentOper.PaymentList.IC.ControlSwitch == BusinessEnum.ControlSwitch.Run)
            {
                string strLogType = "QueryIcStatus";

                // 如果开启刷卡终端
                int intErrCode = m_CardOper.QueryIcStatus(out strIcStatus);

                if ((intErrCode == 0) && (strIcStatus == "02"))
                {
                    result = true;
                }

                AddBusLog_Code(strLogType, intErrCode.ToString(), strIcStatus);
            }
            else
            {
                strIcStatus = "00";// 没有安装刷卡设备
            }

            // 检测刷卡终端设备状态是否发生了改变
            CheckIsSendDeviceSataus(BusinessEnum.DeviceList.IC, strIcStatus,"");
            
            return result;
        }

        /// <summary>
        /// 查询卡基本信息
        /// </summary>
        /// <param name="_cardData"></param>
        /// <returns>错误代码</returns>
        private int QueryCardBaseData(out string _cardData)
        {
            string _errICCode = string.Empty;

            int intErrCode = m_CardOper.QueryCardBaseData(out _cardData, out _errICCode);

            return intErrCode;
        }

        #endregion

        #region 磁条卡

        /// <summary>
        /// 初始化磁条卡终端设备
        /// </summary>
        /// <returns></returns>
        private int InitNoFeeCard()
        {
            string strLogType = "InitNoFeeCard";
            AddBusLog(strLogType + "  " + "Port:" + m_NoFeeCardOper.NoFeeCardConfig.Port);

            int intErrCode = m_NoFeeCardOper.InitNoFeeCardDevice();

            string strStatus = string.Empty;
            if (intErrCode != 0)
            {
                strStatus = "01";
            }
            else
            {
                strStatus = "02";
            }
            DeviceInfo.NoFeeCardStatus.Status = strStatus;

            AddBusLog_Code(strLogType, intErrCode.ToString(), strStatus);

            return intErrCode;
        }

        /// <summary>
        /// 查询磁条卡终端设备状态
        /// </summary>
        /// <returns>设备状态 True：正常 False：故障</returns>
        private bool QueryNoFeeCardStatus()
        {
            bool result = false;

            string strStatus = string.Empty;
            if (PaymentOper.PaymentList.NoFeeCard.ControlSwitch == BusinessEnum.ControlSwitch.Run)
            {
                string strLogType = "QueryNoFeeCardStatus";

                // 如果开启刷卡终端
                int intErrCode = m_NoFeeCardOper.CheckNoFeeCardDevice();

                if (intErrCode == 0)
                {
                    result = true;
                    strStatus = "02";
                }
                else
                {
                    strStatus = "01";
                }

                AddBusLog_Code(strLogType, intErrCode.ToString(), strStatus);
            }
            else
            {
                strStatus = "00";// 没有安装刷卡设备
            }

            // 检测刷卡终端设备状态是否发生了改变
            CheckIsSendDeviceSataus(BusinessEnum.DeviceList.NoFeeCard, strStatus, "");

            return result;
        }

        #endregion

        #endregion

        #region 条形码扫描设备相关控制 2015-01-15

        /// <summary>
        /// 初始化条形码扫描设备
        /// </summary>
        /// <returns></returns>
        private int InitBarCodeScan()
        {
            int intErrCode = 0;

            if (CheckIsBarCode())
            {
                intErrCode = BarCodeOper.InitBarCodeScan();
                DeviceInfo.BarCodeDeviceStatus.Status = BarCodeOper.BarCodeDeviceStatus;
            }

            return intErrCode;
        }

        /// <summary>
        /// 查询条形码扫描设备状态
        /// </summary>
        /// <returns>设备状态 True：正常 False：故障</returns>
        private bool QueryBarCodeScanStatus()
        {
            bool result = false;

            string strStatus = string.Empty;
            if (CheckIsBarCode())
            {
                int intErrCode = BarCodeOper.CheckDeviceStatus();

                if (intErrCode == 0)
                {
                    result = true;
                }
                strStatus = BarCodeOper.BarCodeDeviceStatus;
            }
            else
            {
                strStatus = "00";// 没有安装刷卡设备
            }

            // 检测条形码设备状态是否发生了改变
            CheckIsSendDeviceSataus(BusinessEnum.DeviceList.BarCodeScan, strStatus, "");

            return result;
        }

        /// <summary>
        /// 开启条形码扫描
        /// </summary>
        /// <returns></returns>
        private int BeginBarCodeScan()
        {
            int intErrCode = 0;

            if (CheckIsBarCode())
            {
                intErrCode = BarCodeOper.BeginScan();
            }

            return intErrCode;
        }

        /// <summary>
        /// 停止条形码扫描
        /// </summary>
        /// <returns></returns>
        private int StopBarCodeScan()
        {
            int intErrCode = 0;

            if (CheckIsBarCode())
            {
                intErrCode = BarCodeOper.StopScan();
            }

            return intErrCode;
        }

        /// <summary>
        /// 检测是否需要条形码扫描
        /// </summary>
        /// <returns></returns>
        private bool CheckIsBarCode()
        {
            if ((PaymentOper.PaymentList.AliPay_Code.ControlSwitch == BusinessEnum.ControlSwitch.Run) ||
                (PaymentOper.PaymentList.WeChatCode.ControlSwitch == BusinessEnum.ControlSwitch.Run))
            {
                return true;
            }

            return false;
        }

        #endregion

        #region 私有函数

        /// <summary>
        /// 检测是否需要发送设备状态数据—公共设备
        /// </summary>
        /// <param name="deviceName">设备名称</param>
        /// <param name="newStatus">设备新状态</param>
        private void CheckIsSendDeviceSataus(BusinessEnum.DeviceList deviceName,string newStatus,string newValue)
        {
            bool blnIsSendStatus = true;// 是否发送状态改变信息到网关
            switch (deviceName)
            {
                case BusinessEnum.DeviceList.BillValidator:// 纸币器
                    if (DeviceInfo.CashStatus.Status != newStatus)
                    {
                        // 更新纸币器状态
                        if (newStatus == "00")
                        {
                            blnIsSendStatus = false;
                        }
                        if ((DeviceInfo.CashStatus.Status == "00") && (newStatus == "02"))
                        {
                            blnIsSendStatus = false;
                        }
                        DeviceInfo.CashStatus.Status = newStatus;
                        if (blnIsSendStatus)
                        {
                            // 向网关汇报信息
                            m_GateSocket.UpdateCashStatus(newStatus);
                        }
                    }
                    break;

                case BusinessEnum.DeviceList.CoinChanger:// 硬币器
                    if (DeviceInfo.CoinStatus.Status != newStatus)
                    {
                        if (newStatus == "00")
                        {
                            blnIsSendStatus = false;
                        }
                        if ((DeviceInfo.CoinStatus.Status == "00") && (newStatus == "02"))
                        {
                            blnIsSendStatus = false;
                        }
                        // 更新硬币器状态
                        DeviceInfo.CoinStatus.Status = newStatus;
                        if (blnIsSendStatus)
                        {
                            // 向网关汇报信息
                            m_GateSocket.UpdateCoinStatus(newStatus);
                        }
                    }
                    break;

                case BusinessEnum.DeviceList.IC:// 刷卡器
                    if (DeviceInfo.ICStatus.Status != newStatus)
                    {
                        // 网关发送
                        m_GateSocket.UpdateCardStatus(newStatus);
                        DeviceInfo.ICStatus.Status = newStatus;
                    }
                    break;

                case BusinessEnum.DeviceList.NoFeeCard:// 磁条卡
                    if (DeviceInfo.NoFeeCardStatus.Status != newStatus)
                    {
                        // 网关发送
                        m_GateSocket.UpdateCardStatus(newStatus);
                        DeviceInfo.NoFeeCardStatus.Status = newStatus;
                    }
                    break;

                case BusinessEnum.DeviceList.BarCodeScan:// 条形码扫描设备
                    if (DeviceInfo.BarCodeDeviceStatus.Status != newStatus)
                    {
                        // 网关发送
                        ////m_GateSocket.UpdateCardStatus(newStatus);
                        DeviceInfo.BarCodeDeviceStatus.Status = newStatus;
                    }
                    break;
            }
        }

        /// <summary>
        /// 检测是否需要发送设备状态数据—货柜设备
        /// </summary>
        /// <param name="deviceName">设备名称</param>
        /// <param name="newStatus">设备新状态</param>
        private void CheckIsSendDeviceSataus_Box(int vendBoxIndex,BusinessEnum.DeviceList deviceName, string newStatus, string newValue)
        {
            bool blnIsSendStatus = false;// 是否发送状态改变信息到网关
            if (AsileOper.VendBoxList[vendBoxIndex].VendBoxCode == "1")
            {
                // 如果是一号柜（也即主柜），才发送门、驱动板、掉货检测的设备状态
                blnIsSendStatus = true;
            }

            switch (deviceName)
            {
                case BusinessEnum.DeviceList.Door:// 门控
                    if (AsileOper.VendBoxList[vendBoxIndex].DoorStatus != newStatus)
                    {
                        AsileOper.VendBoxList[vendBoxIndex].DoorStatus = newStatus;// 门碰开关状态
                        if (blnIsSendStatus)
                        {
                            // 如果是一号柜（也即主柜），才发送门碰状态
                            m_GateSocket.UpdateDoorStatus(newStatus);
                        }
                    }
                    break;

                case BusinessEnum.DeviceList.DriverBoard:// 驱动板
                    if (AsileOper.VendBoxList[vendBoxIndex].DriveMoardStatus != newStatus)
                    {
                        AsileOper.VendBoxList[vendBoxIndex].DriveMoardStatus = newStatus;// 驱动板状态
                        if (blnIsSendStatus)
                        {
                            m_GateSocket.UpdateMainBoardStatus(newStatus);
                        }
                    }
                    break;

                case BusinessEnum.DeviceList.DropSensor:// 掉货检测
                    if (AsileOper.VendBoxList[vendBoxIndex].DropStatus != newStatus)
                    {
                        AsileOper.VendBoxList[vendBoxIndex].DropStatus = newStatus;// 掉货检测设备状态
                        if (blnIsSendStatus)
                        {
                            m_GateSocket.UpdateDropCheckStatus(newStatus);
                        }
                    }
                    break;

                case BusinessEnum.DeviceList.Lifter:// 升降机
                    if (AsileOper.VendBoxList[vendBoxIndex].LifterStatus != newStatus)
                    {
                        // 升降机状态发生改变
                        AsileOper.VendBoxList[vendBoxIndex].LifterStatus = newStatus;// 升降机状态
                    }
                    break;

                case BusinessEnum.DeviceList.TemSensor:// 温度传感器
                    bool blnIsSendTmp = false;
                    try
                    {
                        if (AsileOper.VendBoxList[vendBoxIndex].TmpControlModel != BusinessEnum.TmpControlModel.Normal)
                        {
                            // 如果该货柜的温控模式是制冷或者加热，才处理
                            if (AsileOper.VendBoxList[vendBoxIndex].TmpStatus != newStatus)
                            {
                                blnIsSendTmp = true;
                                AsileOper.VendBoxList[vendBoxIndex].TmpStatus = newStatus;
                            }
                            if (newStatus == "00")
                            {
                                // 温度传感器正常
                                AsileOper.VendBoxList[vendBoxIndex].TmpValue = newValue;
                            }
                            else
                            {
                                AsileOper.VendBoxList[vendBoxIndex].TmpValue = "";
                            }
                            if (!blnIsSendTmp)
                            {
                                DateTime dtmNowTime = DateTime.Now;

                                TimeSpan ts1 = new TimeSpan(AsileOper.VendBoxList[vendBoxIndex].LastQueryTempTime.Ticks);
                                TimeSpan ts2 = new TimeSpan(dtmNowTime.Ticks);
                                TimeSpan ts = ts1.Subtract(ts2).Duration();
                                if (ts.Minutes > ConfigInfo.QueryTmpDelay)
                                {
                                    // 距离上次查询出温度值超过10分钟                        
                                    AsileOper.VendBoxList[vendBoxIndex].LastQueryTempTime = dtmNowTime;
                                    blnIsSendTmp = true;
                                }
                            }
                            if (blnIsSendTmp)
                            {
                                m_GateSocket.UpdateTempStatus(AsileOper.VendBoxList[vendBoxIndex].VendBoxCode, newStatus,
                                    AsileOper.VendBoxList[vendBoxIndex].TmpValue);

                                #region 向第三方汇报温度数据 2015-09-07

                                if ((ConfigInfo.O2OTake_Switch == BusinessEnum.ControlSwitch.Run) && (newStatus == "00"))
                                {
                                    O2OServerOper.Report_Tmp(AsileOper.VendBoxList[vendBoxIndex].VendBoxCode, AsileOper.VendBoxList[vendBoxIndex].TmpValue);
                                }

                                #endregion
                            }
                        }
                    }
                    catch
                    {
                    }
                    break;
            }
        }

        /// <summary>
        /// 检测是否需要吞币
        /// </summary>
        /// <returns>结果 False：不需要吞币 True：需要吞币</returns>
        public bool CheckIsTunCoin()
        {
            bool result = false;

            if (m_TotalPayMoney > 0)
            {
                DateTime dtmNowTime = DateTime.Now;

                TimeSpan ts1 = new TimeSpan(m_LastTunMoneyTime.Ticks);
                TimeSpan ts2 = new TimeSpan(dtmNowTime.Ticks);
                TimeSpan ts = ts1.Subtract(ts2).Duration();
                if (ts.Minutes > ConfigInfo.TunOutTime)
                {
                    // 距离上次吞币重新计时时间已经超过，开始吞币
                    int intErrCode = TunMoney(m_TotalPayMoney);
                    if (intErrCode == 0)
                    {
                        // 吞币成功
                        ClearTunMoneyTime();
                        ////m_LastTunMoneyTime = dtmNowTime;
                        DeviceInfo.IsClearMoney = true;

                        if (m_TotalPayMoney == 0)
                        {
                            m_SellGoodsNum = 0;
                        }
                    }
                }
            }

            return result;
        }

        #endregion

        #endregion

        #region 公共函数（硬件设备相关控制）

        /// <summary>
        /// 禁能现金支付设备
        /// </summary>
        /// <param name="isTime">是否实时禁能</param>
        /// <returns></returns>
        public int DisableCashPaymnet(bool isTime)
        {
            int intErrCode = 0;

            if (isTime)
            {
                intErrCode = ControlCash("0",true);
                intErrCode = ControlCoin("0",true);
            }
            else
            {
                if (DeviceInfo.CashEnableKind != BusinessEnum.EnableKind.Disable)
                {
                    intErrCode = ControlCash("0",true);
                }
                if (m_TotalPayMoney > 0)
                {
                    // 如果当前有剩余金额，则主界面上可以退币，使能硬币器
                    if (DeviceInfo.CoinEnableKind != BusinessEnum.EnableKind.Enable)
                    {
                        intErrCode = ControlCoin("1",true);
                    }
                }
                else
                {
                    if (DeviceInfo.CoinEnableKind != BusinessEnum.EnableKind.Disable)
                    {
                        intErrCode = ControlCoin("0",true);
                    }
                }
            }
            return intErrCode;
        }

        /// <summary>
        /// 货道电机测试
        /// </summary>
        /// <param name="paCode">货道外部编号</param>
        /// <returns>结果代码</returns>
        public int TestAsile(string paCode)
        {
            int intErrCode = ERR_SYS;

            int intPaIndex = 0;

            string strPaId = string.Empty;

            intErrCode = AsileOper.GetPaIndex(paCode, out intPaIndex);
            if (intErrCode != 0)
            {
                // 没有该货道
                return intErrCode;
            }

            strPaId = AsileOper.AsileList[intPaIndex].PaId;

            string strValue = string.Empty;

            int intVendBoxCode = Convert.ToInt32(AsileOper.AsileList[intPaIndex].VendBoxCode);

            string strLogType = "TestAsile";
            AddBusLog(strLogType + "  " + paCode + "  " + strPaId);

            int intVendBoxIndex = AsileOper.GetVendBoxIndex(intVendBoxCode.ToString());
            switch (AsileOper.VendBoxList[intVendBoxIndex].SellGoodsType)
            {
                case BusinessEnum.SellGoodsType.Spring:// 弹簧方式
                    intErrCode = m_KmbOper.TestAsile(intVendBoxCode, Convert.ToInt32(strPaId.Substring(0, 1)),
                        Convert.ToInt32(strPaId.Substring(1, 1)), out strValue);
                    break;

                case BusinessEnum.SellGoodsType.Lifter_Comp:// 复杂型升降机方式
                case BusinessEnum.SellGoodsType.Lifter_Simple:// 简易型升降机方式
                    intErrCode = m_SellEquUp.Reset(Convert.ToInt32(AsileOper.VendBoxList[intVendBoxIndex].ShippPort));

                    switch (AsileOper.VendBoxList[intVendBoxIndex].UpDownSellModel)
                    {
                        case "0":// 直接升降
                            intErrCode = m_SellEquUp.SellGoods(Convert.ToInt32(strPaId.Substring(0, 1)),
                                Convert.ToInt32(strPaId.Substring(1, 1)), out strValue);
                            break;
                        default:// 按参数升降
                            intErrCode = UpDown_SellGoods_ByParameter(intVendBoxCode.ToString(), 
                                intVendBoxIndex, intPaIndex, strPaId, out strValue);
                            break;
                    }
                        // strValue：0:成功 1：电机卡塞 2：没有货物 3：有货无法送 4：小门未打开 5：超时或失败
                        // 0：成功 1：无该货道 2：控制主板可用金额不足 3：无货 9：失败
                     switch (intErrCode)
                     {
                         case 0:
                             if ((strValue != "0") && ( strValue != "2"))
                             {
                                 intErrCode = 9;
                             }
                             else
                             {
                                 strValue = "00|02";
                             }
                             break;
                         default:// 其它原因，失败
                             intErrCode = 9;
                             break;
                     }
                    break;
            }

            if (intErrCode == 0)
            {
                #region 执行结果|货道状态

                string[] hexValue = strValue.Split('|');
                if (hexValue.Length > 1)
                {
                    // 检测执行结果和货道状态
                    string strResult = hexValue[0];
                    string strPaStatus = hexValue[1];

                    bool result = false;

                    if (strResult == "00")
                    {
                        // 出货成功
                        result = true;

                        #region 2014-12-04 增加
                        if (AsileOper.AsileList[intPaIndex].PaStatus != strPaStatus)
                        {
                            AsileOper.AsileList[intPaIndex].PaStatus = strPaStatus;

                            // 向网关汇报信息
                            int intNetCode = m_GateSocket.UpdatePaStatus(strPaStatus, paCode);
                        }
                        #endregion
                    }

                    if (!result)
                    {
                        // 认为出货失败
                        intErrCode = ERR_SYS;
                    }
                }
                else
                {
                    intErrCode = ERR_SYS;
                }

                #endregion
            }

            AddBusLog_Code(strLogType, intErrCode.ToString(), strValue);

            return intErrCode;
        }

        /// <summary>
        /// 升降机按参数升降测试
        /// </summary>
        /// <param name="paCode">货道编号</param>
        /// <param name="upDownNums">上下移动码盘</param>
        /// <param name="delayTimeNums">处货延时</param>
        /// <returns></returns>
        public int TestUpDown(BusinessEnum.SellGoodsType sellGoodsType,string paCode,int upDownNums,int delayTimeNums)
        {
            int intErrCode = ERR_SYS;

            int intPaIndex = 0;

            string strPaId = string.Empty;

            intErrCode = AsileOper.GetPaIndex(paCode, out intPaIndex);
            if (intErrCode != 0)
            {
                // 没有该货道
                return intErrCode;
            }

            strPaId = AsileOper.AsileList[intPaIndex].PaId;

            string strValue = string.Empty;

            int intVendBoxCode = Convert.ToInt32(AsileOper.AsileList[intPaIndex].VendBoxCode);

            string strLogType = "TestUpDownAsile";
            AddBusLog(strLogType + "  " + paCode + "  " + strPaId);

            int intVendBoxIndex = AsileOper.GetVendBoxIndex(intVendBoxCode.ToString());
            int intLeftRightNums = AsileOper.UpDown_GetLeftRightCodeNum(intVendBoxCode.ToString(),
                strPaId.Substring(1, 1), AsileOper.VendBoxList[intVendBoxIndex].SellGoodsType,
                ConfigInfo.UpDownLeftRightNum_Left, ConfigInfo.UpDownLeftRightNum_Center, ConfigInfo.UpDownLeftRightNum_Right);
            m_SellEquUp.Reset(Convert.ToInt32(AsileOper.VendBoxList[intVendBoxIndex].ShippPort));

            string strSellGoodsType = "1";
            if (sellGoodsType == BusinessEnum.SellGoodsType.Lifter_Simple)
            {
                strSellGoodsType = "2";
            }
            intErrCode = m_SellEquUp.SellGoods_ByParameter(strSellGoodsType,Convert.ToInt32(strPaId.Substring(0, 1)),
                            Convert.ToInt32(strPaId.Substring(1, 1)), upDownNums, intLeftRightNums, delayTimeNums,Convert.ToInt32(AsileOper.VendBoxList[intVendBoxIndex].UpDownSendGoodsTimes), out strValue);

            // strValue：0:成功 1：电机卡塞 2：没有货物 3：有货无法送 4：小门未打开 5：超时或失败
            // 0：成功 1：无该货道 2：控制主板可用金额不足 3：无货 9：失败
            switch (intErrCode)
            {
                case 0:
                    if ((strValue != "0") && (strValue != "2"))
                    {
                        intErrCode = 9;
                    }
                    else
                    {
                        strValue = "00|02";
                    }
                    break;
                default:// 其它原因，失败
                    intErrCode = 9;
                    break;
            }

            if (intErrCode == 0)
            {
                #region 执行结果|货道状态

                string[] hexValue = strValue.Split('|');
                if (hexValue.Length > 1)
                {
                    // 检测执行结果和货道状态
                    string strResult = hexValue[0];
                    string strPaStatus = hexValue[1];

                    bool result = false;

                    if (strResult == "00")
                    {
                        // 出货成功
                        result = true;
                    }

                    if (!result)
                    {
                        // 认为出货失败
                        intErrCode = ERR_SYS;
                    }
                }
                else
                {
                    intErrCode = ERR_SYS;
                }

                #endregion
            }

            AddBusLog_Code(strLogType, intErrCode.ToString(), strValue);

            return intErrCode;
        }

        /// <summary>
        /// 升降机按参数升降公共函数
        /// </summary>
        /// <param name="vendBoxCode"></param>
        /// <param name="paID"></param>
        /// <param name="_value"></param>
        /// <returns></returns>
        private int UpDown_SellGoods_ByParameter(string vendBoxCode,int vendBoxIndex,int paIndex,string paID,out string _value)
        {
            _value = string.Empty;
            int intUpDownNums = 0;
            
            for (int j = 0; j < AsileOper.UpDown_UpDownNumsList.Count; j++)
            {
                if ((AsileOper.UpDown_UpDownNumsList[j].VendBoxCode == vendBoxCode) &&
                    (AsileOper.UpDown_UpDownNumsList[j].TrayNum == AsileOper.AsileList[paIndex].TrayIndex + 1))
                {
                    intUpDownNums = AsileOper.UpDown_UpDownNumsList[j].UpDownCodeNums;
                    break;
                }
            }

            int intLeftRightNums = AsileOper.UpDown_GetLeftRightCodeNum(vendBoxCode,
                paID.Substring(1, 1), AsileOper.VendBoxList[vendBoxIndex].SellGoodsType,
                ConfigInfo.UpDownLeftRightNum_Left, ConfigInfo.UpDownLeftRightNum_Center, ConfigInfo.UpDownLeftRightNum_Right);

            int intUpDownDelayTimeNums = Convert.ToInt32(AsileOper.VendBoxList[vendBoxIndex].UpDownDelayTimeNums);

            m_SellEquUp.Reset(Convert.ToInt32(AsileOper.VendBoxList[vendBoxIndex].ShippPort));

            string strSellGoodsType = "1";
            if (AsileOper.VendBoxList[vendBoxIndex].SellGoodsType == BusinessEnum.SellGoodsType.Lifter_Simple)
            {
                strSellGoodsType = "2";
            }
            int intErrCode = m_SellEquUp.SellGoods_ByParameter(strSellGoodsType,Convert.ToInt32(paID.Substring(0, 1)),
                            Convert.ToInt32(paID.Substring(1, 1)), intUpDownNums, intLeftRightNums, intUpDownDelayTimeNums,
                            Convert.ToInt32(AsileOper.VendBoxList[vendBoxIndex].UpDownSendGoodsTimes), out _value);

            AddBusLog_Code("UpDownNums:" + intUpDownNums + ",LeftRightNums:" + intLeftRightNums +
                                ",DelayTime:" + intUpDownDelayTimeNums, intErrCode.ToString(), _value);

            return intErrCode;
        }

        #endregion

        #region 公共函数（设备设备相关检测及监控）

        /// <summary>
        /// 设备启动自检流程
        /// </summary>
        /// <returns></returns>
        public int Device_SelfCheck(bool isInit)
        {
            int intErrCode = ERR_SYS;
            string strValue = string.Empty;

            DeviceInfo.KmbConnectStatus = false;

            if (isInit)
            {
                AddBusLog("===========================================");
            }

            intErrCode = InitKmb();
            if (intErrCode != 0)
            {
                return intErrCode;
            }

            #region 初始化自检的时候要做的工作

            if (isInit)
            {
                #region 初始化刷卡设备

                if (PaymentOper.PaymentList.IC.ControlSwitch == BusinessEnum.ControlSwitch.Run)
                {
                    intErrCode = InitICCard();
                }

                #endregion

                #region 初始化非储值卡设备（磁条卡）

                if (PaymentOper.PaymentList.NoFeeCard.ControlSwitch == BusinessEnum.ControlSwitch.Run)
                {
                    intErrCode = InitNoFeeCard();
                }

                #endregion

                #region 初始化条形码扫描设备 2015-01-15

                if ((PaymentOper.PaymentList.AliPay_Code.ControlSwitch == BusinessEnum.ControlSwitch.Run) ||
                    (PaymentOper.PaymentList.WeChatCode.ControlSwitch == BusinessEnum.ControlSwitch.Run) ||
                    (PaymentOper.PaymentList.BestPay_Code.ControlSwitch == BusinessEnum.ControlSwitch.Run) ||
                    (PaymentOper.PaymentList.Volunteer_Code.ControlSwitch == BusinessEnum.ControlSwitch.Run))
                {
                    intErrCode = InitBarCodeScan();
                }

                #endregion

                #region 设置控制主板相关数据

                if (DeviceInfo.KmbControlSwitch == BusinessEnum.ControlSwitch.Run)
                {
                    // 设置控制主板时间
                    intErrCode = m_KmbOper.SetKmbTime(DateTime.Now.ToString("yyyyMMddHHmmss"), out strValue);
                    Thread.Sleep(100);
                    if (intErrCode != 0)
                    {
                        return intErrCode;
                    }

                    //设置控制主板序列号
                    intErrCode = SetKmbId();
                    Thread.Sleep(100);
                    if (intErrCode != 0)
                    {
                        return intErrCode;
                    }

                    // 查询控制主板信息
                    intErrCode = QueryKmbInfo();
                    Thread.Sleep(100);
                    if (intErrCode != 0)
                    {
                        return intErrCode;
                    }

                    // 设置控制主板通讯功能
                    if (SysCfgOper.GetSysCfgValue("NetEquKind") == "1")
                    {
                        // 如果是控制主板连接网络设备
                        intErrCode = SetKmbNetSwitch();
                        Thread.Sleep(100);
                        if (intErrCode != 0)
                        {
                            return intErrCode;
                        }
                    }

                    // 清除控制主板可用金额
                    intErrCode = ClearUsableMoney(true);
                    Thread.Sleep(100);
                    if (intErrCode != 0)
                    {
                        return intErrCode;
                    }

                    // 检测纸币器/硬币器状态
                    intErrCode = CheckPayMentStatus(false);
                    Thread.Sleep(100);
                    if (intErrCode != 0)
                    {
                        return intErrCode;
                    }

                    // 查询当前可找硬币情况
                    bool blnIsEnouthCoin = false;
                    string strOutValue = string.Empty;
                    intErrCode = QueryCoinMoney(out blnIsEnouthCoin, out strOutValue);
                    if (intErrCode != 0)
                    {
                        return intErrCode;
                    }
                    DeviceInfo.IsEnougthCoin = blnIsEnouthCoin;

                    // 初始化纸币器、硬币器等相关设备
                    intErrCode = InitDevice();
                    Thread.Sleep(100);
                    if (intErrCode != 0)
                    {
                        return intErrCode;
                    }

                    #region 2015-11-28 谷霖添加 查询各面值货币库存余量及查询纸币使能/禁能状态

                    // 检测货币管理模式
                    CheckCashManagerModel();

                    // 查询各面值货币库存余量
                    QueryDenomChangeNum_AllCoin();
                    // 查询各面值纸币接收使能/禁能状态
                    QueryBillStatus_All();

                    #endregion
                }

                #endregion

                #region 检测制冷压缩机/风机/照明/除雾设备以及掉货检测/驱动板/温度状态

                intErrCode = Device_Monitor(true);// QueryEquStatus(1);
                Thread.Sleep(100);
                AddBusLog("Device_Monitor " + intErrCode.ToString());
                if (intErrCode != 0)
                {
                    return intErrCode;
                }

                #endregion

                #region 检测货道状态

                if (!m_IsLoadAsileInfo)
                {
                    // 检测货道状态
                    string strPaNet1 = string.Empty;// 存储货道编号
                    string strPaNet2 = string.Empty;// 存储货道状态
                    string strPaNet3 = string.Empty;// 存储货道价格
                    int intPaTotal = 0;

                    int intPaListCout = AsileOper.AsileList.Count;
                    string strPaStatus = string.Empty;
                    bool blnIsUpdatePaKind = true;
                    for (int i = 0; i < intPaListCout; i++)
                    {
                        Thread.Sleep(30);
                        blnIsUpdatePaKind = true;
                        intErrCode = QueryPaStatus(i, false);

                        if ((intErrCode == 0) && (AsileOper.AsileList[i].PaKind == "0"))
                        {
                            // 如果该货道正常启用
                            if ((AsileOper.AsileList[i].PaStatus != "01") &&
                                (AsileOper.AsileList[i].PaStatus != "03") &&
                                (AsileOper.AsileList[i].PaStatus != "04"))
                            {
                                blnIsUpdatePaKind = false;
                                intPaTotal++;
                                strPaNet1 += AsileOper.AsileList[i].PaCode;
                                if (AsileOper.AsileList[i].PaStatus == "02")
                                {
                                    strPaNet2 += "0";
                                }
                                else
                                {
                                    strPaNet2 += "1";
                                }
                                ////strPaNet2 += AsileOper.AsileList[i].PaStatus;
                                strPaNet3 += AsileOper.AsileList[i].SellPrice + ",";
                            }
                        }
                        if (blnIsUpdatePaKind)
                        {
                            AsileOper.UpdateAsileKind(i);
                        }
                        if (intErrCode != 0)
                        {
                            return intErrCode;
                        }
                    }

                    // 向网关汇报货道初始化信息
                    if (intPaTotal > 0)
                    {
                        // 刷新货道信息列表
                        AsileOper.RefreshAsileInfo_Total();
                        m_GateSocket.InitAsileList(intPaTotal, strPaNet1);// 汇报货道数量
                        m_GateSocket.InitAsileStatus(intPaTotal, strPaNet2);// 汇报货道状态

                        #region 加载商品信息列表

                        LoadMcdInfo();

                        #endregion

                        m_IsLoadAsileInfo = true;
                    }
                    else
                    {
                        AddBusLog_Code("No Asile", "", "");
                        return ERR_SYS;
                    }
                }

                #endregion

                #region 汇报部件状态

                // 门状态、硬币器状态、纸币器状态、掉货检测状态、读卡器状态、
                // 驱动板0状态、温度传感器状态、驱动板0的温度

                if (AsileOper.VendBoxList.Count > 0)
                {
                    m_GateSocket.InitDeviceStatus(AsileOper.VendBoxList[0].DoorStatus + "*" +
                        DeviceInfo.CoinStatus + "*" +
                        DeviceInfo.CashStatus + "*" +
                        AsileOper.VendBoxList[0].DropStatus + "*" +
                        DeviceInfo.ICStatus + "*" +
                        AsileOper.VendBoxList[0].DriveMoardStatus + "*" +
                        AsileOper.VendBoxList[0].TmpStatus + "*" +
                        AsileOper.VendBoxList[0].TmpValue);
                }

                #endregion

                #region 汇报软件版本号

                m_GateSocket.InitSoftInfo(m_IVendSoftVer, SysCfgOper.GetSysCfgValue("NetPhone"));

                #endregion
            }

            #endregion            

            DeviceInfo.KmbConnectStatus = true;

            if (intErrCode == 0)
            {
                // 自检成功，启动发送货道配置数据线程，以减少自检时间
                Init_SendListData();
            }

            return intErrCode;
        }

        /// <summary>
        /// 发送货道弹簧圈数、设置商品、配置参数等大量数据到平台
        /// </summary>
        private void Init_SendListData()
        {
            #region 检查当天的配置信息是否已经上传

            string strNowDate = DateTime.Now.ToString("yyyyMMdd");
            if (SysCfgOper.GetSysCfgValue("UploadParaDate") == strNowDate)
            {
                // 当天的配置信息已经上传
                return;
            }

            #endregion

            #region 汇报货道弹簧圈数和货道设置商品

            if (!m_IsSendSprintNum)
            {
                AddBusLog("Send SprintNum To Server");
                string strMcdCode = string.Empty;
                for (int i = 0; i < AsileOper.AsileList.Count; i++)
                {
                    m_GateSocket.SetAsileSprintNum("1", AsileOper.AsileList[i].PaCode, AsileOper.AsileList[i].SpringNum.ToString(), "");// 货道弹簧圈数
                    strMcdCode = AsileOper.AsileList[i].McdCode;
                    if (string.IsNullOrEmpty(strMcdCode))
                    {
                        strMcdCode = "0";
                    }
                    m_GateSocket.SetAsileGoods("1", AsileOper.AsileList[i].PaCode, strMcdCode, "");// 货道设置商品
                }
                m_IsSendSprintNum = true;
            }

            #endregion

            #region 汇报参数

            if (!m_IsSendParameter)
            {
                AddBusLog("Send Parameter To Server");
                for (int i = 0; i < SysCfgOper.SysCfgList.Count; i++)
                {
                    if ((SysCfgOper.SysCfgList[i].CfgLevel == "U") &&
                        (SysCfgOper.SysCfgList[i].IsReset == "1"))
                    {
                        if (SysCfgOper.SysCfgList[i].CfgId != "702")
                        {
                            // 如果是用户级别且允许出厂初始化的参数，则发送参数值
                            m_GateSocket.UpdateParameter(SysCfgOper.SysCfgList[i].CfgId, SysCfgOper.SysCfgList[i].CfgFactValue,
                                SysCfgOper.SysCfgList[i].CfgFactValue, "");
                        }
                    }
                }
                m_IsSendParameter = true;
            }

            #endregion

            // 修改当前的参数上传日期
            UpdateSysCfgValue("UploadParaDate", strNowDate);
        }

        /// <summary>
        /// 设备监控
        /// </summary>
        /// <param name="vendBoxCode"></param>
        /// <param name="isTimeSend"></param>
        /// <returns>错误代码</returns>
        public int Device_Monitor(bool isTimeSend)
        {
            int intErrCode = 0;
            int intErrCode_UpDown = 0;
            string strValue = string.Empty;
            string strQueryValue = string.Empty;

            string strLogType = "MonitorDevice";

            #region 设备状态码说明

            /*
             * 门控状态（0：关闭 1：打开 2：故障）
            */
            #endregion

            try
            {
                #region 检测磁盘空间是否充足

                DeviceInfo.DiskSpaceEnougth = CheckDataOper.CheckDiskIsSpace(100 * 1024 * 1024);

                #endregion

                #region 如果启用了纸币器、硬币器，则检查是否处于禁能状态

                if (DeviceInfo.KmbControlSwitch == BusinessEnum.ControlSwitch.Run)
                {
                    // 查询纸币器、硬币器状态
                    intErrCode = CheckPayMentStatus(true);

                    CheckKmbConnect(intErrCode);

                    if (intErrCode == 0)
                    {
                        DisableCashPaymnet(false);
                    }
                    else
                    {
                        return intErrCode;
                    }
                }

                #endregion

                #region 查询各货柜的相关设备状态

                string strVendBoxCode = "1";
                int intCode_Temp = 0;
                for (int i = 0; i < AsileOper.VendBoxList.Count; i++)
                {
                    strVendBoxCode = AsileOper.VendBoxList[i].VendBoxCode;

                    #region 监控制冷压缩机/风机/紫外灯等设备状态

                    intCode_Temp = QueryEqu1Status(i);
                    // 如果是主柜出现问题，则退出；如果是副柜出现问题，则可以继续
                    if ((intCode_Temp != 0) && (i == 0))
                    {
                        return intCode_Temp;
                    }
                    intErrCode = intCode_Temp;

                    #endregion

                    #region 如果属于弹簧出货，需要监控门控、温度  临时注释

                    if (AsileOper.VendBoxList[i].SellGoodsType == BusinessEnum.SellGoodsType.Spring)
                    {
                        strLogType = "Mon_QueryEquStatus";
                        intErrCode = m_KmbOper.QueryEquStatus(Convert.ToInt32(strVendBoxCode), out strValue);

                        if (intErrCode == 0)
                        {
                            AsileOper.VendBoxList[i].VendBoxStatus = true;

                            #region 解析各设备状态，主要有：驱动板状态|温度传感器状态|当前温度值|目标温度值|门碰开关状态|掉货检测设备状态|网络设备状态|网络待发数据数量

                            strQueryValue = strLogType + "  " + strValue;

                            string[] hexValue = strValue.Split('|');
                            if (hexValue.Length > 5)
                            {
                                CheckIsSendDeviceSataus_Box(i,BusinessEnum.DeviceList.DriverBoard, hexValue[0], "");// 驱动板状态

                                CheckIsSendDeviceSataus_Box(i,BusinessEnum.DeviceList.TemSensor, hexValue[1], hexValue[2]);// 温度传感器状态

                                CheckIsSendDeviceSataus_Box(i,BusinessEnum.DeviceList.Door, hexValue[4], "");// 门碰开关状态

                                CheckIsSendDeviceSataus_Box(i,BusinessEnum.DeviceList.DropSensor, hexValue[5], "");// 掉货检测设备状态
                            }

                            #endregion
                        }
                        else
                        {
                            if (intErrCode == 10)
                            {
                                // 不存在机柜编号
                                AsileOper.VendBoxList[i].VendBoxStatus = false;
                            }
                        }

                        ////if (m_QueryEquStatusValue != (intErrCode.ToString() + strValue))
                        ////{
                        ////    m_QueryEquStatusValue = intErrCode.ToString() + strValue;
                        AddBusLog_Code(strLogType, intErrCode.ToString(), "Box:" + strVendBoxCode + "  " + strValue);
                        ////}
                        
                        CheckKmbConnect(intErrCode);

                        if (intErrCode != 0)
                        {
                            return intErrCode;
                        }
                    }

                    #endregion

                    #region 如果属于升降机出货，需要监控升降机状态、光电管状态、门控、温度

                    if ((AsileOper.VendBoxList[i].SellGoodsType == BusinessEnum.SellGoodsType.Lifter_Comp) ||
                        (AsileOper.VendBoxList[i].SellGoodsType == BusinessEnum.SellGoodsType.Lifter_Simple))
                    {
                        #region 发送光电管、门控查询指令

                        // 重新设置升降机串口
                        m_SellEquUp.Reset(Convert.ToInt32(AsileOper.VendBoxList[i].ShippPort));
                        intErrCode_UpDown = m_SellEquUp.QueryElectDoorStatus(out strValue);

                        AddBusLog_Code("QueryElectDoorStatus", intErrCode_UpDown.ToString(), strValue);
                        if (intErrCode_UpDown == 0)
                        {
                            string[] hexValuesSplit = strValue.Split('|');
                            if (hexValuesSplit.Length > 10)
                            {
                                CheckIsSendDeviceSataus_Box(i,BusinessEnum.DeviceList.Door, hexValuesSplit[0], "");// 门控状态
                            }
                        }
                        if (intErrCode_UpDown != 0)
                        {
                            CheckIsSendDeviceSataus_Box(i,BusinessEnum.DeviceList.DriverBoard, "01", "");// 驱动板状态（认为驱动板故障）
                            if (i == 0)
                            {
                                return intErrCode_UpDown;
                            }
                        }

                        #endregion

                        if (intErrCode_UpDown == 0)
                        {
                            #region 发送升降机状态查询指令

                            intErrCode_UpDown = m_SellEquUp.QueryUpDownStatus(out strValue);

                            AddBusLog_Code("QueryUpDownStatus", intErrCode_UpDown.ToString(), strValue);

                            if (intErrCode_UpDown == 0)
                            {
                                CheckIsSendDeviceSataus_Box(i,BusinessEnum.DeviceList.Lifter, strValue, "");// 升降机状态
                            }

                            #endregion

                            if (intErrCode_UpDown == 0)
                            {
                                #region 发送温度查询指令

                                intErrCode_UpDown = m_SellEquUp.QueryTmp(out strValue);

                                AddBusLog_Code("QueryTmp", intErrCode_UpDown.ToString(), strValue);

                                if (intErrCode_UpDown == 0)
                                {
                                    string[] hexValuesSplit = strValue.Split('|');
                                    if (hexValuesSplit.Length > 1)
                                    {
                                        CheckIsSendDeviceSataus_Box(i, BusinessEnum.DeviceList.TemSensor, hexValuesSplit[0], hexValuesSplit[1]);// 温控状态
                                    }
                                }

                                #endregion
                            }
                        }
                        if (intErrCode_UpDown != 0)
                        {
                            CheckIsSendDeviceSataus_Box(i, BusinessEnum.DeviceList.DriverBoard, "01", "");// 驱动板状态（认为驱动板故障）
                            if (i == 0)
                            {
                                return intErrCode_UpDown;
                            }
                        }
                        else
                        {
                            CheckIsSendDeviceSataus_Box(i, BusinessEnum.DeviceList.DriverBoard, "02", "");// 驱动板状态（认为驱动板正常）
                        }
                        intErrCode = intErrCode_UpDown;
                    }

                    #endregion

                    #region 监控所在货柜的制冷、照明灯等设备控制策略

                    SetRefrigerationTactics(i,BusinessEnum.DeviceControlStatus.Keep);// 设置制冷压缩机控制
                    SetJacklightTactics(i,BusinessEnum.DeviceControlStatus.Keep);// 设置照明灯控制
                    SetAdvertLightTactics(i,BusinessEnum.DeviceControlStatus.Keep);// 设置广告灯控制
                    SetUltravioletLampTactics(i,BusinessEnum.DeviceControlStatus.Keep);// 设置紫外灯控制
                    SetDemisterTactics(i,BusinessEnum.DeviceControlStatus.Keep);// 设置除雾设备控制

                    #endregion
                }

                #endregion

                return intErrCode;
            }
            catch (Exception ex)
            {
                intErrCode = ERR_SYS;
                AddErrLog(strLogType, intErrCode.ToString(), ex.Message);
                return intErrCode;
            }
            finally
            {
                ////try
                ////{
                ////    CheckKmbConnect(intErrCode);
                ////}
                ////catch
                ////{
                ////}
            }
        }

        /// <summary>
        /// 选择商品时，检测是否允许购买—正常情况购买
        /// </summary>
        /// <param name="paCode">货道编号或商品编号</param>
        /// <param name="isSend"></param>
        /// <returns></returns>
        public BusinessEnum.ServerReason Device_CheckSaleEnvir(string paCode,int sellPrice, bool isSend,bool isGift)
        {
            int intErrCode = ERR_SYS;
            BusinessEnum.ServerReason _CheckServerReason = BusinessEnum.ServerReason.Err_Other;// 其它故障
            ////string strCheckCode = "99";// 其它故障
            string strValue = string.Empty;

            string strLogType = "CheckSaleEnvir";

            /* 检测结果说明
             * 0：正常
                1：货道不存在
                2：货道暂停销售
                3：库存不足
                4：货道故障 
                5：现金支付设备故障
                6：储值卡刷卡设备故障
                7：升降系统故障，U开头表示升降系统具体故障
                8：光电管故障
                9：取货仓有货
                10：温度超限
                11：没有开通任何支付方式
             *  12：非初始卡设备故障
                99：其它故障
             * 
            */

            try
            {
                string strPaId = string.Empty;
                string strVendBoxCode = "1";

                AddBusLog("****************CheckSaleEnvir****************");
                if ((GoodsShowModelType == BusinessEnum.GoodsShowModelType.GoodsToOnlyAsile) || (isGift))
                {
                    if (AsileOper.CurrentMcdInfo != null)
                    {
                        strPaId = AsileOper.CurrentMcdInfo.PaId;
                        strVendBoxCode = AsileOper.CurrentMcdInfo.VendBoxCode;
                    }

                    AddBusLog(strLogType + "  " + strVendBoxCode + "  " + paCode + "  " + strPaId);
                }

                PaymentOper.PaymentList.Now_PayMent = BusinessEnum.PayMent.All;

                if (m_TotalPayMoney > 0)
                {
                    ClearTunMoneyTime();// 更新吞币开始计算时间
                }

                // 初始磁条卡数据
                ClearNoFeeCardData();
                // 初始条形码扫描数据
                BarCodeOper.ClearBarCodeNum();

                #region 检测磁盘空间是否充足

                if (!DeviceInfo.DiskSpaceEnougth)
                {
                    // 磁盘空间不足，不能购买
                    _CheckServerReason = BusinessEnum.ServerReason.Err_DiskSpaceNoEnougth;
                    return _CheckServerReason;
                }

                #endregion

                #region 检测相关货道信息

                _CheckServerReason = BusinessEnum.ServerReason.Normal;

                string strChoicePaCode = string.Empty;
                int paIndex = 0; 
                if ((GoodsShowModelType == BusinessEnum.GoodsShowModelType.GoodsToOnlyAsile) || (isGift))
                {
                    // 如果商品和货道一一对应
                    strChoicePaCode = paCode;

                    #region 检查货道是否存在

                    intErrCode = AsileOper.GetPaIndex(strChoicePaCode, out paIndex);
                    if (intErrCode != 0)
                    {
                        // 没有该货道
                        _CheckServerReason = BusinessEnum.ServerReason.Err_NoAsile;
                        return _CheckServerReason;
                    }

                    if (AsileOper.AsileList[paIndex].PaKind == "1")
                    {
                        // 货道被暂停销售
                        _CheckServerReason = BusinessEnum.ServerReason.Err_AsilePause;
                        return _CheckServerReason;
                    }

                    if (AsileOper.AsileList[paIndex].PaKind != "0")
                    {
                        // 不存在该货道
                        _CheckServerReason = BusinessEnum.ServerReason.Err_NoAsile;
                        return _CheckServerReason;
                    }

                    #endregion

                    #region 检测该货道库存

                    if (!AsileOper.CheckGoodsStock(strVendBoxCode,strChoicePaCode, ConfigInfo.IsRunStock))
                    {
                        // 库存不足
                        _CheckServerReason = BusinessEnum.ServerReason.Err_NoStock;
                        return _CheckServerReason;
                    }

                    #endregion

                    #region 检测货道状态及出货相关的设备状态

                    intErrCode = QueryPaStatus(strChoicePaCode, true);
                    if (intErrCode != 0)
                    {
                        _CheckServerReason = BusinessEnum.ServerReason.Err_Other;// 其它故障
                    }
                    else
                    {
                        if (AsileOper.CurrentMcdInfo.PaStatus != "02")
                        {
                            // 货道故障或不存在
                            _CheckServerReason = BusinessEnum.ServerReason.Err_AsileStatus;
                        }
                    }

                    if (_CheckServerReason != BusinessEnum.ServerReason.Normal)
                    {
                        return _CheckServerReason;
                    }

                    #endregion
                }

                if ((GoodsShowModelType == BusinessEnum.GoodsShowModelType.GoodsToMultiAsile) && (!isGift))
                {
                    // 如果商品不和货道一一对应

                    #region 检测该商品库存

                    if (!GoodsOper.CheckGoodsStock(paCode,sellPrice, ConfigInfo.IsRunStock))
                    {
                        // 库存不足
                        _CheckServerReason = BusinessEnum.ServerReason.Err_NoStock;
                        return _CheckServerReason;
                    }

                    #endregion

                    #region 选择该商品所对应的某一个货道
                    bool blnPaIsChoice = false;// 货道是否被选中 False：未选中 True：选中
                    int intPaChoiceNum = 0;// 货道被选中的个数
                    int intTotalStockNum = 0;// 该商品的总库存

                    for (int i = 0; i < AsileOper.AsileList.Count; i++)
                    {
                        if ((AsileOper.AsileList[i].McdCode == paCode) && (AsileOper.AsileList[i].SellPrice == sellPrice.ToString()))
                        {
                            AsileOper.CurrentMcdInfo = AsileOper.AsileList[i];
                            strChoicePaCode = AsileOper.AsileList[i].PaCode;
                            strVendBoxCode = AsileOper.AsileList[i].VendBoxCode;

                            blnPaIsChoice = true;

                            #region 检测该货道是否存在

                            if (AsileOper.AsileList[i].PaKind != "0")
                            {
                                // 不存在该货道
                                blnPaIsChoice = false;
                            }

                            #endregion

                            if (blnPaIsChoice)
                            {
                                #region 检测该货道下的该商品库存是否有

                                if (!AsileOper.CheckGoodsStock(strVendBoxCode,strChoicePaCode, ConfigInfo.IsRunStock))
                                {
                                    // 该货道库存不足
                                    blnPaIsChoice = false;
                                }
                                else
                                {
                                    intTotalStockNum++;
                                }

                                #endregion
                            }

                            if (blnPaIsChoice)
                            {
                                #region 检测货道状态

                                intErrCode = QueryPaStatus(strChoicePaCode, true);
                                if (intErrCode != 0)
                                {
                                    blnPaIsChoice = false;
                                    break;// 查询失败，直接退出
                                }
                                else
                                {
                                    if (AsileOper.CurrentMcdInfo.PaStatus != "02")
                                    {
                                        // 货道故障或不存在
                                        blnPaIsChoice = false;
                                    }
                                }

                                #endregion
                            }

                            if (blnPaIsChoice)
                            {
                                // 选中了该货道
                                intPaChoiceNum = 1;
                                ////AsileOper.CurrentMcdInfo = AsileOper.AsileList[i];
                                break;
                            }
                        }
                    }

                    if ((blnPaIsChoice) && (intPaChoiceNum > 0))
                    {
                        // 选中了某个货道
                        _CheckServerReason = BusinessEnum.ServerReason.Normal;
                    }
                    else
                    {
                        if (intTotalStockNum == 0)
                        {
                            // 商品库存不足
                            _CheckServerReason = BusinessEnum.ServerReason.Err_NoStock;
                        }
                        else
                        {
                            // 认为货道故障
                            _CheckServerReason = BusinessEnum.ServerReason.Err_AsileStatus;
                        }

                        return _CheckServerReason;
                    }

                    #endregion
                }

                #endregion

                #region 如果是升降出货方式，检查升降系统状态

                int intVendBoxIndex = AsileOper.GetVendBoxIndex(strVendBoxCode);

                if ((AsileOper.VendBoxList[intVendBoxIndex].SellGoodsType == BusinessEnum.SellGoodsType.Lifter_Comp) ||
                    (AsileOper.VendBoxList[intVendBoxIndex].SellGoodsType == BusinessEnum.SellGoodsType.Lifter_Simple))
                {
                    // 重新设置升降机串口
                    m_SellEquUp.Reset(Convert.ToInt32(AsileOper.VendBoxList[intVendBoxIndex].ShippPort));

                    for (int i = 0; i < 2; i++)
                    {
                        intErrCode = QueryLifterStatus(intVendBoxIndex);

                        #region 检测升降机设备信息

                        switch (AsileOper.VendBoxList[intVendBoxIndex].LifterStatus)
                        {
                            case "02":// 正常
                                _CheckServerReason = BusinessEnum.ServerReason.Normal;
                                break;

                            case "10":// 取货口有货
                                ////////if (AsileOper.VendBoxList[intVendBoxIndex].SellGoodsType == BusinessEnum.SellGoodsType.Lifter_Comp)
                                ////////{
                                ////////    _CheckServerReason = BusinessEnum.ServerReason.Err_GoodsExist;
                                ////////}
                                ////////else
                                ////////{
                                ////////    _CheckServerReason = BusinessEnum.ServerReason.Normal;
                                ////////}
                                _CheckServerReason = BusinessEnum.ServerReason.Normal;
                                break;

                            case "11":// 光电管故障
                                _CheckServerReason = BusinessEnum.ServerReason.Err_GuangDian;
                                break;

                            case "03":// 升降机位置不在初始位
                            case "04":// 纵向电机卡塞
                                _CheckServerReason = BusinessEnum.ServerReason.Err_UpDown_VertMotor;
                                break;
                            case "05":// 接货台不在初始位
                            case "06":// 横向电机卡塞
                                _CheckServerReason = BusinessEnum.ServerReason.Err_UpDown_HoriMotor;
                                break;
                            case "07":// 小门电机卡塞
                                _CheckServerReason = BusinessEnum.ServerReason.Err_UpDown_DoorMotor;
                                break;
                            case "08":// 接货台有货
                                _CheckServerReason = BusinessEnum.ServerReason.Err_UpDown_JieHuo;
                                break;
                            case "09":// 接货台电机卡塞
                                _CheckServerReason = BusinessEnum.ServerReason.Err_UpDown_JieHuoMotor;
                                break;
                            default:// 其它故障
                                _CheckServerReason = BusinessEnum.ServerReason.Err_UpDown_Other;
                                break;
                        }

                        #endregion

                        #region 如果升降机设备在某些状态，则复位下升降机，再次查询其状态

                        if ((_CheckServerReason == BusinessEnum.ServerReason.Err_UpDown_VertMotor) ||
                            (_CheckServerReason == BusinessEnum.ServerReason.Err_UpDown_HoriMotor))
                        {
                            // 如果是升降机纵向电机或横向电机故障，则复位升降机，再次查询其状态
                            intErrCode = 0;// m_SellEquUp.ResetUpDownEqu();
                            if (intErrCode == 0)
                            {
                                // 查询当前升降机状态
                                string strLifterStatus = string.Empty;
                                int intNums = 0;
                                while (true)
                                {
                                    Thread.Sleep(200);
                                    intNums++;
                                    intErrCode = m_SellEquUp.QueryUpDownStatus(out strLifterStatus);
                                    // 9：丛机正忙
                                    if (intErrCode != 9)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        if (intNums > 50)
                                        {
                                            // 超过10秒则自动退出
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            // 如果是其他设备状态，则直接退出
                            break;
                        }

                        #endregion
                    }

                    if (_CheckServerReason != BusinessEnum.ServerReason.Normal)
                    {
                        return _CheckServerReason;
                    }
                }

                #endregion

                #region 检查温度是否超限

                if (SysCfgOper.GetSysCfgValue("OutTmpWarnModel") == "1")
                {
                    bool blnIsOutTmp = false;

                    if (AsileOper.VendBoxList[intVendBoxIndex].TmpStatus != "00")
                    {
                        // 温度传感器故障
                        blnIsOutTmp = true;
                    }
                    else
                    {
                        // 如果超温停售打开，且温度传感器故障或者温度传感器正常但当前温度大于报警温度
                        if (Convert.ToInt32(AsileOper.VendBoxList[intVendBoxIndex].TmpValue) >
                            Convert.ToInt32(SysCfgOper.GetSysCfgValue("OutTmpWarnValue")))
                        {
                            blnIsOutTmp = true;
                        }
                    }
                    if (blnIsOutTmp)
                    {
                        // 温度超限
                        _CheckServerReason = BusinessEnum.ServerReason.Err_TmpLimit;
                        return _CheckServerReason;
                    }
                }

                #endregion

                if (AsileOper.CurrentMcdInfo.IsFree == "0")
                {
                    // 当前货道商品不免费
                    if (ConfigInfo.IsFreeSellNoPay == BusinessEnum.ControlSwitch.Stop)
                    {
                        // 必须支付才能出货
                        PaymentOper.PaymentList.PayShow_IDCode = BusinessEnum.PayShowStatus.Pause;
                        #region 检查支付方式

                        PaymentOper.GetEnablePayNum();
                        int intEnablePayNum = PaymentOper.PaymentList.EnablePayNum;
                        AddBusLog("EnablePayNum" + "  Num:" + intEnablePayNum.ToString());
                        if (intEnablePayNum == 0)
                        {
                            // 没有开通任何支付方式（纸币器/硬币器/刷卡器/支付宝等都不启用）
                            _CheckServerReason = BusinessEnum.ServerReason.Err_NoPayment;
                            return _CheckServerReason;
                        }

                        _CheckServerReason = BusinessEnum.ServerReason.Normal;

                        #region 检测当前支付方式的可用性

                        bool result = CheckPayEnviro();
                        if (!result)
                        {
                            // 支付方式都不能使用
                            _CheckServerReason = BusinessEnum.ServerReason.Err_NoPayment;
                        }

                        #endregion

                        #endregion
                    }
                    else
                    {
                        // 无需支付免费出货
                        int intEnablePayNum = 1;
                        AddBusLog("FreeSell No Pay" + "  Num:" + intEnablePayNum.ToString());

                        PaymentOper.PaymentList.PayShow_Cash = BusinessEnum.PayShowStatus.Pause;// 暂停现金支付

                        _CheckServerReason = BusinessEnum.ServerReason.Normal;
                        Thread.Sleep(600);
                    }
                }
                else
                {
                    #region 当前货道商品免费，只能使用身份证
                    // 关闭除身份证外的支付方式环境
                    CloseAllPayEnviro();

                    PaymentOper.PaymentList.PayShow_Cash = BusinessEnum.PayShowStatus.Pause;
                    PaymentOper.PaymentList.PayShow_IC = BusinessEnum.PayShowStatus.Pause;
                    PaymentOper.PaymentList.PayShow_AliPay_Code = BusinessEnum.PayShowStatus.Pause;
                    PaymentOper.PaymentList.PayShow_NoFeeCard = BusinessEnum.PayShowStatus.Pause;
                    PaymentOper.PaymentList.PayShow_WeChatCode = BusinessEnum.PayShowStatus.Pause;
                    PaymentOper.PaymentList.PayShow_QRCode = BusinessEnum.PayShowStatus.Pause;

                    // 打开身份证扫描环境
                    intErrCode = IDCardOper.OpenIDCard();
                    if (intErrCode == 0)
                    {
                        PaymentOper.PaymentList.PayShow_IDCode = BusinessEnum.PayShowStatus.Normal;
                        _CheckServerReason = BusinessEnum.ServerReason.Normal;
                    }
                    else
                    {
                        PaymentOper.PaymentList.PayShow_IDCode = BusinessEnum.PayShowStatus.Pause;
                        _CheckServerReason = BusinessEnum.ServerReason.Err_IDCard;
                    }
                    #endregion
                }

                #region 如果当前有现金余额，且当前现金支付方式正常

                ////if (PaymentOper.PaymentList.PayShow_Cash != BusinessEnum.PayShowStatus.Pause)
                ////{
                ////    // 检测ARM和PC上的金额是否一致
                ////    CheckARMPCMoney();
                ////}

                #endregion

                return _CheckServerReason;
            }
            catch (Exception ex)
            {
                _CheckServerReason = BusinessEnum.ServerReason.Err_Other;
                AddErrLog(strLogType, intErrCode.ToString(), ex.Message);
                return _CheckServerReason;
            }
            finally
            {
                AddBusLog(strLogType + "  Code:" + _CheckServerReason.ToString());
                AddBusLog("**********************************************");
            }
        }

        /// <summary>
        /// 检测是否允许出某商品—O2O模式（身份证取、扫码取等） 2015-08-06
        /// </summary>
        /// <param name="mcdCode">商品编号</param>
        /// <param name="codeNum">码号，如：身份证号码、取货码、条形码等</param>
        /// <param name="payType">支付类型 0：取货码 1：身份证 2：条形码</param>
        /// <returns></returns>
        public BusinessEnum.ServerReason Device_CheckSaleEnvir_O2O(string mcdCode,string codeNum,string payType)
        {
            int intErrCode = ERR_SYS;
            BusinessEnum.ServerReason _CheckServerReason = BusinessEnum.ServerReason.Err_Other;// 其它故障
            ////string strCheckCode = "99";// 其它故障
            string strValue = string.Empty;

            string strLogType = "CheckSaleEnvir";

            /* 检测结果说明
             * 0：正常
                1：货道不存在
                2：货道暂停销售
                3：库存不足
                4：货道故障 
                5：现金支付设备故障
                6：储值卡刷卡设备故障
                7：升降系统故障，U开头表示升降系统具体故障
                8：光电管故障
                9：取货仓有货
                10：温度超限
                11：没有开通任何支付方式
             *  12：非初始卡设备故障
                99：其它故障
             * 
            */

            try
            {
                string strPaId = string.Empty;
                string strVendBoxCode = "1";

                AddBusLog("****************CheckSaleEnvir O2O****************");

                #region 选择该商品所对应的某一个货道

                _CheckServerReason = BusinessEnum.ServerReason.Normal;

                _CheckServerReason = Device_CheckAsileList("3", mcdCode);
                if (_CheckServerReason != BusinessEnum.ServerReason.Normal)
                {
                    return _CheckServerReason;
                }

                #endregion

                #region 如果是升降出货方式，检查升降系统状态

                int intVendBoxIndex = AsileOper.GetVendBoxIndex(strVendBoxCode);

                if ((AsileOper.VendBoxList[intVendBoxIndex].SellGoodsType == BusinessEnum.SellGoodsType.Lifter_Comp) ||
                    (AsileOper.VendBoxList[intVendBoxIndex].SellGoodsType == BusinessEnum.SellGoodsType.Lifter_Simple))
                {
                    // 重新设置升降机串口
                    m_SellEquUp.Reset(Convert.ToInt32(AsileOper.VendBoxList[intVendBoxIndex].ShippPort));

                    for (int i = 0; i < 2; i++)
                    {
                        intErrCode = QueryLifterStatus(intVendBoxIndex);

                        #region 检测升降机设备信息

                        switch (AsileOper.VendBoxList[intVendBoxIndex].LifterStatus)
                        {
                            case "02":// 正常
                                _CheckServerReason = BusinessEnum.ServerReason.Normal;
                                break;

                            case "10":// 取货口有货
                                if (AsileOper.VendBoxList[intVendBoxIndex].SellGoodsType == BusinessEnum.SellGoodsType.Lifter_Comp)
                                {
                                    _CheckServerReason = BusinessEnum.ServerReason.Err_GoodsExist;
                                }
                                else
                                {
                                    _CheckServerReason = BusinessEnum.ServerReason.Normal;
                                }
                                break;

                            case "11":// 光电管故障
                                _CheckServerReason = BusinessEnum.ServerReason.Err_GuangDian;
                                break;

                            case "03":// 升降机位置不在初始位
                            case "04":// 纵向电机卡塞
                                _CheckServerReason = BusinessEnum.ServerReason.Err_UpDown_VertMotor;
                                break;
                            case "05":// 接货台不在初始位
                            case "06":// 横向电机卡塞
                                _CheckServerReason = BusinessEnum.ServerReason.Err_UpDown_HoriMotor;
                                break;
                            case "07":// 小门电机卡塞
                                _CheckServerReason = BusinessEnum.ServerReason.Err_UpDown_DoorMotor;
                                break;
                            case "08":// 接货台有货
                                _CheckServerReason = BusinessEnum.ServerReason.Err_UpDown_JieHuo;
                                break;
                            case "09":// 接货台电机卡塞
                                _CheckServerReason = BusinessEnum.ServerReason.Err_UpDown_JieHuoMotor;
                                break;
                            default:// 其它故障
                                _CheckServerReason = BusinessEnum.ServerReason.Err_UpDown_Other;
                                break;
                        }

                        #endregion

                        #region 如果升降机设备在某些状态，则复位下升降机，再次查询其状态

                        if ((_CheckServerReason == BusinessEnum.ServerReason.Err_UpDown_VertMotor) ||
                            (_CheckServerReason == BusinessEnum.ServerReason.Err_UpDown_HoriMotor))
                        {
                            // 如果是升降机纵向电机或横向电机故障，则复位升降机，再次查询其状态
                            intErrCode = 0;// m_SellEquUp.ResetUpDownEqu();
                            if (intErrCode == 0)
                            {
                                // 查询当前升降机状态
                                string strLifterStatus = string.Empty;
                                int intNums = 0;
                                while (true)
                                {
                                    Thread.Sleep(200);
                                    intNums++;
                                    intErrCode = m_SellEquUp.QueryUpDownStatus(out strLifterStatus);
                                    // 9：丛机正忙
                                    if (intErrCode != 9)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        if (intNums > 50)
                                        {
                                            // 超过10秒则自动退出
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            // 如果是其他设备状态，则直接退出
                            break;
                        }

                        #endregion
                    }

                    if (_CheckServerReason != BusinessEnum.ServerReason.Normal)
                    {
                        return _CheckServerReason;
                    }
                }

                #endregion

                #region 创建支付交易的相关值

                if (_CheckServerReason == BusinessEnum.ServerReason.Normal)
                {
                    m_SaleType = "1";// 更改销售方式
                    // 创建新的交易流水号
                    bool result = CreateNewBusiness();
                    // 保存扣款交易数据
                    string strCardSerNo = m_BusId.ToString();
                    string strPayType = string.Empty;
                    // 0：取货码 1：身份证 2：条形码
                    switch (payType)
                    {
                        case "0":// 取货码
                            strPayType = "82";
                            break;
                        case "1":// 身份证
                            strPayType = "85";
                            break;
                        case "2":// 条形码
                            strPayType = "81";
                            break;
                    }
                    int intNetCode = m_GateSocket.PosCard(m_BusId.ToString(), strPayType, codeNum, "", 
                        Convert.ToInt32(AsileOper.CurrentMcdInfo.SellPrice), 0, strCardSerNo, "0", "0");
                }

                #endregion

                return _CheckServerReason;
            }
            catch (Exception ex)
            {
                _CheckServerReason = BusinessEnum.ServerReason.Err_Other;
                AddErrLog(strLogType, intErrCode.ToString(), ex.Message);
                return _CheckServerReason;
            }
            finally
            {
                AddBusLog(strLogType + "  Code:" + _CheckServerReason.ToString());
                AddBusLog("**********************************************");
            }
        }

        /// <summary>
        /// 检测是否允许出某商品—微信取货码模式 2015-08-31
        /// </summary>
        /// <param name="codeNum">取货码</param>
        /// <param name="takeType">取货类型 0：用户自己选择 1：指定货到 2：随机出货</param>
        /// <param name="takePaCode">取货货道</param>
        /// <returns></returns>
        public BusinessEnum.ServerReason Device_CheckSaleEnvir_WxTakeCode(string codeNum,string takeType,string takePaCode)
        {
            int intErrCode = ERR_SYS;
            BusinessEnum.ServerReason _CheckServerReason = BusinessEnum.ServerReason.Err_Other;// 其它故障
            ////string strCheckCode = "99";// 其它故障
            string strValue = string.Empty;

            string strLogType = "CheckSaleEnvir";

            /* 检测结果说明
             * 0：正常
                1：货道不存在
                2：货道暂停销售
                3：库存不足
                4：货道故障 
                5：现金支付设备故障
                6：储值卡刷卡设备故障
                7：升降系统故障，U开头表示升降系统具体故障
                8：光电管故障
                9：取货仓有货
                10：温度超限
                11：没有开通任何支付方式
             *  12：非初始卡设备故障
                99：其它故障
             * 
            */

            try
            {
                string strPaId = string.Empty;
                string strVendBoxCode = "1";

                AddBusLog("****************CheckSaleEnvir Wx TakeCode****************");

                #region 选择某一个货道

                _CheckServerReason = BusinessEnum.ServerReason.Normal;

                _CheckServerReason = Device_CheckAsileList(takeType, takePaCode);
                if (_CheckServerReason != BusinessEnum.ServerReason.Normal)
                {
                    return _CheckServerReason;
                }

                #endregion

                #region 如果是升降出货方式，检查升降系统状态

                int intVendBoxIndex = AsileOper.GetVendBoxIndex(strVendBoxCode);

                if ((AsileOper.VendBoxList[intVendBoxIndex].SellGoodsType == BusinessEnum.SellGoodsType.Lifter_Comp) ||
                    (AsileOper.VendBoxList[intVendBoxIndex].SellGoodsType == BusinessEnum.SellGoodsType.Lifter_Simple))
                {
                    // 重新设置升降机串口
                    m_SellEquUp.Reset(Convert.ToInt32(AsileOper.VendBoxList[intVendBoxIndex].ShippPort));

                    for (int i = 0; i < 2; i++)
                    {
                        intErrCode = QueryLifterStatus(intVendBoxIndex);

                        #region 检测升降机设备信息

                        switch (AsileOper.VendBoxList[intVendBoxIndex].LifterStatus)
                        {
                            case "02":// 正常
                                _CheckServerReason = BusinessEnum.ServerReason.Normal;
                                break;

                            case "10":// 取货口有货
                                if (AsileOper.VendBoxList[intVendBoxIndex].SellGoodsType == BusinessEnum.SellGoodsType.Lifter_Comp)
                                {
                                    _CheckServerReason = BusinessEnum.ServerReason.Err_GoodsExist;
                                }
                                else
                                {
                                    _CheckServerReason = BusinessEnum.ServerReason.Normal;
                                }
                                break;

                            case "11":// 光电管故障
                                _CheckServerReason = BusinessEnum.ServerReason.Err_GuangDian;
                                break;

                            case "03":// 升降机位置不在初始位
                            case "04":// 纵向电机卡塞
                                _CheckServerReason = BusinessEnum.ServerReason.Err_UpDown_VertMotor;
                                break;
                            case "05":// 接货台不在初始位
                            case "06":// 横向电机卡塞
                                _CheckServerReason = BusinessEnum.ServerReason.Err_UpDown_HoriMotor;
                                break;
                            case "07":// 小门电机卡塞
                                _CheckServerReason = BusinessEnum.ServerReason.Err_UpDown_DoorMotor;
                                break;
                            case "08":// 接货台有货
                                _CheckServerReason = BusinessEnum.ServerReason.Err_UpDown_JieHuo;
                                break;
                            case "09":// 接货台电机卡塞
                                _CheckServerReason = BusinessEnum.ServerReason.Err_UpDown_JieHuoMotor;
                                break;
                            default:// 其它故障
                                _CheckServerReason = BusinessEnum.ServerReason.Err_UpDown_Other;
                                break;
                        }

                        #endregion

                        #region 如果升降机设备在某些状态，则复位下升降机，再次查询其状态

                        if ((_CheckServerReason == BusinessEnum.ServerReason.Err_UpDown_VertMotor) ||
                            (_CheckServerReason == BusinessEnum.ServerReason.Err_UpDown_HoriMotor))
                        {
                            // 如果是升降机纵向电机或横向电机故障，则复位升降机，再次查询其状态
                            intErrCode = 0;// m_SellEquUp.ResetUpDownEqu();
                            if (intErrCode == 0)
                            {
                                // 查询当前升降机状态
                                string strLifterStatus = string.Empty;
                                int intNums = 0;
                                while (true)
                                {
                                    Thread.Sleep(200);
                                    intNums++;
                                    intErrCode = m_SellEquUp.QueryUpDownStatus(out strLifterStatus);
                                    // 9：丛机正忙
                                    if (intErrCode != 9)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        if (intNums > 50)
                                        {
                                            // 超过10秒则自动退出
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            // 如果是其他设备状态，则直接退出
                            break;
                        }

                        #endregion
                    }

                    if (_CheckServerReason != BusinessEnum.ServerReason.Normal)
                    {
                        return _CheckServerReason;
                    }
                }

                #endregion

                #region 创建支付交易的相关值

                if (_CheckServerReason == BusinessEnum.ServerReason.Normal)
                {
                    m_SaleType = "1";// 更改销售方式
                    // 创建新的交易流水号
                    bool result = CreateNewBusiness();
                    // 保存扣款交易数据
                    string strCardSerNo = m_BusId.ToString();
                    string strPayType = string.Empty;
                    // 0：取货码 1：身份证 2：条形码
                    strPayType = "82";

                    ////int intNetCode = m_GateSocket.PosCard(m_BusId.ToString(), strPayType, codeNum, "",
                    ////    Convert.ToInt32(AsileOper.CurrentMcdInfo.SellPrice), 0, strCardSerNo, "0", "0");
                    int intNetCode = m_GateSocket.WxTakeCode_Pay(m_BusId.ToString(), codeNum,
                        Convert.ToInt32(AsileOper.CurrentMcdInfo.SellPrice), strPayType);
                }

                #endregion

                return _CheckServerReason;
            }
            catch (Exception ex)
            {
                _CheckServerReason = BusinessEnum.ServerReason.Err_Other;
                AddErrLog(strLogType, intErrCode.ToString(), ex.Message);
                return _CheckServerReason;
            }
            finally
            {
                AddBusLog(strLogType + "  Code:" + _CheckServerReason.ToString());
                AddBusLog("**********************************************");
            }
        }

        /// <summary>
        /// 检测满足条件的货道
        /// </summary>
        /// <param name="takeType">类型 1：指定货道或托盘 2：随机出货 3：指定商品</param>
        /// <param name="asileCode">货道或托盘编码或商品编码</param>
        /// <returns>结果 0：成功 1：没有找到符合条件的货道 2：库存不足 3：货道故障</returns>
        private BusinessEnum.ServerReason Device_CheckAsileList(string takeType, string asileCode)
        {
            BusinessEnum.ServerReason _serverReason = BusinessEnum.ServerReason.Normal;
            int intErrCode = 0;
            List<AsileModel> _choiceAsileList = new List<AsileModel>();
            _choiceAsileList.Clear();

            #region 查询满足条件的货道

            for (int i = 0; i < AsileOper.AsileList.Count; i++)
            {
                if (takeType == "1")
                {
                    #region 指定货道或托盘出货

                    if (asileCode.IndexOf("Z") > -1)
                    {
                        // 指定托盘出货
                        if (AsileOper.AsileList[i].PaCode.Substring(0, 1) == asileCode.Substring(0, 1))
                        {
                            _choiceAsileList.Add(AsileOper.AsileList[i]);
                        }
                    }
                    else
                    {
                        // 指定具体货道
                        if (AsileOper.AsileList[i].PaCode == asileCode)
                        {
                            _choiceAsileList.Add(AsileOper.AsileList[i]);
                            break;
                        }
                    }

                    #endregion
                }
                if (takeType == "2")
                {
                    // 随机出货
                    _choiceAsileList.Add(AsileOper.AsileList[i]);
                }
                if (takeType == "3")
                {
                    #region 指定商品出货

                    if (AsileOper.AsileList[i].McdCode == asileCode)
                    {
                        _choiceAsileList.Add(AsileOper.AsileList[i]);
                    }

                    #endregion
                }
            }

            #endregion

            #region 从初步满足条件的货道中选择一个库存、状态等都满足的货道

            if (_choiceAsileList.Count == 0)
            {
                // 没有合适条件的货道
                return BusinessEnum.ServerReason.Err_NoAsile;
            }

            bool blnPaIsChoice = false;
            string strVendBoxCode = string.Empty;
            string strChoicePaCode = string.Empty;
            int intTotalStockNum = 0;
            int intPaChoiceNum = 0;
            for (int i = 0; i < _choiceAsileList.Count; i++)
            {
                blnPaIsChoice = true;
                #region 检测该货道是否存在

                if (_choiceAsileList[i].PaKind != "0")
                {
                    // 不存在该货道
                    blnPaIsChoice = false;
                }

                #endregion

                strVendBoxCode = _choiceAsileList[i].VendBoxCode;
                strChoicePaCode = _choiceAsileList[i].PaCode;

                if (blnPaIsChoice)
                {
                    #region 检测该货道下的该商品库存是否有

                    if (!AsileOper.CheckGoodsStock(strVendBoxCode, strChoicePaCode, ConfigInfo.IsRunStock))
                    {
                        // 该货道库存不足
                        blnPaIsChoice = false;
                    }
                    else
                    {
                        intTotalStockNum++;
                    }

                    #endregion
                }

                if (blnPaIsChoice)
                {
                    #region 检测货道状态

                    intErrCode = QueryPaStatus(strChoicePaCode, true);
                    if (intErrCode != 0)
                    {
                        blnPaIsChoice = false;
                    }
                    else
                    {
                        if (AsileOper.CurrentMcdInfo.PaStatus != "02")
                        {
                            // 货道故障或不存在
                            blnPaIsChoice = false;
                        }
                    }

                    #endregion
                }

                if (blnPaIsChoice)
                {
                    // 选中了该货道
                    intPaChoiceNum = 1;
                    AsileOper.CurrentMcdInfo = _choiceAsileList[i];
                    break;
                }
            }

            #endregion

            if ((blnPaIsChoice) && (intPaChoiceNum > 0))
            {
                // 选中了某个货道
                _serverReason = BusinessEnum.ServerReason.Normal;
            }
            else
            {
                if (intTotalStockNum == 0)
                {
                    // 商品库存不足
                    _serverReason = BusinessEnum.ServerReason.Err_NoStock;
                }
                else
                {
                    // 认为货道故障
                    _serverReason = BusinessEnum.ServerReason.Err_AsileStatus;
                }
            }

            return _serverReason;
        }

        /// <summary>
        /// 根据当前可允许的现金支付方式来控制纸币器、硬币器
        /// </summary>
        private void ControlCashCoinPayment()
        {
            int intErrCode = 0;

            switch (PaymentOper.PaymentList.PayShow_Cash)
            {
                case BusinessEnum.PayShowStatus.Cash:// 只能使用纸币器
                    if (DeviceInfo.CoinEnableKind != BusinessEnum.EnableKind.Disable)
                    {
                        intErrCode = ControlCoin("0",true);
                    }
                    if (DeviceInfo.CashEnableKind != BusinessEnum.EnableKind.Enable)
                    {
                        intErrCode = ControlCash("1",true);
                    }
                    break;
                case BusinessEnum.PayShowStatus.Cash_Coin:// 纸币器、硬币器都能使用
                    if (DeviceInfo.CoinEnableKind != BusinessEnum.EnableKind.Enable)
                    {
                        intErrCode = ControlCoin("1",true);
                    }
                    if (DeviceInfo.CashEnableKind != BusinessEnum.EnableKind.Enable)
                    {
                        intErrCode = ControlCash("1",true);
                    }
                    break;
                case BusinessEnum.PayShowStatus.Coin:// 只能使用硬币器
                    if (DeviceInfo.CoinEnableKind != BusinessEnum.EnableKind.Enable)
                    {
                        intErrCode = ControlCoin("1",true);
                    }
                    if (DeviceInfo.CashEnableKind != BusinessEnum.EnableKind.Disable)
                    {
                        ////intErrCode = ControlCash("0",true);//////// 2016-07-06 Rain修改 强制打开纸币器

                        intErrCode = ControlCash("1", true);
                    }
                    break;
                case BusinessEnum.PayShowStatus.Pause:// 不能使用现金支付
                    // 禁能纸币器、硬币器
                    if (DeviceInfo.KmbControlSwitch == BusinessEnum.ControlSwitch.Run)
                    {
                        DisableCashPaymnet(true);
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 检测商品列表主界面上的某个货道是否能够点击
        /// </summary>
        /// <param name="paCode">货道编号或商品编号</param>
        /// <returns>结果 False：不允许点击 True：允许点击</returns>
        public bool CheckIsClickGoods(string vendBoxCode,string paCode,int sellPrice,bool isGift)
        {
            bool result = false;
            if ((GoodsShowModelType == BusinessEnum.GoodsShowModelType.GoodsToOnlyAsile) || (isGift))
            {
                // 商品和货道一一对应或者属于赠品
                result = AsileOper.CheckGoodsStock(vendBoxCode,paCode, ConfigInfo.IsRunStock);
            }
            else
            {
                result = GoodsOper.CheckGoodsStock(paCode,sellPrice, ConfigInfo.IsRunStock);
            }

            if (!result)
            {
                // 库存不足
                if (ConfigInfo.NoStockClickGoods == "1")
                {
                    // 允许点击
                    result = true;
                }
            }

            return result;
        }

        /// <summary>
        /// 检测PC和ARM上的金额是否一致
        /// </summary>
        private void CheckARMPCMoney()
        {
            int intUsableMoney = 0;
            // 获取当前ARM上的金额
            int intUsableCode = GetUsableMoney(out intUsableMoney);
            if (intUsableCode == 0)
            {
                if (m_TotalPayMoney != intUsableMoney)
                {
                    // 如果当前ARM上的金额和软件上的金额不一致
                    AddBusLog("Different Money  PC:" + m_TotalPayMoney.ToString() +
                        "  ARM:" + intUsableMoney.ToString());
                }
                m_TotalPayMoney = intUsableMoney;
            }
        }

        #endregion

        #region 业务相关流程控制（公共函数）

        #region 金额处理业务流程

        /// <summary>
        /// 查询金额
        /// </summary>
        /// <param name="querySource">查询路径 0：商品详细界面查询金额 1：商品主界面查询金额</param>
        /// <param name="money">金额</param>
        /// <param name="isReturnCoin">是否触发退币按钮，False：没有触发 True：触发</param>
        /// <returns>结果代码</returns>
        public int QueryMoney(string querySource,out int money, out bool isReturnCoin)
        {
            int intErrCode = ERR_SYS;

            money = 0;
            isReturnCoin = false;

            if (PaymentOper.PaymentList.Cash.ControlSwitch == BusinessEnum.ControlSwitch.Stop)
            {
                // 如果纸币器、硬币器都被停用
                return intErrCode;
            }

            string strValue = string.Empty;

            string strLogType = "QueryMoney";

            ErrMsg = string.Empty;
            int _stockNum = 0;

            try
            {
                intErrCode = m_KmbOper.QueryMoney(out strValue);
                if (intErrCode == 0)
                {
                    #region 如果查询成功，解析数据
                    // 纸币器状态|硬币器状态|可接收货币标识|货币类型|金额
                    string[] hexValue = strValue.Split('|');
                    if (hexValue.Length > 4)
                    {
                        DeviceInfo.MoneyRecType = hexValue[2];

                        // 检测硬件的退币按钮是否被触发
                        if (hexValue[1] == "03")
                        {
                            #region 硬件退币按钮被触发

                            isReturnCoin = true;

                            #endregion
                        }
                        else
                        {
                            DeviceInfo.CoinStatus.Status = hexValue[1];

                            #region 2015-01-20 修改

                            if (querySource == "0")
                            {
                                if ((hexValue[0] == "00") && (hexValue[1] == "02"))
                                {
                                    // 如果此时纸币器状态为禁能，则查询当前硬币数量，如果硬币数量为充足，则要重新使能纸币器
                                    bool blnIsEnoughCoin = false;
                                    string _value = string.Empty;
                                    int intCode = QueryCoinMoney(out blnIsEnoughCoin, out _value);
                                    if ((intCode == 0) && (!blnIsEnoughCoin))
                                    {
                                        AddBusLog("Anew Cash Enable");
                                        // 如果此时硬币数量为充足，使能纸币器
                                        intCode = ControlCash("1",true);
                                    }
                                }
                                else
                                {
                                    DeviceInfo.CashStatus.Status = hexValue[0];
                                }
                            }
                            else
                            {
                                DeviceInfo.CashStatus.Status = hexValue[0];
                            }

                            #endregion

                            #region 存在金额投入

                            if (hexValue[3] != "FF")
                            {
                                money = Convert.ToInt32(hexValue[4]);
                                if (money > 0)
                                {
                                    m_SaleType = "0";// 更改销售方式

                                    PaymentOper.PaymentList.Now_PayMent = BusinessEnum.PayMent.Cash;// 当前支付方式为现金

                                    ClearTunMoneyTime();// 更新吞币开始计算时间

                                    // 投币类型 
                                    // 00：硬币（进硬币币筒或进Hopper找零箱） 
                                    // 01：纸币（进钞箱） 
                                    // 02：纸币（进纸币找零箱）
                                    // 03：硬币（进溢币盒，只针对普通硬币器）
                                    // 04：纸币（缓存）
                                    string strMoneyTypeCode = hexValue[3];
                                    string strCashType = "0";

                                    if (strMoneyTypeCode == "04")
                                    {
                                        // 纸币暂存
                                        m_TotalPayMoney_NoStack = money;
                                        // 检测当前零钱是否充足
                                        long intCoinStock = CashInfoOper.GetCashStockMoney_Type("0","0");
                                        int intGoodsPrice = Convert.ToInt32(AsileOper.CurrentMcdInfo.SellPrice);
                                        if (m_TotalPayMoney + m_TotalPayMoney_NoStack - intGoodsPrice <= intCoinStock)
                                        {
                                            // 找零零钱充足，可以压钞
                                            StackBillMoney("1");
                                        }
                                    }
                                    else
                                    {
                                        #region 处理接收各货币的库存量
                                        switch (strMoneyTypeCode)
                                        {
                                            case "00":// 硬币（进硬币币筒或进Hopper找零箱） 
                                            case "03":// 硬币（进溢币盒，只针对普通硬币器）
                                                strCashType = "0";
                                                break;
                                            case "01":// 纸币（进钞箱） 
                                            case "02":// 纸币（进纸币找零箱）
                                                strCashType = "1";
                                                break;
                                        }
                                        // 更新硬币库存数据 2015-07-27增加
                                        QueryDenomChangeNum(strCashType, money, strMoneyTypeCode, true, out _stockNum);
                                        strMoneyTypeCode = strCashType;
                                        #endregion

                                        if (m_TotalPayMoney == 0)
                                        {
                                            // 第一次投币，更新交易号
                                            CreateNewBusiness();
                                            // 禁止非现金支付方式环境
                                            ControlNoCashEnviro(false);
                                        }

                                        m_TotalPayMoney += money;

                                        // 记录投币信息数据
                                        AddMoneyData(strMoneyTypeCode, "0", money, 1);

                                        // 向网关汇报信息
                                        m_GateSocket.OperMoney(m_BusId.ToString(), strMoneyTypeCode, "0", money, 1);
                                    }
                                }
                            }

                            #endregion
                        }
                    }

                    #endregion
                }
                ////if (m_QueryMoneyValue != (intErrCode.ToString() + strValue))
                ////{
                m_QueryMoneyValue = intErrCode.ToString() + strValue;
                AddBusLog_Code(strLogType, intErrCode.ToString(), strValue);
                ////}
            }
            catch (Exception ex)
            {
                intErrCode = ERR_SYS;
                AddErrLog(strLogType, intErrCode.ToString(), ex.Message);
            }

            return intErrCode;
        }

        /// <summary>
        /// 纸币压钞或退钞
        /// </summary>
        /// <param name="operType">0：退钞 1：压钞</param>
        /// <returns></returns>
        public int StackBillMoney(string operType)
        {
            int intErrCode = ERR_SYS;

            string _value = string.Empty;
            intErrCode = m_KmbOper.StackBillMoney(operType, out _value);
            m_TotalPayMoney_NoStack = 0;

            return intErrCode;
        }

        /// <summary>
        /// 查询金额—用于纸币找零投入查询
        /// </summary>
        /// <param name="_value"></param>
        /// <returns></returns>
        public int QueryMoney_AddBill(out string _value)
        {
            int intErrCode = ERR_SYS;
            _value = string.Empty;

            try
            {
                intErrCode = m_KmbOper.QueryMoney(out _value);
            }
            catch
            {
                intErrCode = ERR_SYS;
            }
            return intErrCode;
        }

        /// <summary>
        /// 退币
        /// </summary>
        /// <param name="returnSource">退币途径 0：商品详细界面退币 1：商品主界面退币</param>
        /// <returns>结果代码 0：成功 1：要退币的金额小于控制主板目前已有金额 2：零钱不足 3：硬币器故障，退币失败 4：其它原因失败</returns>
        public int ReturnCoin(string returnSource)
        {
            int intResultCode = 0;

            int intErrCode = ERR_SYS;

            // 退币时要使能硬币器，禁能纸币器
            intErrCode = ControlCoin("1", true);
            if (intErrCode != 0)
            {
                return 4;
            }

            intErrCode = ControlCash("0", true);

            #region 获取当前控制主板的可用金额，和控制主板保持一致
            bool blnIsGetUserableMoney = false;
            int intUsableMoney = 0;
            for (int i = 0; i < 10; i++)
            {
                intErrCode = GetUsableMoney(out intUsableMoney);
                if (intErrCode == 0)
                {
                    blnIsGetUserableMoney = true;
                    m_TotalPayMoney = intUsableMoney;
                    break;
                }
                else
                {
                    // 获取失败
                    intErrCode = 4;
                }
                Thread.Sleep(40);
            }
            if (intErrCode != 0)
            {
                return intErrCode;
            }

            #endregion

            // 退币后控制主板当前的余额
            int intMbFee = 0;

            if (blnIsGetUserableMoney)
            {
                #region 执行退币（新版）

                #region 2015-07-23 如果纸币找零（临时测试）

                //////bool blnIsEnougth = false;
                //////string _value = string.Empty;
                //////int intCeShiCode = 0;
                //////int intOld10Num = 0;
                //////int intNew10Num = 0;

                //////if (ConfigInfo.IsReturnBill == BusinessEnum.ControlSwitch.Run)
                //////{
                //////    intCeShiCode = QueryCoinMoney(out blnIsEnougth, out _value);
                //////    intOld10Num = CashStockOper.GetCashStockNum(1000, "1");
                //////}

                #endregion

                Thread.Sleep(20);

                // 发送退币指令
                intErrCode = ReturnMoney(m_TotalPayMoney, returnSource, out intMbFee);
                if (intErrCode != 0)
                {
                    #region 退币失败

                    switch (intErrCode)
                    {
                        case 13:// 控制主板可用金额不足
                            intResultCode = 1;
                            break;

                        case 14:
                            // 零钱不足
                            intResultCode = 2;
                            break;

                        default:
                            // 硬币器故障
                            intResultCode = 3;
                            break;
                    }

                    #endregion

                    // 查询当前可用金额
                    Thread.Sleep(40);
                    int intCode = GetUsableMoney(out intMbFee);
                    if (intCode == 0)
                    {
                        m_TotalPayMoney = intMbFee;
                    }
                }
                else
                {
                    m_TotalPayMoney = intMbFee;

                    if (m_TotalPayMoney == 0)
                    {
                        m_SellGoodsNum = 0;
                    }
                }

                #region 2015-11-28 谷霖 添加 查询各货币找零后的库存余量

                // 查询当前各货币的库存余量
                QueryDenomChangeNum_AllCoin();

                #endregion

                #region 2015-07-23 如果纸币找零，获取当前纸币可找数量（临时测试）

                //////if (ConfigInfo.IsReturnBill == BusinessEnum.ControlSwitch.Run)
                //////{
                //////    intCeShiCode = QueryCoinMoney(out blnIsEnougth, out _value);
                //////    intNew10Num = CashStockOper.GetCashStockNum(1000, "1");

                //////    // 更改1元硬币数量
                //////    int intCoinNum = 0;
                //////    intCoinNum = (m_TotalPayMoney - (intOld10Num - intNew10Num) * 1000) / 100;
                //////    CashStockOper.UpdateCashStockNum(100, intCoinNum, "0", "1");
                //////}

                #endregion

                #endregion
            }

            // 根据零钱是否充足来控制纸币器
            //PubHelper.p_BusinOper.ControlCashEnoughCoin(true, out blnIsRefreshEnouthCoin, out strOutValue);

            intResultCode = intErrCode;

            intErrCode = ControlCoin("0", true);// 禁能硬币器

            ClearTunMoneyTime();// 更新吞币开始计算时间

            return intResultCode;
        }

        /// <summary>
        /// 退币—用于纸币找零退或硬币找零库存退
        /// </summary>
        /// <param name="money">要退币的金额</param>
        /// <param name="operType">要退币类型 0：只退硬币 1：只退纸币 2：纸硬币皆可</param>
        /// <param name="money1">退币后的控制主板当前余额</param>
        /// <returns>结果代码</returns>
        public int ReturnCoin_CashStock(int money,string operType, out string _value)
        {
            int intErrCode = ERR_SYS;

            _value = string.Empty;

            try
            {
                intErrCode = m_KmbOper.ReturnMoney_Test(operType,"0",money, out _value);
            }
            catch
            {
                intErrCode = ERR_SYS;
            }

            return intErrCode;
        }

        #endregion

        #region 出货业务流程（2014-11-18添加）

        /// <summary>
        /// 出货—开始出货
        /// </summary>
        /// <param name="paCode">货道外部编号</param>
        /// <returns>结果代码 0：成功 1：无该货道 2：控制主板可用金额不足 3：无货 9：失败</returns>
        public int SellGoods_Begin(string paCode,out int paIndex)
        {
            int intErrCode = ERR_SYS;

            paIndex = 0;
            int intPaIndex = 0;

            string strPaId = string.Empty;
            string strMcdCode = string.Empty;

            intErrCode = AsileOper.GetPaIndex(paCode, out intPaIndex);
            if (intErrCode != 0)
            {
                // 没有该货道
                return 1;
            }

            paIndex = intPaIndex;

            strPaId = AsileOper.AsileList[intPaIndex].PaId;
            strMcdCode = AsileOper.AsileList[intPaIndex].McdCode;

            string strValue = string.Empty;
            string strDrop = SysCfgOper.GetSysCfgValue("DropModel");// 掉货检测是否参与判断

            int intVendBoxCode = Convert.ToInt32(AsileOper.AsileList[intPaIndex].VendBoxCode);
            string strSellPrice = "0";
            if (ConfigInfo.IsFreeSellNoPay == BusinessEnum.ControlSwitch.Run)
            {
                m_SaleType = "1";// 如果是无需支付免费出货，则修改为非现金支付
            }
            if (m_SaleType == "0")
            {
                strSellPrice = AsileOper.AsileList[intPaIndex].SellPrice;
            }

            string strLogType = "SellGoods_Begin";

            AddBusLog("****************" + strLogType + "****************");
            AddBusLog(strLogType + "  PaCode:" + paCode + "  PaId:" + strPaId + "  SaleType:" + m_SaleType + "  Price:" + strSellPrice + "  Drop:" + strDrop);

            // 关闭支付方式环境
            CloseAllPayEnviro();

            string strSellFailTryNum = SysCfgOper.GetSysCfgValue("SellFailTryNum");
            if (string.IsNullOrEmpty(strSellFailTryNum))
            {
                strSellFailTryNum = "0";
            }

            int intVendBoxIndex = AsileOper.GetVendBoxIndex(intVendBoxCode.ToString());
            for (int i = 0; i < Convert.ToInt32(strSellFailTryNum) + 1; i++)
            {
                #region 开始出货

                switch (AsileOper.VendBoxList[intVendBoxIndex].SellGoodsType)
                {
                    case BusinessEnum.SellGoodsType.Spring:// 弹簧方式出货

                        intErrCode = m_KmbOper.SellGoods_Begin(intVendBoxCode, Convert.ToInt32(strPaId.Substring(0, 1)),
                            Convert.ToInt32(strPaId.Substring(1, 1)), m_SaleType, strSellPrice, strDrop, out strValue);

                        AddBusLog_Code(strLogType + "  " + "Drop:" + strDrop, intErrCode.ToString(), strValue);

                        switch (intErrCode)
                        {
                            case 0:
                                break;
                            case 13:// 控制主板余额不足
                                intErrCode = 2;
                                break;
                            default:// 其它原因，失败
                                intErrCode = 9;
                                break;
                        }

                        break;

                    case BusinessEnum.SellGoodsType.Lifter_Comp:// 复杂型升降机方式
                    case BusinessEnum.SellGoodsType.Lifter_Simple:// 简易型升降机方式
                        // 重新设置升降机串口
                        intErrCode = m_SellEquUp.Reset(Convert.ToInt32(AsileOper.VendBoxList[intVendBoxIndex].ShippPort));

                        if (AsileOper.VendBoxList[intVendBoxIndex].UpDownSellModel == "0")
                        {
                            // 升降机直接出货
                            intErrCode = m_SellEquUp.SellGoods_Begin(Convert.ToInt32(strPaId.Substring(0, 1)),
                                Convert.ToInt32(strPaId.Substring(1, 1)), out strValue);
                            // strValue：0:成功 1：电机卡塞 2：没有货物 3：有货无法送 4：小门未打开 5：超时或失败
                            // 0：成功 1：无该货道 2：控制主板可用金额不足 3：无货 9：失败
                            AddBusLog_Code(strLogType, intErrCode.ToString(), strValue);
                        }
                        else
                        {
                            // 升降机按参数出货
                            intErrCode = UpDown_SellGoods_ByParameter(intVendBoxCode.ToString(), intVendBoxIndex, 
                                intPaIndex, strPaId, out strValue);
                        }

                        switch (intErrCode)
                        {
                            case 0://
                                break;

                            default:// 其它原因，失败
                                intErrCode = 9;
                                break;
                        }

                        break;
                }

                #endregion

                if (intErrCode != 9)
                {
                    break;
                }
                else
                {
                    Thread.Sleep(100);
                }
            }

            if ((intErrCode != 0) && (ConfigInfo.IsFreeSellNoPay == BusinessEnum.ControlSwitch.Stop))
            {
                // 如果开始出货指令出现错误且为支付出货
                OperSellGoodsData(intVendBoxIndex, false, false, false, intVendBoxCode, strMcdCode, intPaIndex, paCode,
                    Convert.ToInt32(strSellPrice), strDrop);
            }

            if (m_SaleType == "0")
            {
                // 如果是现金支付
                ClearTunMoneyTime();// 更新吞币开始计算时间
            }

            return intErrCode;
        }

        /// <summary>
        /// 出货—查询出货进度
        /// </summary>
        /// <param name="paIndex">货道索引</param>
        /// <returns>结果代码</returns>
        public int SellGoods_Query(int paIndex)
        {
            int intErrCode = ERR_SYS;

            string strPaId = string.Empty;
            string strMcdCode = string.Empty;

            string strPaCode = AsileOper.AsileList[paIndex].PaCode;
            strPaId = AsileOper.AsileList[paIndex].PaId;
            strMcdCode = AsileOper.AsileList[paIndex].McdCode;
            int intVendBoxCode = Convert.ToInt32(AsileOper.AsileList[paIndex].VendBoxCode);
            string strSellPrice = AsileOper.AsileList[paIndex].SellPrice;
            string strDrop = SysCfgOper.GetSysCfgValue("DropModel");// 掉货检测是否参与判断

            string strValue = string.Empty;

            bool blnFailStock = false;// 出货失败后是否记录库存 False：不记录 True：失败记录库存

            string strLogType = "SellGoods_Query";

            AddBusLog("****************" + strLogType + "****************");

            bool blnIsSuccess = false;
            bool blnIsNoGoods = false;

            #region 开始查询出货结果

            int intVendBoxIndex = AsileOper.GetVendBoxIndex(intVendBoxCode.ToString());

            switch (AsileOper.VendBoxList[intVendBoxIndex].SellGoodsType)
            {
                case BusinessEnum.SellGoodsType.Spring:// 弹簧方式出货

                    intErrCode = m_KmbOper.SellGoods_Query(out strValue);

                    AddBusLog_Code(strLogType, intErrCode.ToString(), strValue);

                    switch (intErrCode)
                    {
                        case 0:
                            #region 执行结果|货道状态|出货后余额|掉货检测结果

                            string[] hexValue = strValue.Split('|');
                            if (hexValue.Length > 3)
                            {
                                // 检测执行结果和掉货检测结果
                                string strResult = hexValue[0];
                                string strPaStatus = hexValue[1];
                                string strDropStatus = hexValue[3];

                                if (strDrop == "1")
                                {
                                    // 掉货检测参与出货判断
                                    if ((strDropStatus != "02") && (strResult == "00"))
                                    {
                                        // 如果掉货检测结果不为02（没有检测到货物），且执行结果为00，都可以认为出货成功
                                        blnIsSuccess = true;
                                        strDrop = "1";
                                    }
                                }
                                else
                                {
                                    if ((strDrop == "0") && (strResult == "00"))
                                    {
                                        // 掉货检测不参与出货物判断，且执行结果为成功，认为出货成功
                                        blnIsSuccess = true;
                                    }
                                    strDrop = "0";
                                }

                                if (!blnIsSuccess)
                                {
                                    // 认为出货失败
                                    intErrCode = 9;
                                }
                            }
                            else
                            {
                                intErrCode = 9;
                            }

                            #endregion
                            break;
                        case 13:// 控制主板余额不足
                            intErrCode = 2;
                            break;
                        default:// 其它原因，失败
                            intErrCode = 9;
                            break;
                    }

                    break;

                case BusinessEnum.SellGoodsType.Lifter_Comp:// 复杂型升降机方式
                case BusinessEnum.SellGoodsType.Lifter_Simple:// 简易型升降机方式

                    intErrCode = m_SellEquUp.Reset(Convert.ToInt32(AsileOper.VendBoxList[intVendBoxIndex].ShippPort));
                    intErrCode = m_SellEquUp.SellGoods_Query(out strValue);
                    // strValue：0:成功 1：电机卡塞 2：没有货物 3：有货无法送 4：小门未打开 5：超时或失败
                    // 0：成功 1：无该货道 2：控制主板可用金额不足 3：无货 9：失败
                    AddBusLog_Code(strLogType, intErrCode.ToString(), strValue);
                    switch (intErrCode)
                    {
                        case 0://
                            switch (strValue)
                            {
                                case "0":// 成功
                                    blnIsSuccess = true;
                                    strDrop = "1";
                                    break;

                                case "2":// 无货
                                    blnFailStock = true;
                                    intErrCode = 3;
                                    blnIsNoGoods = true;
                                    break;

                                case "3":// 有货无法送
                                case "4":// 小门未打开
                                    blnFailStock = true;
                                    intErrCode = 9;
                                    break;

                                default:// 其它原因，失败
                                    intErrCode = 9;
                                    break;
                            }
                            break;

                        default:// 其它原因，失败
                            intErrCode = 9;
                            break;
                    }

                    break;
            }

            #endregion

            ////Thread.Sleep(50);
            ////int intCode = DisableCashPaymnet(true);

            ////if ((DeviceInfo.KmbControlSwitch == BusinessEnum.ControlSwitch.Run) &&
            ////    (PaymentOper.PaymentList.Cash.ControlSwitch == BusinessEnum.ControlSwitch.Run))
            if (DeviceInfo.KmbControlSwitch == BusinessEnum.ControlSwitch.Run)
            {
                #region 现金支付出货结果异常处理

                if ((intErrCode != 0) && (m_SaleType == "0") && (m_TotalPayMoney > 0))
                {
                    // 如果当前是现金支付，且当前PC金额不为0，则获取当前ARM上的金额
                    int intUsableMoney = 0;
                    int intUsableCode = 0;
                    for (int i = 0; i < 2; i++)
                    {
                        Thread.Sleep(200);
                        intUsableCode = GetUsableMoney(out intUsableMoney);
                        if (intUsableCode == 0)
                        {
                            break;
                        }
                    }
                    
                    if ((intUsableCode == 0) &&
                        ((m_TotalPayMoney - Convert.ToInt32(strSellPrice)) == intUsableMoney))
                    {
                        // 当前PC上的金额减去出货金额后，等于ARM上的金额，则可以认为实际出货成功
                        blnIsSuccess = true;
                        intErrCode = 0;
                    }

                    AddBusLog("Check Sell Fail,Code:" + intUsableCode + "  " +
                        "Result:" + blnIsSuccess.ToString() + "  " +
                        "PC:" + m_TotalPayMoney.ToString() + "  " +
                        "ARM:" + intUsableMoney.ToString());
                }

                #endregion
            }

            if (m_SaleType == "0")
            {
                // 如果是现金支付
                ClearTunMoneyTime();// 更新吞币开始计算时间
            }

            if (ConfigInfo.IsFreeSellNoPay == BusinessEnum.ControlSwitch.Stop)
            {
                OperSellGoodsData(intVendBoxIndex, blnIsSuccess, blnFailStock, blnIsNoGoods, intVendBoxCode, strMcdCode, paIndex, strPaCode,
                    Convert.ToInt32(strSellPrice), strDrop);
            }

            AddBusLog("**********************************************");
            return intErrCode;
        }

        /// <summary>
        /// 出货结果数据处理
        /// </summary>
        /// <param name="sellResult"></param>
        /// <param name="failStock"></param>
        /// <param name="vendBoxCode"></param>
        /// <param name="mcdCode"></param>
        /// <param name="paIndex"></param>
        /// <param name="paCode"></param>
        /// <param name="sellPrice"></param>
        /// <param name="drop"></param>
        private void OperSellGoodsData(int vendBoxIndex,bool sellResult,bool failStock,bool isNoGoods,int vendBoxCode,string mcdCode,
            int paIndex,string paCode,int sellPrice,string drop)
        {
            int intSellNum = 0;
            if (sellResult)
            {
                #region 出货成功后的处理

                #region 如果是驱动板没有连接到控制主板上，且支付方式是现金支付，则通知控制主板减少出货金额

                if ((AsileOper.VendBoxList[vendBoxIndex].SellGoodsType != BusinessEnum.SellGoodsType.Spring) &&
                    (m_SaleType == "0") &&
                    (DeviceInfo.KmbControlSwitch == BusinessEnum.ControlSwitch.Run))
                {
                    // 通知控制主板减少出货金额
                    string _value = string.Empty;
                    int intDeductCode;
                    for (int i = 0; i < 2; i++)
                    {
                        Thread.Sleep(50);
                        intDeductCode = m_KmbOper.DeductMoney(sellPrice, out _value);
                        AddBusLog_Code("DeductMoney", intDeductCode.ToString(), _value);
                        if ((intDeductCode == 0) ||
                            (intDeductCode == 2) ||
                            (intDeductCode == 13) ||
                            (intDeductCode == 17))
                        {
                            // 0：成功 2：参数无效 13：控制主板可用金额不足 17：不能执行该指令
                            break;
                        }
                    }
                    //strSellPrice
                }
                #endregion

                if ((m_SaleType == "0") && (m_TotalPayMoney > 0))
                {
                    // 如果是现金支付，且出货成功
                    m_TotalPayMoney -= sellPrice;
                }

                // 记录当前交易出货次数
                if (m_TotalPayMoney == 0)
                {
                    m_SellGoodsNum = 0;
                }
                else
                {
                    m_SellGoodsNum++;
                }

                // 记录出货成功数据
                bool result = AddSellData(vendBoxCode, paCode, sellPrice.ToString(), m_SaleType, paIndex);
                if (result)
                {
                    OperGoodsStock(paIndex, false);
                }

                intSellNum = 1;
                #endregion
            }
            else
            {
                #region 出货失败后的处理
                if (failStock)
                {
                    bool blnIsEmpty = false;
                    if (isNoGoods)
                    {
                        // 如果是无货，则清空库存
                        blnIsEmpty = true;
                    }
                    // 失败，记录库存数据
                    bool result = AsileOper.DecuAsileStock(vendBoxCode, paCode, paIndex, blnIsEmpty);
                    if (result)
                    {
                        OperGoodsStock(paIndex, blnIsEmpty);
                    }
                }
                drop = "2";
                #endregion
            }

            #region 如果当前支付方式是支付宝付款码

            if ((PaymentOper.PaymentList.Now_PayMent == BusinessEnum.PayMent.AliPay_Code) ||
                (PaymentOper.PaymentList.Now_PayMent == BusinessEnum.PayMent.WeChatCode) ||
                (PaymentOper.PaymentList.Now_PayMent == BusinessEnum.PayMent.BestPay_Code) ||
                (PaymentOper.PaymentList.Now_PayMent == BusinessEnum.PayMent.JDPay_Code) ||
                (PaymentOper.PaymentList.Now_PayMent == BusinessEnum.PayMent.BaiDuPay_Code))
            {
                // 发送条形码扫描在线支付出货结果数据
                BarCode_Net_UploadData(true, sellResult, paCode);
            }

            #endregion

            // 向网关汇报出货结果信息
            m_GateSocket.SellGoods(m_BusId.ToString(), paCode, sellPrice, "", intSellNum, drop);

            #region 计算其它

            string strPayMent = string.Empty;
            string strPayType = string.Empty;
            string strPayAccount = "0";

            switch (PaymentOper.PaymentList.Now_PayMent)
            {
                case BusinessEnum.PayMent.Cash:
                    strPayType = "cash";
                    strPayMent = "现金";
                    break;
                case BusinessEnum.PayMent.AliPay_Code:
                    strPayType = "alipay";
                    strPayMent = "支付宝付款码";
                    break;
                case BusinessEnum.PayMent.IcCard:
                    strPayType = "iccard";
                    strPayMent = "刷卡";
                    if (UserCardInfo != null)
                    {
                        strPayAccount = UserCardInfo.CardNum_Upload;
                    }
                    break;
                case BusinessEnum.PayMent.NoFeeCard:
                    strPayType = "nofeecard";
                    strPayMent = "会员卡";
                    break;
                case BusinessEnum.PayMent.WeChatCode:
                    strPayType = "wechat";
                    strPayMent = "微信刷卡";
                    break;
                default:
                    strPayMent = string.Empty;
                    break;
            }

            #endregion

            #region 向第三方汇报出货数据 2015-09-07 谷霖

            if (ConfigInfo.O2OTake_Switch == BusinessEnum.ControlSwitch.Run)
            {
                if (!string.IsNullOrEmpty(strPayMent))
                {
                    O2OServerOper.Report_SellResult(m_BusId.ToString(), mcdCode, intSellNum.ToString(), sellPrice.ToString(),
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        paCode, strPayType, strPayAccount, sellPrice.ToString());
                }
            }

            #endregion

            #region 打印购物单据

            if (ConfigInfo.IsPrintConsumeBill == BusinessEnum.ControlSwitch.Run)
            {
                PrintInfo = new PrintInfoModel();
                PrintInfo.BuyNum = intSellNum;
                PrintInfo.GoodsName = AsileOper.AsileList[paIndex].McdName;
                PrintInfo.GoodsCode = AsileOper.AsileList[paIndex].McdCode;
                PrintInfo.GoodsPrice = AsileOper.AsileList[paIndex].SellPrice;
                PrintInfo.GoodsPiCi = AsileOper.AsileList[paIndex].PiCi;
                PrintInfo.Manufacturer = AsileOper.AsileList[paIndex].Manufacturer;
                PrintInfo.GoodsSpec = AsileOper.AsileList[paIndex].GoodsSpec;
                PrintInfo.Money = AsileOper.AsileList[paIndex].SellPrice;
                PrintInfo.BuyTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                PrintInfo.TermCode = ConfigInfo.VmId;
                PrintInfo.SerNo = m_BusId.ToString();
                PrintInfo.ProductDate = AsileOper.AsileList[paIndex].ProductDate;
                PrintInfo.MaxValidDate = AsileOper.AsileList[paIndex].MaxValidDate;

                PrintInfo.PayMent = strPayMent;

                int intErrCode = PrintConsumeInfo();
            }

            #endregion
        }

        #region 商品库存变更

        /// <summary>
        /// 某货道库存变更后，同步更改所属商品库存
        /// </summary>
        /// <param name="paIndex">货道编号</param>
        private void OperGoodsStock(int paIndex,bool isEmpty)
        {
            if (ConfigInfo.GoodsShowModel == BusinessEnum.GoodsShowModelType.GoodsToMultiAsile)
            {
                if (!isEmpty)
                {
                    for (int i = 0; i < GoodsOper.GoodsList_Show.Count; i++)
                    {
                        if ((GoodsOper.GoodsList_Show[i].McdCode == AsileOper.AsileList[paIndex].McdCode) &&
                            (GoodsOper.GoodsList_Show[i].Price.ToString() == AsileOper.AsileList[paIndex].SellPrice))
                        {
                            GoodsOper.GoodsList_Show[i].SurNum -= 1;
                            break;
                        }
                    }
                }
                if (isEmpty)
                {
                    string strMcdCode = AsileOper.AsileList[paIndex].McdCode;
                    string strSellPrice = AsileOper.AsileList[paIndex].SellPrice;
                    int intSurNum = 0;
                    for (int j = 0; j < AsileOper.AsileList.Count; j++)
                    {
                        if ((strMcdCode == AsileOper.AsileList[j].McdCode) &&
                            (strSellPrice == AsileOper.AsileList[j].SellPrice))
                        {
                            intSurNum = intSurNum + AsileOper.AsileList[j].SurNum;
                        }
                    }
                    for (int j = 0; j < GoodsOper.GoodsList_Show.Count; j++)
                    {
                        if ((strMcdCode == GoodsOper.GoodsList_Show[j].McdCode) &&
                            (strSellPrice == GoodsOper.GoodsList_Show[j].Price.ToString()))
                        {
                            GoodsOper.GoodsList_Show[j].SurNum = intSurNum;
                            break;
                        }
                    }
                }
            }
        }

        #endregion

        #endregion

        #region 储值卡业务流程

        /// <summary>
        /// 查询卡信息
        /// </summary>
        /// <returns></returns>
        public int QueryCardInfo()
        {
            int intErrCode = 0;
            string _cardData = string.Empty;

            string strLogType = "QueryCardInfo";

            try
            {
                #region 发送查询卡信息指令

                intErrCode = QueryCardBaseData(out _cardData);
                if (intErrCode == 0)
                {
                    #region 查询成功
                    // 获取余额 格式：逻辑卡号*物理卡号*卡余额*卡状态*卡类型*要上传的卡号
                    UserCardInfo = new CardInfoModel();
                    string[] hexValuesSplit = _cardData.Split('*');
                    int hexNum = hexValuesSplit.Length;

                    if (hexNum > 3)
                    {
                        UserCardInfo.CardNum_Show = hexValuesSplit[0];
                        UserCardInfo.BanFee = hexValuesSplit[2];
                        if (hexNum > 5)
                        {
                            UserCardInfo.CardNum_Upload = hexValuesSplit[5];
                        }
                        else
                        {
                            UserCardInfo.CardNum_Upload = UserCardInfo.CardNum_Show;
                        }
                    }
                    else
                    {
                        intErrCode = 99;
                    }
                    #endregion
                }
                else if (intErrCode == 18)
                {
                    // 终端编号设置错误
                    m_CardOper.TermCode = SysCfgOper.GetSysCfgValue("VmId");
                }

                if (m_QueryCardInfo != _cardData)
                {
                    m_QueryCardInfo = _cardData;
                    AddBusLog_Code(strLogType, intErrCode.ToString(), _cardData);
                }

                #endregion
            }
            catch (Exception ex)
            {
                intErrCode = 99;
                AddErrLog(strLogType, intErrCode.ToString(), ex.Message);
            }

            return intErrCode;
        }

        /// <summary>
        /// 提交储值卡扣款
        /// </summary>
        /// <param name="money"></param>
        /// <returns></returns>
        public int PostCardPay(int money)
        {
            int intErrCode = 0;

            string strLogType = "PostCardPay";
            string _errICCode = string.Empty;
            string _cardData = string.Empty;
            int intBanFee = 0;

            try
            {
                string strSerNo = m_BusId.ToString();
                if (string.IsNullOrEmpty(strSerNo))
                {
                    strSerNo = "1";
                }
                if (strSerNo == "0")
                {
                    strSerNo = "9999";
                }
                if (UserCardInfo == null)
                {
                    UserCardInfo = new CardInfoModel();
                }
                intErrCode = m_CardOper.DeductMoney(UserCardInfo.CardNum_Show, "", money, strSerNo, "", "", 
                    out _errICCode, out _cardData);
                if (intErrCode == 0)
                {
                    #region 扣款成功

                    string strCardNum = string.Empty;
                    string strUpLoadCardNum = string.Empty;

                    #region 如果是普通一卡通，解析相关数据

                    if (ConfigInfo.PosBusiType == BusinessEnum.PosBusiType.Other)
                    {
                        // 扣款成功，解析相关数据，格式：逻辑卡号*物理卡号*卡余额*要上传的卡号
                        string[] hexPayValuesSplit = _cardData.Split('*');
                        int hexNum = hexPayValuesSplit.Length;
                        string strBanFee = string.Empty;
                        if (hexNum > 1)
                        {
                            strCardNum = hexPayValuesSplit[0];
                            strBanFee = hexPayValuesSplit[2];
                            UserCardInfo.CardNum_Show = strCardNum;
                            if (hexNum > 3)
                            {
                                strUpLoadCardNum = hexPayValuesSplit[3];
                            }
                            else
                            {
                                strUpLoadCardNum = strCardNum;
                            }
                            UserCardInfo.CardNum_Upload = strUpLoadCardNum;
                            if (!string.IsNullOrEmpty(strBanFee))
                            {
                                intBanFee = Convert.ToInt32(strBanFee);
                                UserCardInfo.BanFee = strBanFee;
                            }
                            else
                            {
                                intBanFee = 0;
                                UserCardInfo.BanFee = (Convert.ToInt32(UserCardInfo.BanFee) - money).ToString();
                            }
                        }
                        else
                        {
                            intErrCode = 99;
                        }
                    }

                    #endregion

                    #region 如果是武汉一卡通，解析相关数据

                    if (ConfigInfo.PosBusiType == BusinessEnum.PosBusiType.WuHanTong)
                    {
                        UserCardInfo.BanFee = (Convert.ToInt32(UserCardInfo.BanFee) - money).ToString();
                    }

                    #endregion

                    m_SaleType = "1";// 更改销售方式

                    PaymentOper.PaymentList.Now_PayMent = BusinessEnum.PayMent.IcCard;// 当前支付方式为刷卡

                    // 保存扣款数据
                    bool result = CreateNewBusiness();
                    int intNetCode = 0;
                    switch (ConfigInfo.PosBusiType)
                    {
                        case BusinessEnum.PosBusiType.WuHanTong:// 武汉一卡通
                            intNetCode = m_GateSocket.PosCard_WH_Suc(m_BusId.ToString(), _cardData);
                            break;
                        default:// 其它一卡通
                            string strCardSerNo = m_BusId.ToString();
                            intNetCode = m_GateSocket.PosCard(m_BusId.ToString(), m_CardOper.CardType, strUpLoadCardNum, "", money, intBanFee, strCardSerNo, "0", "0");
                            break;
                    }
                    
                    #endregion
                }
                else if (intErrCode == 18)
                {
                    // 终端编号设置错误
                    m_CardOper.TermCode = SysCfgOper.GetSysCfgValue("VmId");
                }

                AddBusLog_Code(strLogType, intErrCode.ToString(), _cardData);
            }
            catch (Exception ex)
            {
                intErrCode = 99;
                AddErrLog(strLogType, intErrCode.ToString(), ex.Message);
            }

            return intErrCode;
        }

        #endregion

        #region 非储值卡及二维码会员卡业务流程

        /// <summary>
        /// 打开二维码扫描设备
        /// </summary>
        /// <returns>错误代码</returns>
        public int InitQrCodeDevice()
        {
            int intErrCode = ERR_SYS;

            return intErrCode;
        }

        /// <summary>
        /// 获取二维码扫描数据
        /// </summary>
        /// <param name="_qrCodeData"></param>
        /// <returns>错误代码</returns>
        public int QueryQrCodeData(out string _qrCodeData)
        {
            _qrCodeData = string.Empty;

            int intErrCode = ERR_SYS;

            return intErrCode;
        }

        /// <summary>
        /// 查询非储值卡卡号
        /// </summary>
        /// <param name="_cardData"></param>
        /// <returns></returns>
        public int QueryNoFeeCardData(out string _cardData)
        {
            _cardData = string.Empty;
            int intErrCode = m_NoFeeCardOper.QueryNoFeeCardData(out _cardData);

            return intErrCode;
        }

        /// <summary>
        /// 清除磁条卡数据
        /// </summary>
        /// <returns></returns>
        public int ClearNoFeeCardData()
        {
            return m_NoFeeCardOper.ClearNoFeeCardData();
        }

        /// <summary>
        /// 查询非储值卡用户信息
        /// </summary>
        /// <param name="noFeeCardNum"></param>
        /// <param name="money"></param>
        /// <returns></returns>
        public int QueryNoFeeCardInfo(string noFeeCardNum, int money)
        {
            int intErrCode = 0;

            // 关闭支付方式环境
            ControlPayEnviro(BusinessEnum.PayMent.NoFeeCard, false,true);

            intErrCode = m_NoFeeCardOper.QueryCardInfo(noFeeCardNum, money);
            if (intErrCode == 0)
            {
                MemberUserInfo = m_NoFeeCardOper.MemberUserInfo;
            }
            else
            {
                // 查询失败，开启支付方式环境
                ControlPayEnviro(BusinessEnum.PayMent.NoFeeCard, true,true);
            }

            return intErrCode;
        }

        /// <summary>
        /// 提交非储值卡在线支付
        /// </summary>
        /// <param name="noFeeCardNum">卡号</param>
        /// <returns></returns>
        public int PostNoFeeCardPay(string noFeeCardNum,int money,out int _factMoney)
        {
            int intErrCode = ERR_SYS;

            // 关闭支付方式环境
            ControlPayEnviro(BusinessEnum.PayMent.NoFeeCard, false,true);

            _factMoney = 0;
            bool result = CreateNewBusiness();// 创建新的业务流水号
            intErrCode = m_NoFeeCardOper.DecuctCardMoney(noFeeCardNum, money, m_BusId.ToString(), 
                ConfigInfo.NoFeeCardIsRebate, out _factMoney);
            if (intErrCode == 0)
            {
                MemberUserInfo = m_NoFeeCardOper.MemberUserInfo;

                m_SaleType = "1";// 更改销售方式
                PaymentOper.PaymentList.Now_PayMent = BusinessEnum.PayMent.NoFeeCard;// 设置支付方式为会员卡

                // 保存扣款数据
                int intNetCode = m_GateSocket.PosCard(m_BusId.ToString(), m_NoFeeCardOper.MemberUserInfo.CardType,
                    m_NoFeeCardOper.MemberUserInfo.CardNum_Out, "", _factMoney, Convert.ToInt32(m_NoFeeCardOper.MemberUserInfo.BanFee), "", "0", "0");
            }
            else
            {
                // 失败，开启支付方式环境
                ControlPayEnviro(BusinessEnum.PayMent.NoFeeCard, true,true);
            }

            return intErrCode;
        }

        #endregion

        #region 条形码扫描在线支付业务流程（支付宝付款码、微信刷卡） 2015-01-15

        /// <summary>
        /// 提交条形码扫描在线支付请求
        /// </summary>
        /// <param name="barCode"></param>
        /// <param name="asileCode"></param>
        /// <param name="money"></param>
        /// <param name="_resultCode">支付请求结果 1：成功 2：失败 3：等待</param>
        /// <returns></returns>
        public int BarCode_Net_Pay(string barCode,string asileCode,string goodsCode,int money,string type,out int _resultCode)
        {
            int intErrCode = 0;

            _resultCode = 0;// 1：支付成功 2：支付失败 3：等待支付
            string _resultData = string.Empty;// 回复数据
            string strPayNum = "0";// 支付账号
            strPayNum = strPayNum.PadLeft(10, '0');
            string strPayType = "90";// 支付类型

            string strLogType = "BarCode_Net_Pay";

            try
            {
                #region 停止工作

                ControlPayEnviro(BusinessEnum.PayMent.AliPay_Code, false, true);

                #endregion

                BarCode_Net_PayInfo.Money = money;
                BarCode_Net_PayInfo.PayCode = barCode;
                if (type == "1")
                {
                    strPayType = "81";
                }
                BarCode_Net_PayInfo.PayType = strPayType;
                BarCode_Net_PayInfo.PayNum = strPayNum;

                bool result = CreateNewBusiness();// 创建新的业务流水号

                if (type == "0")
                {
                    // 非志愿者兑换
                    intErrCode = m_OnlinePayOper.BarCode_Net_Pay(m_BusId.ToString(), barCode, asileCode, money, out _resultData);
                }
                else
                {
                    intErrCode = O2OServerOper.PostVolunteerPay(barCode, asileCode,goodsCode, money.ToString());
                }

                bool blnIsSuc = false;
                switch (intErrCode)
                {
                    case 0:
                        #region
                        // 成功，解析数据
                        if (type == "0")
                        {
                            string[] hexData = _resultData.Split('*');
                            if (hexData.Length > 3)
                            {
                                _resultCode = Convert.ToInt32(hexData[0]);
                                strPayNum = hexData[3];
                                strPayType = hexData[1];

                                if (_resultCode == 1)
                                {
                                    // 支付成功
                                    blnIsSuc = true;
                                }
                            }
                            else
                            {
                                intErrCode = 9;
                            }
                        }
                        else
                        {
                            blnIsSuc = true;
                        }

                        if (blnIsSuc)
                        {
                            m_SaleType = "1";// 更改销售方式
                            switch (strPayType)
                            {
                                case "90":// 支付宝付款码
                                    PaymentOper.PaymentList.Now_PayMent = BusinessEnum.PayMent.AliPay_Code;// 当前支付方式为支付宝付款码
                                    break;
                                case "92":// 微信刷卡
                                    PaymentOper.PaymentList.Now_PayMent = BusinessEnum.PayMent.WeChatCode;// 当前支付方式为微信刷卡
                                    break;
                                case "76":// 翼支付付款码
                                    PaymentOper.PaymentList.Now_PayMent = BusinessEnum.PayMent.BestPay_Code;// 当前支付方式为翼支付付款码
                                    break;
                                case "81":// 志愿者兑换
                                    PaymentOper.PaymentList.Now_PayMent = BusinessEnum.PayMent.Volunteer;// 当前支付方式为志愿者兑换
                                    break;
                            }
                            BarCode_Net_PayInfo.PayNum = strPayNum;
                            BarCode_Net_PayInfo.PayType = strPayType;
                        }
                        #endregion
                        break;
                    case 3:// 等待付款
                        _resultCode = 3;
                        break;
                    default:// 支付失败
                        _resultCode = 2;
                        break;
                }
                
                if (type == "0")
                {
                    if ((_resultCode != 1) && (_resultCode != 3))
                    {
                        // 支付失败，发送支付失败相关数据到网关
                        BarCode_Net_UploadData(false, false, asileCode);

                        // 不是成功状态或等待状态，则开启支付方式环境
                        ControlPayEnviro(BusinessEnum.PayMent.AliPay_Code, true, true);
                    }
                }
                else
                {
                    // 志愿者兑换
                    if (intErrCode != 0)
                    {
                        // 失败则开启支付方式环境
                        ControlPayEnviro(BusinessEnum.PayMent.AliPay_Code, true, true);
                    }
                }

                AddBusLog_Code(strLogType, intErrCode.ToString(), "Code:" + barCode + ",Result:" + _resultCode);
            }
            catch(Exception ex)
            {
                _resultCode = 0;
                intErrCode = 99;
                AddErrLog(strLogType, intErrCode.ToString(), ex.Message);
            }

            return intErrCode;
        }

        /// <summary>
        /// 查询条形码扫描在线支付结果
        /// </summary>
        /// <param name="aliPayCode"></param>
        /// <param name="asileCode"></param>
        /// <param name="money"></param>
        /// <param name="_resultCode">支付请求结果 1：成功 2：失败 3：等待</param>
        /// <returns></returns>
        public int BarCode_Net_Query(string payCode, string asileCode,int money, out int _resultCode)
        {
            int intErrCode = 0;

            _resultCode = 0;
            string _resultData = string.Empty;// 回复数据
            string strPayNum = "0";// 支付账号
            strPayNum = strPayNum.PadLeft(10, '0');
            string strPayType = "90";// 支付类型

            string strLogType = "BarCode_Net_Query";

            try
            {
                BarCode_Net_PayInfo.Money = money;

                /*查询策略
                 * 总共查询6次，每次间隔5秒
                */
                int intDelayNum = 0;
                for (int i = 0; i < 6; i++)
                {
                    while (true)
                    {
                        Thread.Sleep(50);
                        intDelayNum++;
                        if (intDelayNum > 100)
                        {
                            intDelayNum = 0;
                            break;
                        }
                    }

                    intErrCode = m_OnlinePayOper.BarCode_Net_Query(m_BusId.ToString(), payCode, money, out _resultData);
                    if (intErrCode == 0)
                    {
                        // 成功，解析数据
                        string[] hexData = _resultData.Split('*');
                        if (hexData.Length > 3)
                        {
                            _resultCode = Convert.ToInt32(hexData[0]);
                            strPayNum = hexData[3];// 支付账号
                            strPayType = hexData[1];// 支付类型
                        }
                        else
                        {
                            intErrCode = 9;
                        }
                    }
                    AddBusLog_Code(strLogType, intErrCode.ToString(), "Code:" + payCode + ",Result:" + _resultCode);
                    if (_resultCode == 1)
                    {
                        // 查询支付结果成功
                        break;
                    }
                }

                if (_resultCode != 1)
                {
                    // 不是成功状态或等待状态，则开启支付方式环境
                    ControlPayEnviro(BusinessEnum.PayMent.AliPay_Code, true, true);
                }

                // 传送支付数据到网关
                if (_resultCode == 1)
                {
                    // 支付成功
                    m_SaleType = "1";// 更改销售方式
                    switch (strPayType)
                    {
                        case "90":// 支付宝付款码
                            PaymentOper.PaymentList.Now_PayMent = BusinessEnum.PayMent.AliPay_Code;// 当前支付方式为支付宝付款码
                            break;
                        case "92":// 微信刷卡
                            PaymentOper.PaymentList.Now_PayMent = BusinessEnum.PayMent.WeChatCode;// 当前支付方式为微信刷卡
                            break;
                        case "76":// 翼支付付款码
                            PaymentOper.PaymentList.Now_PayMent = BusinessEnum.PayMent.BestPay_Code;// 当前支付方式为翼支付付款码刷卡
                            break;
                    }
                    
                    BarCode_Net_PayInfo.PayCode = payCode;
                    BarCode_Net_PayInfo.PayNum = strPayNum;
                    BarCode_Net_PayInfo.PayType = strPayType;
                }
                else
                {
                    // 支付失败，发送支付失败相关数据到网关
                    BarCode_Net_UploadData(false, false, asileCode);
                }
            }
            catch(Exception ex)
            {
                _resultCode = 0;
                intErrCode = 99;
                AddErrLog(strLogType, intErrCode.ToString(), ex.Message);
            }

            return intErrCode;
        }

        /// <summary>
        /// 上传条形码扫描在线支付数据（支付宝付款码、微信刷卡）
        /// </summary>
        /// <param name="payResult">支付结果 False：失败 True：成功</param>
        /// <param name="payCode"></param>
        /// <param name="asileCode"></param>
        /// <param name="money"></param>
        private void BarCode_Net_UploadData(bool payResult, bool sellResult, string asileCode)
        {
            string strPayResult = "1";// 扣款结果 0：成功 其它：失败
            string strSellResult = "1";// 出货结果 0：成功 其它：失败

            if (payResult)
            {
                strPayResult = "0";// 扣款成功
                if (sellResult)
                {
                    strSellResult = "0";// 出货成功
                }
            }
            
            // 发送条形码扫描在线支付数据

            m_GateSocket.BarCode_Pay_Result(m_BusId.ToString(), BarCode_Net_PayInfo.PayType,
                BarCode_Net_PayInfo.PayNum, BarCode_Net_PayInfo.PayCode, BarCode_Net_PayInfo.Money, strPayResult, asileCode, strSellResult);
            ////////if ((!payResult) || (!sellResult))
            ////////{
            ////////    // 如果支付失败或者出货失败，发送冲正数据
            ////////    string strResultData = string.Empty;
            ////////    m_RealTimeGateServer.BarCode_Net_Return(m_BusId.ToString(), BarCode_Net_PayInfo.PayCode,
            ////////        BarCode_Net_PayInfo.Money, out strResultData);
            ////////}
        }

        #endregion

        #region 二代身份证业务流程

        /// <summary>
        /// 初始化二代身份证模块
        /// </summary>
        private void InitIDCard()
        {
            ConfigInfo.IDCardPort = SysCfgOper.GetSysCfgValue("IDCardPort");
            IDCardOper.ComPort = ConfigInfo.IDCardPort;
        }

        #endregion

        #region 上海志愿者项目业务流程（爱心捐赠）

        /// <summary>
        /// 上海志愿者—提交志愿者兑换码支付请求
        /// </summary>
        /// <param name="barCode"></param>
        /// <param name="asileCode"></param>
        /// <param name="money"></param>
        /// <param name="_resultCode">支付请求结果 1：成功 2：失败 3：等待</param>
        /// <returns></returns>
        public int ShZyZ_BarCode_Net_Pay(string barCode, string asileCode, int money, out int _resultCode, out string _orderNo)
        {
            int intErrCode = 0;

            _resultCode = 0;// 1：支付成功 2：支付失败 3：等待支付
            _orderNo = string.Empty;

            string strPayType = "90";// 支付类型

            string strLogType = "ShZyZ_BarCode_Net_Pay";

            try
            {
                #region 停止工作

                BarCodeOper.StopScan();

                #endregion

                bool result = CreateNewBusiness();// 创建新的业务流水号

                HttpOper httpOper = new HttpOper();
                string strUrl = SysCfgOper.GetSysCfgValue("WxTake_ServerUrl") + "/volunteer/ExchangeCode_Update_Vm?" +
                    "id=" + m_BusId.ToString() +
                    "&vm=" + ConfigInfo.VmId +
                    "&pa=" + asileCode +
                    "&money=" + money +
                    "&code=" + barCode +
                    "&token=" + DateTime.Now.ToString("yyyyMMddHHmmss");
                string strResponseData = string.Empty;
                string strStatusCode = httpOper.CreateGetHttpResponse(strUrl, Encoding.UTF8, 30000, "", null, out strResponseData);
                string strResultCode = string.Empty;
                if (strStatusCode == "200")
                {
                    JsonOper m_JsonOper = new JsonOper();
                    strResultCode = m_JsonOper.GetJsonKeyValue(strResponseData, "ret").Trim();
                    // 1成功、2失败、3等待、4无效的订单、5传入的参数错误、9非法的接入
                    switch (strResultCode)
                    {
                        case "1":// 成功
                            _resultCode = 1;
                            break;
                        case "2":// 失败
                            _resultCode = 2;
                            break;
                        case "3":// 等待
                            _resultCode = 3;
                            _orderNo = m_JsonOper.GetJsonKeyValue(strResponseData, "order").Trim();
                            break;
                        default:// 认为失败
                            _resultCode = 2;
                            break;
                    }
                }
                else
                {
                    intErrCode = 99;
                }
            }
            catch (Exception ex)
            {
                _resultCode = 0;
                intErrCode = 99;
                AddErrLog(strLogType, intErrCode.ToString(), ex.Message);
            }

            return intErrCode;
        }

        /// <summary>
        /// 上海志愿者—提交条形码扫描在线捐赠请求
        /// </summary>
        /// <param name="barCode"></param>
        /// <param name="asileCode"></param>
        /// <param name="money"></param>
        /// <param name="_resultCode">支付请求结果 1：成功 2：失败 3：等待</param>
        /// <returns></returns>
        public int ShZyZ_BarCode_Don_Pay(string barCode, string asileCode, int money, out int _resultCode,out string _orderNo)
        {
            int intErrCode = 0;

            _resultCode = 0;// 1：支付成功 2：支付失败 3：等待支付
            _orderNo = string.Empty;

            string strPayType = "90";// 支付类型

            string strLogType = "ShZyZ_BarCode_Don_Pay";

            try
            {
                #region 停止工作

                BarCodeOper.StopScan();

                #endregion

                bool result = CreateNewBusiness();// 创建新的业务流水号

                HttpOper httpOper = new HttpOper();
                string strUrl = SysCfgOper.GetSysCfgValue_Third("DonUploadWebUrl") + "/Create?" +
                    "id=" + m_BusId.ToString() + 
                    "&vm=" + ConfigInfo.VmId +
                    "&pa=" + asileCode + 
                    "&money=" + money + 
                    "&code=" + barCode +
                    "&token=" + DateTime.Now.ToString("yyyyMMddHHmmss");
                string strResponseData = string.Empty;
                string strStatusCode = httpOper.CreateGetHttpResponse(strUrl,Encoding.UTF8, 30000, "", null, out strResponseData);
                string strResultCode = string.Empty;
                if (strStatusCode == "200")
                {
                    JsonOper m_JsonOper = new JsonOper();
                    strResultCode = m_JsonOper.GetJsonKeyValue(strResponseData, "ret").Trim();
                    // 1成功、2失败、3等待、4无效的订单、5传入的参数错误、9非法的接入
                    switch (strResultCode)
                    {
                        case "1":// 成功
                            _resultCode = 1;
                            break;
                        case "2":// 失败
                            _resultCode = 2;
                            break;
                        case "3":// 等待
                            _resultCode = 3;
                            _orderNo = m_JsonOper.GetJsonKeyValue(strResponseData, "order").Trim();
                            break;
                        default:// 认为失败
                            _resultCode = 2;
                            break;
                    }
                }
                else
                {
                    intErrCode = 99;
                }
            }
            catch (Exception ex)
            {
                _resultCode = 0;
                intErrCode = 99;
                AddErrLog(strLogType, intErrCode.ToString(), ex.Message);
            }

            return intErrCode;
        }

        /// <summary>
        /// 上海志愿者—查询条形码扫描在线捐赠结果
        /// </summary>
        /// <param name="aliPayCode"></param>
        /// <param name="asileCode"></param>
        /// <param name="money"></param>
        /// <param name="_resultCode">支付请求结果 1：成功 2：失败 3：等待</param>
        /// <returns></returns>
        public int ShZyZ_BarCode_Don_Query(string orderNo, out int _resultCode)
        {
            int intErrCode = 0;

            _resultCode = 0;
            string _resultData = string.Empty;// 回复数据

            string strLogType = "ShZyZ_BarCode_Don_Query";

            try
            {
                /*查询策略
                 * 总共查询4次，每次间隔5秒
                */
                int intDelayNum = 0;
                for (int i = 0; i < 4; i++)
                {
                    while (true)
                    {
                        Thread.Sleep(50);
                        intDelayNum++;
                        if (intDelayNum > 100)
                        {
                            intDelayNum = 0;
                            break;
                        }
                    }

                    HttpOper httpOper = new HttpOper();
                    string strUrl = SysCfgOper.GetSysCfgValue_Third("DonUploadWebUrl") + "/Query?" +
                        "vm=" + ConfigInfo.VmId +
                        "&order=" + orderNo +
                        "&paytype=" + "1" +
                        "&token=" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    string strResponseData = string.Empty;
                    string strStatusCode = httpOper.CreateGetHttpResponse(strUrl, Encoding.UTF8, 30000, "", null, out strResponseData);
                    string strResultCode = string.Empty;
                    if (strStatusCode == "200")
                    {
                        JsonOper m_JsonOper = new JsonOper();
                        strResultCode = m_JsonOper.GetJsonKeyValue(strResponseData, "ret").Trim();
                        // 1成功、2失败、3等待、4无效的订单、5传入的参数错误、9非法的接入
                        switch (strResultCode)
                        {
                            case "1":// 成功
                                _resultCode = 1;
                                break;
                            case "2":// 失败
                                _resultCode = 2;
                                break;
                            case "3":// 等待
                                _resultCode = 3;
                                break;
                            default:// 认为失败
                                _resultCode = 2;
                                break;
                        }
                        if (_resultCode != 3)
                        {
                            break;
                        }
                    }
                    else
                    {
                        intErrCode = 99;
                    }
                }
            }
            catch (Exception ex)
            {
                _resultCode = 0;
                intErrCode = 99;
                AddErrLog(strLogType, intErrCode.ToString(), ex.Message);
            }

            return intErrCode;
        }

        #endregion

        #endregion

        #region 系统配置参数相关控制

        /// <summary>
        /// 更新某系统参数配置
        /// </summary>
        /// <param name="sysCfgName">系统配置参数名称</param>
        /// <param name="sysCfgValue">系统配置参数实际值</param>
        /// <returns>保存结果 True：成功 False：失败</returns>
        public bool UpdateSysCfgValue(string sysCfgName, string sysCfgValue)
        {
            bool result = false;

            try
            {
                int intSysCfgCount = SysCfgOper.SysCfgList.Count;
                string strSysCfgID = string.Empty;
                for (int i = 0; i < intSysCfgCount; i++)
                {
                    if (SysCfgOper.SysCfgList[i].CfgName == sysCfgName)
                    {
                        strSysCfgID = SysCfgOper.SysCfgList[i].CfgId;
                        #region 保存到数据库中
                        result = SysCfgOper.UpdateSysCfg(sysCfgName, sysCfgValue);
                        ////string strSql = "update T_SYS_CONFIG set [CfgFactValue] = '" + sysCfgValue + "' where [CfgName]='" + sysCfgName + "'";

                        ////DbOper dbOper = new DbOper();
                        ////dbOper.DbFileName = _m_DbFileName;
                        ////dbOper.ConnType = ConnectType.CloseConn;
                        ////result = dbOper.excuteSql(strSql);
                        ////dbOper.closeConnection();

                        if (result)
                        {
                            #region 更新内存中存储的参数值
                            switch (sysCfgName)
                            {
                                case "Language":// 语言
                                    ConfigInfo.Language = FunHelper.ConvertLanguage(sysCfgValue);
                                    break;
                                case "IsRunStock":// 是否启用库存
                                    ConfigInfo.IsRunStock = FunHelper.ChangeControlSwitch(sysCfgValue);// 是否启用库存
                                    break;
                                case "SaleModel":// 销售模式
                                    ConfigInfo.SaleModel = sysCfgValue;
                                    break;
                                case "ChangeModel":// 兑零模式
                                    ConfigInfo.ChangeModel = sysCfgValue;
                                    break;
                                case "TunOutTime":// 吞币超时时间
                                    ConfigInfo.TunOutTime = Convert.ToInt32(sysCfgValue);
                                    break;
                                case "SellOperOutTime":// 购物无操作超时时间
                                    ConfigInfo.SellOperOutTime = Convert.ToInt32(sysCfgValue);
                                    break;
                                ////////case "RefOpenMaxTime":// 压缩机最大工作时间
                                ////////    RefControl.OpenMaxTime = Convert.ToInt32(sysCfgValue);
                                ////////    break;
                                ////////case "TmpRunDelay":// 压缩机关闭延时时间
                                ////////    RefControl.CloseDelayTime = Convert.ToInt32(sysCfgValue);
                                ////////    break;
                                ////////case "TargetTmp1":// 目标温度
                                ////////    RefControl.TargetTmp = sysCfgValue;
                                    ////////break;
                                case "IcControlSwitch":
                                    PaymentOper.PaymentList.IC.ControlSwitch = PaymentOper.ChangePaymentControl(BusinessEnum.PayMent.IcCard,sysCfgValue);
                                    break;

                                case "IcQuerySwitch":// 是否查询卡信息
                                    ConfigInfo.IcQuerySwitch = FunHelper.ChangeControlSwitch(sysCfgValue);
                                    break;
                                case "ShowCardInfoTime":// 在主界面时显示卡查询信息展现时间 以秒为单位
                                    ConfigInfo.ShowCardInfoTime = Convert.ToInt32(sysCfgValue);
                                    break;

                                case "CashControlSwitch":
                                    PaymentOper.PaymentList.Cash.ControlSwitch = PaymentOper.ChangePaymentControl(BusinessEnum.PayMent.Cash, sysCfgValue);
                                    break;
                                case "WeChatCodeSwitch":
                                    PaymentOper.PaymentList.WeChatCode.ControlSwitch = PaymentOper.ChangePaymentControl(BusinessEnum.PayMent.WeChatCode, sysCfgValue);
                                    break;
                                case "NoFeeCardSwitch":
                                    PaymentOper.PaymentList.NoFeeCard.ControlSwitch = PaymentOper.ChangePaymentControl(BusinessEnum.PayMent.NoFeeCard, sysCfgValue);
                                    break;
                                case "QRCodeCardSwitch":
                                    PaymentOper.PaymentList.QRCodeCard.ControlSwitch = PaymentOper.ChangePaymentControl(BusinessEnum.PayMent.QRCodeCard, sysCfgValue);
                                    break;
                                case "UnionPaySwitch":
                                    PaymentOper.PaymentList.UnionPay.ControlSwitch = PaymentOper.ChangePaymentControl(BusinessEnum.PayMent.QuickPass, sysCfgValue);
                                    break;
                                case "AliPayCodeSwitch":
                                    PaymentOper.PaymentList.AliPay_Code.ControlSwitch = PaymentOper.ChangePaymentControl(BusinessEnum.PayMent.AliPay_Code, sysCfgValue);
                                    break;
                                case "BestPayCodeSwitch":
                                    PaymentOper.PaymentList.BestPay_Code.ControlSwitch = PaymentOper.ChangePaymentControl(BusinessEnum.PayMent.BestPay_Code, sysCfgValue);
                                    break;
                                case "VolunteerPay_Switch":
                                    PaymentOper.PaymentList.Volunteer_Code.ControlSwitch = PaymentOper.ChangePaymentControl(BusinessEnum.PayMent.Volunteer, sysCfgValue);
                                    break;

                                ////case "CoinControlSwitch":
                                ////    DeviceInfo.CoinControlSwitch = FunHelper.ChangeControlSwitch(sysCfgValue);
                                ////    break;

                                case "NetSwitch":
                                    if (sysCfgValue == "1")
                                    {
                                        // 启用
                                        m_GateSocket.IsEnable = true;
                                    }
                                    else
                                    {
                                        // 禁用
                                        m_GateSocket.IsEnable = false;
                                    }
                                    break;

                                case "GoodsShowContent":// 商品列表展示内容
                                    ConfigInfo.GoodsShowContent = sysCfgValue;
                                    break;

                                case "GoodsNameShowType":// 商品展示页面中商品名称显示类型
                                    ConfigInfo.GoodsNameShowType = sysCfgValue;
                                    break;

                                case "NoStockClickGoods":// 商品无库存时是否允许点击
                                    ConfigInfo.NoStockClickGoods = sysCfgValue;
                                    break;

                                case "GoodsOpacity":// 商品无库存时透明度
                                    ConfigInfo.GoodsOpacity = Convert.ToInt32(sysCfgValue);
                                    break;

                                case "IcBusiModel":// 储值卡业务流程模式
                                    ConfigInfo.IcBusiModel = sysCfgValue;
                                    break;
                                case "IcPayShowSwitch":// 储值卡用户信息是否显示
                                    ConfigInfo.IcPayShowSwitch = FunHelper.ChangeControlSwitch(sysCfgValue);
                                    break;
                                case "NoFeeCardPayShow":// 会员卡用户信息是否显示
                                    ConfigInfo.NoFeeCardPayShow = FunHelper.ChangeControlSwitch(sysCfgValue);
                                    break;
                                case "IcCardNumHide":// 储值卡卡号信息*字显示
                                    ConfigInfo.IcCardNumHide = FunHelper.ChangeControlSwitch(sysCfgValue);
                                    break;
                                case "NoFeeCardNumHide":// 会员卡卡号信息*字显示
                                    ConfigInfo.NoFeeCardNumHide = FunHelper.ChangeControlSwitch(sysCfgValue);
                                    break;
                                case "CountryCode":// 国家代码
                                    // 获取国家相关信息，包括小数点位数、最小货币面值
                                    SysCfgOper.GetCountryInfo(sysCfgValue);
                                    ConfigInfo.DecimalNum = Convert.ToInt32(SysCfgOper.CountryInfo.DecimalNum);
                                    ConfigInfo.MinMoneyValue = Convert.ToInt32(SysCfgOper.CountryInfo.MinMoneyValue);
                                    ConfigInfo.MoneySymbol = SysCfgOper.CountryInfo.MoneySymbol;
                                    break;
                                case "IsShowMoneySymbol":// 是否显示货币符号
                                    ConfigInfo.IsShowMoneySymbol = FunHelper.ChangeControlSwitch(sysCfgValue);
                                    break;
                                case "NoFeeCardIsRebate":// 会员卡是否打折
                                    ConfigInfo.NoFeeCardIsRebate = sysCfgValue;
                                    break;

                                #region  广告播放参数 2015-03-19添加

                                case "AdvertPlaySwitch":// 广告是否播放
                                    ConfigInfo.AdvertPlaySwitch = FunHelper.ChangeControlSwitch(sysCfgValue);
                                    break;
                                case "AdvertImgShowTime":// 图片广告显示时间
                                    ConfigInfo.AdvertImgShowTime = Convert.ToInt32(sysCfgValue);
                                    break;
                                case "AdvertPlayOutTime":// 无人操作播放广告时间
                                    ConfigInfo.AdvertPlayOutTime = Convert.ToInt32(Convert.ToDouble(sysCfgValue) * 60);
                                    break;
                                case "AdvertUploadType":// 广告更新策略
                                    ConfigInfo.AdvertUploadType = sysCfgValue;
                                    AdvertOper.AdvertUploadType = sysCfgValue;
                                    break;

                                case "GoodsUploadType":// 产品更新策略
                                    ConfigInfo.GoodsUploadType = sysCfgValue;
                                    break;
                                #endregion

                                #region 商品不对应货道模式参数 2015-05-23添加
                                case "EachPageMaxRowNum":// 
                                    ConfigInfo.EachPageMaxRowNum = Convert.ToInt32(sysCfgValue);
                                    break;
                                case "EachRowMaxColuNum":// 
                                    ConfigInfo.EachRowMaxColuNum = Convert.ToInt32(sysCfgValue);
                                    break;
                                #endregion

                                ////case "UpDownSellModel"://
                                ////    ConfigInfo.UpDownSellModel = sysCfgValue;// 升降机出货形式
            
                                    ////break;
                                ////case "UpDownDelayTimeNums":
                                ////    ConfigInfo.UpDownDelayTimeNums = Convert.ToInt32(sysCfgValue);// 升降机出货延时
                                ////    break;
                                ////case "UpDownSendGoodsTimes"://
                                ////    ConfigInfo.UpDownSendGoodsTimes = Convert.ToInt32(sysCfgValue);// 云台至出货口后延时时长，以毫秒为单位
                                ////    break;
                                ////case "UpDownIsQueryElectStatus"://
                                ////    ConfigInfo.UpDownIsQueryElectStatus = FunHelper.ChangeControlSwitch(sysCfgValue);// 是否检测升降机光电管状态
                                ////    break;

                                case "UpDownLeftRightNum_Left":// 升降机横向点击移动码盘数—最左边
                                    ConfigInfo.UpDownLeftRightNum_Left = Convert.ToInt32(sysCfgValue);
                                    break;
                                case "UpDownLeftRightNum_Center":// 升降机横向点击移动码盘数—中间
                                    ConfigInfo.UpDownLeftRightNum_Center = Convert.ToInt32(sysCfgValue);
                                    break;
                                case "UpDownLeftRightNum_Right":// 升降机横向点击移动码盘数—最右边
                                    ConfigInfo.UpDownLeftRightNum_Right = Convert.ToInt32(sysCfgValue);
                                    break;


                                case "GoodsPropShowType"://
                                    ConfigInfo.GoodsPropShowType = sysCfgValue;
                                    break;
                                case "IsShowGoodsDetailContent"://
                                    ConfigInfo.IsShowGoodsDetailContent = FunHelper.ChangeControlSwitch(sysCfgValue);
                                    break;
                                case "GoodsDetailFontSize"://
                                    ConfigInfo.GoodsDetailFontSize = Convert.ToInt32(sysCfgValue);
                                    break;

                                case "IsPrintConsumeBill"://
                                    ConfigInfo.IsPrintConsumeBill = FunHelper.ChangeControlSwitch(sysCfgValue);//是否打印购物单据
                                    break;
                                case "PrintPort"://
                                    ConfigInfo.PrintPort = sysCfgValue;
                                    break;
                                case "PrintTmepContent"://
                                    ConfigInfo.PrintTmepContent = sysCfgValue;// 打印内容
                                    break;
                                case "PrintTmepTitle"://
                                    ConfigInfo.PrintTmepTitle = sysCfgValue;// 打印表头
                                    break;

                                case "IDCardPort"://
                                    ConfigInfo.IDCardPort = sysCfgValue;
                                    IDCardOper.ComPort = sysCfgValue;
                                    break;

                                case "IsShowChoiceKeyBoard":// 是否显示选货键盘
                                    ConfigInfo.IsShowChoiceKeyBoard = FunHelper.ChangeControlSwitch(sysCfgValue);// 
                                    break;
                                case "KeyBoardType":// 输入键盘类型
                                    ConfigInfo.KeyBoardType = sysCfgValue;
                                    break;

                                case "IDCardFreeTake_Switch"://
                                    ConfigInfo.IDCardFreeTake_Switch = FunHelper.ChangeControlSwitch(sysCfgValue);//
                                    break;
                                case "O2OTake_Switch"://
                                    ConfigInfo.O2OTake_Switch = FunHelper.ChangeControlSwitch(sysCfgValue);//
                                    break;
                                case "WxTake_Switch"://
                                    ConfigInfo.WxTake_Switch = FunHelper.ChangeControlSwitch(sysCfgValue);//
                                    break;

                                case "CoinDeviceType"://
                                    ConfigInfo.CoinDeviceType = FunHelper.ConvertCoinDeviceType(sysCfgValue);
                                    break;
                                case "IsReturnBill"://
                                    ConfigInfo.IsReturnBill = FunHelper.ChangeControlSwitch(sysCfgValue);//
                                    break;

                                case "BillStackSwitch":// 纸币器缓存开关 0：关闭缓存 1：打开缓存
                                    ConfigInfo.BillStackSwitch = FunHelper.ChangeControlSwitch(sysCfgValue);//
                                    break;

                                case "RemoteControlSwitch"://
                                    ConfigInfo.RemoteControlSwitch = FunHelper.ChangeControlSwitch(sysCfgValue);
                                    break;

                                #region 电子支付网关参数
                                case "OnlinePayUrl"://
                                    m_OnlinePayOper.OnlinePayUrl = sysCfgValue;
                                    break;
                                case "OnlinePayToken":
                                    m_OnlinePayOper.Token = sysCfgValue;
                                    break;            

                                #endregion

                                case "PriceUploadType":// 价格设置策略 0：本地设置 1：远程更新
                                    ConfigInfo.PriceUploadType = sysCfgValue;
                                    break;

                                ////case "CashManagerModel"://
                                ////    ConfigInfo.CashManagerModel = sysCfgValue;
                                ////    break;
                            }
                            #endregion

                            if ((SysCfgOper.SysCfgList[i].CfgLevel == "U") &&
                                (SysCfgOper.SysCfgList[i].IsReset == "1") &&
                                (SysCfgOper.SysCfgList[i].CfgFactValue != sysCfgValue))
                            {
                                // 如果是用户级别的参数发生了改变，则更新参数值
                                m_GateSocket.UpdateParameter(SysCfgOper.SysCfgList[i].CfgId, sysCfgValue,
                                    SysCfgOper.SysCfgList[i].CfgFactValue, "");
                            }
                            SysCfgOper.SysCfgList[i].CfgFactValue = sysCfgValue;

                            #region 另外刷新一些内存的参数

                            #region 临时注释

                            if ((strSysCfgID == "340") || (strSysCfgID == "341") || (strSysCfgID == "342") ||
                                (strSysCfgID == "343") || (strSysCfgID == "344"))
                            {
                                // 照明设备控制策略参数需要更新
                                UpdateDeviceControlCfg(BusinessEnum.DeviceList.Light);
                            }
                            if ((strSysCfgID == "380") || (strSysCfgID == "381") || (strSysCfgID == "382") ||
                                (strSysCfgID == "383") || (strSysCfgID == "384"))
                            {
                                // 广告灯控制策略参数需要更新
                                UpdateDeviceControlCfg(BusinessEnum.DeviceList.AdvertLight);
                            }
                            if ((strSysCfgID == "400") || (strSysCfgID == "401") || (strSysCfgID == "402") ||
                                (strSysCfgID == "403") || (strSysCfgID == "404"))
                            {
                                // 工控电脑控制策略参数需要更新
                                ComputerControl.IsRefreshCfg = true;
                            }
                            if ((strSysCfgID == "252") || (strSysCfgID == "253") || (strSysCfgID == "254") ||
                                (strSysCfgID == "255") || (strSysCfgID == "256"))
                            {
                                // 除雾设备控制策略参数需要更新
                                UpdateDeviceControlCfg(BusinessEnum.DeviceList.Demister);
                            }
                            if ((strSysCfgID == "360") || (strSysCfgID == "361") || (strSysCfgID == "362") ||
                                (strSysCfgID == "363") || (strSysCfgID == "364"))
                            {
                                // 紫外灯控制策略参数需要更新
                                UpdateDeviceControlCfg(BusinessEnum.DeviceList.UltravioletLamp);
                            }
                            #endregion
                            #endregion
                        }
                        #endregion

                        //result = true;
                        break;
                    }
                }
            }
            catch
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// 系统参数恢复出厂设置
        /// </summary>
        /// <returns>结果 False：失败 True：成功</returns>
        public bool InitFactoryCfg()
        {
            bool result = false;

            DbOper dbOper = new DbOper();
            dbOper.DbFileName = _m_DbFileName;

            try
            {
                #region 恢复之前先备份数据

                result = FunHelper.BackUpDbFile("TermInfo");
                if (!result)
                {
                    return false;
                }

                #endregion

                // 把能恢复出产设置的参数恢复出厂设置
                string strSql = "update T_SYS_CONFIG set [CfgFactValue] = [CfgFactoryValue] where [IsReset] = '1'";

                dbOper.ConnType = ConnectType.KeepConn;
                result = dbOper.excuteSql(strSql);
                if (result)
                {
                    // 初始化货道数据
                    strSql = @"update T_VM_PAINFO set [SurNum] = 0,[PaStackNum] = 0,[SellPrice] = 1,[PaKind] = '0'";
                    result = dbOper.excuteSql(strSql);
                    if (result)
                    {
                        // 初始化货道数据成功，更新货道列表数据
                        int intPaListCount = AsileOper.AsileList.Count;
                        for (int i = 0; i < intPaListCount; i++)
                        {
                            AsileOper.AsileList[i].PaKind = "0";
                            AsileOper.AsileList[i].SurNum = 0;
                            AsileOper.AsileList[i].PaStackNum = 0;
                            AsileOper.AsileList[i].SellPrice = "1";
                        }
                    }
                }
            }
            catch
            {
                result = false;
            }
            finally
            {
                dbOper.closeConnection();
            }

            return result;
        }

        /// <summary>
        /// 刷新当前交易号
        /// </summary>
        /// <returns>结果 False：失败 True：成功</returns>
        public bool CreateNewBusiness()
        {
            bool result = false;

            m_BusId++;

            // 保存到数据库中
            result = UpdateSysCfgValue("BusinessId", m_BusId.ToString());

            return result;
        }

        /// <summary>
        /// 读取相关设备控制参数
        /// </summary>
        private void RefreshDeviceControlCfg_Main(bool isInit)
        {
            #region 临时注释

            ////////if (RefControl.IsRefreshCfg)
            ////////{
            ////////    // 制冷压缩机控制参数
            ////////    RefreshDeviceControlCfg(BusinessEnum.DeviceList.Refrigeration);
            ////////    RefControl.IsRefreshCfg = false;
            ////////}

            ////////if (LightControl.IsRefreshCfg)
            ////////{
            ////////    // 照明灯控制参数
            ////////    RefreshDeviceControlCfg(BusinessEnum.DeviceList.Light);
            ////////    LightControl.IsRefreshCfg = false;
            ////////}

            ////////if (DeviceInfo.DriveConnType == BusinessEnum.ControlSwitch.Stop)
            ////////{
            ////////    if (UltravioletLampControl.IsRefreshCfg)
            ////////    {
            ////////        // 紫外灯控制参数
            ////////        RefreshDeviceControlCfg(BusinessEnum.DeviceList.UltravioletLamp);
            ////////        UltravioletLampControl.IsRefreshCfg = false;
            ////////    }
            ////////}
            ////////// 广告灯控制参数
            ////////if (AdvertLightControl.IsRefreshCfg)
            ////////{
            ////////    RefreshDeviceControlCfg(BusinessEnum.DeviceList.AdvertLight);
            ////////    AdvertLightControl.IsRefreshCfg = false;
            ////////}
            ////////if (DeviceInfo.DriveConnType == BusinessEnum.ControlSwitch.Run)
            ////////{
            ////////    if (DemisterControl.IsRefreshCfg)
            ////////    {
            ////////        // 除雾设备控制参数
            ////////        RefreshDeviceControlCfg(BusinessEnum.DeviceList.Demister);
            ////////        DemisterControl.IsRefreshCfg = false;
            ////////    }
            ////////    if (ComputerControl.IsRefreshCfg)
            ////////    {
            ////////        // 工控/显示屏控制参数
            ////////        RefreshDeviceControlCfg(BusinessEnum.DeviceList.ComPuter);
            ////////        ComputerControl.IsRefreshCfg = false;
            ////////    }
            ////////}

            #endregion
        }

        /// <summary>
        /// 刷新制冷、照明灯等相关设备控制参数
        /// </summary>
        /// <param name="deviceName">设备</param>
        /// <returns></returns>
        private bool RefreshDeviceControlCfg(int vendBoxIndex,BusinessEnum.DeviceList deviceName)
        {
            bool result = false;

            #region 临时注释

            switch (deviceName)
            {
                case BusinessEnum.DeviceList.Refrigeration:// 制冷压缩机
                    //AsileOper.VendBoxList[vendBoxIndex].RefControl.ControlModel = SysCfgOper.GetSysCfgValue("Tmp1RunModel");
                    //RefControl.TargetTmp = SysCfgOper.GetSysCfgValue("TargetTmp1");
                    //RefControl.BeginTime1 = SysCfgOper.GetSysCfgValue("Tmp1BeginTime1");
                    //RefControl.EndTime1 = SysCfgOper.GetSysCfgValue("Tmp1EndTime1");
                    //RefControl.BeginTime2 = SysCfgOper.GetSysCfgValue("Tmp1BeginTime2");
                    //RefControl.EndTime2 = SysCfgOper.GetSysCfgValue("Tmp1EndTime2");
                    ////RefControl.BeginTime3 = SysCfgOper.GetSysCfgValue("Tmp1BeginTime3");
                    ////RefControl.EndTime3 = SysCfgOper.GetSysCfgValue("Tmp1EndTime3");
                    break;

                case BusinessEnum.DeviceList.Demister:// 除雾设备
                    AsileOper.VendBoxList[vendBoxIndex].DemisterControl.ControlModel = SysCfgOper.GetSysCfgValue("FogModel");

                    AsileOper.VendBoxList[vendBoxIndex].DemisterControl.BeginTime1 = SysCfgOper.GetSysCfgValue("FogBeginTime1");
                    AsileOper.VendBoxList[vendBoxIndex].DemisterControl.EndTime1 = SysCfgOper.GetSysCfgValue("FogEndTime1");
                    AsileOper.VendBoxList[vendBoxIndex].DemisterControl.BeginTime2 = SysCfgOper.GetSysCfgValue("FogBeginTime2");
                    AsileOper.VendBoxList[vendBoxIndex].DemisterControl.EndTime2 = SysCfgOper.GetSysCfgValue("FogEndTime2");
                    break;

                case BusinessEnum.DeviceList.Light:// 照明灯
                    AsileOper.VendBoxList[vendBoxIndex].LightControl.ControlModel = SysCfgOper.GetSysCfgValue("LightModel");

                    AsileOper.VendBoxList[vendBoxIndex].LightControl.BeginTime1 = SysCfgOper.GetSysCfgValue("LightBeginTime1");
                    AsileOper.VendBoxList[vendBoxIndex].LightControl.EndTime1 = SysCfgOper.GetSysCfgValue("LightEndTime1");
                    AsileOper.VendBoxList[vendBoxIndex].LightControl.BeginTime2 = SysCfgOper.GetSysCfgValue("LightBeginTime2");
                    AsileOper.VendBoxList[vendBoxIndex].LightControl.EndTime2 = SysCfgOper.GetSysCfgValue("LightEndTime2");
                    break;

                case BusinessEnum.DeviceList.AdvertLight:// 广告灯
                    AsileOper.VendBoxList[vendBoxIndex].AdvertLightControl.ControlModel = SysCfgOper.GetSysCfgValue("AdLampModel");

                    AsileOper.VendBoxList[vendBoxIndex].AdvertLightControl.BeginTime1 = SysCfgOper.GetSysCfgValue("AdLampBeginTime1");
                    AsileOper.VendBoxList[vendBoxIndex].AdvertLightControl.EndTime1 = SysCfgOper.GetSysCfgValue("AdLampEndTime1");
                    AsileOper.VendBoxList[vendBoxIndex].AdvertLightControl.BeginTime2 = SysCfgOper.GetSysCfgValue("AdLampBeginTime2");
                    AsileOper.VendBoxList[vendBoxIndex].AdvertLightControl.EndTime2 = SysCfgOper.GetSysCfgValue("AdLampEndTime2");
                    break;

                case BusinessEnum.DeviceList.UltravioletLamp:// 紫外灯

                    AsileOper.VendBoxList[vendBoxIndex].UltravioletLampControl.ControlModel = SysCfgOper.GetSysCfgValue("SterilampModel");

                    AsileOper.VendBoxList[vendBoxIndex].UltravioletLampControl.BeginTime1 = SysCfgOper.GetSysCfgValue("SterilampBeginTime1");
                    AsileOper.VendBoxList[vendBoxIndex].UltravioletLampControl.EndTime1 = SysCfgOper.GetSysCfgValue("SterilampEndTime1");
                    AsileOper.VendBoxList[vendBoxIndex].UltravioletLampControl.BeginTime2 = SysCfgOper.GetSysCfgValue("SterilampBeginTime2");
                    AsileOper.VendBoxList[vendBoxIndex].UltravioletLampControl.EndTime2 = SysCfgOper.GetSysCfgValue("SterilampEndTime2");
                    break;

                //////case BusinessEnum.DeviceList.ComPuter:// 工控机及显示屏
                //////    ComputerControl.ControlModel = SysCfgOper.GetSysCfgValue("PowerModel");

                //////    ComputerControl.BeginTime1 = SysCfgOper.GetSysCfgValue("PowerBeginTime1");
                //////    ComputerControl.EndTime1 = SysCfgOper.GetSysCfgValue("PowerEndTime1");
                //////    ComputerControl.BeginTime2 = SysCfgOper.GetSysCfgValue("PowerBeginTime2");
                //////    ComputerControl.EndTime2 = SysCfgOper.GetSysCfgValue("PowerEndTime1");
                //////    break;

                //////case BusinessEnum.DeviceList.Sound:// 声音设备
                //////    SoundControl.ControlModel = SysCfgOper.GetSysCfgValue("SoundModel");

                //////    SoundControl.BeginTime1 = SysCfgOper.GetSysCfgValue("SoundBeginTime1");
                //////    SoundControl.EndTime1 = SysCfgOper.GetSysCfgValue("SoundEndTime1");
                //////    SoundControl.BeginTime2 = SysCfgOper.GetSysCfgValue("SoundBeginTime2");
                //////    SoundControl.EndTime2 = SysCfgOper.GetSysCfgValue("SoundEndTime2");
                //////    break;
            }

            #endregion

            result = true;

            return result;
        }

        /// <summary>
        /// 设置制冷、照明灯等相关设备控制参数需要刷新
        /// </summary>
        /// <param name="deviceName"></param>
        /// <returns></returns>
        private bool UpdateDeviceControlCfg(BusinessEnum.DeviceList deviceName)
        {
            bool result = false;

            for (int i = 0; i < AsileOper.VendBoxList.Count; i++)
            {
                switch (deviceName)
                {
                    case BusinessEnum.DeviceList.Refrigeration:// 制冷压缩机
                        break;
                    case BusinessEnum.DeviceList.Demister:// 除雾设备
                        AsileOper.VendBoxList[i].DemisterControl.IsRefreshCfg = true;
                        break;
                    case BusinessEnum.DeviceList.Light:// 照明灯
                        AsileOper.VendBoxList[i].LightControl.IsRefreshCfg = true;
                        break;
                    case BusinessEnum.DeviceList.AdvertLight:// 广告灯
                        AsileOper.VendBoxList[i].AdvertLightControl.IsRefreshCfg = true;
                        break;
                    case BusinessEnum.DeviceList.UltravioletLamp:// 紫外灯
                        AsileOper.VendBoxList[i].UltravioletLampControl.IsRefreshCfg = true;
                        break;
                }
            }
            result = true;

            return result;
        }

        #endregion

        #region 货道价格、弹簧圈数、库存等设置操作

        /// <summary>
        /// 更新货道价格/弹簧圈数/状态/销售模式
        /// </summary>
        /// <param name="paCode"></param>
        /// <param name="sellPrice"></param>
        /// <param name="springNum"></param>
        /// <param name="asileKind"></param>
        /// <param name="sellModel">销售模式 0：正常销售 1：赠品</param>
        /// <returns></returns>
        public bool UpdateAsileInfo(string paCode, string sellPrice, string springNum, string asileKind, string sellModel)
        {
            bool result = false;

            result = AsileOper.UpdateAsileInfo(paCode, sellPrice, springNum, asileKind,sellModel);

            if (result)
            {
                // 向网关汇报信息
                m_GateSocket.SetAsilePrice("1", paCode, sellPrice, "");// 货道价格
                m_GateSocket.SetAsileSprintNum("1", paCode, springNum, "");// 货道弹簧圈数
            }

            return result;
        }

        /// <summary>
        /// 更新托盘货道价格
        /// </summary>
        /// <param name="trayCode"></param>
        /// <param name="sellPrice"></param>
        /// <returns></returns>
        public bool UpdateTrayPrice(string trayCode, string sellPrice,string vendBoxCode)
        {
            bool result = false;

            result = AsileOper.UpdateTrayPrice(trayCode, sellPrice, vendBoxCode);
            if (result)
            {
                // 向网关汇报信息
                m_GateSocket.SetAsilePrice("2", FunHelper.ChangeTray((Convert.ToInt32(trayCode) + 1).ToString(), ConfigInfo.ColumnType), sellPrice, "");// 托盘价格
            }

            return result;
        }

        /// <summary>
        /// 商品上架补货
        /// </summary>
        /// <param name="operType"></param>
        /// <param name="putType"></param>
        /// <param name="paCode"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool PutGoods(string vendBoxCode,string operType, string putType, string paCode, string value,out string mcdCode)
        {
            bool result = false;
            mcdCode = string.Empty;

            result = AsileOper.PutGoods(vendBoxCode,operType, putType, paCode, value,out mcdCode);
            if (result)
            {
                switch (operType)
                {
                    case "1":// 单货道设置
                        m_GateSocket.SetAsileStock("1", paCode, value, "");
                        break;
                    case "2":// 单托盘设置
                        for (int i = 0; i < AsileOper.AsileList.Count; i++)
                        {
                            if ((AsileOper.AsileList[i].TrayIndex == Convert.ToInt32(paCode)) &&
                                (AsileOper.AsileList[i].VendBoxCode == vendBoxCode))
                            {
                                m_GateSocket.SetAsileStock("1", AsileOper.AsileList[i].PaCode, AsileOper.AsileList[i].SpringNum.ToString(), "");
                            }
                        }
                        break;
                }
            }

            return result;
        }

        /// <summary>
        /// 更新货道商品生产批次
        /// </summary>
        /// <param name="paCode"></param>
        /// <param name="mcdInfo"></param>
        /// <returns></returns>
        public bool UpdateAsileBatch(string paCode, string piCi,string productDate,string maxValidDate,out string mcdCode)
        {
            mcdCode = string.Empty;
            bool result = AsileOper.UpdateAsileBatch(paCode, piCi,productDate,maxValidDate,out mcdCode);

            ////if (result)
            ////{
            ////    // 向网关汇报信息
            ////    m_GateSocket.SetAsileGoods("1", paCode, mcdInfo.McdCode, "");
            ////}

            return result;
        }

        /// <summary>
        /// 更新货道商品
        /// </summary>
        /// <param name="paCode"></param>
        /// <param name="mcdInfo"></param>
        /// <returns></returns>
        public bool UpdateAsileGoods(string paCode, GoodsModel mcdInfo)
        {
            bool result = AsileOper.UpdateAsileGoods(paCode, mcdInfo);

            if (result)
            {
                // 重新读取商品类别信息
                if (ConfigInfo.GoodsShowModel == BusinessEnum.GoodsShowModelType.GoodsType)
                {
                    GoodsOper.LoadGoodsTypeList();
                }

                // 向网关汇报信息
                m_GateSocket.SetAsileGoods("1", paCode, mcdInfo.McdCode, "");
            }

            return result;
        }

        /// <summary>
        /// 移除货道商品
        /// </summary>
        /// <param name="paCode">货道外部编号</param>
        /// <returns>结果</returns>
        public bool RemoveAsileGoods(string paCode)
        {
            bool result = AsileOper.RemoveAsileGoods(paCode);

            if (result)
            {
                // 重新读取商品类别信息
                if (ConfigInfo.GoodsShowModel == BusinessEnum.GoodsShowModelType.GoodsType)
                {
                    GoodsOper.LoadGoodsTypeList();
                }

                // 向网关汇报信息
                m_GateSocket.SetAsileGoods("1", paCode, "0", "");
            }

            return result;
        }

        /// <summary>
        /// 移除所有货道上的商品
        /// </summary>
        /// <returns></returns>
        public bool ClearAllAsileGoods()
        {
            bool result = AsileOper.ClearAllAsileGoods();

            return result;
        }

        #endregion

        #region 销售数据库操作

        /// <summary>
        /// 恢复出厂数据
        /// </summary>
        /// <returns></returns>
        public bool InitFactoryData()
        {
            // 恢复出厂数据，主要恢复一下数据
            // 1、清空销售数据
            // 2、清空待发数据
            // 3、恢复交易号

            bool result = false;

            try
            {
                #region 为了防止误恢复出厂数据，先进行备份

                result = FunHelper.BackUpDbFile("NetDataInfo");
                if (!result)
                {
                    return false;
                }

                #endregion

                #region 恢复交易号为初始值

                result = UpdateSysCfgValue("BusinessId", "0");
                if (!result)
                {
                    return false;
                }

                m_BusId = 0;

                #endregion

                #region 清空网络待发数据

                int intErrCode = m_GateSocket.ClearNetData();
                if (intErrCode != 0)
                {
                    return false;
                }

                #endregion

                #region 清空销售统计数据

                result = ClearStatData();

                #endregion

                result = true;
            }
            catch
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// 获取总销售统计数据
        /// </summary>
        /// <param name="totalSaleNum"></param>
        /// <param name="totalStatMoney"></param>
        /// <returns></returns>
        public bool QueryStatTotalData(out string totalSaleNum,out string totalStatMoney)
        {
            bool result = false;
            totalSaleNum = "0";
            totalStatMoney = "0";

            try
            {
                string strSql = string.Empty;//

                DbOper dbOper = new DbOper();
                dbOper.DbFileName = _m_DbFileName;
                dbOper.ConnType = ConnectType.CloseConn;

                strSql = "select TotalSaleNum,TotalSaleMoney from T_SALE_STAT";
                DataSet dataSet = dbOper.dataSet(strSql);
                if (dataSet.Tables.Count > 0)
                {
                    if (dataSet.Tables[0].Rows.Count > 0)
                    {
                        totalSaleNum = dataSet.Tables[0].Rows[0]["TotalSaleNum"].ToString();
                        totalStatMoney = dataSet.Tables[0].Rows[0]["TotalSaleMoney"].ToString();
                    }
                }
                dbOper.closeConnection();

                result = true;
            }
            catch
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 清除销售数据
        /// </summary>
        /// <returns></returns>
        private bool ClearStatData()
        {
            bool result = false;

            try
            {
                string strSql = string.Empty;

                DbOper dbOper = new DbOper();
                dbOper.DbFileName = _m_DbFileName;
                dbOper.ConnType = ConnectType.KeepConn;

                // 清除销售统计总数据
                strSql = "update T_SALE_STAT set TotalSaleNum = 0,TotalSaleMoney = 0";
                result = dbOper.excuteSql(strSql);
                if (result)
                {
                    // 清除各货道销售数据
                    strSql = "update T_VM_PAINFO set SaleNum = 0,SaleMoney = 0";
                    result = dbOper.excuteSql(strSql);
                    if (result)
                    {
                        // 更改内存中的各货道销售数据
                        for (int i = 0; i < AsileOper.AsileList.Count; i++)
                        {
                            AsileOper.AsileList[i].SaleNum = 0;
                            AsileOper.AsileList[i].SaleMoney = 0;
                        }
                    }
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
        /// 保存货币数据
        /// </summary>
        /// <param name="moneyType">货币类型 0：硬币 1：纸币</param>
        /// <param name="payment">操作类型 0：收币 1：找零 2：吞币</param>
        /// <param name="amount">面值</param>
        /// <param name="num">数量</param>
        /// <returns>结果 True：成功 False：失败</returns>
        public bool AddMoneyData(string moneyType, string payment, int amount, int num)
        {
            bool result = false;

            try
            {
                string strTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string strDate = DateTime.Now.ToString("yyyyMMdd");

                string strSql = string.Empty;

                DbOper dbOper = new DbOper();
                dbOper.DbFileName = _m_DbFileName;
                dbOper.ConnType = ConnectType.KeepConn;

                if (strDate != m_TodayMoneyDate)
                {
                    // 如果当前日期进行了切换，则首先清除当天的相关金额数据
                    strSql = "update T_MONEY_STAT set [TodayDate] = '" + strDate + @"',[TodayCashMoney] = 0,[TodayCashNum] = 0,
                            [TodayCoinRecMoney] = 0,[TodayCoinRecNum] = 0," +
                        "[TodayCoinChangeMoney] = 0,[TodayCoinChangeNum] = 0";
                    result = dbOper.excuteSql(strSql);
                }

                strSql = "update T_MONEY_STAT set ";

                if (m_BeginMoneyDate.Length == 0)
                {
                    strSql = strSql + "[BeginDate] = '" + strTime + "',";
                }
                if (m_CycleMoneyDate.Length == 0)
                {
                    strSql = strSql + "[CycleDate] = '" + strTime + "',";
                }

                switch (moneyType)
                {
                    case "0":// 硬币
                        if (payment == "0")
                        {
                            // 收币
                            strSql += @" [TotalCoinRecMoney] = [TotalCoinRecMoney] + " + amount + @",[TotalCoinRecNum] = [TotalCoinRecNum] + 1,
                                        [CycleCoinRecMoney] = [CycleCoinRecMoney] + " + amount + @",[CycleCoinRecNum] = [CycleCoinRecNum] + 1,
                                        [TodayCoinRecMoney] = [TodayCoinRecMoney] + " + amount + ",[TodayCoinRecNum] = [TodayCoinRecNum] + 1";
                        }
                        if (payment == "1")
                        {
                            // 找零
                            strSql += @" [TotalCoinChangeMoney] = [TotalCoinChangeMoney] + " + amount + @",[TotalCoinChangeNum] = [TotalCoinChangeNum] + 1,
                                        [CycleCoinChangeMoney] = [CycleCoinChangeMoney] + " + amount + @",[CycleCoinChangeNum] = [CycleCoinChangeNum] + 1,
                                        [TodayCoinChangeMoney] = [TodayCoinChangeMoney] + " + amount + ",[TodayCoinChangeNum] = [TodayCoinChangeNum] + 1";
                        }
                        break;

                    case "1":// 纸币
                        strSql += @" [TotalCashMoney] = [TotalCashMoney] + " + amount + @",[TotalCashNum] = [TotalCashNum] + 1,
                                    [CycleCashMoney] = [CycleCashMoney] + " + amount + @",[CycleCashNum] = [CycleCashNum] + 1,
                                    [TodayCashMoney] = [TodayCashMoney] + " + amount + ",[TodayCashNum] = [TodayCashNum] + 1";
                        break;
                }

                result = dbOper.excuteSql(strSql);

                dbOper.closeConnection();

                if (result)
                {
                    if (m_BeginMoneyDate.Length == 0)
                    {
                        m_BeginMoneyDate = strTime;
                    }
                    if (m_CycleMoneyDate.Length == 0)
                    {
                        m_CycleMoneyDate = strTime;
                    }

                    m_TodayMoneyDate = strDate;

                    // 保存待发数据
                }
            }
            catch
            {
            }

            return result;
        }

        /// <summary>
        /// 保存出货数据
        /// </summary>
        /// <param name="vendBoxCode">货柜编号</param>
        /// <param name="paCode">货道外部编号</param>
        /// <param name="sellPrice">出货价格</param>
        /// <param name="sellType">出货类型</param>
        /// <returns>结果 False：失败 True：成功</returns>
        public bool AddSellData(int vendBoxCode, string paCode, string sellPrice, string sellType,int paIndex)
        {
            bool result = false;

            try
            {
                int intSellPrice = Convert.ToInt32(sellPrice);

                string strTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string strDate = DateTime.Now.ToString("yyyyMMdd");

                string strSql = string.Empty;//

                DbOper dbOper = new DbOper();
                dbOper.DbFileName = _m_DbFileName;
                dbOper.ConnType = ConnectType.KeepConn;

                #region 记录销售统计数据

                strSql = "update T_SALE_STAT set [TotalSaleMoney] = [TotalSaleMoney] + " + intSellPrice +
                        ",[TotalSaleNum] = [TotalSaleNum] + 1";

                ////strSql = "update T_SALE_STAT set [TotalSaleMoney] = [TotalSaleMoney] + " + intSellPrice +
                ////        ",[TotalSaleNum] = [TotalSaleNum] + 1,[CycleSaleMoney] = [CycleSaleMoney] +" + intSellPrice +
                ////        ",[CycleSaleNum] = [CycleSaleNum] + 1";

                ////if (strDate != m_TodaySaleDate)
                ////{
                ////    // 如果当前日期进行了切换
                ////    strSql = strSql + ",[TodayDate] = '" + strDate + "',[TodaySaleMoney] = " + intSellPrice +
                ////        ",[TodaySaleNum] = 1";
                ////}
                ////else
                ////{
                ////    strSql = strSql + ",[TodaySaleMoney] = [TodaySaleMoney] + " + intSellPrice +
                ////            ",[TodaySaleNum] = [TodaySaleNum] + 1";
                ////}

                ////if (m_BeginSaleDate.Length == 0)
                ////{
                ////    strSql = strSql + ",[BeginDate] = '" + strTime + "'";
                ////}
                ////if (m_CycleSaleDate.Length == 0)
                ////{
                ////    strSql = strSql + ",[CycleDate] = '" + strTime + "'";
                ////}

                result = dbOper.excuteSql(strSql);

                #endregion

                if (result)
                {
                    // 保存该货道的商品余量库存数据及货道销售数据
                    strSql = @"update T_VM_PAINFO set [SurNum] = [SurNum] - 1,[SaleNum] = [SaleNum] + 1,
                            [SaleMoney] = [SaleMoney] + " + intSellPrice +
                            " where [paId]='" + AsileOper.AsileList[paIndex].PaId + "' and vendboxcode = '" + vendBoxCode.ToString() + "'";
                    result = dbOper.excuteSql(strSql);
                    if (result)
                    {
                        // 减少该货道的内存商品余量，记录该货道销售数据
                        int intPaIndex = 0;
                        int intErrCode = 0;// AsileOper.GetPaIndex(paCode, out intPaIndex);
                        if (intErrCode == 0)
                        {
                            AsileOper.AsileList[paIndex].SurNum -= 1;
                            AsileOper.AsileList[paIndex].SaleNum++;
                            AsileOper.AsileList[paIndex].SaleMoney = AsileOper.AsileList[paIndex].SaleMoney +
                                intSellPrice;
                        }
                    }
                }

                dbOper.closeConnection();

                if (result)
                {
                    if (m_BeginSaleDate.Length == 0)
                    {
                        m_BeginSaleDate = strTime;
                    }
                    if (m_CycleSaleDate.Length == 0)
                    {
                        m_CycleSaleDate = strTime;
                    }

                    m_TodaySaleDate = strDate;

                    // 保存待发数据
                }
            }
            catch
            {
            }

            return result;
        }

        #endregion

        #region 管理员相关操作

        /// <summary>
        /// 验证管理员密码
        /// </summary>
        /// <param name="pwd">密码</param>
        /// <returns>结果</returns>
        public bool CheckUserLogin(string pwd)
        {
            bool result = false;

            string strAdminPwd1 = SysCfgOper.GetSysCfgValue("AdminPwd1");
            string strSuperPwd = SysCfgOper.GetSysCfgValue("SuperPwd");

            if ((pwd != strAdminPwd1) && (pwd != strSuperPwd))
            {
                // 密码验证失败
                result = false;
            }
            else
            {
                // 密码验证成功
                if (pwd == strAdminPwd1)
                {
                    // 一级管理员
                    m_UserType = BusinessEnum.UserType.NormalUser;
                }
                if (pwd == strSuperPwd)
                {
                    // 厂商管理员
                    m_UserType = BusinessEnum.UserType.SystemUser;
                }

                result = true;
            }

            return result;
        }

        #endregion

        #region 公有业务函数

        #region 支付方式处理

        public bool CheckPayEnviro()
        {
            bool result = true;

            try
            {
                #region 检测现金支付环境

                CheckPayEnviro_Cash();

                #endregion

                if (m_TotalPayMoney <= 0)
                {
                    #region 检测储值卡支付环境

                    CheckPayEnviro_ICCard();

                    #endregion

                    #region 检测非储值卡支付环境

                    CheckPayEnviro_NoFeeCard();

                    #endregion

                    #region 检测二维码支付环境

                    CheckPayEnviro_QRCode();

                    #endregion

                    #region 检测条形码相关的支付方式环境

                    #region 检测支付宝支付环境

                    CheckPayEnviro_AliPayCode();

                    #endregion

                    #region 检测微信刷卡支付环境

                    CheckPayEnviro_WeChatCode();

                    #endregion

                    #region 检测翼支付支付环境

                    CheckPayEnviro_BestPayCode();

                    #endregion

                    #region 检测志愿者兑换支付环境

                    CheckPayEnviro_Volunteer();

                    #endregion

                    if ((PaymentOper.PaymentList.PayShow_AliPay_Code == BusinessEnum.PayShowStatus.Normal) ||
                        (PaymentOper.PaymentList.PayShow_WeChatCode == BusinessEnum.PayShowStatus.Normal) ||
                        (PaymentOper.PaymentList.PayShow_BestPay_Code == BusinessEnum.PayShowStatus.Normal) ||
                        (PaymentOper.PaymentList.PayShow_Volunteer_Code == BusinessEnum.PayShowStatus.Normal))
                    {
                        // 检测条形码设备
                        bool checkResult = QueryBarCodeScanStatus();
                        if (checkResult)
                        {
                            // 设备正常
                            // 开启条形码扫描
                            BeginBarCodeScan();
                        }
                        else
                        {
                            // 设备不正常
                            PaymentOper.PaymentList.PayShow_AliPay_Code = BusinessEnum.PayShowStatus.Pause;
                            PaymentOper.PaymentList.PayShow_WeChatCode = BusinessEnum.PayShowStatus.Pause;
                            PaymentOper.PaymentList.PayShow_BestPay_Code = BusinessEnum.PayShowStatus.Pause;
                            PaymentOper.PaymentList.PayShow_Volunteer_Code = BusinessEnum.PayShowStatus.Pause;
                        }
                    }

                    #endregion

                    #region 检测银联支付环境

                    #endregion
                }

                #region 记录各支付方式的状态

                AddBusLog("Payment_Cash  " + PaymentOper.PaymentList.PayShow_Cash.ToString());
                AddBusLog("Payment_IC  " + PaymentOper.PaymentList.PayShow_IC.ToString());
                AddBusLog("Payment_NoFeeCard  " + PaymentOper.PaymentList.PayShow_NoFeeCard.ToString());
                AddBusLog("Payment_QRCode  " + PaymentOper.PaymentList.PayShow_QRCode.ToString());
                AddBusLog("Payment_AliPayCode  " + PaymentOper.PaymentList.PayShow_AliPay_Code.ToString());
                AddBusLog("Payment_WeChatCode  " + PaymentOper.PaymentList.PayShow_WeChatCode.ToString());
                AddBusLog("Payment_BestPayCode  " + PaymentOper.PaymentList.PayShow_BestPay_Code.ToString());
                AddBusLog("Payment_Volunteer  " + PaymentOper.PaymentList.PayShow_Volunteer_Code.ToString());

                #endregion

                if ((PaymentOper.PaymentList.PayShow_Cash == BusinessEnum.PayShowStatus.Pause) &&
                    (PaymentOper.PaymentList.PayShow_IC == BusinessEnum.PayShowStatus.Pause) &&
                    (PaymentOper.PaymentList.PayShow_NoFeeCard == BusinessEnum.PayShowStatus.Pause) &&
                    (PaymentOper.PaymentList.PayShow_QRCode == BusinessEnum.PayShowStatus.Pause) &&
                    (PaymentOper.PaymentList.PayShow_AliPay_Code == BusinessEnum.PayShowStatus.Pause) &&
                    (PaymentOper.PaymentList.PayShow_BestPay_Code == BusinessEnum.PayShowStatus.Pause) &&
                    (PaymentOper.PaymentList.PayShow_Volunteer_Code == BusinessEnum.PayShowStatus.Pause))
                {
                    // 支付方式都不能使用
                    result = false;
                }
                else
                {
                    result = true;
                }

                return result;
            }
            catch(Exception ex)
            {
                AddBusLog("CheckPayEnviro  " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 检测现金支付方式环境—切换到现金支付方式时使用
        /// </summary>
        private void CheckPayEnviro_Cash()
        {
            string strCashStatus = DeviceInfo.CashStatus.Status;
            string strCoinStatus = DeviceInfo.CoinStatus.Status;

            string strMoneyRecType = string.Empty;// DeviceInfo.MoneyRecType;

            BusinessEnum.ControlSwitch strCashControlSwitch = PaymentOper.PaymentList.Cash.ControlSwitch;
            ////BusinessEnum.ControlSwitch strCoinControlSwitch = PaymentOper.PaymentList.Cash.ControlSwitch;

            int intErrCode = 0;

            string strLogType = "CheckPayEnviro_Cash";

            try
            {
                // 如果现金支付方式不启用
                if (!PaymentOper.CheckPaymentControl(BusinessEnum.PayMent.Cash))
                {
                    PaymentOper.PaymentList.PayShow_Cash = BusinessEnum.PayShowStatus.Pause; // 不支持现金支付
                    ControlCashCoinPayment();
                    return;
                }
                else
                {
                    intErrCode = CheckPayMentStatus(false);
                    bool blnIsNormal = false;
                    if (intErrCode == 0)
                    {
                        if ((DeviceInfo.CashStatus.Status != "00") ||
                            (DeviceInfo.CoinStatus.Status != "00"))
                        {
                            blnIsNormal = true;
                        }
                    }

                    if (!blnIsNormal)
                    {
                        PaymentOper.PaymentList.PayShow_Cash = BusinessEnum.PayShowStatus.Pause; // 不支持现金支付
                    }
                    else
                    {
                        if (DeviceInfo.CoinStatus.Status == "02")
                        {
                            intErrCode = ControlCoin("1",true);
                        }

                        Thread.Sleep(200);

                        // 查询当前零钱是否充足，如果零钱充足，则纸币器状态正常情况下，使能纸币器
                        if (DeviceInfo.CashStatus.Status == "02")
                        {
                            bool blnIsEnouthCoin = false;// 零钱是否充足 False：充足 True：不足
                            string strValue = string.Empty;
                            int intCode = QueryCoinMoney(out blnIsEnouthCoin, out strValue);
                            if ((intCode == 0) && (!blnIsEnouthCoin))
                            {
                                intErrCode = ControlCash("1",true);
                            }
                            else
                            {
                                if (DeviceInfo.CashEnableKind != BusinessEnum.EnableKind.Disable)
                                {
                                    ControlCash("0",true);
                                }
                            }
                        }

                        Thread.Sleep(200);
                        CheckPayMentStatus(false);

                        #region 如果硬币器状态正常或退币按钮被触发

                        strMoneyRecType = DeviceInfo.MoneyRecType;
                        strCashStatus = DeviceInfo.CashStatus.Status;
                        strCoinStatus = DeviceInfo.CoinStatus.Status;

                        bool blnIsCoin = false;// 能否收硬币 False：不能 True：可以
                        bool blnIsCash = false;// 能否接收纸币 False：不能 True：可以
                        if ((strCoinStatus == "02") || (strCoinStatus == "03"))
                        {
                            #region 硬币器工作状态正常

                            switch (strMoneyRecType)
                            {
                                case "00":// 纸硬币皆可接收
                                    // 检测零钱是否充足
                                    if (DeviceInfo.IsEnougthCoin)
                                    {
                                        // 零钱不足，不能接受纸币
                                        blnIsCoin = true;
                                    }
                                    else
                                    {
                                        blnIsCoin = true;
                                        if ((strCashStatus == "02"))
                                        {
                                            // 纸币器工作状态正常
                                            blnIsCash = true;
                                        }
                                    }
                                    break;

                                case "01":// 只接收硬币
                                    blnIsCoin = true;
                                    break;

                                case "02":// 只接收纸币
                                case "04":// 无法接收硬币或者已满
                                case "05":
                                    blnIsCoin = false;
                                    break;

                                default:
                                    break;
                            }
                            #endregion
                        }

                        #region 临时注释 2016-07-21 
                        ////////blnIsCash = true;
                        ////////blnIsCoin = true;
                        #endregion

                        if ((blnIsCash) && (blnIsCoin))
                        {
                            PaymentOper.PaymentList.PayShow_Cash = BusinessEnum.PayShowStatus.Cash_Coin;// 纸币、硬币
                        }
                        else if ((!blnIsCash) && (blnIsCoin))
                        {
                            PaymentOper.PaymentList.PayShow_Cash = BusinessEnum.PayShowStatus.Coin;// 只能硬币
                        }
                        else
                        {
                            PaymentOper.PaymentList.PayShow_Cash = BusinessEnum.PayShowStatus.Pause;// 不能现金支付
                        }

                        #endregion
                    }
                }

                #region 根据现金支付当前的可用方式来控制纸币器、硬币器

                ControlCashCoinPayment();

                #endregion

                AddBusLog(strLogType + "  " + PaymentOper.PaymentList.PayShow_Cash.ToString());
            }
            catch (Exception ex)
            {
                AddErrLog(strLogType, "99", ex.Message);
                PaymentOper.PaymentList.PayShow_Cash = BusinessEnum.PayShowStatus.Pause; // 不能现金支付
            }
        }

        /// <summary>
        /// 检测现金支付方式环境—使用现金支付方式时
        /// </summary>
        public void CheckPayEnviro_Cash_1()
        {
            string strCashStatus = DeviceInfo.CashStatus.Status;
            string strCoinStatus = DeviceInfo.CoinStatus.Status;

            string strMoneyRecType = DeviceInfo.MoneyRecType;

            BusinessEnum.ControlSwitch strCashControlSwitch = PaymentOper.PaymentList.Cash.ControlSwitch;
            BusinessEnum.ControlSwitch strCoinControlSwitch = PaymentOper.PaymentList.Cash.ControlSwitch;

            string strLogType = "CheckPayEnviro_Cash_1";

            try
            {
                if (!PaymentOper.CheckPaymentControl(BusinessEnum.PayMent.Cash))
                {
                    PaymentOper.PaymentList.PayShow_Cash = BusinessEnum.PayShowStatus.Pause; // 不支持现金支付
                    return;
                }

                #region 如果硬币器状态正常或退币按钮被触发

                bool blnIsCoin = false;// 能否收硬币 False：不能 True：可以
                bool blnIsCash = false;// 能否接收纸币 False：不能 True：可以
                if ((strCoinStatus == "02") || (strCoinStatus == "03"))
                {
                    #region 硬币器工作状态正常
                    switch (strMoneyRecType)
                    {
                        case "00":// 纸硬币皆可接收
                            // 检测零钱是否充足
                            if (DeviceInfo.IsEnougthCoin)
                            {
                                // 零钱不足，不能接受纸币
                                blnIsCoin = true;
                            }
                            else
                            {
                                blnIsCoin = true;
                                if ((strCashStatus == "02"))
                                {
                                    // 纸币器工作状态正常
                                    blnIsCash = true;
                                }
                            }
                            break;

                        case "01":// 只接收硬币
                            blnIsCoin = true;
                            break;

                        case "02":// 只接收纸币
                        case "04":// 无法接收硬币或者已满
                        case "05":
                            blnIsCoin = false;
                            break;

                        default:
                            break;
                    }
                    #endregion
                }

                if ((blnIsCoin) && (blnIsCash))
                {
                    PaymentOper.PaymentList.PayShow_Cash = BusinessEnum.PayShowStatus.Cash_Coin;
                }
                else
                {
                    if ((!blnIsCash) && (blnIsCoin))
                    {
                        PaymentOper.PaymentList.PayShow_Cash = BusinessEnum.PayShowStatus.Coin;// 只能硬币
                    }
                    else
                    {
                        PaymentOper.PaymentList.PayShow_Cash = BusinessEnum.PayShowStatus.Pause;// 不支持现金支付
                    }
                }

                #endregion

                #region 根据现金支付当前的可用方式来控制纸币器、硬币器

                ControlCashCoinPayment();

                #endregion

                AddBusLog(strLogType + "  " + PaymentOper.PaymentList.PayShow_Cash.ToString());
            }
            catch (Exception ex)
            {
                AddErrLog(strLogType, "99", ex.Message);
                PaymentOper.PaymentList.PayShow_Cash = BusinessEnum.PayShowStatus.Pause; // 不支持现金支付
            }
        }

        /// <summary>
        /// 检测储值卡支付方式环境
        /// </summary>
        public void CheckPayEnviro_ICCard()
        {
            if (!PaymentOper.CheckPaymentControl(BusinessEnum.PayMent.IcCard))
            {
                // 刷卡器不启用
                PaymentOper.PaymentList.PayShow_IC = BusinessEnum.PayShowStatus.Pause; ;// 储值卡支付暂停
                return;
            }
            string strLogType = "CheckPayEnviro_ICCard";

            try
            {
                bool blnResult = QueryIcStatus();

                if ((DeviceInfo.ICStatus.Status == "02") && (blnResult))
                {
                    // 刷卡器正常
                    PaymentOper.PaymentList.PayShow_IC = BusinessEnum.PayShowStatus.Normal; // 请刷卡购物
                }
                else
                {
                    PaymentOper.PaymentList.PayShow_IC = BusinessEnum.PayShowStatus.Pause; ;// 储值卡支付暂停
                }
            }
            catch (Exception ex)
            {
                AddErrLog(strLogType, "99", ex.Message);
                PaymentOper.PaymentList.PayShow_IC = BusinessEnum.PayShowStatus.Pause; // 储值卡支付暂停
            }
        }

        /// <summary>
        /// 检测非初始卡支付方式环境—磁条卡
        /// </summary>
        public void CheckPayEnviro_NoFeeCard()
        {
            if (!PaymentOper.CheckPaymentControl(BusinessEnum.PayMent.NoFeeCard))
            {
                // 非储值卡支付不启用
                PaymentOper.PaymentList.PayShow_NoFeeCard = BusinessEnum.PayShowStatus.Pause; ;// 非储值卡支付暂停
                return;
            }

            bool intErrCode = QueryNoFeeCardStatus();
            if (intErrCode)
            {
                // 设备正常
                PaymentOper.PaymentList.PayShow_NoFeeCard = BusinessEnum.PayShowStatus.Normal; // 请非储值卡购物
            }
            else
            {
                PaymentOper.PaymentList.PayShow_NoFeeCard = BusinessEnum.PayShowStatus.Pause; ;// 非储值卡支付暂停
            }
        }

        /// <summary>
        /// 检测非初始卡支付方式环境—二维码
        /// </summary>
        public void CheckPayEnviro_QRCode()
        {
            if (!PaymentOper.CheckPaymentControl(BusinessEnum.PayMent.QRCodeCard))
            {
                // 支付不启用
                PaymentOper.PaymentList.PayShow_QRCode = BusinessEnum.PayShowStatus.Pause; ;// 支付暂停
                return;
            }

            bool intErrCode = QueryNoFeeCardStatus();
            if (intErrCode)
            {
                // 设备正常
                PaymentOper.PaymentList.PayShow_QRCode = BusinessEnum.PayShowStatus.Normal; // 请非储值卡购物
            }
            else
            {
                PaymentOper.PaymentList.PayShow_QRCode = BusinessEnum.PayShowStatus.Pause; ;// 非储值卡支付暂停
            }
        }

        /// <summary>
        /// 检测条形设备支付方式环境—支付宝付款码
        /// </summary>
        public void CheckPayEnviro_AliPayCode()
        {
            if (!PaymentOper.CheckPaymentControl(BusinessEnum.PayMent.AliPay_Code))
            {
                // 支付宝付款码不启用
                PaymentOper.PaymentList.PayShow_AliPay_Code = BusinessEnum.PayShowStatus.Pause; ;// 支付暂停
                return;
            }

            PaymentOper.PaymentList.PayShow_AliPay_Code = BusinessEnum.PayShowStatus.Normal; // 可以购物
            ////bool intErrCode = QueryNoFeeCardStatus();
            ////if (intErrCode)
            ////{
            ////    // 设备正常
            ////    PaymentOper.PaymentList.PayShow_AliPay_Code = BusinessEnum.PayShowStatus.Normal; // 可以购物
            ////}
            ////else
            ////{
            ////    PaymentOper.PaymentList.PayShow_AliPay_Code = BusinessEnum.PayShowStatus.Pause; ;// 支付暂停
            ////}
        }

        /// <summary>
        /// 检测条形设备支付方式环境—微信刷卡
        /// </summary>
        public void CheckPayEnviro_WeChatCode()
        {
            if (!PaymentOper.CheckPaymentControl(BusinessEnum.PayMent.WeChatCode))
            {
                // 微信刷卡不启用
                PaymentOper.PaymentList.PayShow_WeChatCode = BusinessEnum.PayShowStatus.Pause; ;// 支付暂停
                return;
            }

            PaymentOper.PaymentList.PayShow_WeChatCode = BusinessEnum.PayShowStatus.Normal; // 可以购物
        }

        /// <summary>
        /// 检测条形设备支付方式环境—翼支付付款码
        /// </summary>
        public void CheckPayEnviro_BestPayCode()
        {
            if (!PaymentOper.CheckPaymentControl(BusinessEnum.PayMent.BestPay_Code))
            {
                // 翼支付付款码不启用
                PaymentOper.PaymentList.PayShow_BestPay_Code = BusinessEnum.PayShowStatus.Pause; ;// 支付暂停
                return;
            }

            PaymentOper.PaymentList.PayShow_BestPay_Code = BusinessEnum.PayShowStatus.Normal; // 可以购物
        }

        /// <summary>
        /// 检测条形设备支付方式环境—志愿者兑换
        /// </summary>
        public void CheckPayEnviro_Volunteer()
        {
            if (!PaymentOper.CheckPaymentControl(BusinessEnum.PayMent.Volunteer))
            {
                // 志愿者兑换不启用
                PaymentOper.PaymentList.PayShow_Volunteer_Code = BusinessEnum.PayShowStatus.Pause; ;// 支付暂停
                return;
            }

            PaymentOper.PaymentList.PayShow_Volunteer_Code = BusinessEnum.PayShowStatus.Normal; // 可以购物
        }

        /// <summary>
        /// 控制支付方式环境
        /// </summary>
        /// <param name="payMent">支付方式</param>
        /// <param name="enable">状态 False：停止支付方式工作 True：开启支付方式工作</param>
        public void ControlPayEnviro(BusinessEnum.PayMent payMent,bool enable,bool isTime)
        {
            int intErrCode = 0;

            if (!enable)
            {
                // 禁能纸币器/硬币器
                intErrCode = DisableCashPaymnet(isTime);
                Thread.Sleep(50);

                // 停止条形码扫描监控
                intErrCode = StopBarCodeScan();
                ////Thread.Sleep(50);

                // 关闭二代身份证
                intErrCode = IDCardOper.CloseIDCard();
            }
            else
            {
                // 使能纸币器、硬币器
                ControlCashCoinPayment();

                // 开启条形码扫描监控
                intErrCode = BeginBarCodeScan();
                Thread.Sleep(50);
            }
        }

        /// <summary>
        /// 关闭所有支付环境
        /// </summary>
        private void CloseAllPayEnviro()
        {
            // 禁能纸币器/硬币器
            int intErrCode = ControlCash("0",false);
            intErrCode = ControlCoin("0",false);
            Thread.Sleep(50);

            // 停止条形码扫描监控
            intErrCode = StopBarCodeScan();
        }

        /// <summary>
        /// 控制非现金支付方式环境
        /// </summary>
        /// <param name="enable">False：禁止非现金支付方式 True：开启非现金支付方式环境</param>
        private void ControlNoCashEnviro(bool enable)
        {
            if (enable)
            {
                // 开启非现金支付方式环境
            }
            else
            {
                // 禁止非现金支付方式环境
                StopBarCodeScan();// 停止条形码扫描
            }
        }

        #endregion

        /// <summary>
        /// 将金额从转后为string,即将金额加小数点
        /// </summary>
        /// <param name="money">要转换的金额</param>
        /// <returns>转换后的金额</returns>
        public string MoneyIntToString(string money)
        {
            return FunHelper.MoneyIntToString(money, ConfigInfo.DecimalNum, ConfigInfo.PointNum,
                ConfigInfo.MoneySymbol,ConfigInfo.IsShowMoneySymbol,true);
        }

        public string MoneyIntToString(string money,bool isSet)
        {
            return FunHelper.MoneyIntToString(money, ConfigInfo.DecimalNum, ConfigInfo.PointNum,
                ConfigInfo.MoneySymbol, ConfigInfo.IsShowMoneySymbol, false);
        }

        /// <summary>
        /// 在现金支付打开情况下，检测价格是否属于最小面值货币的倍数
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        public bool CheckPriceIsMultiple(string price)
        {
            bool result = true;
            try
            {
                if (PaymentOper.CheckPaymentControl(BusinessEnum.PayMent.Cash))
                {
                    double dblAsilePrice = Convert.ToDouble(price) * ConfigInfo.DecimalNum;
                    if (dblAsilePrice % ConfigInfo.MinMoneyValue != 0)
                    {
                        // 不能被最小货币面值整除
                        result = false;
                    }
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 转换托盘编码
        /// </summary>
        public string ChangeTray(string trayNum)
        {
            return FunHelper.ChangeTray(trayNum, ConfigInfo.ColumnType);
        }

        /// <summary>
        /// 清空吞币超时时间
        /// </summary>
        private void ClearTunMoneyTime()
        {
            m_LastTunMoneyTime = DateTime.Now;// 更新吞币开始计算时间
            AddBusLog("Clear TunMoneyTime");
        }

        /// <summary>
        /// 检测商品列表界面是否分页
        /// </summary>
        /// <returns>False：不分页 True：分页</returns>
        public bool CheckIsPage()
        {
            bool result = false;

            if (ConfigInfo.GoodsShowModel == BusinessEnum.GoodsShowModelType.GoodsToOnlyAsile)
            {
                // 如果商品和货道一一对应模式
                if (AsileOper.VendBoxList.Count > 1)
                {
                    // 有多个货柜
                    result = true;
                }
            }

            if (ConfigInfo.GoodsShowModel == BusinessEnum.GoodsShowModelType.GoodsToMultiAsile)
            {
                // 如果商品和货道不对应
                if (GoodsOper.GoodsShowPageCount > 1)
                {
                    // 有多页
                    result = true;
                }
            }

            return result;
        }

        #endregion

        #region 日志记录函数

        private void AddBusLog_Code(string logType, string code, string data)
        {
            LogHelper.AddBusLog_Code("[" + OperStep.ToString() + "]" + logType, code, data);
        }

        private void AddErrLog(string logType, string code, string errInfo)
        {
            LogHelper.AddErrLog("[" + OperStep.ToString() + "]" + logType, code, errInfo);
        }

        private void AddBusLog(string logInfo)
        {
            LogHelper.AddBusLog("[" + OperStep.ToString() + "]" + logInfo);
        }

        #endregion

        #region 磁盘空间业务函数



        #endregion

        #region 购物打印业务函数

        /// <summary>
        /// 打印机业务处理对象
        /// </summary>
        private PrintOper PrintHelper = new PrintOper();

        /// <summary>
        /// 打印信息类
        /// </summary>
        private PrintInfoModel PrintInfo = new PrintInfoModel();

        /// <summary>
        /// 打印购物单据
        /// </summary>
        /// <returns></returns>
        public int PrintConsumeInfo()
        {
            int intErrCode = 0;

            try
            {
                if (ConfigInfo.IsPrintConsumeBill == BusinessEnum.ControlSwitch.Stop)
                {
                    return intErrCode;
                }

                // 打开打印机
                intErrCode = PrintHelper.Initialize(Convert.ToInt32(ConfigInfo.PrintPort));
                if (intErrCode != 0)
                {
                    return intErrCode;
                }

                // 查询打印机当前状态
                intErrCode = PrintHelper.QueryPrintStatus();
                if (intErrCode != 0)
                {
                    return intErrCode;
                }

                // 打印内容
                string strPrintData = ConfigInfo.PrintTmepContent;
                if (PrintInfo != null)
                {
                    strPrintData = strPrintData.Replace("{VmCode}", ConfigInfo.VmId);
                    strPrintData = strPrintData.Replace("{SerNo}", PrintInfo.SerNo);
                    strPrintData = strPrintData.Replace("{GoodsName}", PrintInfo.GoodsName);
                    strPrintData = strPrintData.Replace("{GoodsPiCi}", PrintInfo.GoodsPiCi);
                    strPrintData = strPrintData.Replace("{Manufacturer}", PrintInfo.Manufacturer);
                    strPrintData = strPrintData.Replace("{BuyTime}", PrintInfo.BuyTime);
                    strPrintData = strPrintData.Replace("{GoodsCode}", PrintInfo.GoodsCode);
                    strPrintData = strPrintData.Replace("{GoodsPrice}", MoneyIntToString(PrintInfo.GoodsPrice));
                    strPrintData = strPrintData.Replace("{GoodsSpec}", PrintInfo.GoodsSpec);
                    strPrintData = strPrintData.Replace("{Money}", MoneyIntToString(PrintInfo.Money));
                    strPrintData = strPrintData.Replace("{BuyNum}", PrintInfo.BuyNum.ToString());
                    strPrintData = strPrintData.Replace("{TermCode}", PrintInfo.TermCode);
                    strPrintData = strPrintData.Replace("{PayMent}", PrintInfo.PayMent);
                    strPrintData = strPrintData.Replace("{CardNum}", PrintInfo.CardNum);
                    strPrintData = strPrintData.Replace("{ProductDate}", PrintInfo.ProductDate);
                    strPrintData = strPrintData.Replace("{MaxValidDate}", PrintInfo.MaxValidDate);
                }

                if (!string.IsNullOrEmpty(ConfigInfo.PrintTmepTitle))
                {
                    // 打印表头
                    intErrCode = PrintHelper.SetFontSizeAlignMent(0, "1");// 设置字体居中
                    intErrCode = PrintHelper.PrintData(ConfigInfo.PrintTmepTitle);// 打印表头
                    intErrCode = PrintHelper.PrintData("");
                }

                intErrCode = PrintHelper.SetFontSizeAlignMent(0, "0");// 设置字体居左
                intErrCode = PrintHelper.PrintData(strPrintData);

                intErrCode = PrintHelper.PrintSpaceLine(2);
                // 切纸
                intErrCode = PrintHelper.CutPaper("0");

                // 关闭打印机
                PrintHelper.ClosePrint();
            }
            catch
            {
                intErrCode = 99;
            }
            finally
            {

            }

            return intErrCode;
        }

        /// <summary>
        /// 查询打印机设备状态
        /// </summary>
        /// <returns></returns>
        public int QueryPrintStatus()
        {
            int intErrCode = 0;

            try
            {
                // 关闭打印机
                PrintHelper.ClosePrint();

                // 打开打印机
                intErrCode = PrintHelper.Initialize(Convert.ToInt32(ConfigInfo.PrintPort));
                if (intErrCode != 0)
                {
                    return intErrCode;
                }

                intErrCode = PrintHelper.QueryPrintStatus();
            }
            catch
            {
                intErrCode = 99;
            }

            return intErrCode;
        }

        /// <summary>
        /// 切纸
        /// </summary>
        /// <returns></returns>
        public int CutPrintPaper()
        {
            int intErrCode = 0;

            try
            {
                // 关闭打印机
                PrintHelper.ClosePrint();

                // 打开打印机
                intErrCode = PrintHelper.Initialize(Convert.ToInt32(ConfigInfo.PrintPort));
                if (intErrCode != 0)
                {
                    return intErrCode;
                }

                intErrCode = PrintHelper.CutPaper("0");
            }
            catch
            {
                intErrCode = 99;
            }

            return intErrCode;
        }

        #endregion

        #region 远程控制指令处理业务函数

        /// <summary>
        /// 获取远程控制指令队列数量
        /// </summary>
        /// <returns></returns>
        public int GetRemoteCount()
        {
            return RemoteControlOper.GetRemoteCount();
        }

        /// <summary>
        /// 获取远程控制指令队列值
        /// </summary>
        /// <returns></returns>
        public string GetRemoteData()
        {
            string strRemoteData =  RemoteControlOper.ReadRemoteData();
            if (strRemoteData.Length > 0)
            {
                string[] hexValue = strRemoteData.Split('*');
                if (hexValue.Length > 0)
                {
                    // *1234567890*000000*10*14692602784056157*127.0.0.1*1
                    string strControlType = hexValue[3];
                    string strControlSerNo = hexValue[4];
                    switch (strControlType)
                    {
                        case "10":// 远程广告更新
                            string strAdverNo = hexValue[6];
                            // 重新读取广告配置
                            AdvertOper.LoadAdvertList(strAdverNo);
                            AdvertOper.IsRefreshAdvert = true;
                            break;
                    }
                }
            }

            return strRemoteData;
        }

        public string FenXiRemoteData()
        {
            return "";
        }

        #endregion

        #region 远程更新业务函数

        /// <summary>
        /// 检测广告节目单是否需要更新
        /// </summary>
        public void UploadAdvertOper()
        {
            // 检查广告节目单是否需要更新 
            AdvertOper.AdvertCheckUpdateMon();
            if (ConfigInfo.UpdateAdvertListID != AdvertOper.UpdateAdvertListID)
            {
                SysCfgOper.UpdateSysCfg("UpdateAdvertListID", AdvertOper.UpdateAdvertListID);
                ConfigInfo.UpdateAdvertListID = AdvertOper.UpdateAdvertListID;
            }
        }

        /// <summary>
        /// 检查需要下载更新的广告文件
        /// </summary>
        public void QueryAdvUpload()
        {
            // 检查需要下载更新的广告文件
            AdvertOper.AdvertUploadMon();

            // 检查当前需要更新的广告节目单是否可以播放
            if (ConfigInfo.NowAdvertPlayID != ConfigInfo.UpdateAdvertListID)
            {
                bool result = AdvertOper.CheckUploadAdvIsPlay(ConfigInfo.UpdateAdvertListID);
                if (result)
                {
                    // 当前需更新的广告节目单可以播放
                    ConfigInfo.NowAdvertPlayID = ConfigInfo.UpdateAdvertListID;
                    SysCfgOper.UpdateSysCfg("NowAdvertPlayID", ConfigInfo.NowAdvertPlayID);

                    // 重新读取广告配置
                    AdvertOper.LoadAdvertList_New(ConfigInfo.NowAdvertPlayID);
                    AdvertOper.IsRefreshAdvert = true;
                }
            }
        }

        /// <summary>
        /// 检查商品是否需要更新
        /// </summary>
        public void UploadGoodsOper()
        {
            // 检查商品是否需要更新
            if ((ConfigInfo.GoodsUploadType == "1") || (ConfigInfo.GoodsUploadType == "2"))
            {
                string _goodsListInfo = string.Empty;
                bool result = AdvertOper.GoodsCheckUpdateMon(out _goodsListInfo);
                if (result)
                {
                    // 成功，解析商品数据
                    string _updateMcdList = string.Empty;
                    int intErrcode = GoodsOper.ImportGoodsList(_goodsListInfo, "1", "", out _updateMcdList);
                }
            }
        }

        /// <summary>
        /// 下载需要下载更新的商品图片文件
        /// </summary>
        public void QueryMcdPicDown()
        {
            GoodsOper.CheckMcdPicDown();
        }

        /// <summary>
        /// 在线更新系统配置参数
        /// </summary>
        /// <returns></returns>
        public bool UploadSysCfg_Net(out string _errCode)
        {
            bool result = false;

            _errCode = "9";

            string strLogType = "UploadSysCfg_Net";

            try
            {
                string strResponseData = string.Empty;

                result = AdvertOper.UploadVmConfig_Net(out strResponseData);

                if (result)
                {
                    #region 处理返回数据
                    JsonOper jsonOper = new JsonOper();
                    string strRetCode = jsonOper.GetJsonKeyValue(strResponseData, "ret").Trim();// 返回码
                    int intCfgCount = Convert.ToInt32(jsonOper.GetJsonKeyValue(strResponseData, "cfgcount").Trim());// 配置参数数量

                    _errCode = strRetCode;

                    if (strRetCode == "0")
                    {
                        #region 解析配置参数数据

                        if (intCfgCount > 0)
                        {
                            #region 获取参数配置
                            JsonData jdItems = null;
                            jdItems = JsonMapper.ToObject(strResponseData);

                            jdItems = jdItems["cfglist"];

                            string strCfgName = string.Empty;
                            string strCfgValue = string.Empty;

                            foreach (JsonData item in jdItems)
                            {
                                strCfgName = item["cfgname"].ToString();
                                strCfgValue = item["value"].ToString();
                                UpdateSysCfgValue(strCfgName, strCfgValue);
                            }

                            #endregion

                            #region 获取一卡通及会员卡支付方式名称

                            ////JsonData jdItems_Pay = null;
                            ////jdItems_Pay = JsonMapper.ToObject(strResponseData);

                            ////jdItems_Pay = jdItems_Pay["paylist"];
                            ////string strPayCode = string.Empty;

                            ////foreach (JsonData item in jdItems_Pay)
                            ////{
                            ////    PaymentOper.UpdatePaymentName(item["paycode"].ToString(), item["cn"].ToString(),
                            ////        item["en"].ToString(), item["rus"].ToString());
                            ////}
                            ////PaymentOper.LoadPaymentName();
                            #endregion
                        }

                        result = true;
                        #endregion
                    }
                    #endregion
                }

                AddBusLog_Code(strLogType, "0", strResponseData);
            }
            catch
            {
                result = false;
            }

            return result;
        }

        #region 在线更新

        /// <summary>
        /// 在线更新打印单据内容参数
        /// </summary>
        /// <param name="_errCode"></param>
        /// <returns></returns>
        public bool UploadSysCfg_Print(out string _errCode)
        {
            bool result = false;

            _errCode = "9";
            string strRetCode = string.Empty;

            try
            {
                string strLogType = "UploadSysCfg_Print";
                string methodName = "Down_VmConfig_Print";
                object[] args = new object[1];
                args[0] = ConfigInfo.VmId;// 机器出厂号

                string errMsg = string.Empty;
                string strUrl = SysCfgOper.GetSysCfgValue("VmTermApiUrl");

                WebServiceOper WebServiceHelper = new WebServiceOper();

                object objData = WebServiceHelper.InvokeWebService(strUrl, methodName, args, out result, out errMsg);

                ////object objData = InvokeWebService(m_ThirdWebUrl, methodName, args);
                if (objData == null)
                {
                    return false;
                }

                string strResponseData = objData.ToString();

                #region 处理返回数据
                JsonOper jsonOper = new JsonOper();
                strRetCode = jsonOper.GetJsonKeyValue(strResponseData, "ret").Trim();// 返回码

                _errCode = strRetCode;

                if (strRetCode == "0")
                {
                    #region 解析配置参数数据

                    string strPrintTitle = jsonOper.GetJsonKeyValue(strResponseData, "title").Trim();
                    string strPrintContent = jsonOper.GetJsonKeyValue(strResponseData, "content");
                    // 转码换行符等
                    strPrintContent = strPrintContent.Replace("\\r", "\r");
                    strPrintContent = strPrintContent.Replace("\\n", "\n");

                    UpdateSysCfgValue("PrintTmepTitle", strPrintTitle);
                    UpdateSysCfgValue("PrintTmepContent", strPrintContent);

                    result = true;
                    #endregion
                }
                #endregion

                AddBusLog_Code(strLogType, "0", strResponseData);
            }
            catch
            {
                result = false;
            }

            return result;
        }

        #endregion

        #endregion
    }
}
