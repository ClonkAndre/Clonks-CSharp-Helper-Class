/// <summary>
/// Windows Presentation Foundation (WPF) stuff.
/// <para>Updated: <b>7/21/2022</b></para>
/// </summary>
public class WPFStuff {

    #region Method
    public static void GetLogicalChildCollection<T>(DependencyObject parent, List<T> logicalCollection) where T : DependencyObject
    {
        IEnumerable children = LogicalTreeHelper.GetChildren(parent);
        foreach (object child in children) {
            if (child is DependencyObject) {
                DependencyObject depChild = child as DependencyObject;
                if (child is T) {
                    logicalCollection.Add(child as T);
                }
                GetLogicalChildCollection(depChild, logicalCollection);
            }
        }
    }
    #endregion

    #region Functions
    public static List<T> GetLogicalChildCollection<T>(DependencyObject parent) where T : DependencyObject
    {
        List<T> logicalCollection = new List<T>();
        GetLogicalChildCollection(parent, logicalCollection);
        return logicalCollection;
    }
    public static bool IsWindowOpen<T>(string name = "") where T : Window
    {
        return string.IsNullOrEmpty(name)
           ? Application.Current.Windows.OfType<T>().Any()
           : Application.Current.Windows.OfType<T>().Any(w => w.Name.Equals(name));
    }
    public static Window GetOpenedWindow<T>(string name = "") where T : Window
    {
        return string.IsNullOrEmpty(name)
           ? Application.Current.Windows.OfType<T>().FirstOrDefault()
           : Application.Current.Windows.OfType<T>().FirstOrDefault(w => w.Name.Equals(name));
    }
	
    /// <summary>
    /// Caches the image into memory which prevents the image file from being locked.
    /// </summary>
    /// <param name="uri">The uri that leads to the file path.</param>
    /// <param name="freeze">If the image should be frozen or not.</param>
    /// <returns>The loaded <see cref="BitmapImage"/>. Returns null on error.</returns>
    public static BitmapImage LoadBitmapImageToMemory(Uri uri, bool freeze = false)
    {
        try {
            BitmapImage bmi = new BitmapImage();
            bmi.BeginInit();
            bmi.UriSource = uri;
            bmi.CacheOption = BitmapCacheOption.OnLoad;
            bmi.EndInit();
            if (freeze) bmi.Freeze();
            return bmi;
        }
        catch (Exception) {
            return null;
        }
    }
    #endregion
			
		#region Classes

    /// <summary>
    /// Class that allows a WPF <see cref="System.Windows.Window"/> to be on top of every opened Window. Even games that run in fullscreen mode.
    /// </summary>
    public class WindowSinker {

        #region Variables
        private const UInt32 SWP_NOSIZE = 0x0001;
        private const UInt32 SWP_NOMOVE = 0x0002;
        private const UInt32 SWP_NOACTIVATE = 0x0010;
        private const UInt32 SWP_NOZORDER = 0x0004;
        private const int WM_ACTIVATEAPP = 0x001C;
        private const int WM_ACTIVATE = 0x0006;
        private const int WM_SETFOCUS = 0x0007;
        private const int WM_WINDOWPOSCHANGING = 0x0046;

        private static readonly IntPtr HWND_BOTTOM = new IntPtr(0);

        private Window Window = null;
        #endregion

        #region Constructor
        public WindowSinker(Window Window)
        {
            this.Window = Window;
        }
        #endregion

        #region Methods

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        static extern IntPtr DeferWindowPos(IntPtr hWinPosInfo, IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        static extern IntPtr BeginDeferWindowPos(int nNumWindows);

        [DllImport("user32.dll")]
        static extern bool EndDeferWindowPos(IntPtr hWinPosInfo);

        void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var Handle = (new WindowInteropHelper(Window)).Handle;

	    if (Handle == IntPtr.Zero)
		return;
		
            var Source = HwndSource.FromHwnd(Handle);
            Source.RemoveHook(new HwndSourceHook(WndProc));
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            var Hwnd = new WindowInteropHelper(Window).Handle;
            SetWindowPos(Hwnd, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE);

            var Handle = (new WindowInteropHelper(Window)).Handle;

            var Source = HwndSource.FromHwnd(Handle);
            Source.AddHook(new HwndSourceHook(WndProc));
        }

        IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_SETFOCUS) {
                hWnd = new WindowInteropHelper(Window).Handle;
                SetWindowPos(hWnd, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE);
                handled = true;
            }
            return IntPtr.Zero;
        }

        public void Sink()
        {
            Window.Loaded += OnLoaded;
            Window.Closing += OnClosing;
        }

        public void Unsink()
        {
            Window.Loaded -= OnLoaded;
            Window.Closing -= OnClosing;
        }

        #endregion

    }

    #endregion
			
}
