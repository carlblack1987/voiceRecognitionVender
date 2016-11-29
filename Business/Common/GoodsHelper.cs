#region [ KIMMA Co.,Ltd. Copyright (C) 2014 ]
//-------------------------------------------------------------------------------------
// 开发单位：湖南金码智能设备制造有限公司
// 业务模块：iVend终端软件业务处理类
// 业务功能：商品信息处理类
// 创建标识：2014-06-10		谷霖
//-------------------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Threading;

using KdbPlug;
using Business.Model;
using LitJson;
using JsonPlug;
using Business.Enum;

namespace Business.Common
{
    public class GoodsHelper
    {
        #region 变量声明

        private string _m_DbFileName = "TermInfo.db";

        /// <summary>
        /// 商品信息链式表—全部
        /// </summary>
        public List<GoodsModel> GoodsList_Total = new List<GoodsModel>();

        /// <summary>
        /// 商品信息链式表—上架
        /// </summary>
        public List<GoodsModel> GoodsList_Show = new List<GoodsModel>();

        /// <summary>
        /// 商品类型链式表
        /// </summary>
        public List<GoodsTypeModel> GoodsTypeList = new List<GoodsTypeModel>();

        #endregion

        #region 属性

        /// <summary>
        /// 当前选择的商品类型
        /// </summary>
        public GoodsTypeModel CurrentGoodsType
        {
            get;
            set;
        }

        /// <summary>
        /// 当前选择的商品
        /// </summary>
        public GoodsModel CurrentGoods
        {
            get;
            set;
        }

        /// <summary>
        /// 商品列表总页数
        /// </summary>
        public int GoodsShowPageCount
        {
            get;
            set;
        }

        /// <summary>
        /// 当前页序号
        /// </summary>
        public int CurrentPageNo
        {
            get;
            set;
        }

        #endregion

        #region

        /// <summary>
        /// 加载商品信息列表—全部
        /// </summary>
        /// <returns>加载结果 True：成功 False：失败</returns>
        public bool LoadGoodsInfo_Total()
        {
            bool result = false;

            GoodsList_Total.Clear();

            DbOper dbOper = new DbOper();
            dbOper.DbFileName = _m_DbFileName;

            try
            {
                string strSql = string.Empty;

                strSql += @"select McdCode,McdName,McdContent,Price,
                    PicName,McdType,IsFree,Manufacturer,
                    GoodsSpec,DrugType,DetailInfo,Unit,McdSaleType       
                    from T_MCD_BASEINFO order by id desc";

                DataSet dataSet = dbOper.dataSet(strSql);
                if (dataSet.Tables.Count > 0)
                {
                    int recordCount = dataSet.Tables[0].Rows.Count;
                    for (int i = 0; i < recordCount; i++)
                    {
                        GoodsList_Total.Add(new GoodsModel()
                        {
                            McdCode = dataSet.Tables[0].Rows[i]["McdCode"].ToString(),
                            McdName = dataSet.Tables[0].Rows[i]["McdName"].ToString(),
                            McdContent = dataSet.Tables[0].Rows[i]["McdContent"].ToString(),
                            PicName = dataSet.Tables[0].Rows[i]["PicName"].ToString(),
                            Price = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Price"].ToString()),
                            IsFree = dataSet.Tables[0].Rows[i]["IsFree"].ToString(),
                            Manufacturer = dataSet.Tables[0].Rows[i]["Manufacturer"].ToString(),
                            GoodsSpec = dataSet.Tables[0].Rows[i]["GoodsSpec"].ToString(),
                            DrugType = dataSet.Tables[0].Rows[i]["DrugType"].ToString(),
                            TypeCode = dataSet.Tables[0].Rows[i]["McdType"].ToString(),
                            Unit = dataSet.Tables[0].Rows[i]["Unit"].ToString(),
                            DetailInfo = dataSet.Tables[0].Rows[i]["DetailInfo"].ToString(),
                            McdSaleType = FunHelper.ChangeMcdSaleType(dataSet.Tables[0].Rows[i]["McdSaleType"].ToString()),
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
        /// 加载商品信息列表—上架
        /// </summary>
        /// <param name="pageNum">每页显示的最大行数</param>
        /// <param name="rowNum">每行显示的最大列数</param>
        /// <returns></returns>
        public bool LoadGoodsInfo_Show(int eachPageMaxRowNum,int eachRowMaxNum)
        {
            bool result = false;

            GoodsList_Show.Clear();
            GoodsShowPageCount = 0;
            CurrentPageNo = 1;

            DbOper dbOper = new DbOper();
            dbOper.DbFileName = _m_DbFileName;

            try
            {
                string strSql = string.Empty;

                strSql += @"select distinct t1.mcdcode,t2.mcdname,t2.mcdcontent,t2.picname,t1.sellprice,
                                t2.IsFree,t2.Manufacturer,t2.GoodsSpec,
                                t2.DrugType,t2.McdType,t2.Unit,t2.DetailInfo,t2.McdSaleType         
                                from t_vm_painfo t1,t_mcd_baseinfo t2 
                        where t1.mcdcode = t2.mcdcode and t1.pakind = '0' and t1.sellmodel = '0' order by t1.pacode_num";

                DataSet dataSet = dbOper.dataSet(strSql);
                if (dataSet.Tables.Count > 0)
                {
                    int recordCount = dataSet.Tables[0].Rows.Count;
                    // 获取总页面
                    int intEachMaxNum = eachPageMaxRowNum * eachRowMaxNum;// 每页显示的最大商品数量
                    GoodsShowPageCount = ((recordCount / intEachMaxNum) +
                            ((recordCount % intEachMaxNum) > 0 ? 1 : 0));

                    string strSql12 = string.Empty;
                    string strMcdCode = string.Empty;
                    int intSellPrice = 0;
                    int intSurNum = 0;
                    string strStockNum = string.Empty;
                    DataSet dataSet_Sur = new DataSet();
                    for (int i = 0; i < recordCount; i++)
                    {
                        strMcdCode = dataSet.Tables[0].Rows[i]["McdCode"].ToString();
                        intSellPrice = Convert.ToInt32(dataSet.Tables[0].Rows[i]["sellprice"].ToString());
                        // 查询每个商品当前的库存
                        intSurNum = 0;
                        strSql12 = @"select sum(surnum) as surnum from t_vm_painfo 
                                where pakind = '0' and mcdcode = '" + strMcdCode + "' and sellprice = " + intSellPrice;
                        dataSet_Sur = dbOper.dataSet(strSql12);
                        if (dataSet_Sur.Tables.Count > 0)
                        {
                            if (dataSet_Sur.Tables[0].Rows.Count > 0)
                            {
                                strStockNum = dataSet_Sur.Tables[0].Rows[0]["surnum"].ToString();
                                intSurNum = Convert.ToInt32(strStockNum);
                            }
                        }

                        GoodsList_Show.Add(new GoodsModel()
                        {
                            McdCode = strMcdCode,
                            McdName = dataSet.Tables[0].Rows[i]["McdName"].ToString(),
                            McdContent = dataSet.Tables[0].Rows[i]["McdContent"].ToString(),
                            PicName = dataSet.Tables[0].Rows[i]["PicName"].ToString(),
                            Price = intSellPrice,
                            Unit = dataSet.Tables[0].Rows[i]["Unit"].ToString(),
                            SurNum = intSurNum,
                            IsFree = dataSet.Tables[0].Rows[i]["IsFree"].ToString(),
                            Manufacturer = dataSet.Tables[0].Rows[i]["Manufacturer"].ToString(),
                            GoodsSpec = dataSet.Tables[0].Rows[i]["GoodsSpec"].ToString(),
                            DrugType = dataSet.Tables[0].Rows[i]["DrugType"].ToString(),
                            TypeCode = dataSet.Tables[0].Rows[i]["McdType"].ToString(),
                            DetailInfo = dataSet.Tables[0].Rows[i]["DetailInfo"].ToString(),
                            McdSaleType = FunHelper.ChangeMcdSaleType(dataSet.Tables[0].Rows[i]["McdSaleType"].ToString()),
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
        /// 批量添加商品
        /// </summary>
        /// <param name="goodsListInfo">商品列表的JSON格式</param>
        /// <param name="importType">导入方式 0：本地导入 1：远程下载</param>
        /// <param name="picSourcePath">本地导入时的商品图片来源路径</param>
        /// <returns>结果 0：成功 1：获取商品导入信息失败 2：没有要导入的商品信息 3：导入失败</returns>
        public int ImportGoodsList(string goodsListInfo,string importType,string picSourcePath,out string updateMcdList)
        {
            int intErrCode = 0;
            updateMcdList = string.Empty;

            try
            {
                JsonOper jsonOper = new JsonOper();
                string strResultCode = jsonOper.GetJsonKeyValue(goodsListInfo, "ret");
                if (strResultCode == "0")
                {
                    int intGoodsCount = 0;
                    intGoodsCount = Convert.ToInt32(jsonOper.GetJsonKeyValue(goodsListInfo, "count"));
                    if (intGoodsCount == 0)
                    {
                        intErrCode = 2;// 没有要导入的商品信息
                    }
                    else
                    {
                        string strMcdCode = string.Empty;
                        string strMcdName = string.Empty;
                        string strMcdContent = string.Empty;
                        string strMcdType = string.Empty;
                        string strMcdPic = string.Empty;
                        string strIsFree = "0";
                        string strManufacturer = string.Empty;
                        string strGoodsSpec = string.Empty;
                        string strDrugType = "0";
                        string strDetailInfo = string.Empty;
                        string strMcdUnit = string.Empty;
                        string strMcdSaleType = string.Empty;
                        string strUrl = string.Empty;
                        JsonData jdItems = null;

                        string strSql = string.Empty;

                        DbOper dbOper = new DbOper();
                        dbOper.DbFileName = _m_DbFileName;
                        dbOper.ConnType = ConnectType.CloseConn;
                        DataSet dataSet = new DataSet();

                        jdItems = JsonMapper.ToObject(goodsListInfo);
                        jdItems = jdItems["goodslist"];
                        bool blnIsExit = false;
                        bool result = false;

                        #region 检查是否存在新的字段

                        bool blnIsExitIsFree = false;// 是否存在免费字段 False：不存在 True：存在
                        if (goodsListInfo.IndexOf("isfree") > 10)
                        {
                            blnIsExitIsFree = true;
                        }

                        bool blnIsExitManufacturer = false;// 是否存在生产厂商 False：不存在 True：存在
                        if (goodsListInfo.IndexOf("manufacturer") > 10)
                        {
                            blnIsExitManufacturer = true;
                        }

                        bool blnIsExitUnit = false;// 是否存在单位 False：不存在 True：存在
                        if (goodsListInfo.IndexOf("unit") > 10)
                        {
                            blnIsExitUnit = true;
                        }

                        bool blnIsExitGoodsSpec = false;// 是否存在商品规格 False：不存在 True：存在
                        if (goodsListInfo.IndexOf("goodsspec") > 10)
                        {
                            blnIsExitGoodsSpec = true;
                        }

                        bool blnIsExitDrugType = false;// 是否存在商品标示 False：不存在 True：存在
                        if (goodsListInfo.IndexOf("drugtype") > 10)
                        {
                            blnIsExitDrugType = true;
                        }

                        bool blnIsExitDetail = false;// 是否存在商品详细介绍 False：不存在 True：存在
                        if (goodsListInfo.IndexOf("detail") > 10)
                        {
                            blnIsExitDetail = true;
                        }

                        bool blnIsExitMcdSaleType = false;// 是否存在商品销售类型 False：不存在 True：存在
                        if (goodsListInfo.IndexOf("saletype") > 10)
                        {
                            blnIsExitMcdSaleType = true;
                        }

                        bool blnIsExitPicUrl = false;// 是否存在商品图片下载URL False：不存在 True：存在
                        if (goodsListInfo.IndexOf("picurl") > 10)
                        {
                            blnIsExitPicUrl = true;
                        }

                        #endregion

                        #region 遍历商品导入信息

                        foreach (JsonData item in jdItems)
                        {
                            strMcdCode = item["mcdcode"].ToString();
                            strMcdName = item["mcdname"].ToString();
                            strMcdContent = item["mcdcontent"].ToString();
                            strMcdPic = item["mcdpic"].ToString();
                            strMcdType = item["mcdtype"].ToString();
                            if (blnIsExitIsFree)
                            {
                                strIsFree = item["isfree"].ToString();
                            }
                            if (blnIsExitManufacturer)
                            {
                                strManufacturer = item["manufacturer"].ToString();
                            }
                            if (blnIsExitUnit)
                            {
                                strMcdUnit = item["unit"].ToString();
                            }
                            if (blnIsExitGoodsSpec)
                            {
                                strGoodsSpec = item["goodsspec"].ToString();
                            }
                            if (blnIsExitDrugType)
                            {
                                strDrugType = item["drugtype"].ToString();
                            }
                            if (blnIsExitDetail)
                            {
                                strDetailInfo = item["detail"].ToString();
                                // 转码商品详细介绍中的换行符等
                                strDetailInfo = strDetailInfo.Replace("\\r", "\r");
                                strDetailInfo = strDetailInfo.Replace("\\n", "\n");
                            }

                            if (blnIsExitMcdSaleType)
                            {
                                strMcdSaleType = item["saletype"].ToString();
                            }
                            else
                            {
                                strMcdSaleType = "0";
                            }

                            if (blnIsExitPicUrl)
                            {
                                strUrl = item["picurl"].ToString();
                            }

                            // 检查是否已经存在了该商品，如果已经存在该商品编号，则更新；否则添加
                            strSql = "select McdCode from T_MCD_BASEINFO where mcdcode = '" + strMcdCode + "'";
                            blnIsExit = false;
                            dataSet = dbOper.dataSet(strSql);
                            if (dataSet.Tables.Count > 0)
                            {
                                if (dataSet.Tables[0].Rows.Count > 0)
                                {
                                    blnIsExit = true;
                                }
                            }

                            if (blnIsExit)
                            {
                                // 已经存在，则更新
                                strSql = @"update T_MCD_BASEINFO set McdName = '" + strMcdName + "'," +
                                    "McdContent = '" + strMcdContent + "'," +
                                    "PicName = '" + strMcdPic + "',IsFree = '" + strIsFree + "',manufacturer = '" + strManufacturer +
                                    "',GoodsSpec = '" + strGoodsSpec + "',DrugType = '" + strDrugType + 
                                    "',McdType = '" + strMcdType + "',Unit = '" + strMcdUnit + "',DetailInfo = '" + strDetailInfo + "',McdSaleType = '" + strMcdSaleType + "' where mcdcode = '" + strMcdCode + "'";
                                updateMcdList += strMcdCode + ",";
                            }
                            else
                            {
                                // 不存在，则添加
                                strSql = @"insert into T_MCD_BASEINFO(McdCode,McdName,McdContent,
                                    PicName,Price,IsFree,manufacturer,
                                    GoodsSpec,DrugType,McdType,Unit,DetailInfo,McdSaleType) values('" +
                                    strMcdCode + "','" + strMcdName + "','" + strMcdContent + "','" +
                                    strMcdPic + "'," + "0" + ",'" + strIsFree + "','" + 
                                    strManufacturer + "','" + strGoodsSpec + "','" + 
                                    strDrugType + "','" + strMcdType + "','" + 
                                    strMcdUnit + "','" + strDetailInfo + "','" + strMcdSaleType + "')";
                            }
                            result = dbOper.excuteSql(strSql);
                            if (result)
                            {
                                switch (importType)
                                {
                                    case "0":// 本地导入商品信息
                                        #region 拷贝商品图片
                                        if (File.Exists(picSourcePath + strMcdPic))
                                        {
                                            try
                                            {
                                                // 存在商品图片，拷贝覆盖
                                                File.Copy(picSourcePath + strMcdPic,
                                                    AppDomain.CurrentDomain.BaseDirectory.ToString() + "Images\\GoodsPic\\" + strMcdPic, true);
                                            }
                                            catch
                                            {

                                            }
                                        }
                                        #endregion
                                        break;
                                    case "1":// 远程下载更新商品信息
                                        #region 商品图片信息先保存，再另外下载图片
                                        if (!File.Exists(picSourcePath + strMcdPic))
                                        {
                                            // 添加到商品图片下载中间表
                                            strSql = @"insert into T_MCD_PICTEMP(McdCode,McdPic,Url,Status) 
                                                    values('" + strMcdCode + "','" + strMcdPic + "','" + strUrl + "','0')";
                                            result = dbOper.excuteSql(strSql);
                                        }
                                        #endregion
                                        break;
                                }
                            }
                        }
                        #endregion

                        dbOper.closeConnection();

                        // 更新商品列表
                        LoadGoodsInfo_Total();
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
            return intErrCode;
        }

        public void CheckMcdPicDown()
        {
            DbOper dbOper = new DbOper();
            dbOper.DbFileName = _m_DbFileName;

            try
            {
                dbOper.ConnType = ConnectType.KeepConn;
                string strSql = @"select McdCode,McdPic,Url 
                            from T_MCD_PICTEMP where Status = '0' limit 1";

                DataSet dataSet = dbOper.dataSet(strSql);
                if (dataSet.Tables.Count > 0)
                {
                    int intCount = dataSet.Tables[0].Rows.Count;
                    bool result = false;
                    string strMcdCode = string.Empty;
                    if (intCount > 0)
                    {
                        for (int i = 0; i < intCount; i++)
                        {
                            strMcdCode = dataSet.Tables[0].Rows[i]["McdCode"].ToString();
                            result = DownMcdPicFile(dataSet.Tables[0].Rows[i]["McdPic"].ToString(),
                                dataSet.Tables[0].Rows[i]["Url"].ToString());
                            if (result)
                            {
                                // 下载成功，删除商品图片下载临时表记录
                                strSql = "delete T_MCD_PICTEMP where McdCode = '" + strMcdCode + "' and Status = '0'";

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
        /// 清空商品信息
        /// </summary>
        /// <returns>结果 0：成功 1：失败</returns>
        public int ClearAllGoods()
        {
            bool result = false;
            int intErrCode = 0;
            string strSql = string.Empty;

            DbOper dbOper = new DbOper();
            dbOper.DbFileName = _m_DbFileName;
            dbOper.ConnType = ConnectType.CloseConn;

            strSql = @"delete from T_MCD_BASEINFO ";
            result = dbOper.excuteSql(strSql);

            dbOper.closeConnection();

            if (!result)
            {
                intErrCode = 1;
            }
            else
            {
                GoodsList_Total.Clear();
            }

            return intErrCode;
        }

        /// <summary>
        /// 删除某商品信息
        /// </summary>
        /// <returns>结果 0：成功 1：已经上架了该商品 其它：失败</returns>
        public int DeleteGoods(string mcdCode)
        {
            bool result = false;
            int intErrCode = 0;
            string strSql = string.Empty;

            try
            {
                DbOper dbOper = new DbOper();
                dbOper.DbFileName = _m_DbFileName;
                dbOper.ConnType = ConnectType.CloseConn;

                // 查询是否有货道已经上架了该商品
                strSql = "select McdCode from T_VM_PAINFO where McdCode = '" + mcdCode + "'";
                DataSet dataSet = dbOper.dataSet(strSql);
                if (dataSet.Tables.Count > 0)
                {
                    if (dataSet.Tables[0].Rows.Count > 0)
                    {
                        // 有货道已经上架了该商品
                        intErrCode = 1;
                    }
                }

                if (intErrCode == 0)
                {
                    // 删除商品
                    strSql = @"delete from T_MCD_BASEINFO where mcdcode = '" + mcdCode + "'";
                    result = dbOper.excuteSql(strSql);
                    if (!result)
                    {
                        intErrCode = 3;
                    }
                    else
                    {
                        LoadGoodsInfo_Total();
                    }
                }

                dbOper.closeConnection();
            }
            catch
            {
                intErrCode = 3;
            }

            return intErrCode;
        }

        /// <summary>
        /// 检测某商品库存是否还有
        /// </summary>
        /// <param name="mcdCode">商品编号</param>
        /// <returns>结果 False：库存不足 True：库存足</returns>
        public bool CheckGoodsStock(string mcdCode, int sellPrice,BusinessEnum.ControlSwitch isRunStock)
        {
            int intStockNum = 0;
            bool result = false;

            string strPaId = string.Empty;

            // 检测是否启用库存
            if (isRunStock == BusinessEnum.ControlSwitch.Stop)
            {
                // 不启用库存
                return true;
            }

            DbOper dbOper = new DbOper();
            dbOper.DbFileName = _m_DbFileName;

            try
            {
                string strSql = @"select sum(SurNum) as SurNum 
                    from t_vm_painfo where mcdcode = '" + mcdCode + "' and sellprice = " + sellPrice;

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

                // 如果数据库中的库存够，则检查内存中的该商品库存是否够
                if (result)
                {
                    for (int i = 0; i < GoodsList_Show.Count; i++)
                    {
                        if ((GoodsList_Show[i].McdCode == mcdCode) &&
                            (GoodsList_Show[i].Price == sellPrice))
                        {
                            if (GoodsList_Show[i].SurNum < 1)
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
        /// 加载商品类型信息
        /// </summary>
        /// <returns></returns>
        public bool LoadGoodsTypeList()
        {
            bool result = false;

            GoodsTypeList.Clear();

            DbOper dbOper = new DbOper();
            dbOper.DbFileName = _m_DbFileName;

            try
            {
                string strSql = string.Empty;

                strSql += @"select TypeCode,TypeName     
                    from T_MCD_TYPE order by TypeCode desc";

                DataSet dataSet = dbOper.dataSet(strSql);
                int recordCount = 0;
                if (dataSet.Tables.Count > 0)
                {
                    recordCount = dataSet.Tables[0].Rows.Count;
                    string strTypeCode = string.Empty;
                    int intTypeCount = 0;
                    DataSet ds_Num = new DataSet();
                    for (int i = 0; i < recordCount; i++)
                    {
                        strTypeCode = dataSet.Tables[0].Rows[i]["TypeCode"].ToString();

                        #region 获取该类别商品下的商品数量
                        intTypeCount = 0;
                        strSql = @"select distinct t2.mcdname    
                             from t_vm_painfo t1,t_mcd_baseinfo t2,T_MCD_TYPE t3 where t1.mcdcode = t2.mcdcode and t1.pakind = '0' 
                                and t2.mcdtype = t3.typecode and t2.mcdtype = '" + strTypeCode + "'";
                        ds_Num = dbOper.dataSet(strSql);
                        if (ds_Num.Tables.Count > 0)
                        {
                            intTypeCount = ds_Num.Tables[0].Rows.Count;
                        }

                        #endregion

                        GoodsTypeList.Add(new GoodsTypeModel()
                        {
                            TypeCode = strTypeCode,
                            TypeName = dataSet.Tables[0].Rows[i]["TypeName"].ToString(),
                            GoodsCount = intTypeCount,
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

        #endregion

        /// <summary>
        /// 远程下载商品图片文件
        /// </summary>
        /// <param name="advertInfo"></param>
        /// <returns></returns>
        private bool DownMcdPicFile(string mcdPic,string url)
        {
            string strFileLocalPath = AppDomain.CurrentDomain.BaseDirectory.ToString() + "Images\\GoodsPic\\" + mcdPic;

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
            ////string strFileName = advertInfo.FileName;
            ////string strFilePath = advertInfo.FileName + "." + advertInfo.FileType;

            ////if ((string.IsNullOrEmpty(strFileName)) ||
            ////    (string.IsNullOrEmpty(strFilePath)))
            ////{
            ////    return false;
            ////}

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
                System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(url);
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
    }
}
