using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

using AutoSellGoodsMachine.Controls;
using Business.Model;

namespace AutoSellGoodsMachine
{
    /// <summary>
    /// FrmStock.xaml 的交互逻辑
    /// </summary>
    public partial class FrmStock : Window
    {
        private DispatcherTimer timer = new DispatcherTimer();

        /// <summary>
        /// 当前的货柜编号
        /// </summary>
        private string m_VendBoxCode = "1";

        private GoodsWayProduct currentGoodsWay = null;
        private int currentTrayIndex = 0;
        private string m_Title_NoGoodsTitle = string.Empty;
        private string m_Title_StockNum = string.Empty;
        private string m_Title_SpringNum = string.Empty;

        public FrmStock()
        {
            InitializeComponent();

            InitForm();

            Loaded += (MainWindow_Loaded);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //Tick是执行的事件           
            timer.Tick += new EventHandler(Timer1_Tick);
            //Internal是间隔时间
            timer.Interval = TimeSpan.FromSeconds(0.01);

            timer.Start();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            timer.Stop();

            m_VendBoxCode = "1";// 默认第一个柜子
            CreateBox();//
            ////CreateTray();
        }

        private void InitForm()
        {
            tbTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Stock");
            btnCancel.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Cancel");
            btnSave.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Save");

            tbBatch.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Stock_Batch");
            tbMaxValidDate.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Stock_MaxValidDate");
            tbProductDate.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Stock_ProductDate");

            tbFillUpNum.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Stock_FullNumTitle");

            btnTrayFull.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_TrayFull");
            btnTraySetOther.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_TraySetOther");

            m_Title_NoGoodsTitle = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Stock_NoGoods");
            m_Title_StockNum = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Stock_StockNum");
            m_Title_SpringNum = PubHelper.p_LangOper.GetStringBundle("Pub_SpringNum") +
                PubHelper.p_LangOper.GetStringBundle("Pub_Symbol");
        }

        /////// <summary>
        /////// 根据分辨率设置大小(该代码在这里为了在电脑上显示效果美化 实际终端请去掉)
        /////// </summary>
        ////private void SetSize()
        ////{
        ////    mainPanel.Width = 700;// this.ActualWidth * 0.9;
        ////}

        /// <summary>
        /// 创建货柜
        /// </summary>
        private void CreateBox()
        {
            var map = new Dictionary<int, string>();
            for (int i = 1; i < 10; i++)
            {
                map.Add(i, DictionaryHelper.Dictionary_VendBoxName(i.ToString()));
            }

            int intBoxCount = PubHelper.p_BusinOper.AsileOper.VendBoxList.Count;
            if (intBoxCount > 1)
            {
                #region 有多个柜子
                for (int i = 1; i <= intBoxCount; i++)
                {
                    panelBox.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                    var palletButton = new RadioButton()
                    {
                        Focusable = false,

                        HorizontalAlignment = System.Windows.HorizontalAlignment.Center,

                        VerticalAlignment = System.Windows.VerticalAlignment.Center,

                        Style = App.Current.Resources["VendBoxButtonStyle"] as Style,

                        Foreground = new SolidColorBrush(Colors.White),

                        Padding = new Thickness(20, 4, 20, 8),

                        FontSize = 22,

                        Tag = i
                    };

                    Grid.SetColumn(palletButton, i - 1);

                    palletButton.Checked += (VendBoxButtonChecked);

                    palletButton.Content = map[i];

                    panelBox.Children.Add(palletButton);

                    if (i == 1)
                    {
                        palletButton.IsChecked = true;
                    }
                }
                #endregion
            }
            else
            {
                // 只有一个柜子
                CreateTray();
            }
        }

        /// <summary>
        /// 创建托盘
        /// </summary>
        private void CreateTray()
        {
            var map = new Dictionary<int, string>();
            for (int i = 1; i < 11; i++)
            {
                map.Add(i, PubHelper.p_BusinOper.ChangeTray(i.ToString()));
            }

            int intVendTrayMaxNum = 0;
            if (PubHelper.p_BusinOper.AsileOper.VendBoxList.Count == 1)
            {
                // 只有一个柜子
                intVendTrayMaxNum = PubHelper.p_BusinOper.AsileOper.TrayMaxNum_Total;
            }
            else
            {
                // 多个柜子
                for (int i = 0; i < PubHelper.p_BusinOper.AsileOper.AsileList.Count; i++)
                {
                    if (PubHelper.p_BusinOper.AsileOper.AsileList[i].VendBoxCode == m_VendBoxCode)
                    {
                        if (PubHelper.p_BusinOper.AsileOper.AsileList[i].TrayIndex >= intVendTrayMaxNum)
                        {
                            intVendTrayMaxNum = PubHelper.p_BusinOper.AsileOper.AsileList[i].TrayIndex;
                        }
                    }
                }
            }

            panelTray.ColumnDefinitions.Clear();
            panelTray.Children.Clear();

            for (int i = 1; i <= intVendTrayMaxNum + 1; i++)
            {
                panelTray.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                var palletButton = new RadioButton()
                {
                    Focusable = false,

                    HorizontalAlignment = System.Windows.HorizontalAlignment.Center,

                    VerticalAlignment = System.Windows.VerticalAlignment.Center,

                    Style = App.Current.Resources["PalletButtonStyle"] as Style,

                    Foreground = new SolidColorBrush(Colors.White),

                    Padding = new Thickness(20, 8, 20, 8),

                    FontSize = 32,

                    Tag = i - 1
                };

                Grid.SetColumn(palletButton, i - 1);

                palletButton.Checked += (PalletButtonChecked);

                palletButton.Content = map[i];

                panelTray.Children.Add(palletButton);

                if (i == 1)
                {
                    palletButton.IsChecked = true;
                }
            }
        }

        /// <summary>
        /// 创建货道
        /// </summary>
        private void CreateAsile(List<AsileModel> products)
        {
            panelAsile.Children.Clear();

            int index = 0;

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    GoodsWayProduct productControl = new GoodsWayProduct()
                    {
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                        VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                        Margin = new Thickness(5)
                    };

                    Grid.SetRow(productControl, i);
                    Grid.SetColumn(productControl, j);
                    panelAsile.Children.Add(productControl);

                    productControl.MouseLeftButtonUp += (GoodsWayChecked);

                    if (index < products.Count)
                    {
                        productControl.SetCurrentGoodsWayProduct(products[index]);
                        productControl.SetOneText(products[index].PaCode +
                            "【" + m_Title_SpringNum + products[index].SpringNum + "】");
                        productControl.SetSecondText(m_Title_StockNum.Replace("{N}", products[index].SurNum.ToString()));
                        if (index == 0)
                        {
                            productControl.IsDefaultCheck(true);
                            GoodsWayChecked(productControl, null);
                        }
                    }
                    else
                    {
                        productControl.SetNoGoods(m_Title_NoGoodsTitle);
                    }
                    index++;
                }
            }

            if (products.Count == 0)
            {
                btnTrayFull.IsEnabled = false;
            }
            else
            {
                btnTrayFull.IsEnabled = true;
            }
        }

        /// <summary>
        /// 货柜选中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VendBoxButtonChecked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton)
            {
                currentTrayIndex = Convert.ToInt32((sender as RadioButton).Tag);
                m_VendBoxCode = currentTrayIndex.ToString();
                CreateTray();
            }
        }

        /// <summary>
        /// 托盘选中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PalletButtonChecked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton)
            {
                currentTrayIndex = Convert.ToInt32((sender as RadioButton).Tag);
                var currentRowProducts = PubHelper.p_BusinOper.AsileOper.AsileList.Where(item => item.TrayIndex == currentTrayIndex).ToList();
                currentRowProducts = currentRowProducts.Where(item => item.VendBoxCode == m_VendBoxCode).ToList();
                CreateAsile(currentRowProducts);
            }
        }

        /// <summary>
        /// 货道中的产品选中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GoodsWayChecked(object sender, MouseButtonEventArgs e)
        {
            var goodsWay = (sender as GoodsWayProduct);
            if (goodsWay == currentGoodsWay)
                return;
            if (goodsWay != null)
            {
                if (currentGoodsWay != null)
                {
                    currentGoodsWay.ToNormal();
                }
                goodsWay.ToCheck();
                currentGoodsWay = goodsWay;
            }

            if (currentGoodsWay.CurrentGoodsWayProduct != null)
            {
                bdFillUpNumArea.Background = new SolidColorBrush(Colors.White);
                ResetProductInventory(currentGoodsWay.CurrentGoodsWayProduct.SurNum);
                btnDelete.Source = new BitmapImage(new Uri(SkinHelper.FILEPATH_REDUCE_NORMAL));

                tbBatch_Value.Text = currentGoodsWay.CurrentGoodsWayProduct.PiCi;
                tbMaxValidDate_Value.Text = currentGoodsWay.CurrentGoodsWayProduct.MaxValidDate;
                tbProductDate_Value.Text = currentGoodsWay.CurrentGoodsWayProduct.ProductDate;
                tbBatch_Value.IsEnabled = tbMaxValidDate_Value.IsEnabled = tbProductDate_Value.IsEnabled = true;

                btnSave.IsEnabled = btnTrayFull.IsEnabled = true;

                int intSurNum = currentGoodsWay.CurrentGoodsWayProduct.SurNum;// 当前货道库存
                int intSpringNum = currentGoodsWay.CurrentGoodsWayProduct.SpringNum;// 当前货道弹簧圈数
                if (intSurNum < intSpringNum)
                {
                    #region 当前货道的库存小于货道弹簧圈数
                    if ((intSpringNum >= 1) && (intSurNum == 0))
                    {
                        btnAdd.Source = new BitmapImage(new Uri(SkinHelper.FILEPATH_ADD_NORMAL));
                        btnDelete.Source = new BitmapImage(new Uri(SkinHelper.FILEPATH_REDUCE_GRAY));
                        btnAdd.IsEnabled = true;
                        btnDelete.IsEnabled = false;
                    }
                    else
                    {
                        btnAdd.Source = new BitmapImage(new Uri(SkinHelper.FILEPATH_ADD_NORMAL));
                        btnDelete.Source = new BitmapImage(new Uri(SkinHelper.FILEPATH_REDUCE_NORMAL));
                        btnAdd.IsEnabled = btnDelete.IsEnabled = true;
                    }
                    #endregion
                }
                else
                {
                    btnAdd.Source = new BitmapImage(new Uri(SkinHelper.FILEPATH_ADD_GRAY));
                    btnDelete.Source = new BitmapImage(new Uri(SkinHelper.FILEPATH_REDUCE_NORMAL));
                    btnAdd.IsEnabled = false;
                    btnDelete.IsEnabled = true;
                }
            }
            else
            {
                bdFillUpNumArea.Background = new SolidColorBrush(Color.FromRgb(209,204,204));
                btnDelete.Source = new BitmapImage(new Uri(SkinHelper.FILEPATH_REDUCE_GRAY));
                btnAdd.Source = new BitmapImage(new Uri(SkinHelper.FILEPATH_ADD_GRAY));
                btnSave.IsEnabled = btnTrayFull.IsEnabled = false;
                tbBatch_Value.Text = tbMaxValidDate_Value.Text = tbProductDate_Value.Text = string.Empty;
                tbBatch_Value.IsEnabled = tbMaxValidDate_Value.IsEnabled = tbProductDate_Value.IsEnabled = false;
            }
        }

        /// <summary>
        /// 添加库存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddInventory(object sender, MouseButtonEventArgs e)
        {
            SetProductInventory(1);
        }

        /// <summary>
        /// 减少库存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteInventory(object sender, MouseButtonEventArgs e)
        {
            SetProductInventory(-1);
        }

        /// <summary>
        /// 重置产品库存到默认值
        /// </summary>
        /// <param name="i"></param>
        private void ResetProductInventory(int i)
        {
            if (currentGoodsWay != null)
            {
                currentGoodsWay.SetSecondText(m_Title_StockNum.Replace("{N}", i.ToString()));
            }
            tnProductInventory.Text = i.ToString();
            tnProductInventory.Tag = i;
        }

        /// <summary>
        /// 从当前库存添加或减少产品库存
        /// </summary>
        private void SetProductInventory(int i)
        {
            if (currentGoodsWay.CurrentGoodsWayProduct != null)
            {
                int intStockNum = Convert.ToInt32(tnProductInventory.Tag) + i;

                if (i == 1)
                {
                    // 增加库存
                    if (intStockNum > currentGoodsWay.CurrentGoodsWayProduct.SpringNum)
                    {
                        return;
                    }
                    btnDelete.IsEnabled = true;
                    btnDelete.Source = new BitmapImage(new Uri(SkinHelper.FILEPATH_REDUCE_NORMAL));
                    if (intStockNum == currentGoodsWay.CurrentGoodsWayProduct.SpringNum)
                    {
                        btnAdd.Source = new BitmapImage(new Uri(SkinHelper.FILEPATH_ADD_GRAY));
                        ////
                        btnAdd.IsEnabled = false;
                    }
                    else
                    {
                        btnAdd.Source = new BitmapImage(new Uri(SkinHelper.FILEPATH_ADD_NORMAL));
                        ////btnDelete.Source = new BitmapImage(new Uri(SkinHelper.FILEPATH_REDUCE_GRAY));
                        btnAdd.IsEnabled = true;
                    }
                }
                else
                {
                    // 减少库存，补货后数量不能小于1
                    if (intStockNum < 0)
                    {
                        return;
                    }
                    btnAdd.IsEnabled = true;
                    btnAdd.Source = new BitmapImage(new Uri(SkinHelper.FILEPATH_ADD_NORMAL));
                    if (intStockNum == 0)
                    {
                        btnDelete.IsEnabled = false;
                        btnDelete.Source = new BitmapImage(new Uri(SkinHelper.FILEPATH_REDUCE_GRAY));
                        ////
                    }
                    else
                    {
                        btnDelete.IsEnabled = true;
                        btnDelete.Source = new BitmapImage(new Uri(SkinHelper.FILEPATH_REDUCE_NORMAL));
                        ////btnAdd.Source = new BitmapImage(new Uri(SkinHelper.FILEPATH_ADD_GRAY));
                    }
                }

                ResetProductInventory(intStockNum);
            }
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 按钮—全托盘设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTraySetOther_Click(object sender, RoutedEventArgs e)
        {
            string strNum = tnProductInventory.Text;
            string strAskInfo = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Stock_TraySetAsk");
            strAskInfo = strAskInfo.Replace("{N}", PubHelper.p_BusinOper.ChangeTray((currentTrayIndex + 1).ToString()));
            strAskInfo = strAskInfo.Replace("{M}", strNum);

            PubHelper.ShowMsgInfo(strAskInfo, PubHelper.MsgType.YesNo);

            if (PubHelper.p_MsgResult)
            {
                SetTrayStockNum("1");
            }
        }

        /// <summary>
        /// 托盘补满
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTrayFull_Click(object sender, RoutedEventArgs e)
        {
            string strAskInfo = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Stock_TrayFillAsk");
            strAskInfo = strAskInfo.Replace("{N}", PubHelper.p_BusinOper.ChangeTray((currentTrayIndex + 1).ToString()));

            PubHelper.ShowMsgInfo(strAskInfo, PubHelper.MsgType.YesNo);

            if (PubHelper.p_MsgResult)
            {
                SetTrayStockNum("0");
            }
        }

        /// <summary>
        /// 设置托盘库存
        /// </summary>
        /// <param name="type">类型 0：托盘补满 1：按照设置的数量进行托盘设置</param>
        private void SetTrayStockNum(string type)
        {
            ControlSet(false);

            // 确定要补满
            string mcdCode = string.Empty;
            bool result = false;
            result = PubHelper.p_BusinOper.PutGoods(m_VendBoxCode, "2", type,
                currentTrayIndex.ToString(), tnProductInventory.Text, out mcdCode);
            if (result)
            {
                PubHelper.p_IsRefreshAsile = true;

                #region 更新该托盘内的所有货道库存

                int intAsileCount = panelAsile.Children.Count;
                for (int asileIndex = 0; asileIndex < intAsileCount; asileIndex++)
                {
                    GoodsWayProduct productControl = (GoodsWayProduct)panelAsile.Children[asileIndex];
                    if (productControl.CurrentGoodsWayProduct != null)
                    {
                        productControl.SetSecondText(m_Title_StockNum.Replace("{N}", productControl.CurrentGoodsWayProduct.SurNum.ToString()));
                    }
                }

                #endregion

                PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperSuc"), PubHelper.MsgType.Ok);
            }
            else
            {
                PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperFail"), PubHelper.MsgType.Ok);
            }

            ControlSet(true);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string strAskInfo = string.Empty;

            ControlSet(false);

            if (currentGoodsWay.CurrentGoodsWayProduct != null)
            {
                bool blnIsStock = false;// 库存是否发生改变 False:没有 True：有
                bool blnIsPiCi = false;// 生产批次、最大有效期、生产日期是否发生改变 False：没有 True：有

                string strPaCode = currentGoodsWay.CurrentGoodsWayProduct.PaCode;
                string strNum = tnProductInventory.Text;
                string strPiCi = tbBatch_Value.Text;
                string strMaxValidDate = tbMaxValidDate_Value.Text;
                string strProductDate = tbProductDate_Value.Text;

                if (currentGoodsWay.CurrentGoodsWayProduct.SurNum != Convert.ToInt32(strNum))
                {
                    // 库存改变
                    blnIsStock = true;
                }
                if ((currentGoodsWay.CurrentGoodsWayProduct.PiCi != strPiCi) ||
                    (currentGoodsWay.CurrentGoodsWayProduct.ProductDate != strProductDate) ||
                    (currentGoodsWay.CurrentGoodsWayProduct.MaxValidDate != strMaxValidDate))
                {
                    // 生产批次或者最大有效期、或者生产日期改变
                    blnIsPiCi = true;
                }

                if ((blnIsStock) || (blnIsPiCi))
                {
                    if ((blnIsStock) && (!blnIsPiCi))
                    {
                        // 只保存库存
                        strAskInfo = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Stock_AsileAsk");
                        strAskInfo = strAskInfo.Replace("{N}", strPaCode);
                        strAskInfo = strAskInfo.Replace("{K}", strNum);
                        PubHelper.ShowMsgInfo(strAskInfo, PubHelper.MsgType.YesNo);
                    }
                    if ((blnIsStock) && (blnIsPiCi))
                    {
                        // 库存和批次都保存
                        PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_Oper_Ask"), PubHelper.MsgType.YesNo);
                    }
                    if ((!blnIsStock) && (blnIsPiCi))
                    {
                        // 只保存批次
                        PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_Oper_Ask"), PubHelper.MsgType.YesNo);
                    }
                    if (PubHelper.p_MsgResult)
                    {
                        bool result = false;
                        string strMcdCode = string.Empty;
                        if (blnIsStock)
                        {
                            // 确定要补货
                            result = PubHelper.p_BusinOper.PutGoods(m_VendBoxCode,"1", "1", strPaCode, strNum,out strMcdCode);
                        }
                        if (blnIsPiCi)
                        {
                            // 确定要更改生产批次
                            result = PubHelper.p_BusinOper.UpdateAsileBatch(strPaCode, strPiCi,strProductDate,strMaxValidDate,out strMcdCode);
                        }

                        if (!result)
                        {
                            // 设置失败
                            PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperFail"), PubHelper.MsgType.Ok);
                        }
                        else
                        {
                            #region 保存商品补货数据到第三方

                            if ((blnIsPiCi) || (blnIsStock))
                            {
                                if (PubHelper.p_BusinOper.ConfigInfo.O2OTake_Switch == Business.Enum.BusinessEnum.ControlSwitch.Run)
                                {
                                    PubHelper.p_BusinOper.O2OServerOper.Report_Stock(strMcdCode, strNum, strPaCode, strProductDate, strMaxValidDate, strPiCi);
                                }
                            }

                            #endregion

                            PubHelper.p_IsRefreshAsile = true;
                            currentGoodsWay.SetSecondText(m_Title_StockNum.Replace("{N}", strNum));
                            PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperSuc"), PubHelper.MsgType.Ok);
                        }
                    }
                }
            }
            ControlSet(true);
        }

        private void ControlSet(bool enable)
        {
            bool blnAdd = btnAdd.IsEnabled;
            bool blnDelete = btnDelete.IsEnabled;

            ////if (enable)
            ////{
            ////    bdFillUpNumArea.Background = new SolidColorBrush(Colors.White);
            ////}
            ////else
            ////{
            ////    bdFillUpNumArea.Background = new SolidColorBrush(Color.FromRgb(209, 204, 204));
            ////    btnAdd.IsEnabled = btnDelete.IsEnabled = false;
            ////    btnDelete.Source = new BitmapImage(new Uri(SkinHelper.FILEPATH_REDUCE_GRAY));
            ////    btnAdd.Source = new BitmapImage(new Uri(SkinHelper.FILEPATH_ADD_GRAY));
            ////}

            btnTrayFull.IsEnabled = btnTraySetOther.IsEnabled = btnSave.IsEnabled = btnCancel.IsEnabled = enable;
            tbBatch_Value.IsEnabled = tbMaxValidDate_Value.IsEnabled = tbProductDate_Value.IsEnabled = enable;
            DispatcherHelper.SleepControl();
        }

        private void tbBatch_Value_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PubHelper.ShowKeyBoard(tbBatch_Value.Text);
            tbBatch_Value.Text = PubHelper.p_Keyboard_Input;
        }

        private void tbMaxValidDate_Value_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PubHelper.ShowKeyBoard(tbMaxValidDate_Value.Text);
            tbMaxValidDate_Value.Text = PubHelper.p_Keyboard_Input;
        }

        private void tbProductDate_Value_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PubHelper.ShowKeyBoard(tbProductDate_Value.Text);
            tbProductDate_Value.Text = PubHelper.p_Keyboard_Input;
        }
    }
}
