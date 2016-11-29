using System;
using System.Security.Permissions;
using System.Windows.Threading;
using System.Threading;

namespace AutoSellGoodsMachine
{
    public class DispatcherHelper
    {
        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(ExitFrames), frame);
            try { Dispatcher.PushFrame(frame); }
            catch (InvalidOperationException) { }
        }
        private static object ExitFrames(object frame)
        {
            ((DispatcherFrame)frame).Continue = false;
            return null;
        }

        public static void SleepControl()
        {
            Thread.Sleep(20);
            DoEvents();
        }

        /// <summary>
        /// 延时控件显示
        /// </summary>
        /// <param name="sleepTime">要延时的时间，以毫秒为单位</param>
        public static void SleepControl(int sleepTime)
        {
            int intSleepNum = 0;
            while (true)
            {
                intSleepNum++;
                if (intSleepNum > sleepTime / 20)
                {
                    break;
                }
                Thread.Sleep(20);
                DoEvents();
            }
        }
    }
}
