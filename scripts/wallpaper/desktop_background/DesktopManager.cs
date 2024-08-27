using Godot;
using System.Runtime.InteropServices;
using System;
using static NativeMethods;
using System.Diagnostics;

public partial class DesktopManager : Node2D {
    public static DesktopManager Instance;
    public bool IsAttached = false;
    private IntPtr progman, workerw, program_handle;
    [Export] public bool WallpaperInstantlly = true;
    public bool IsDesktopVisible = true;

    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    public override void _EnterTree() {
        Instance = this;
        if (WallpaperInstantlly) {
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Maximized);
            Attach();
            this.IsAttached = true;
        }
    }
    public override void _Process(double delta) {
        if (IsAttached) {
            this.IsDesktopVisible = _IsDesktopVisible();
        }
    }
    private bool _IsDesktopVisible()
    {
        IntPtr window = GetForegroundWindow();
        IntPtr desktop = GetDesktopWindow();
        if (WindowFocus.IsDesktopActive()) return true;
        RECT a, b;
        GetWindowRect(window, out a);
        GetWindowRect(desktop, out b);
        return !(a.Left - b.Left < 20  &&
                b.Right - a.Right < 20   &&
                b.Bottom - a.Bottom < 50 &&
                a.Top - b.Top < 20);
    }
    public void Attach() {
        program_handle = GetActiveWindow();
        // Fetch the Progman window
        progman = NativeMethods.FindWindow("Progman", null);

        IntPtr result = IntPtr.Zero;

        // Send 0x052C to Progman. This message directs Progman to spawn a 
        // WorkerW behind the desktop icons. If it is already there, nothing 
        // happens.
        NativeMethods.SendMessageTimeout(progman,
            0x052C,
            new IntPtr(0xD),
            new IntPtr(0x1),
            NativeMethods.SendMessageTimeoutFlags.SMTO_NORMAL,
            1000,
            out result);
        // Spy++ output
        // .....
        // 0x00010190 "" WorkerW
        //   ...
        //   0x000100EE "" SHELLDLL_DefView
        //     0x000100F0 "FolderView" SysListView32
        // 0x00100B8A "" WorkerW       <-- This is the WorkerW instance we are after!
        // 0x000100EC "Program Manager" Progman
        workerw = IntPtr.Zero;
        // We enumerate all Windows, until we find one, that has the SHELLDLL_DefView 
        // as a child. 
        // If we found that window, we take its next sibling and assign it to workerw.
        NativeMethods.EnumWindows(new NativeMethods.EnumWindowsProc((tophandle, topparamhandle) => {
            IntPtr p = NativeMethods.FindWindowEx(tophandle,
                IntPtr.Zero,
                "SHELLDLL_DefView",
                IntPtr.Zero);

            if (p != IntPtr.Zero) {
                // Gets the WorkerW Window after the current one.
                workerw = NativeMethods.FindWindowEx(IntPtr.Zero,
                    tophandle,
                    "WorkerW",
                    IntPtr.Zero);
            }

            return true;
        }), IntPtr.Zero);

        NativeMethods.SetParent(program_handle, workerw);
    }
    void Deatatch() {
        NativeMethods.SetParent(program_handle, IntPtr.Zero);
    }
}