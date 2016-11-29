#region [ KIMMA Co.,Ltd. Copyright (C) 2014 ]
//-------------------------------------------------------------------------------------
// 开发单位：湖南金码智能设备制造有限公司
// 业务模块：iVend终端软件业务处理类
// 业务功能：设备数据模板
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
    public class DeviceInfoModel
    {
        /// <summary>
        /// 析构
        /// </summary>
        public DeviceInfoModel()
        {
            KmbSoftVer = string.Empty;
            ////CashControlSwitch = BusinessEnum.ControlSwitch.Stop;
            ////CoinControlSwitch = BusinessEnum.ControlSwitch.Stop;
            ////IcControlSwitch = BusinessEnum.ControlSwitch.Stop;
            KmbControlSwitch = BusinessEnum.ControlSwitch.Stop;
            ////DriveConnType = BusinessEnum.ControlSwitch.Run;
            MoneyRecType = "00";
            IsEnougthCoin = false;
            KmbConnectStatus = false;
            CashStatus = new DeviceStatusModel();
            CoinStatus = new DeviceStatusModel();
            CashEnable = BusinessEnum.EnableKind.Unknown;
            CoinEnable = BusinessEnum.EnableKind.Unknown;
            ICStatus = new DeviceStatusModel();
            ICSoftVer = string.Empty;
            NoFeeCardStatus = new DeviceStatusModel();
            NoFeeCardSoftVer = string.Empty;
            QrDeviceStatus = "01";
            QrSoftVer = string.Empty;
            AliPayWaveStatus = "01";
            AliPayWaveSoftVer = string.Empty;
            AliPayCodeStatus = "01";
            AliPayCodeSoftVer = string.Empty;
            UnionPayStatus = "01";
            UnionPaySoftVer = string.Empty;
            BarCodeDeviceStatus = new DeviceStatusModel();
            BarCodeSoftVer = string.Empty;
            ////DoorStatus = "02";// 默认故障
            ////DriveMoardStatus = "02";
            ////DropStatus = "FF";
            ////TmpStatus = "02";// 默认未连接
            ////TmpValue = "0";
            ////RefrStatus = "00";
            ////JacklightStatus = "00";
            ////AdvertlightStatus = "00";
            ////DraughtFanStatus = "00";
            ////DemisterStatus = "00";
            NetStatus = BusinessEnum.NetStatus.OffLine;
            NetNoSendNum = 0;
            ////LifterStatus = "01";
            CashEnableKind = BusinessEnum.EnableKind.Unknown;
            CoinEnableKind = BusinessEnum.EnableKind.Unknown;
            IsClearMoney = false;
            DiskSpaceEnougth = true;
            PrintStatus = new DeviceStatusModel();
        }

        /////// <summary>
        /////// 出货方式 0：弹簧方式（默认） 1：升降方式
        /////// </summary>
        ////public BusinessEnum.SellGoodsType SellGoodsType { get; set; }

        /// <summary>
        /// 控制主板通讯连接状态 False：连接失败 True：连接正常
        /// </summary>
        public bool KmbConnectStatus { get; set; }

        /////// <summary>
        /////// 驱动板是否连接到控制主板 0：关闭（不连接） 1：启用（连接）
        /////// </summary>
        ////public BusinessEnum.ControlSwitch DriveConnType { get; set; }

        /// <summary>
        /// 控制主板开关 0：关闭控制主板（不启用） 1：启用控制主板
        /// </summary>
        public BusinessEnum.ControlSwitch KmbControlSwitch { get; set; }

        /// <summary>
        /// 控制主板软件版本
        /// </summary>
        public string KmbSoftVer { get; set; }

        /////// <summary>
        /////// 纸币器开关 0：关闭纸币器 1：打开纸币器
        /////// </summary>
        ////public BusinessEnum.ControlSwitch CashControlSwitch { get; set; }

        /////// <summary>
        /////// 硬币器开关 0：关闭硬币器 1：打开硬币器
        /////// </summary>
        ////public BusinessEnum.ControlSwitch CoinControlSwitch { get; set; }

        /////// <summary>
        /////// 刷卡设备开关 0：关闭刷卡器 1：打开刷卡器
        /////// </summary>
        ////public BusinessEnum.ControlSwitch IcControlSwitch { get; set; }

        /// <summary>
        /// 可接收货币标识
        /// 0x00：纸硬币皆可 0x01：只接收硬币 0x02：只接收纸币 0x04：无法工作，都不接收 0x05：已满，暂不收钱
        /// </summary>
        public string MoneyRecType { get; set; }

        /// <summary>
        /// 零钱是否不足 False：充足 True：不足 
        /// </summary>
        public bool IsEnougthCoin { get; set; }

        /// <summary>
        /// 纸币器状态，正常（02）
        /// </summary>
        public DeviceStatusModel CashStatus { get; set; }

        /// <summary>
        /// 硬币器状态，默认正常（02）
        /// </summary>
        public DeviceStatusModel CoinStatus { get; set; }

        /// <summary>
        /// 纸币器使能状态
        /// </summary>
        public BusinessEnum.EnableKind CashEnable { get; set; }

        /// <summary>
        /// 硬币器使能状态
        /// </summary>
        public BusinessEnum.EnableKind CoinEnable { get; set; }

        /// <summary>
        /// 储值卡刷卡器状态，默认正常（02）
        /// </summary>
        public DeviceStatusModel ICStatus { get; set; }

        /// <summary>
        /// 储值卡组件版本
        /// </summary>
        public string ICSoftVer { get; set; }

        /// <summary>
        /// 非储值卡磁条卡设备状态，默认正常（02）
        /// </summary>
        public DeviceStatusModel NoFeeCardStatus { get; set; }

        /// <summary>
        /// 非储值卡磁条卡组件版本
        /// </summary>
        public string NoFeeCardSoftVer { get; set; }

        /// <summary>
        /// 二维码扫描设备状态，默认正常（02）
        /// </summary>
        public string QrDeviceStatus { get; set; }

        /// <summary>
        /// 二维码扫描组件版本
        /// </summary>
        public string QrSoftVer { get; set; }

        /// <summary>
        /// 支付宝声波支付设备状态，默认正常（02）
        /// </summary>
        public string AliPayWaveStatus { get; set; }

        /// <summary>
        /// 支付宝声波组件版本
        /// </summary>
        public string AliPayWaveSoftVer { get; set; }

        /// <summary>
        /// 支付宝扫码支付设备状态，默认正常（02）
        /// </summary>
        public string AliPayCodeStatus { get; set; }

        /// <summary>
        /// 支付宝扫码组件版本
        /// </summary>
        public string AliPayCodeSoftVer { get; set; }

        /// <summary>
        /// 银联设备状态，默认正常（02）
        /// </summary>
        public string UnionPayStatus { get; set; }

        /// <summary>
        /// 银联组件版本
        /// </summary>
        public string UnionPaySoftVer { get; set; }

        /// <summary>
        /// 条形码扫描设备状态，默认正常（02）
        /// </summary>
        public DeviceStatusModel BarCodeDeviceStatus { get; set; }

        /// <summary>
        /// 条形码扫描组件版本
        /// </summary>
        public string BarCodeSoftVer { get; set; }

        ///////// <summary>
        ///////// 门控状态，默认关门（00） 01：门开 02：故障
        ///////// </summary>
        //////public string DoorStatus { get; set; }

        /////// <summary>
        /////// 驱动板状态，默认正常（02）
        /////// </summary>
        ////public string DriveMoardStatus { get; set; }

        /////// <summary>
        /////// 掉货检测状态，默认没有安装（FF）
        /////// </summary>
        ////public string DropStatus { get; set; }

        /////// <summary>
        /////// 温度传感器状态，默认正常（00）
        /////// </summary>
        ////public string TmpStatus { get; set; }

        /////// <summary>
        /////// 当前温度值，默认0
        /////// </summary>
        ////public string TmpValue { get; set; }

        /////// <summary>
        /////// 压缩机（加热器）状态，默认00（关闭） FF：不存在控制回路
        /////// </summary>
        ////public string RefrStatus { get; set; }

        /////// <summary>
        /////// 照明设备状态，默认00（关闭）01：开启 02：不存在控制回路
        /////// </summary>
        ////public string JacklightStatus { get; set; }

        /////// <summary>
        /////// 广告灯状态，默认00（关闭）01：开启 02：不存在控制回路
        /////// </summary>
        ////public string AdvertlightStatus { get; set; }

        /////// <summary>
        /////// 风机状态，默认00（关闭）01：开启 02：不存在控制回路
        /////// </summary>
        ////public string DraughtFanStatus { get; set; }

        /////// <summary>
        /////// 除雾设备状态，默认00（关闭）01：开启 02：不存在控制回路
        /////// </summary>
        ////public string DemisterStatus { get; set; }

        /////// <summary>
        /////// 紫外灯状态，默认00（关闭） 01：开启
        /////// </summary>
        ////public string UltravioletLamp{ get; set;}

        /// <summary>
        /// 网络设备状态，默认00（离线），01：联机 FF：不存在网络设备或不存在网络功能
        /// </summary>
        public BusinessEnum.NetStatus NetStatus { get; set; }

        /// <summary>
        /// 网络待发数据数量
        /// </summary>
        public int NetNoSendNum { get; set; }

        //////#region 升降机设备状态

        ///////// <summary>
        ///////// 升降机设备状态，
        ///////// 00：正常 
        ///////// 01：升降机位置不在初始位
        ///////// 02：纵向电机卡塞
        ///////// 03：接货台不在初始位
        ///////// 04：横向电机卡塞
        ///////// 05：小门电机卡塞
        ///////// 06：接货台有货
        ///////// 07：接货台电机卡塞
        ///////// 08：取货口有货
        ///////// 09：其它故障 
        ///////// 10：光电管故障
        ///////// 
        ///////// /// 01：故障
        ///////// 02：正常
        ///////// 03：升降机位置不在初始位
        ///////// 04：纵向电机卡塞
        ///////// 05：接货台不在初始位
        ///////// 06：横向电机卡塞
        ///////// 07：小门电机卡塞
        ///////// 08：接货台有货
        ///////// 09：接货台电机卡塞
        ///////// 10：取货口有货
        ///////// 11：光电管故障
        ///////// </summary>
        //////public string LifterStatus { get; set; }

        //////#endregion

        /// <summary>
        /// 纸币器使能/禁能状态
        /// </summary>
        public BusinessEnum.EnableKind CashEnableKind { get; set; }

        /// <summary>
        /// 硬币器使能/禁能状态
        /// </summary>
        public BusinessEnum.EnableKind CoinEnableKind { get; set; }

        /// <summary>
        /// 是否需要清空界面上的投币金额 False：不需要 True：需要
        /// </summary>
        public bool IsClearMoney { get; set; }

        /// <summary>
        /// 网络通信组件名称
        /// </summary>
        public string NetSoftVer { get; set; }

        /// <summary>
        /// 磁盘空间是否充足 False：不充足 True：充足
        /// </summary>
        public bool DiskSpaceEnougth { get; set; }

        /// <summary>
        /// 打印机设备状态
        /// </summary>
        public DeviceStatusModel PrintStatus { get; set; }
    }
}
