#region [ KIMMA Co.,Ltd. Copyright (C) 2014 ]
//-------------------------------------------------------------------------------------
// 开发单位：湖南金码智能设备制造有限公司
// 业务模块：iVend终端软件业务处理类
// 业务功能：货币库存处理类
// 创建标识：2015-07-23		谷霖
//-------------------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using KdbPlug;
using Business.Model;

namespace Business.Common
{
    public class CashInfoHelper
    {
        #region 变量声明

        private string _m_DbFileName = "TermInfo.db";

        /// <summary>
        /// 货币信息链式表
        /// </summary>
        public List<CashInfoModel> CashInfoList = new List<CashInfoModel>();

        #endregion

        #region 业务接口函数

        /// <summary>
        /// 加载货币信息表
        /// </summary>
        /// <returns>加载结果 True：成功 False：失败</returns>
        public bool LoadCashInfoList()
        {
            bool result = false;

            CashInfoList.Clear();

            DbOper dbOper = new DbOper();
            dbOper.DbFileName = _m_DbFileName;

            try
            {
                string strSql = string.Empty;

                #region 获取货币信息

                strSql = @"select CashValue,StockNum,BoxStockNum,CashType,Channel,Status from T_VM_CASHINFO";

                DataSet dataSet = dbOper.dataSet(strSql);
                if (dataSet.Tables.Count > 0)
                {
                    int recordCount = dataSet.Tables[0].Rows.Count;
                    for (int i = 0; i < recordCount; i++)
                    {
                        CashInfoList.Add(new CashInfoModel()
                        {
                            CashValue = Convert.ToInt32(dataSet.Tables[0].Rows[i]["CashValue"].ToString()),
                            StockNum = Convert.ToInt32(dataSet.Tables[0].Rows[i]["StockNum"].ToString()),
                            BoxStockNum = Convert.ToInt32(dataSet.Tables[0].Rows[i]["BoxStockNum"].ToString()),
                            CashType = dataSet.Tables[0].Rows[i]["CashType"].ToString(),
                            Channel = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Channel"].ToString()),
                            Status = dataSet.Tables[0].Rows[i]["Status"].ToString()
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
            }
            return result;
        }

        /// <summary>
        /// 根据货币面值、类型获取其库存数量
        /// </summary>
        /// <param name="cashValue">货币面值</param>
        /// <param name="cashType">货币类型</param>
        /// <returns>库存数量</returns>
        public int GetCashStockNum(int cashValue, string cashType, string boxType)
        {
            int intCashStockNum = 0;

            try
            {
                for (int i = 0; i < CashInfoList.Count; i++)
                {
                    if ((CashInfoList[i].CashValue == cashValue) && (CashInfoList[i].CashType == cashType))
                    {
                        if (boxType == "0")
                        {
                            // 硬币币筒/Hopper的库存，纸币钞箱库存
                            intCashStockNum = CashInfoList[i].StockNum;
                        }
                        else
                        {
                            // 硬币溢币盒的库存，纸币找零器的库存
                            intCashStockNum = CashInfoList[i].BoxStockNum;
                        }
                        break;
                    }
                }
            }
            catch
            {
                intCashStockNum = 0;
            }
            return intCashStockNum;
        }

        public long GetCashStockMoney(int cashValue, string cashType)
        {
            long intCashStockMoney = 0;

            try
            {
                for (int i = 0; i < CashInfoList.Count; i++)
                {
                    if ((CashInfoList[i].CashValue == cashValue) && (CashInfoList[i].CashType == cashType))
                    {
                        intCashStockMoney += CashInfoList[i].StockNum * CashInfoList[i].CashValue;
                    }
                }
            }
            catch
            {
                intCashStockMoney = 0;
            }
            return intCashStockMoney;
        }

        public long GetCashStockMoney_Type(string cashType,string boxType)
        {
            long intCashStockMoney = 0;

            try
            {
                for (int i = 0; i < CashInfoList.Count; i++)
                {
                    if (CashInfoList[i].CashType == cashType)
                    {
                        if (boxType == "0")
                        {
                            // 硬币币筒/Hopper的库存，纸币钞箱库存
                            intCashStockMoney += CashInfoList[i].StockNum * CashInfoList[i].CashValue;
                        }
                        else
                        {
                            // 硬币溢币盒的库存，纸币找零器的库存
                            intCashStockMoney += CashInfoList[i].BoxStockNum * CashInfoList[i].CashValue;
                        }
                    }
                }
            }
            catch
            {
                intCashStockMoney = 0;
            }
            return intCashStockMoney;
        }

        /// <summary>
        /// 增加或减少或覆盖某货币面值库存
        /// </summary>
        /// <param name="cashValue"></param>
        /// <param name="stockNum"></param>
        /// <param name="cashType"></param>
        /// <param name="source">货币进入途径 0：不属于进入 
        /// 1：进入正轨途径，硬币进入币筒，纸币进入钞箱 2：进入非正轨途径，硬币进入溢币盒，纸币进入找零器</param>
        /// <param name="type">更改类型 0：增加 1：减少 2：覆盖</param>
        /// <returns></returns>
        public bool UpdateCashStockNum(int cashValue, int stockNum, string cashType,string source,string type)
        {
            bool result = false;
            string strSql = string.Empty;
            string strStockComm = "StockNum";

            try
            {
                switch (source)
                {
                    case "1"://
                        strStockComm = "StockNum";// 修改的是其正轨途径进入的库存数量
                        break;
                    case "2"://
                        strStockComm = "BoxStockNum";// 修改的是其非正轨途径进入的库存数量
                        break;
                    case "0"://
                        if (cashType == "0")
                        {
                            // 如果是硬币
                            strStockComm = "StockNum";
                        }
                        else
                        {
                            // 如果是纸币
                            strStockComm = "BoxStockNum";
                        }
                        break;
                }

                int intListCount = CashInfoList.Count;
                for (int i = 0; i < intListCount; i++)
                {
                    if ((CashInfoList[i].CashValue == cashValue) && (CashInfoList[i].CashType == cashType))
                    {
                        switch (type)
                        {
                            case "0": // 增加
                                strSql = "update T_VM_CASHINFO set " + strStockComm + " = " + strStockComm + " + " + stockNum +
                                " where CashValue = '" + cashValue + "' and CashType = '" + cashType + "'";
                                break;
                            case "1":// 减少
                                strSql = "update T_VM_CASHINFO set " + strStockComm + " = " + strStockComm + " - " + stockNum +
                                " where CashValue = '" + cashValue + "' and CashType = '" + cashType + "'";
                                break;
                            case "2":// 覆盖
                                strSql = "update T_VM_CASHINFO set " + strStockComm + " = " + stockNum +
                               " where CashValue = '" + cashValue + "' and CashType = '" + cashType + "'";
                                break;
                        }
                        DbOper dbOper = new DbOper();
                        dbOper.DbFileName = _m_DbFileName;
                        dbOper.ConnType = ConnectType.CloseConn;
                        result = dbOper.excuteSql(strSql);
                        dbOper.closeConnection();

                        if (result)
                        {

                            switch (type)
                            {
                                case "0":
                                    if (strStockComm == "StockNum")
                                    {
                                        CashInfoList[i].StockNum = CashInfoList[i].StockNum + stockNum;
                                    }
                                    else if (strStockComm == "BoxStockNum")
                                    {
                                        CashInfoList[i].BoxStockNum = CashInfoList[i].BoxStockNum + stockNum;
                                    }
                                    
                                    break;
                                case "1":
                                    if (strStockComm == "StockNum")
                                    {
                                        CashInfoList[i].StockNum = CashInfoList[i].StockNum - stockNum;
                                    }
                                    else if (strStockComm == "BoxStockNum")
                                    {
                                        CashInfoList[i].BoxStockNum = CashInfoList[i].BoxStockNum - stockNum;
                                    }
                                    break;
                                case "2":
                                    if (strStockComm == "StockNum")
                                    {
                                        CashInfoList[i].StockNum = stockNum;
                                    }
                                    else if (strStockComm == "BoxStockNum")
                                    {
                                        CashInfoList[i].BoxStockNum = stockNum;
                                    }
                                    break;
                            }
                        }
                        break;
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
        /// 更新某货币面值库存
        /// </summary>
        /// <param name="cashValue"></param>
        /// <param name="stockNum"></param>
        /// <param name="cashType"></param>
        /// <returns></returns>
        ////public bool UpdateCashStockNum_All(int cashValue, int stockNum, string cashType)
        ////{
        ////    bool result = false;
        ////    string strSql = string.Empty;

        ////    try
        ////    {
        ////        int intListCount = CashInfoList.Count;
        ////        for (int i = 0; i < intListCount; i++)
        ////        {
        ////            if ((CashInfoList[i].CashValue == cashValue) && (CashInfoList[i].CashType == cashType))
        ////            {
        ////                strSql = "update T_VM_CASHINFO set StockNum = " + stockNum +
        ////                        " where CashValue = '" + cashValue + "' and CashType = '" + cashType + "'";
        ////                DbOper dbOper = new DbOper();
        ////                dbOper.DbFileName = _m_DbFileName;
        ////                dbOper.ConnType = ConnectType.CloseConn;
        ////                result = dbOper.excuteSql(strSql);
        ////                dbOper.closeConnection();

        ////                if (result)
        ////                {
        ////                    CashInfoList[i].StockNum = stockNum;
        ////                }
        ////                break;
        ////            }
        ////        }
        ////    }
        ////    catch
        ////    {
        ////        result = false;
        ////    }
        ////    return result;
        ////}

        /// <summary>
        /// 清除某货币面值库存
        /// </summary>
        /// <param name="cashValue"></param>
        /// <param name="cashType"></param>
        /// <returns></returns>
        public bool ClearCashStockNum(int cashValue, string cashType)
        {
            bool result = false;
            string strSql = string.Empty;

            try
            {
                int intListCount = CashInfoList.Count;
                for (int i = 0; i < intListCount; i++)
                {
                    if ((CashInfoList[i].CashValue == cashValue) && (CashInfoList[i].CashType == cashType))
                    {
                        strSql = "update T_VM_CASHINFO set StockNum = 0 where CashValue = '" + cashValue + "' and CashType = '" + cashType + "'";
                        DbOper dbOper = new DbOper();
                        dbOper.DbFileName = _m_DbFileName;
                        dbOper.ConnType = ConnectType.CloseConn;
                        result = dbOper.excuteSql(strSql);
                        dbOper.closeConnection();

                        if (result)
                        {
                            CashInfoList[i].StockNum = 0;
                        }
                        break;
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
        /// 清除指定类型货币库存
        /// </summary>
        /// <param name="cashType">货币类型 0：硬币 1：纸币</param>
        /// <param name="operType">清除类型 0：全部库存 1：正轨途径库存（如：币筒、Hopper、钞箱） 2：非正轨途径库存（如：溢币盒、纸币找零器）</param>
        /// <returns></returns>
        public bool ClearCashStockNum_All(string cashType,string operType)
        {
            bool result = false;
            string strSql = string.Empty;

            try
            {
                switch (operType)
                {
                    case "0":// 全部库存
                        strSql = "update T_VM_CASHINFO set StockNum = 0,BoxStockNum = 0 where CashType = '" + cashType + "'";
                        break;
                    case "1":// 正轨途径库存
                        strSql = "update T_VM_CASHINFO set StockNum = 0 where CashType = '" + cashType + "'";
                        break;
                    case "2":// 非正轨途径库存
                        strSql = "update T_VM_CASHINFO set BoxStockNum = 0 where CashType = '" + cashType + "'";
                        break;
                }

                
                DbOper dbOper = new DbOper();
                dbOper.DbFileName = _m_DbFileName;
                dbOper.ConnType = ConnectType.CloseConn;
                result = dbOper.excuteSql(strSql);
                dbOper.closeConnection();

                if (result)
                {
                    int intListCount = CashInfoList.Count;
                    for (int i = 0; i < intListCount; i++)
                    {
                        if (CashInfoList[i].CashType == cashType)
                        {
                            switch (operType)
                            {
                                case "0":// 全部库存
                                    CashInfoList[i].StockNum = 0;
                                    CashInfoList[i].BoxStockNum = 0;
                                    break;
                                case "1":// 正轨途径库存
                                    CashInfoList[i].StockNum = 0;
                                    break;
                                case "2":// 非正轨途径库存
                                    CashInfoList[i].BoxStockNum = 0;
                                    break;
                            }
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
        /// 更新某货币面值接收使能/禁能状态
        /// </summary>
        /// <param name="cashValue"></param>
        /// <param name="stockNum"></param>
        /// <param name="cashType"></param>
        /// <returns></returns>
        public bool UpdateCashStatus_All(int cashValue, string cashType,string status)
        {
            bool result = false;
            string strSql = string.Empty;

            try
            {
                int intListCount = CashInfoList.Count;
                for (int i = 0; i < intListCount; i++)
                {
                    if ((CashInfoList[i].CashValue == cashValue) && (CashInfoList[i].CashType == cashType))
                    {
                        strSql = "update T_VM_CASHINFO set Status = '" + status +
                                "' where CashValue = '" + cashValue + "' and CashType = '" + cashType + "'";
                        DbOper dbOper = new DbOper();
                        dbOper.DbFileName = _m_DbFileName;
                        dbOper.ConnType = ConnectType.CloseConn;
                        result = dbOper.excuteSql(strSql);
                        dbOper.closeConnection();

                        if (result)
                        {
                            CashInfoList[i].Status = status;
                        }
                        break;
                    }
                }
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
