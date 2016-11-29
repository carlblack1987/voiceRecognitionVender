#region [ KIMMA Co.,Ltd. Copyright (C) 2014 ]
//-------------------------------------------------------------------------------------
// 开发单位：湖南金码智能设备制造有限公司
// 业务模块：iVend终端软件业务处理类
// 业务功能：货道信息处理类
// 创建标识：2014-06-10		谷霖
//-------------------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using KdbPlug;
using Business.Model;
using Business.Enum;

namespace Business.Common
{
    public class AsileHelper
    {
        #region 变量声明

        private string _m_DbFileName = "TermInfo.db";

        /// <summary>
        /// 货道信息链式表（总表）
        /// </summary>
        public List<AsileModel> AsileList = new List<AsileModel>();

        /// <summary>
        /// 当前货柜下的货道信息链式表 2015-05-19
        /// </summary>
        public List<AsileModel> AsileList_Current = new List<AsileModel>();

        private List<AsileModel> AsileListTemp = new List<AsileModel>();

        private List<AsileModel> AsileList_His = new List<AsileModel>();

        /// <summary>
        /// 货柜信息链式表
        /// </summary>
        public List<VendBoxCodeModel> VendBoxList = new List<VendBoxCodeModel>();
        public List<VendBoxCodeModel> VendBoxList_Pa = new List<VendBoxCodeModel>();
        /// <summary>
        /// 具有升降出货方式的货柜信息链式表
        /// </summary>
        public List<VendBoxCodeModel> VendBoxList_Lifter = new List<VendBoxCodeModel>();
 
        #endregion

        #region 货道属性

        /// <summary>
        /// 当前所选择的货柜 2015-05-19
        /// </summary>
        public string CurrentVendBox
        {
            get;
            set;
        }

        /// <summary>
        /// 当前选择的货道
        /// </summary>
        public AsileModel CurrentMcdInfo
        {
            get;
            set;
        }

        public int ColumnCount_Current
        {
            get
            {
                return AsileList_Current.Max(item => item.ColumnIndex);
            }
        }

        public int RowCount_Current
        {
            get
            {
                return AsileList_Current.Max(item => item.RowIndex);
            }
        }

        public int TrayMaxNum_Current
        {
            get
            {
                return AsileList_Current.Max(item => item.TrayIndex);
            }
        }

        public int TrayMaxNum_Total
        {
            get
            {
                return AsileList.Max(item => item.TrayIndex);
            }
        }

        #endregion

        #region 公有函数

        /// <summary>
        /// 全部—加载货道信息列表
        /// </summary>
        /// <returns>加载结果 True：成功 False：失败</returns>
        public bool LoadAsileInfo_Total(string commonType,string isTenAsileNum)
        {
            bool result = false;

            AsileList.Clear();
            AsileListTemp.Clear();
            AsileList_Current.Clear();

            VendBoxList_Pa.Clear();

            CurrentVendBox = "1";// 默认为第一货柜

            DbOper dbOper = new DbOper();
            dbOper.DbFileName = _m_DbFileName;

            try
            {
                string strSql = string.Empty;

                #region 获取货柜信息 2015-05-19

                strSql = "select distinct vendboxcode from t_vm_painfo";
                DataSet dataSet_Vend = dbOper.dataSet(strSql);
                if (dataSet_Vend.Tables.Count > 0)
                {
                    int intVendCount = dataSet_Vend.Tables[0].Rows.Count;
                    for (int i = 0; i < intVendCount; i++)
                    {
                        VendBoxList_Pa.Add(new VendBoxCodeModel()
                        {
                            VendBoxCode = dataSet_Vend.Tables[0].Rows[i]["VendBoxCode"].ToString(),
                        });
                    }
                }

                LoadVendBox();

                #endregion

                #region 设置所有货道状态为0（正常）

                strSql = "update t_vm_painfo set pakind = '0'";
                result = dbOper.excuteSql(strSql);

                #endregion

                #region 获取货道信息

                strSql = @"select t1.VendBoxCode as VendBoxCode,t1.PaId as PaId,t1.PaCode as PaCode,
                    t1.PaCode_Str as PaCode_Str,t1.PaCode_Num as PaCode_Num,
                    t1.SellPrice as SellPrice,t1.PaStackNum as PaStackNum,
                    t1.SurNum as SurNum,t2.McdCode as McdCode,t2.McdName as McdName,
                    t2.PicName as McdPicName,
                    t1.PaKind as PaKind,t1.PaStatus as PaStatus,t1.PaAdvertFile as PaAdvertFile,
                    t1.SpringNum as SpringNum,t1.IsNew as IsNew,t1.TrayNum as TrayNum,t2.McdContent as McdContent,
                    t1.PaPostion as PaPostion,t2.IsFree as IsFree,t1.PiCi as PiCi,t1.ProductDate as ProductDate,
                    t2.Manufacturer as Manufacturer,t2.GoodsSpec as GoodsSpec,t1.MaxValidDate as MaxValidDate,t2.DrugType as DrugType,
                    t1.SaleNum as SaleNum,t1.SaleMoney as SaleMoney,t1.SellModel as SellModel,t2.Unit as Unit,t2.DetailInfo as DetailInfo,t2.McdSaleType as McdSaleType          
                    from t_vm_painfo t1,t_mcd_baseinfo t2
                    where t1.pakind = '0' and t1.mcdcode = t2.mcdcode ";
                strSql += " union ";

                strSql += @"select VendBoxCode,PaId,PaCode,
                    PaCode_Str,PaCode_Num,
                    SellPrice,PaStackNum,
                    SurNum,McdCode,'' as McdName,
                    '' as McdPicName,
                    PaKind,PaStatus,PaAdvertFile,
                    SpringNum,IsNew,TrayNum,'' as McdContent,PaPostion,'0' as IsFree,PiCi,ProductDate,'' as Manufacturer,'' as GoodsSpec,
                    MaxValidDate,'0' as DrugType,SaleNum,SaleMoney,SellModel,'' as Unit,'' as DetailInfo,McdSaleType            
                    from T_VM_PAINFO where pakind = '0' and mcdcode not in (select mcdcode from t_mcd_baseinfo)";

                string strPaCode = string.Empty;
                string strPaId = string.Empty;
                string strPaPostion = string.Empty;
                bool blnIsNew = false;
                int intTrayIndex = 0;
                int intPaPostion = 0;

                DataSet dataSet = dbOper.dataSet(strSql);
                if (dataSet.Tables.Count > 0)
                {
                    int recordCount = dataSet.Tables[0].Rows.Count;
                    for (int i = 0; i < recordCount; i++)
                    {
                        switch (commonType)
                        {
                            case "0":// 字符标签
                                strPaCode = dataSet.Tables[0].Rows[i]["PaCode_Str"].ToString();
                                break;

                            case "1":// 数字标签
                                strPaCode = dataSet.Tables[0].Rows[i]["PaCode_Num"].ToString();
                                break;
                        }
                        strPaId = dataSet.Tables[0].Rows[i]["PaId"].ToString();
                        strPaPostion = dataSet.Tables[0].Rows[i]["PaPostion"].ToString();
                        if (dataSet.Tables[0].Rows[i]["IsNew"].ToString() == "1")
                        {
                            blnIsNew = true;
                        }
                        else
                        {
                            blnIsNew = false;
                        }
                        intTrayIndex = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TrayNum"].ToString()) - 1;
                        if (isTenAsileNum == "1")
                        {
                            // 托盘上有10个货道
                            intPaPostion = Convert.ToInt32(strPaPostion.Substring(1));
                        }
                        else
                        {
                            // 托盘上没有10个货道
                            intPaPostion = Convert.ToInt32(strPaPostion.Substring(1)) - 1;
                        }

                        AsileList.Add(new AsileModel()
                        {
                            VendBoxCode = dataSet.Tables[0].Rows[i]["VendBoxCode"].ToString(),
                            PaId = strPaId,
                            PaCode = strPaCode,
                            SellPrice = dataSet.Tables[0].Rows[i]["SellPrice"].ToString(),
                            PaStackNum = Convert.ToInt32(dataSet.Tables[0].Rows[i]["PaStackNum"].ToString()),
                            SurNum = Convert.ToInt32(dataSet.Tables[0].Rows[i]["SurNum"].ToString()),
                            SpringNum = Convert.ToInt32(dataSet.Tables[0].Rows[i]["SpringNum"].ToString()),
                            PaKind = dataSet.Tables[0].Rows[i]["PaKind"].ToString(),
                            PaStatus = "01",//dataSet.Tables[0].Rows[i]["PaStatus"].ToString(),
                            McdCode = dataSet.Tables[0].Rows[i]["McdCode"].ToString(),
                            McdPicName = dataSet.Tables[0].Rows[i]["McdPicName"].ToString(),
                            McdName = dataSet.Tables[0].Rows[i]["McdName"].ToString(),
                            McdContent = dataSet.Tables[0].Rows[i]["McdContent"].ToString(),
                            PaAdvertFile = dataSet.Tables[0].Rows[i]["PaAdvertFile"].ToString(),
                            ColumnIndex = intPaPostion,
                            TrayIndex = intTrayIndex,
                            RowIndex = intTrayIndex,
                            Unit = dataSet.Tables[0].Rows[i]["Unit"].ToString(),
                            IsNew = blnIsNew,
                            IsQueryStatus = false,
                            IsFree = dataSet.Tables[0].Rows[i]["IsFree"].ToString(),
                            PiCi = dataSet.Tables[0].Rows[i]["PiCi"].ToString(),
                            ProductDate = dataSet.Tables[0].Rows[i]["ProductDate"].ToString(),
                            Manufacturer = dataSet.Tables[0].Rows[i]["Manufacturer"].ToString(),
                            GoodsSpec = dataSet.Tables[0].Rows[i]["GoodsSpec"].ToString(),
                            MaxValidDate = dataSet.Tables[0].Rows[i]["MaxValidDate"].ToString(),
                            DrugType = dataSet.Tables[0].Rows[i]["DrugType"].ToString(),
                            SaleNum = Convert.ToInt32(dataSet.Tables[0].Rows[i]["SaleNum"].ToString()),
                            SaleMoney = Convert.ToInt32(dataSet.Tables[0].Rows[i]["SaleMoney"].ToString()),
                            DetailInfo = dataSet.Tables[0].Rows[i]["DetailInfo"].ToString(),
                            SellModel = ConvertSellModel(dataSet.Tables[0].Rows[i]["SellModel"].ToString()),
                            McdSaleType = FunHelper.ChangeMcdSaleType(dataSet.Tables[0].Rows[i]["McdSaleType"].ToString())
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

                AsileListTemp = AsileList;

                if (AsileList.Count < 1)
                {
                    result = false;
                }
            }
            return result;
        }

        /// <summary>
        /// 全部—初始化检测货道状态后，重新刷新货道信息列表
        /// </summary>
        /// <returns></returns>
        public bool RefreshAsileInfo_Total()
        {
            try
            {
                //AsileListTemp.Clear();
                AsileListTemp = AsileList;
                int intCount = AsileListTemp.Count;
                //AsileList.Clear();
                AsileList_His.Clear();
                
                for (int i = 0; i < intCount; i++)
                {
                    if (AsileListTemp[i].PaKind == "0")
                    {
                        AsileList_His.Add(new AsileModel()
                        {
                            VendBoxCode = AsileListTemp[i].VendBoxCode,
                            PaId = AsileListTemp[i].PaId,
                            PaCode = AsileListTemp[i].PaCode,
                            SellPrice = AsileListTemp[i].SellPrice,
                            PaStackNum = AsileListTemp[i].PaStackNum,
                            SurNum = AsileListTemp[i].SurNum,
                            SpringNum = AsileListTemp[i].SpringNum,
                            PaKind = AsileListTemp[i].PaKind,
                            PaStatus = AsileListTemp[i].PaStatus,
                            McdCode = AsileListTemp[i].McdCode,
                            McdPicName = AsileListTemp[i].McdPicName,
                            McdName = AsileListTemp[i].McdName,
                            McdContent = AsileListTemp[i].McdContent,
                            PaAdvertFile = AsileListTemp[i].PaAdvertFile,
                            TrayIndex = AsileListTemp[i].TrayIndex,
                            RowIndex = AsileListTemp[i].TrayIndex,
                            ColumnIndex = AsileListTemp[i].ColumnIndex,
                            Unit = AsileListTemp[i].Unit,
                            IsNew = AsileListTemp[i].IsNew,
                            IsQueryStatus = true,
                            IsFree = AsileListTemp[i].IsFree,
                            PiCi = AsileListTemp[i].PiCi,
                            ProductDate = AsileListTemp[i].ProductDate,
                            Manufacturer = AsileListTemp[i].Manufacturer,
                            GoodsSpec = AsileListTemp[i].GoodsSpec,
                            MaxValidDate = AsileListTemp[i].MaxValidDate,
                            DrugType = AsileListTemp[i].DrugType,
                            SaleNum = AsileListTemp[i].SaleNum,
                            SaleMoney = AsileListTemp[i].SaleMoney,
                            DetailInfo = AsileListTemp[i].DetailInfo,
                            SellModel = AsileListTemp[i].SellModel,
                            McdSaleType = AsileListTemp[i].McdSaleType,
                        });
                    }
                }

                #region 重新计算各货道的布局所在行数

                ////int intMaxRowIndex = AsileList_His.Max(item => item.TrayIndex) + 1;
                ////bool blnIsAsile = false;// 是否存在该行的货道
                ////int intEmptyNum = 0;// 不存在货道的行数记录总数量
                ////for (int i = 0; i < intMaxRowIndex; i++)
                ////{
                ////    blnIsAsile = false;
                ////    for (int j = 0; j < AsileList_His.Count; j++)
                ////    {
                ////        if (AsileList_His[j].TrayIndex == i)
                ////        {
                ////            // 存在
                ////            blnIsAsile = true;
                ////            AsileList_His[j].RowIndex = AsileList_His[j].RowIndex - intEmptyNum;
                ////        }
                ////    }
                ////    if (!blnIsAsile)
                ////    {
                ////        intEmptyNum++;
                ////    }
                ////}

                #endregion

                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                AsileList = AsileList_His;
            }
        }

        /// <summary>
        /// 货柜—加载某货柜货道信息列表
        /// </summary>
        /// <param name="vendBoxCode">货柜编号</param>
        /// <returns></returns>
        public bool LoadAsileInfo_VendBox()
        {
            bool result = false;

            AsileList_Current.Clear();

            try
            {
                for (int i = 0; i < AsileList.Count; i++)
                {
                    if (AsileList[i].VendBoxCode == CurrentVendBox)
                    {
                        AsileList_Current.Add(AsileList[i]);
                    }
                }

                result = true;
            }
            catch
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// 根据货道外部编号获取货道索引值
        /// </summary>
        /// <param name="paCode">货道外部编号</param>
        /// <param name="paIndex">货道索引值</param>
        /// <returns>结果代码</returns>
        public int GetPaIndex(string paCode, out int paIndex)
        {
            int intErrCode = -1;
            bool blnIsPa = false;
            paIndex = 0;

            int intPalistCount = AsileList.Count;
            for (int i = 0; i < intPalistCount; i++)
            {
                if (AsileList[i].PaCode == paCode)
                {
                    paIndex = i;
                    blnIsPa = true;
                    intErrCode = 0;
                    break;
                }
            }

            if (!blnIsPa)
            {
                // 没有该货道
                return intErrCode;
            }

            return intErrCode;
        }

        /// <summary>
        /// 更新货道价格/弹簧圈数/状态/销售模式
        /// </summary>
        /// <param name="paCode">货道外部编号</param>
        /// <param name="sellPrice">货道价格</param>
        /// <param name="springNum">弹簧圈数</param>
        /// <param name="asileKind">货道类型 0：正常 1：暂停服务</param>
        /// <param name="sellModel">销售模式 0：正常销售 1：赠品</param>
        /// <returns>结果</returns>
        public bool UpdateAsileInfo(string paCode,string sellPrice,string springNum,string asileKind,string sellModel)
        {
            bool result = false;
            string strSql = string.Empty;

            int intPaListCount = AsileList.Count;
            string strVendBoxCode = "1";
            for (int i = 0; i < intPaListCount; i++)
            {
                if (AsileList[i].PaCode == paCode)
                {
                    strVendBoxCode = AsileList[i].VendBoxCode;
                    strSql = "update t_vm_painfo set sellprice = '" + sellPrice + "',springnum = " + springNum +
                        ",pakind = '" + asileKind + "',SellModel = '" + sellModel + "' where paid = '" + AsileList[i].PaId + "' and VendBoxCode = '" + strVendBoxCode + "'";
                    DbOper dbOper = new DbOper();
                    dbOper.DbFileName = _m_DbFileName;
                    dbOper.ConnType = ConnectType.CloseConn;
                    result = dbOper.excuteSql(strSql);
                    dbOper.closeConnection();

                    if (result)
                    {
                        AsileList[i].SellPrice = sellPrice;
                        AsileList[i].PaKind = asileKind;
                        AsileList[i].SpringNum = Convert.ToInt32(springNum);
                        AsileList[i].SellModel = ConvertSellModel(sellModel);
                    }
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// 更新货道商品
        /// </summary>
        /// <param name="paCode">货道外部编号</param>
        /// <param name="mcdCode">货道商品编码</param>
        /// <returns>结果</returns>
        public bool UpdateAsileGoods(string paCode, GoodsModel mcdInfo)
        {
            bool result = false;
            string strSql = string.Empty;

            string strVendBoxCode = "1";
            int intPaListCount = AsileList.Count;
            for (int i = 0; i < intPaListCount; i++)
            {
                if (AsileList[i].PaCode == paCode)
                {
                    strVendBoxCode = AsileList[i].VendBoxCode;
                    strSql = "update t_vm_painfo set mcdCode = '" + mcdInfo.McdCode + "',PiCi = '',ProductDate = ''," +
                        "MaxValidDate = '',McdSaleType = '" + mcdInfo.McdSaleType + "'  " +
                        "where paid = '" + AsileList[i].PaId + "' and VendBoxCode = '" + strVendBoxCode + "'" ;
                    DbOper dbOper = new DbOper();
                    dbOper.DbFileName = _m_DbFileName;
                    dbOper.ConnType = ConnectType.CloseConn;
                    result = dbOper.excuteSql(strSql);
                    dbOper.closeConnection();

                    if (result)
                    {
                        AsileList[i].McdCode = mcdInfo.McdCode;
                        AsileList[i].McdName = mcdInfo.McdName;
                        AsileList[i].McdContent = mcdInfo.McdContent;
                        AsileList[i].McdPicName = mcdInfo.PicName;
                        AsileList[i].IsFree = mcdInfo.IsFree;
                        AsileList[i].PiCi = string.Empty;
                        AsileList[i].ProductDate = string.Empty;
                        AsileList[i].Manufacturer = mcdInfo.Manufacturer;
                        AsileList[i].GoodsSpec = mcdInfo.GoodsSpec;
                        AsileList[i].MaxValidDate = string.Empty;
                        AsileList[i].DrugType = mcdInfo.DrugType;
                        AsileList[i].Unit = mcdInfo.Unit;
                        AsileList[i].DetailInfo = mcdInfo.DetailInfo;
                        AsileList[i].McdSaleType = mcdInfo.McdSaleType;
                    }
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// 移除货道商品
        /// </summary>
        /// <param name="paCode">货道外部编号</param>
        /// <returns>结果</returns>
        public bool RemoveAsileGoods(string paCode)
        {
            bool result = false;
            string strSql = string.Empty;

            string strVendBoxCode = "1";
            int intPaListCount = AsileList.Count;
            for (int i = 0; i < intPaListCount; i++)
            {
                if (AsileList[i].PaCode == paCode)
                {
                    strVendBoxCode = AsileList[i].VendBoxCode;
                    strSql = "update t_vm_painfo set mcdCode = '',PiCi = '',ProductDate = '',MaxValidDate = '',McdSaleType = '0'  " +
                        "where paid = '" + AsileList[i].PaId + "' and VendBoxCode = '" + strVendBoxCode + "'";
                    DbOper dbOper = new DbOper();
                    dbOper.DbFileName = _m_DbFileName;
                    dbOper.ConnType = ConnectType.CloseConn;
                    result = dbOper.excuteSql(strSql);
                    dbOper.closeConnection();

                    if (result)
                    {
                        AsileList[i].McdCode = string.Empty;
                        AsileList[i].McdName = string.Empty;
                        AsileList[i].McdContent = string.Empty;
                        AsileList[i].McdPicName = string.Empty;
                        AsileList[i].IsFree = "0";
                        AsileList[i].PiCi = string.Empty;
                        AsileList[i].ProductDate = string.Empty;
                        AsileList[i].Manufacturer = string.Empty;
                        AsileList[i].GoodsSpec = string.Empty;
                        AsileList[i].MaxValidDate = string.Empty;
                        AsileList[i].DrugType = "0";
                        AsileList[i].Unit = string.Empty;
                        AsileList[i].DetailInfo = string.Empty;
                        AsileList[i].McdSaleType = BusinessEnum.McdSaleType.Normal;
                    }
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// 移除所有货道上的商品
        /// </summary>
        /// <returns></returns>
        public bool ClearAllAsileGoods()
        {
            bool result = false;
            string strSql = string.Empty;

            DbOper dbOper = new DbOper();
            dbOper.DbFileName = _m_DbFileName;
            dbOper.ConnType = ConnectType.CloseConn;
            strSql = "update t_vm_painfo set mcdCode = '',PiCi = '',ProductDate = '',MaxValidDate = '',McdSaleType = '0'  ";

            result = dbOper.excuteSql(strSql);
            if (result)
            {
                int intPaListCount = AsileList.Count;
                for (int i = 0; i < intPaListCount; i++)
                {
                    AsileList[i].McdCode = string.Empty;
                    AsileList[i].McdName = string.Empty;
                    AsileList[i].McdContent = string.Empty;
                    AsileList[i].McdPicName = string.Empty;
                    AsileList[i].IsFree = "0";
                    AsileList[i].PiCi = string.Empty;
                    AsileList[i].ProductDate = string.Empty;
                    AsileList[i].Manufacturer = string.Empty;
                    AsileList[i].GoodsSpec = string.Empty;
                    AsileList[i].MaxValidDate = string.Empty;
                    AsileList[i].DrugType = "0";
                    AsileList[i].Unit = string.Empty;
                    AsileList[i].DetailInfo = string.Empty;
                    AsileList[i].McdSaleType = BusinessEnum.McdSaleType.Normal;
                }
            }
            dbOper.closeConnection();
            return result;
        }

        public bool UpdateTrayPrice(string trayCode,string sellPrice,string vendBoxCode)
        {
            bool result = false;
            string strSql = string.Empty;

            ////int intVendBoxCode = Convert.ToInt32(vendBoxCode);
            int intPaListCount = AsileList.Count;
            for (int i = 0; i < intPaListCount; i++)
            {
                if ((AsileList[i].TrayIndex == Convert.ToInt32(trayCode)) && (AsileList[i].VendBoxCode == vendBoxCode))
                {
                    strSql = "update t_vm_painfo set sellprice = '" + sellPrice + "' " +
                        "where paid = '" + AsileList[i].PaId + "' and VendBoxCode = '" + vendBoxCode + "'";
                    DbOper dbOper = new DbOper();
                    dbOper.DbFileName = _m_DbFileName;
                    dbOper.ConnType = ConnectType.CloseConn;
                    result = dbOper.excuteSql(strSql);
                    dbOper.closeConnection();

                    if (result)
                    {
                        AsileList[i].SellPrice = sellPrice;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 商品上架补货
        /// </summary>
        /// <param name="operType">操作类型 1：单货道设置 2：托盘设置  3：整机设置</param>
        /// <param name="putType">补货类型 0：补满 1：其它数量</param>
        /// <param name="paCode">货道外部编号</param>
        /// <param name="value">补货后的数量</param>
        /// <returns>结果 False：失败 True：成功</returns>
        public bool PutGoods(string vendBoxCode,string operType, string putType, string paCode, string value,out string mcdCode)
        {
            bool result = false;

            string strOldValue = string.Empty;
            mcdCode = string.Empty;

            string strFiled = "SurNum";

            int intValue = Convert.ToInt32(value);

            int intPaListCount = AsileList.Count;
            string strSql = string.Empty;
            string strVendBoxCode = "1";

            #region 单货道设置

            if (operType == "1")
            {
                // 单货道设置
                for (int i = 0; i < intPaListCount; i++)
                {
                    if (AsileList[i].PaCode == paCode)
                    {
                        mcdCode = AsileList[i].McdCode;
                        strVendBoxCode = AsileList[i].VendBoxCode;
                        strOldValue = AsileList[i].SurNum.ToString();

                        if (putType == "0")
                        {
                            // 补满
                            intValue = AsileList[i].SpringNum;
                        }

                        AsileList[i].SurNum = intValue;

                        // 保存到数据库中
                        strSql = "update T_VM_PAINFO set [" + strFiled + "] = " + intValue +
                            " where [paid]='" + AsileList[i].PaId + "' and vendboxcode = '" + strVendBoxCode + "'";

                        DbOper dbOper = new DbOper();
                        dbOper.DbFileName = _m_DbFileName;
                        dbOper.ConnType = ConnectType.CloseConn;
                        result = dbOper.excuteSql(strSql);
                        dbOper.closeConnection();

                        if (!result)
                        {
                            AsileList[i].SurNum = Convert.ToInt32(strOldValue);
                        }

                        break;
                    }
                }
            }

            #endregion

            #region 托盘设置

            if (operType == "2")
            {
                // 托盘设置
                for (int i = 0; i < intPaListCount; i++)
                {
                    if ((AsileList[i].TrayIndex == Convert.ToInt32(paCode)) && 
                        (AsileList[i].VendBoxCode == vendBoxCode))
                    {
                        strOldValue = AsileList[i].SurNum.ToString();

                        if (putType == "0")
                        {
                            // 补满
                            intValue = AsileList[i].SpringNum;
                        }
                        AsileList[i].SurNum = intValue;

                        // 保存到数据库中
                        strSql = "update T_VM_PAINFO set [" + strFiled + "] = " + intValue + 
                            " where [paId]='" + AsileList[i].PaId + "' and vendboxcode = '" + vendBoxCode + "'";

                        DbOper dbOper = new DbOper();
                        dbOper.DbFileName = _m_DbFileName;
                        dbOper.ConnType = ConnectType.CloseConn;
                        result = dbOper.excuteSql(strSql);
                        dbOper.closeConnection();

                        if (!result)
                        {
                            AsileList[i].SurNum = Convert.ToInt32(strOldValue);
                        }
                    }
                }
            }

            #endregion

            #region 整机设置

            if (operType == "3")
            {
                // 整机价格设置
                // 保存到数据库中
                if (putType == "0")
                {
                    // 补满
                    strSql = "update T_VM_PAINFO set [" + strFiled + "] = [SpringNum]";
                }
                else
                {
                    strSql = "update T_VM_PAINFO set [" + strFiled + "] = SurNum + " + intValue + "";
                }

                DbOper dbOper = new DbOper();
                dbOper.DbFileName = _m_DbFileName;
                dbOper.ConnType = ConnectType.CloseConn;
                result = dbOper.excuteSql(strSql);
                dbOper.closeConnection();

                if (result)
                {
                    for (int i = 0; i < intPaListCount; i++)
                    {
                        strOldValue = AsileList[i].SurNum.ToString();

                        if (putType == "0")
                        {
                            AsileList[i].SurNum = AsileList[i].SpringNum;
                        }
                        else
                        {
                            AsileList[i].SurNum = intValue;
                        }
                    }
                }
            }

            #endregion

            ////paFactNum = intValue;

            return result;
        }

        /// <summary>
        /// 检测某货道的商品库存是否还有
        /// </summary>
        /// <param name="paCode">货道编号</param>
        /// <returns>结果 False：库存不足 True：库存足</returns>
        public bool CheckGoodsStock(string vendBoxCode,string paCode,BusinessEnum.ControlSwitch isRunStock)
        {
            int intStockNum = 0;
            bool result = false;

            string strPaId = string.Empty;

            // 检测是否启用库存
            if (isRunStock ==  BusinessEnum.ControlSwitch.Stop)
            {
                // 不启用库存
                return true;
            }

            int intErrCode = GetPaId(paCode, out strPaId);
            if (intErrCode != 0)
            {
                return false;
            }

            DbOper dbOper = new DbOper();
            dbOper.DbFileName = _m_DbFileName;

            try
            {
                string strSql = @"select SurNum  
                    from T_VM_PAINFO where PaId = '" + strPaId + "' and vendboxcode = '" + vendBoxCode + "'";

                DataSet dataSet = dbOper.dataSet(strSql);

                if (dataSet.Tables.Count > 0)
                {
                    if (dataSet.Tables[0].Rows.Count > 0)
                    {
                        string strStockNum = dataSet.Tables[0].Rows[0]["SurNum"].ToString();
                        if (!string.IsNullOrEmpty(strStockNum))
                        {
                            intStockNum = Convert.ToInt32(strStockNum);
                            if (intStockNum > 0)
                            {
                                // 库存够
                                result = true;
                            }
                        }
                    }
                }

                // 如果数据库中的库存够，则检查内存中的该货道库存是否够
                if (result)
                {
                    for (int i = 0; i < AsileList.Count; i++)
                    {
                        if ((AsileList[i].PaId == strPaId) && (AsileList[i].VendBoxCode == vendBoxCode))
                        {
                            if (AsileList[i].SurNum < 1)
                            {
                                result = false;
                            }
                            break;
                        }
                    }
                }
            }
            catch
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// 减少货道库存数据
        /// </summary>
        /// <param name="vendBoxCode">货柜编号</param>
        /// <param name="paCode">货道外部编号</param>
        /// <param name="paIndex">货道索引</param>
        /// <param name="isEmpty">是否清空库存，False：否 True：是</param>
        /// <returns>结果 False：失败 True：成功</returns>
        public bool DecuAsileStock(int vendBoxCode, string paCode, int paIndex, bool isEmpty)
        {
            bool result = false;

            try
            {
                string strSql = string.Empty;
                if (isEmpty)
                {
                    strSql = "update T_VM_PAINFO set [SurNum] = 0 where [paId]='" + AsileList[paIndex].PaId + "' and vendboxcode = '" + vendBoxCode + "'";
                }
                else
                {
                    strSql = "update T_VM_PAINFO set [SurNum] = [SurNum] - 1 where [paId]='" + AsileList[paIndex].PaId + "' and vendboxcode = '" + vendBoxCode + "'";
                }
                DbOper dbOper = new DbOper();
                dbOper.DbFileName = _m_DbFileName;
                dbOper.ConnType = ConnectType.CloseConn;

                // 保存该货道的商品余量库存数据
                result = dbOper.excuteSql(strSql);
                if (result)
                {
                    // 减少该货道的内存商品余量
                    if (isEmpty)
                    {
                        AsileList[paIndex].SurNum = 0;
                    }
                    else
                    {
                        AsileList[paIndex].SurNum -= 1;
                    }
                    //int intPaIndex = 0;
                    //int intErrCode = GetPaIndex(paCode, out intPaIndex);
                    //if (intErrCode == 0)
                    //{
                    //    AsileList[intPaIndex].SurNum -= 1;
                    //}
                }

                dbOper.closeConnection();

                // 保存待发数据
            }
            catch
            {
            }

            return result;
        }

        /// <summary>
        /// 更新货道商品生产批次、生产日期、最大有效期
        /// </summary>
        /// <param name="paCode">货道外部编号</param>
        /// <param name="mcdCode">货道商品生产批次</param>
        /// <returns>结果</returns>
        public bool UpdateAsileBatch(string paCode, string piCi, string productDate, string maxValidDate,out string mcdCode)
        {
            bool result = false;
            string strSql = string.Empty;
            mcdCode = string.Empty;

            string strVendBoxCode = "1";
            int intPaListCount = AsileList.Count;
            for (int i = 0; i < intPaListCount; i++)
            {
                if (AsileList[i].PaCode == paCode)
                {
                    mcdCode = AsileList[i].McdCode;
                    strVendBoxCode = AsileList[i].VendBoxCode;
                    strSql = "update t_vm_painfo set PiCi = '" + piCi + "',ProductDate = '" + productDate + "'," +
                        "MaxValidDate = '" + maxValidDate + "' " +  
                        "where paid = '" + AsileList[i].PaId + "' and vendboxcode = '" + strVendBoxCode + "'";
                    DbOper dbOper = new DbOper();
                    dbOper.DbFileName = _m_DbFileName;
                    dbOper.ConnType = ConnectType.CloseConn;
                    result = dbOper.excuteSql(strSql);
                    dbOper.closeConnection();

                    if (result)
                    {
                        AsileList[i].PiCi = piCi;
                        AsileList[i].ProductDate = productDate;
                        AsileList[i].MaxValidDate = maxValidDate;
                    }
                    break;
                }
            }
            return result;
        }

        public bool InsertAsileInfo()
        {
            bool result = false;

            string strSql = string.Empty;
            DbOper dbOper = new DbOper();
            dbOper.DbFileName = _m_DbFileName;
            dbOper.ConnType = ConnectType.CloseConn;

            string strStr = string.Empty;
            for(int i = 1; i < 7; i++)
            {
                switch(i)
                {
                    case 1:
                        strStr = "A";
                        break;
                    case 2:
                        strStr = "B";
                        break;
                    case 3:
                        strStr = "C";
                        break;
                    case 4:
                        strStr = "D";
                        break;
                    case 5:
                        strStr = "E";
                        break;
                    case 6:
                        strStr = "F";
                        break;
                }
                for(int j = 1; j < 10 ; j++)
                {
                    strSql = @"insert into t_vm_painfo(TrayNum,IsNew,PaCode_Num,PaCode_Str,
                        SpringNum,PaAdvertFile,VendBoxCode,PaId,PaStatus,PaKind,
    McdPicName,McdName,SurNum,PaStackNum,SellPrice,McdCode,PaCode) 
            values('" + i.ToString() + "','0','" + i.ToString() + j.ToString() + "','" + strStr + j.ToString() + 
                      "',5,'',1,'" + i.ToString() + j.ToString() + "','02','0'," +
                      "'','',6,0,1,'001','" + strStr + j.ToString() + "')";
                    result = dbOper.excuteSql(strSql);
                }
                
            }
            
            dbOper.closeConnection();

            return result;
        }

        public bool UpdateAsileKind(int paIndex)
        {
            bool result = false;

            try
            {
                string strSql = string.Empty;
                strSql = "update T_VM_PAINFO set pakind = '2' where [paId]='" + AsileList[paIndex].PaId + "'";
                DbOper dbOper = new DbOper();
                dbOper.DbFileName = _m_DbFileName;
                dbOper.ConnType = ConnectType.CloseConn;

                dbOper.closeConnection();
            }
            catch
            {
            }

            return result;
        }

        #endregion

        #region 升降机公有函数

        /// <summary>
        /// 升降机上下移动码盘配置链式表
        /// </summary>
        public List<UpDownNumsModel> UpDown_UpDownNumsList = new List<UpDownNumsModel>();

        /// <summary>
        /// 加载升降机上下码盘配置信息
        /// </summary>
        /// <returns></returns>
        public bool UpDown_LoadUpDownCodeNum()
        {
            bool result = false;

            UpDown_UpDownNumsList.Clear();

            DbOper dbOper = new DbOper();
            dbOper.DbFileName = _m_DbFileName;

            try
            {
                string strSql = @"select VendBoxCode,TrayNum,CodeNum from T_VM_UPDOWN_CODENUM";
                DataSet dataSet = dbOper.dataSet(strSql);
                if (dataSet.Tables.Count > 0)
                {
                    int recordCount = dataSet.Tables[0].Rows.Count;
                    for (int i = 0; i < recordCount; i++)
                    {
                        UpDown_UpDownNumsList.Add(new UpDownNumsModel()
                        {
                            VendBoxCode = dataSet.Tables[0].Rows[i]["VendBoxCode"].ToString(),
                            TrayNum = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TrayNum"].ToString()),
                            UpDownCodeNums = Convert.ToInt32(dataSet.Tables[0].Rows[i]["CodeNum"].ToString())
                        });
                    }
                }

                result = true;
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
        /// 根据货柜编号、托盘编号获取码盘值
        /// </summary>
        /// <param name="vendBoxCode"></param>
        /// <param name="trayNum"></param>
        /// <returns></returns>
        public int UpDown_GetUpDownCodeNum(string vendBoxCode, int trayNum)
        {
            int intCodeNum = 0;

            for (int i = 0; i < UpDown_UpDownNumsList.Count; i++)
            {
                if ((UpDown_UpDownNumsList[i].VendBoxCode == vendBoxCode) &&
                    (UpDown_UpDownNumsList[i].TrayNum == trayNum))
                {
                    intCodeNum = UpDown_UpDownNumsList[i].UpDownCodeNums;
                    break;
                }
            }

            return intCodeNum;
        }

        /// <summary>
        /// 根据货柜编号、货道横向位置获取左右码盘值
        /// </summary>
        /// <param name="vendBoxCode"></param>
        /// <param name="asileNum"></param>
        /// <returns></returns>
        public int UpDown_GetLeftRightCodeNum(string vendBoxCode, string asileNum,BusinessEnum.SellGoodsType lifterType,
            int UpDownLeftRightNum_Left, int UpDownLeftRightNum_Center, int UpDownLeftRightNum_Right)
        {
            int intCodeNum = 0;
            if (lifterType == BusinessEnum.SellGoodsType.Lifter_Comp)
            {
                // 复杂型升降机
                switch (asileNum)
                {
                    case "1":
                        intCodeNum = UpDownLeftRightNum_Left;//140;//0;
                        break;
                    case "2":
                        intCodeNum = UpDownLeftRightNum_Center;//101;//14;
                        break;
                    case "3":
                        intCodeNum = UpDownLeftRightNum_Right;//62;//31;
                        break;
                    default:
                        intCodeNum = 0;
                        break;
                }
            }
            else
            {
                // 简易型升降机
                intCodeNum = 500;
            }
            return intCodeNum;
        }

        /// <summary>
        /// 更新保存某托盘的上下移动码盘值
        /// </summary>
        /// <param name="vendBoxCode"></param>
        /// <param name="trayNum"></param>
        /// <param name="codeNums"></param>
        /// <returns></returns>
        public bool UpDown_UpdateUpDownCodeNum(string vendBoxCode, int trayNum,int codeNums)
        {
            bool result = false;

            try
            {
                string strSql = "update T_VM_UPDOWN_CODENUM set CodeNum = " + codeNums + " " +
                        "where vendboxcode = '" + vendBoxCode + "' and TrayNum = " + trayNum;
                DbOper dbOper = new DbOper();
                dbOper.DbFileName = _m_DbFileName;
                dbOper.ConnType = ConnectType.CloseConn;
                result = dbOper.excuteSql(strSql);
                if (result)
                {
                    for (int i = 0; i < UpDown_UpDownNumsList.Count; i++)
                    {
                        if ((UpDown_UpDownNumsList[i].VendBoxCode == vendBoxCode) &&
                            (UpDown_UpDownNumsList[i].TrayNum == trayNum))
                        {
                            UpDown_UpDownNumsList[i].UpDownCodeNums = codeNums;
                            break;
                        }
                    }
                }
                dbOper.closeConnection();
            }
            catch
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// 更新保存某升降机型货柜的出货延时等参数
        /// </summary>
        /// <param name="vendBoxCode"></param>
        /// <param name="upDownDelayTimeNums"></param>
        /// <param name="upDownSendGoodsTimes"></param>
        /// <returns></returns>
        public bool UpDown_Update_Times(string vendBoxCode, string upDownDelayTimeNums, string upDownSendGoodsTimes)
        {
            bool result = false;

            try
            {
                string strSql = "update T_VM_VENDBOX set UpDownDelayTimeNums = " + upDownDelayTimeNums + ",UpDownSendGoodsTimes=" + upDownSendGoodsTimes +
                        " where vendboxcode = '" + vendBoxCode + "' ";
                DbOper dbOper = new DbOper();
                dbOper.DbFileName = _m_DbFileName;
                dbOper.ConnType = ConnectType.CloseConn;
                result = dbOper.excuteSql(strSql);
                if (result)
                {
                    for (int i = 0; i < VendBoxList.Count; i++)
                    {
                        if (VendBoxList[i].VendBoxCode == vendBoxCode)
                        {
                            VendBoxList[i].UpDownDelayTimeNums = upDownDelayTimeNums;
                            VendBoxList[i].UpDownSendGoodsTimes = upDownSendGoodsTimes;
                            break;
                        }
                    }
                    for (int i = 0; i < VendBoxList_Lifter.Count; i++)
                    {
                        if (VendBoxList_Lifter[i].VendBoxCode == vendBoxCode)
                        {
                            VendBoxList_Lifter[i].UpDownDelayTimeNums = upDownDelayTimeNums;
                            VendBoxList_Lifter[i].UpDownSendGoodsTimes = upDownSendGoodsTimes;
                            break;
                        }
                    }
                }
                dbOper.closeConnection();
            }
            catch
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// 更新保存某升降机型货柜的通信端口等参数
        /// </summary>
        /// <param name="vendBoxCode"></param>
        /// <param name="upDownDelayTimeNums"></param>
        /// <param name="upDownSendGoodsTimes"></param>
        /// <returns></returns>
        public bool UpDown_Update_Port(string vendBoxCode, string port, string upDownSellModel, string upDownIsQueryElectStatus)
        {
            bool result = false;

            try
            {
                string strSql = "update T_VM_VENDBOX set ShippPort = '" + port + "',UpDownSellModel='" + upDownSellModel +
                        "',UpDownIsQueryElectStatus = '" +  upDownIsQueryElectStatus + "' where vendboxcode = '" + vendBoxCode + "' ";
                DbOper dbOper = new DbOper();
                dbOper.DbFileName = _m_DbFileName;
                dbOper.ConnType = ConnectType.CloseConn;
                result = dbOper.excuteSql(strSql);
                if (result)
                {
                    for (int i = 0; i < VendBoxList.Count; i++)
                    {
                        if (VendBoxList[i].VendBoxCode == vendBoxCode)
                        {
                            VendBoxList[i].ShippPort = port;
                            VendBoxList[i].UpDownSellModel = upDownSellModel;
                            VendBoxList[i].UpDownIsQueryElectStatus = FunHelper.ChangeControlSwitch(upDownIsQueryElectStatus);
                            break;
                        }
                    }
                    for (int i = 0; i < VendBoxList_Lifter.Count; i++)
                    {
                        if (VendBoxList_Lifter[i].VendBoxCode == vendBoxCode)
                        {
                            VendBoxList_Lifter[i].ShippPort = port;
                            VendBoxList_Lifter[i].UpDownSellModel = upDownSellModel;
                            VendBoxList_Lifter[i].UpDownIsQueryElectStatus = FunHelper.ChangeControlSwitch(upDownIsQueryElectStatus);
                            break;
                        }
                    }
                }
                dbOper.closeConnection();
            }
            catch
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// 检测所有货柜中是否存在弹簧出货方式
        /// </summary>
        /// <returns>False：不存在弹簧出货 True：存在弹簧出货</returns>
        public bool UpDown_CheckIsExistSpring()
        {
            int intNum = 0;
            for (int i = 0; i < VendBoxList.Count; i++)
            {
                if (VendBoxList[i].SellGoodsType == Business.Enum.BusinessEnum.SellGoodsType.Spring)
                {
                    intNum++;
                    break;
                }
            }
            if (intNum == 0)
            {
                return false;
            }
            return true;
        }

        #endregion

        #region 货柜公有函数

        /// <summary>
        /// 加载货柜信息列表
        /// </summary>
        /// <returns>加载结果 True：成功 False：失败</returns>
        public bool LoadVendBox()
        {
            bool result = false;

            VendBoxList.Clear();
            VendBoxList_Lifter.Clear();

            DbOper dbOper = new DbOper();
            dbOper.DbFileName = _m_DbFileName;

            try
            {
                string strSql = string.Empty;

                strSql += @"select VendBoxCode,SellGoodsType,TmpControlModel,TargetTmp,
                    OutTmpWarnValue,ShippPort,IsControlCircle,UpDownSellModel,
                    UpDownDelayTimeNums,UpDownSendGoodsTimes,UpDownIsQueryElectStatus,
                    TmpRunModel,TargetTmp,TmpBeginTime1,TmpEndTime1,TmpBeginTime2,TmpEndTime2 
                    from T_VM_VENDBOX";

                DataSet dataSet = dbOper.dataSet(strSql);
                if (dataSet.Tables.Count > 0)
                {
                    int recordCount = dataSet.Tables[0].Rows.Count;
                    for (int i = 0; i < recordCount; i++)
                    {
                        for (int j = 0; j < VendBoxList_Pa.Count; j++)
                        {
                            if (VendBoxList_Pa[j].VendBoxCode == dataSet.Tables[0].Rows[i]["VendBoxCode"].ToString())
                            {
                                VendBoxList.Add(new VendBoxCodeModel()
                                {
                                    VendBoxCode = dataSet.Tables[0].Rows[i]["VendBoxCode"].ToString(),
                                    VendBoxStatus = true,
                                    SellGoodsType = ConvertSellGoodsType(dataSet.Tables[0].Rows[i]["SellGoodsType"].ToString()),
                                    TmpControlModel = ConvertTmpControlModel(dataSet.Tables[0].Rows[i]["TmpControlModel"].ToString()),
                                    TargetTmp = dataSet.Tables[0].Rows[i]["TargetTmp"].ToString(),
                                    OutTmpWarnValue = dataSet.Tables[0].Rows[i]["OutTmpWarnValue"].ToString(),
                                    ShippPort = dataSet.Tables[0].Rows[i]["ShippPort"].ToString(),
                                    IsControlCircle = dataSet.Tables[0].Rows[i]["IsControlCircle"].ToString(),
                                    UpDownSellModel = dataSet.Tables[0].Rows[i]["UpDownSellModel"].ToString(),
                                    UpDownDelayTimeNums = dataSet.Tables[0].Rows[i]["UpDownDelayTimeNums"].ToString(),
                                    UpDownSendGoodsTimes = dataSet.Tables[0].Rows[i]["UpDownSendGoodsTimes"].ToString(),
                                    UpDownIsQueryElectStatus = FunHelper.ChangeControlSwitch(dataSet.Tables[0].Rows[i]["UpDownIsQueryElectStatus"].ToString()),

                                    RefControl = new DeviceControlModel()
                                    {
                                        ControlModel = dataSet.Tables[0].Rows[i]["TmpRunModel"].ToString(),
                                        BeginTime1 = dataSet.Tables[0].Rows[i]["TmpBeginTime1"].ToString(),
                                        EndTime1 = dataSet.Tables[0].Rows[i]["TmpEndTime1"].ToString(),
                                        BeginTime2 = dataSet.Tables[0].Rows[i]["TmpBeginTime2"].ToString(),
                                        EndTime2 = dataSet.Tables[0].Rows[i]["TmpEndTime2"].ToString()
                                    }
                                });
                                break;
                            }
                        }
                        
                    }
                }

                // 获取出货方式为升降的货柜列表
                for (int i = 0; i < VendBoxList.Count; i++)
                {
                    if ((VendBoxList[i].SellGoodsType == BusinessEnum.SellGoodsType.Lifter_Comp) ||
                        (VendBoxList[i].SellGoodsType == BusinessEnum.SellGoodsType.Lifter_Simple))
                    {
                        VendBoxList_Lifter.Add(VendBoxList[i]);
                    }
                }

                result = true;
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
        /// 根据货柜编码获取货柜索引
        /// </summary>
        /// <param name="vendBoxCode"></param>
        /// <returns></returns>
        public int GetVendBoxIndex(string vendBoxCode)
        {
            int intIndex = 0;
            for (int i = 0; i < VendBoxList.Count; i++)
            {
                if (VendBoxList[i].VendBoxCode == vendBoxCode)
                {
                    intIndex = i;
                    break;
                }
            }

            return intIndex;
        }

        /// <summary>
        /// 更新保存货柜温控相关参数
        /// </summary>
        /// <param name="vendBoxCode"></param>
        /// <param name="upDownDelayTimeNums"></param>
        /// <param name="upDownSendGoodsTimes"></param>
        /// <returns></returns>
        public bool UpdateVendBoxTmpCfg(string vendBoxCode, string targetTmp,string outTmpWarnValue,
            string tmpRunModel, string beginTime1, string endTime1,string beginTime2,string endTime2)
        {
            bool result = false;

            try
            {
                string strSql = string.Empty;
                if (tmpRunModel != "0")
                {
                    strSql = "update T_VM_VENDBOX set TargetTmp = '" + targetTmp + "',OutTmpWarnValue='" + outTmpWarnValue +
                            "',TmpRunModel = '" + tmpRunModel + "' where vendboxcode = '" + vendBoxCode + "' ";
                }
                else
                {
                    strSql = "update T_VM_VENDBOX set TargetTmp = '" + targetTmp + "',OutTmpWarnValue='" + outTmpWarnValue +
                            "',TmpRunModel = '" + tmpRunModel + "',TmpBeginTime1 ='" + beginTime1 + "',TmpEndTime1='" + endTime1 +
                            "',TmpBeginTime2 ='" + beginTime2 + "',TmpEndTime2='" + endTime2 + "' where vendboxcode = '" + vendBoxCode + "' ";
                }
                DbOper dbOper = new DbOper();
                dbOper.DbFileName = _m_DbFileName;
                dbOper.ConnType = ConnectType.CloseConn;
                result = dbOper.excuteSql(strSql);
                if (result)
                {
                    for (int i = 0; i < VendBoxList.Count; i++)
                    {
                        if (VendBoxList[i].VendBoxCode == vendBoxCode)
                        {
                            VendBoxList[i].TargetTmp = targetTmp;
                            VendBoxList[i].OutTmpWarnValue = outTmpWarnValue;
                            VendBoxList[i].RefControl.ControlModel = tmpRunModel;
                            if (tmpRunModel == "0")
                            {
                                VendBoxList[i].RefControl.BeginTime1 = beginTime1;
                                VendBoxList[i].RefControl.EndTime1 = endTime1;
                                VendBoxList[i].RefControl.BeginTime2 = beginTime2;
                                VendBoxList[i].RefControl.EndTime2 = endTime2;
                            }
                            VendBoxList[i].RefControl.IsRefreshCfg = true;
                            break;
                        }
                    }
                }
                dbOper.closeConnection();
            }
            catch
            {
                result = false;
            }

            return result;
        }

        #endregion

        #region 私有函数

        /// <summary>
        /// 根据货道外部编号获取货道主键值
        /// </summary>
        /// <param name="paCode">货道外部编号</param>
        /// <param name="paIndex">货道主键值</param>
        /// <returns>结果代码</returns>
        private int GetPaId(string paCode, out string paID)
        {
            int intErrCode = -1;
            bool blnIsPa = false;
            paID = string.Empty;

            int intPalistCount = AsileList.Count;
            for (int i = 0; i < intPalistCount; i++)
            {
                if (AsileList[i].PaCode == paCode)
                {
                    paID = AsileList[i].PaId;
                    blnIsPa = true;
                    intErrCode = 0;
                    break;
                }
            }

            if (!blnIsPa)
            {
                // 没有该货道
                return intErrCode;
            }

            return intErrCode;
        }

        /// <summary>
        /// 转换出货方式
        /// </summary>
        /// <param name="sellGoodsType"></param>
        /// <returns></returns>
        private BusinessEnum.SellGoodsType ConvertSellGoodsType(string sellGoodsType)
        {
            // 出货方式 0：弹簧方式 1：复杂型升降方式 2：简易型升降方式
            BusinessEnum.SellGoodsType eNumSellGoods = BusinessEnum.SellGoodsType.Spring;
            switch (sellGoodsType)
            {
                case "0":
                    break;
                case "1":// 升降方式
                    eNumSellGoods = BusinessEnum.SellGoodsType.Lifter_Comp;
                    break;
                case "2":// 简易型升降方式
                    eNumSellGoods = BusinessEnum.SellGoodsType.Lifter_Simple;
                    break;
            }

            return eNumSellGoods;
        }

        /// <summary>
        /// 转换温控模式
        /// </summary>
        /// <param name="tmpControlModel"></param>
        /// <returns></returns>
        private BusinessEnum.TmpControlModel ConvertTmpControlModel(string tmpControlModel)
        {
            // 温控方式 0：制冷 1：加热
            BusinessEnum.TmpControlModel eNumTmpControlModel = BusinessEnum.TmpControlModel.Refrigeration;
            switch (tmpControlModel)
            {
                case "0":// 制冷
                    break;
                case "1":// 加热
                    eNumTmpControlModel = BusinessEnum.TmpControlModel.Heating;
                    break;
            }
            return eNumTmpControlModel;
        }

        /// <summary>
        /// 转换货道销售模式
        /// </summary>
        /// <param name="sellModel"></param>
        /// <returns></returns>
        private BusinessEnum.AsileSellModel ConvertSellModel(string sellModel)
        {
            BusinessEnum.AsileSellModel eNumSellModel = BusinessEnum.AsileSellModel.Normal;
            switch (sellModel)
            {
                case "1":// 赠品
                    eNumSellModel = BusinessEnum.AsileSellModel.Gift;
                    break;
            }
            return eNumSellModel;
        }

        #endregion
    }
}
