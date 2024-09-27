using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Newtonsoft.Json;
using static System.Net.Mime.MediaTypeNames;

namespace DisplayProfileManager
{
    public class DisplayConfig
    {

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
            DISPLAYCONFIG_MODE_INFO_TYPE_SOURCE = 0,
            DISPLAYCONFIG_MODE_INFO_TYPE_TARGET = 1,
            DISPLAYCONFIG_MODE_INFO_TYPE_DESKTOP_IMAGE = 2,
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

            // Union representation
            [FieldOffset(16)]
            public DISPLAYCONFIG_TARGET_MODE targetMode;

            [FieldOffset(16)]
            public DISPLAYCONFIG_SOURCE_MODE sourceMode;

            [FieldOffset(16)]
            public DISPLAYCONFIG_DESKTOP_IMAGE_INFO desktopImageInfo;
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
            DISPLAYCONFIG_PIXELFORMAT_UNKNOWN = 0,
            DISPLAYCONFIG_PIXELFORMAT_X8R8G8B8 = 1,
            DISPLAYCONFIG_PIXELFORMAT_R8G8B8A8 = 2,
            // Add other pixel formats as needed
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

            // Define the union and its structures
            [StructLayout(LayoutKind.Explicit)]
            public struct SignalInfoUnion
            {
                [FieldOffset(0)]
                public uint videoStandard;

                [FieldOffset(0)]
                public AdditionalSignalInfoStruct AdditionalSignalInfo;

                [StructLayout(LayoutKind.Sequential)]
                public struct AdditionalSignalInfoStruct
                {
                    public ushort videoStandard;
                    public ushort vSyncFreqDivider;
                    public ushort reserved;
                }
            }

            public SignalInfoUnion DUMMYUNIONNAME;
            public DISPLAYCONFIG_SCANLINE_ORDERING scanLineOrdering;
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
        public static extern int DisplayConfigGetDeviceInfo(ref DISPLAYCONFIG_DEVICE_INFO_HEADER requestPacket);

        /*
        The DisplayConfigSetDeviceInfo function sets the properties of a target.
        https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-displayconfigsetdeviceinfo
        */
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int DisplayConfigSetDeviceInfo(ref DISPLAYCONFIG_DEVICE_INFO_HEADER setPacket);

        /*
        The DISPLAYCONFIG_DEVICE_INFO_HEADER structure contains display information about the device.
        https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-displayconfig_device_info_header
        */
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct DISPLAYCONFIG_DEVICE_INFO_HEADER
        {
            public DISPLAYCONFIG_DEVICE_INFO_TYPE type;
            public uint size;
            public long adapterId;  // Equivalent to LUID in C++
            public uint id;
        }

        /*
        The DISPLAYCONFIG_SOURCE_DEVICE_NAME structure contains the GDI device name for the source or view.
        https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-displayconfig_source_device_name
        */
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct DISPLAYCONFIG_SOURCE_DEVICE_NAME
        {
            public DisplayConfig.DISPLAYCONFIG_DEVICE_INFO_HEADER header;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] // CCHDEVICENAME is usually 32
            public string viewGdiDeviceName;
        }

        /*
        The DISPLAYCONFIG_TARGET_DEVICE_NAME structure contains information about the target.
        https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-displayconfig_target_device_name
        */
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct DISPLAYCONFIG_TARGET_DEVICE_NAME
        {
            public DisplayConfig.DISPLAYCONFIG_DEVICE_INFO_HEADER header; // Nested structure
            public DISPLAYCONFIG_TARGET_DEVICE_NAME_FLAGS flags; // Define this struct or enum separately
            public DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY outputTechnology; // Enum for video technology
            public ushort edidManufactureId; // UINT16 maps to ushort in C#
            public ushort edidProductCodeId; // UINT16 maps to ushort in C#
            public uint connectorInstance; // UINT32 maps to uint in C#

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)] // WCHAR[64] becomes a string in C#
            public string monitorFriendlyDeviceName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] // WCHAR[128] becomes a string in C#
            public string monitorDevicePath;
        }

        /*
        The DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY enumeration specifies the target's connector type.
        https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ne-wingdi-displayconfig_video_output_technology
        */
        public enum DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY : uint
        {
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_OTHER = 0xFFFFFFFF,
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
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DISPLAYPORT_USB_TUNNEL = 18, // Add missing value
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

            private const uint FRIENDLY_NAME_FROM_EDID_MASK = 0x00000001;
            private const uint FRIENDLY_NAME_FORCED_MASK = 0x00000002;
            private const uint EDID_IDS_VALID_MASK = 0x00000004;

            public bool FriendlyNameFromEdid
            {
                get => (value & FRIENDLY_NAME_FROM_EDID_MASK) != 0;
                set
                {
                    if (value)
                        this.value |= FRIENDLY_NAME_FROM_EDID_MASK;
                    else
                        this.value &= ~FRIENDLY_NAME_FROM_EDID_MASK;
                }
            }

            public bool FriendlyNameForced
            {
                get => (value & FRIENDLY_NAME_FORCED_MASK) != 0;
                set
                {
                    if (value)
                        this.value |= FRIENDLY_NAME_FORCED_MASK;
                    else
                        this.value &= ~FRIENDLY_NAME_FORCED_MASK;
                }
            }

            public bool EdidIdsValid
            {
                get => (value & EDID_IDS_VALID_MASK) != 0;
                set
                {
                    if (value)
                        this.value |= EDID_IDS_VALID_MASK;
                    else
                        this.value &= ~EDID_IDS_VALID_MASK;
                }
            }

            public uint Value
            {
                get => value;
                set => this.value = value;
            }
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
        The DISPLAYCONFIG_SET_TARGET_PERSISTENCE structure contains information about setting the display.
        https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-displayconfig_set_target_persistence
        */
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct DISPLAYCONFIG_SET_TARGET_PERSISTENCE
        {
            public DISPLAYCONFIG_DEVICE_INFO_HEADER header;

            private uint flags; // This will hold the bit fields

            // Property to access the bit fields
            public bool BootPersistenceOn
            {
                get => (flags & 0x00000001U) != 0; // Use 'U' to specify an unsigned constant
                set
                {
                    if (value)
                    {
                        flags |= 0x00000001U; // Use 'U' to specify an unsigned constant
                    }
                    else
                    {
                        flags &= ~0x00000001U; // Use 'U' to specify an unsigned constant
                    }
                }
            }

            public uint Flags
            {
                get => flags;
                set => flags = value;
            }
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

        /*
        The DISPLAYCONFIG_SUPPORT_VIRTUAL_RESOLUTION structure contains information on the state of virtual resolution support for the monitor.
        https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-displayconfig_support_virtual_resolution
        */
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct DISPLAYCONFIG_SUPPORT_VIRTUAL_RESOLUTION
        {
            public DISPLAYCONFIG_DEVICE_INFO_HEADER header;
            private uint _value; // Use a private field to hold the bit field value

            // Property to get or set the disableMonitorVirtualResolution bit
            public bool DisableMonitorVirtualResolution
            {
                get => (_value & 0x1) != 0;
                set
                {
                    if (value)
                        _value |= 0x1;  // Set the bit to 1
                    else
                        _value = unchecked(_value & ~0x1U); // Clear the bit to 0, with U for uint
                    }
                }

            // Property to get or set the reserved bits
            public uint Reserved
            {
                get => (_value >> 1) & 0x7FFFFFFF; // Shift right and mask to get the reserved bits
                set => _value = (_value & 0x1) | ((value & 0x7FFFFFFF) << 1); // Set the reserved bits
            }

            // Optionally provide a way to access the raw value if needed
            public uint RawValue
            {
                get => _value;
                set => _value = value;
            }
        }

        /*
        The DISPLAYCONFIG_SDR_WHITE_LEVEL structure contains information about a display's current SDR white level. This is the brightness level that SDR "white" is rendered at within an HDR monitor.
        https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-displayconfig_sdr_white_level
        */
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct DISPLAYCONFIG_SDR_WHITE_LEVEL
        {
            public DISPLAYCONFIG_DEVICE_INFO_HEADER header;
            public uint SDRWhiteLevel; // Use uint for ULONG
        }

        //POINTL used in DISPLAYCONFIG_DESKTOP_IMAGE_INFO
        [StructLayout(LayoutKind.Sequential)]
        public struct POINTL
        {
            public int x;
            public int y;
        }

        //RECTL used in DISPLAYCONFIG_DESKTOP_IMAGE_INFO
        [StructLayout(LayoutKind.Sequential)]
        public struct RECTL
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        /*
        The DISPLAYCONFIG_DESKTOP_IMAGE_INFO structure contains information about the image displayed on the desktop.
        https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-displayconfig_desktop_image_info
        */
        [StructLayout(LayoutKind.Sequential)]
        public struct DISPLAYCONFIG_DESKTOP_IMAGE_INFO
        {
            public POINTL PathSourceSize;
            public RECTL DesktopImageRegion;
            public RECTL DesktopImageClip;
        }

        /*
        The DISPLAYCONFIG_SCALING enumeration specifies the scaling transformation applied to content displayed on a video present network (VidPN) present path.
        https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ne-wingdi-displayconfig_scaling
        */
        public enum DISPLAYCONFIG_SCALING : uint
        {
            DISPLAYCONFIG_SCALING_IDENTITY = 1,
            DISPLAYCONFIG_SCALING_CENTERED = 2,
            DISPLAYCONFIG_SCALING_STRETCHED = 3,
            DISPLAYCONFIG_SCALING_ASPECTRATIOCENTEREDMAX = 4,
            DISPLAYCONFIG_SCALING_CUSTOM = 5,
            DISPLAYCONFIG_SCALING_PREFERRED = 128,
            DISPLAYCONFIG_SCALING_FORCE_UINT32 = 0xFFFFFFFF
        }

        /*
        The SetDisplayConfig function modifies the display topology, source, and target modes by exclusively enabling the specified paths in the current session.
        https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setdisplayconfig
        */
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int SetDisplayConfig(
            uint numPathArrayElements,
            IntPtr pathArray, // Use IntPtr for optional pointers
            uint numModeInfoArrayElements,
            IntPtr modeInfoArray, // Use IntPtr for optional pointers
            uint flags
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
            public uint flags; // UINT32 in C++
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

        [StructLayout(LayoutKind.Sequential)]
        public struct DISPLAYCONFIG_PATH_SOURCE_INFO
        {
            public LUID adapterId;
            public uint id;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            private byte[] unionData;

            // Represents the union fields
            public uint ModeInfoIdx
            {
                get => BitConverter.ToUInt32(unionData, 0);
                set
                {
                    unionData = BitConverter.GetBytes(value);
                }
            }

            public ushort CloneGroupId
            {
                get => (ushort)(BitConverter.ToUInt32(unionData, 0) & 0xFFFF);
                set
                {
                    uint temp = BitConverter.ToUInt32(unionData, 0);
                    temp &= 0xFFFF0000; // Clear the lower 16 bits
                    temp |= (uint)value;
                    unionData = BitConverter.GetBytes(temp);
                }
            }

            public ushort SourceModeInfoIdx
            {
                get => (ushort)((BitConverter.ToUInt32(unionData, 0) >> 16) & 0xFFFF);
                set
                {
                    uint temp = BitConverter.ToUInt32(unionData, 0);
                    temp &= 0xFFFF; // Clear the upper 16 bits
                    temp |= (uint)(value << 16);
                    unionData = BitConverter.GetBytes(temp);
                }
            }

            public uint StatusFlags;

            public DISPLAYCONFIG_PATH_SOURCE_INFO(uint modeInfoIdx = 0)
            {
                adapterId = new LUID();
                id = 0;
                unionData = new byte[8];
                ModeInfoIdx = modeInfoIdx;
                StatusFlags = 0;
            }
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

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            private byte[] unionData;

            // Represents the union fields
            public uint ModeInfoIdx
            {
                get => BitConverter.ToUInt32(unionData, 0);
                set
                {
                    unionData = BitConverter.GetBytes(value);
                }
            }

            public ushort DesktopModeInfoIdx
            {
                get => (ushort)(BitConverter.ToUInt32(unionData, 0) & 0xFFFF);
                set
                {
                    uint temp = BitConverter.ToUInt32(unionData, 0);
                    temp &= 0xFFFF0000; // Clear the lower 16 bits
                    temp |= (uint)value;
                    unionData = BitConverter.GetBytes(temp);
                }
            }

            public ushort TargetModeInfoIdx
            {
                get => (ushort)((BitConverter.ToUInt32(unionData, 0) >> 16) & 0xFFFF);
                set
                {
                    uint temp = BitConverter.ToUInt32(unionData, 0);
                    temp &= 0xFFFF; // Clear the upper 16 bits
                    temp |= (uint)(value << 16);
                    unionData = BitConverter.GetBytes(temp);
                }
            }

            public DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY outputTechnology;
            public DISPLAYCONFIG_ROTATION rotation;
            public DISPLAYCONFIG_SCALING scaling;
            public DISPLAYCONFIG_RATIONAL refreshRate;
            public DISPLAYCONFIG_SCANLINE_ORDERING scanLineOrdering;
            public bool targetAvailable; // BOOL in C++
            public uint statusFlags;
        }

        /*
        The DISPLAYCONFIG_ROTATION enumeration specifies the clockwise rotation of the display.
        https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ne-wingdi-displayconfig_rotation
        */
        [Flags]
        public enum DISPLAYCONFIG_ROTATION : uint
        {
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
            DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED_UPPERFIELDFIRST = 3,
            DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED_LOWERFIELDFIRST = 4,
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

        /*
        The QueryDisplayConfig function retrieves information about all possible display paths for all display devices, or views, in the current setting.
        https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-querydisplayconfig
        */
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int QueryDisplayConfig(
            uint flags,
            ref uint numPathArrayElements,
            [Out] DISPLAYCONFIG_PATH_INFO[] pathArray,
            ref uint numModeInfoArrayElements,
            [Out] DISPLAYCONFIG_MODE_INFO[] modeInfoArray,
            [Out] out DISPLAYCONFIG_TOPOLOGY_ID currentTopologyId
        );

        /*
        The DISPLAYCONFIG_TOPOLOGY_ID enumeration specifies the type of display topology.
        https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ne-wingdi-displayconfig_topology_id
        */
        [Flags]
        public enum DISPLAYCONFIG_TOPOLOGY_ID : uint
        {
            DISPLAYCONFIG_TOPOLOGY_INTERNAL = 0x00000001,
            DISPLAYCONFIG_TOPOLOGY_CLONE = 0x00000002,
            DISPLAYCONFIG_TOPOLOGY_EXTEND = 0x00000004,
            DISPLAYCONFIG_TOPOLOGY_EXTERNAL = 0x00000008,
            DISPLAYCONFIG_TOPOLOGY_FORCE_UINT32 = 0xFFFFFFFF
        }

        public static (DISPLAYCONFIG_PATH_INFO[], DISPLAYCONFIG_MODE_INFO[], DISPLAYCONFIG_TOPOLOGY_ID) GetDisplayConfig()
        {
            uint numPathArrayElements = 0;
            uint numModeInfoArrayElements = 0;
            DISPLAYCONFIG_TOPOLOGY_ID currentTopologyId;

            // First call to get the required buffer sizes
            QueryDisplayConfig(0, ref numPathArrayElements, null, ref numModeInfoArrayElements, null, out currentTopologyId);

            var pathArray = new DISPLAYCONFIG_PATH_INFO[numPathArrayElements];
            var modeInfoArray = new DISPLAYCONFIG_MODE_INFO[numModeInfoArrayElements];

            // Second call to get the actual data
            QueryDisplayConfig(0, ref numPathArrayElements, pathArray, ref numModeInfoArrayElements, modeInfoArray, out currentTopologyId);

            return (pathArray, modeInfoArray, currentTopologyId);
        }

        public static void SaveDisplayConfig(string filePath, DISPLAYCONFIG_PATH_INFO[] pathArray, DISPLAYCONFIG_MODE_INFO[] modeInfoArray, DISPLAYCONFIG_TOPOLOGY_ID topologyId)
        {
            var adapterConfigs = new Dictionary<string, object>();

            // Group modes by adapter ID
            foreach (var modeInfo in modeInfoArray)
            {
                string adapterKey = modeInfo.adapterId.ToString();
                if (!adapterConfigs.ContainsKey(adapterKey))
                {
                    adapterConfigs[adapterKey] = new
                    {
                        Modes = new List<DISPLAYCONFIG_MODE_INFO>(),
                        Paths = new List<DISPLAYCONFIG_PATH_INFO>()
                    };
                }

                ((List<DISPLAYCONFIG_MODE_INFO>)((dynamic)adapterConfigs[adapterKey]).Modes).Add(modeInfo);
            }

            // Group paths by adapter ID
            foreach (var path in pathArray)
            {
                string adapterKey = path.sourceInfo.adapterId.ToString();
                if (!adapterConfigs.ContainsKey(adapterKey))
                {
                    adapterConfigs[adapterKey] = new
                    {
                        Modes = new List<DISPLAYCONFIG_MODE_INFO>(),
                        Paths = new List<DISPLAYCONFIG_PATH_INFO>()
                    };
                }

                ((List<DISPLAYCONFIG_PATH_INFO>)((dynamic)adapterConfigs[adapterKey]).Paths).Add(path);
            }

            // Create the settings object
            var settings = new
            {
                Adapters = adapterConfigs,
                Topology = topologyId
            };

            // Serialize and save to file
            var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public static (DISPLAYCONFIG_PATH_INFO[], DISPLAYCONFIG_MODE_INFO[], DISPLAYCONFIG_TOPOLOGY_ID) LoadDisplayConfig(string filePath)
        {
            
            var json = File.ReadAllText(filePath);
            dynamic settings = JsonConvert.DeserializeObject(json);

            DISPLAYCONFIG_PATH_INFO[] pathArray = JsonConvert.DeserializeObject<DISPLAYCONFIG_PATH_INFO[]>(settings.Paths.ToString());
            DISPLAYCONFIG_MODE_INFO[] modeInfoArray = JsonConvert.DeserializeObject<DISPLAYCONFIG_MODE_INFO[]>(settings.Modes.ToString());
            DISPLAYCONFIG_TOPOLOGY_ID topologyId = (DISPLAYCONFIG_TOPOLOGY_ID)settings.Topology;

            return (pathArray, modeInfoArray, topologyId);
        }

        public static void ApplyDisplayConfig(DISPLAYCONFIG_PATH_INFO[] pathArray, DISPLAYCONFIG_MODE_INFO[] modeInfoArray, DISPLAYCONFIG_TOPOLOGY_ID topologyId)
        {
            IntPtr pathArrayPtr = IntPtr.Zero;
            IntPtr modeInfoArrayPtr = IntPtr.Zero;

            try
            {
                if (pathArray.Length > 0)
                {
                    pathArrayPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(DISPLAYCONFIG_PATH_INFO)) * pathArray.Length);
                    for (int i = 0; i < pathArray.Length; i++)
                    {
                        Marshal.StructureToPtr(pathArray[i], pathArrayPtr + (i * Marshal.SizeOf(typeof(DISPLAYCONFIG_PATH_INFO))), false);
                    }
                }

                if (modeInfoArray.Length > 0)
                {
                    modeInfoArrayPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(DISPLAYCONFIG_MODE_INFO)) * modeInfoArray.Length);
                    for (int i = 0; i < modeInfoArray.Length; i++)
                    {
                        Marshal.StructureToPtr(modeInfoArray[i], modeInfoArrayPtr + (i * Marshal.SizeOf(typeof(DISPLAYCONFIG_MODE_INFO))), false);
                    }
                }

                int result = SetDisplayConfig(
                    (uint)pathArray.Length,
                    pathArrayPtr,
                    (uint)modeInfoArray.Length,
                    modeInfoArrayPtr,
                    (uint)topologyId // Cast to uint
                );

                if (result != 0)
                {
                    uint unsignedResult = (uint)result;
                    throw new InvalidOperationException($"Failed to apply display configuration. Error code: {unsignedResult}");
                }
            }
            finally
            {
                // Free allocated memory
                if (pathArrayPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pathArrayPtr);
                }

                if (modeInfoArrayPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(modeInfoArrayPtr);
                }
            }
        } 

        static void Main()
        {

        }
    }
}
