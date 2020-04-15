using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Reflection;
using System.Runtime.InteropServices;
using ShadowPlayApi;
using System.Windows.Threading;

namespace Experienceless
{
    public class SPApiWrapper
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string dllName);
        [DllImport("kernel32.dll")]
        static extern IntPtr FreeLibrary(IntPtr hModule);
        [DllImport("kernel32.dll")]
        static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
        

        IntPtr hNVSPapi;
        IntPtr hProxyInterface;
        ulong hSession;
        CShadowPlayApi api;
        //callbacks
        delegate int HotKeyCallback(IntPtr ptr, IntPtr ptr2);
        delegate int NotificationCallback(IntPtr ptr);
        HotKeyCallback hotkeyCB;
        NotificationCallback notificationCB;
        //local
        public EventHandler<string> OnFileSaved;
        public EventHandler<uint> OnCommandExecuted;
        public SPApiWrapper() {
            hNVSPapi = LoadLibrary(@"C:/Program Files (x86)/NVIDIA Corporation/ShadowPlay/nvspapi.dll");
            
            IntPtr hCreateShadowPlayApiInterface = GetProcAddress(hNVSPapi, "CreateShadowPlayApiInterface");
            CreateShadowPlayApiInterface createShadowPlayApiInterface 
                = (CreateShadowPlayApiInterface)Marshal.GetDelegateForFunctionPointer(hCreateShadowPlayApiInterface, typeof(CreateShadowPlayApiInterface));

            CShadowPlayApiProxyInterface ppvInterface = new CShadowPlayApiProxyInterface
            {
                api = Marshal.AllocHGlobal(4)
            };

            CreateShadowPlayApiParams createShadowPlayApiParams = new CreateShadowPlayApiParams
            {
                api_version = 0x10010,
                interface_ver = 0x10004,
                client = 0,
                ppvInterface = ppvInterface
            };
            
            int result = createShadowPlayApiInterface(ref createShadowPlayApiParams);
            hProxyInterface = Marshal.ReadIntPtr(ppvInterface.api);
            api = Marshal.PtrToStructure<CShadowPlayApi>(Marshal.ReadIntPtr(hProxyInterface));

            hotkeyCB += (IntPtr unk01, IntPtr unk02) =>
            {
                if ((bool)Application.Current.Properties["IsHotkeysEnabled"])
                {
                    byte hotkeyId = Marshal.ReadByte(unk02 + 20);
                    byte state = Marshal.ReadByte(unk02 + 21);
                    switch (hotkeyId)
                    {
                        case (byte)HotKeys.SAVE_IR: ExecuteCaptureCommand(IRAction.SaveInstantReplay); break;
                        case (byte)HotKeys.TOGGLE_RECORD: this.ToggleManualRecord(); break;
                    }
                }


                return 0;
            };
            notificationCB += (IntPtr unk01) =>
            {
                IntPtr obj = Marshal.ReadIntPtr(unk01 + 28);
                int msgId = Marshal.ReadInt32(obj);
                if (msgId == 0x00010218)
                {
                    if (OnFileSaved != null)
                    {
                        string filename = Marshal.PtrToStringAuto(obj + 16);
                        Application.Current.Dispatcher.Invoke(()=> { OnFileSaved(this, filename); });
                        
                    }

                }
                return 0;
            };
            RegisterCallback(1, hotkeyCB);
            RegisterCallback(2, notificationCB);
        }

        public uint GetStatus() {
            ShadowPlayApi.Args.GetStatus args = new ShadowPlayApi.Args.GetStatus()
            {
                api_version = 0x10010,
            };
            int result = api.GetStatus(hProxyInterface, ref args);
            if (result == 0)
            {
                return args.status;
            }
            else
            {
                return 0;
            }
        }

        public void EnableShadowPlay(bool value) {
            hSession = 0;
            ShadowPlayApi.Args.EnableShadowPlay args = new ShadowPlayApi.Args.EnableShadowPlay()
            {
                api_version = 0x10010,
                value = 1
            };
            int result = value ? api.EnableShadowPlay(hProxyInterface, ref args) : api.DisableShadowPlay(hProxyInterface, ref args);
        }


        public void RegisterCallback<TDelegate>(int type, TDelegate func) {
            ShadowPlayApi.Args.RegisterCallback args = new ShadowPlayApi.Args.RegisterCallback()
            {
                api_version = 0x10010,
                type = type,
                callback = Marshal.GetFunctionPointerForDelegate(func)
            };
            int result = api.RegisterCallback(hProxyInterface, ref args);
        }

        public void UnregisterCallback<TDelegate>(int type, TDelegate func)
        {
            ShadowPlayApi.Args.RegisterCallback args = new ShadowPlayApi.Args.RegisterCallback()
            {
                api_version = 0x10008,
                type = type,
                callback = Marshal.GetFunctionPointerForDelegate(func)
            };
            int result = api.UnegisterCallback(hProxyInterface, ref args);
        }

        public void ToggleManualRecord()
        {
            uint command = GetInstantReplayStatus(true) == 1 ? IRAction.ManualRecordStop : IRAction.ManualRecordStart;

            int result = ExecuteCaptureCommand(command);
        }

        public int ExecuteCaptureCommand(uint command)
        {
            if (hSession == 0) {
                CreateNewCaptureSession();
            }

            ShadowPlayApi.Args.CaptureControl capcontrol = new ShadowPlayApi.Args.CaptureControl()
            {
                api_version = 0x40038,
                hSession = hSession,
                command = command
            };
            int result = api.CaptureSessionControl(hProxyInterface, ref capcontrol);
            if (result == 0 && OnCommandExecuted != null)
            {
                Application.Current.Dispatcher.Invoke(() => { OnCommandExecuted(this, command); });
            }

            return result;
        }

        private void CreateNewCaptureSession() {
            ShadowPlayApi.Args.CreateCaptureSessionParams captureSessionParams = new ShadowPlayApi.Args.CreateCaptureSessionParams
            {
                api_version = 0x10020,
                sessiontype = 1,
                capcontroller = 3,
                hSession = 0
            };
            api.CreateCaptureSession(hProxyInterface, ref captureSessionParams);
            hSession = captureSessionParams.hSession;
        }

        public uint GetInstantReplayStatus(bool manual = false) {
            ShadowPlayApi.Args.CaptureSessionValue.INT value = new ShadowPlayApi.Args.CaptureSessionValue.INT(){
                header = new ShadowPlayApi.Args.CaptureSessionValue.Header() {
                    cmd_version = 1,
                    unk01 = 0x1c
                }
            };
            int size = Marshal.SizeOf(value);
            GetCaptureSessionParam(0x0c, ref value);
            return manual ? value.value : value.result;
        }

        public uint GetInstantReplayBufferLength() {
            ShadowPlayApi.Args.CaptureSessionValue.INT value = new ShadowPlayApi.Args.CaptureSessionValue.INT()
            {
                header = new ShadowPlayApi.Args.CaptureSessionValue.Header()
                {
                    unk01 = 0x08,
                    cmd_version = 1
                    
                }
            };
            int size = Marshal.SizeOf(value);
            int result = GetCaptureSessionParam(0x08, ref value);
            return value.value;
        }

        public int GetCaptureSessionParam<T>(ushort command, ref T value) {
            int size = Marshal.SizeOf(value);
            ShadowPlayApi.Args.GetCaptureSessionParams captureSessionParams = new ShadowPlayApi.Args.GetCaptureSessionParams
            {
                api_version = 0x10020,
                hSession = hSession == 0 ? uint.MaxValue : hSession,
                command = command,
                uiDataSize = Convert.ToUInt32(size),
                pvalue = Marshal.AllocCoTaskMem(size)
            };
            Marshal.StructureToPtr(value, captureSessionParams.pvalue, true);
            int result = api.GetCaptureSessionParam(hProxyInterface, ref captureSessionParams);
            if (result == 0)
            {
                value =  Marshal.PtrToStructure<T>(captureSessionParams.pvalue);
            }
            Marshal.FreeCoTaskMem(captureSessionParams.pvalue);
            return result;
        }

        public object GetProperty(string name) {
            ShadowPlayApi.Args.PropertyArgs args = new ShadowPlayApi.Args.PropertyArgs
            {
                api_version = 0x10058,
                name = name,
            };
            api.GetProperty(hProxyInterface, ref args);
            string s = args.value?.ToString();
            return args.value;
        }

        public int SetProperty(string name, object value) {
            ShadowPlayApi.Args.PropertyArgs args = new ShadowPlayApi.Args.PropertyArgs
            {
                api_version = 0x10058,
                name = name,
                value = value
            };
            return api.SetProperty(hProxyInterface, ref args);
        }

        ~SPApiWrapper() {
            UnregisterCallback(1, hotkeyCB);
            UnregisterCallback(2, notificationCB);
            FreeLibrary(hNVSPapi);
        }
    }
}
