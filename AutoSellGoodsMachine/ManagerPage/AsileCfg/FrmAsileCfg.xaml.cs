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
using System.Windows.Threading;

using AutoSellGoodsMachine.Controls;
using Business.Model;
using Business.Enum;

namespace AutoSellGoodsMachine
{
    /// <summary>
    /// FrmAsileCfg.xaml 的交互逻辑
    /// </summary>
    public partial class FrmAsileCfg : Window
    {
        private DispatcherTimer timer = new DispatcherTimer();

        /// <summary>
        /// 当前的货柜编号
        /// </summary>
        private string m_VendBoxCode = "1";

        private GoodsWayProduct currentGoodsWay = null;
        private int currentTrayIndex = 0;
        private string m_Title_NoGoodsTitle = string.Empty;
        private string m_Title_SpringNum = string.Empty;

        public FrmAsileCfg()
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
            tbTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AsileCfg");
            btnCancel.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Cancel");
            btnSave.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Save");
            btnSetTray.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_SetTray_Price");
            btnSelectGoods.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_ChoiceGoods");
            btnRemoveGoods.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_RemoveGoods");

            tbAsilePrice.Text = PubHelper.p_LangOper.GetStringBundle("Pub_Price_Cfg");
            tbSpringNum.Text = PubHelper.p_LangOper.GetStringBundle("Pub_SpringNum");
            ////tbAsileStatus.Text = PubHelper.p_LangOper.GetStringBundle("Pub_AsileStatus");
            tbAsileGoods.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Goods_Asile");
            tbMcdCode.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Goods_Code");
            tbMcdName.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Goods_Name");
            tbMcdContent.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Goods_Content");

            rbUsing.Content = PubHelper.p_LangOper.GetStringBundle("Pub_AsileKind_Run");
            rbStop.Content = PubHelper.p_LangOper.GetStringBundle("Pub_AsileKind_Pause");

            m_Title_NoGoodsTitle = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Stock_NoGoods");

            m_Title_SpringNum = PubHelper.p_LangOper.GetStringBundle("Pub_SpringNum") +
                PubHelper.p_LangOper.GetStringBundle("Pub_Symbol");

            if (PubHelper.p_BusinOper.ConfigInfo.GoodsUploadType == "0")
            {

            }
            switch (PubHelper.p_BusinOper.ConfigInfo.GoodsUploadType)
            {
                case "1":// 远程更新价格
                    tbAsilePrice_Value.IsEnabled = btnSetTray.IsEnabled = false;
                    break;
                default:// 本地设置价格
                    tbAsilePrice_Value.IsEnabled = btnSetTray.IsEnabled = true;
                    break;
            }

            #region 加载列表项数据

            ////for (int i = 1; i < 22; i++)
            ////{
            ////    cmbAsileSpring.Items.Add(i.ToString());
            ////}

            #endregion
        }

        #region 托盘和货道格子操作

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

                palletButton.Checked += (TrayButtonChecked);

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
            string strOneText = string.Empty;

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

                    productControl.MouseLeftButtonUp += (AsileButtonChecked);

                    if (index < products.Count)
                    {
                        productControl.SetCurrentGoodsWayProduct(products[index]);
                        SetAsileInfo(productControl);
                        ////if (string.IsNullOrEmpty(products[index].McdName))
                        ////{
                        ////    strOneText = products[index].PaCode;
                        ////}
                        ////else
                        ////{
                        ////    strOneText = products[index].PaCode + "【" + products[index].McdName + "】";
                        ////}
                        ////productControl.SetOneText(strOneText);
                        ////productControl.SetSecondText(PubHelper.p_BusinOper.MoneyIntToString(products[index].SellPrice));
                        if (index == 0)
                        {
                            productControl.IsDefaultCheck(true);
                            AsileButtonChecked(productControl, null);
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
                ControlAsile(false);
            }
            else
            {
                ControlAsile(true);
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
        private void TrayButtonChecked(object sender, RoutedEventArgs e)
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
        private void AsileButtonChecked(object sender, MouseButtonEventArgs e)
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

            ShowAsileInfo();
            if (currentGoodsWay.CurrentGoodsWayProduct != null)
            {
                ControlAsile(true);
            }
            else
            {
                ControlAsile(false);
            }
        }

        private void ShowAsileInfo()
        {
            if (currentGoodsWay.CurrentGoodsWayProduct != null)
            {
                tbAsilePrice_Value.Text = PubHelper.p_BusinOper.MoneyIntToString(currentGoodsWay.CurrentGoodsWayProduct.SellPrice,false);
                tbSpringNum_Value.Text = currentGoodsWay.CurrentGoodsWayProduct.SpringNum.ToString();
                tbMcdCode_Value.Text = currentGoodsWay.CurrentGoodsWayProduct.McdCode;
                tbMcdName_Value.Text = currentGoodsWay.CurrentGoodsWayProduct.McdName;
                tbMcdContent_Value.Text = currentGoodsWay.CurrentGoodsWayProduct.McdContent;
                if (currentGoodsWay.CurrentGoodsWayProduct.PaKind == "0")
                {
                    rbUsing.IsChecked = true;
                }
                else
                {
                    rbStop.IsChecked = true;
                }

                if (currentGoodsWay.CurrentGoodsWayProduct.SellModel == BusinessEnum.AsileSellModel.Normal)
                {
                    rdbSellModel_Normal.IsChecked = true;
                }
                else
                {
                    rdbSellModel_Gift.IsChecked = true;
                }
            }
            else
            {
                tbAsilePrice_Value.Text = string.Empty;
                tbSpringNum_Value.Text = "0";
                //////cmbAsileSpring.Text = string.Empty;
                tbMcdCode_Value.Text = string.Empty;
                tbMcdName_Value.Text = string.Empty;
                tbMcdContent_Value.Text = string.Empty;
            }
        }

        #endregion

        #region 界面控件操作

        private void SetAsileInfo(GoodsWayProduct asileInfo)
        {
            string strOneText = string.Empty;

            if (string.IsNullOrEmpty(asileInfo.CurrentGoodsWayProduct.McdCode))
            {
                strOneText = asileInfo.CurrentGoodsWayProduct.PaCode;
            }
            else
            {
                strOneText = asileInfo.CurrentGoodsWayProduct.PaCode +
                    " " + asileInfo.CurrentGoodsWayProduct.McdName;// +"】";
            }
            asileInfo.SetOneText(strOneText);
            asileInfo.SetSecondText(PubHelper.p_BusinOper.MoneyIntToString(asileInfo.CurrentGoodsWayProduct.SellPrice) +
                "【" + asileInfo.CurrentGoodsWayProduct.SpringNum + "】");
        }

        private void ControlAsile(bool enable)
        {
            btnSave.IsEnabled = btnSetTray.IsEnabled =  enable;

            if (PubHelper.p_BusinOper.GoodsOper.GoodsList_Total.Count > 0)
            {
                btnSelectGoods.IsEnabled = enable;
            }
            else
            {
                btnSelectGoods.IsEnabled = false;
            }

            bool blnRemove = false;
            if ((enable) && 
                (currentGoodsWay.CurrentGoodsWayProduct != null) &&
                (!string.IsNullOrEmpty(currentGoodsWay.CurrentGoodsWayProduct.McdCode)))
            {
                blnRemove = true;
            }
            btnRemoveGoods.IsEnabled = blnRemove;

            tbAsilePrice_Value.IsEnabled = enable;
            tbSpringNum_Value.IsEnabled = enable;

            if (!enable)
            {
                btnAdd.Source = new BitmapImage(new Uri(SkinHelper.FILEPATH_ADD_GRAY));
                btnDelete.Source = new BitmapImage(new Uri(SkinHelper.FILEPATH_REDUCE_GRAY));
                btnAdd.IsEnabled = btnDelete.IsEnabled = false;
            }
            else
            {
                int intSpringNum = Convert.ToInt32(tbSpringNum_Value.Text);
                if (intSpringNum == 1)
                {
                    btnAdd.Source = new BitmapImage(new Uri(SkinHelper.FILEPATH_ADD_NORMAL));
                    btnDelete.Source = new BitmapImage(new Uri(SkinHelper.FILEPATH_REDUCE_GRAY));
                    btnAdd.IsEnabled = true;
                    btnDelete.IsEnabled = false;
                }
                else if (intSpringNum == 21)
                {
                    btnAdd.Source = new BitmapImage(new Uri(SkinHelper.FILEPATH_ADD_GRAY));
                    btnDelete.Source = new BitmapImage(new Uri(SkinHelper.FILEPATH_REDUCE_NORMAL));
                    btnAdd.IsEnabled = false;
                    btnDelete.IsEnabled = true;
                }
                else
                {
                    btnAdd.Source = new BitmapImage(new Uri(SkinHelper.FILEPATH_ADD_NORMAL));
                    btnDelete.Source = new BitmapImage(new Uri(SkinHelper.FILEPATH_REDUCE_NORMAL));
                    btnAdd.IsEnabled = true;
                    btnDelete.IsEnabled = true;
                }
            }

            ////////cmbAsileSpring.IsEnabled = enable;
            rbStop.IsEnabled = rbUsing.IsEnabled = enable;
            rdbSellModel_Gift.IsEnabled = rdbSellModel_Normal.IsEnabled = enable;
        }

        #endregion

        #region 按钮操作

        /// <summary>
        /// 按钮—托盘设置价格
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSetTray_Click(object sender, RoutedEventArgs e)
        {
            string strAsilePrice = tbAsilePrice_Value.Text;

            #region 检查数据有效性

            string _errInfo = string.Empty;
            if (!PubHelper.CheckInputPrice(strAsilePrice, out _errInfo))
            {
                // 金额格式错误
                PubHelper.ShowMsgInfo(_errInfo, PubHelper.MsgType.Ok);
                return;
            }

            double dblAsilePrice = Convert.ToDouble(strAsilePrice) * PubHelper.p_BusinOper.ConfigInfo.DecimalNum;

            #endregion

            #region 保存托盘价格

            string strAskInfo = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AsileCfg_TrayAsk");
            strAskInfo = strAskInfo.Replace("{N}", PubHelper.p_BusinOper.ChangeTray((currentTrayIndex + 1).ToString()));
            strAskInfo = strAskInfo.Replace("{K}", strAsilePrice);

            PubHelper.ShowMsgInfo(strAskInfo, PubHelper.MsgType.YesNo);
            if (PubHelper.p_MsgResult)
            {
                ControlAsile(false);
                btnCancel.IsEnabled = false;
                DispatcherHelper.DoEvents();

                bool result = PubHelper.p_BusinOper.UpdateTrayPrice(currentTrayIndex.ToString(),
                    dblAsilePrice.ToString(), m_VendBoxCode);
                if (result)
                {
                    PubHelper.p_IsRefreshAsile = true;

                    currentGoodsWay.CurrentGoodsWayProduct.SellPrice = dblAsilePrice.ToString();

                    #region 更新该托盘内的所有货道价格

                    int intAsileCount = panelAsile.Children.Count;
                    for (int asileIndex = 0; asileIndex < intAsileCount; asileIndex++)
                    {
                        GoodsWayProduct productControl = (GoodsWayProduct)panelAsile.Children[asileIndex];
                        if (productControl.CurrentGoodsWayProduct != null)
                        {
                            productControl.SetSecondText(PubHelper.p_BusinOper.MoneyIntToString(dblAsilePrice.ToString()) +
                                "【" + productControl.CurrentGoodsWayProduct.SpringNum + "】");
                        }
                    }

                    #endregion

                    PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperSuc"), PubHelper.MsgType.Ok);
                }
                else
                {
                    PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperFail"), PubHelper.MsgType.Ok);
                }

                ControlAsile(true);
                btnCancel.IsEnabled = true;
            }

            #endregion
        }

        /// <summary>
        /// 按钮—选择商品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectGoods_Click(object sender, RoutedEventArgs e)
        {
            PubHelper.p_BusinOper.GoodsOper.CurrentGoods = null;
            FrmGoodsChoice frmGoodsChoice = new FrmGoodsChoice();
            frmGoodsChoice.ShowDialog();

            if ((PubHelper.p_BusinOper.GoodsOper.CurrentGoods != null) && (currentGoodsWay.CurrentGoodsWayProduct != null))
            {
                GoodsModel mcdInfo = new GoodsModel();
                mcdInfo.McdCode = PubHelper.p_BusinOper.GoodsOper.CurrentGoods.McdCode;
                mcdInfo.McdName = PubHelper.p_BusinOper.GoodsOper.CurrentGoods.McdName;
                mcdInfo.McdContent = PubHelper.p_BusinOper.GoodsOper.CurrentGoods.McdContent;
                mcdInfo.PicName = PubHelper.p_BusinOper.GoodsOper.CurrentGoods.PicName;

                bool result = PubHelper.p_BusinOper.UpdateAsileGoods(currentGoodsWay.CurrentGoodsWayProduct.PaCode, mcdInfo);
                if (result)
                {
                    PubHelper.p_IsRefreshAsile = true;
                    if (PubHelper.p_BusinOper.ConfigInfo.GoodsShowModel == BusinessEnum.GoodsShowModelType.GoodsType)
                    {
                        PubHelper.p_IsRefreshGoodsType = true;
                    }

                    // 更改货道商品显示
                    tbMcdCode_Value.Text = mcdInfo.McdCode;
                    tbMcdName_Value.Text = mcdInfo.McdName;
                    tbMcdContent_Value.Text = mcdInfo.McdContent;
                    currentGoodsWay.CurrentGoodsWayProduct.McdCode = tbMcdCode_Value.Text;
                    currentGoodsWay.CurrentGoodsWayProduct.McdName = tbMcdName_Value.Text;
                    currentGoodsWay.CurrentGoodsWayProduct.McdPicName = mcdInfo.PicName;
                    currentGoodsWay.CurrentGoodsWayProduct.McdContent = mcdInfo.McdContent;
                    SetAsileInfo(currentGoodsWay);
                    currentGoodsWay.SetAsilePic();

                    if (!string.IsNullOrEmpty(mcdInfo.McdCode))
                    {
                        btnRemoveGoods.IsEnabled = true;
                    }
                }
                else
                {
                    PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperFail"), PubHelper.MsgType.Ok);
                }
            }
        }

        /// <summary>
        /// 按钮—移除货道商品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRemoveGoods_Click(object sender, RoutedEventArgs e)
        {
            string strPaCode = currentGoodsWay.CurrentGoodsWayProduct.PaCode;

            string strAskInfo = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AsileCfg_RemoveAsk");
            strAskInfo = strAskInfo.Replace("{N}", strPaCode);

            PubHelper.ShowMsgInfo(strAskInfo, PubHelper.MsgType.YesNo);
            if (PubHelper.p_MsgResult)
            {
                bool result = PubHelper.p_BusinOper.RemoveAsileGoods(strPaCode);

                if (result)
                {
                    PubHelper.p_IsRefreshAsile = true;
                    if (PubHelper.p_BusinOper.ConfigInfo.GoodsShowModel == BusinessEnum.GoodsShowModelType.GoodsType)
                    {
                        PubHelper.p_IsRefreshGoodsType = true;
                    }

                    currentGoodsWay.CurrentGoodsWayProduct.McdCode = string.Empty;
                    currentGoodsWay.CurrentGoodsWayProduct.McdName = string.Empty;
                    currentGoodsWay.CurrentGoodsWayProduct.McdContent = string.Empty;
                    tbMcdCode_Value.Text = tbMcdContent_Value.Text = tbMcdName_Value.Text = string.Empty;
                    SetAsileInfo(currentGoodsWay);
                    currentGoodsWay.SetAsilePic();
                }
                else
                {
                    PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperFail"), PubHelper.MsgType.Ok);
                }
            }
        }

        /// <summary>
        /// 按钮—取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #endregion

        private void tbAsilePrice_Value_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FrmKeyboard frmKeyBoard = new FrmKeyboard();
            frmKeyBoard.ShowDialog();
        }

        private void tbAsilePrice_Value_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            FrmKeyboard frmKeyBoard = new FrmKeyboard();
            frmKeyBoard.ShowDialog();
        }

        private void tbAsilePrice_Value_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PubHelper.ShowKeyBoard(tbAsilePrice_Value.Text);
            tbAsilePrice_Value.Text = PubHelper.p_Keyboard_Input;
        }

        private void btnSave_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            
        }

        /// <summary>
        /// 添加弹簧圈数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddInventory(object sender, MouseButtonEventArgs e)
        {
            SetProductInventory(1);
        }

        /// <summary>
        /// 减少弹簧圈数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteInventory(object sender, MouseButtonEventArgs e)
        {
            SetProductInventory(-1);
        }

        /// <summary>
        /// 添加或减少弹簧圈数
        /// </summary>
        private void SetProductInventory(int i)
        {
            if (currentGoodsWay.CurrentGoodsWayProduct != null)
            {
                int intSprintNum = Convert.ToInt32(tbSpringNum_Value.Text) + i;

                if (i == 1)
                {
                    // 增加弹簧圈数
                    if (intSprintNum > 21)
                    {
                        return;
                    }
                    btnDelete.IsEnabled = true;
                    btnDelete.Source = new BitmapImage(new Uri(SkinHelper.FILEPATH_REDUCE_NORMAL));
                    if (intSprintNum == 21)
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
                    // 减少弹簧圈数，数量不能小于1
                    if (intSprintNum < 1)
                    {
                        return;
                    }
                    btnAdd.IsEnabled = true;
                    btnAdd.Source = new BitmapImage(new Uri(SkinHelper.FILEPATH_ADD_NORMAL));
                    if (intSprintNum == 1)
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

                tbSpringNum_Value.Text = intSprintNum.ToString();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string strAsilePrice = tbAsilePrice_Value.Text;
            string strSpringNum = tbSpringNum_Value.Text;
            string strAsileKind = "0";
            string strSellModel = "0";
            BusinessEnum.AsileSellModel asileSellModel = BusinessEnum.AsileSellModel.Normal;
            if (rbStop.IsChecked == true)
            {
                strAsileKind = "1";
            }

            if (rdbSellModel_Gift.IsChecked == true)
            {
                strSellModel = "1";
                asileSellModel = BusinessEnum.AsileSellModel.Gift;
            }

            #region 检查数据有效性

            string _errInfo = string.Empty;
            if (!PubHelper.CheckInputPrice(strAsilePrice, out _errInfo))
            {
                // 金额格式错误
                PubHelper.ShowMsgInfo(_errInfo, PubHelper.MsgType.Ok);
                return;
            }

            double dblAsilePrice = Convert.ToDouble(strAsilePrice) * PubHelper.p_BusinOper.ConfigInfo.DecimalNum;

            #endregion

            #region 检查是否需要修改

            bool blnIsSave = false;

            if ((currentGoodsWay.CurrentGoodsWayProduct.SellPrice != dblAsilePrice.ToString()) ||
                (currentGoodsWay.CurrentGoodsWayProduct.SpringNum.ToString() != strSpringNum) ||
                (currentGoodsWay.CurrentGoodsWayProduct.PaKind != strAsileKind) ||
                (currentGoodsWay.CurrentGoodsWayProduct.SellModel != asileSellModel))
            {
                blnIsSave = true;
            }

            #endregion

            if (blnIsSave)
            {
                ControlAsile(false);
                btnCancel.IsEnabled = false;
                DispatcherHelper.DoEvents();

                bool result = PubHelper.p_BusinOper.UpdateAsileInfo(currentGoodsWay.CurrentGoodsWayProduct.PaCode,
                    dblAsilePrice.ToString(), strSpringNum, strAsileKind, strSellModel);
                if (result)
                {
                    currentGoodsWay.CurrentGoodsWayProduct.SellPrice = dblAsilePrice.ToString();
                    currentGoodsWay.CurrentGoodsWayProduct.SpringNum = Convert.ToInt32(strSpringNum);
                    SetAsileInfo(currentGoodsWay);

                    PubHelper.p_IsRefreshAsile = true;

                    PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperSuc"), PubHelper.MsgType.Ok);
                }
                else
                {
                    PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperFail"), PubHelper.MsgType.Ok);
                }
                ControlAsile(true);
                btnCancel.IsEnabled = true;
            }
        }
    }
}
