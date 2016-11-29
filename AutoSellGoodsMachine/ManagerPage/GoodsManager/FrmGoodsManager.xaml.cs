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

using AutoSellGoodsMachine.Controls;
using Business.Model;

namespace AutoSellGoodsMachine
{
    /// <summary>
    /// FrmGoodsManager.xaml 的交互逻辑
    /// </summary>
    public partial class FrmGoodsManager : Window
    {
        private GoodsButton m_CurrentGoods = null;

        private int m_GoodsCount = 0;

        private int m_PageCount = 0;

        private int m_CurrentPage = 1;

        ////private int m_EachPageNum = 15;

        private int m_EachRowNum = 5;// 每行显示的最大数量

        private int m_MaxRowNum = 4;// 最大行数

        public FrmGoodsManager()
        {
            InitializeComponent();

            InitForm();

            Loaded += (MainWindow_Loaded);
        }

        private void InitForm()
        {
            tbTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Goods");
            btnPrevious.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Page_Preview");
            btnDown.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Page_Next");
            btnView.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_View");
            btnCancel.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Cancel");
            btnImport.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Import");
            btnClearGoods.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_ClearGoods");
            btnDelete.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Del");
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadPage();
        }

        private void LoadPage()
        {
            m_GoodsCount = PubHelper.p_BusinOper.GoodsOper.GoodsList_Total.Count;

            if (m_GoodsCount == 0)
            {
                btnPrevious.IsEnabled = btnDown.IsEnabled = btnClearGoods.IsEnabled = false;
            }
            else
            {
                btnClearGoods.IsEnabled = true;
            }

            btnView.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnImport.IsEnabled = true;
            btnCancel.IsEnabled = true;

            InitProductPage();

            InitCurrentPageProduct(m_CurrentPage);

            if (m_GoodsCount > 0)
            {
                ButtonEnable();
            }
        }

        #region 初始化页数

        /// <summary>
        /// 初始化页数
        /// </summary>
        private void InitProductPage()
        {
            int intNum = m_EachRowNum * m_MaxRowNum;
            m_PageCount = ((m_GoodsCount / intNum) +
                ((m_GoodsCount % intNum) > 0 ? 1 : 0));

            tbCountPage.Text = m_PageCount.ToString();
        }

        #endregion

        #region 创建商品控件
        /// <summary>
        /// 创建商品控件
        /// </summary>
        private void CreateProduct(List<GoodsModel> products)
        {
            panelProduct.Children.Clear();

            int index = 0;

            for (int i = 0; i < m_MaxRowNum; i++)
            {
                for (int j = 0; j < m_EachRowNum; j++)
                {
                    if (index < products.Count)
                    {
                        GoodsButton productControl = new GoodsButton()
                        {
                            HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                            VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                            Margin = new Thickness(5)
                        };

                        Grid.SetRow(productControl, i);
                        Grid.SetColumn(productControl, j);
                        panelProduct.Children.Add(productControl);

                        productControl.MouseLeftButtonUp += (GoodsWayChecked);

                        productControl.SetCurrentGoods(products[index]);

                        productControl.SetOneText(products[index].McdCode);
                        productControl.SetSecondText(products[index].McdName);
                    }
                    index++;
                }
            }
        }

        /// <summary>
        /// 产品选中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GoodsWayChecked(object sender, MouseButtonEventArgs e)
        {
            var goodsWay = (sender as GoodsButton);
            if (goodsWay == m_CurrentGoods)
                return;
            if (goodsWay != null)
            {
                if (m_CurrentGoods != null)
                {
                    m_CurrentGoods.ToNormal();
                }
                goodsWay.ToCheck();
                m_CurrentGoods = goodsWay;

                btnView.IsEnabled = btnDelete.IsEnabled = true;
            }
        }

        #endregion

        #region 上一页 下一页

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            InitCurrentPageProduct(m_CurrentPage + 1);
            ButtonEnable();
        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            InitCurrentPageProduct(m_CurrentPage - 1);
            ButtonEnable();
        }

        /// <summary>
        /// 按钮状态控制
        /// </summary>
        private void ButtonEnable()
        {
            if (m_PageCount == 1)
            {
                btnPrevious.IsEnabled = btnDown.IsEnabled = false;
            }
            else
            {
                if (m_CurrentPage == 1)
                {
                    btnPrevious.IsEnabled = false;
                    btnDown.IsEnabled = true;
                }
                else if (m_CurrentPage == m_PageCount)
                {
                    btnPrevious.IsEnabled = true;
                    btnDown.IsEnabled = false;
                }
                else
                {
                    btnPrevious.IsEnabled = true;
                    btnDown.IsEnabled = true;
                }
            }
        }

        /// <summary>
        /// 初始化当前页的商品
        /// </summary>
        /// <param name="i"></param>
        private void InitCurrentPageProduct(int i)
        {
            m_CurrentPage = i;

            tbCurrentPage.Text = i.ToString();

            int intNum = m_MaxRowNum * m_EachRowNum;
            int startIndex = (i - 1) * intNum;

            int endIndex = i * intNum;

            List<GoodsModel> currentSource = new List<GoodsModel>();

            for (int index = 0; index < m_GoodsCount; index++)
            {
                if (index >= startIndex && index < endIndex)
                {
                    currentSource.Add(PubHelper.p_BusinOper.GoodsOper.GoodsList_Total[index]);
                }
            }

            CreateProduct(currentSource);
        }
        #endregion

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            PubHelper.p_BusinOper.GoodsOper.CurrentGoods = null;
            this.Close();
        }

        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            if (m_CurrentGoods.CurrentGoods != null)
            {
                PubHelper.p_BusinOper.GoodsOper.CurrentGoods = m_CurrentGoods.CurrentGoods;
                ////this.Close();
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (m_CurrentGoods.CurrentGoods != null)
            {
                string strAskInfo = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_GoodsManager_DeleteAsk");

                PubHelper.ShowMsgInfo(strAskInfo, PubHelper.MsgType.YesNo);
                if (PubHelper.p_MsgResult)
                {
                    bool blnPrevious = false;
                    bool blnNext = false;

                    blnPrevious = btnPrevious.IsEnabled;
                    blnNext = btnDown.IsEnabled;

                    btnImport.IsEnabled = btnClearGoods.IsEnabled = btnCancel.IsEnabled = btnView.IsEnabled =
                        btnDelete.IsEnabled = btnPrevious.IsEnabled = btnDown.IsEnabled = false;

                    string strMsgInfo = string.Empty;
                    int intErrCode = PubHelper.p_BusinOper.GoodsOper.DeleteGoods(m_CurrentGoods.CurrentGoods.McdCode);
                    switch (intErrCode)
                    {
                        case 0:// 删除成功

                            LoadPage();

                            PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperSuc"), PubHelper.MsgType.Ok);
                            break;
                        case 1:// 有货道上架了该商品
                            PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_GoodsManager_Delete_Put"), PubHelper.MsgType.Ok);
                            break;
                        default:// 失败
                            PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperFail"), PubHelper.MsgType.Ok);
                            break;
                    }

                    if (intErrCode != 0)
                    {
                        btnImport.IsEnabled = btnCancel.IsEnabled = true;
                        btnPrevious.IsEnabled = blnPrevious;
                        btnDown.IsEnabled = blnNext;
                        btnDelete.IsEnabled = btnView.IsEnabled = true;
                        btnClearGoods.IsEnabled = true;
                    }
                }
            }
            else
            {
                PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_GoodsManager_Delete_NoSelect"), PubHelper.MsgType.Ok);
            }
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            string strAskInfo = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_GoodsManager_ImportAsk");

            PubHelper.ShowMsgInfo(strAskInfo, PubHelper.MsgType.YesNo);
            if (PubHelper.p_MsgResult)
            {
                Thread TrdImportGoods = new Thread(new ThreadStart(ImportGoodsTrd));
                TrdImportGoods.IsBackground = true;
                TrdImportGoods.Start();
            }
        }

        /// <summary>
        /// 本地导入商品信息线程
        /// </summary>
        private void ImportGoodsTrd()
        {
            int intErrCode = 0;

            bool blnPrevious = false;
            bool blnNext = false;
            bool blnView = false;
            this.tbTitle.Dispatcher.Invoke(new Action(() =>
            {
                // 记录操作前按钮的使能状态
                blnPrevious = btnPrevious.IsEnabled;
                blnNext = btnDown.IsEnabled;
                blnView = btnView.IsEnabled;
                btnImport.IsEnabled = btnCancel.IsEnabled = btnPrevious.IsEnabled = 
                    btnDown.IsEnabled = btnView.IsEnabled = btnClearGoods.IsEnabled = btnDelete.IsEnabled = false;
            }));

            try
            {
                #region 遍历各盘符，查询是否有商品信息导入文件

                string strDiskPath = "d;e;f;g;h;i;j;k";
                string[] hexDiskPath = strDiskPath.Split(';');
                int hexDiskLen = hexDiskPath.Length;
                string strUDiskFilePath = string.Empty;
                string strGoodsFilaPath = string.Empty;

                // 遍历各盘符，查询是否有商品信息导入文件
                string strGoodsFileName = "goodsimport.txt";

                bool blnFileExist = false;
                for (int i = 0; i < hexDiskLen; i++)
                {
                    strUDiskFilePath = hexDiskPath[i] + ":\\";
                    strGoodsFilaPath = strUDiskFilePath + strGoodsFileName;
                    // 检测商品导入文件是否存在
                    if (File.Exists(strGoodsFilaPath))
                    {
                        // 商品导入文件存在                    
                        blnFileExist = true;
                        break;
                    }
                }

                // 0：成功 1：没有找到商品信息导入文件 2：读取商品信息导入文件失败 3：导入失败

                if (blnFileExist)
                {
                    #region 读取导入信息文件内容

                    string strGoodsListInfo = File.ReadAllText(strGoodsFilaPath, Encoding.Default);

                    // 0：成功 1：获取商品导入信息失败 2：没有要导入的商品信息 3：导入失败
                    string updateMcdList = string.Empty;
                    intErrCode = PubHelper.p_BusinOper.GoodsOper.ImportGoodsList(strGoodsListInfo, "0",
                        strUDiskFilePath, out updateMcdList);
                    if (intErrCode != 0)
                    {
                        intErrCode = intErrCode + 2;
                    }
                    else
                    {
                        // 导入成功，更新当前所有商品信息列表及货道信息列表
                        if (!string.IsNullOrEmpty(updateMcdList))
                        {
                            PubHelper.p_IsRefreshAsile = true;
                            for (int i = 0; i < PubHelper.p_BusinOper.GoodsOper.GoodsList_Total.Count; i++)
                            {
                                if (updateMcdList.IndexOf(PubHelper.p_BusinOper.GoodsOper.GoodsList_Total[i].McdCode + ",") >= 0)
                                {
                                    for (int j = 0; j < PubHelper.p_BusinOper.AsileOper.AsileList.Count; j++)
                                    {
                                        if (PubHelper.p_BusinOper.AsileOper.AsileList[j].McdCode == PubHelper.p_BusinOper.GoodsOper.GoodsList_Total[i].McdCode)
                                        {
                                            PubHelper.p_BusinOper.AsileOper.AsileList[j].McdName = PubHelper.p_BusinOper.GoodsOper.GoodsList_Total[i].McdName;
                                            PubHelper.p_BusinOper.AsileOper.AsileList[j].McdContent = PubHelper.p_BusinOper.GoodsOper.GoodsList_Total[i].McdContent;
                                            PubHelper.p_BusinOper.AsileOper.AsileList[j].McdPicName = PubHelper.p_BusinOper.GoodsOper.GoodsList_Total[i].PicName;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    #endregion
                }
                else
                {
                    intErrCode = 1;// 没有找到信息导入文件
                }

                #endregion
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
                case 3:// 获取商品导入信息失败
                    strMsgInfo = PubHelper.p_LangOper.GetStringBundle("Err_Import_GetInfoFail");
                    break;
                case 4:// 没有要导入的商品信息
                    strMsgInfo = PubHelper.p_LangOper.GetStringBundle("Err_Import_NoInfo");
                    break;
                default:// 导入失败
                    strMsgInfo = PubHelper.p_LangOper.GetStringBundle("Pub_OperFail");
                    break;
            }

            this.tbTitle.Dispatcher.Invoke(new Action(() =>
            {
                if (intErrCode == 0)
                {
                    LoadPage();
                }
                else
                {
                    btnImport.IsEnabled = btnCancel.IsEnabled = true;
                    btnPrevious.IsEnabled = blnPrevious;
                    btnDown.IsEnabled = blnNext;
                    btnView.IsEnabled = blnView;
                    btnDelete.IsEnabled = false;
                    if (m_GoodsCount > 0)
                    {
                        btnClearGoods.IsEnabled = true;
                    }
                }
                PubHelper.ShowMsgInfo(strMsgInfo, PubHelper.MsgType.Ok);
            }));
        }

        private void btnClearGoods_Click(object sender, RoutedEventArgs e)
        {
            string strAskInfo = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_GoodsManager_ClearAsk");

            PubHelper.ShowMsgInfo(strAskInfo, PubHelper.MsgType.YesNo);
            if (PubHelper.p_MsgResult)
            {
                Thread TrdClearGoods = new Thread(new ThreadStart(ClearGoodsTrd));
                TrdClearGoods.IsBackground = true;
                TrdClearGoods.Start();
            }
        }

        /// <summary>
        /// 清除商品信息
        /// </summary>
        private void ClearGoodsTrd()
        {
            int intErrCode = 0;

            bool blnPrevious = false;
            bool blnNext = false;
            bool blnView = false;
            this.tbTitle.Dispatcher.Invoke(new Action(() =>
            {
                // 记录操作前按钮的使能状态
                blnPrevious = btnPrevious.IsEnabled;
                blnNext = btnDown.IsEnabled;
                blnView = btnView.IsEnabled;
                btnImport.IsEnabled = btnCancel.IsEnabled = btnPrevious.IsEnabled =
                    btnDown.IsEnabled = btnView.IsEnabled = btnClearGoods.IsEnabled = btnDelete.IsEnabled = false;
            }));

            intErrCode = PubHelper.p_BusinOper.GoodsOper.ClearAllGoods();

            string strMsgInfo = string.Empty;
            switch (intErrCode)
            {
                case 0:// 操作成功
                    strMsgInfo = PubHelper.p_LangOper.GetStringBundle("Pub_OperSuc");
                    // 清空所有货道的商品信息
                    PubHelper.p_BusinOper.AsileOper.ClearAllAsileGoods();
                    PubHelper.p_IsRefreshAsile = true;
                    break;
                default:// 操作失败
                    strMsgInfo = PubHelper.p_LangOper.GetStringBundle("Pub_OperFail");
                    break;
            }

            this.tbTitle.Dispatcher.Invoke(new Action(() =>
            {
                if (intErrCode == 0)
                {
                    LoadPage();
                }
                else
                {
                    btnImport.IsEnabled = btnCancel.IsEnabled = true;
                    btnPrevious.IsEnabled = blnPrevious;
                    btnDown.IsEnabled = blnNext;
                    btnView.IsEnabled = blnView;
                    btnDelete.IsEnabled = false;
                    if (m_GoodsCount > 0)
                    {
                        btnClearGoods.IsEnabled = true;
                    }
                }
                PubHelper.ShowMsgInfo(strMsgInfo, PubHelper.MsgType.Ok);
            }));
        }
    }
}
