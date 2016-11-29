#region [ KIMMA Co.,Ltd. Copyright (C) 2014 ]
//-------------------------------------------------------------------------------------
// 开发单位：湖南金码智能设备制造有限公司
// 业务模块：iVend终端软件业务处理类
// 业务功能：货柜数据模板
// 创建标识：2015-05-19		谷霖
//-------------------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Business.Enum;

namespace Business.Model
{
    public class VendBoxCodeModel
    {
        /// <summary>
        /// 析构
        /// </summary>
        public VendBoxCodeModel()
        {
            VendBoxCode = "1";
            VendBoxStatus = true;
            SellGoodsType = BusinessEnum.SellGoodsType.Spring;
            TmpControlModel = BusinessEnum.TmpControlModel.Refrigeration;
            TargetTmp = "6";
            OutTmpWarnValue = "18";
            ShippPort = "3";
            IsControlCircle = "0";
            UpDownSellModel = "1";
            UpDownDelayTimeNums = "150";
            UpDownSendGoodsTimes = "30";
            UpDownIsQueryElectStatus = BusinessEnum.ControlSwitch.Run;
            LifterStatus = "01";
            DriveMoardStatus = "02";
            DropStatus = "FF";
            TmpStatus = "02";// 默认未连接
            TmpValue = "0";
            DoorStatus = "02";// 默认故障
            LastQueryTempTime = DateTime.Now;
            RefControl = new DeviceControlModel();
        }

        /// <summary>
        /// 货柜编号
        /// </summary>
        public string VendBoxCode { get; set; }

        /// <summary>
        /// 是否存在该机柜编号 False：不存在 True：存在
        /// </summary>
        public bool VendBoxStatus { get; set; }

        /// <summary>
        /// 出货方式 0：弹簧方式 1：复杂型升降方式 2：简易型升降方式
        /// </summary>
        public BusinessEnum.SellGoodsType SellGoodsType { get; set; }

        /// <summary>
        /// 温控工作模式 0：制冷 1：加热
        /// </summary>
        public BusinessEnum.TmpControlModel TmpControlModel { get; set; }

        /// <summary>
        /// 目标温度
        /// </summary>
        public string TargetTmp { get; set; }

        /// <summary>
        /// 报警温度
        /// </summary>
        public string OutTmpWarnValue { get; set; }

        /// <summary>
        /// 驱动板通信串口号
        /// </summary>
        public string ShippPort { get; set; }

        /// <summary>
        /// 是否具有控制回路 0：无 1：有
        /// </summary>
        public string IsControlCircle { get; set; }

        /// <summary>
        /// 升降机出货形式参数 0：直接升降 1：按参数升降
        /// </summary>
        public string UpDownSellModel { get; set; }

        /// <summary>
        /// 升降机出货延时 以5毫秒为单位，最大500ms
        /// </summary>
        public string UpDownDelayTimeNums { get; set; }

        /// <summary>
        /// 云台至出货口后延时时长，以毫秒为单位，最大延时255毫秒，默认30毫秒，0表示不延时
        /// </summary>
        public string UpDownSendGoodsTimes { get; set; }

        /// <summary>
        /// 是否检测升降机光电管状态 0：不检测 1：检测（默认）
        /// </summary>
        public BusinessEnum.ControlSwitch UpDownIsQueryElectStatus { get; set; }

        #region 升降机设备状态

        /// <summary>
        /// 升降机设备状态，
        /// 00：正常 
        /// 01：升降机位置不在初始位
        /// 02：纵向电机卡塞
        /// 03：接货台不在初始位
        /// 04：横向电机卡塞
        /// 05：小门电机卡塞
        /// 06：接货台有货
        /// 07：接货台电机卡塞
        /// 08：取货口有货
        /// 09：其它故障 
        /// 10：光电管故障
        /// 
        /// /// 01：故障
        /// 02：正常
        /// 03：升降机位置不在初始位
        /// 04：纵向电机卡塞
        /// 05：接货台不在初始位
        /// 06：横向电机卡塞
        /// 07：小门电机卡塞
        /// 08：接货台有货
        /// 09：接货台电机卡塞
        /// 10：取货口有货
        /// 11：光电管故障
        /// </summary>
        public string LifterStatus { get; set; }

        #endregion

        /// <summary>
        /// 压缩机控制模式
        /// </summary>
        public DeviceControlModel RefControl = new DeviceControlModel();

        /// <summary>
        /// 照明系统控制模式
        /// </summary>
        public DeviceControlModel LightControl = new DeviceControlModel();

        /// <summary>
        /// 风机控制模式
        /// </summary>
        public DeviceControlModel DraughtFanControl = new DeviceControlModel();

        /// <summary>
        /// 广告灯控制模式
        /// </summary>
        public DeviceControlModel AdvertLightControl = new DeviceControlModel();

        /// <summary>
        /// 除雾设备控制模式
        /// </summary>
        public DeviceControlModel DemisterControl = new DeviceControlModel();

        /// <summary>
        /// 紫外灯控制模式
        /// </summary>
        public DeviceControlModel UltravioletLampControl = new DeviceControlModel();

        /// <summary>
        /// 驱动板状态，默认正常（02）
        /// </summary>
        public string DriveMoardStatus { get; set; }

        /// <summary>
        /// 掉货检测状态，默认没有安装（FF）
        /// </summary>
        public string DropStatus { get; set; }

        /// <summary>
        /// 温度传感器状态，默认正常（00）
        /// </summary>
        public string TmpStatus { get; set; }

        /// <summary>
        /// 当前温度值，默认0
        /// </summary>
        public string TmpValue { get; set; }

        /// <summary>
        /// 门控状态，默认关门（00） 01：门开 02：故障
        /// </summary>
        public string DoorStatus { get; set; }

        /// <summary>
        /// 上次查询温度的时间
        /// </summary>
        public DateTime LastQueryTempTime { get; set; }
    }
}
