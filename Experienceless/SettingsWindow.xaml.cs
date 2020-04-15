using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Experienceless.Model;
using Keys = System.Windows.Forms.Keys;
namespace Experienceless
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class SettingsWindow : INotifyPropertyChanged
    {


        private SPApiWrapper api;
        private EncoderProfile _encoderprofile;
        private string _resolution;
        private int _bitrate;
        private string _temppath;
        private string _defaultpath;
        private int _reclenght;
        private string _fps;
        private HotKey _irHotKey;
        private HotKey _mrHotKey;
        public List<EncoderProfile> lstEncoderProfiles { get; set; }
        public List<string> lstResolutions { get; set; }
        public List<string> lstFPS { get; set; }

        public EncoderProfile encoderprofile {
            get => _encoderprofile; set {
                if (!Equals(_encoderprofile, value))
                {
                    _encoderprofile = value;
                    OnPropertyChanged();
                    if (!value.value.Equals("Custom"))
                    {
                        bitrate = value.bitrate;
                        fps = "60";
                        resolution = "In-game";
                    }
                }
            }
        }
        public int bitrate { get => _bitrate; set { if (_bitrate != value) { _bitrate = value; OnPropertyChanged(); check_profile(); } } }
        public string resolution { get => _resolution; set { if (_resolution != value) { _resolution = value; OnPropertyChanged(); check_profile(); } } }
        public string temppath { get => _temppath; set { if (_temppath != value) { _temppath = value; OnPropertyChanged(); } } }
        public string defaultpath { get => _defaultpath; set { if (_defaultpath != value) { _defaultpath = value; OnPropertyChanged(); } } }
        public int reclenght { get => _reclenght; set { if (_reclenght != value) { _reclenght = value; OnPropertyChanged(); } } }
        public string fps { get => _fps; set { if (_fps != value) {_fps = value; OnPropertyChanged(); check_profile(); } } }


        public HotKey irHotKey { get => _irHotKey; set { if (_irHotKey != value) { _irHotKey = value; OnPropertyChanged(); } } }
        public HotKey mrHotKey { get => _mrHotKey; set { if (_mrHotKey != value) { _mrHotKey = value; OnPropertyChanged(); } } }
        public SettingsWindow(SPApiWrapper api)
        {
            Application.Current.Properties["IsHotkeysEnabled"] = false;
            this.api = api;
            lstEncoderProfiles = new List<EncoderProfile>() {
                    new EncoderProfile("Low", "Average", 15),
                    new EncoderProfile("Medium", "Good", 22),
                    new EncoderProfile("High", "VeryGood", 50),
                    new EncoderProfile("Custom", "Custom", 50)
            };
            lstResolutions = new List<string> {
                    "In-game",
                    "2160p 4K",
                    "1440p HD",
                    "1080p HD",
                    "720p HD",
                    "480p",
                    "360p",
                    "240p"
            };

            lstFPS = new List<string>() { "30", "60" };
            InitializeComponent();
            DataContext = this;
        }


        public void GetSettings() {
            temppath = api.GetProperty(ShadowPlayApi.Params.TempFilePath)?.ToString();
            defaultpath = api.GetProperty(ShadowPlayApi.Params.DefaultPath)?.ToString();
            object current_quality = api.GetProperty(ShadowPlayApi.Params.EncoderProfile);
            
            resolution = api.GetProperty(ShadowPlayApi.Params.Resolution)?.ToString();

            reclenght = Convert.ToInt32(api.GetProperty(ShadowPlayApi.Params.DVRBufferLen)) / 60;
            object current_fps = api.GetProperty(ShadowPlayApi.Params.RecordingFPS);
            fps = current_fps != null ? Convert.ToUInt32(current_fps).ToString() : "";
            bitrate = Convert.ToInt32(api.GetProperty(ShadowPlayApi.Params.BitrateBps)) / 1000000;
            encoderprofile = lstEncoderProfiles.Find(item => item.value.Equals(current_quality));
            irHotKey = HotKey.FromVirtual(new Keys[] {
                (Keys)Convert.ToInt32(api.GetProperty("DVRHKey0")),
                (Keys)Convert.ToInt32(api.GetProperty("DVRHKey1")),
                (Keys)Convert.ToInt32(api.GetProperty("DVRHKey2")),
                (Keys)Convert.ToInt32(api.GetProperty("DVRHKey3"))
            });

            mrHotKey = HotKey.FromVirtual(new Keys[] {
                (Keys)Convert.ToInt32(api.GetProperty("ManualHKey0")),
                (Keys)Convert.ToInt32(api.GetProperty("ManualHKey1")),
                (Keys)Convert.ToInt32(api.GetProperty("ManualHKey2")),
                (Keys)Convert.ToInt32(api.GetProperty("ManualHKey3"))
            });
        }

        private void BtnTempBrowse_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new FolderSelect.FolderSelectDialog();
            dlg.InitialDirectory = temppath;
            if (dlg.ShowDialog())
            {
                temppath = dlg.FileName;
            }
        }

        private void BtnDefaultPathBrowse_Click(object sender, RoutedEventArgs e)

        {
            var dlg = new FolderSelect.FolderSelectDialog();
            dlg.InitialDirectory = defaultpath;
            if (dlg.ShowDialog())
            {
                defaultpath = dlg.FileName;
            }
        }

        private void check_profile()
        {
            if (!Equals(encoderprofile.value, "Custom"))
            {
                if (encoderprofile.bitrate != bitrate
                    || resolution != "In-game"
                    || fps != "60"
                    ) {
                    encoderprofile = lstEncoderProfiles.Find(ep => ep.value.Equals("Custom"));
                }

            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            List<string> errors = new List<string> { };

            if (api.SetProperty(ShadowPlayApi.Params.TempFilePath, temppath) != 0)  errors.Add("Temporary files");
            if (api.SetProperty(ShadowPlayApi.Params.DefaultPath, defaultpath) != 0) errors.Add("Videos");
            if (api.SetProperty(ShadowPlayApi.Params.EncoderProfile, encoderprofile.value) != 0) errors.Add("Preset");
            if (api.SetProperty(ShadowPlayApi.Params.Resolution, resolution) != 0) errors.Add("Resolution");
            if (api.SetProperty(ShadowPlayApi.Params.DVRBufferLen, reclenght * 60) != 0) errors.Add("Instant Replay length");
            if (api.SetProperty(ShadowPlayApi.Params.RecordingFPS, Convert.ToDouble(fps)) != 0) errors.Add("FPS");
            if (api.SetProperty(ShadowPlayApi.Params.BitrateBps, Convert.ToUInt32(bitrate * 1000000)) != 0) errors.Add("Bit rate");

            Keys[] ir_vk_keys = irHotKey.ToVirtual(); 
            for (int i = 0; i < ir_vk_keys.Length; i++)
            {
                if (api.SetProperty("DVRHKey" + i, Convert.ToInt32(ir_vk_keys[i])) != 0) errors.Add("Instant Replay Key = " + ir_vk_keys[i]);
            }
            Keys[] mr_vk_keys = mrHotKey.ToVirtual();
            for (int i = 0; i < mr_vk_keys.Length; i++)
            {
                if (api.SetProperty("ManualHKey" + i, Convert.ToInt32(mr_vk_keys[i])) != 0) errors.Add("Manual Record Key = " + mr_vk_keys[i]);
            }

            if (errors.Count > 0)
            {
                MessageBox.Show(string.Join("\n", errors), "An error occurred while saving the following settings");
            } else
            {
                Close();
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Application.Current.Properties["IsHotkeysEnabled"] = true;
            ((App)Application.Current).UpdateHotkeys();
        }
    }
}
