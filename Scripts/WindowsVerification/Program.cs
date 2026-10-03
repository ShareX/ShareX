// Windows service smoke checks. Registry writes use unique test keys/value names;
// shortcuts and synthetic files stay in a temporary directory. No uploads occur.
using Microsoft.Win32;
using ShareX.Platform;
using ShareX.Platform.Windows;
using System.Reflection;
using System.Runtime.InteropServices;

internal static class Program
{
    private static int failures;

    [STAThread]
    private static int Main()
    {
        Console.WriteLine($"Windows platform verification: {Environment.OSVersion.VersionString}");
        Run("Policy precedence and invalid-value fallback", VerifyPolicies);
        Run("Explorer menu, file associations and browser hosts", VerifyShellIntegration);
        Run("Startup shortcut and missing-target preservation", VerifyStartup);
        Run("Command-line argument round trip", VerifyArguments);
        Run("Taskbar COM progress calls (visual sign-off pending)", VerifyTaskbar);
        Run("Hotkey registration, conflicts and callback isolation", VerifyHotkeys);
        Run("Hotkey timeout cancellation and callback disposal", VerifyHotkeyShutdown);
        Run("Window enumeration and inspector service handoff", VerifyInspector);
        Console.WriteLine($"Verification complete: {failures} failure(s). Visual Windows 10/11 checklist remains separate.");
        return failures == 0 ? 0 : 1;
    }

    private static void Run(string name, Action action)
    {
        try
        {
            action();
            Console.WriteLine($"PASS: {name}");
        }
        catch (Exception exception)
        {
            failures++;
            Console.WriteLine($"FAIL: {name}: {exception}");
        }
    }

    private static void Equal<T>(T expected, T actual)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
        {
            throw new InvalidOperationException($"Expected '{expected}', received '{actual}'.");
        }
    }

    private static T Create<T>(params object[] arguments) => (T)Activator.CreateInstance(typeof(T),
        BindingFlags.Instance | BindingFlags.NonPublic, null, arguments, null)!;

    private static void VerifyPolicies()
    {
        object? Policy(string name, object? machine, object? user)
        {
            Func<RegistryHive, string, object?> reader = (hive, _) => hive == RegistryHive.LocalMachine ? machine : user;
            return Create<WindowsSystemPreferencesService>(reader).GetPolicy(name);
        }

        foreach (string name in new[] { "DisableUpdateCheck", "DisableUpload", "DisableLogging" })
        {
            Equal<object?>(false, Policy(name, 0, 1));
            Equal<object?>(true, Policy(name, 1, 0));
            Equal<object?>(true, Policy(name, "invalid", 1));
            Equal<object?>(false, Policy(name, "invalid", "false"));
            Equal<object?>(true, Policy(name, null, "true"));
            Equal<object?>(null, Policy(name, new byte[] { 1 }, "invalid"));
        }

        Equal<object?>(@"C:\Machine", Policy("PersonalPath", @"C:\Machine", @"C:\User"));
        Equal<object?>(@"C:\User", Policy("PersonalPath", 1, @"C:\User"));
        Equal<object?>(null, Policy("PersonalPath", 1, new byte[] { 1 }));
        Equal<object?>("", Policy("PersonalPath", "", @"C:\User"));
        Equal<object?>(42, Policy("FuturePolicy", 42, 0));
    }

    private static void VerifyShellIntegration()
    {
        string keyPath = @"Software\ShareX.Platform.Verification\" + Guid.NewGuid().ToString("N");
        string directory = Path.Combine(Path.GetTempPath(), "sharex-windows-verification-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);

        try
        {
            using RegistryKey root = Registry.CurrentUser.CreateSubKey(keyPath);
            WindowsShellIntegrationService service = Create<WindowsShellIntegrationService>(root, directory);
            string executable = Path.Combine(directory, "Share X.exe");
            File.WriteAllText(executable, "Synthetic shortcut target, never executed.");

            ShellMenuEntry upload = new("ShareX", "Upload with ShareX", executable, [], ShellMenuTarget.FilesAndFolders);
            ShellMenuEntry edit = new("ShareXImageEditor", "Edit with ShareX", executable, ["-ImageEditor"], ShellMenuTarget.Images);

            foreach (ShellMenuEntry entry in new[] { upload, edit })
            {
                Equal(false, service.IsRegistered(entry));
                service.Register(entry);
                Equal(true, service.IsRegistered(entry));

                foreach (string menuPath in WindowsShellIntegrationService.GetMenuKeys(entry))
                {
                    using RegistryKey menu = root.OpenSubKey(menuPath)!;
                    Equal(entry.Label, menu.GetValue(null));
                    Equal($"\"{executable}\",0", menu.GetValue("Icon"));
                    using RegistryKey command = menu.OpenSubKey("command")!;
                    Equal(entry.Target == ShellMenuTarget.Images ? $"\"{executable}\" -ImageEditor \"%1\"" : $"\"{executable}\" \"%1\"", command.GetValue(null));
                }

                service.Unregister(entry);
                Equal(false, service.IsRegistered(entry));
            }

            foreach ((string extension, string type, string argument) in new[]
                { (".sxcu", "ShareX.sxcu", "-CustomUploader"), (".sxie", "ShareX.sxie", "-ImageEffect") })
            {
                FileAssociation association = new(extension, type, "Synthetic type", "application/x-sharex-verification", executable, [argument])
                {
                    Icon = Path.Combine(directory, "ShareX_File_Icon.ico")
                };
                Equal(false, service.IsAssociated(association));
                service.Associate(association);
                Equal(true, service.IsAssociated(association));
                Equal(true, service.IsAssociated(association with { ExecutablePath = executable.ToUpperInvariant(), TypeId = type.ToUpperInvariant() }));
                using (RegistryKey icon = root.OpenSubKey($@"Software\Classes\{type}\DefaultIcon")!)
                {
                    Equal($"\"{association.Icon}\"", icon.GetValue(null));
                }
                using (RegistryKey command = root.OpenSubKey($@"Software\Classes\{type}\shell\open\command")!)
                {
                    Equal($"\"{executable}\" {argument} \"%1\"", command.GetValue(null));
                }
                service.RemoveAssociation(association);
                Equal(false, service.IsAssociated(association));
            }

            string manifest = Path.Combine(directory, "host-manifest.json");
            File.WriteAllText(manifest, "{}");
            foreach ((BrowserFamily browser, string name) in new[]
                { (BrowserFamily.Chromium, "com.getsharex.sharex"), (BrowserFamily.Firefox, "ShareX") })
            {
                BrowserHost host = new(browser, name, manifest, executable);
                service.RegisterBrowserHost(host);
                Equal(true, service.IsBrowserHostRegistered(host));
                Equal(true, service.IsBrowserHostRegistered(host with { ManifestPath = manifest.ToUpperInvariant() }));
                string browserKey = browser == BrowserFamily.Firefox
                    ? $@"SOFTWARE\Mozilla\NativeMessagingHosts\{name}"
                    : $@"SOFTWARE\Google\Chrome\NativeMessagingHosts\{name}";
                using (RegistryKey registered = root.OpenSubKey(browserKey)!)
                {
                    Equal(manifest, registered.GetValue(null));
                }
                service.UnregisterBrowserHost(host);
                Equal(false, service.IsBrowserHostRegistered(host));
            }

            service.SetInSendTo("ShareX", executable, true);
            Equal(true, service.IsInSendTo("ShareX", executable));
            service.SetInSendTo("ShareX", Path.Combine(directory, "missing.exe"), true);
            Equal(true, service.IsInSendTo("ShareX", executable));
            service.SetInSendTo("ShareX", executable, false);
            Equal(false, service.IsInSendTo("ShareX", executable));
        }
        finally
        {
            Registry.CurrentUser.DeleteSubKeyTree(keyPath, false);
            Directory.Delete(directory, true);
        }
    }

    private static void VerifyStartup()
    {
        string directory = Path.Combine(Path.GetTempPath(), "sharex-startup-verification-" + Guid.NewGuid().ToString("N"));
        string name = "ShareX verification " + Guid.NewGuid().ToString("N");
        const string approvedPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\StartupFolder";
        Directory.CreateDirectory(directory);

        try
        {
            string executable = Path.Combine(directory, "Share X.exe");
            File.WriteAllText(executable, "Synthetic shortcut target, never executed.");
            StartupRegistration registration = new(name, "ShareX verification", executable, ["-silent"]);
            WindowsStartupService service = new(directory);
            Equal(StartupRegistrationState.Disabled, service.GetState(registration));
            service.SetEnabled(registration, true);
            Equal(StartupRegistrationState.Enabled, service.GetState(registration));
            ReadShortcut(service.GetShortcutPath(registration), executable, "-silent");
            using (RegistryKey approved = Registry.CurrentUser.CreateSubKey(approvedPath))
            {
                approved.SetValue(name + ".lnk", new byte[] { 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, RegistryValueKind.Binary);
                Equal(StartupRegistrationState.DisabledByUser, service.GetState(registration));
                approved.DeleteValue(name + ".lnk", false);
            }
            service.SetEnabled(registration with { ExecutablePath = Path.Combine(directory, "missing.exe") }, true);
            Equal(StartupRegistrationState.Enabled, service.GetState(registration));
            service.SetEnabled(registration, false);
            Equal(StartupRegistrationState.Disabled, service.GetState(registration));
        }
        finally
        {
            using RegistryKey? approved = Registry.CurrentUser.OpenSubKey(approvedPath, writable: true);
            approved?.DeleteValue(name + ".lnk", false);
            Directory.Delete(directory, true);
        }
    }

    private static void ReadShortcut(string path, string executable, string arguments)
    {
        object shell = Activator.CreateInstance(Type.GetTypeFromProgID("WScript.Shell")!)!;
        object? shortcut = null;
        try
        {
            shortcut = shell.GetType().InvokeMember("CreateShortcut", BindingFlags.InvokeMethod, null, shell, [path]);
            Equal(executable, shortcut!.GetType().InvokeMember("TargetPath", BindingFlags.GetProperty, null, shortcut, null));
            Equal(arguments, shortcut.GetType().InvokeMember("Arguments", BindingFlags.GetProperty, null, shortcut, null));
            Equal(Path.GetDirectoryName(executable), shortcut.GetType().InvokeMember("WorkingDirectory", BindingFlags.GetProperty, null, shortcut, null));
        }
        finally
        {
            if (shortcut != null) Marshal.FinalReleaseComObject(shortcut);
            Marshal.FinalReleaseComObject(shell);
        }
    }

    private static void VerifyArguments()
    {
        MethodInfo quote = typeof(WindowsStartupService).GetMethod("QuoteArgument", BindingFlags.NonPublic | BindingFlags.Static)!;
        string[] arguments = ["-silent", "", "two words", "a\"b", @"C:\folder with spaces\", "a\\\"b", "tab\tvalue"];
        string command = "verification.exe " + string.Join(" ", arguments.Select(argument => (string)quote.Invoke(null, [argument])!));
        IntPtr argv = CommandLineToArgvW(command, out int count);
        if (argv == IntPtr.Zero) throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
        try
        {
            Equal(arguments.Length + 1, count);
            for (int i = 0; i < arguments.Length; i++)
            {
                Equal(arguments[i], Marshal.PtrToStringUni(Marshal.ReadIntPtr(argv, (i + 1) * IntPtr.Size)));
            }
        }
        finally
        {
            LocalFree(argv);
        }
    }

    private static void VerifyTaskbar()
    {
        // A hidden window owned by this harness keeps progress calls away from the user's applications.
        IntPtr window = CreateWindowExW(0, "STATIC", "ShareX verification", 0, 0, 0, 1, 1, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        if (window == IntPtr.Zero) throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
        try
        {
            WindowsTaskbarService service = new();
            typeof(WindowsTaskbarService).GetField("mainWindowHandle", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(service, window);
            service.SetProgressState(TaskbarProgressState.Normal);
            service.SetProgressValue(-1, 100);
            service.SetProgressValue(50, 100);
            service.SetProgressValue(101, 100);
            service.SetProgressValue(1, 0);
            service.SetProgressState(TaskbarProgressState.Indeterminate);
            service.SetProgressState(TaskbarProgressState.Paused);
            service.SetProgressState(TaskbarProgressState.Error);
            service.SetProgressState(TaskbarProgressState.None);
            Equal(true, service.Support.IsSupported);
        }
        finally
        {
            DestroyWindow(window);
        }
    }

    private static readonly PlatformHotkey Hotkey = new(0x87, HotkeyModifiers.Control | HotkeyModifiers.Alt | HotkeyModifiers.Shift); // F24
    private static readonly PlatformHotkey OtherHotkey = Hotkey with { KeyCode = 0x86 }; // F23

    private static void VerifyHotkeys()
    {
        using WindowsHotkeyService first = new();
        using WindowsHotkeyService second = new();
        Equal(HotkeyRegistrationStatus.UnsupportedKey, first.Register(1, new PlatformHotkey(0, HotkeyModifiers.None)));
        Equal(HotkeyRegistrationStatus.UnsupportedKey, first.Register(1, new PlatformHotkey(0x10, HotkeyModifiers.None)));
        Equal(HotkeyRegistrationStatus.UnsupportedKey, first.Register(1, new PlatformHotkey(0x100, HotkeyModifiers.None)));
        Equal(HotkeyRegistrationStatus.Registered, first.Register(1, Hotkey));
        Equal(HotkeyRegistrationStatus.InUse, second.Register(2, Hotkey));
        Equal(HotkeyRegistrationStatus.Registered, first.Register(1, OtherHotkey));
        Equal(HotkeyRegistrationStatus.Registered, second.Register(2, Hotkey));
        Equal(true, first.Unregister(1));
        Equal(false, first.Unregister(1));
        Equal(HotkeyRegistrationStatus.Registered, second.Register(3, OtherHotkey));
        second.UnregisterAll();
        Equal(HotkeyRegistrationStatus.Registered, first.Register(4, Hotkey));

        int delivered = 0;
        using ManualResetEventSlim received = new();
        first.HotkeyPressed += (_, _) => throw new InvalidOperationException("Synthetic subscriber failure.");
        first.HotkeyPressed += (_, args) =>
        {
            Equal(4, args.Id);
            Equal(Hotkey, args.Hotkey);
            if (Interlocked.Increment(ref delivered) == 2) received.Set();
        };

        // A queued message for the old key must not activate the replacement registration.
        PostHotkey(first, 4, OtherHotkey);
        PostHotkey(first, 4, Hotkey);
        PostHotkey(first, 4, Hotkey);
        Equal(true, received.Wait(TimeSpan.FromSeconds(3)));
        Equal(2, Volatile.Read(ref delivered));
        Equal(true, first.Unregister(4));
        Equal(HotkeyRegistrationStatus.Registered, second.Register(4, Hotkey));
    }

    private static void VerifyHotkeyShutdown()
    {
        using WindowsHotkeyService first = new();
        using WindowsHotkeyService second = new();
        using ManualResetEventSlim blocked = new();
        using ManualResetEventSlim release = new();
        using ManualResetEventSlim disposed = new();
        EventHandler<HotkeyPressedEventArgs> blockingHandler = (_, _) =>
        {
            blocked.Set();
            release.Wait(TimeSpan.FromSeconds(10));
        };

        try
        {
            Equal(HotkeyRegistrationStatus.Registered, first.Register(5, Hotkey));
            first.HotkeyPressed += blockingHandler;
            PostHotkey(first, 5, Hotkey);
            Equal(true, blocked.Wait(TimeSpan.FromSeconds(3)));
            Equal(HotkeyRegistrationStatus.Failed, first.Register(6, OtherHotkey));
            release.Set();
            // Flush the queued cancelled command before checking for an unexpected native registration.
            Equal(false, first.Unregister(999));
            Equal(HotkeyRegistrationStatus.Registered, second.Register(6, OtherHotkey));
            first.HotkeyPressed -= blockingHandler;
            first.UnregisterAll();

            Equal(HotkeyRegistrationStatus.Registered, first.Register(7, Hotkey));
            first.HotkeyPressed += (_, _) =>
            {
                first.Dispose();
                disposed.Set();
            };
            PostHotkey(first, 7, Hotkey);
            Equal(true, disposed.Wait(TimeSpan.FromSeconds(1)));
            first.Dispose(); // Wait for cleanup from outside the native thread.
            Equal(false, first.Support.IsSupported);
            Equal(HotkeyRegistrationStatus.Failed, first.Register(8, Hotkey));
            Equal(false, first.Unregister(7));
            Equal(HotkeyRegistrationStatus.Registered, second.Register(7, Hotkey));

            Task[] callers = Enumerable.Range(0, 16).Select(id => Task.Run(() => second.Register(id + 100, Hotkey))).ToArray();
            second.Dispose();
            Equal(true, Task.WaitAll(callers, TimeSpan.FromSeconds(3)));
        }
        finally
        {
            release.Set();
        }
    }

    private static void PostHotkey(WindowsHotkeyService service, int id, PlatformHotkey hotkey)
    {
        TaskCompletionSource<uint> threadId = (TaskCompletionSource<uint>)typeof(WindowsHotkeyService)
            .GetField("threadId", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(service)!;
        uint data = ((uint)hotkey.KeyCode << 16) | (uint)hotkey.Modifiers;
        if (!PostThreadMessageW(threadId.Task.Result, 0x0312, (nuint)id, (nint)data))
        {
            throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
        }
    }

    private static void VerifyInspector()
    {
        PlatformServices.Initialize(new WindowsPlatformServices());
        // Off-screen tool window: visible to enumeration without putting a window on the user's desktop.
        IntPtr window = CreateWindowExW(0x08000080, "STATIC", "ShareX inspector verification", 0x10000000,
            -32000, -32000, 100, 80, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        if (window == IntPtr.Zero) throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());

        try
        {
            Equal(true, PlatformServices.Current.Windows.GetWindows().Any(item => item.Handle == window.ToInt64()));
            PlatformRectangle client = PlatformServices.Current.WindowManagement.GetDetails(window.ToInt64())!.ClientBounds!.Value;
            Equal(0, client.X);
            Equal(0, client.Y);
            Equal(false, client.IsEmpty);
            using ShareX.Tools.InspectWindowViewModel viewModel = new();
            viewModel.SelectWindow(window, true);
            Equal(true, viewModel.HasSelection);
            Equal("ShareX inspector verification", viewModel.SelectedTitle);
            Equal(true, viewModel.CanChangeTopMost);
            Equal(true, viewModel.CanChangeOpacity);
            Equal(10, viewModel.Details.Count);
            viewModel.IsTopMost = true;
            Equal(true, PlatformServices.Current.WindowManagement.GetDetails(window.ToInt64())!.IsTopMost!.Value);
            viewModel.Opacity = 50;
            Equal((byte)128, PlatformServices.Current.WindowManagement.GetDetails(window.ToInt64())!.Opacity!.Value);
            viewModel.SelectWindow(window, false);
            Equal(false, viewModel.CanChangeTopMost);
            Equal(false, viewModel.CanChangeOpacity);
            DestroyWindow(window);
            window = IntPtr.Zero;
            viewModel.RefreshCommand.Execute(null);
            Equal(false, viewModel.HasSelection);
            Equal(0, viewModel.Details.Count);
        }
        finally
        {
            if (window != IntPtr.Zero) DestroyWindow(window);
            PlatformServices.Shutdown();
        }
    }

    [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern IntPtr CommandLineToArgvW(string command, out int count);

    [DllImport("kernel32.dll")]
    private static extern IntPtr LocalFree(IntPtr memory);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern IntPtr CreateWindowExW(uint extendedStyle, string className, string title, uint style,
        int x, int y, int width, int height, IntPtr parent, IntPtr menu, IntPtr instance, IntPtr parameter);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DestroyWindow(IntPtr window);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool PostThreadMessageW(uint threadId, uint message, nuint wParam, nint lParam);
}
