using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Threading;
using System.IO;

namespace AutoSellGoodsMachine
{
    /// <summary>
    /// FrmAdvertImport.xaml 的交互逻辑
    /// </summary>
    public partial class FrmAdvertImport : Window
    {
        public FrmAdvertImport()
        {
            InitializeComponent();
            InitForm();
        }

        private void InitForm()
        {
            tbTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Advert_Import");
            tbInfo.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Advert_ImportAsk");
            btnNo.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_No");
            btnYes.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Yes");
        }

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            btnYes.Visibility = btnNo.Visibility = System.Windows.Visibility.Hidden;
            tbInfo.Text = PubHelper.p_LangOper.GetStringBundle("Pub_OperProgress");

            // 启动一个工作线程
            Thread TrdImport = new Thread(new ThreadStart(ImportTrd));
            TrdImport.IsBackground = true;
            TrdImport.Start();     
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 广告导入工作线程
        /// </summary>
        private void ImportTrd()
        {
            /* 广告导入处理流程
             * 1、搜索移动盘根目录下的广告配置文件，并读取广告配置文件
               2、根据广告配置文件，检查相关广告文件是否存在及格式是否正确；
             * 3、拷贝复制配置文件以及广告播放文件到指定目录
               3、如果成功，则清空原有广告播放文件
               4、拷贝新的文件到广告正式播放目录下
             * 
            */

            int intErrCode = 0;

            try
            {
                this.tbTitle.Dispatcher.Invoke(new Action(() =>
                {
                    tbInfo.Text = PubHelper.p_LangOper.GetStringBundle("Pub_Import_Checking");// 正在读取导入文件
                    DispatcherHelper.SleepControl();
                    DispatcherHelper.DoEvents();
                }));

                #region 遍历各盘符，查询是否有广告信息导入文件

                string strDiskPath = "d;e;f;g;h;i;j;k";
                string[] hexDiskPath = strDiskPath.Split(';');
                int hexDiskLen = hexDiskPath.Length;
                string strUDiskFilePath = string.Empty;
                string strFilaPath = string.Empty;

                string strImportFileName = "advertlist.txt";

                bool blnFileExist = false;
                for (int i = 0; i < hexDiskLen; i++)
                {
                    strUDiskFilePath = hexDiskPath[i] + ":\\";
                    strFilaPath = strUDiskFilePath + strImportFileName;
                    // 检测广告导入文件是否存在
                    if (File.Exists(strFilaPath))
                    {
                        // 导入文件存在                    
                        blnFileExist = true;
                        break;
                    }
                }

                #endregion

                // 0：成功 1：没有找到商品信息导入文件 2：读取商品信息导入文件失败 3：导入失败
                if (blnFileExist)
                {
                    #region 读取导入信息文件内容

                    string strListInfo = File.ReadAllText(strFilaPath, Encoding.Default);

                    // 0：成功 1：获取导入信息失败 2：没有要导入的信息 3：导入失败
                    string updateMcdList = string.Empty;
                    intErrCode = PubHelper.p_BusinOper.AdvertOper.ImportAdvertList(strListInfo, "0",
                        strUDiskFilePath, out updateMcdList);
                    if (intErrCode != 0)
                    {
                        intErrCode = intErrCode + 2;
                    }
                    else
                    {
                        #region 读取信息成功，检查相关广告文件

                        int intCount = PubHelper.p_BusinOper.AdvertOper.AdvertList_Import.Count;
                        if (intCount == 0)
                        {
                            intErrCode = 4;// 没有广告信息
                        }
                        else
                        {
                            // 广告文件存储临时文件夹
                            string strAdvertDicPath_Temp = AppDomain.CurrentDomain.BaseDirectory.ToString() + "Advert_Temp\\";
                            // 广告文件存储正式文件夹
                            string strAdvertDicPath_Formal = AppDomain.CurrentDomain.BaseDirectory.ToString() + "Advert\\";

                            #region 如果广告临时文件夹不存在，则创建

                            bool result = PubHelper.CreatDire(strAdvertDicPath_Temp);

                            #endregion

                            #region 计算空间大小

                            this.tbTitle.Dispatcher.Invoke(new Action(() =>
                            {
                                tbInfo.Text = PubHelper.p_LangOper.GetStringBundle("Pub_Import_Disking");// 正在计算空间大小
                                DispatcherHelper.SleepControl();
                                DispatcherHelper.DoEvents();
                            }));

                            string strAdvertFileName = string.Empty;
                            string strAdvertFileType = string.Empty;
                            string strAdvertFileNameType = string.Empty;
                            string strImportFilePath = string.Empty;
                            FileInfo fileInfo;
                            long fileCountSize = 0;

                            for (int i = 0; i < intCount; i++)
                            {
                                strAdvertFileName = PubHelper.p_BusinOper.AdvertOper.AdvertList_Import[i].FileName;
                                strAdvertFileType = PubHelper.p_BusinOper.AdvertOper.AdvertList_Import[i].FileType;
                                strAdvertFileNameType = strAdvertFileName + "." + strAdvertFileType;
                                strImportFilePath = strUDiskFilePath + strAdvertFileNameType;
                                // 检测该广告文件是否存在
                                if (File.Exists(strImportFilePath))
                                {
                                    // 广告文件存在
                                    fileInfo = new FileInfo(strImportFilePath);// 获取该广告文件大小
                                    fileCountSize += fileInfo.Length;
                                }
                            }

                            #endregion

                            if (fileCountSize * 10 > PubHelper.p_BusinOper.CheckDataOper.GetCurrentDiskSpaceSize())
                            {
                                // 硬盘空间不足
                                intErrCode = 5;
                            }
                            else
                            {
                                #region 拷贝广告文件到临时文件夹

                                string strOperInfo = PubHelper.p_LangOper.GetStringBundle("Pub_Import_Opering");// 正在处理导入文件
                                for (int i = 0; i < intCount; i++)
                                {
                                    strAdvertFileName = PubHelper.p_BusinOper.AdvertOper.AdvertList_Import[i].FileName;
                                    strAdvertFileType = PubHelper.p_BusinOper.AdvertOper.AdvertList_Import[i].FileType;
                                    strAdvertFileNameType = strAdvertFileName + "." + strAdvertFileType;
                                    strImportFilePath = strUDiskFilePath + strAdvertFileNameType;
                                    // 检测该广告文件是否存在
                                    if (File.Exists(strImportFilePath))
                                    {
                                        this.tbTitle.Dispatcher.Invoke(new Action(() =>
                                        {
                                            tbInfo.Text = strOperInfo.Replace("{N}", strAdvertFileNameType);// 正在处理导入文件

                                            DispatcherHelper.SleepControl();
                                            DispatcherHelper.DoEvents();
                                        }));

                                        // 广告文件存在 拷贝广告文件到临时文件夹
                                        File.Copy(strImportFilePath,
                                                strAdvertDicPath_Temp + strAdvertFileNameType, true);
                                    }
                                }

                                File.Copy(strFilaPath, strAdvertDicPath_Temp + strImportFileName, true);

                                #endregion

                                #region 清除历史广告文件

                                this.tbTitle.Dispatcher.Invoke(new Action(() =>
                                {
                                    tbInfo.Text = PubHelper.p_LangOper.GetStringBundle("Pub_Import_Clearing");// 正在清除历史文件
                                    DispatcherHelper.SleepControl();
                                    DispatcherHelper.DoEvents();
                                }));

                                DirectoryInfo di = new DirectoryInfo(strAdvertDicPath_Formal);
                                // 判断目录是否存在
                                FileInfo[] fi = di.GetFiles();//获得目录下文件
                                foreach (FileInfo f in fi)
                                {
                                    //判断指定文件是否存在
                                    File.Delete(f.FullName);
                                }

                                #endregion

                                #region 从临时文件夹把新的广告文件移动到正式广告文件夹里

                                DirectoryInfo di_Temp = new DirectoryInfo(strAdvertDicPath_Temp);
                                // 判断目录是否存在
                                FileInfo[] fi_Temp = di_Temp.GetFiles();//获得目录下文件
                                foreach (FileInfo f in fi_Temp)
                                {
                                    //判断指定文件是否存在
                                    f.MoveTo(strAdvertDicPath_Formal + f.Name);
                                }

                                #endregion

                                PubHelper.p_BusinOper.AdvertOper.AdvertList = PubHelper.p_BusinOper.AdvertOper.AdvertList_Import;
                            }
                        }
                        #endregion

                    }

                    #endregion
                }
                else
                {
                    intErrCode = 1;// 没有找到信息导入文件
                }
            }
            catch
            {
                intErrCode = 2;// 认为读取导入文件失败
            } 

            string strMsgInfo = string.Empty;
            switch (intErrCode)
            {
                case 0:// 导入成功
                    strMsgInfo = PubHelper.p_LangOper.GetStringBundle("Pub_OperSuc");
                    break;
                case 1:// 没有找到信息导入文件
                    strMsgInfo = PubHelper.p_LangOper.GetStringBundle("Err_Import_NoFile");
                    break;
                case 2:// 读取信息导入文件失败
                    strMsgInfo = PubHelper.p_LangOper.GetStringBundle("Err_Import_ReadFileFail");
                    break;
                case 3:// 获取导入信息失败
                    strMsgInfo = PubHelper.p_LangOper.GetStringBundle("Err_Import_GetInfoFail");
                    break;
                case 4:// 没有要导入的信息
                    strMsgInfo = PubHelper.p_LangOper.GetStringBundle("Err_Import_NoInfo");
                    break;
                case 5:// 硬盘空间不足
                    strMsgInfo = PubHelper.p_LangOper.GetStringBundle("Err_Import_NoSpace").Replace("{N}", PubHelper.p_BusinOper.CheckDataOper.GetAppDisk());
                    break;
                default:// 导入失败
                    strMsgInfo = PubHelper.p_LangOper.GetStringBundle("Pub_OperFail");
                    break;
            }

            this.tbTitle.Dispatcher.Invoke(new Action(() =>
            {
                PubHelper.ShowMsgInfo(strMsgInfo, PubHelper.MsgType.Ok);
                this.Close();
            }));
        }
    }
}
