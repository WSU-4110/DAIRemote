using Newtonsoft.Json;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DisplayProfileManager;

public class DisplayConfig
{
    public const int ERROR_CONST = 0;
    private static AudioManager.AudioDeviceManager audioManager = AudioManager.AudioDeviceManager.GetInstance();
    private static string profileAudioDevice;
    public static event EventHandler<NotificationEventArgs> NotificationRequested;

    public static void RequestNotification(string notificationText)
    {
        NotificationRequested?.Invoke(null, new NotificationEventArgs(notificationText));
    }

    public static string GetDisplayProfilesDirectory()
    {
        string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DAIRemote/DisplayProfiles");

        if (!Directory.Exists(folderPath))
        {
            _ = Directory.CreateDirectory(folderPath);
        }
        return folderPath;
    }

    public static string[] GetDisplayProfiles()
    {
        return Directory.GetFiles(GetDisplayProfilesDirectory(), "*.json");
    }

    public static string GetFullDisplayProfilePath(string profileName)
    {
        // Get the directory path where display profiles are stored
        string directoryPath = GetDisplayProfilesDirectory();

        // Get all JSON files in the directory
        string[] profiles = Directory.GetFiles(directoryPath, "*.json");

        // Loop through the profiles and find the matching one
        foreach (string profile in profiles)
        {
            // Get the file name without extension
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(profile);

            // Check if the file name matches the provided profile name
            if (string.Equals(fileNameWithoutExtension, profileName, StringComparison.OrdinalIgnoreCase))
            {
                return profile;  // Return the full path of the matching file
            }
        }

        // Return null or a default path if the profile is not found
        return null;
    }

    private static void setAudioDevice(string device)
    {
        if (device != null)
        {
            audioManager.RefreshAudioDeviceSubscriptions();
            audioManager.SetDefaultAudioDevice(device);
        }
    }

    /*
    The DISPLAYCONFIG_DEVICE_INFO_TYPE enumeration specifies the type of display device info 
    to configure or obtain through the DisplayConfigSetDeviceInfo or DisplayConfigGetDeviceInfo function.
    https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ne-wingdi-displayconfig_device_info_type
    */
    public enum DISPLAYCONFIG_DEVICE_INFO_TYPE : uint
    {
        DISPLAYCONFIG_DEVICE_INFO_GET_SOURCE_NAME = 1,
        DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME = 2,
        DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_PREFERRED_MODE = 3,
        DISPLAYCONFIG_DEVICE_INFO_GET_ADAPTER_NAME = 4,
        DISPLAYCONFIG_DEVICE_INFO_SET_TARGET_PERSISTENCE = 5,
        DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_BASE_TYPE = 6,
        DISPLAYCONFIG_DEVICE_INFO_GET_SUPPORT_VIRTUAL_RESOLUTION = 7,
        DISPLAYCONFIG_DEVICE_INFO_SET_SUPPORT_VIRTUAL_RESOLUTION = 8,
        DISPLAYCONFIG_DEVICE_INFO_GET_ADVANCED_COLOR_INFO = 9,
        DISPLAYCONFIG_DEVICE_INFO_SET_ADVANCED_COLOR_STATE = 10,
        DISPLAYCONFIG_DEVICE_INFO_GET_SDR_WHITE_LEVEL = 11,
        DISPLAYCONFIG_DEVICE_INFO_FORCE_UINT32 = 0xFFFFFFFF
    }

    /*
    The DISPLAYCONFIG_MODE_INFO_TYPE enumeration specifies that the information that is contained within the DISPLAYCONFIG_MODE_INFO structure is either source or target mode.
    https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ne-wingdi-displayconfig_mode_info_type
    */
    public enum DISPLAYCONFIG_MODE_INFO_TYPE : uint
    {
        DISPLAYCONFIG_MODE_INFO_TYPE_ZERO = 0,
        DISPLAYCONFIG_MODE_INFO_TYPE_SOURCE = 1,
        DISPLAYCONFIG_MODE_INFO_TYPE_TARGET = 2,
        DISPLAYCONFIG_MODE_INFO_TYPE_DESKTOP_IMAGE = 3,
        DISPLAYCONFIG_MODE_INFO_TYPE_FORCE_UINT32 = 0xFFFFFFFF
    }

    /*
    The DISPLAYCONFIG_MODE_INFO structure contains either source mode or target mode information.
    https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-displayconfig_mode_info
    */
    [StructLayout(LayoutKind.Explicit)]
    public struct DISPLAYCONFIG_MODE_INFO
    {
        [FieldOffset(0)]
        public DISPLAYCONFIG_MODE_INFO_TYPE infoType;

        [FieldOffset(4)]
        public uint id;

        [FieldOffset(8)]
        public LUID adapterId;

        [FieldOffset(16)]
        public DISPLAYCONFIG_TARGET_MODE targetMode;

        [FieldOffset(16)]
        public DISPLAYCONFIG_SOURCE_MODE sourceMode;
    }

    /*
    The DISPLAYCONFIG_SOURCE_MODE structure represents a point or an offset in a two-dimensional space.
    https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-displayconfig_source_mode
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_SOURCE_MODE
    {
        public uint width;
        public uint height;
        public DISPLAYCONFIG_PIXELFORMAT pixelFormat;
        public POINTL position;
    }

    /*
    The DISPLAYCONFIG_PIXELFORMAT enumeration specifies pixel format in various bits per pixel (BPP) values.
    https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ne-wingdi-displayconfig_pixelformat
    */
    public enum DISPLAYCONFIG_PIXELFORMAT : uint
    {
        DISPLAYCONFIG_PIXELFORMAT_ZERO = 0x0,
        DISPLAYCONFIG_PIXELFORMAT_8BPP = 1,
        DISPLAYCONFIG_PIXELFORMAT_16BPP = 2,
        DISPLAYCONFIG_PIXELFORMAT_24BPP = 3,
        DISPLAYCONFIG_PIXELFORMAT_32BPP = 4,
        DISPLAYCONFIG_PIXELFORMAT_NONGDI = 5,
        DISPLAYCONFIG_PIXELFORMAT_FORCE_UINT32 = 0xFFFFFFFF
    }

    /*
    The DISPLAYCONFIG_TARGET_MODE structure describes a display path target mode.
    https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-displayconfig_target_mode
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_TARGET_MODE
    {
        public DISPLAYCONFIG_VIDEO_SIGNAL_INFO targetVideoSignalInfo;
    }

    /*
    The DISPLAYCONFIG_VIDEO_SIGNAL_INFO structure contains information about the video signal for a display.
    https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-displayconfig_video_signal_info
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_VIDEO_SIGNAL_INFO
    {
        public ulong pixelRate;
        public DISPLAYCONFIG_RATIONAL hSyncFreq;
        public DISPLAYCONFIG_RATIONAL vSyncFreq;
        public DISPLAYCONFIG_2DREGION activeSize;
        public DISPLAYCONFIG_2DREGION totalSize;

        public D3DKMDT_VIDEO_SIGNAL_STANDARD videoStandard;
        public DISPLAYCONFIG_SCANLINE_ORDERING scanLineOrdering;
    }

    /*
     The D3DKMDT_VIDEO_SIGNAL_STANDARD enumeration contains constants that represent video signal standards.
     https://learn.microsoft.com/en-us/windows-hardware/drivers/ddi/d3dkmdt/ne-d3dkmdt-_d3dkmdt_video_signal_standard
     */
    public enum D3DKMDT_VIDEO_SIGNAL_STANDARD : UInt32
    {
        D3DKMDT_VIDEO_SIGNAL_STANDARD_UNINITIALIZED = 0,
        D3DKMDT_VIDEO_SIGNAL_STANDARD_VESADMT = 1,
        D3DKMDT_VIDEO_SIGNAL_STANDARD_VESAGTF = 2,
        D3DKMDT_VIDEO_SIGNAL_STANDARD_VESACVT = 3,
        D3DKMDT_VIDEO_SIGNAL_STANDARD_IBM = 4,
        D3DKMDT_VIDEO_SIGNAL_STANDARD_APPLE = 5,
        D3DKMDT_VIDEO_SIGNAL_STANDARD_NTSCM = 6,
        D3DKMDT_VIDEO_SIGNAL_STANDARD_NTSCJ = 7,
        D3DKMDT_VIDEO_SIGNAL_STANDARD_NTSC443 = 8,
        D3DKMDT_VIDEO_SIGNAL_STANDARD_PALB = 9,
        D3DKMDT_VIDEO_SIGNAL_STANDARD_PALB1 = 10,
        D3DKMDT_VIDEO_SIGNAL_STANDARD_PALG = 11,
        D3DKMDT_VIDEO_SIGNAL_STANDARD_PALH = 12,
        D3DKMDT_VIDEO_SIGNAL_STANDARD_PALI = 13,
        D3DKMDT_VIDEO_SIGNAL_STANDARD_PALD = 14,
        D3DKMDT_VIDEO_SIGNAL_STANDARD_PALN = 15,
        D3DKMDT_VIDEO_SIGNAL_STANDARD_PALNC = 16,
        D3DKMDT_VIDEO_SIGNAL_STANDARD_SECAMB = 17,
        D3DKMDT_VIDEO_SIGNAL_STANDARD_SECAMD = 18,
        D3DKMDT_VIDEO_SIGNAL_STANDARD_SECAMG = 19,
        D3DKMDT_VIDEO_SIGNAL_STANDARD_SECAMH = 20,
        D3DKMDT_VIDEO_SIGNAL_STANDARD_SECAMK = 21,
        D3DKMDT_VIDEO_SIGNAL_STANDARD_SECAMK1 = 22,
        D3DKMDT_VIDEO_SIGNAL_STANDARD_SECAML = 23,
        D3DKMDT_VIDEO_SIGNAL_STANDARD_SECAML1 = 24,
        D3DKMDT_VIDEO_SIGNAL_STANDARD_EIA861 = 25,
        D3DKMDT_VIDEO_SIGNAL_STANDARD_EIA861A = 26,
        D3DKMDT_VIDEO_SIGNAL_STANDARD_EIA861B = 27,
        D3DKMDT_VIDEO_SIGNAL_STANDARD_PALK = 28,
        D3DKMDT_VIDEO_SIGNAL_STANDARD_PALK1 = 29,
        D3DKMDT_VIDEO_SIGNAL_STANDARD_PALL = 30,
        D3DKMDT_VIDEO_SIGNAL_STANDARD_PALM = 31,
        D3DKMDT_VIDEO_SIGNAL_STANDARD_OTHER = 255,
        D3DKMDT_VIDEO_SIGNAL_STANDARD_USB = 65791
    }

    /*
    The DISPLAYCONFIG_2DREGION structure represents a point or an offset in a two-dimensional space.
    https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-displayconfig_2dregion
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_2DREGION
    {
        public uint cx; // Width
        public uint cy; // Height
    }

    /*
    The DisplayConfigGetDeviceInfo function retrieves display configuration information about the device.
    https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-displayconfiggetdeviceinfo
    */
    [DllImport("User32.dll")]
    private static extern int DisplayConfigGetDeviceInfo(ref DISPLAYCONFIG_TARGET_DEVICE_NAME deviceName);

    /*
    The DISPLAYCONFIG_DEVICE_INFO_HEADER structure contains display information about the device.
    https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-displayconfig_device_info_header
    */
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DISPLAYCONFIG_DEVICE_INFO_HEADER
    {
        public DISPLAYCONFIG_DEVICE_INFO_TYPE type;
        public uint size;
        public LUID adapterId;
        public uint id;
    }

    /*
    The DISPLAYCONFIG_SOURCE_DEVICE_NAME structure contains the GDI device name for the source or view.
    https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-displayconfig_source_device_name
    */
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DISPLAYCONFIG_SOURCE_DEVICE_NAME : IDISPLAYCONFIG_INFO_CONTRACT
    {
        private const int cchDeviceName = 32;

        public DISPLAYCONFIG_DEVICE_INFO_HEADER header;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = cchDeviceName)]
        public string viewGDIDeviceName;
    }

    /*
    The DISPLAYCONFIG_TARGET_DEVICE_NAME structure contains information about the target.
    https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-displayconfig_target_device_name
    */
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DISPLAYCONFIG_TARGET_DEVICE_NAME
    {
        public DISPLAYCONFIG_DEVICE_INFO_HEADER header;
        public DISPLAYCONFIG_TARGET_DEVICE_NAME_FLAGS flags;
        public DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY outputTechnology;
        public ushort edidManufactureId;
        public ushort edidProductCodeId;
        public uint connectorInstance;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string monitorFriendlyDeviceName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string monitorDevicePath;
    }

    /*
    The DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY enumeration specifies the target's connector type.
    https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ne-wingdi-displayconfig_video_output_technology
    */
    public enum DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY : uint
    {
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_OTHER = 4294967295,         // This is equivalent to -1
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_HD15 = 0,
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_SVIDEO = 1,
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_COMPOSITE_VIDEO = 2,
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_COMPONENT_VIDEO = 3,
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DVI = 4,
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_HDMI = 5,
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_LVDS = 6,
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_D_JPN = 8,
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_SDI = 9,
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DISPLAYPORT_EXTERNAL = 10,
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DISPLAYPORT_EMBEDDED = 11,
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_UDI_EXTERNAL = 12,
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_UDI_EMBEDDED = 13,
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_SDTVDONGLE = 14,
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_MIRACAST = 15,
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_INDIRECT_WIRED = 16,
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_INDIRECT_VIRTUAL = 17,
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_INTERNAL = 0x80000000,
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_FORCE_UINT32 = 0xFFFFFFFF
    }

    /*
    The DISPLAYCONFIG_TARGET_DEVICE_NAME_FLAGS structure contains information about a target device.
    https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-displayconfig_target_device_name_flags
    */
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct DISPLAYCONFIG_TARGET_DEVICE_NAME_FLAGS
    {
        private readonly uint value;
    }

    /*
    The DISPLAYCONFIG_ADAPTER_NAME structure contains information about the display adapter.
    https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-displayconfig_adapter_name
    */
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DISPLAYCONFIG_ADAPTER_NAME
    {
        public DISPLAYCONFIG_DEVICE_INFO_HEADER header;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string adapterDevicePath;
    }

    /*
    Specifies base output technology info for a given target ID.
    https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-displayconfig_target_base_type
    */
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DISPLAYCONFIG_TARGET_BASE_TYPE
    {
        public DISPLAYCONFIG_DEVICE_INFO_HEADER header;
        public DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY baseOutputTechnology;
    }

    // POINTL used in DISPLAYCONFIG_DESKTOP_IMAGE_INFO
    [StructLayout(LayoutKind.Sequential)]
    public struct POINTL
    {
        public int x;
        public int y;
    }

    /*
    The DISPLAYCONFIG_SCALING enumeration specifies the scaling transformation applied to content displayed on a video present network (VidPN) present path.
    https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ne-wingdi-displayconfig_scaling
    */
    public enum DISPLAYCONFIG_SCALING : uint
    {
        DISPLAYCONFIG_SCALING_ZERO = 0x0,
        DISPLAYCONFIG_SCALING_IDENTITY = 1,
        DISPLAYCONFIG_SCALING_CENTERED = 2,
        DISPLAYCONFIG_SCALING_STRETCHED = 3,
        DISPLAYCONFIG_SCALING_ASPECTRATIOCENTEREDMAX = 4,
        DISPLAYCONFIG_SCALING_CUSTOM = 5,
        DISPLAYCONFIG_SCALING_PREFERRED = 128,
        DISPLAYCONFIG_SCALING_FORCE_UINT32 = 0xFFFFFFFF
    }

    [Flags]
    public enum SetDisplayConfigFlags : uint
    {
        SetDisplayConfigFlags_ZERO = 0,
        SetDisplayConfigFlags_TOPOLOGYINTERNAL = 0X00000001,
        SetDisplayConfigFlags_TOPOLOGYCLONE = 0X00000002,
        SetDisplayConfigFlags_TOPOLOGYEXTEND = 0X00000004,
        SetDisplayConfigFlags_TOPOLOGYEXTERNAL = 0X00000008,
        SetDisplayConfigFlags_TOPOLOGYSUPPLIED = 0X00000010,
        SetDisplayConfigFlags_USESUPPLIEDDISPLAYCONFIG = 0X00000020,
        SetDisplayConfigFlags_VALIDATE = 0X00000040,
        SetDisplayConfigFlags_APPLY = 0X00000080,
        SetDisplayConfigFlags_NOOPTIMIZATION = 0X00000100,
        SetDisplayConfigFlags_SAVETODATABASE = 0X00000200,
        SetDisplayConfigFlags_ALLOWCHANGES = 0X00000400,
        SetDisplayConfigFlags_PATHPERSISTIFREQUIRED = 0X00000800,
        SetDisplayConfigFlags_FORCEMODEENUMERATION = 0X00001000,
        SetDisplayConfigFlags_ALLOWPATHORDERCHANGES = 0X00002000,
        SetDisplayConfigFlags_VIRTUALMODEAWARE = 0X00008000,

        SetDisplayConfigFlags_USEDATABASECURRENT = SetDisplayConfigFlags_TOPOLOGYINTERNAL | SetDisplayConfigFlags_TOPOLOGYCLONE | SetDisplayConfigFlags_TOPOLOGYEXTEND | SetDisplayConfigFlags_TOPOLOGYEXTERNAL
    }

    /*
    The SetDisplayConfig function modifies the display topology, source, and target modes by exclusively enabling the specified paths in the current session.
    https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setdisplayconfig
    */
    [DllImport("User32.dll")]
    private static extern int SetDisplayConfig(
        uint numPathArrayElements,
        [In] DISPLAYCONFIG_PATH_INFO[] pathArray,
        uint numModeInfoArrayElements,
        [In] DISPLAYCONFIG_MODE_INFO[] modeInfoArray,
        SetDisplayConfigFlags flags
    );

    /*
    The DISPLAYCONFIG_PATH_INFO structure is used to describe a single path from a target to a source.
    https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-displayconfig_path_info
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_PATH_INFO
    {
        public DISPLAYCONFIG_PATH_SOURCE_INFO sourceInfo;
        public DISPLAYCONFIG_PATH_TARGET_INFO targetInfo;
        public uint flags;
    }

    /*
    The DISPLAYCONFIG_PATH_SOURCE_INFO structure contains source information for a single path.
    https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-displayconfig_path_source_info
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct LUID
    {
        public uint LowPart;
        public int HighPart;
    }

    [Flags]
    public enum DISPLAYCONFIG_SOURCE_STATUS
    {
        DISPLAYCONFIG_SOURCE_STATUS_Zero = 0x0,
        DISPLAYCONFIG_SOURCE_STATUS_INUSE = 0x00000001
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_PATH_SOURCE_INFO
    {
        public LUID adapterId;
        public uint id;
        public uint modeInfoIdx;
        public DISPLAYCONFIG_SOURCE_STATUS statusFlags;
    }

    [Flags]
    public enum DISPLAYCONFIG_PATH_TARGET_STATUS : uint
    {
        DISPLAYCONFIG_PATH_TARGET_STATUS_ZERO = 0X0,
        DISPLAYCONFIG_PATH_TARGET_STATUS_INUSE = 0X00000001,
        DISPLAYCONFIG_PATH_TARGET_STATUS_FORCIBLE = 0X00000002,
        DISPLAYCONFIG_PATH_TARGET_STATUS_FORCEDAVAILABILITYBOOT = 0X00000004,
        DISPLAYCONFIG_PATH_TARGET_STATUS_FORCEDAVAILABILITYPATH = 0X00000008,
        DISPLAYCONFIG_PATH_TARGET_STATUS_FORCEDAVAILABILITYSYSTEM = 0X00000010,
        DISPLAYCONFIG_PATH_TARGET_STATUS_IS_HMD = 0x00000020
    }

    /*
    The DISPLAYCONFIG_PATH_TARGET_INFO structure contains target information for a single path.
    https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-displayconfig_path_target_info
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_PATH_TARGET_INFO
    {
        public LUID adapterId;
        public uint id;
        public uint modeInfoIdx;
        public DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY outputTechnology;
        public DISPLAYCONFIG_ROTATION rotation;
        public DISPLAYCONFIG_SCALING scaling;
        public DISPLAYCONFIG_RATIONAL refreshRate;
        public DISPLAYCONFIG_SCANLINE_ORDERING scanLineOrdering;
        public bool targetAvailable;
        public DISPLAYCONFIG_PATH_TARGET_STATUS statusFlags;
    }

    /*
    The DISPLAYCONFIG_ROTATION enumeration specifies the clockwise rotation of the display.
    https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ne-wingdi-displayconfig_rotation
    */
    [Flags]
    public enum DISPLAYCONFIG_ROTATION : uint
    {
        DISPLAYCONFIG_ROTATION_ZERO = 0x0,
        DISPLAYCONFIG_ROTATION_IDENTITY = 1,
        DISPLAYCONFIG_ROTATION_ROTATE90 = 2,
        DISPLAYCONFIG_ROTATION_ROTATE180 = 3,
        DISPLAYCONFIG_ROTATION_ROTATE270 = 4,
        DISPLAYCONFIG_ROTATION_FORCE_UINT32 = 0xFFFFFFFF
    }

    /*
    The DISPLAYCONFIG_SCANLINE_ORDERING enumeration specifies the method that the display uses to create an image on a screen.
    https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ne-wingdi-displayconfig_scanline_ordering
    */
    public enum DISPLAYCONFIG_SCANLINE_ORDERING : uint
    {
        DISPLAYCONFIG_SCANLINE_ORDERING_UNSPECIFIED = 0,
        DISPLAYCONFIG_SCANLINE_ORDERING_PROGRESSIVE = 1,
        DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED = 2,
        DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED_UPPERFIELDFIRST = DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED,
        DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED_LOWERFIELDFIRST = 3,
        DISPLAYCONFIG_SCANLINE_ORDERING_FORCE_UINT32 = 0xFFFFFFFF
    }

    /*
    The DISPLAYCONFIG_RATIONAL structure describes a fractional value that represents vertical and horizontal frequencies of a video mode (that is, vertical sync and horizontal sync).
    https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-displayconfig_rational
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_RATIONAL
    {
        public uint Numerator;
        public uint Denominator;
    }

    [Flags]
    public enum QueryDisplayConfigFlags : uint
    {
        QueryDisplayConfigFlags_ZERO = 0X0,
        QueryDisplayConfigFlags_ALLPATHS = 0X00000001,
        QueryDisplayConfigFlags_ONLYACTIVEPATHS = 0X00000002,
        QueryDisplayConfigFlags_DATABASECURRENT = 0X00000004,
        QueryDisplayConfigFlags_VIRTUALMODEAWARE = 0X00000010,
        QueryDisplayConfigFlags_INCLUDEHMD = 0X00000020
    }

    /*
    The QueryDisplayConfig function retrieves information about all possible display paths for all display devices, or views, in the current setting.
    https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-querydisplayconfig
    */
    [DllImport("User32.dll")]
    private static extern int QueryDisplayConfig(
        QueryDisplayConfigFlags flags,
        ref uint numPathArrayElements,
        [Out] DISPLAYCONFIG_PATH_INFO[] pathInfoArray,
        ref uint numModeInfoArrayElements,
        [Out] DISPLAYCONFIG_MODE_INFO[] modeInfoArray,
        IntPtr z
    );

    [DllImport("User32.dll")]
    private static extern int GetDisplayConfigBufferSizes(QueryDisplayConfigFlags flags, out uint numPathArrayElements, out uint numModeInfoArrayElements);

    /*
    The DISPLAYCONFIG_TOPOLOGY_ID enumeration specifies the type of display topology.
    https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ne-wingdi-displayconfig_topology_id
    */
    [Flags]
    public enum DISPLAYCONFIG_TOPOLOGY_ID : uint
    {
        DISPLAYCONFIG_TOPOLOGY_ZERO = 0x0,
        DISPLAYCONFIG_TOPOLOGY_INTERNAL = 0x00000001,
        DISPLAYCONFIG_TOPOLOGY_CLONE = 0x00000002,
        DISPLAYCONFIG_TOPOLOGY_EXTEND = 0x00000004,
        DISPLAYCONFIG_TOPOLOGY_EXTERNAL = 0x00000008,
        DISPLAYCONFIG_TOPOLOGY_FORCE_UINT32 = 0xFFFFFFFF
    }

    public enum StatusCode : uint
    {
        StatusCode_SUCCESS = 0,
        StatusCode_INVALIDPARAMETER = 87,
        StatusCode_NOTSUPPORTED = 50,
        StatusCode_ACCESSDENIED = 5,
        StatusCode_GENFAILURE = 31,
        StatusCode_BADCONFIGURATION = 1610,
        StatusCode_INSUFFICIENTBUFFER = 122
    }

    public interface IDISPLAYCONFIG_INFO_CONTRACT
    {
    }

    public static string MonitorFriendlyName(LUID adapterId, uint targetId)
    {
        DISPLAYCONFIG_TARGET_DEVICE_NAME displayName = new()
        {
            header =
            {
                size = (uint)Marshal.SizeOf(typeof (DISPLAYCONFIG_TARGET_DEVICE_NAME)),
                adapterId = adapterId,
                id = targetId,
                type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME
            }
        };
        int error = DisplayConfigGetDeviceInfo(ref displayName);
        return error != ERROR_CONST ? throw new Win32Exception(error) : displayName.monitorFriendlyDeviceName;
    }

    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public struct MonitorAdditionalInfo
    {
        public ushort manufactureId { get; set; }
        public ushort productCodeId { get; set; }
        public bool valid { get; set; }

        [JsonPropertyName("monitorDevicePath")]
        public string monitorDevicePath64
        {
            readonly get
            {
                string outValue = monitorDevicePath ?? "";
                return Convert.ToBase64String(Encoding.UTF32.GetBytes(outValue));
            }
            set
            {
                if (value == null)
                {
                    monitorDevicePath = null;
                    return;
                }

                monitorDevicePath = Encoding.UTF32.GetString(Convert.FromBase64String(value));
            }
        }

        [System.Text.Json.Serialization.JsonIgnore]
        public string monitorDevicePath;

        [JsonPropertyName("monitorFriendlyDevice")]
        public string monitorFriendlyDevice64
        {
            readonly get
            {
                string outValue = monitorFriendlyDevice ?? "";
                return Convert.ToBase64String(Encoding.UTF32.GetBytes(outValue));
            }
            set
            {
                if (value == null)
                {
                    monitorFriendlyDevice = null;
                    return;
                }

                monitorFriendlyDevice = Encoding.UTF32.GetString(Convert.FromBase64String(value));
            }
        }

        [System.Text.Json.Serialization.JsonIgnore]
        public string monitorFriendlyDevice;
    }

    public static MonitorAdditionalInfo GetMonitorAdditionalInfo(LUID adapterId, uint targetId)
    {
        MonitorAdditionalInfo additionalInfo = new();
        DISPLAYCONFIG_TARGET_DEVICE_NAME monitorName = new()
        {
            header =
            {
                size = (uint)Marshal.SizeOf(typeof (DISPLAYCONFIG_TARGET_DEVICE_NAME)),
                adapterId = adapterId,
                id = targetId,
                type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME
            }
        };
        int error = DisplayConfigGetDeviceInfo(ref monitorName);
        if (error != ERROR_CONST)
        {
            throw new Win32Exception(error);
        }

        additionalInfo.valid = true;
        additionalInfo.manufactureId = monitorName.edidManufactureId;
        additionalInfo.productCodeId = monitorName.edidProductCodeId;
        additionalInfo.monitorDevicePath = monitorName.monitorDevicePath;
        additionalInfo.monitorFriendlyDevice = monitorName.monitorFriendlyDeviceName;

        return additionalInfo;
    }

    // To ensure type safety
    /// <typeparam name="T"></typeparam>
    /// <param name="displayConfig"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    private static StatusCode MarshalStructureAndCall<T>(ref T displayConfig,
        Func<IntPtr, StatusCode> func) where T : IDISPLAYCONFIG_INFO_CONTRACT
    {
        nint ptr = Marshal.AllocHGlobal(Marshal.SizeOf(displayConfig));
        Marshal.StructureToPtr(displayConfig, ptr, false);

        StatusCode returnValue = func(ptr);

        displayConfig = (T)Marshal.PtrToStructure(ptr, displayConfig.GetType());

        Marshal.FreeHGlobal(ptr);
        return returnValue;
    }

    public static bool GetDisplaySettings(ref DISPLAYCONFIG_PATH_INFO[] pathInfoArray, ref DISPLAYCONFIG_MODE_INFO[] modeInfoArray, ref MonitorAdditionalInfo[] additionalInfo, bool ActiveOnly)
    {
        QueryDisplayConfigFlags queryFlags = QueryDisplayConfigFlags.QueryDisplayConfigFlags_ALLPATHS;
        if (ActiveOnly)
        {
            queryFlags = QueryDisplayConfigFlags.QueryDisplayConfigFlags_ONLYACTIVEPATHS;
        }

        int status = GetDisplayConfigBufferSizes(queryFlags, out uint numPathArrayElements, out uint numModeInfoArrayElements);
        if (status == 0)
        {
            pathInfoArray = new DISPLAYCONFIG_PATH_INFO[numPathArrayElements];
            modeInfoArray = new DISPLAYCONFIG_MODE_INFO[numModeInfoArrayElements];
            additionalInfo = new MonitorAdditionalInfo[numModeInfoArrayElements];

            status = QueryDisplayConfig(queryFlags,
                                                   ref numPathArrayElements, pathInfoArray, ref numModeInfoArrayElements,
                                                   modeInfoArray, IntPtr.Zero);

            if (status == 0)
            {
                // Removal of bad elements in modeInfo 
                int validCount = 0;
                foreach (DISPLAYCONFIG_MODE_INFO modeInfo in modeInfoArray)
                {
                    if (modeInfo.infoType != DISPLAYCONFIG_MODE_INFO_TYPE.DISPLAYCONFIG_MODE_INFO_TYPE_ZERO)
                    {
                        validCount++;
                    }
                }
                if (validCount > 0)
                {
                    DISPLAYCONFIG_MODE_INFO[] tempInfoArray = new DISPLAYCONFIG_MODE_INFO[modeInfoArray.Length];
                    modeInfoArray.CopyTo(tempInfoArray, 0);
                    modeInfoArray = new DISPLAYCONFIG_MODE_INFO[validCount];
                    int index = 0;
                    foreach (DISPLAYCONFIG_MODE_INFO modeInfo in tempInfoArray)
                    {
                        if (modeInfo.infoType != DISPLAYCONFIG_MODE_INFO_TYPE.DISPLAYCONFIG_MODE_INFO_TYPE_ZERO)
                        {
                            modeInfoArray[index] = modeInfo;
                            index++;
                        }
                    }
                }

                // Removal of unavailable pathInfo elements
                validCount = 0;
                foreach (DISPLAYCONFIG_PATH_INFO pathInfo in pathInfoArray)
                {
                    if (pathInfo.targetInfo.targetAvailable)
                    {
                        validCount++;
                    }
                }
                if (validCount > 0)
                {
                    DISPLAYCONFIG_PATH_INFO[] tempInfoArray = new DISPLAYCONFIG_PATH_INFO[pathInfoArray.Length];
                    pathInfoArray.CopyTo(tempInfoArray, 0);
                    pathInfoArray = new DISPLAYCONFIG_PATH_INFO[validCount];
                    int index = 0;
                    foreach (DISPLAYCONFIG_PATH_INFO pathInfo in tempInfoArray)
                    {
                        if (pathInfo.targetInfo.targetAvailable)
                        {
                            pathInfoArray[index] = pathInfo;
                            index++;
                        }
                    }
                }

                // Get display names
                for (int iMode = 0; iMode < modeInfoArray.Length; iMode++)
                {
                    if (modeInfoArray[iMode].infoType == DISPLAYCONFIG_MODE_INFO_TYPE.DISPLAYCONFIG_MODE_INFO_TYPE_TARGET)
                    {
                        try
                        {
                            additionalInfo[iMode] = GetMonitorAdditionalInfo(modeInfoArray[iMode].adapterId, modeInfoArray[iMode].id);
                        }
                        catch (Exception)
                        {
                            additionalInfo[iMode].valid = false;
                        }
                    }
                }
                return true;
            }
        }

        return false;
    }

    public static string PrintDisplaySettings(DISPLAYCONFIG_PATH_INFO[] pathInfoArray, DISPLAYCONFIG_MODE_INFO[] modeInfoArray)
    {
        string output = "";

        // Create an object to hold the display information for serialization
        var displaySettings = new
        {
            pathInfoArray = pathInfoArray.Select(pathInfo => new
            {
                sourceInfo = new
                {
                    adapterId = new
                    {
                        pathInfo.sourceInfo.adapterId.LowPart,
                        pathInfo.sourceInfo.adapterId.HighPart
                    },
                    pathInfo.sourceInfo.id,
                    pathInfo.sourceInfo.modeInfoIdx,
                    statusFlags = pathInfo.sourceInfo.statusFlags.ToString()
                },
                targetInfo = new
                {
                    adapterId = new
                    {
                        pathInfo.targetInfo.adapterId.LowPart,
                        pathInfo.targetInfo.adapterId.HighPart
                    },
                    pathInfo.targetInfo.id,
                    pathInfo.targetInfo.modeInfoIdx,
                    outputTechnology = pathInfo.targetInfo.outputTechnology.ToString(),
                    rotation = pathInfo.targetInfo.rotation.ToString(),
                    scaling = pathInfo.targetInfo.scaling.ToString(),
                    refreshRate = new
                    {
                        pathInfo.targetInfo.refreshRate.Numerator,
                        pathInfo.targetInfo.refreshRate.Denominator
                    },
                    scanLineOrdering = pathInfo.targetInfo.scanLineOrdering.ToString(),
                    pathInfo.targetInfo.targetAvailable,
                    statusFlags = pathInfo.targetInfo.statusFlags.ToString()
                },
                pathInfo.flags
            }).ToList(),
            modeInfoArray = modeInfoArray.Select(modeInfo => new
            {
                modeInfo.id,
                adapterId = new
                {
                    modeInfo.adapterId.LowPart,
                    modeInfo.adapterId.HighPart
                },
                infoType = modeInfo.infoType.ToString(),
                mode = modeInfo.infoType == DISPLAYCONFIG_MODE_INFO_TYPE.DISPLAYCONFIG_MODE_INFO_TYPE_TARGET ?
               (object)new
               {
                   targetVideoSignalInfo = new
                   {
                       modeInfo.targetMode.targetVideoSignalInfo.pixelRate,
                       hSyncFreq = new
                       {
                           modeInfo.targetMode.targetVideoSignalInfo.hSyncFreq.Numerator,
                           modeInfo.targetMode.targetVideoSignalInfo.hSyncFreq.Denominator
                       },
                       vSyncFreq = new
                       {
                           modeInfo.targetMode.targetVideoSignalInfo.vSyncFreq.Numerator,
                           modeInfo.targetMode.targetVideoSignalInfo.vSyncFreq.Denominator
                       },
                       activeSize = new
                       {
                           modeInfo.targetMode.targetVideoSignalInfo.activeSize.cx,
                           modeInfo.targetMode.targetVideoSignalInfo.activeSize.cy
                       },
                       totalSize = new
                       {
                           modeInfo.targetMode.targetVideoSignalInfo.totalSize.cx,
                           modeInfo.targetMode.targetVideoSignalInfo.totalSize.cy
                       },
                       videoStandard = modeInfo.targetMode.targetVideoSignalInfo.videoStandard.ToString(),
                       scanLineOrdering = modeInfo.targetMode.targetVideoSignalInfo.scanLineOrdering.ToString()
                   }
               } :
               (object)new
               {
                   sourceMode = new
                   {
                       modeInfo.sourceMode.width,
                       modeInfo.sourceMode.height,
                       pixelFormat = modeInfo.sourceMode.pixelFormat.ToString(),
                       position = new
                       {
                           modeInfo.sourceMode.position.x,
                           modeInfo.sourceMode.position.y
                       }
                   }
               }
            }).ToList()
        };

        using (MemoryStream memoryStream = new())
        {
            using (Utf8JsonWriter jsonWriter = new(memoryStream, new JsonWriterOptions { Indented = true }))
            {
                JsonSerializerOptions options = new() { WriteIndented = true };
                System.Text.Json.JsonSerializer.Serialize(jsonWriter, displaySettings, options);
            }
            output = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
        }

        return output;
    }

    public static bool LoadDisplaySettings(string fileName, out DISPLAYCONFIG_PATH_INFO[] pathInfoArray, out DISPLAYCONFIG_MODE_INFO[] modeInfoArray, out MonitorAdditionalInfo[] additionalInfo)
    {
        pathInfoArray = new DISPLAYCONFIG_PATH_INFO[0];
        modeInfoArray = new DISPLAYCONFIG_MODE_INFO[0];
        additionalInfo = new MonitorAdditionalInfo[0];

        try
        {
            string json = System.IO.File.ReadAllText(fileName);
            var displaySettings = JsonConvert.DeserializeObject<dynamic>(json);

            profileAudioDevice = displaySettings.defaultAudioDevice;

            List<DISPLAYCONFIG_PATH_INFO> pathInfoList = [];

            foreach (var pathInfo in displaySettings.pathInfoArray)
            {
                DISPLAYCONFIG_PATH_SOURCE_INFO sourceInfo = new()
                {
                    adapterId = new LUID
                    {
                        LowPart = (uint)pathInfo.sourceInfo.adapterId.LowPart,
                        HighPart = (int)pathInfo.sourceInfo.adapterId.HighPart
                    },
                    id = (uint)pathInfo.sourceInfo.id,
                    modeInfoIdx = (uint)pathInfo.sourceInfo.modeInfoIdx,
                    statusFlags = pathInfo.sourceInfo.statusFlags
                };

                DISPLAYCONFIG_PATH_TARGET_INFO targetInfo = new()
                {
                    adapterId = new LUID
                    {
                        LowPart = (uint)pathInfo.targetInfo.adapterId.LowPart,
                        HighPart = (int)pathInfo.targetInfo.adapterId.HighPart
                    },
                    id = (uint)pathInfo.targetInfo.id,
                    modeInfoIdx = (uint)pathInfo.targetInfo.modeInfoIdx,
                    outputTechnology = pathInfo.targetInfo.outputTechnology,
                    rotation = pathInfo.targetInfo.rotation,
                    scaling = pathInfo.targetInfo.scaling,
                    refreshRate = new DISPLAYCONFIG_RATIONAL
                    {
                        Numerator = (uint)pathInfo.targetInfo.refreshRate.numerator,
                        Denominator = (uint)pathInfo.targetInfo.refreshRate.denominator
                    },
                    scanLineOrdering = pathInfo.targetInfo.scanLineOrdering,
                    targetAvailable = (bool)pathInfo.targetInfo.targetAvailable,
                    statusFlags = pathInfo.targetInfo.statusFlags
                };

                pathInfoList.Add(new DISPLAYCONFIG_PATH_INFO
                {
                    sourceInfo = sourceInfo,
                    targetInfo = targetInfo,
                    flags = (uint)pathInfo.flags
                });
            }
            pathInfoArray = [.. pathInfoList];

            List<DISPLAYCONFIG_MODE_INFO> modeInfoList = [];
            foreach (var modeInfo in displaySettings.modeInfoArray)
            {
                LUID adapterId = new()
                {
                    LowPart = (uint)modeInfo.adapterId.LowPart,
                    HighPart = (int)modeInfo.adapterId.HighPart
                };

                DISPLAYCONFIG_MODE_INFO mode = new()
                {
                    id = (uint)modeInfo.id,
                    adapterId = adapterId,
                    infoType = modeInfo.infoType
                };

                if (mode.infoType == DISPLAYCONFIG_MODE_INFO_TYPE.DISPLAYCONFIG_MODE_INFO_TYPE_TARGET)
                {
                    mode.targetMode = new DISPLAYCONFIG_TARGET_MODE
                    {
                        targetVideoSignalInfo = new DISPLAYCONFIG_VIDEO_SIGNAL_INFO
                        {
                            pixelRate = (ulong)modeInfo.mode.targetVideoSignalInfo.pixelRate,
                            hSyncFreq = new DISPLAYCONFIG_RATIONAL
                            {
                                Numerator = (uint)modeInfo.mode.targetVideoSignalInfo.hSyncFreq.numerator,
                                Denominator = (uint)modeInfo.mode.targetVideoSignalInfo.hSyncFreq.denominator
                            },
                            vSyncFreq = new DISPLAYCONFIG_RATIONAL
                            {
                                Numerator = (uint)modeInfo.mode.targetVideoSignalInfo.vSyncFreq.numerator,
                                Denominator = (uint)modeInfo.mode.targetVideoSignalInfo.vSyncFreq.denominator
                            },
                            activeSize = new DISPLAYCONFIG_2DREGION
                            {
                                cx = (uint)modeInfo.mode.targetVideoSignalInfo.activeSize.cx,
                                cy = (uint)modeInfo.mode.targetVideoSignalInfo.activeSize.cy
                            },
                            totalSize = new DISPLAYCONFIG_2DREGION
                            {
                                cx = (uint)modeInfo.mode.targetVideoSignalInfo.totalSize.cx,
                                cy = (uint)modeInfo.mode.targetVideoSignalInfo.totalSize.cy
                            },
                            videoStandard = modeInfo.mode.targetVideoSignalInfo.videoStandard,
                            scanLineOrdering = modeInfo.mode.targetVideoSignalInfo.scanLineOrdering
                        }
                    };
                }
                else if (mode.infoType == DISPLAYCONFIG_MODE_INFO_TYPE.DISPLAYCONFIG_MODE_INFO_TYPE_SOURCE)
                {
                    mode.sourceMode = new DISPLAYCONFIG_SOURCE_MODE
                    {
                        width = (uint)modeInfo.mode.sourceMode.width,
                        height = (uint)modeInfo.mode.sourceMode.height,
                        pixelFormat = modeInfo.mode.sourceMode.pixelFormat,
                        position = new POINTL
                        {
                            x = modeInfo.mode.sourceMode.position.x,
                            y = modeInfo.mode.sourceMode.position.y
                        }
                    };
                }

                modeInfoList.Add(mode);
            }
            modeInfoArray = [.. modeInfoList];

            List<MonitorAdditionalInfo> additionalInfoList = [];
            foreach (var info in displaySettings.additionalInfo)
            {
                additionalInfoList.Add(new MonitorAdditionalInfo
                {
                    manufactureId = (ushort)info.manufactureId,
                    productCodeId = (ushort)info.productCodeId,
                    valid = (bool)info.valid,
                    monitorDevicePath = (string)info.monitorDevicePath,
                    monitorFriendlyDevice = (string)info.monitorFriendlyDevice
                });
            }
            additionalInfo = [.. additionalInfoList];

            return true;
        }
        catch (Exception)
        {
            RequestNotification("Error loading display profile");
            return false;
        }
    }

    public static bool SetDisplaySettings(string fileName)
    {
        if (!File.Exists(fileName))
        {
            RequestNotification("Display profile {" + fileName + "} does not exist");
            return false;
        }

        bool success = LoadDisplaySettings(fileName, out DISPLAYCONFIG_PATH_INFO[] pathInfoArray, out DISPLAYCONFIG_MODE_INFO[] modeInfoArray, out MonitorAdditionalInfo[] additionalInfo);
        if (!success)
        {
            return false;
        }

        DISPLAYCONFIG_PATH_INFO[] pathInfoArrayCurrent = [];
        DISPLAYCONFIG_MODE_INFO[] modeInfoArrayCurrent = [];
        MonitorAdditionalInfo[] additionalInfoCurrent = [];

        success = GetDisplaySettings(ref pathInfoArrayCurrent, ref modeInfoArrayCurrent, ref additionalInfoCurrent, false);
        bool idMatch = true;

        if (success)
        {
            if (idMatch)
            {
                for (int iPathInfo = 0; iPathInfo < pathInfoArray.Length; iPathInfo++)
                {
                    for (int iPathInfoCurrent = 0; iPathInfoCurrent < pathInfoArrayCurrent.Length; iPathInfoCurrent++)
                    {
                        if ((pathInfoArray[iPathInfo].sourceInfo.id == pathInfoArrayCurrent[iPathInfoCurrent].sourceInfo.id) &&
                                (pathInfoArray[iPathInfo].targetInfo.id == pathInfoArrayCurrent[iPathInfoCurrent].targetInfo.id))
                        {
                            pathInfoArray[iPathInfo].sourceInfo.adapterId.LowPart = pathInfoArrayCurrent[iPathInfoCurrent].sourceInfo.adapterId.LowPart;
                            pathInfoArray[iPathInfo].targetInfo.adapterId.LowPart = pathInfoArrayCurrent[iPathInfoCurrent].targetInfo.adapterId.LowPart;
                            break;
                        }
                    }
                }

                for (int iModeInfo = 0; iModeInfo < modeInfoArray.Length; iModeInfo++)
                {
                    for (int iPathInfo = 0; iPathInfo < pathInfoArray.Length; iPathInfo++)
                    {
                        if ((modeInfoArray[iModeInfo].id == pathInfoArray[iPathInfo].targetInfo.id) &&
                                (modeInfoArray[iModeInfo].infoType == DISPLAYCONFIG_MODE_INFO_TYPE.DISPLAYCONFIG_MODE_INFO_TYPE_TARGET))
                        {
                            for (int iModeInfoSource = 0; iModeInfoSource < modeInfoArray.Length; iModeInfoSource++)
                            {
                                if ((modeInfoArray[iModeInfoSource].id == pathInfoArray[iPathInfo].sourceInfo.id) &&
                                    (modeInfoArray[iModeInfoSource].adapterId.LowPart == modeInfoArray[iModeInfo].adapterId.LowPart) &&
                                    (modeInfoArray[iModeInfoSource].infoType == DISPLAYCONFIG_MODE_INFO_TYPE.DISPLAYCONFIG_MODE_INFO_TYPE_SOURCE))
                                {
                                    modeInfoArray[iModeInfoSource].adapterId.LowPart = pathInfoArray[iPathInfo].sourceInfo.adapterId.LowPart;
                                    break;
                                }
                            }
                            modeInfoArray[iModeInfo].adapterId.LowPart = pathInfoArray[iPathInfo].targetInfo.adapterId.LowPart;
                            break;
                        }
                    }
                }
            }

            uint numPathArrayElements = (uint)pathInfoArray.Length;
            uint numModeInfoArrayElements = (uint)modeInfoArray.Length;
            long status = SetDisplayConfig(numPathArrayElements, pathInfoArray, numModeInfoArrayElements, modeInfoArray,
                                            SetDisplayConfigFlags.SetDisplayConfigFlags_APPLY | SetDisplayConfigFlags.SetDisplayConfigFlags_USESUPPLIEDDISPLAYCONFIG | SetDisplayConfigFlags.SetDisplayConfigFlags_SAVETODATABASE | SetDisplayConfigFlags.SetDisplayConfigFlags_NOOPTIMIZATION | SetDisplayConfigFlags.SetDisplayConfigFlags_ALLOWCHANGES);
            if (status != 0)
            {
                if ((additionalInfoCurrent.Length > 0) && (additionalInfo.Length > 0))
                {

                    for (int iModeInfo = 0; iModeInfo < modeInfoArray.Length; iModeInfo++)
                    {
                        for (int iAdditionalInfoCurrent = 0; iAdditionalInfoCurrent < additionalInfoCurrent.Length; iAdditionalInfoCurrent++)
                        {
                            if ((additionalInfoCurrent[iAdditionalInfoCurrent].monitorFriendlyDevice != null) && (additionalInfo[iModeInfo].monitorFriendlyDevice != null))
                            {
                                if (additionalInfoCurrent[iAdditionalInfoCurrent].monitorFriendlyDevice.Equals(additionalInfo[iModeInfo].monitorFriendlyDevice))
                                {
                                    LUID originalID = modeInfoArray[iModeInfo].adapterId;
                                    for (int iPathInfo = 0; iPathInfo < pathInfoArray.Length; iPathInfo++)
                                    {
                                        if ((pathInfoArray[iPathInfo].targetInfo.adapterId.LowPart == originalID.LowPart) &&
                                           (pathInfoArray[iPathInfo].targetInfo.adapterId.HighPart == originalID.HighPart))
                                        {
                                            pathInfoArray[iPathInfo].targetInfo.adapterId = modeInfoArrayCurrent[iAdditionalInfoCurrent].adapterId;
                                            pathInfoArray[iPathInfo].sourceInfo.adapterId = modeInfoArrayCurrent[iAdditionalInfoCurrent].adapterId;
                                            pathInfoArray[iPathInfo].targetInfo.id = modeInfoArrayCurrent[iAdditionalInfoCurrent].id;
                                        }
                                    }
                                    for (int iModeInfoFix = 0; iModeInfoFix < modeInfoArray.Length; iModeInfoFix++)
                                    {
                                        if ((modeInfoArray[iModeInfoFix].adapterId.LowPart == originalID.LowPart) &&
                                            (modeInfoArray[iModeInfoFix].adapterId.HighPart == originalID.HighPart))
                                        {
                                            modeInfoArray[iModeInfoFix].adapterId = modeInfoArrayCurrent[iAdditionalInfoCurrent].adapterId;
                                        }
                                    }
                                    modeInfoArray[iModeInfo].adapterId = modeInfoArrayCurrent[iAdditionalInfoCurrent].adapterId;
                                    modeInfoArray[iModeInfo].id = modeInfoArrayCurrent[iAdditionalInfoCurrent].id;

                                    break;
                                }
                            }
                        }
                    }

                    status = SetDisplayConfig(numPathArrayElements, pathInfoArray, numModeInfoArrayElements, modeInfoArray,
                                            SetDisplayConfigFlags.SetDisplayConfigFlags_APPLY | SetDisplayConfigFlags.SetDisplayConfigFlags_USESUPPLIEDDISPLAYCONFIG | SetDisplayConfigFlags.SetDisplayConfigFlags_SAVETODATABASE | SetDisplayConfigFlags.SetDisplayConfigFlags_NOOPTIMIZATION | SetDisplayConfigFlags.SetDisplayConfigFlags_ALLOWCHANGES);
                    if (status != 0)
                    {
                        RequestNotification("Failed to apply display profile");
                        return false;
                    }

                    setAudioDevice(profileAudioDevice);

                    return true;
                }
                return false;
            }

            setAudioDevice(profileAudioDevice);

            return true;
        }

        return false;
    }

    public static bool SaveDisplaySettings(string fileName)
    {
        DISPLAYCONFIG_PATH_INFO[] pathInfoArray = new DISPLAYCONFIG_PATH_INFO[0];
        DISPLAYCONFIG_MODE_INFO[] modeInfoArray = new DISPLAYCONFIG_MODE_INFO[0];
        MonitorAdditionalInfo[] additionalInfo = new MonitorAdditionalInfo[0];

        bool status = GetDisplaySettings(ref pathInfoArray, ref modeInfoArray, ref additionalInfo, true);
        if (status)
        {
            var displaySettings = new
            {
                pathInfoArray = pathInfoArray.Select(pathInfo => new
                {
                    sourceInfo = new
                    {
                        adapterId = new
                        {
                            LowPart = pathInfo.sourceInfo.adapterId.LowPart,
                            HighPart = pathInfo.sourceInfo.adapterId.HighPart
                        },
                        id = pathInfo.sourceInfo.id,
                        modeInfoIdx = pathInfo.sourceInfo.modeInfoIdx,
                        statusFlags = pathInfo.sourceInfo.statusFlags.ToString()
                    },
                    targetInfo = new
                    {
                        adapterId = new
                        {
                            LowPart = pathInfo.targetInfo.adapterId.LowPart,
                            HighPart = pathInfo.targetInfo.adapterId.HighPart
                        },
                        id = pathInfo.targetInfo.id,
                        modeInfoIdx = pathInfo.targetInfo.modeInfoIdx,
                        outputTechnology = pathInfo.targetInfo.outputTechnology.ToString(),
                        rotation = pathInfo.targetInfo.rotation.ToString(),
                        scaling = pathInfo.targetInfo.scaling.ToString(),
                        refreshRate = new
                        {
                            numerator = pathInfo.targetInfo.refreshRate.Numerator,
                            denominator = pathInfo.targetInfo.refreshRate.Denominator
                        },
                        scanLineOrdering = pathInfo.targetInfo.scanLineOrdering.ToString(),
                        targetAvailable = pathInfo.targetInfo.targetAvailable,
                        statusFlags = pathInfo.targetInfo.statusFlags.ToString()
                    },
                    flags = pathInfo.flags
                }).ToList(),
                modeInfoArray = modeInfoArray.Select(modeInfo => new
                {
                    id = modeInfo.id,
                    adapterId = new
                    {
                        LowPart = modeInfo.adapterId.LowPart,
                        HighPart = modeInfo.adapterId.HighPart
                    },
                    infoType = modeInfo.infoType.ToString(),
                    mode = modeInfo.infoType == DISPLAYCONFIG_MODE_INFO_TYPE.DISPLAYCONFIG_MODE_INFO_TYPE_TARGET ?
                           (object)new
                           {
                               targetVideoSignalInfo = new
                               {
                                   pixelRate = modeInfo.targetMode.targetVideoSignalInfo.pixelRate,
                                   hSyncFreq = new
                                   {
                                       numerator = modeInfo.targetMode.targetVideoSignalInfo.hSyncFreq.Numerator,
                                       denominator = modeInfo.targetMode.targetVideoSignalInfo.hSyncFreq.Denominator
                                   },
                                   vSyncFreq = new
                                   {
                                       numerator = modeInfo.targetMode.targetVideoSignalInfo.vSyncFreq.Numerator,
                                       denominator = modeInfo.targetMode.targetVideoSignalInfo.vSyncFreq.Denominator
                                   },
                                   activeSize = new
                                   {
                                       cx = modeInfo.targetMode.targetVideoSignalInfo.activeSize.cx,
                                       cy = modeInfo.targetMode.targetVideoSignalInfo.activeSize.cy
                                   },
                                   totalSize = new
                                   {
                                       cx = modeInfo.targetMode.targetVideoSignalInfo.totalSize.cx,
                                       cy = modeInfo.targetMode.targetVideoSignalInfo.totalSize.cy
                                   },
                                   videoStandard = modeInfo.targetMode.targetVideoSignalInfo.videoStandard.ToString(),
                                   scanLineOrdering = modeInfo.targetMode.targetVideoSignalInfo.scanLineOrdering.ToString()
                               }
                           } :
                           (object)new
                           {
                               sourceMode = new
                               {
                                   width = modeInfo.sourceMode.width,
                                   height = modeInfo.sourceMode.height,
                                   pixelFormat = modeInfo.sourceMode.pixelFormat.ToString(),
                                   position = new
                                   {
                                       x = modeInfo.sourceMode.position.x,
                                       y = modeInfo.sourceMode.position.y
                                   }
                               }
                           }
                }).ToList(),
                additionalInfo = additionalInfo,
                defaultAudioDevice = audioManager.GetDefaultAudioDevice().FullName
            };

            fileName = Path.Combine(GetDisplayProfilesDirectory(), fileName);

            JsonSerializerOptions options = new() { WriteIndented = true };
            string jsonString = System.Text.Json.JsonSerializer.Serialize(displaySettings, options);
            File.WriteAllText(fileName, jsonString);

            return true;
        }
        else
        {
            RequestNotification("Failed to save display settings");
        }

        return false;
    }

    public static bool DeleteDisplaySettings(string fileName)
    {
        fileName = Path.Combine(GetDisplayProfilesDirectory(), fileName);

        if (!File.Exists(fileName))
        {
            return false;
        }

        File.Delete(fileName);
        return true;
    }

    public static bool RenameDisplayProfile(string fileName, string newFileName)
    {
        if (!File.Exists(fileName))
        {
            return false;
        }

        System.IO.File.Move(Path.Combine(GetDisplayProfilesDirectory(), fileName), Path.Combine(GetDisplayProfilesDirectory(), newFileName + ".json"));
        return true;
    }

    [DllImport("user32.dll")]
    private static extern int PostMessage(int hWnd, int hMsg, int wParam, int lParam);

/*    public static void DisplayToggleSleep(bool sleep = true)
    {
        const int WM_SYSCOMMAND = 0x0112;
        const int SC_MONITORPOWER = 0xF170;
        const int HWND_BROADCAST = 0xFFFF;
        if (sleep)
        {
            // Go into sleep mode (2)
            _ = PostMessage(HWND_BROADCAST, WM_SYSCOMMAND, SC_MONITORPOWER, 2);
        }
        else
        {
            // Wakeup from sleep mode (-1)
            _ = Task.Run(() =>
            {
                _ = PostMessage(HWND_BROADCAST, WM_SYSCOMMAND, SC_MONITORPOWER, -1);
                Thread.Sleep(100);
            });
        }
    }*/

    static void Main()
    {

    }
}