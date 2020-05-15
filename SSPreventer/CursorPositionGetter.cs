using System.Runtime.InteropServices;

namespace SSPreventer
{
    public class CursorPositionGetter
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct Point
        {
            public int X;

            public int Y;
        }

        public Position GetCursorPosition()
            => GetCursorPos(out var lpPoint)
                ? new Position(lpPoint.X, lpPoint.Y)
                : new Position(default, default);

        // https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getcursorpos
        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out Point lpPoint);
    }
}
