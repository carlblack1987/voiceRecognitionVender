#region [ KIMMA Co.,Ltd. Copyright (C) 2014 ]
//-------------------------------------------------------------------------------------
// 开发单位：湖南金码智能设备制造有限公司
// 业务模块：iVend终端软件业务处理类
// 业务功能：设备控制策略处理类
// 创建标识：2014-06-10		谷霖
//-------------------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Business.Model;
using Business.Enum;

namespace Business
{
    public class DeviceControlRole
    {
        /// <summary>
        /// 检测控制设备是否需要改变当前状态，主要针对照明灯、除雾设备、紫外灯、广告灯设备
        /// </summary>
        /// <param name="deviceModel"></param>
        /// <returns></returns>
        public static BusinessEnum.DeviceControlStatus CheckDeviceControlRole(DeviceControlModel deviceModel)
        {
            BusinessEnum.DeviceControlStatus deviceControlStatus = BusinessEnum.DeviceControlStatus.Keep;

            try
            {
                switch (deviceModel.ControlModel)
                {
                    case "2":// 全时段关闭
                        if (deviceModel.ControlStatus == BusinessEnum.DeviceControlStatus.Open)
                        {
                            // 如果此时设备控制状态为打开，则需要关闭
                            deviceControlStatus = BusinessEnum.DeviceControlStatus.Close;
                        }
                        break;

                    case "1":// 全时段开启
                        if (deviceModel.ControlStatus == BusinessEnum.DeviceControlStatus.Close)
                        {
                            // 如果此时设备控制状态为关闭，则需要打开
                            deviceControlStatus = BusinessEnum.DeviceControlStatus.Open;
                        }
                        break;

                    case "0":// 定时开启
                        // 如果在规定时间内，当前状态为已开启，则保持不变；当前状态为已关闭，则需要打开
                        // 如果超过规定时间，当前状态为已开启，则需要关闭；当前状态为已关闭，则保持不变
                        string strNowTime = DateTime.Now.ToShortTimeString();
                        int intNowTime = FunHelper.ConvertHourMinTime(DateTime.Now.ToShortTimeString().Replace(":", ""));

                        int intBeginTime1 = FunHelper.ConvertHourMinTime(deviceModel.BeginTime1);
                        int intEndTime1 = FunHelper.ConvertHourMinTime(deviceModel.EndTime1);
                        int intBeginTime2 = FunHelper.ConvertHourMinTime(deviceModel.BeginTime2);
                        int intEndTime2 = FunHelper.ConvertHourMinTime(deviceModel.EndTime2);

                        bool blnIsInTime = false;// 当前时间是否在定时时间段内 False：不在 True：在
                        if ((intNowTime > intBeginTime1) && (intNowTime < intEndTime1))
                        {
                            // 在第一个时间段内
                            blnIsInTime = true;
                        }
                        if (!blnIsInTime)
                        {
                            if ((intNowTime > intBeginTime2) && (intNowTime < intEndTime2))
                            {
                                // 在第二个时间段内
                                blnIsInTime = true;
                            }
                        }

                        if (blnIsInTime)
                        {
                            // 在定时时间段内
                            if (deviceModel.ControlStatus == BusinessEnum.DeviceControlStatus.Close)
                            {
                                // 如果此时设备控制状态为关闭，则需要打开
                                deviceControlStatus = BusinessEnum.DeviceControlStatus.Open;
                            }
                        }
                        else
                        {
                            // 不在定时时间段内
                            if (deviceModel.ControlStatus == BusinessEnum.DeviceControlStatus.Open)
                            {
                                // 如果此时设备控制状态为打开，则需要关闭
                                deviceControlStatus = BusinessEnum.DeviceControlStatus.Close;
                            }
                        }

                        break;

                    default:
                        break;
                }
            }
            catch
            {
                deviceControlStatus = BusinessEnum.DeviceControlStatus.Keep;
            }

            return deviceControlStatus;
        }

        /// <summary>
        /// 检测制冷压缩机设备是否需要改变当前状态，只针对制冷压缩机
        /// </summary>
        /// <param name="deviceModel">压缩机控制策略</param>
        /// <param name="doorStatus">当前门状态</param>
        /// <param name="nowTmpValue">当前温度</param>
        /// <returns></returns>
        public static BusinessEnum.DeviceControlStatus CheckRefControlRole(DeviceControlModel deviceModel,
            BusinessEnum.TmpControlModel tmpControlModel,
            string doorStatus,string nowTmpStatus,string nowTmpValue,string targetTmp)
        {
            /* 制冷压缩机控制策略
             * 1、开机后，如果压缩机需要打开，则需要等待7分钟
             * 2、压缩机连续工作50分钟后，必须要关闭
             * 3、压缩机下次打开时，必须距离上次关闭的时间超过7分钟
             * 4、当前温度达到最低温度，则关闭压缩机
             * 5、门开后，关闭压缩机
            */

            BusinessEnum.DeviceControlStatus deviceControlStatus = BusinessEnum.DeviceControlStatus.Keep;

            try
            {
                deviceControlStatus = CheckDeviceControlRole(deviceModel);

                if (deviceControlStatus == BusinessEnum.DeviceControlStatus.Open)
                {
                    // 如果此时从时间上检测需要打开压缩机
                    bool blnIsOpen = true;// 是否能打开压缩机 False：不能 True：打开

                    #region 其它条件检测压缩机是否能打开

                    doorStatus = "00";
                    if (doorStatus == "01")
                    {
                        // 此时门开，则不能打开压缩机
                        blnIsOpen = false;
                    }
                    else
                    {
                        blnIsOpen = true;

                        if (tmpControlModel == BusinessEnum.TmpControlModel.Refrigeration)
                        {
                            // 制冷情况：如果当前温度超过了最低温度，则不能打开压缩机
                            if ((nowTmpStatus == "00") && (Convert.ToInt32(nowTmpValue) <= Convert.ToInt32(targetTmp)))
                            {
                                blnIsOpen = false;
                            }
                        }
                        if (tmpControlModel == BusinessEnum.TmpControlModel.Heating)
                        {
                            // 加热情况：如果当前温度超过了最高温度，则不能打开加热器
                            if ((nowTmpStatus == "00") && (Convert.ToInt32(nowTmpValue) >= Convert.ToInt32(targetTmp)))
                            {
                                blnIsOpen = false;
                            }
                        }

                        if (blnIsOpen)
                        {
                            // 如果距离上次关闭时间不足7分钟，则不能打开压缩机
                            DateTime dtmNowTime = DateTime.Now;

                            TimeSpan ts1 = new TimeSpan(deviceModel.LastCloseTime.Ticks);
                            TimeSpan ts2 = new TimeSpan(dtmNowTime.Ticks);
                            TimeSpan ts = ts1.Subtract(ts2).Duration();
                            if (ts.Minutes < deviceModel.CloseDelayTime)
                            {
                                // 距离上次压缩机关闭时间不足7分钟                        
                                blnIsOpen = false;
                            }
                        }
                    }

                    #endregion

                    if (!blnIsOpen)
                    {
                        // 压缩机不能打开，需要关闭
                        switch (deviceModel.ControlStatus)
                        {
                            case BusinessEnum.DeviceControlStatus.Close:// 如果当前压缩状态已经为关闭，则状态保持不变
                                deviceControlStatus = BusinessEnum.DeviceControlStatus.Keep;
                                break;

                            case BusinessEnum.DeviceControlStatus.Open:// 如果当前压缩机状态已经为打开，则状态切换为关闭
                                deviceControlStatus = BusinessEnum.DeviceControlStatus.Close;
                                break;
                        }
                    }
                }

                if ((deviceModel.ControlStatus == BusinessEnum.DeviceControlStatus.Open) &&
                    (deviceControlStatus == BusinessEnum.DeviceControlStatus.Keep))
                {
                    // 压缩机当前状态如果为已经打开且当前需要切换的状态为保持打开状态，则检测打开的连续时间是否超过了规定的最大时间，如果超过，则关闭；如果没有超过，则保持
                    if ((nowTmpStatus == "00") && 
                        (tmpControlModel == BusinessEnum.TmpControlModel.Refrigeration) && 
                        (Convert.ToInt32(nowTmpValue) <= Convert.ToInt32(targetTmp)))
                    {
                        deviceControlStatus = BusinessEnum.DeviceControlStatus.Close;
                    }
                    else if ((nowTmpStatus == "00") &&
                        (tmpControlModel == BusinessEnum.TmpControlModel.Heating) &&
                        (Convert.ToInt32(nowTmpValue) >= Convert.ToInt32(targetTmp)))
                    {
                        deviceControlStatus = BusinessEnum.DeviceControlStatus.Close;
                    }
                    else
                    {
                        DateTime dtmNowTime = DateTime.Now;

                        TimeSpan ts1 = new TimeSpan(deviceModel.LastOpenTime.Ticks);
                        TimeSpan ts2 = new TimeSpan(dtmNowTime.Ticks);
                        TimeSpan ts = ts1.Subtract(ts2).Duration();
                        if (ts.Minutes > deviceModel.OpenMaxTime)
                        {
                            // 距离上次压缩机打开时间超过规定的最长时间                     
                            deviceControlStatus = BusinessEnum.DeviceControlStatus.Close;
                        }
                    }
                }
            }
            catch
            {
                deviceControlStatus = BusinessEnum.DeviceControlStatus.Keep;
            }

            return deviceControlStatus;
        }
    }
}
