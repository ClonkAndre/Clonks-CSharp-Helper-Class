// Clonk's Helper Class
// https://github.com/ClonkAndre/Clonks-CSharp-Helper-Class
// Last updated: 8/30/2022
// Change: Renamed "IsUserConnectedToTheInternet" to "CheckForInternetConnection" and made it more reliable?

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Microsoft.VisualBasic;
using Microsoft.Win32;

#region Public Enums
public enum FileSizes {
    Byte,
    Kilobyte,
    Megabyte,
    Gigabyte,
    Terabyte,
    Petabyte,
    Exabyte,
}
#endregion

#region Public Structs
public struct FileSize {
    #region Properties
    public FileSizes FileSizes { get; private set; }
    public double Size { get; private set; }
    #endregion

    #region Constructor
    internal FileSize(double fileSize, FileSizes fileSizes)
    {
        FileSizes = fileSizes;
        Size = fileSize;
    }
    #endregion
}
public struct AResult {

    #region Properties
    public Exception Exception { get; private set; }
    public object Result { get; private set; }
    #endregion

    #region Constructor
    public AResult(Exception ex, object result)
    {
        Exception = ex;
        Result = result;
    }
    #endregion

}
#endregion

internal static class Helper {

    #region Classes
    /// <summary>
    /// Convert stuff.
    /// </summary>
    public class ConverterStuff {

        public static bool ConvertIntToBool(int number)
        {
            return number == 1;
        }

    }

    /// <summary>
    /// If you override the <see cref="Console.Out"/> property with a new instance of the <see cref="ConsoleOutputRedirector"/>
    /// you can subscribe to its <see cref="ConsoleOutputRedirector.OnLineAdded"/> event to be notified when a new line got written to the console using <see cref="Console.WriteLine(string)"/>.
    /// <br/><br/>
    /// You also have the possibility to view the history of the console using <see cref="ConsoleOutputRedirector.GetLines()"/>.
    /// </summary>
    public class ConsoleOutputRedirector : TextWriter {

        #region Variables
        private List<string> lines;
        private TextWriter original;
        #endregion

        #region Events
        public delegate void LineAdded(string value);
        public event LineAdded OnLineAdded;
        #endregion

        #region Constructor
        public ConsoleOutputRedirector(TextWriter original)
        {
            lines = new List<string>();
            this.original = original;
        }
        #endregion

        #region Overrides
        public override Encoding Encoding
        {
            get { return Encoding.Default; }
        }
        public override void WriteLine(string value)
        {
            lines.Add(value);
            original.WriteLine(value);
            OnLineAdded?.Invoke(value);
        }
        // You need to override other methods also
        #endregion

        #region Functions
        public string[] GetLines()
        {
            return lines.ToArray();
        }
        #endregion

    }

    /// <summary>
    /// Compression stuff.
    /// </summary>
    public class DataCompression {

        #region Byte Array
        /// <summary>
        /// Compresses and returns a byte array.
        /// </summary>
        /// <param name="uncompressedByteArray">Byte array to compress</param>
        public static byte[] CompressByteArray(byte[] uncompressedByteArray)
        {
            try {
                byte[] compressedBytes;

                using (var uncompressedStream = new MemoryStream(uncompressedByteArray)) {
                    using (var compressedStream = new MemoryStream()) {
                        using (var compressorStream = new DeflateStream(compressedStream, CompressionLevel.Optimal, true)) {
                            uncompressedStream.CopyTo(compressorStream);
                        }
                        compressedBytes = compressedStream.ToArray();
                    }
                }

                return compressedBytes;
            }
            catch (Exception) {
                return null;
            }
        }

        /// <summary>
        /// Decompresses and returns a compressed byte array.
        /// </summary>
        /// <param name="compressedByteArray">Byte array to decompress.</param>
        public static byte[] DecompressByteArray(byte[] compressedByteArray)
        {
            try {
                byte[] decompressedBytes;

                var compressedStream = new MemoryStream(compressedByteArray);

                using (var decompressorStream = new DeflateStream(compressedStream, CompressionMode.Decompress)) {
                    using (var decompressedStream = new MemoryStream()) {
                        decompressorStream.CopyTo(decompressedStream);

                        decompressedBytes = decompressedStream.ToArray();
                    }
                }

                return decompressedBytes;
            }
            catch (Exception) {
                return null;
            }
        }
        #endregion

        #region String
        /// <summary>
        /// Compresses a string and returns a deflate compressed, Base64 encoded string.
        /// </summary>
        /// <param name="uncompressedString">String to compress</param>
        public static string CompressString(string uncompressedString, string returnStringOnError = "")
        {
            try {
                byte[] compressedBytes;

                using (var uncompressedStream = new MemoryStream(Encoding.UTF8.GetBytes(uncompressedString))) {
                    using (var compressedStream = new MemoryStream()) {
                        using (var compressorStream = new DeflateStream(compressedStream, CompressionLevel.Optimal, true)) {
                            uncompressedStream.CopyTo(compressorStream);
                        }
                        compressedBytes = compressedStream.ToArray();
                    }
                }

                return Convert.ToBase64String(compressedBytes);
            }
            catch (Exception) {
                return returnStringOnError;
            }
        }

        /// <summary>
        /// Decompresses a deflate compressed, Base64 encoded string and returns an uncompressed string.
        /// </summary>
        /// <param name="compressedString">String to decompress.</param>
        public static string DecompressString(string compressedString, string returnStringOnError = "")
        {
            try {
                byte[] decompressedBytes;

                var compressedStream = new MemoryStream(Convert.FromBase64String(compressedString));

                using (var decompressorStream = new DeflateStream(compressedStream, CompressionMode.Decompress)) {
                    using (var decompressedStream = new MemoryStream()) {
                        decompressorStream.CopyTo(decompressedStream);

                        decompressedBytes = decompressedStream.ToArray();
                    }
                }

                return Encoding.UTF8.GetString(decompressedBytes);
            }
            catch (Exception) {
                return returnStringOnError;
            }
        }
        #endregion

    }

    /// <summary>
    /// Parsing stuff.
    /// </summary>
    public class ParseExtension {
        public static bool Parse(string s, bool defaultValue = false)
        {
            bool result;
            bool parseResult = bool.TryParse(s, out result);
            if (parseResult) {
                return result;
            }
            return defaultValue;
        }
        public static int Parse(string s, int defaultValue = 0)
        {
            int result;
            bool parseResult = int.TryParse(s, out result);
            if (parseResult) {
                return result;
            }
            return defaultValue;
        }
        public static ushort Parse(string s, ushort defaultValue = 0)
        {
            ushort result;
            bool parseResult = ushort.TryParse(s, out result);
            if (parseResult) {
                return result;
            }
            return defaultValue;
        }
        public static long Parse(string s, long defaultValue = 0)
        {
            long result;
            bool parseResult = long.TryParse(s, out result);
            if (parseResult) {
                return result;
            }
            return defaultValue;
        }
        public static ulong Parse(string s, ulong defaultValue = 0)
        {
            ulong result;
            bool parseResult = ulong.TryParse(s, out result);
            if (parseResult) {
                return result;
            }
            return defaultValue;
        }
        public static double Parse(string s, double defaultValue = 0)
        {
            double result;
            bool parseResult = double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out result);
            if (parseResult) {
                return result;
            }
            return defaultValue;
        }
        public static float Parse(string s, float defaultValue = 0f)
        {
            float result;
            bool parseResult = float.TryParse(s, out result);
            if (parseResult) {
                return result;
            }
            return defaultValue;
        }
    }
    
    /// <summary>
    /// Class to register or unregister URI Schemes.
    /// </summary>
    public class URISchemeHandler {
    
        public static void Register(string protocolName, string executablePath)
        {
            RegistryKey regKey = Registry.ClassesRoot.CreateSubKey(protocolName);

            regKey.CreateSubKey("DefaultIcon").SetValue(null, string.Format("{0}{1},1{0}", (char)34, executablePath));

            regKey.SetValue(null, string.Format("URL:{0} Protocol", protocolName));
            regKey.SetValue("URL Protocol", "");

            regKey = regKey.CreateSubKey(@"shell\open\command");
            regKey.SetValue(null, string.Format("{0}{1}{0} {0}%1{0}", (char)34, executablePath));
        }
        public static void Unregister(string protocolName)
        {
            Registry.ClassesRoot.DeleteSubKeyTree(protocolName, false);
        }

    }
	
	/// <summary>
    /// Some helper functions in use with <see cref="Process"/>.
    /// </summary>
    public class ProcessHelper {

        #region DllImports
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();


        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);
        #endregion

        #region Structs
        private struct WINDOWPLACEMENT {
            public int length;
            public int flags;
            public int showCmd;
            public System.Drawing.Point ptMinPosition;
            public System.Drawing.Point ptMaxPosition;
            public System.Drawing.Rectangle rcNormalPosition;
        }
        #endregion

        /// <summary>
        /// Gets the current window state of the given <see cref="Process"/>.
        /// </summary>
        /// <param name="p">Target <see cref="Process"/>.</param>
        /// <returns>
        /// -1 : MainWindowHandle of given <see cref="Process"/> is null.<br/>
        /// 1  : <see cref="Process"/> Window is <b>Normalized</b>.<br/>
        /// 2  : <see cref="Process"/> Window is <b>Minimized</b>.<br/>
        /// 3  : <see cref="Process"/> Window is <b>Maximized</b>.
        /// </returns>
        public static int GetProcessWindowState(Process p)
        {
            if (p.MainWindowHandle != IntPtr.Zero) {
                WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
                GetWindowPlacement(p.MainWindowHandle, ref placement);
                return placement.showCmd;
            }
            return -1;
        }

        /// <summary>
        /// Gets if the given <see cref="Process"/> has focus.
        /// </summary>
        /// <param name="p">Target <see cref="Process"/>.</param>
        /// <returns>True if the <see cref="Process"/> is in focus, otherwise false.</returns>
        public static bool IsProcessInFocus(Process p)
        {
            var activatedHandle = GetForegroundWindow();
            if (activatedHandle == IntPtr.Zero) {
                return false; // No window is currently activated
            }

            int procId = p.Id;
            int activeProcId;
            GetWindowThreadProcessId(activatedHandle, out activeProcId);

            return activeProcId == procId;
        }

    }
    #endregion

    #region Functions
    public static string GetYouTubeVideoIDFromURL(string url)
    {
        try {
            if (!string.IsNullOrWhiteSpace(url)) {
                string id = url;

                // Check URL format
                if (id.Contains("youtube.com") && id.Contains("embed")) { // Example: https://www.youtube.com/embed/M80K51DosFo?rel=0&autoplay=0&controls=1&fs=0&iv_load_policy=3
                    id = id.Split('/')[4];

                    if (id.Contains('?')) { // Contains parameters
                        return id.Split('?')[0];
                    }
                    else {
                        return id;
                    }
                }
                if (id.Contains("youtube.com") && id.Contains("watch?v=")) { // Example: https://www.youtube.com/watch?v=M80K51DosFo
                    return id.Split('=')[1];
                }
                if (id.Contains("youtu.be")) { // Example: https://youtu.be/M80K51DosFo
                    return id.Split('/')[3];
                }

                return id;
            }
            return string.Empty;
        }
        catch (Exception) { }
        return string.Empty;
    }
    public static string GetYouTubeThumbnailURLFromVideoID(string id)
    {
        return string.Format("https://img.youtube.com/vi/{0}/0.jpg", id);
    }
    
    public static byte[] GetByteArray(Stream input)
    {
        using (MemoryStream ms = new MemoryStream()) {
            input.CopyTo(ms);
            return ms.ToArray();
        }
    }

public static bool CheckForInternetConnection(int timeoutMs = 10000, string url = null)
{
    try {
        if (string.IsNullOrWhiteSpace(url)) {
            switch (CultureInfo.CurrentUICulture.Name) {
                case "fa-IR": // Iran
                    url = "http://www.aparat.com";
                    break;
                case "zh-CN": // China
                    url = "http://www.baidu.com";
                    break;
                default:
                    url = "http://www.gstatic.com/generate_204";
                    break;
            }
        }

        var request = (HttpWebRequest)WebRequest.Create(url);
        request.KeepAlive = false;
        request.Timeout = timeoutMs;
        using (var response = (HttpWebResponse)request.GetResponse())
            return true;
    }
    catch {
        return false;
    }
}
	
	public static string GetDateAndTimeStringForFileName()
    {
        DateTime now = DateTime.Now;
        return string.Format("{0}.{1}.{2}@{3}_{4}_{5}-{6}", now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, now.Millisecond);
    }
    public static string GetFileVersion(string fileName)
    {
        try {
            string raw = FileVersionInfo.GetVersionInfo(fileName).FileVersion;
            if (!string.IsNullOrEmpty(raw)) {
                if (raw.Contains(",")) {
                    string raw2 = raw.Replace(',', '.');
                    if (raw2.Contains(" ")) {
                        return raw2.Replace(" ", "");
                    }
                    return raw2;
                }
                return raw;
            }
            return string.Empty;
        }
        catch (Exception) {
            return string.Empty;
        }
    }
    
    public static int GetWeekNumberOfMonth(DateTime date)
    {
        DateTime firstMonthDay = new DateTime(date.Year, date.Month, 1);
        DateTime firstMonthMonday = firstMonthDay.AddDays((DayOfWeek.Monday + 7 - firstMonthDay.DayOfWeek) % 7);
        if (firstMonthMonday > date) {
            firstMonthDay = firstMonthDay.AddMonths(-1);
            firstMonthMonday = firstMonthDay.AddDays((DayOfWeek.Monday + 7 - firstMonthDay.DayOfWeek) % 7);
        }
        return (date - firstMonthMonday).Days / 7 + 1;
    }
    
    /// <summary>
    /// Creates an MD5 Hash string from the given directory.
    /// </summary>
    /// <param name="folder">The directory the hash should be created from.</param>
    /// <param name="ignoredFiles">
    /// Specify files that should be ignored while creating the MD5 Hash.<br/>
    /// The file name will be <b>lowered</b> while checking, so you should add file names to this list that are <b>lowercase</b>.
    /// </param>
    /// <returns>A <see cref="AResult"/> object that contains information if the operation failed or not. Returns an MD5 Hash string if successful.</returns>
    public static AResult GetMD5StringFromFolder(string folder, List<string> ignoredFiles = null)
    {
        try {
            List<string> files = Directory.GetFiles(folder, "*.*", SearchOption.TopDirectoryOnly).OrderBy(p => p).ToList();
            using (MD5 md5 = MD5.Create()) {

                // Generate hash from all files in directory
                for (int i = 0; i < files.Count; i++) {
                    string file = files[i];

                    // There are files to be ignored
                    if (ignoredFiles != null) {
                        if (ignoredFiles.Contains(Path.GetFileName(file).ToLower()))
                            continue;
                    }

                    // Hash path
                    string realtivePath = file.Substring(folder.Length + 1);
                    byte[] pathBytes = Encoding.UTF8.GetBytes(realtivePath.ToLower());
                    md5.TransformBlock(pathBytes, 0, pathBytes.Length, pathBytes, 0);

                    // Hash contents
                    byte[] contentBytes = File.ReadAllBytes(file);
                    if (contentBytes == null) return new AResult(new ArgumentNullException("contentBytes was null."), null);

                    if (i == (files.Count - 1)) {
                        md5.TransformFinalBlock(contentBytes, 0, contentBytes.Length);
                    }
                    else {
                        md5.TransformBlock(contentBytes, 0, contentBytes.Length, contentBytes, 0);
                    }

                }

                return new AResult(null, BitConverter.ToString(md5.Hash).Replace("-", "").ToLower());
            }
        }
        catch (Exception ex) {
            return new AResult(ex, null);
        }
    }
    /// <summary>
    /// Creates an MD5 Hash string from the given file.
    /// </summary>
    /// <param name="file">The file the hash should be created from.</param>
    /// <returns>A <see cref="AResult"/> object that contains information if the operation failed or not. Returns an MD5 Hash string if successful.</returns>
    public static AResult GetMD5StringFromFile(string file)
    {
        try {
            using (MD5 md5 = MD5.Create()) {
                byte[] contentBytes = File.ReadAllBytes(file);
                if (contentBytes == null) return new AResult(new ArgumentNullException("contentBytes was null."), null);

                md5.TransformFinalBlock(contentBytes, 0, contentBytes.Length);

                return new AResult(null, BitConverter.ToString(md5.Hash).Replace("-", "").ToLower());
            }
        }
        catch (Exception ex) {
            return new AResult(ex, null);
        }
    }

    public static FileSize GetExactFileSize(long byteCount)
    {
        if (byteCount == 0)
            return new FileSize(0, FileSizes.Byte);

        long bytes = Math.Abs(byteCount);
        int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
        double num = Math.Round(bytes / Math.Pow(1024, place), 1);
        return new FileSize(Math.Sign(byteCount) * num, (FileSizes)place);
    }
    public static string GetExactFileSize2(long byteCount)
    {
        string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; // Longs run out around EB
        if (byteCount == 0)
            return "0" + suf[0];
        long bytes = Math.Abs(byteCount);
        int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
        double num = Math.Round(bytes / Math.Pow(1024, place), 1);
        return string.Format("{0} {1}", (Math.Sign(byteCount) * num).ToString(), suf[place]);
    }
    
        /// <summary>
    /// Takes a Screenshot of the Screen.
    /// </summary>
    /// <param name="saveTo">The path where the Screenshot will be saved to.</param>
    /// <param name="width">The width of the Screenshot.</param>
    /// <param name="height">The height of the Screenshot.</param>
    /// <param name="imageFormat">The <see cref="ImageFormat"/> the Screenshot should be saved as.</param>
    /// <returns>A <see cref="AResult"/> object that contains information if the operation failed or not. Returns true if the Screenshot was taken and saved successfully.</returns>
    public static AResult TakeScreenshot(string saveTo, int width, int height, ImageFormat imageFormat)
    {
        try {
            // Take Screenshot
            using (Bitmap b = new Bitmap(width, height)) {
                using (Graphics g = Graphics.FromImage(b)) {
                    g.CopyFromScreen(0, 0, 0, 0, b.Size);
                }

                b.Save(saveTo, imageFormat);
            }

            // Check if Screenshot was saved
            if (File.Exists(saveTo)) {
                return new AResult(null, true);
            }
            else {
                return new AResult(null, false);
            }
        }
        catch (Exception ex) {
            return new AResult(ex, null);
        }
    }
    #endregion

	#region Extensions
	/// <summary>
	/// Converts a <see cref="string"/> to a <see cref="SolidColorBrush"/>.
	/// </summary>
	/// <param name="HexColorString">The string representation of a HEXadecimal color string.</param>
	/// <returns>A new <see cref="SolidColorBrush"/> from the HEXadecimal color string.</returns>
	public static SolidColorBrush ToBrush(this string HexColorString)
	{
		return (SolidColorBrush)(new BrushConverter().ConvertFrom(HexColorString));
	}
	
	/// <summary>
	/// Converts a <see cref="string"/> to a <see cref="System.Windows.Media.Color"/>.
	/// </summary>
	/// <param name="HexColorString">The string representation of a HEXadecimal color string.</param>
	/// <returns>A new <see cref="System.Windows.Media.Color"/> from the HEXadecimal color string.</returns>
	public static System.Windows.Media.Color ToColor(this string HexColorString)
	{
		return (System.Windows.Media.Color)(new System.Windows.Media.ColorConverter().ConvertFrom(HexColorString));
	}
	#endregion

}
