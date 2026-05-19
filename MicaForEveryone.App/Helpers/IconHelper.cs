using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media.Imaging;
using TerraFX.Interop.Windows;
using Windows.Storage.Streams;

namespace MicaForEveryone.App.Helpers;

public static class IconHelper
{
    private static readonly ConcurrentDictionary<string, BitmapImage> _cache = new();

    private static string? _lastExtractedPath;

    public static string? GetLastExtractedPath() => _lastExtractedPath;

    public static async Task<Icon?> ExtractIconFromWindowAsync(HWND hwnd)
    {
        return await Task.Run(() =>
        {
            uint processId;
            if (GetWindowThreadProcessId(hwnd, out processId) == 0 || processId == 0)
                return null;

            string? exePath = GetProcessExePath(processId);
            if (exePath == null || !File.Exists(exePath))
                return null;

            _lastExtractedPath = exePath;

            try
            {
                return Icon.ExtractAssociatedIcon(exePath);
            }
            catch
            {
                return null;
            }
        });
    }

    public static BitmapImage IconToBitmapImage(Icon icon)
    {
        using var bitmap = icon.ToBitmap();
        using var ms = new MemoryStream();
        bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
        ms.Position = 0;

        var bitmapImage = new BitmapImage();
        bitmapImage.SetSource(ms.AsRandomAccessStream());
        return bitmapImage;
    }

    private static string? GetProcessExePath(uint processId)
    {
        IntPtr hProcess = IntPtr.Zero;
        try
        {
            hProcess = OpenProcess(PROCESS_QUERY_LIMITED_INFORMATION, false, processId);
            if (hProcess == IntPtr.Zero)
                return null;

            var sb = new StringBuilder(260);
            uint size = (uint)sb.Capacity;
            if (QueryFullProcessImageName(hProcess, 0, sb, ref size))
            {
                return sb.ToString();
            }
        }
        catch
        {
        }
        finally
        {
            if (hProcess != IntPtr.Zero)
                CloseHandle(hProcess);
        }
        return null;
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint GetWindowThreadProcessId(HWND hWnd, out uint lpdwProcessId);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CloseHandle(IntPtr hObject);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool QueryFullProcessImageName(IntPtr hProcess, uint dwFlags, StringBuilder lpExeName, ref uint lpdwSize);

    private const uint PROCESS_QUERY_LIMITED_INFORMATION = 0x1000;
}