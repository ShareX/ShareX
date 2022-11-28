using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security;

namespace ShareX.ContextMenuHandler
{
    using GETPROPERTYSTOREFLAGS = UInt32;

    public enum SIGDN : uint
    {
        NORMALDISPLAY,
        PARENTRELATIVEPARSING = 2147581953,
        DESKTOPABSOLUTEPARSING = 2147647488,
        PARENTRELATIVEEDITING = 2147684353,
        DESKTOPABSOLUTEEDITING = 2147794944,
        FILESYSPATH = 2147844096,
        URL = 2147909632,
        PARENTRELATIVEFORADDRESSBAR = 2147991553,
        PARENTRELATIVE = 2148007937
    }

    [Flags]
    public enum SIATTRIBFLAGS
    {
        SIATTRIBFLAGS_AND = 1,
        SIATTRIBFLAGS_OR = 2,
        SIATTRIBFLAGS_APPCOMPAT = 3,
        SIATTRIBFLAGS_MASK = 3,
        SIATTRIBFLAGS_ALLITEMS = 16384
    }

    [Flags]
    public enum EXPCMDSTATE
    {
        ECS_ENABLED = 0,
        ECS_DISABLED = 1,
        ECS_HIDDEN = 2,
        ECS_CHECKBOX = 4,
        ECS_CHECKED = 8,
        ECS_RADIOCHECK = 16,
    };

    [Flags]
    public enum EXPCMDFLAGS
    {
        ECF_DEFAULT = 0,
        ECF_HASSUBCOMMANDS = 1,
        ECF_HASSPLITBUTTON = 2,
        ECF_HIDELABEL = 4,
        ECF_ISSEPARATOR = 8,
        ECF_HASLUASHIELD = 16,
        ECF_SEPARATORBEFORE = 32,
        ECF_SEPARATORAFTER = 64,
        ECF_ISDROPDOWN = 128,
        ECF_TOGGLEABLE = 256,
        ECF_AUTOMENUICONS = 512
    };

    [Flags]
    public enum SFGAO : uint
    {
        CANCOPY = 1,
        CANMOVE = 2,
        CANLINK = 4,
        STORAGE = 8,
        CANRENAME = 16,
        CANDELETE = 32,
        HASPROPSHEET = 64,
        DROPTARGET = 256,
        SYSTEM = 4096,
        ENCRYPTED = 8192,
        ISSLOW = 16384,
        GHOSTED = 32768,
        LINK = 65536,
        SHARE = 131072,
        READONLY = 262144,
        HIDDEN = 524288,
        NONENUMERATED = 1048576,
        NEWCONTENT = 2097152,
        STREAM = 4194304,
        STORAGEANCESTOR = 8388608,
        VALIDATE = 16777216,
        REMOVABLE = 33554432,
        COMPRESSED = 67108864,
        BROWSABLE = 134217728,
        FILESYSANCESTOR = 268435456,
        FOLDER = 536870912,
        FILESYSTEM = 1073741824,
        HASSUBFOLDER = 2147483648,
    }

    public struct PROPERTYKEY
    {
        public Guid fmtid;

        public uint pid;
    }

    [ComImport]
    [Guid("A08CE4D0-FA25-44AB-B57C-C7B1C323E0B9")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [SuppressUnmanagedCodeSecurity]
    public interface IExplorerCommand
    {
        void GetTitle(IShellItemArray psiItemArray, [MarshalAs(UnmanagedType.LPWStr)] out string ppszName);

        void GetIcon(IShellItemArray psiItemArray, [MarshalAs(UnmanagedType.LPWStr)] out string ppszIcon);

        void GetToolTip(IShellItemArray psiItemArray, [MarshalAs(UnmanagedType.LPWStr)] out string ppszInfotip);

        void GetCanonicalName(out Guid pguidCommandName);

        void GetState(IShellItemArray psiItemArray, bool fOkToBeSlow, out EXPCMDSTATE pCmdState);

        void Invoke(IShellItemArray psiItemArray, IBindCtx pbc);

        void GetFlags(out EXPCMDFLAGS pFlags);

        void EnumSubCommands(out object ppenum);
    }

    [ComImport]
    [Guid("B63EA76D-1F85-456F-A19C-48159EFA858B")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [SuppressUnmanagedCodeSecurity]
    public interface IShellItemArray
    {
        void BindToHandler(IBindCtx pbc, in Guid bhid, in Guid riid, out IntPtr ppvOut);

        void GetPropertyStore(GETPROPERTYSTOREFLAGS Flags, in Guid riid, out IntPtr ppv);

        void GetPropertyDescriptionList(in PROPERTYKEY keyType, in Guid riid, out IntPtr ppv);

        void GetAttributes(SIATTRIBFLAGS AttribFlags, SFGAO sfgaoMask, out SFGAO psfgaoAttribs);

        void GetCount(out uint pdwNumItems);

        void GetItemAt(uint dwIndex, out IShellItem ppsi);

        void EnumItems(out object ppenumShellItems);
    }

    [ComImport]

    [Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [SuppressUnmanagedCodeSecurity]
    public interface IShellItem
    {
        void BindToHandler(IBindCtx pbc, in Guid bhid, in Guid riid, out IntPtr ppv);

        void GetParent(out object ppsi);

        void GetDisplayName(SIGDN sigdnName, [MarshalAs(UnmanagedType.LPWStr)] out string ppszName);

        void GetAttributes(SFGAO sfgaoMask, out SFGAO psfgaoAttribs);

        void Compare(IShellItem psi, uint hint, out int piOrder);
    }
}

