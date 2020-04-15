using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace ShadowPlayApi
{
    delegate int CreateShadowPlayApiInterface(ref CreateShadowPlayApiParams args);
    delegate int ReleaseInterface(int notUsed, int firstParam, uint secondParam);
    delegate int OnInstall(int notUsed, int firstParam, uint secondParam);
    delegate int OnUninstall(int notUsed, uint firstParam, uint secondParam);
    delegate int EnableShadowPlay(IntPtr ppvInterface, ref Args.EnableShadowPlay args);
    delegate int DisableShadowPlay(IntPtr ppvInterface, ref Args.EnableShadowPlay args);
    delegate int GetStatus(IntPtr ppvInterface, ref Args.GetStatus args);
    delegate int SetProperty(IntPtr ppvInterface, ref Args.PropertyArgs args);
    delegate int GetProperty(IntPtr ppvInterface, ref Args.PropertyArgs args);
    delegate int UnkFunc1(int notUsed, int firstParam);
    delegate int UnkFunc2(int notUsed, int firstParam);
    delegate int CreateCaptureSession(IntPtr ppvInterface, ref Args.CreateCaptureSessionParams args);
    delegate int DestroyCaptureSession(int notUsed, int firstParam);
    delegate int SetCaptureSessionSettings(int notUsed, int firstParam);
    delegate int GetCaptureSessionSettings(int notUsed, int firstParam);
    delegate int CaptureSessionControl(IntPtr ppvInterface, ref Args.CaptureControl args);
    delegate int GetCaptureSessionParam(IntPtr ppvInterface, ref Args.GetCaptureSessionParams args);
    delegate int SetCaptureSessionParam(int notUsed, int firstParam);
    delegate int RegisterCallback(IntPtr ppvInterface, ref Args.RegisterCallback args);
    delegate int UnegisterCallback(IntPtr ppvInterface, ref Args.RegisterCallback args);

    public struct CreateShadowPlayApiParams {
        public uint api_version;
        public uint interface_ver;
        public uint client;
        public CShadowPlayApiProxyInterface ppvInterface;
};
    public struct CShadowPlayApiProxyInterface {
        public IntPtr api;
    }
    #pragma warning disable 0649
    struct CShadowPlayApi
    {
        public ReleaseInterface ReleaseInterface;
        public OnInstall OnInstall;
        public OnUninstall OnUninstall;
        public EnableShadowPlay EnableShadowPlay;
        public DisableShadowPlay DisableShadowPlay;
        public GetStatus GetStatus;
        public SetProperty SetProperty;
        public GetProperty GetProperty;
        public UnkFunc1 UnkFunc1;
        public UnkFunc2 UnkFunc2;
        public CreateCaptureSession CreateCaptureSession;
        public DestroyCaptureSession DestroyCaptureSession;
        public SetCaptureSessionSettings SetCaptureSessionSettings;
        public GetCaptureSessionSettings GetCaptureSessionSettings;
        public CaptureSessionControl CaptureSessionControl;
        public GetCaptureSessionParam GetCaptureSessionParam;
        public SetCaptureSessionParam SetCaptureSessionParam;
        public RegisterCallback RegisterCallback;
        public UnegisterCallback UnegisterCallback;
    };
    #pragma warning restore 0649

    namespace Args
    {
        namespace CaptureSessionValue
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct Header
            {
                public ushort unk01;
                public ushort cmd_version;

            }
            [StructLayout(LayoutKind.Sequential)]
            public struct INT {
                public Header header;
                public uint value;
                public uint result;
            }
        }

        public struct GetStatus
        {
            public uint api_version;
            public uint isShadowPlayEnabled;
            public uint isShadowPlayEnabledUser;
            public uint status;
        }

        public struct EnableShadowPlay {
            public uint api_version;
            public uint unk01;
            public uint unk02;
            public uint value;
        }

        public struct RegisterCallback{
            public int api_version;
            public int type;
            public IntPtr callback;
        }

        public struct CaptureControl {
            public uint api_version;
            public ulong hSession;
            public uint capcontroller;
            public uint unk01;
            public uint unk02;
            public uint unk03;
            public uint unk04;
            public uint unk05;
            public uint command;
        }

        public struct CreateCaptureSessionParams
        {
            public uint api_version;
            public uint unk1;
            public ulong hSession;
            public uint sessiontype;
            public uint capcontroller;

        };
        
        public struct GetCaptureSessionParams
        {
            public uint api_version;
            public uint unk1;
            public ulong hSession;
            public uint command;
            public uint uiDataSize;
            public IntPtr pvalue;
        };
        
        public struct UnkObject
        {
            public uint unk1;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
            public string name;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
            public string value;

        }

        public struct PropertyArgs
        {
            public uint api_version;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string name;
            public uint unk;
            [MarshalAs(UnmanagedType.Struct)]
            public object value;
        };
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct VariantStr
    {
        public VarEnum vt;
        public ushort wReserved1;
        public ushort wReserved2;
        public ushort wReserved3;
        public IntPtr data01;
        public IntPtr data02;
    }
    class Params
    {   
        public const string TempFilePath = "TempFilePath";
        public const string DefaultPath = "DefaultPath";
        //Качество (VeryGood)
        public const string EncoderProfile = "EncoderProfile";
        //время повтора в сек
        public const string DVRBufferLen = "DVRBufferLen";
        public const string RecordingFPS = "RecordingFPS";
        //Разрешение (InGame)
        public const string Resolution = "yuJ4f7df";
        public const string BitrateBps = "m7Yhs4x6";
        //Запись рабочего стола
        public const string DesktopCapture = "DwmEnabled";
    }

    class IRAction {
        public const uint ManualRecordStart = 1;
        public const uint ManualRecordStop = 2;
        public const uint EnableInstantReplay = 3;
        public const uint DisableInstantReplay = 4;
        public const uint SaveInstantReplay = 5;
    }

    public class ShadowPlayStatus
    {
        public const uint Off = 1;
        public const uint Unknown = 2;
        public const uint On = 3;
    }

    public enum HotKeys
    {
        SAVE_IR = 1,
        TOGGLE_RECORD = 2,
        TOGGLE_OVERLAY = 8,
        SAVE_SCREENSHOT = 9
    }
}


