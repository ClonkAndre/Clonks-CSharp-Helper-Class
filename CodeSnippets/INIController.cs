/// <summary>
/// A basic ini reader/writer using kernel32 dll imports.
/// </summary>
public class INIController {

    #region DllImports
    [DllImport("kernel32.dll", EntryPoint = "GetPrivateProfileStringA", CharSet = CharSet.Ansi, SetLastError = true)]
    private static extern int GetPrivateProfileString([MarshalAs(UnmanagedType.VBByRefStr)] ref string lpApplicationName, [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpKeyName, [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpDefault, [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpReturnedString, int nSize, [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpFileName);

    [DllImport("kernel32.dll", EntryPoint = "WritePrivateProfileStringA", CharSet = CharSet.Ansi, SetLastError = true)]
    private static extern int WritePrivateProfileString([MarshalAs(UnmanagedType.VBByRefStr)] ref string lpApplicationName, [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpKeyName, [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpString, [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpFileName);
    #endregion

    #region Functions
    public static string ReadValueFromFile(string strSection, string strKey, string strDefault, string strFile)
    {
        string lpReturnedString = Strings.Space(1024);
        int privateProfileString = GetPrivateProfileString(ref strSection, ref strKey, ref strDefault, ref lpReturnedString, lpReturnedString.Length, ref strFile);
        return lpReturnedString.Substring(0, privateProfileString);
    }
    public static bool WriteValueToFile(string strSection, string strKey, string strValue, string strFile)
    {
        return WritePrivateProfileString(ref strSection, ref strKey, ref strValue, ref strFile) != 0;
    }
    #endregion

}