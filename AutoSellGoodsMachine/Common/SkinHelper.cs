using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoSellGoodsMachine
{
    public class SkinHelper
    {
        #region 图片文件地址常量

        public const string FILEPATH_ADD_NORMAL = "pack://siteoforigin:,,,/Images/ManagerPic/Pic_Button/+.png";
        public const string FILEPATH_ADD_GRAY = "pack://siteoforigin:,,,/Images/ManagerPic/Pic_Button/+gray.png";
        public const string FILEPATH_REDUCE_NORMAL = "pack://siteoforigin:,,,/Images/ManagerPic/Pic_Button/-.png";
        public const string FILEPATH_REDUCE_GRAY = "pack://siteoforigin:,,,/Images/ManagerPic/Pic_Button/-gray.png";

        #endregion

        public static string p_SkinName = string.Empty;

        public static void GetSkinStyle(string skinStyle)
        {
            switch (skinStyle)
            {
                case "0":// 蓝天白云样式
                    p_SkinName = "pack://siteoforigin:,,,/Images/FormPic/Skin/Cloud/";
                    break;

                case "1":// 星空样式
                    p_SkinName = "pack://siteoforigin:,,,/Images/FormPic/Skin/Star/";
                    break;

                default:// 粉红样式
                    p_SkinName = "pack://siteoforigin:,,,/Images/FormPic/Skin/Pink/";
                    break;
            }
        }

        /// <summary>
        /// 皮肤样式
        /// </summary>
        public enum SkinStyle
        {
            Cloud,// 蓝天白云
            Star,// 星空
            Pink,// 粉红
        }
    }
}
