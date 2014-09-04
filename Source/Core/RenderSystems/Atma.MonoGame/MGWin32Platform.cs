using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Atma.Graphics;

namespace Atma.MonoGame
{
    internal class MGWin32Platform : Platform
    {
        #region P/Invoke Declarations

        private enum WindowMessage
        {
            Create = 0x0001,
            Destroy = 0x0002,
            Move = 0x0003,
            Size = 0x0005,
            Activate = 0x0006,
            Close = 0x0010,

            GetMinMaxInfo = 0x0024,
            SysKeyDown = 0x0104,
            SysKeyUp = 0x0105,
            EnterSizeMove = 0x0231,
            ExitSizeMove = 0x0232
        }

        private enum ActivateState
        {
            InActive = 0,
            Active = 1,
            ClickActive = 2
        }

        private enum VirtualKeys
        {
            Shift = 0x10,
            Control = 0x11,
            Menu = 0x12
        }

        private struct Msg
        {
            public int hWnd;
            public int Message;
            public int wParam;
            public int lParam;
            public int time;
            public POINTAPI pt;
        }

        private struct POINTAPI
        {
            public int x;
            public int y;

            // Just to get rid of Warning CS0649.
            public POINTAPI(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        /// <summary>
        ///		PeekMessage option to remove the message from the queue after processing.
        /// </summary>
        private const int PM_REMOVE = 0x0001;

        private const string USER_DLL = "user32.dll";

        /// <summary>
        ///		The PeekMessage function dispatches incoming sent messages, checks the thread message 
        ///		queue for a posted message, and retrieves the message (if any exist).
        /// </summary>
        /// <param name="msg">A <see cref="Msg"/> structure that receives message information.</param>
        /// <param name="handle"></param>
        /// <param name="msgFilterMin"></param>
        /// <param name="msgFilterMax"></param>
        /// <param name="removeMsg"></param>
        [DllImport(USER_DLL)]
        private static extern int PeekMessage(out Msg msg, IntPtr handle, int msgFilterMin, int msgFilterMax, int removeMsg);

        /// <summary>
        ///		The TranslateMessage function translates virtual-key messages into character messages.
        /// </summary>
        /// <param name="msg">
        ///		an MSG structure that contains message information retrieved from the calling thread's message queue 
        ///		by using the GetMessage or <see cref="PeekMessage"/> function.
        /// </param>
        [DllImport(USER_DLL)]
        private static extern void TranslateMessage(ref Msg msg);

        /// <summary>
        ///		The DispatchMessage function dispatches a message to a window procedure.
        /// </summary>
        /// <param name="msg">A <see cref="Msg"/> structure containing the message.</param>
        [DllImport(USER_DLL)]
        private static extern void DispatchMessage(ref Msg msg);

        #endregion P/Invoke Declarations

        #region Fields and Properties

        #endregion Fields and Properties

        public static MGWin32Platform instance { get { return (MGWin32Platform)_instance; } }

        #region Construction and Destruction

        static MGWin32Platform()
        {
            if (_instance != null)
                throw new Exception("Platform was already initialized");

            _instance = new MGWin32Platform();
        }


        internal static void Initialize()
        {

        }

        protected override void processEvents()
        {
            Msg msg;

            // pump those events!
            while (!(PeekMessage(out msg, IntPtr.Zero, 0, 0, PM_REMOVE) == 0))
            {
                TranslateMessage(ref msg);
                DispatchMessage(ref msg);
            }
        }

        public Form CreateForm(RenderWindow window)
        {
            return new RenderForm(window);
        }

        #endregion Construction and Destruction

        #region Methods

        /// <summary>
        /// Internal winProc (RenderWindow's use this when creating the Win32 Window)
        /// </summary>
        /// <param name="m"></param>
        public static bool WndProc(RenderWindow win, ref Message m)
        {
            switch ((WindowMessage)m.Msg)
            {
                case WindowMessage.Activate:
                    {
                        bool active = ((ActivateState)(m.WParam.ToInt32() & 0xFFFF)) != ActivateState.InActive;
                        win.IsActive = active;
                        _instance.WindowFocusChange(win, active);
                        break;
                    }
                case WindowMessage.SysKeyDown:
                    switch ((VirtualKeys)m.WParam)
                    {
                        case VirtualKeys.Control:
                        case VirtualKeys.Shift:
                        case VirtualKeys.Menu: //ALT
                            //return true to bypass defProc and signal we processed the message
                            return true;
                    }
                    break;
                case WindowMessage.SysKeyUp:
                    switch ((VirtualKeys)m.WParam)
                    {
                        case VirtualKeys.Control:
                        case VirtualKeys.Shift:
                        case VirtualKeys.Menu: //ALT
                            //return true to bypass defProc and signal we processed the message
                            return true;
                    }
                    break;
                case WindowMessage.EnterSizeMove:
                    //log->logMessage("WM_ENTERSIZEMOVE");
                    break;
                case WindowMessage.ExitSizeMove:
                    //log->logMessage("WM_EXITSIZEMOVE");
                    break;
                case WindowMessage.Move:
                    //log->logMessage("WM_MOVE");
                    _instance.WindowMoved(win);
                    break;
                case WindowMessage.Size:
                    //log->logMessage("WM_SIZE");
                    _instance.WindowResized(win);
                    break;
                case WindowMessage.GetMinMaxInfo:
                    // Prevent the window from going smaller than some minimum size
                    //((MINMAXINFO*)lParam)->ptMinTrackSize.x = 100;
                    //((MINMAXINFO*)lParam)->ptMinTrackSize.y = 100;
                    break;
                case WindowMessage.Close:
                    //log->logMessage("WM_CLOSE");
                    _instance.WindowClosed(win);
                    break;
            }
            return false;
        }

        #endregion Methods
    }

}
