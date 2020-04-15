using Hardcodet.Wpf.TaskbarNotification;
using ShadowPlayApi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Experienceless
{
    public partial class SysTray : INotifyPropertyChanged
    {
        private SPApiWrapper api;
        public ICommand TurnOffServer {
            get {
                return new DelegateCommand()
                {
                    CommandAction = () =>
                    {
                        api.EnableShadowPlay(false);
                        UpdateServerStatus();

                    },
                    CanExecuteFunc = () => serverIsRunning
                };
            }
        }
        public ICommand TurnOnServer {
            get {
                return new DelegateCommand()
                {
                    CommandAction = () =>
                    {
                        api.EnableShadowPlay(true);
                        UpdateServerStatus();
                    },
                    CanExecuteFunc = () => !serverIsRunning
                };
            }
        }

        public ICommand ToggleRecord {
            get {
                return new DelegateCommand()
                {
                    CommandAction = () =>
                    {
                        api.ToggleManualRecord();
                    },
                    CanExecuteFunc = () => serverIsRunning
                };
            }
        }
        public ICommand IRSave {
            get {
                return new DelegateCommand()
                {
                    CommandAction = () =>
                    {
                        api.ExecuteCaptureCommand(IRAction.SaveInstantReplay);
                    },
                    CanExecuteFunc = () => serverIsRunning && irIsRunning
                };
            }
        }
        public ICommand TurnOffIR {
            get {
                return new DelegateCommand()
                {
                    CommandAction = () =>
                    {
                        api.ExecuteCaptureCommand(IRAction.DisableInstantReplay);
                        UpdateServerStatus();

                    },
                    CanExecuteFunc = () => serverIsRunning && irIsRunning
                };
            }
        }
        public ICommand TurnOnIR {
            get {
                return new DelegateCommand()
                {
                    CommandAction = () =>
                    {
                        api.ExecuteCaptureCommand(IRAction.EnableInstantReplay);
                        UpdateServerStatus();

                    },
                    CanExecuteFunc = () => serverIsRunning && !irIsRunning
                };
            }
        }
        public ICommand TurnOnDWM {
            get {
                return new DelegateCommand()
                {
                    CommandAction = () =>
                    {
                        api.SetProperty(Params.DesktopCapture, 1);
                        UpdateServerStatus();

                    },
                    CanExecuteFunc = () => serverIsRunning && !dwnenabled
                };
            }
        }
        public ICommand TurnOffDWM {
            get {
                return new DelegateCommand()
                {
                    CommandAction = () =>
                    {
                        api.SetProperty(Params.DesktopCapture, 0);
                        UpdateServerStatus();

                    },
                    CanExecuteFunc = () => serverIsRunning && dwnenabled
                };
            }
        }
        public ICommand ShowSettings {
            get {
                return new DelegateCommand()
                {
                    CommandAction = () =>
                    {
                        SettingsWindow sw = Application.Current.Windows.OfType<SettingsWindow>().FirstOrDefault();
                        if (sw == null)
                        {
                            sw = new SettingsWindow(api);
                            sw.GetSettings();
                            sw.Show();
                        }
                        else
                        {
                            sw.WindowState = WindowState.Normal;
                            sw.Focus();
                        }
                    },
                    CanExecuteFunc = () => serverIsRunning
                };
            }
        }
        public ICommand ShowAbout {
            get {
                return new DelegateCommand()
                {
                    CommandAction = () =>
                    {
                        AboutWindow w = Application.Current.Windows.OfType<AboutWindow>().FirstOrDefault();
                        if (w == null)
                        {
                            w = new AboutWindow();
                            w.Show();
                        }
                        else
                        {
                            w.WindowState = WindowState.Normal;
                            w.Focus();
                        }
                    },
                    CanExecuteFunc = () => true
                };
            }
        }

        public ICommand Exit {
            get {
                return new DelegateCommand()
                {
                    CommandAction = () =>
                    {
                        Application.Current.Shutdown();
                    },
                    CanExecuteFunc = () => true
                };
            }
        }
        public ObservableCollection<SavedFile> RecentCaptures { get => _recentCaptures; set {
                _recentCaptures = value;
                OnPropertyChanged();
        } }

        public bool serverIsRunning;
        public bool irIsRunning;
        public bool dwnenabled;
        private ObservableCollection<SavedFile> _recentCaptures;
        public event PropertyChangedEventHandler PropertyChanged;
        public SysTray(SPApiWrapper api)
        {

            this.api = api;
            RecentCaptures = new ObservableCollection<SavedFile>() { };
            UpdateServerStatus();
            api.OnFileSaved += (sender, filepath) => RecentCaptures.Add(new SavedFile(filepath));
        }

        private void UpdateServerStatus()
        {
            serverIsRunning = api.GetStatus() == ShadowPlayStatus.On;
            irIsRunning = api.GetInstantReplayStatus() == 1;
            dwnenabled = Convert.ToBoolean(api.GetProperty(Params.DesktopCapture));
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }



    public class DelegateCommand : ICommand
    {
        public Action CommandAction { get; set; }
        public Func<bool> CanExecuteFunc { get; set; }

        public void Execute(object parameter)
        {
            CommandAction();
        }

        public bool CanExecute(object parameter)
        {
            return CanExecuteFunc == null || CanExecuteFunc();
        }

        public event EventHandler CanExecuteChanged {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }

    public class SavedFile
    {
        private string _filepath;
        private string _title;
        private string filepath { get => _filepath; set => _filepath = value; }
        public string Title { get => _title; set => _title = value; }
        public ICommand Browse {
            get {
                return new DelegateCommand()
                {
                    CommandAction = () =>
                    {
                        ProcessStartInfo info = new ProcessStartInfo();
                        info.FileName = "explorer.exe";
                        info.Arguments = "/select, \"" + filepath + "\"";
                        Process.Start(info);

                    },
                    CanExecuteFunc = () => true
                };
            }
        }
        

        public SavedFile(string filepath)
        {
            this.filepath = filepath;
            this.Title = Path.GetFileName(filepath);
        }
    }
}
