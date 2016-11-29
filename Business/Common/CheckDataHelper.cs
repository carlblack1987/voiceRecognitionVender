#region [ KIMMA Co.,Ltd. Copyright (C) 2014 ]
//-------------------------------------------------------------------------------------
// 开发单位：湖南金码智能设备制造有限公司
// 业务模块：iVend终端软件业务处理类
// 业务功能：数据检验类
// 创建标识：2014-06-10		谷霖
//-------------------------------------------------------------------------------------
#endregion

using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Management;
using System.IO;

namespace Business.Common
{
    public class CheckDataHelper
    {
        #region 数据有效性验证

        #region 机器出厂编号验证

        /// <summary>
        /// 机器出厂编号验证
        /// </summary>
        /// <param name="cardNum">要验证的机器出厂编号</param>
        /// <returns>验证结果 True：通过 False：不通过</returns>
        public bool CheckVmId(string vmId)
        {
            if (string.IsNullOrEmpty(vmId))
            {
                return false;
            }

            if (vmId.Length != 10)
            {
                return false;
            }

            try
            {
                long intVmId = Convert.ToInt64(vmId);
            }
            catch
            {
                return false;
            }

            return true;
        }

        #endregion

        #region 卡号验证

        /// <summary>
        /// 卡号有效性验证
        /// </summary>
        /// <param name="cardNum">要验证的卡号</param>
        /// <returns>验证结果 True：通过 False：不通过</returns>
        public bool CheckCardNum(string cardNum)
        {
            if (string.IsNullOrEmpty(cardNum))
            {
                return false;
            }
            if (cardNum.Length == 0)
            {
                return false;
            }

            // 卡号正则表达式
            try
            {
                long intCard = Convert.ToInt64(cardNum);
            }
            catch
            {
                return false;
            }
            //Regex RegCardNo = new Regex("^[0-9]{10}$");

            //if (!RegCardNo.Match(cardNum).Success)
            //{
            //    CardHelper.ShowMsgBox(StringHelper.GetStringBundle("Err_ValidCard"), 
            //        MsgButtonType.Ok, MsgType.Warning);
            //    return false;
            //}

            return true;
        }

        /// <summary>
        /// 卡号有效性验证
        /// </summary>
        /// <param name="cardNum">要验证的卡号</param>
        /// <param name="cardLength">卡号长度</param>
        /// <returns>验证结果 True：通过 False：不通过</returns>
        public bool CheckCardNum(string cardNum, string cardLength)
        {
            if (string.IsNullOrEmpty(cardNum))
            {
                return false;
            }
            if (cardNum.Length == 0)
            {
                return false;
            }

            // 卡号正则表达式
            try
            {
                if (cardNum.Length.ToString() != cardLength)
                {
                    return false;
                }

                long intCard = Convert.ToInt64(cardNum);
            }
            catch
            {
                return false;
            }

            return true;
        }

        #endregion

        #region Email验证

        /// <summary>
        /// 检测是否属于Email
        /// </summary>
        /// <param name="mail">要验证的Mail</param>
        /// <returns>结果 True：通过 False：不通过</returns>
        public bool CheckMail(string mail)
        {
            bool result = false;

            try
            {
                result = Regex.IsMatch(mail, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)" +

                        @"|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
            }
            catch
            {
                result = false;
            }

            return result;
        }

        #endregion

        #region 身份证验证

        /// <summary>
        /// 校验身份证有效性
        /// </summary>
        /// <param name="Id">身份证号</param>
        /// <returns>结果 True：是 False：否</returns>
        public bool CheckIDCard(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return false;
            }
            if (Id.Length == 0)
            {
                // 没有输入身份证号
                return false;
            }

            bool check;

            if (Id.Length == 18)
            {
                check = CheckIDCard18(Id);
            }
            else if (Id.Length == 15)
            {
                check = CheckIDCard15(Id);
            }
            else
            {
                check = false;
            }
            return check;
        }

        /// <summary>
        /// 校验18位身份证有效性
        /// </summary>
        /// <param name="Id">身份证号</param>
        /// <returns>结果 True：是 False：否</returns>
        private bool CheckIDCard18(string Id)
        {
            long n = 0;
            if (long.TryParse(Id.Remove(17), out n) == false || n < Math.Pow(10, 16) || long.TryParse(Id.Replace('x', '0').Replace('X', '0'), out n) == false)
            {
                return false;//数字验证
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(Id.Remove(2)) == -1)
            {
                return false;//省份验证
            }
            string birth = Id.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证
            }
            string[] arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
            string[] Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
            char[] Ai = Id.Remove(17).ToCharArray();
            int sum = 0;
            for (int i = 0; i < 17; i++)
            {
                sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
            }
            int y = -1;
            Math.DivRem(sum, 11, out y);
            if (arrVarifyCode[y] != Id.Substring(17, 1).ToLower())
            {
                return false;//校验码验证
            }
            return true;//符合GB11643-1999标准
        }

        /// <summary>
        /// 校验15位身份证有效性
        /// </summary>
        /// <param name="Id">身份证号</param>
        /// <returns>结果 True：是 False：否</returns>
        private bool CheckIDCard15(string Id)
        {
            long n = 0;
            if (long.TryParse(Id, out n) == false || n < Math.Pow(10, 14))
            {
                return false;//数字验证
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(Id.Remove(2)) == -1)
            {
                return false;//省份验证
            }
            string birth = Id.Substring(6, 6).Insert(4, "-").Insert(2, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证
            }
            return true;//符合15位身份证标准
        }

        #endregion

        #region 手机号码处理

        /// <summary>
        /// 手机号码验证
        /// </summary>
        /// <param name="phoneNum">要验证的手机号码</param>
        /// <returns>结果 True：是 False：否</returns>
        public bool CheckPhoneNum(string phoneNum)
        {
            if (string.IsNullOrEmpty(phoneNum))
            {
                return false;
            }

            //手机正则表达式
            Regex RegPhone = new Regex("^(((13|15|18)[0-9])|147|145)[0-9]{8}$");

            if (!RegPhone.Match(phoneNum).Success)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取手机号码所属运营商
        /// </summary>
        /// <param name="phoneNum">手机号码</param>
        /// <returns>运营商 0：非运营商号段 1：移动 2：联通 3：电信</returns>
        public string GetPhoneNumType(string phoneNum)
        {
            if (!CheckPhoneNum(phoneNum))
            {
                return "0";
            }

            string strPhoneType = "0";
            string strPhoneTypeCode = phoneNum.Substring(0, 3);

            switch (strPhoneTypeCode)
            {
                case "134":// 移动
                case "135":
                case "136":
                case "137":
                case "138":
                case "139":
                case "147":
                case "150":
                case "151":
                case "152":
                case "157":
                case "158":
                case "159":
                case "182":
                case "183":
                case "184":
                case "187":
                case "188":
                    strPhoneType = "1";
                    break;

                case "130":// 联通
                case "131":
                case "132":
                case "145":
                case "155":
                case "156":
                case "185":
                case "186":
                    strPhoneType = "2";
                    break;

                case "133":// 电信
                case "153":
                case "180":
                case "181":
                case "189":
                    strPhoneType = "3";
                    break;

                default:
                    break;
            }

            return strPhoneType;
        }

        #endregion

        #region 数字型验证

        /// <summary>
        /// 检测是否是整数数字
        /// </summary>
        /// <param name="num">要检测的字符串</param>
        /// <returns>验证结果 True：通过 False：不通过</returns>
        public bool CheckIsNum(string num)
        {
            if (string.IsNullOrEmpty(num))
            {
                return false;
            }
            if (num.Length == 0)
            {
                return false;
            }

            try
            {
                long intSerNo = Convert.ToInt64(num);
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 检测是否为正整数
        /// </summary>
        /// <param name="num">要检测的字符串</param>
        /// <returns>验证结果 True：通过 False：不通过</returns>
        public bool CheckIsPosInt(string num)
        {
            bool result = false;
            if (string.IsNullOrEmpty(num))
            {
                return false;
            }
            result = CheckIsNum(num);
            if (result)
            {
                long intNum = Convert.ToInt64(num);
                if (intNum <= 0)
                {
                    result = false;
                }
            }
            return result;
        }

        /// <summary>
        /// 检测是否为负整数
        /// </summary>
        /// <param name="num">要检测的字符串</param>
        /// <returns>验证结果 True：通过 False：不通过</returns>
        public bool CheckIsNegInt(string num)
        {
            bool result = false;
            if (string.IsNullOrEmpty(num))
            {
                return false;
            }
            result = CheckIsNum(num);
            if (result)
            {
                long intNum = Convert.ToInt64(num);
                if (intNum >= 0)
                {
                    result = false;
                }
            }
            return result;
        }

        /// <summary>
        /// 检测是否属于浮点数
        /// </summary>
        /// <param name="num">要检测的字符串</param>
        /// <returns>验证结果 True：通过 False：不通过</returns>
        public bool CheckIsDouble(string num)
        {
            if (string.IsNullOrEmpty(num))
            {
                return false;
            }

            if (num.Length == 0)
            {
                return false;
            }

            try
            {
                double intSerNo = Convert.ToDouble(num);
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 检测是否为正浮点数
        /// </summary>
        /// <param name="num">要检测的字符串</param>
        /// <returns>验证结果 True：通过 False：不通过</returns>
        public bool CheckIsPosDouble(string num)
        {
            bool result = false;
            if (string.IsNullOrEmpty(num))
            {
                return false;
            }

            result = CheckIsDouble(num);
            if (result)
            {
                double intNum = Convert.ToDouble(num);
                if (intNum <= 0)
                {
                    result = false;
                }
            }
            return result;
        }

        /// <summary>
        /// 检测是否为负浮点数
        /// </summary>
        /// <param name="num">要检测的字符串</param>
        /// <returns>验证结果 True：通过 False：不通过</returns>
        public bool CheckIsNegDouble(string num)
        {
            bool result = false;
            if (string.IsNullOrEmpty(num))
            {
                return false;
            }

            result = CheckIsDouble(num);
            if (result)
            {
                double intNum = Convert.ToDouble(num);
                if (intNum >= 0)
                {
                    result = false;
                }
            }
            return result;
        }

        #endregion

        #region 字符型处理

        /// <summary>
        /// 判断是否是中文
        /// </summary>
        /// <param name="s">输入的字符串</param>
        /// <returns>验证结果，False：否，True：是</returns>
        public bool IsChineseWord(string s)
        {
            string[] stringMatchs = new string[]
            {
                @"[\u3040-\u318f]+",
                @"[\u3300-\u337f]+",
                @"[\u3400-\u3d2d]+",
                @"[\u4e00-\u9fff]+",
                @"[\u4e00-\u9fa5]+",
                @"[\uf900-\ufaff]+"
            };

            foreach (string stringMatch in stringMatchs)
            {
                if (Regex.IsMatch(s, stringMatch))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取字符串长度，中文长度为2
        /// </summary>
        /// <param name="s">输入的字符串</param>
        /// <returns>字符串长度</returns>
        public int StringLength(string s)
        {
            int totalLen = 0;
            try
            {
                if (s == null)
                {
                    return 0;
                }
                char[] ch = s.ToCharArray();

                foreach (char c in ch)
                {
                    if (IsChineseWord(c.ToString()))
                    {
                        totalLen += 2;
                    }
                    else
                    {
                        totalLen += 1;
                    }
                }
            }
            catch
            {

            }
            return totalLen;
        }

        /// <summary>
        /// 取得字符串长度（中文长度为2计算）
        /// </summary>
        /// <param name="s">输入字符串</param>
        /// <returns>字符串长度</returns>
        public Int32 GetByteCount(String s)
        {
            return Encoding.Default.GetByteCount(s.ToCharArray());
        }

        /// <summary>
        /// 取得整数部分的长度。
        /// </summary>
        /// <param name="s">输入字符串</param>
        /// <returns>整数部分长度</returns>
        public int GetIntegerPartLength(String s)
        {
            s = s.Trim();
            if (s.IndexOf(".") != -1)
            {
                return s.Split('.')[0].Length;
            }
            return s.Length;
        }

        /// <summary>
        /// 取得小数部分的长度。
        /// </summary>
        /// <param name="s">输入字符串</param>
        /// <returns>小数部分长度</returns>
        public int GetDecimalPartLength(String s)
        {
            s = s.Trim();
            if (s.IndexOf(".") != -1)
            {
                return s.Split('.')[1].Length;
            }
            return 0;
        }

        /// <summary>
        /// 验证字符串是否合法
        /// </summary>
        /// <param name="input">输入的字符串</param>
        /// <returns>验证结果，False：非法，True：合法</returns>
        public bool IsValid(string input)
        {
            // 初始化正则表达式
            string regex = @"[^\$\%\^<>]*";
            // 返回true or false
            return Regex.Match(input, regex).Success;
        }

        /// <summary>
        /// 截取字符串(中文长度为2计算)
        /// </summary>
        /// <param name="sourceString">源字符串</param>
        /// <param name="byteLength">要截取的长度</param>
        /// <returns>截取后的字符串</returns>
        public string CutString(string sourceString, int byteLength)
        {
            StringBuilder sb = new StringBuilder();
            Int32 byteCount = 0;
            for (Int32 i = 0; i < sourceString.Length; i++)
            {
                byteCount += Encoding.Default.GetByteCount(sourceString[i].ToString());

                if (byteCount > byteLength)
                {
                    break;
                }
                sb.Append(sourceString[i]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 检测是否为指定长度的数值。
        /// </summary>
        /// <param name="s">输入字符串</param>
        /// <param name="length">指定长度</param>
        /// <returns>检测结果，False：否，True：是</returns>
        public bool IsNumber(String s, int length)
        {
            if (s.Length > length)
            {
                return false;
            }
            return CheckIsNum(s);
        }

        /// <summary>
        /// 判断是否为十进制。
        /// </summary>
        /// <param name="s">输入字符串</param>
        /// <returns>检测结果，False：否，True：是</returns>
        public bool IsDecimal(String s)
        {
            try
            {
                Decimal.Parse(s);
            }
            catch
            {
                return false;
            }
            return true;
        }

        #endregion

        #region 日期有效性检测

        /// <summary>
        /// 判断是否为日期类型
        /// </summary>
        /// <param name="s">输入字符串</param>
        /// <returns>检测结果，False：否，True：是</returns>
        public bool IsDateTime(String s)
        {
            try
            {
                DateTime.Parse(s);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool IsValidDateTimeSpan(DateTime start, DateTime end)
        {
            return DateTime.Compare(start, end) <= 0;
        }

        #endregion

        #region 金额有效性检测

        #endregion

        #region IP地址检测

        /// <summary>
        /// 检测有效IP
        /// </summary>
        /// <param name="s">IP地址</param>
        /// <returns>验证结果，False：无效，True：有效</returns>
        public bool IsValidIP(String s)
        {
            if (Regex.IsMatch(s, @"\d{1,3}.\d{1,3}.\d{1,3}.\d{1,3}"))
            {
                String[] ipParts = s.Split('.');
                if (ipParts.Length != 4)
                {
                    return false;
                }
                for (Int32 i = 0; i < ipParts.Length; i++)
                {
                    if (ipParts[i].Length > 3 || ipParts[i].Length < 1)
                    {
                        return false;
                    }
                    if (!CheckIsNum(ipParts[i]))
                    {
                        return false;
                    }
                    Int32 num = Convert.ToInt32(ipParts[i]);
                    if (num < 0 || num > 255)
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        #endregion

        #endregion

        #region MD5加密

        /// <summary>
        /// 16位MD5加密
        /// </summary>
        /// <param name="sourceStr">加密原始字符串</param>
        /// <param name="strType">转换后形式 0：大写 1：小写</param>
        /// <returns>加密字符串</returns>
        public string Md516(string sourceStr, string md5Type)
        {
            string md5Str = "";

            if (sourceStr == null)
            {
                return "";
            }

            try
            {
                string strType = md5Type;
                if ((strType != "0") &&
                    (strType != "1"))
                {
                    strType = "1";
                }

                MD5 m = new MD5CryptoServiceProvider();

                md5Str = BitConverter.ToString(m.ComputeHash(UTF8Encoding.Default.GetBytes(sourceStr)), 4, 8);

                md5Str = md5Str.Replace("-", "");

                if (strType == "0")
                {
                    // 大写
                    md5Str = md5Str.ToUpper();
                }
                else
                {
                    // 小写
                    md5Str = md5Str.ToLower();
                }
            }
            catch
            {
            }

            return md5Str;
        }

        /// <summary>
        /// 32位MD5加密
        /// </summary>
        /// <param name="sourceStr">加密原始字符串</param>
        /// <param name="md5Type">转换后形式 0：大写 1：小写</param>
        /// <returns>加密字符串</returns>
        public string Md532(string sourceStr, string md5Type)
        {
            string md5Str = "";

            if (sourceStr == null)
            {
                return "";
            }

            try
            {
                string strType = md5Type;
                if ((strType != "0") &&
                    (strType != "1"))
                {
                    strType = "1";
                }

                MD5 m = new MD5CryptoServiceProvider();

                // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
                byte[] s = m.ComputeHash(Encoding.UTF8.GetBytes(sourceStr));
                // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
                for (int i = 0; i < s.Length; i++)
                {
                    // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 

                    md5Str = md5Str + s[i].ToString("X").PadLeft(2, '0');

                }

                //md5Str = md5Str.Replace("-", "");

                if (strType == "1")
                {
                    // 小写
                    md5Str = md5Str.ToLower();
                }
            }
            catch
            {
            }
            return md5Str;
        }

        #endregion

        #region 金额转换

        /// <summary>
        /// 将金额从转后为string,即将金额加小数点
        /// </summary>
        /// <param name="money">要转换的金额</param>
        /// <param name="money">小数点位数100、10等</param>
        /// <returns>转换结果</returns>
        public string MoneyIntToString(int money, string deciPoint)
        {
            string strMoney = money.ToString();
            int length = strMoney.Length;
            if (length < deciPoint.Length)
            {
                strMoney = strMoney.PadLeft(deciPoint.Length, '0');
            }
            return strMoney.Insert(strMoney.Length - deciPoint.Length + 1, ".");
        }

        /// <summary>
        /// 将金额从转后为string,即将金额加小数点
        /// </summary>
        /// <param name="money">要转换的金额</param>
        /// <param name="money">小数点位数100、10等</param>
        /// <returns>转换结果</returns>
        public string MoneyLongToString(long money, string deciPoint)
        {
            string strMoney = money.ToString();
            int length = strMoney.Length;
            if (length < deciPoint.Length)
            {
                strMoney = strMoney.PadLeft(deciPoint.Length, '0');
            }
            return strMoney.Insert(strMoney.Length - deciPoint.Length + 1, ".");
        }

        /// <summary>
        /// 把金额转换为以分为单位
        /// </summary>
        /// <param name="money">要转换的金额</param>
        /// <param name="money">小数点位数100、10等</param>
        /// <returns>结果</returns>
        public int MoneyStringToInt(string money, string deciPoint)
        {
            int intMoney = 0;
            if (!CheckIsDouble(money))
            {
                return 0;
            }
            try
            {
                intMoney = (int)(Convert.ToDouble(money) * Convert.ToInt32(deciPoint));
            }
            catch
            {
            }
            return intMoney;
        }

        #endregion

        #region 获取GUID码

        /// <summary>
        /// 获取GUID码
        /// </summary>
        /// <returns>返回的GUID码</returns>
        public string GetGuid()
        {
            string strGuid = string.Empty;

            strGuid = System.Guid.NewGuid().ToString();
            strGuid = strGuid.Replace("-", "");
            return strGuid;
        }

        #endregion

        #region 生成随机数字码

        /// <summary>
        /// 生成随机数字码
        /// </summary>
        /// <param name="lenNum">随机数字码长度</param>
        /// <returns>随机数字码</returns>
        public string CreateRandNum(int lenNum)
        {
            string strCodeNum = string.Empty;

            if (lenNum < 1)
            {
                return strCodeNum;
            }

            try
            {
                Random rm = new Random(Guid.NewGuid().GetHashCode());

                string strMaxNum = "9";
                strMaxNum = strMaxNum.PadLeft(lenNum, '9');

                int intMaxNum = Convert.ToInt32(strMaxNum);

                int nValue = rm.Next(intMaxNum);

                strCodeNum = nValue.ToString();
                strCodeNum = strCodeNum.PadLeft(lenNum, '0');

            }
            catch
            {
                strCodeNum = string.Empty;
            }
            return strCodeNum;
        }

        #endregion

        #region 获取磁盘空间大小

        /// <summary>
        /// 获取软件所在盘符
        /// </summary>
        /// <returns></returns>
        public string GetAppDisk()
        {
            string strBaseDire = AppDomain.CurrentDomain.BaseDirectory.ToString();
            string[] hexValue = strBaseDire.Split(':');
            strBaseDire = hexValue[0] + ":\\";
            return strBaseDire;
        }

        /// <summary>
        /// 获取当前软件所在磁盘空间
        /// </summary>
        /// <returns></returns>
        public long GetCurrentDiskSpaceSize()
        {
            string str_HardDiskName = GetAppDisk();

            if (!str_HardDiskName.EndsWith(":\\"))
            {
                str_HardDiskName += ":\\";
            }

            long freeSpace = new long();
            freeSpace = 0;

            System.IO.DriveInfo[] drives = System.IO.DriveInfo.GetDrives();
            foreach (System.IO.DriveInfo drive in drives)
            {
                if (drive.Name == str_HardDiskName)
                {
                    freeSpace = drive.TotalFreeSpace;
                    break;
                }
            }

            return freeSpace;
        }

        /// <summary>
        /// 检测软件所在盘空间是否足—根据制定大小
        /// </summary>
        /// <returns></returns>
        public bool CheckDiskIsSpace(long fileTotalSize)
        {
            long freeSpace = GetCurrentDiskSpaceSize();
            if (freeSpace > fileTotalSize)
            {
                // 空间足
                return true;
            }

            return false;
        }

        #endregion
    }
}
