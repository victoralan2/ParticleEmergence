using System.Runtime.InteropServices;
using System.Text;

public class WindowFocus {
    [DllImport("user32.dll")]
    static extern int GetForegroundWindow();

    [DllImport("user32.dll")]
    static extern int GetClassName(int hWnd, StringBuilder lpClassName, int nMaxCount);

    public static bool IsDesktopActive() {
        const int maxChars = 256;
        int handle = 0;
        StringBuilder className = new StringBuilder(maxChars);

        handle = GetForegroundWindow();

        if (GetClassName(handle, className, maxChars) > 0) {
            string cName = className.ToString();
            if (cName == "Progman" || cName == "WorkerW") {
                return true;
            } else {
                return false;
            }
        }
        return false;
    }
}