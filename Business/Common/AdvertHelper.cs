#region [ KIMMA Co.,Ltd. Copyright (C) 2014 ]
//-------------------------------------------------------------------------------------
// 开发单位：湖南金码智能设备制造有限公司
// 业务模块：iVend终端软件业务处理类
// 业务功能：广告文件信息处理类
// 创建标识：2015-03-16		谷霖
//-------------------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Threading;
using KdbPlug;
using Business.Model;
using SoftTermUpdate;
using LitJson;
using JsonPlug;

namespace Business.Common
{
    public class AdvertHelper
    {
        #region 变量声明

        private string _m_DbFileName = "TermInfo.db";

        /// <summary>
        /// 广告文件信息链表
        /// </summary>
        public List<AdvertInfoModel> AdvertList = new List<AdvertInfoModel>();

        public List<AdvertInfoModel> AdvertList_Import = new List<AdvertInfoModel>();

        private List<AdvertInfoModel> AdvertList_Temp = new List<AdvertInfoModel>();

        public string AdvertPath = "";

        /// <summary>
        /// 是否结束 False：未结束 True：结束
        /// </summary>
        private bool m_Close = false;

        private SoftTermUpdateOper m_SoftTermUpdateOper = new SoftTermUpdateOper();

        #endregion

        #region 属性

        private string m_NowAdvertPlayID = string.Empty;
        /// <summary>
        /// 当前广告节目单编号
        /// </summary>
        public string NowAdvertPlayID
        {
            get { return m_NowAdvertPlayID; }
            set { m_NowAdvertPlayID = value; }
        }

        private string m_UpdateAdvertListID = string.Empty;
        /// <summary>
        /// 正在更新的广告节目单编号
        /// </summary>
        public string UpdateAdvertListID
        {
            get { return m_UpdateAdvertListID; }
            set { m_UpdateAdvertListID = value; }
        }

        private string m_VmSoftUrl = string.Empty;
        /// <summary>
        /// 终端远程更新URL
        /// </summary>
        public string VmSoftUrl
        {
            get { return m_VmSoftUrl; }
            set { m_VmSoftUrl = value;
                    m_SoftTermUpdateOper.VmSoftUrl = value;
            }
        }

        private string m_VmID = string.Empty;
        /// <summary>
        /// 机器出厂号
        /// </summary>
        public string VmID
        {
            get { return m_VmID; }
            set { m_VmID = value;
                m_SoftTermUpdateOper.VmID = value;
            }
        }

        private string m_UserKey = string.Empty;
        /// <summary>
        /// UserKey
        /// </summary>
        public string UserKey
        {
            get { return m_UserKey; }
            set { m_UserKey = value;
                    m_SoftTermUpdateOper.UserKey = value;
            }
        }

        private string m_AdvertUploadType = "0";
        /// <summary>
        /// 广告更新策略 0：只允许本地更新 1：只允许远程更新 2：本地更新和远程更新皆可
        /// </summary>
        public string AdvertUploadType
        {
            get { return m_AdvertUploadType; }
            set { m_AdvertUploadType = value; }
        }

        /// <summary>
        /// 当前正在播放的广告
        /// </summary>
        public AdvertInfoModel CurrentAdvertInfo
        {
            get;
            set;
        }

        /// <summary>
        /// 是否刷新当前广告
        /// </summary>
        public bool IsRefreshAdvert
        {
            get;
            set;
        }

        #endregion

        #region 2016-09-09添加接口函数

        /// <summary>
        /// 初始化广告
        /// </summary>
        /// <returns></returns>
        public bool InitAdvert()
        {
            bool result = false;

            // 加载广告
            LoadAdvertList_New(m_NowAdvertPlayID);

            ////Thread m_TrdSendNetData = new Thread(new ThreadStart(AdvertOperMain));
            ////m_TrdSendNetData.IsBackground = true;
            ////m_TrdSendNetData.Start();

            result = true;

            return result;
        }

        /// <summary>
        /// 释放广告
        /// </summary>
        public void Dispose()
        {
            m_Close = true;
        }

        /// <summary>
        /// 检测广告节目单是否需要更新
        /// </summary>
        public bool AdvertCheckUpdateMon()
        {
            bool result = false;
            string strAdvertListInfo = string.Empty;
            string _value = string.Empty;
            int intErrCode = 0;

            ////int intUploadCheckTime = 100 * 5;// 3000 * 5;// 10分钟自动检查一次
            ////int intUploadAddNum = 0;

            /* 流程说明
                 * 1、每隔固定时间，到服务器检查是否有需要更新的广告节目单，如果有，则进行处理
                 * 2、每隔固定时间，到本地数据库检查是否有需要远程下载的广告文件，如果有，则进行处理
                */

            if ((m_AdvertUploadType == "1") || (m_AdvertUploadType == "2"))
            {
                #region 定时检查广告节目单是否需要更新
                result = CheckAdvUpdate(out strAdvertListInfo);
                if (result)
                {
                    // 导入更新的广告节目单
                    intErrCode = ImportAdvertList(strAdvertListInfo, "1", "", out _value);
                    ////if (intErrCode == 0)
                    ////{
                    ////    // 导入更新成功
                    ////}
                }
                #endregion
            }
            return result;
        }

        /// <summary>
        /// 更新终端配置参数
        /// </summary>
        /// <param name="_configList"></param>
        /// <returns></returns>
        public bool UploadVmConfig_Net(out string _configList)
        {
            bool result = false;
            _configList = string.Empty;

            result = m_SoftTermUpdateOper.UploadVmConfig_Net(out _configList);

            return result;
        }

        public bool GoodsCheckUpdateMon(out string _goodsList)
        {
            bool result = false;
            _goodsList = string.Empty;

            result = m_SoftTermUpdateOper.GoodsCheckUpdateMon(out _goodsList);

            return result;
        }

        /// <summary>
        /// 检测是否有需要下载的广告文件
        /// </summary>
        public void AdvertUploadMon()
        {
            DbOper dbOper = new DbOper();
            dbOper.DbFileName = _m_DbFileName;

            try
            {
                dbOper.ConnType = ConnectType.KeepConn;
                string strSql = @"select AdvListID,FileCode,DownUrl,FileName,FileFormat   
                            from T_ADV_ADVFILE where ImportType = '1' and Status in ('0','1','3')";

                DataSet dataSet = dbOper.dataSet(strSql);
                if (dataSet.Tables.Count > 0)
                {
                    int intCount = dataSet.Tables[0].Rows.Count;
                    bool result = false;
                    if (intCount > 0)
                    {
                        AdvertInfoModel advertInfo = new AdvertInfoModel();
                        for (int i = 0; i < intCount; i++)
                        {
                            advertInfo.AdvListID = dataSet.Tables[0].Rows[i]["AdvListID"].ToString();
                            advertInfo.FileCode = dataSet.Tables[0].Rows[i]["FileCode"].ToString();
                            advertInfo.FileName = dataSet.Tables[0].Rows[i]["FileName"].ToString();
                            advertInfo.DownUrl = dataSet.Tables[0].Rows[i]["DownUrl"].ToString();
                            advertInfo.FileType = dataSet.Tables[0].Rows[i]["FileFormat"].ToString();
                            result = DownAdvertFile(advertInfo);
                            if (result)
                            {
                                // 下载成功，保存广告文件状态
                                strSql = "update T_ADV_ADVFILE set Status = '2' where AdvListID = '" + advertInfo.AdvListID + "' and FileCode = '" + advertInfo.FileCode + "'";

                                result = dbOper.excuteSql(strSql);
                            }
                        }

                    }
                }
            }
            catch
            {

            }
            finally
            {
                dbOper.closeConnection();
            }
        }

        /// <summary>
        /// 检查当前更新的广告节目单是否可以播放
        /// </summary>
        /// <param name="advListID"></param>
        /// <returns></returns>
        public bool CheckUploadAdvIsPlay(string advListID)
        {
            bool result = false;
            DbOper dbOper = new DbOper();
            dbOper.DbFileName = _m_DbFileName;

            try
            {
                if (string.IsNullOrEmpty(advListID))
                {
                    result = false;
                    return result;
                }

                dbOper.ConnType = ConnectType.KeepConn;

                #region 检查该广告节目单的开始播放日期是否到

                string strSql = @"select BeginDate,TotalNum   
                            from T_ADV_ADVLIST where AdvListID = '" + advListID + "'";
                int intCount = 0;
                bool blnIsDate = false;
                int intTotalNum = 0;
                DataSet dataSet = dbOper.dataSet(strSql);
                if (dataSet.Tables.Count > 0)
                {
                    intCount = dataSet.Tables[0].Rows.Count;
                    if (intCount > 0)
                    {
                        long intBeginDate = Convert.ToInt64(dataSet.Tables[0].Rows[0]["BeginDate"].ToString());
                        long intNowDate = Convert.ToInt64(DateTime.Now.ToString("yyyyMMdd"));
                        intTotalNum = Convert.ToInt32(dataSet.Tables[0].Rows[0]["TotalNum"].ToString());
                        if (intNowDate >= intBeginDate)
                        {
                            // 到达播放日期
                            blnIsDate = true;
                        }
                    }
                }

                #endregion

                if (blnIsDate)
                {
                    // 检查该广告节目单里的广告文件是否全部准备到位
                    strSql = @"select AdvListID  
                            from T_ADV_ADVFILE where AdvListID = '" + advListID + "' and status = '2'";
                    dataSet = dbOper.dataSet(strSql);
                    if (dataSet.Tables.Count > 0)
                    {
                        intCount = dataSet.Tables[0].Rows.Count;
                        if (intCount >= intTotalNum)
                        {
                            // 广告文件全部更新完毕
                            result = true;
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
        /// 根据广告节目单编号读取广告文件
        /// </summary>
        /// <param name="advListID">广告节目单编号</param>
        /// <returns>结果 True：成功 False：失败</returns>
        public bool LoadAdvertList_New(string advListID)
        {
            bool result = false;

            AdvertList_Temp.Clear();
            

            DbOper dbOper = new DbOper();
            dbOper.DbFileName = _m_DbFileName;

            try
            {
                string strSql = string.Empty;

                #region 获取广告信息

                strSql = @"select FileCode,FileName,FileFormat,FileType,FileSize,PlayNum,DelayTime 
                            from T_ADV_ADVFILE where AdvListID ='" + advListID + "' and status = '2'";

                DataSet dataSet = dbOper.dataSet(strSql);
                if (dataSet.Tables.Count > 0)
                {
                    int recordCount = dataSet.Tables[0].Rows.Count;
                    for (int i = 0; i < recordCount; i++)
                    {
                        AdvertList_Temp.Add(new AdvertInfoModel()
                        {
                            PlayNo = i,
                            FileCode = dataSet.Tables[0].Rows[i]["FileCode"].ToString(),
                            FileName = dataSet.Tables[0].Rows[i]["FileName"].ToString(),
                            FileType = dataSet.Tables[0].Rows[i]["FileFormat"].ToString(),
                            AdvertType = GetAdvertFileType(dataSet.Tables[0].Rows[i]["FileFormat"].ToString()),
                            FileSize = Convert.ToInt32(dataSet.Tables[0].Rows[i]["FileSize"].ToString()),
                            PlayNum = Convert.ToInt32(dataSet.Tables[0].Rows[i]["PlayNum"].ToString()),
                            DelayTime = Convert.ToInt32(dataSet.Tables[0].Rows[i]["DelayTime"].ToString())
                        });
                    }
                }

                #endregion

                result = true;
            }
            catch
            {
                result = false;
            }
            finally
            {
                dbOper.closeConnection();
                AdvertList = AdvertList_Temp;
            }
            return result;
        }

        /// <summary>
        /// 更新广告文件状态
        /// </summary>
        /// <param name="advertInfo"></param>
        /// <param name="status">0：等待更新 1：更新中 2：成功 3：失败</param>
        /// <returns></returns>
        public bool UpdateAdvFileStatus(AdvertInfoModel advertInfo,string status)
        {
            bool result = false;
            DbOper dbOper = new DbOper();
            dbOper.DbFileName = _m_DbFileName;

            try
            {
                string strSql = "update T_ADV_ADVFILE set Status = '" + status + "' where AdvListID = '" + advertInfo.AdvListID + "' and FileCode = '" + advertInfo.FileCode + "'";

                result = dbOper.excuteSql(strSql);
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

        #endregion

        #region 接口函数

        /// <summary>
        /// 读取广告播放文件
        /// </summary>
        /// <returns>结果 0：成功 1：获取导入信息失败 2：没有信息 3：导入失败</returns>
        public int LoadAdvertList(string advNo)
        {
            int intErrCode = 3;

            try
            {
                AdvertList_Temp.Clear();

                #region 读取广告配置文件

                ////string strAdvertListFilePath = AppDomain.CurrentDomain.BaseDirectory + "\\advert\\advertlist.txt";
                string strAdvertListFilePath = string.Empty;
                if ((string.IsNullOrEmpty(advNo)) || (advNo.Length == 0))
                {
                    strAdvertListFilePath = AppDomain.CurrentDomain.BaseDirectory + "\\advert\\advertlist.txt";
                }
                else
                {
                    strAdvertListFilePath = AppDomain.CurrentDomain.BaseDirectory + "\\advert\\AdvertList\\" + advNo + "\\advertlist.txt";
                }
                AdvertPath = advNo;

                if (!File.Exists(strAdvertListFilePath))
                {
                    // 广告配置文件不存在
                    return 2;
                }

                string strListInfo = File.ReadAllText(strAdvertListFilePath, Encoding.Default);

                #endregion

                JsonOper jsonOper = new JsonOper();
                string strResultCode = jsonOper.GetJsonKeyValue(strListInfo, "ret");
                if (strResultCode == "0")
                {
                    int intCount = 0;
                    intCount = Convert.ToInt32(jsonOper.GetJsonKeyValue(strListInfo, "count"));
                    if (intCount == 0)
                    {
                        intErrCode = 2;// 没有广告信息
                    }
                    else
                    {
                        string strFileName = string.Empty;
                        string strFileType = string.Empty;
                        JsonData jdItems = null;

                        string strSql = string.Empty;

                        jdItems = JsonMapper.ToObject(strListInfo);
                        jdItems = jdItems["advertlist"];

                        #region 遍历广告信息

                        int intNo = 0;
                        foreach (JsonData item in jdItems)
                        {
                            intNo++;
                            strFileName = item["filename"].ToString();
                            strFileType = item["filetype"].ToString();
                            AdvertList_Temp.Add(new AdvertInfoModel()
                            {
                                PlayNo = intNo - 1,
                                FileName = strFileName,
                                FileType = strFileType,
                                AdvertType = GetAdvertFileType(strFileType),
                            });
                        }
                        #endregion

                        intErrCode = 0;
                    }
                }
                else
                {
                    intErrCode = 1;
                }
            }
            catch
            {
                intErrCode = 3;// 导入失败
            }
            finally
            {
                AdvertList = AdvertList_Temp;
            }
            return intErrCode;
        }

        /// <summary>
        /// 导入广告文件信息
        /// </summary>
        /// <param name="goodsListInfo">列表的JSON格式</param>
        /// <param name="importType">导入方式 0：本地导入 1：远程下载</param>
        /// <param name="picSourcePath">本地导入时的来源路径</param>
        /// <returns>结果 0：成功 1：获取导入信息失败 2：没有要导入的信息 3：导入失败</returns>
        public int ImportAdvertList(string advertListInfo, string importType, string sourcePath, out string updateList)
        {
            int intErrCode = 0;
            updateList = string.Empty;

            DbOper dbOper = new DbOper();
            dbOper.DbFileName = _m_DbFileName;

            try
            {
                AdvertList_Import.Clear();

                JsonOper jsonOper = new JsonOper();
                string strResultCode = jsonOper.GetJsonKeyValue(advertListInfo, "ret");
                string strisupdate = jsonOper.GetJsonKeyValue(advertListInfo, "isupdate");
                if ((strResultCode == "0") && (strisupdate == "1"))
                {
                    #region
                    int intCount = 0;
                    intCount = Convert.ToInt32(jsonOper.GetJsonKeyValue(advertListInfo, "advcount"));
                    string strAdvListID = jsonOper.GetJsonKeyValue(advertListInfo, "advlistid");
                    string strUpdateSign = jsonOper.GetJsonKeyValue(advertListInfo, "updatesign");
                    string strPlaySign = jsonOper.GetJsonKeyValue(advertListInfo, "playsign");
                    string strBeginDate = jsonOper.GetJsonKeyValue(advertListInfo, "begindate");
                    string strEndDate = jsonOper.GetJsonKeyValue(advertListInfo, "enddate");
                    if (intCount == 0)
                    {
                        intErrCode = 2;// 没有要导入的信息
                    }
                    else if (strAdvListID == m_UpdateAdvertListID)
                    {
                        // 当前要导入的广告节目单编号和正在更新的广告节目单编号一样
                        intErrCode = 2;// 没有要导入的信息
                    }
                    else
                    {
                        // 检查以广告节目单编号为名称的文件夹是否存在，如果不存在，自动创建
                        CreateDirectory(strAdvListID);

                        string strSql = string.Empty;
                        bool result = false;

                        #region 操作广告节目单表

                        dbOper.ConnType = ConnectType.KeepConn;
                        strSql = @"select FileCode 
                            from T_ADV_ADVFILE where AdvListID = '" + strAdvListID + "'";

                        DataSet dataSet = dbOper.dataSet(strSql);
                        bool blnIsExist = false;// 是否已经存在该节目单 False：不存在 True：存在
                        if (dataSet.Tables.Count > 0)
                        {
                            if (dataSet.Tables[0].Rows.Count > 0)
                            {
                                // 已经存在该广告节目单，更新
                                blnIsExist = true;
                            }
                        }
                        if (blnIsExist)
                        {
                            // 更新
                            strSql = "update T_ADV_ADVLIST set UpdateSign = '" + strUpdateSign + "',PlaySign = '" + 
                                strPlaySign + "',TotalNum = '" + intCount + "',BeginDate = '" + strBeginDate +
                                "',EndDate = '" + strEndDate + "',Content = '" + advertListInfo + "',ImportType = '" + importType + "' where AdvListID = '" + strAdvListID + "'";
                        }
                        else
                        {
                            // 添加
                            strSql = @"insert into T_ADV_ADVLIST(AdvListID,UpdateSign,PlaySign,TotalNum,BeginDate,EndDate,Content,ImportType) values('" + strAdvListID + "','" + strUpdateSign + "','" + strPlaySign + "','" + intCount +
                                                  "','" + strBeginDate + "','" + strEndDate + "','" + advertListInfo + "','" + importType + "')";
                        }

                        result = dbOper.excuteSql(strSql);
                        if (!result)
                        {
                            intErrCode = 3;
                        }

                        #endregion

                        if (result)
                        {
                            JsonData jdItems = null;

                            jdItems = JsonMapper.ToObject(advertListInfo);
                            jdItems = jdItems["filelist"];

                            #region 遍历导入信息

                            string strAdvCode = string.Empty;
                            string strFileName = string.Empty;
                            string strFileFormat = string.Empty;
                            string strSize = string.Empty;
                            string strPlayNum = string.Empty;
                            string strDelayTime = string.Empty;
                            string strUrl = string.Empty;
                            string strSource = string.Empty;
                            int intNo = 0;
                            string strFactImportType = string.Empty;
                            foreach (JsonData item in jdItems)
                            {
                                intNo++;
                                strAdvCode = item["advcode"].ToString();
                                strFileName = item["name"].ToString();
                                strFileFormat = item["format"].ToString();
                                strSize = item["size"].ToString();
                                strPlayNum = item["playnum"].ToString();
                                strDelayTime = item["delaytime"].ToString();
                                strUrl = item["url"].ToString();
                                strSource = item["source"].ToString();
                                if (importType == "0")
                                {
                                    strFactImportType = "0";
                                }
                                else
                                {
                                    strFactImportType = strSource;
                                }

                                // 保存表记录
                                strSql = @"insert into T_ADV_ADVFILE(AdvListID,FileCode,FileName,FileFormat,
                                                FileType,FileSize,PlayNum,DelayTime,DownUrl,Status,ImportType)    
                                        values('" + strAdvListID + "','" + strAdvCode + "','" + strFileName + "','" + strFileFormat +
                                                  "','1','" + strSize + "','" + strPlayNum + "','" + strDelayTime + "','" + strUrl +
                                                  "','0','" + strFactImportType + "')";

                                result = dbOper.excuteSql(strSql);

                                ////AdvertList_Import.Add(new AdvertInfoModel()
                                ////{
                                ////    PlayNo = intNo - 1,
                                ////    FileName = strFileName,
                                ////    FileType = strFileType,
                                ////    AdvertType = GetAdvertFileType(strFileType),
                                ////});
                            }
                            #endregion

                            if (intNo == 0)
                            {
                                intErrCode = 2;// 没有要导入的信息
                            }
                            else
                            {
                                m_UpdateAdvertListID = strAdvListID;
                            }
                        }
                    }
                    #endregion
                }
                else if ((strResultCode == "0") && (strisupdate == "0"))
                {
                    intErrCode = 2;
                }
                else
                {
                    intErrCode = 1;
                }

                dbOper.closeConnection();
            }
            catch
            {
                dbOper.closeConnection();
                intErrCode = 3;// 导入失败
            }
            return intErrCode;
        }

        /// <summary>
        /// 根据带有文件格式的文件名称获取该文件所属广告类型（视频或图片）
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public Enum.BusinessEnum.AdvertType GetFileAdvertType(string fileName)
        {
            try
            {
                int intLen = fileName.IndexOf('.');
                if (intLen > 0)
                {
                    string strFileType = fileName.Substring(intLen + 1);
                    return GetAdvertFileType(strFileType);
                }
                else
                {
                    return Enum.BusinessEnum.AdvertType.Image;
                }
            }
            catch
            {
                return Enum.BusinessEnum.AdvertType.Image;
            }
        }

        #endregion

        #region 私有流程函数

        /// <summary>
        /// 检测广告节目单是否需要更新
        /// </summary>
        /// <returns></returns>
        private bool CheckAdvUpdate(out string advertListInfo)
        {
            bool result = false;
            advertListInfo = string.Empty;

            result = m_SoftTermUpdateOper.CheckAdvUpdate(m_UpdateAdvertListID, out advertListInfo);

            return result;
        }

        /// <summary>
        /// 远程下载广告文件
        /// </summary>
        /// <param name="advertInfo"></param>
        /// <returns></returns>
        private bool DownAdvertFile(AdvertInfoModel advertInfo)
        {
            string strFileLocalPath = AppDomain.CurrentDomain.BaseDirectory.ToString() + "advert\\" + advertInfo.AdvListID + "\\" +
                advertInfo.FileName + "." + advertInfo.FileType;

            // 检测目录下是否有需要下载的正式文件
            if (System.IO.File.Exists(strFileLocalPath))
            {
                // 有正式的文件，无需下载
                return true;
            }

            // 打开上次下载的文件或新建文件 
            long lStartPos = 0;
            System.IO.FileStream fs;

            // 获取本地文件的存储位置目录以及文件名称（带文件格式）
            string strFileName = advertInfo.FileName;
            string strFilePath = advertInfo.FileName + "." + advertInfo.FileType;

            if ((string.IsNullOrEmpty(strFileName)) ||
                (string.IsNullOrEmpty(strFilePath)))
            {
                return false;
            }

            // 生成下载临时文件
            string strFileTemp = strFileLocalPath + ".tmp";

            if (System.IO.File.Exists(strFileTemp))
            {
                fs = System.IO.File.OpenWrite(strFileTemp);
                lStartPos = fs.Length;
                fs.Seek(lStartPos, System.IO.SeekOrigin.Current); //移动文件流中的当前指针 
            }
            else
            {
                fs = new System.IO.FileStream(strFileTemp, System.IO.FileMode.Create);
                lStartPos = 0;
            }

            //打开网络连接 
            bool uploadResult = false;

            try
            {
                System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(advertInfo.DownUrl);
                if (lStartPos > 0)
                {
                    request.AddRange((int)lStartPos); //设置Range值
                }

                //向服务器请求，获得服务器回应数据流 
                System.IO.Stream ns = request.GetResponse().GetResponseStream();

                byte[] nbytes = new byte[1024];
                int nReadSize = 0;
                nReadSize = ns.Read(nbytes, 0, 1024);
                while (nReadSize > 0)
                {
                    fs.Write(nbytes, 0, nReadSize);
                    nReadSize = ns.Read(nbytes, 0, 1024);
                }
                ns.Close();

                uploadResult = true;

                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                fs.Close();

                // 下载成功，把临时文件修改成正式文件
                if (uploadResult)
                {
                    Thread.Sleep(200);
                    try
                    {
                        if (System.IO.File.Exists(strFileTemp))
                        {
                            System.IO.File.Copy(strFileTemp, strFileLocalPath, true);
                            Thread.Sleep(200);
                            System.IO.File.Delete(strFileTemp);
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }

        #endregion

        #region 私有业务函数

        private Enum.BusinessEnum.AdvertType GetAdvertFileType(string advertType)
        {
            string strAdvertType = advertType.ToLower();
            if ((strAdvertType == "jpg") ||
                (strAdvertType == "jpeg") ||
                (strAdvertType == "bmp") ||
                (strAdvertType == "png"))
            {
                return Enum.BusinessEnum.AdvertType.Image;
            }
            if ((strAdvertType == "mp4") || (strAdvertType == "wmv"))
            {
                return Enum.BusinessEnum.AdvertType.Video;
            }
            return Enum.BusinessEnum.AdvertType.Image;
        }

        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="dire"></param>
        /// <returns></returns>
        private bool CreateDirectory(string dire)
        {
            bool result = false;
            try
            {
                string strPath = AppDomain.CurrentDomain.BaseDirectory.ToString() + "advert\\" + dire + "\\";
                if (!Directory.Exists(strPath))
                {
                    // 不存在，创建
                    Directory.CreateDirectory(strPath);
                }
                result = true;
            }
            catch
            {
                result = false;
            }
            return result;
        }

        #endregion
    }
}
