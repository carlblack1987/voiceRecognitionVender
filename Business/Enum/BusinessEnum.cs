#region [ KIMMA Co.,Ltd. Copyright (C) 2014 ]
//-------------------------------------------------------------------------------------
// 开发单位：湖南金码智能设备制造有限公司
// 业务模块：iVend终端软件业务处理类
// 业务功能：枚举数据类
// 创建标识：2014-06-10		谷霖
//-------------------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Business.Enum
{
    public class BusinessEnum
    {
        #region 2015-01-22 移动添加
        /// <summary>
        /// 业务操作步骤枚举
        /// </summary>
        public enum OperStep
        {
            Loading,// 信息初始化
            InitErr,// 初始化失败
            Main,// 商品列表显示
            ChoiceGoods,// 选择商品
            ChoiceGoodsEnd,// 选择商品结束
            ReturnMain,// 返回商品列表显示
            Payment,// 等待支付
            SellGoods,// 出货
            ChangeCoin,// 退币
            SysCfg,// 系统设置
            AsileTest,// 货道测试
            ShowCardInfo,// 查询卡用户信息
            DeviceCfg,// 设备维护
            O2OServer,// O2O业务服务（包括：身份证领取、取货码、条形码取货等）
            Gift,// 礼品赠送
        }
        #endregion

        /// <summary>
        /// 硬件设备列表枚举
        /// </summary>
        public enum DeviceList
        {
            BillValidator,// 纸币器
            CoinChanger,// 硬币器
            IC,// 储值卡刷卡器
            DropSensor,// 掉货检测
            Door,// 门控
            DriverBoard,// 驱动板
            Lifter,// 升降机
            TemSensor,// 温度传感器,
            Light,// 照明灯
            DraughtFan,// 风机
            AdvertLight,// 广告灯
            Demister,// 除雾设备
            UltravioletLamp,// 紫外灯
            Refrigeration,// 制冷压缩机/加热器
            ComPuter,// 工控机及显示屏
            NoFeeCard,// 非储值卡磁条设备
            QRDevice,// 二维码扫描设备
            Sound,// 声音设备
            BarCodeScan,// 条形码扫描设备
        }

        /// <summary>
        /// 服务原因类型
        /// </summary>
        public enum ServerReason
        {
            Normal,// 正常
            Err_NoAsile,// 货道不存在
            Err_AsilePause,// 货道暂停销售
            Err_NoStock,// 库存不足
            Err_AsileStatus,// 货道故障
            Err_Cash,// 现金支付设备故障
            Err_IC,// 储值卡刷卡设备故障
            Err_UpDown_VertMotor,// 升降系统故障—升降机位置不在初始位或纵向电机卡塞
            Err_UpDown_HoriMotor,// 升降系统故障—接货台不在初始位或横向电机卡塞
            Err_UpDown_DoorMotor,// 升降系统故障—小门电机卡塞
            Err_UpDown_JieHuo,// 升降系统故障—接货台有货
            Err_UpDown_JieHuoMotor,// 升降系统故障—接货台电机卡塞
            Err_UpDown_Other,// 升降系统故障—其它故障
            Err_GuangDian,// 光电管故障
            Err_GoodsExist,// 取货仓有货
            Err_TmpLimit,// 温度超限
            Err_NoPayment,// 没有开通任何支付方式
            Err_NoFeeCard,// 非储值卡设备故障
            Err_QrCode,// 二维码扫描设备故障
            Err_DiskSpaceNoEnougth,// 磁盘空间不足
            Err_IDCard,// 二代身份证设备故障
            Err_Other,// 其它故障
        }

        /// <summary>
        /// 支付方式枚举
        /// </summary>
        public enum PayMent
        {
            All,// 所有支付方式
            No,// 无支付方式
            Cash,// 现金支付
            IcCard,// 刷卡支付（储值卡，离线消费）
            WeChatCode,// 微信扫码支付
            AliPay_Code,// 支付宝付款码支付
            BestPay_Code,// 翼支付付款码支付
            JDPay_Code,// 京东钱包付款码支付【付款码】
            BaiDuPay_Code,// 百度钱包付款码支付【付款码】
            QuickPass,// 银联闪付
            NoFeeCard,// 会员卡（非储值卡）
            QRCodeCard,// 虚拟会员卡（二维码会员）
            IDCard,// 身份证
            Volunteer,// 志愿者兑换
        }

        /// <summary>
        /// 支付方式的状态
        /// </summary>
        public enum PayShowStatus
        {
            Pause,// 暂停服务
            Normal,// 正常
            Cash_Coin,// 支持纸币、硬币
            Cash,// 支持纸币
            Coin,// 支持硬币
        }

        /// <summary>
        /// 语言类型
        /// </summary>
        public enum Language
        {
            Zh_CN,// 中文简体
            English,// 英文
            Russian,// 俄文
            French,// 法文
        }

        /// <summary>
        /// 出货方式
        /// </summary>
        public enum SellGoodsType
        {
            Spring,// 弹簧方式
            Lifter_Comp,// 复杂型升降机
            Lifter_Simple,// 简易型升降机
        }

        /// <summary>
        /// 设备开关
        /// </summary>
        public enum ControlSwitch
        {
            Stop,// 停止
            Run,// 开启
        }

        /// <summary>
        /// 设备运行状态，主要针对制冷压缩机、风机、紫外灯、照明灯等设备
        /// </summary>
        public enum DeviceControlStatus
        {
            Close,// 停止
            Open,// 打开
            NoCircle,// 不存在控制回路
            Keep,// 状态保持不变
        }

        /// <summary>
        /// 使能禁能状态
        /// </summary>
        public enum EnableKind
        {
            Unknown,// 未知
            Enable,// 使能
            Disable,// 禁能
        }

        /// <summary>
        /// 管理员类型
        /// </summary>
        public enum UserType
        {
            Unknow,// 未知
            NormalUser,// 一般管理员
            SecondUser,// 二级管理员
            SystemUser,// 厂商管理员
        }

        /// <summary>
        /// 一卡通业务类型
        /// </summary>
        public enum PosBusiType
        {
            WuHanTong,// 武汉通
            XiAnTong_NoNet,// 长安通（不带网络）
            Other,// 其它一卡通
        }

        /// <summary>
        /// 屏幕类型
        /// </summary>
        public enum ScreenType
        {
            ScreenType26,// 26寸屏
            ScreenType50,// 50寸屏
        }

        /// <summary>
        /// 温控模式 0：制冷 1：加热 2：常温
        /// </summary>
        public enum TmpControlModel
        {
            Refrigeration,// 制冷
            Heating,// 加热
            Normal,// 常温
        }

        /// <summary>
        /// 广告文件格式
        /// </summary>
        public enum AdvertType
        {
            Image,// 图片
            Video,// 视频
        }

        /// <summary>
        /// 商品展示模式
        /// </summary>
        public enum GoodsShowModelType
        {
            GoodsToOnlyAsile,// 商品图片一一对应货道
            GoodsToMultiAsile,// 商品图片对应多个货道
            InputAsileCode,// 键盘输入货道编号模式
            GoodsType,// 商品分类模式
        }

        /// <summary>
        /// 商品标示类型（主要针对药品） 0：普通商品 1：红色OTC 2：绿色OTC 3：保健食品
        /// </summary>
        public enum DrugType
        {
            Normal,// 普通商品
            OTC_Red,// 红色OTC
            OTC_Green,// 绿色OTC
            BaoJian,// 保健食品
        }

        /// <summary>
        /// 网址链接的URL
        /// </summary>
        public enum WebUrl
        {
            ShangHai_ZhiYuanZhe_XieHui,// 上海志愿者协会网址
            ShangHai_ZhiYuanZhe_JiJinHui,// 上海志愿者基金会
            ShangHai_ZhiYuanZhe_ItemList,// 上海志愿者活动列表
            ShangHai_ZhiYuanZhe_JiJinHui_ItemList,// 上海志愿者基金会查询
        }

        /// <summary>
        /// 客户的主界面顶部样式
        /// </summary>
        public enum Main_Lgs_TopType
        {
            ShangHai_ZhiYuanZhe,// 上海志愿者协会的顶部样式
            Normal,// 普通用户的顶部样式
        }

        /// <summary>
        /// 硬币找零设备类型
        /// </summary>
        public enum CoinDeviceType
        {
            CoinDevice,// 硬币器
            Hook,// HOOk找零器
        }

        /// <summary>
        /// 增值服务类型
        /// </summary>
        public enum AddServiceType
        {
            IDCard_Take,// 身份证领取
            BarCode_Take,// 线下扫码领取
            WxTakeCode_Take,// 微信取货码领取
        }

        /// <summary>
        /// 网络状态
        /// </summary>
        public enum NetStatus
        {
            OffLine,// 离线
            OnLine,// 联机
            NoNet,// 不存在网络设备或不存在网络功能
        }

        /// <summary>
        /// 货币管理模式
        /// </summary>
        public enum CashManagerModel
        {
            Singal,// 简单管理模式
            Advance,// 高级管理模式
        }

        /// <summary>
        /// 货道销售类型
        /// </summary>
        public enum AsileSellModel
        {
            Normal,// 正常销售
            Gift,// 赠品，不参与销售
        }

        /// <summary>
        /// 商品销售类型
        /// </summary>
        public enum McdSaleType
        {
            Normal,// 正常销售商品
            Hot,// 热销商品
            New,// 新品
            DaZhe,// 打折商品
            Free,// 免费商品
        }
    }
}
