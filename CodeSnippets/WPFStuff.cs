/// <summary>
/// Windows Presentation Foundation (WPF) stuff.
/// <para>Updated: <b>5/12/2022</b></para>
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
    #endregion

}