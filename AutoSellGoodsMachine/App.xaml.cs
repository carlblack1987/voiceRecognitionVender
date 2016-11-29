using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace AutoSellGoodsMachine
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        System.Threading.Mutex mutex;

        public App()
       {
           this.Startup += new StartupEventHandler(App_Startup);
       }
 
       void App_Startup(object sender, StartupEventArgs e)
       {
           bool ret;
           try
           {
               mutex = new System.Threading.Mutex(true, "iVend", out ret);

               if (!ret)
               {
                   // 程序已经打开
                   Environment.Exit(0);
               }
           }
           catch
           {
               System.Windows.Application.Current.Shutdown();
               System.Reflection.Assembly.GetEntryAssembly();
               string startpath = System.IO.Directory.GetCurrentDirectory();
               System.Diagnostics.Process.Start(startpath + "/iVend.exe");  //xxxx.exe为要启动的程序
               ////Environment.Exit(0);
           }
       }
    }
}
