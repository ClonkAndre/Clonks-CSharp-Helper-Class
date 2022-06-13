// Clonk's Helper Class
// https://github.com/ClonkAndre/Clonks-CSharp-Helper-Class
// Last updated: 6/13/2022

#region Imports
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.VisualBasic;
using Microsoft.Win32;
#endregion

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
    /// String Compression stuff.
    /// </summary>
    public class StringCompression {
        /// <summary>
        /// Compresses a string and returns a deflate compressed, Base64 encoded string.
        /// </summary>
        /// <param name="uncompressedString">String to compress</param>
        public static string Compress(string uncompressedString, string returnStringOnError = "")
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
        public static string Decompress(string compressedString, string returnStringOnError = "")
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
    #endregion

    #region Functions
    public static byte[] GetByteArray(Stream input)
    {
        using (MemoryStream ms = new MemoryStream()) {
            input.CopyTo(ms);
            return ms.ToArray();
        }
    }

    public static bool IsUserConnectedToTheInternet()
    {
        try {
            using (Ping ping = new Ping()) {
                PingReply reply = ping.Send("8.8.8.8");
                if (reply != null && reply.Status == IPStatus.Success) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        catch (Exception) {
            return false;
        }
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

    public static AResult GetMD5StringFromFolder(string folder)
    {
        try {
            List<string> files = Directory.GetFiles(folder, "*.*", SearchOption.TopDirectoryOnly).OrderBy(p => p).ToList();
            using (MD5 md5 = MD5.Create()) {

                // Generate hash from all files in directory
                for (int i = 0; i < files.Count; i++) {
                    string file = files[i];

                    if (Path.GetFileName(file).ToLower() == "installscript.vdf")
                        continue;

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

    public static double GetVRAM()
    {
        try {
            using (ManagementClass c = new ManagementClass("Win32_VideoController")) {
                foreach (ManagementObject o in c.GetInstances()) {
                    FileSize size = GetExactFileSize(long.Parse(o["AdapterRam"].ToString()));
                    return size.Size;
                }
            }
        }
        catch (Exception) { }
        return 1024;
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
    #endregion

}
