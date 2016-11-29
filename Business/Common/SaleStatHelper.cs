#region [ KIMMA Co.,Ltd. Copyright (C) 2014 ]
//-------------------------------------------------------------------------------------
// 开发单位：湖南金码智能设备制造有限公司
// 业务模块：iVend终端软件业务处理类
// 业务功能：销售数据处理类
// 创建标识：2014-06-10		谷霖
//-------------------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using KdbPlug;

namespace Business
{
    public class SaleStatHelper
    {
        private string _m_DbFileName = "TermInfo.db";

        /// <summary>
        /// 获取机器销售统计数据
        /// </summary>
        /// <returns>销售统计数据</returns>
        public DataSet GetSaleStatInfo()
        {
            DbOper dbOper = new DbOper();
            dbOper.DbFileName = _m_DbFileName;
            DataSet dataSet = new DataSet();

            try
            {
                string strSql = @"select BeginDate,TotalSaleMoney,TotalSaleNum,
                    CycleDate,CycleSaleMoney,CycleSaleNum,TodaySaleMoney,TodaySaleNum 
                    from T_SALE_STAT";
                dataSet = dbOper.dataSet(strSql);
            }
            catch
            {
            }
            finally
            {
                dbOper.closeConnection();
            }
            return dataSet;
        }

        /// <summary>
        /// 获取机器当日货币统计数据
        /// </summary>
        /// <returns>当日货币统计数据</returns>
        public DataSet GetMoneyStatInfo()
        {
            DbOper dbOper = new DbOper();
            dbOper.DbFileName = _m_DbFileName;
            DataSet dataSet = new DataSet();

            try
            {
                string strSql = @"select TodayCashMoney,TodayCashNum,TodayCoinRecMoney,TodayCoinRecNum,
                        TodayCoinChangeMoney,TodayCoinChangeNum 
                    from T_MONEY_STAT";
                dataSet = dbOper.dataSet(strSql);
            }
            catch
            {
            }
            finally
            {
                dbOper.closeConnection();
            }
            return dataSet;
        }

        /// <summary>
        /// 清除段销售数据
        /// </summary>
        /// <returns></returns>
        public bool ClearCycleSaleData()
        {
            bool result = false;

            // 清除销售数据
            string strTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string strDate = DateTime.Now.ToString("yyyyMMdd");
            string strSql = @"update T_SALE_STAT set [CycleSaleMoney] = 0,[CycleSaleNum] = 0,
                [CycleDate] = '" + strTime + "',[TodaySaleMoney] = 0,[TodaySaleNum] = 0,[TodayDate] = '" + strDate +
                @"'";

            DbOper dbOper = new DbOper();
            dbOper.DbFileName = _m_DbFileName;
            dbOper.ConnType = ConnectType.KeepConn;
            result = dbOper.excuteSql(strSql);

            // 清除金额数据
            strSql = @"update T_MONEY_STAT set [CycleDate] = '" + strTime +
                @"',[CycleCashMoney] = 0,[CycleCashNum] = 0,[CycleCoinRecMoney] = 0,[CycleCoinRecNum] = 0,
                [CycleCoinChangeMoney] = 0,[CycleCoinChangeNum] = 0,[TodayDate] = '" + strDate +
                @"',[TodayCashMoney] = 0,[TodayCashNum] = 0,[TodayCoinRecMoney] = 0,[TodayCoinRecNum] = 0,
                [TodayCoinChangeMoney] = 0,[TodayCoinChangeNum] = 0";
            result = dbOper.excuteSql(strSql);

            dbOper.closeConnection();

            return result;
        }

        /// <summary>
        /// 清除机器数据
        /// </summary>
        /// <returns>结果 True：成功 False：失败</returns>
        public bool ClearAllData()
        {
            bool result = false;

            // 清除机器销售统计数据
            string strSql = "update T_SALE_STAT set [BeginDate] = '',[TotalSaleMoney] = 0 ,[TotalSaleNum] = 0," +
                "[CycleDate] = '',[CycleSaleMoney] = 0,[CycleSaleNum] = 0," +
                "[TodayDate] = '',[TodaySaleMoney] = 0,[TodaySaleNum] = 0";

            DbOper dbOper = new DbOper();
            dbOper.DbFileName = _m_DbFileName;
            dbOper.ConnType = ConnectType.KeepConn;
            result = dbOper.excuteSql(strSql);

            // 清除货币数据
            strSql = "update T_MONEY_STAT set [BeginDate] = '',[TotalCashMoney] = 0,[TotalCashNum] = 0,[TotalCoinRecMoney] = 0,[TotalCoinRecNum] = 0," +
                "[TotalCoinChangeMoney] = 0,[TotalCoinChangeNum] = 0,[CycleDate] = '',[CycleCashMoney] = 0,[CycleCashNum] = 0,[CycleCoinRecMoney] = 0,[CycleCoinRecNum] = 0," +
                "[CycleCoinChangeMoney] = 0,[CycleCoinChangeNum] = 0,[TodayDate] = '',[TodayCashMoney] = 0,[TodayCashNum] = 0,[TodayCoinRecMoney] = 0,[TodayCoinRecNum] = 0," +
                "[TodayCoinChangeMoney] = 0,[TodayCoinChangeNum] = 0";
            result = dbOper.excuteSql(strSql);

            dbOper.closeConnection();

            // 清除待发的数据

            return result;
        }
    }
}
