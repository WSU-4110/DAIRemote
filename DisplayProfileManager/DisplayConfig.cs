using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DisplayProfileManager
{
    public class DisplayConfig
    {
        public const int ERROR_CONST = 0;
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
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int DisplayConfigGetDeviceInfo(ref DISPLAYCONFIG_TARGET_DEVICE_NAME deviceName);

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
        public struct DISPLAYCONFIG_SOURCE_DEVICE_NAME : DISPLAYCONFIG_INFO_CONTRACT
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
        public struct DISPLAYCONFIG_TARGET_DEVICE_NAME_FLAGS
        {
            private uint value;
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

        //POINTL used in DISPLAYCONFIG_DESKTOP_IMAGE_INFO
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
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int SetDisplayConfig(
            uint numPathArrayElements,
            IntPtr pathArray, // IntPtr for optional pointers
            uint numModeInfoArrayElements,
            IntPtr modeInfoArray,
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
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int QueryDisplayConfig(
            QueryDisplayConfigFlags flags,
            ref uint numPathArrayElements,
            [Out] DISPLAYCONFIG_PATH_INFO[] pathInfoArray,
            ref uint numModeInfoArrayElements,
            [Out] DISPLAYCONFIG_MODE_INFO[] modeInfoArray,
            IntPtr z
        );

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetDisplayConfigBufferSizes(QueryDisplayConfigFlags flags, out uint numPathArrayElements, out uint numModeInfoArrayElements);

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

        public interface DISPLAYCONFIG_INFO_CONTRACT
        {
        }

        public static string MonitorFriendlyName(LUID adapterId, uint targetId)
        {
            var displayName = new DISPLAYCONFIG_TARGET_DEVICE_NAME
            {
                header =
                {
                    size = (uint)Marshal.SizeOf(typeof (DISPLAYCONFIG_TARGET_DEVICE_NAME)),
                    adapterId = adapterId,
                    id = targetId,
                    type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME
                }
            };
            var error = DisplayConfigGetDeviceInfo(ref displayName);
            if (error != ERROR_CONST)
                throw new Win32Exception(error);
            return displayName.monitorFriendlyDeviceName;
        }

        [StructLayout(LayoutKind.Sequential)]
        [Serializable]
        public struct MonitorAdditionalInfo
        {
            public ushort manufactureId { get; set; }
            public ushort productCodeId { get; set; }
            public bool valid { get; set; }

            // Base64 encoded version of monitorDevicePath for JSON serialization
            [JsonPropertyName("monitorDevicePath")]
            public string monitorDevicePath64
            {
                get
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
                get
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
            MonitorAdditionalInfo additionalInfo = new MonitorAdditionalInfo();
            var monitorName = new DISPLAYCONFIG_TARGET_DEVICE_NAME
            {
                header =
                {
                    size = (uint)Marshal.SizeOf(typeof (DISPLAYCONFIG_TARGET_DEVICE_NAME)),
                    adapterId = adapterId,
                    id = targetId,
                    type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME
                }
            };
            var error = DisplayConfigGetDeviceInfo(ref monitorName);
            if (error != ERROR_CONST)
                throw new Win32Exception(error);

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
            Func<IntPtr, StatusCode> func) where T : DISPLAYCONFIG_INFO_CONTRACT
        {
            var ptr = Marshal.AllocHGlobal(Marshal.SizeOf(displayConfig));
            Marshal.StructureToPtr(displayConfig, ptr, false);

            var returnValue = func(ptr);

            displayConfig = (T)Marshal.PtrToStructure(ptr, displayConfig.GetType());

            Marshal.FreeHGlobal(ptr);
            return returnValue;
        }



        private static bool debug = true;
        public static void debugMsg(System.String text)
        {
            if (debug)
            {
                Debug.WriteLine(text);
            }
        }

        public static bool GetDisplaySettings(ref DISPLAYCONFIG_PATH_INFO[] pathInfoArray, ref DISPLAYCONFIG_MODE_INFO[] modeInfoArray, ref MonitorAdditionalInfo[] additionalInfo, bool ActiveOnly)
        {
            uint numPathArrayElements;
            uint numModeInfoArrayElements;

            // active paths from the computer.
            debugMsg("Getting display settings");
            QueryDisplayConfigFlags queryFlags = QueryDisplayConfigFlags.QueryDisplayConfigFlags_ALLPATHS;
            if (ActiveOnly)
            {
                queryFlags = QueryDisplayConfigFlags.QueryDisplayConfigFlags_ONLYACTIVEPATHS;
            }

            debugMsg("Getting buffer size");
            var status = GetDisplayConfigBufferSizes(queryFlags, out numPathArrayElements, out numModeInfoArrayElements);
            if (status == 0)
            {
                pathInfoArray = new DISPLAYCONFIG_PATH_INFO[numPathArrayElements];
                modeInfoArray = new DISPLAYCONFIG_MODE_INFO[numModeInfoArrayElements];
                additionalInfo = new MonitorAdditionalInfo[numModeInfoArrayElements];

                debugMsg("Querying display config");
                status = QueryDisplayConfig(queryFlags,
                                                       ref numPathArrayElements, pathInfoArray, ref numModeInfoArrayElements,
                                                       modeInfoArray, IntPtr.Zero);

                if (status == 0)
                {
                    // cleanup of modeInfo bad elements 
                    int validCount = 0;
                    foreach (DISPLAYCONFIG_MODE_INFO modeInfo in modeInfoArray)
                    {
                        if (modeInfo.infoType != DISPLAYCONFIG_MODE_INFO_TYPE.DISPLAYCONFIG_MODE_INFO_TYPE_ZERO)
                        {   // count number of valid mode Infos
                            validCount++;
                        }
                    }
                    if (validCount > 0)
                    {   // only cleanup if there is at least one valid element found
                        DISPLAYCONFIG_MODE_INFO[] tempInfoArray = new DISPLAYCONFIG_MODE_INFO[modeInfoArray.Count()];
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

                    // cleanup of currently not available pathInfo elements
                    validCount = 0;
                    foreach (DISPLAYCONFIG_PATH_INFO pathInfo in pathInfoArray)
                    {
                        if (pathInfo.targetInfo.targetAvailable)
                        {
                            validCount++;
                        }
                    }
                    if (validCount > 0)
                    {   // only cleanup if there is at least one valid element found
                        DISPLAYCONFIG_PATH_INFO[] tempInfoArray = new DISPLAYCONFIG_PATH_INFO[pathInfoArray.Count()];
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

                    // get the display names for all modes
                    for (var iMode = 0; iMode < modeInfoArray.Count(); iMode++)
                    {
                        if (modeInfoArray[iMode].infoType == DISPLAYCONFIG_MODE_INFO_TYPE.DISPLAYCONFIG_MODE_INFO_TYPE_TARGET)
                        {
                            try
                            {
                                additionalInfo[iMode] = GetMonitorAdditionalInfo(modeInfoArray[iMode].adapterId, modeInfoArray[iMode].id);
                            }
                            catch (Exception e)
                            {
                                additionalInfo[iMode].valid = false;
                            }
                        }
                    }
                    return true;
                }
                else
                {
                    debugMsg("Querying display config failed");
                }
            }
            else
            {
                debugMsg("Getting Buffer Size Failed");
            }

            return false;
        }

        public static string PrintDisplaySettings(DISPLAYCONFIG_PATH_INFO[] pathInfoArray, DISPLAYCONFIG_MODE_INFO[] modeInfoArray)
        {
            // Initialize result
            string output = "";

            // Create an object to hold the data for serialization
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
                }).ToList()
            };

            // Serialize to JSON using MemoryStream and Utf8JsonWriter
            using (var memoryStream = new MemoryStream())
            {
                using (var jsonWriter = new Utf8JsonWriter(memoryStream, new JsonWriterOptions { Indented = true }))
                {
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    System.Text.Json.JsonSerializer.Serialize(jsonWriter, displaySettings, options);
                }
                output = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
            }

            return output;
        }

        public static bool SaveDisplaySettings(string fileName)
        {
            DISPLAYCONFIG_PATH_INFO[] pathInfoArray = new DISPLAYCONFIG_PATH_INFO[0];
            DISPLAYCONFIG_MODE_INFO[] modeInfoArray = new DISPLAYCONFIG_MODE_INFO[0];
            MonitorAdditionalInfo[] additionalInfo = new MonitorAdditionalInfo[0];

            debugMsg("Getting display config");
            bool status = GetDisplaySettings(ref pathInfoArray, ref modeInfoArray, ref additionalInfo, true);
            if (status)
            {
                if (debug)
                {
                    // debug output complete display settings
                    debugMsg("Display settings to write:");
                    debugMsg(PrintDisplaySettings(pathInfoArray, modeInfoArray));
                }

                // Initialize result
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
                    additionalInfo = additionalInfo
                };

                // Serialize to JSON and write to file
                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = System.Text.Json.JsonSerializer.Serialize(displaySettings, options);

                // Write JSON string to file
                File.WriteAllText(fileName, jsonString);

                return true;
            }
            else
            {
                debugMsg("Failed to get display settings, ERROR: " + status.ToString());
            }

            return false;
        }
        static void Main(string[] args)
        {

        }
    }
}
