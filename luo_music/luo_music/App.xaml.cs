﻿using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.UI.Popups;
using Luo.Shared.Extension;
using LuoMusic.Common;
using Luo.Shared.Helper;
using Windows.UI.Core.Preview;

namespace LuoMusic
{
    sealed partial class App
    {
        private UISettings ui;
        private Frame rootFrame;

        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
            this.UnhandledException += OnUnhandledException;
        }

        public static string GetAppVersion()
        {
            var packageVersion = Package.Current.Id.Version;
            var version = $"{packageVersion.Major}.{packageVersion.Minor}.{packageVersion.Build}";
            return version;
        }

        public static AppSettings AppSettings
        {
            get
            {
                return Current.Resources["AppSettings"] as AppSettings;
            }
        }

        private async void OnUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            await new MessageDialog("Application Unhandled Exception:\r\n" + GetExceptionDetailMessage(e.Exception), "爆了 :(")
                .ShowAsync();
        }

        private void RegisterExceptionHandlingSynchronizationContext()
        {
            ExceptionHandlingSynchronizationContext
                .Register()
                .UnhandledException += SynchronizationContext_UnhandledException; ;
        }

        private async void SynchronizationContext_UnhandledException(object sender, Luo.Shared.Extension.UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            await new MessageDialog("Synchronization Context Unhandled Exception:\r\n" + GetExceptionDetailMessage(e.Exception), "爆了 :(")
                .ShowAsync();
        }

        // https://github.com/ljw1004/async-exception-stacktrace
        private string GetExceptionDetailMessage(Exception ex)
        {
            return $"{ex.Message}\r\n{ex.StackTraceEx()}";
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;

                SystemNavigationManagerPreview.GetForCurrentView().CloseRequested += App_CloseRequested;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    CreateRootFrame(e.PreviousExecutionState);

                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();

                SystemNavigationManagerPreview.GetForCurrentView().CloseRequested += App_CloseRequested;
            }
            DispatcherHelper.Initialize();

            Messenger.Default.Register<NotificationMessageAction<string>>(
                this,
                HandleNotificationMessage);

            RegisterExceptionHandlingSynchronizationContext();

        }

        private void App_CloseRequested(object sender, SystemNavigationCloseRequestedPreviewEventArgs e)
        {
            Tile.ClearTileNotification();
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            SystemNavigationManagerPreview.GetForCurrentView().CloseRequested += App_CloseRequested;
            RegisterExceptionHandlingSynchronizationContext();

            if (Window.Current.Content == null)
            {
                CreateRootFrame(ApplicationExecutionState.NotRunning);

                rootFrame.Navigate(typeof(MainPage));

                Window.Current.Activate();
            }
            if (args.Kind == ActivationKind.Protocol)
            {
                var a = args as ProtocolActivatedEventArgs;
                var uri = a.Uri;

                if (uri != null)
                {
                    var volnum = uri.Query.Replace("?volnum=", "");
                    MainPage.Current?.NavigateByVolNum(volnum);
                }
            }
            if(args.Kind == ActivationKind.ToastNotification)
            {
                var a = args as ToastNotificationActivatedEventArgs;
                var uri = a.Argument;

                if (uri != null)
                {
                    var volnum = uri.Replace("?volnum=", "");
                    MainPage.Current?.NavigateByVolNum(volnum);
                }
            }
        }

        private void HandleNotificationMessage(NotificationMessageAction<string> message)
        {
            message.Execute("Success (from App.xaml.cs)!");
        }

        private void CreateRootFrame(ApplicationExecutionState previousExecutionState)
        {
            rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame
                {
                    // Set the default language
                    // Language = Windows.Globalization.ApplicationLanguages.Languages[0]
                };

                rootFrame.NavigationFailed += OnNavigationFailed;
                if (previousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            

            //if (ui != null) ui.ColorValuesChanged -= Ui_ColorValuesChanged;
            //ui = new UISettings();
            //ui.ColorValuesChanged += Ui_ColorValuesChanged;
            //titleBar.ButtonHoverBackgroundColor = ui.GetColorValue(UIColorType.Accent);

            // if you want not to have any window smaller than this size...
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Windows.Foundation.Size(320, 320));


            //var s = Settings.Current;
            //SQLOperator.Current();
            //ImageCache.Instance.CacheDuration = TimeSpan.MaxValue;
            //ImageCache.Instance.RetryCount = 1;
            //await ImageCache.Instance.InitializeAsync(ApplicationData.Current.LocalFolder, "Cache");
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
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
