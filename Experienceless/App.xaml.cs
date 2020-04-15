using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Collections.Generic;
using Keys = System.Windows.Forms.Keys;
using System.Windows;
using Experienceless.Model;
using System.Windows.Controls;

namespace Experienceless
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public SPApiWrapper api;
        private TaskbarIcon tb;
        System.Media.SoundPlayer player = new System.Media.SoundPlayer(Experienceless.Properties.Resources.alert);
        public new int Run() {
            tb = (TaskbarIcon)FindResource("NotifyIcon");
            api = new SPApiWrapper();
            api.OnCommandExecuted += (sender, command) =>
            {
                if (command == ShadowPlayApi.IRAction.ManualRecordStop || command == ShadowPlayApi.IRAction.SaveInstantReplay)
                {
                    player.Play();
                }
            };
            Application.Current.Properties["IsHotkeysEnabled"] = true;
            return base.Run();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            tb.DataContext = new SysTray(api);
            UpdateHotkeys();
        }

        public void UpdateHotkeys() {
            HotKey irHotKey = HotKey.FromVirtual(new Keys[] {
                (Keys)Convert.ToInt32(api.GetProperty("DVRHKey0")),
                (Keys)Convert.ToInt32(api.GetProperty("DVRHKey1")),
                (Keys)Convert.ToInt32(api.GetProperty("DVRHKey2")),
                (Keys)Convert.ToInt32(api.GetProperty("DVRHKey3"))
            });

            HotKey mrHotKey = HotKey.FromVirtual(new Keys[] {
                (Keys)Convert.ToInt32(api.GetProperty("ManualHKey0")),
                (Keys)Convert.ToInt32(api.GetProperty("ManualHKey1")),
                (Keys)Convert.ToInt32(api.GetProperty("ManualHKey2")),
                (Keys)Convert.ToInt32(api.GetProperty("ManualHKey3"))
            });

            foreach (FrameworkElement mi in tb.ContextMenu.Items.SourceCollection)
            {
                
                if (mi.Name == "irHotKey") ((MenuItem)mi).InputGestureText = irHotKey.ToString();
                if (mi.Name == "mrHotKey") ((MenuItem)mi).InputGestureText = mrHotKey.ToString();
            }

        }

    }
}
