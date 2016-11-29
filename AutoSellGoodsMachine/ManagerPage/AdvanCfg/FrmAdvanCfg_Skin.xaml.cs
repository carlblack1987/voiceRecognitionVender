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

using Business.Enum;

namespace AutoSellGoodsMachine
{
    /// <summary>
    /// FrmAdvanCfg_Skin.xaml 的交互逻辑
    /// </summary>
    public partial class FrmAdvanCfg_Skin : Window
    {
        private bool m_IsInit = true;

        public FrmAdvanCfg_Skin()
        {
            InitializeComponent();

            m_IsInit = true;
            InitForm();
        }

        /// <summary>
        /// 初始化界面资源
        /// </summary>
        private void InitForm()
        {
            #region 初始化界面资源

            tbTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_Title");
            tbHideCuror.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_Cursor");
            rdbHideCuror_Hide.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_Cursor_Hide");
            rdbHideCuror_Show.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_Cursor_Show");
            tbSkinStyle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_Skin");
            tbGoodsShowType.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_GoodsShow");

            tbGoodsNameType.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_GoodsName");
            tbGoodsPropShowType.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_GoodsProp");
            tbIsShowGoodsDetailContent.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_GoodsContent");

            tbGoodsOpacity.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_GoodsOpacity");

            tbGoodsClick.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_GoodsClick");

            tbGoodsShowModel.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_GoodsModel");
            tbEachPageMaxRowNum.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_GoodsModel_Row");
            tbEachRowMaxColuNum.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_GoodsModel_Column");

            tbIsShowMoneySymbol.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_MoneySymbol");

            tbIsShowChoiceKeyBoard.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_KeyboardChoice");

            string strSkin_Clud = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_Clud");
            string strSkin_Star = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_Star");
            cmbSoftSkin.Items.Add(strSkin_Clud);
            cmbSoftSkin.Items.Add(strSkin_Star);

            string strGoodsModel_Asile = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_GoodsModel_Asile");
            string strGoodsModel_Goods = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_GoodsModel_Goods");
            string strGoodsModel_Type = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_GoodsModel_Type");
            cmbGoodsShowModel.Items.Add(strGoodsModel_Asile);
            cmbGoodsShowModel.Items.Add(strGoodsModel_Goods);
            cmbGoodsShowModel.Items.Add(strGoodsModel_Type);

            for (int i = 2; i < 7; i++)
            {
                cmbEachPageMaxRowNum.Items.Add(i.ToString());
            }
            cmbEachPageMaxRowNum.Text = PubHelper.p_BusinOper.ConfigInfo.EachPageMaxRowNum.ToString();

            for (int i = 2; i < 10; i++)
            {
                cmbEachRowMaxColuNum.Items.Add(i.ToString());
            }
            cmbEachRowMaxColuNum.Text = PubHelper.p_BusinOper.ConfigInfo.EachRowMaxColuNum.ToString();

            string strGoodsShow_Asile = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_GoodsShow_Asile");
            string strGoodsShow_Price = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_GoodsShow_Price");
            string strGoodsShow_AsilePrice = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_GoodsShow_AsilePrice");
            string strGoodsShow_McdName = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_GoodsShow_McdName");
            string strGoodsShow_McdNamePrice = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_GoodsShow_McdNamePrice");
            cmbGoodsShowType.Items.Add(strGoodsShow_Asile);
            cmbGoodsShowType.Items.Add(strGoodsShow_Price);
            cmbGoodsShowType.Items.Add(strGoodsShow_AsilePrice);
            cmbGoodsShowType.Items.Add(strGoodsShow_McdName);
            cmbGoodsShowType.Items.Add(strGoodsShow_McdNamePrice);

            string strGoodsName_Asile = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_GoodsName_Asile");
            string strGoodsName_Name = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_GoodsName_Name");
            string strGoodsName_All = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_GoodsName_AsileName");
            cmbGoodsNameType.Items.Add(strGoodsName_Asile);
            cmbGoodsNameType.Items.Add(strGoodsName_Name);
            cmbGoodsNameType.Items.Add(strGoodsName_All);

            string strGoodsProp_No = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_GoodsProp_No");
            string strGoodsProp_Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_GoodsProp_Content");
            string strGoodsProp_Spec = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_GoodsProp_Spec");
            string strGoodsProp_Manuf = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_GoodsProp_Manuf");
            string strGoodsProp_SpecManuf = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_GoodsProp_SpecManuf");
            string strGoodsProp_PiCiDate = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_GoodsProp_PiCiDate");
            string strGoodsProp_Drug = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_GoodsProp_Drug");
            cmbGoodsPropShowType.Items.Add(strGoodsProp_No);
            cmbGoodsPropShowType.Items.Add(strGoodsProp_Content);
            cmbGoodsPropShowType.Items.Add(strGoodsProp_Spec);
            cmbGoodsPropShowType.Items.Add(strGoodsProp_Manuf);
            cmbGoodsPropShowType.Items.Add(strGoodsProp_SpecManuf);
            cmbGoodsPropShowType.Items.Add(strGoodsProp_PiCiDate);
            cmbGoodsPropShowType.Items.Add(strGoodsProp_Drug);

            string strGoodsContent_Simple = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_GoodsContent_Simple");
            string strGoodsContent_Detail = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_GoodsContent_Detail");
            cmbIsShowGoodsDetailContent.Items.Add(strGoodsContent_Simple);
            cmbIsShowGoodsDetailContent.Items.Add(strGoodsContent_Detail);

            for (int i = 1; i < 11; i++)
            {
                cmbGoodsOpacity.Items.Add(i.ToString());
            }

            string strGoodsClick_Ok = PubHelper.p_LangOper.GetStringBundle("Pub_Run");
            string strGoodsClick_No = PubHelper.p_LangOper.GetStringBundle("Pub_Stop");
            cmbGoodsClick.Items.Add(strGoodsClick_Ok);
            cmbGoodsClick.Items.Add(strGoodsClick_No);

            cmbIsShowMoneySymbol.Items.Add(strGoodsClick_Ok);
            cmbIsShowMoneySymbol.Items.Add(strGoodsClick_No);

            cmbIsShowChoiceKeyBoard.Items.Add(strGoodsClick_Ok);
            cmbIsShowChoiceKeyBoard.Items.Add(strGoodsClick_No);

            btnSave.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Save");
            btnCancel.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Cancel");

            tbFontSize.Text = PubHelper.p_LangOper.GetStringBundle("Pub_FontSize");
            for (int i = 10; i < 51; i++)
            {
                cmbFontSize.Items.Add(i.ToString());
            }
            cmbFontSize.Text = PubHelper.p_BusinOper.ConfigInfo.GoodsDetailFontSize.ToString();

            #endregion

            #region 加载参数数据

            string strHideCuror = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("HideCuror");
            if (strHideCuror == "0")
            {
                // 不隐藏鼠标
                rdbHideCuror_Show.IsChecked = true;
            }
            else
            {
                // 隐藏鼠标
                rdbHideCuror_Hide.IsChecked = true;
            }

            string strSkin = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("SkinStyle");
            switch (strSkin)
            {
                case "0":// 蓝天白云
                    cmbSoftSkin.Text = strSkin_Clud;
                    break;
                default:// 星空
                    cmbSoftSkin.Text = strSkin_Star;
                    break;
            }

            switch (PubHelper.p_BusinOper.ConfigInfo.GoodsShowModel)
            {
                case BusinessEnum.GoodsShowModelType.GoodsToOnlyAsile:// 商品和货道一一对应
                    cmbGoodsShowModel.Text = strGoodsModel_Asile;
                    cmbEachPageMaxRowNum.IsEnabled = cmbEachRowMaxColuNum.IsEnabled = false;
                    break;
                case BusinessEnum.GoodsShowModelType.GoodsToMultiAsile:// 商品显示
                    cmbGoodsShowModel.Text = strGoodsModel_Goods;
                    cmbEachPageMaxRowNum.IsEnabled = cmbEachRowMaxColuNum.IsEnabled = true;
                    break;
                case BusinessEnum.GoodsShowModelType.GoodsType:// 商品类型显示
                    cmbGoodsShowModel.Text = strGoodsModel_Type;
                    cmbEachPageMaxRowNum.IsEnabled = cmbEachRowMaxColuNum.IsEnabled = true;
                    break;
                default:
                    cmbGoodsShowModel.Text = strGoodsModel_Asile;
                    cmbEachPageMaxRowNum.IsEnabled = cmbEachRowMaxColuNum.IsEnabled = false;
                    break;
            }

            switch (PubHelper.p_BusinOper.ConfigInfo.GoodsShowContent)
            {
                case "1":// 只显示商品价格
                    cmbGoodsShowType.Text = strGoodsShow_Price;
                    break;
                case "2":// 只显示货道编号
                    cmbGoodsShowType.Text = strGoodsShow_Asile;
                    break;
                case "3":// 显示商品价格和货道编号
                    cmbGoodsShowType.Text = strGoodsShow_AsilePrice;
                    break;
                case "4":// 只显示商品名称
                    cmbGoodsShowType.Text = strGoodsShow_McdName;
                    break;
                case "5":// 显示商品名称和价格
                    cmbGoodsShowType.Text = strGoodsShow_McdNamePrice;
                    break;
                default:
                    break;
            }

            switch (PubHelper.p_BusinOper.ConfigInfo.GoodsNameShowType)
            {
                case "1":// 只显示商品名称
                    cmbGoodsNameType.Text = strGoodsName_Name;
                    break;
                case "2":// 只显示货道编号
                    cmbGoodsNameType.Text = strGoodsName_Asile;
                    break;
                case "3":// 显示商品名称和货道编号
                    cmbGoodsNameType.Text = strGoodsName_All;
                    break;
                default:
                    break;
            }

            switch (PubHelper.p_BusinOper.ConfigInfo.GoodsPropShowType)
            {
                case "0":// 不显示任何内容
                    cmbGoodsPropShowType.Text = strGoodsProp_No;
                    break;
                case "1":// 只显示商品介绍
                    cmbGoodsPropShowType.Text = strGoodsProp_Content;
                    break;
                case "2":// 只显示商品规格
                    cmbGoodsPropShowType.Text = strGoodsProp_Spec;
                    break;
                case "3":// 只显示生产厂家
                    cmbGoodsPropShowType.Text = strGoodsProp_Manuf;
                    break;
                case "4":// 显示商品规格和生产厂家
                    cmbGoodsPropShowType.Text = strGoodsProp_SpecManuf;
                    break;
                case "5":// 显示生产批次和有效期
                    cmbGoodsPropShowType.Text = strGoodsProp_PiCiDate;
                    break;
                case "6":// 医药行业显示方式
                    cmbGoodsPropShowType.Text = strGoodsProp_Drug;
                    break;
                default:
                    break;
            }

            switch (PubHelper.p_BusinOper.ConfigInfo.IsShowGoodsDetailContent)
            {
                case BusinessEnum.ControlSwitch.Stop:
                    cmbIsShowGoodsDetailContent.Text = strGoodsContent_Simple;
                    cmbFontSize.IsEnabled = false;
                    break;
                case BusinessEnum.ControlSwitch.Run:
                    cmbIsShowGoodsDetailContent.Text = strGoodsContent_Detail;
                    cmbFontSize.IsEnabled = true;
                    break;
                default:
                    break;
            }

            cmbGoodsOpacity.Text = PubHelper.p_BusinOper.ConfigInfo.GoodsOpacity.ToString();

            switch (PubHelper.p_BusinOper.ConfigInfo.NoStockClickGoods)
            {
                case "0":// 不允许点击
                    cmbGoodsClick.Text = strGoodsClick_No;
                    break;

                default:// 允许点击
                    cmbGoodsClick.Text = strGoodsClick_Ok;
                    break;
            }

            switch (PubHelper.p_BusinOper.ConfigInfo.IsShowMoneySymbol)
            { 
                case BusinessEnum.ControlSwitch.Stop:// 不显示
                    cmbIsShowMoneySymbol.Text = strGoodsClick_No;
                    break;

                default:// 显示
                    cmbIsShowMoneySymbol.Text = strGoodsClick_Ok;
                    break;
            }

            switch (PubHelper.p_BusinOper.ConfigInfo.IsShowChoiceKeyBoard)
            {
                case BusinessEnum.ControlSwitch.Stop:// 不显示
                    cmbIsShowChoiceKeyBoard.Text = strGoodsClick_No;
                    break;

                default:// 显示
                    cmbIsShowChoiceKeyBoard.Text = strGoodsClick_Ok;
                    break;
            }

            #endregion

            if (PubHelper.p_BusinOper.ConfigInfo.IsRunStock == BusinessEnum.ControlSwitch.Stop)
            {
                // 如果库存管理不启用，则与此有关的参数不能设置
                cmbGoodsOpacity.IsEnabled = cmbGoodsClick.IsEnabled = false;
            }

            if (PubHelper.p_BusinOper.ConfigInfo.GoodsShowModel == BusinessEnum.GoodsShowModelType.GoodsToOnlyAsile)
            {
                cmbEachPageMaxRowNum.IsEnabled = cmbEachRowMaxColuNum.IsEnabled = false;
            }
            else
            {
                // 如果商品列表是按商品显示，则行及列的参数可以设置
                cmbEachPageMaxRowNum.IsEnabled = cmbEachRowMaxColuNum.IsEnabled = true;
            }

            m_IsInit = false;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            btnSave.IsEnabled = btnCancel.IsEnabled = false;
            DispatcherHelper.DoEvents();

            string strHideCursor = "0";
            if (rdbHideCuror_Hide.IsChecked == true)
            {
                strHideCursor = "1";
            }

            string strSkin = "0";
            if (cmbSoftSkin.Text == PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_Star"))
            {
                strSkin = "1";
            }

            // 商品显示模式
            string strGoodsShowModel = "0";
            string strEachPageMaxRowNum = string.Empty;
            string strEachRowMaxColuNum = string.Empty;
            if (cmbGoodsShowModel.Text == PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_GoodsModel_Goods"))
            {
                strGoodsShowModel = "1";
                strEachPageMaxRowNum = cmbEachPageMaxRowNum.Text;
                strEachRowMaxColuNum = cmbEachRowMaxColuNum.Text;
            }
            if (cmbGoodsShowModel.Text == PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_GoodsModel_Type"))
            {
                strGoodsShowModel = "3";
                strEachPageMaxRowNum = cmbEachPageMaxRowNum.Text;
                strEachRowMaxColuNum = cmbEachRowMaxColuNum.Text;
            }

            // 商品列表显示内容
            string strGoodsShowType = "1";
            if (cmbGoodsShowType.Text == PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_GoodsShow_Asile"))
            {
                strGoodsShowType = "2";
            }
            if (cmbGoodsShowType.Text == PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_GoodsShow_AsilePrice"))
            {
                strGoodsShowType = "3";
            }
            if (cmbGoodsShowType.Text == PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_GoodsShow_McdName"))
            {
                strGoodsShowType = "4";
            }
            if (cmbGoodsShowType.Text == PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_GoodsShow_McdNamePrice"))
            {
                strGoodsShowType = "5";
            }

            // 商品名称显示内容
            string strGoodsNameType = "1";
            if (cmbGoodsNameType.Text == PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_GoodsName_Asile"))
            {
                strGoodsNameType = "2";
            }
            if (cmbGoodsNameType.Text == PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_GoodsName_AsileName"))
            {
                strGoodsNameType = "3";
            }

            // 商品属性显示内容
            string strGoodsPropType = "0";
            if (cmbGoodsPropShowType.Text == PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_GoodsProp_Content"))
            {
                strGoodsPropType = "1";
            }
            if (cmbGoodsPropShowType.Text == PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_GoodsProp_Spec"))
            {
                strGoodsPropType = "2";
            }
            if (cmbGoodsPropShowType.Text == PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_GoodsProp_Manuf"))
            {
                strGoodsPropType = "3";
            }
            if (cmbGoodsPropShowType.Text == PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_GoodsProp_SpecManuf"))
            {
                strGoodsPropType = "4";
            }
            if (cmbGoodsPropShowType.Text == PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_GoodsProp_PiCiDate"))
            {
                strGoodsPropType = "5";
            }
            if (cmbGoodsPropShowType.Text == PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_GoodsProp_Drug"))
            {
                strGoodsPropType = "6";
            }

            string strIsGoodsContentDetail = "0";
            if (cmbIsShowGoodsDetailContent.Text == PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_GoodsContent_Detail"))
            {
                strIsGoodsContentDetail = "1";
            }

            // 售完商品透明度
            string strGoodsOpacity = cmbGoodsOpacity.Text;

            string strRun = PubHelper.p_LangOper.GetStringBundle("Pub_Run");
            // 售完商品点击
            string strGoodsClick = "0";
            if (cmbGoodsClick.Text == strRun)
            {
                strGoodsClick = "1";
            }

            string strIsShowMoneySymbol = "0";
            if (cmbIsShowMoneySymbol.Text == strRun)
            {
                strIsShowMoneySymbol = "1";
            }

            string strIsShowChoiceKeyboard = "0";
            if (cmbIsShowChoiceKeyBoard.Text == strRun)
            {
                strIsShowChoiceKeyboard = "1";
            }

            // 保存参数
            PubHelper.p_BusinOper.UpdateSysCfgValue("HideCuror", strHideCursor);
            PubHelper.p_BusinOper.UpdateSysCfgValue("SkinStyle", strSkin);

            PubHelper.p_BusinOper.UpdateSysCfgValue("GoodsShowModel", strGoodsShowModel);
            if ((strGoodsShowModel == "1") || (strGoodsShowModel == "3"))
            {
                PubHelper.p_BusinOper.UpdateSysCfgValue("EachPageMaxRowNum", strEachPageMaxRowNum);
                PubHelper.p_BusinOper.UpdateSysCfgValue("EachRowMaxColuNum", strEachRowMaxColuNum);
            }

            PubHelper.p_BusinOper.UpdateSysCfgValue("GoodsShowContent", strGoodsShowType);
            PubHelper.p_BusinOper.UpdateSysCfgValue("GoodsNameShowType", strGoodsNameType);
            PubHelper.p_BusinOper.UpdateSysCfgValue("GoodsPropShowType", strGoodsPropType);
            PubHelper.p_BusinOper.UpdateSysCfgValue("IsShowGoodsDetailContent", strIsGoodsContentDetail);
            
            PubHelper.p_BusinOper.UpdateSysCfgValue("GoodsOpacity", strGoodsOpacity);
            PubHelper.p_BusinOper.UpdateSysCfgValue("NoStockClickGoods", strGoodsClick);

            PubHelper.p_BusinOper.UpdateSysCfgValue("IsShowMoneySymbol", strIsShowMoneySymbol);

            if (PubHelper.p_BusinOper.ConfigInfo.GoodsDetailFontSize.ToString() != cmbFontSize.Text)
            {
                PubHelper.p_BusinOper.UpdateSysCfgValue("GoodsDetailFontSize", cmbFontSize.Text);
            }

            PubHelper.p_BusinOper.UpdateSysCfgValue("IsShowChoiceKeyBoard", strIsShowChoiceKeyboard);

            PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperSuc_Restart"), PubHelper.MsgType.Ok);
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void cmbGoodsShowModel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!m_IsInit)
            {
                string strText = cmbGoodsShowModel.Text;
                cmbEachPageMaxRowNum.IsEnabled = cmbEachRowMaxColuNum.IsEnabled = true;
                // 临时测试
                //////if ((strText != PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_GoodsModel_Goods")) &&
                //////    (strText != PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_GoodsModel_Type")))
                //////{
                //////    cmbEachPageMaxRowNum.IsEnabled = cmbEachRowMaxColuNum.IsEnabled = true;
                //////}
                //////else
                //////{
                //////    cmbEachPageMaxRowNum.IsEnabled = cmbEachRowMaxColuNum.IsEnabled = false;
                //////}
            }
        }

        private void cmbIsShowGoodsDetailContent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!m_IsInit)
            {
                if (cmbIsShowGoodsDetailContent.Text == PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_GoodsContent_Simple"))
                {
                    cmbFontSize.IsEnabled = true;
                }
                else
                {
                    cmbFontSize.IsEnabled = false;
                }
            }
        }
    }
}
