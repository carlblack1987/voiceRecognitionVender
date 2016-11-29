#region [ KIMMA Co.,Ltd. Copyright (C) 2014 ]
//-------------------------------------------------------------------------------------
// 开发单位：湖南金码智能设备制造有限公司
// 业务模块：iVend终端软件业务处理类
// 业务功能：广告文件信息模板
// 创建标识：2015-03-16		谷霖
//-------------------------------------------------------------------------------------
#endregion

using Business.Enum;

namespace Business.Model
{
    public class AdvertInfoModel
    {
         /// <summary>
        /// 析构
        /// </summary>
        public AdvertInfoModel()
        {
            AdvListID = string.Empty;
            FileCode = string.Empty;
            FileName = string.Empty;
            FileType = string.Empty;
            PlayNo = 0;
            FileSize = 0;
            PlayNum = 1;
            DelayTime = 5;
            DownUrl = string.Empty;
            GetType = string.Empty;
            IsPlayTime = "0";
            PlayTime1 = string.Empty;
            PlayTime2 = string.Empty;
        }

        /// <summary>
        /// 节目单编号
        /// </summary>
        public string AdvListID { get; set; }

        /// <summary>
        /// 广告文件编码
        /// </summary>
        public string FileCode { get; set; }

        /// <summary>
        /// 广告文件名称
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 广告文件格式
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// 广告播放顺序序号
        /// </summary>
        public int PlayNo { get; set; }

        /// <summary>
        /// 广告文件类型
        /// </summary>
        public BusinessEnum.AdvertType AdvertType { get; set; }

        

        /// <summary>
        /// 广告文件大小
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// 广告文件播放次数
        /// </summary>
        public int PlayNum { get; set; }

        /// <summary>
        /// 广告文件播放停留时间，以秒为单位
        /// </summary>
        public int DelayTime { get; set; }

        /// <summary>
        /// 广告文件下载URL地址
        /// </summary>
        public string DownUrl { get; set; }

        /// <summary>
        /// 广告文件获取方式
        /// </summary>
        public string GetType { get; set; }

        /// <summary>
        /// 是否存在允许播放时间段 0：不存在 1：存在
        /// </summary>
        public string IsPlayTime { get; set; }

        /// <summary>
        /// 允许播放时间段1:开始时分结束时分，时分格式为：hhmm
        /// </summary>
        public string PlayTime1 { get; set; }

        /// <summary>
        /// 允许播放时间段2:开始时分结束时分，时分格式为：hhmm
        /// </summary>
        public string PlayTime2 { get; set; }
    }
}
