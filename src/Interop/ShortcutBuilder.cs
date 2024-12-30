// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Media.Interop;

internal sealed class ShortcutBuilder
{
    private string _targetPath = string.Empty;
    private string _description = string.Empty;
    private string _iconPath = string.Empty;
    private string _arguments = string.Empty;
    private int _iconIndex;
    private string _workDirectory = string.Empty;

    [ComImport]
    [Guid("00021401-0000-0000-C000-000000000046")]
    internal class ShellLink
    {
    }

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("000214F9-0000-0000-C000-000000000046")]
    internal interface IShellLink
    {
        void GetPath([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxPath, out IntPtr pfd, int fFlags);
        void GetIDList(out IntPtr ppidl);
        void SetIDList(IntPtr pidl);
        void GetDescription([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName, int cchMaxName);
        void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);
        void GetWorkingDirectory([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cchMaxPath);
        void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);
        void GetArguments([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cchMaxPath);
        void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
        void GetHotkey(out short pwHotkey);
        void SetHotkey(short wHotkey);
        void GetShowCmd(out int piShowCmd);
        void SetShowCmd(int iShowCmd);
        void GetIconLocation([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath, int cchIconPath, out int piIcon);
        void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);
        void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, int dwReserved);
        void Resolve(IntPtr hwnd, int fFlags);
        void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
    }

    public ShortcutBuilder WithTargetPath(string targetPath)
    {
        _targetPath = targetPath;
        return this;
    }

    public ShortcutBuilder WithArguments(string arguments)
    {
        _arguments = arguments;
        return this;
    }

    public ShortcutBuilder WithWorkDirectory(string workDirectory)
    {
        _workDirectory = workDirectory;
        return this;
    }

    public ShortcutBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public ShortcutBuilder WithIcon(string iconPath, int index = 0)
    {
        _iconPath = iconPath;
        _iconIndex = index;
        return this;
    }

    public void Build(string targetFile)
    {
        if (string.IsNullOrEmpty(_targetPath))
        {
            throw new InvalidOperationException("Target path is not set");
        }

        IShellLink link = (IShellLink)new ShellLink();
        link.SetPath(_targetPath);
        link.SetDescription(_description);

        if (!string.IsNullOrEmpty(_iconPath))
            link.SetIconLocation(_iconPath, _iconIndex);
        else
            link.SetIconLocation(_targetPath, 0);

        if (!string.IsNullOrEmpty(_arguments))
            link.SetArguments(_arguments);

        if (!string.IsNullOrEmpty(_workDirectory))
            link.SetWorkingDirectory(_workDirectory);


        IPersistFile file = (IPersistFile)link;
        file.Save(targetFile, false);
    }
}
