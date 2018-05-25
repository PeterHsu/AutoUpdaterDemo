//#範例版本 1.2018.0525.0
using AutoUpdaterDotNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoUpdaterDemo
{
    public static class AutoUpdaterHelper
    {
        //#鎖住執行緒
        private static CountdownEvent countdownEvent;
        //#在更新前有需要先釋放的資源, 因為未釋放的資源會鎖住程式無法更新關閉, 例如Socket
        private static IDisposable disposableObj = null;
        static AutoUpdaterHelper()
        {
            AutoUpdater.CheckForUpdateEvent += AutoUpdater_CheckForUpdateEvent;
        }

        /// <summary>
        /// 同步檢查更新，要用UI執行緒執行，如果有新版時，必須先決定是否更新程式才能繼續執行。
        /// </summary>
        /// <param name="updateUrl">檢查更新的網址，例如"http://localhost:5000/update/AutoUpdaterDemo/AutoUpdater.xml"</param>
        public static void StartUpdater(string updateUrl)
        {
            //#自動更新
            countdownEvent = new CountdownEvent(1);
            //#因為AutoUpdater是非同步的，所以使用CountdownEvent來鎖住目前的執行緒以避免程式繼續執行。
            AutoUpdater.Start(updateUrl);
            countdownEvent.Wait();
            countdownEvent.Dispose();
            countdownEvent = null;
        }

        /// <summary>
        /// 非同步檢查更新，要用UI執行緒執行。
        /// </summary>
        /// <param name="isQA"></param>
        /// <param name="obj">更新時有資源要事先釋放的物件</param>
        public static void StartUpdaterAsync(string updateUrl, string updateQAUrl = "", bool isQA = false, IDisposable obj = null)
        {
            disposableObj = obj;
            //#自動更新
            if (isQA) //#因為有可能要進行上線測試，可以到不同的網址進行檢查更新
            {
                AutoUpdater.Start(updateQAUrl);
            }
            else //#正式上線
            {
                AutoUpdater.Start(updateUrl);
            }
        }

        //#客制化更新
        private static void AutoUpdater_CheckForUpdateEvent(UpdateInfoEventArgs args)
        {
            if (args != null)
            {
                //#是否有新版本
                if (args.IsUpdateAvailable)
                {
                    DialogResult dialogResult;

                    dialogResult =
                        MessageBox.Show(
                             $"現在有新的版本 {args.CurrentVersion}。\n"
                            + $"你目前的版本為 {args.InstalledVersion}。\n"
                            + $"你要現在更新版本嗎?",
                            "有新的版本了",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Information);
                    //#要更新版本
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            //#如果有需要釋放的資源要先釋放
                            disposableObj?.Dispose();
                            //#下載新版
                            //#檔案不存在時會顯示錯誤對話盒, 但不用有Exception
                            if (AutoUpdater.DownloadUpdate())
                            {
                                Application.Exit();
                            }
                        }
                        catch (Exception exception) //#有寫有保佑
                        {
                            MessageBox.Show(exception.Message, exception.GetType().ToString(), MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else //#連不到
            {
                MessageBox.Show(
                        @"無法連線更新伺服器，繼續使用目前版本。",
                        @"檢查更新", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //#計數減１後為０則釋放執行緒
            countdownEvent?.Signal();
        }
    }
}
