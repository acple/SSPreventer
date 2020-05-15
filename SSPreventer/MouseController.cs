using System;
using System.Runtime.InteropServices;

namespace SSPreventer
{
    public class MouseController
    {
        [Flags]
        private enum MouseEvent : uint
        {
            Move = 0x0001,

            Absolute = 0x8000,
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MouseInput
        {
            public int dx;

            public int dy;

            public int mouseData;

            public MouseEvent dwFlags;

            public int time;

            public IntPtr dwExtraInfo;
        }

        private enum InputType : uint
        {
            Mouse = 0,

            Keyboard = 1,

            Hardware = 2,
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Input
        {
            public InputType type;

            public MouseInput input;
        }

        public void MoveRelative(int x, int y)
            => SendMouseInput(new MouseInput { dx = x, dy = y, dwFlags = MouseEvent.Move });

        public void MoveAbsolute(ushort x, ushort y)
            => SendMouseInput(new MouseInput { dx = x, dy = y, dwFlags = MouseEvent.Absolute | MouseEvent.Move });

        private static void SendMouseInput(MouseInput input)
        {
            var inputs = new[]
            {
                new Input { type = InputType.Mouse, input = input }
            };

            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(Input)));
        }

        // https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-sendinput
        [DllImport("user32.dll")]
        private static extern uint SendInput(uint cInputs, Input[] pInputs, int cbSize);
    }
}
