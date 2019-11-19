using Newtonsoft.Json;
using ShareX.BridgeLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ShareX.App
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        protected override void OnShareTargetActivated(ShareTargetActivatedEventArgs args)
        {
            create(args);
            shareWithDesktopApplication(args.ShareOperation);
        }
        private async void shareWithDesktopApplication(ShareOperation shareOperation)
        {
            string filepath;
            var fileShareTargetInfo = new BridgeLib.ShareTargetInfo();
            if (shareOperation.Data.Contains(StandardDataFormats.Text))
            {
                shareOperation.ReportStarted();
                var text = await shareOperation.Data.GetTextAsync();
                fileShareTargetInfo.Text = text;
            }
            else if (shareOperation.Data.Contains(StandardDataFormats.StorageItems))
            {
                shareOperation.ReportStarted();
                var items = await shareOperation.Data.GetStorageItemsAsync();
                fileShareTargetInfo.Paths = items.OfType<StorageFile>().Select(a => a.Path).ToList(); 
            }
            else
            {
                return;
            }
            filepath = await WriteShareTargetInfoAsync(fileShareTargetInfo);

            //List<string> fileTypeFilter = new List<string>
            //{
            //    ".json"
            //};
            //var options = new QueryOptions(CommonFileQuery.OrderByName, fileTypeFilter);
            //var query = ApplicationData.Current.TemporaryFolder.CreateFileQueryWithOptions(options);
            //System.Threading.CancellationTokenSource cancellationTokenSource = new System.Threading.CancellationTokenSource();

            //query.ContentsChanged += changed;
            //await query.GetFilesAsync();

            await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync("ShareByJson");
            //void changed(IStorageQueryResultBase args, object b)
            //{
            //    if (args.Folder.TryGetItemAsync(filepath) == null)
            //    {
            //        cancellationTokenSource.Cancel();

            //        query.ContentsChanged -= changed;
            //    }
            //}
            //await Task.Delay(new TimeSpan(0, 15, 0), cancellationTokenSource.Token);
            while (true)
            {
                if (await ApplicationData.Current.TemporaryFolder.TryGetItemAsync(filepath) == null)
                {
                    break;
                }
                await Task.Delay(500);
            }
            shareOperation.ReportCompleted();
            Window.Current.Close();
        }

        private async Task<string> WriteShareTargetInfoAsync(BridgeLib.ShareTargetInfo fileShareTargetInfo)
        {
            var fn = DateTime.UtcNow.Ticks.ToString() + ".json";
            var file1 = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(fn, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(
                file1,
                JsonConvert.SerializeObject(fileShareTargetInfo)
                );
            return fn;
        }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }
        private void create(IActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(100, 100));
            ApplicationView.PreferredLaunchViewSize = new Size(100, 100);

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(typeof(MainPage));
            }

            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            create(e);
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            // Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
