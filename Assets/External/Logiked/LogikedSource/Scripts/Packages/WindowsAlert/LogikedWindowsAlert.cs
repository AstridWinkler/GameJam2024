using System;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

namespace  logiked.source.utilities
{

    /// <summary>
    /// Permet d'invoker un popup Windows natif avec des options (Oui/Non/Annuler...) <br/>
    /// Appeler la méthode <see cref="LogikedWindowsAlert.Message_Box"/>
    /// </summary>
    public class LogikedWindowsAlert
    {


#if UNITY_STANDALONE_WIN

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern System.IntPtr GetActiveWindow();
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern int MessageBox(IntPtr hwnd, String lpText, String lpCaption, uint uType);

        public static System.IntPtr GetWindowHandle()
        {
            return GetActiveWindow();
        }

        public enum WindowsAlertType { AbortRetryIgnore, CancelTryContinue, Help, OK, OkCancel , RetryCancel , YesNo , YesNoCancel };
        public enum WindowsAlertResult { OK = 1, CANCEL = 2, ABORT= 3, RETRY =4, IGNORE = 5, HELP, YES=6, NO=7,  TRY_AGAIN=10 , Unkown};

        /// <summary>
        /// Shows Message Box with button type.
        /// </summary>
        /// <param name="content">Main alert text / content.</param>
        /// <param name="title">Message box title.</param>
        /// <param name="type">Type of message / icon to use - </param>
        /// <remarks>types: AbortRetryIgnore, CancelTryContinue, Help, OK, OkCancel, RetryCancel, YesNo, YesNoCancel</remarks>
        /// <example>Message_Box("My Text Message", "My Title", "OK");</example>
        /// <returns>OK,CANCEL,ABORT,RETRY, IGNORE, YES, NO, TRY AGAIN</returns>
        public static WindowsAlertResult Message_Box(string content, string title, WindowsAlertType type)
        {
            try
            {
                string DialogResult = string.Empty;
                uint MB_ABORTRETRYIGNORE = (uint)(0x00000002L | 0x00000010L);
                uint MB_CANCELTRYCONTINUE = (uint)(0x00000006L | 0x00000030L);
                uint MB_HELP = (uint)(0x00004000L | 0x00000040L);
                uint MB_OK = (uint)(0x00000000L | 0x00000040L);
                uint MB_OKCANCEL = (uint)(0x00000001L | 0x00000040L);
                uint MB_RETRYCANCEL = (uint)0x00000005L;
                uint MB_YESNO = (uint)(0x00000004L | 0x00000040L);
                uint MB_YESNOCANCEL = (uint)(0x00000003L | 0x00000040L);
                int intresult=0;

                WindowsAlertResult result = LogikedWindowsAlert.WindowsAlertResult.OK;

                switch (type)
                {
                    case  WindowsAlertType.AbortRetryIgnore:
                        intresult = MessageBox(GetWindowHandle(), content, title, MB_ABORTRETRYIGNORE);
                        break;
                    case  WindowsAlertType.CancelTryContinue:
                        intresult = MessageBox(GetWindowHandle(), content, title, MB_CANCELTRYCONTINUE);
                        break;
                    case  WindowsAlertType.Help:
                        intresult = MessageBox(GetWindowHandle(), content, title, MB_HELP);
                        break;
                    case  WindowsAlertType.OK:
                        intresult = MessageBox(GetWindowHandle(), content, title, MB_OK);
                        break;
                    case  WindowsAlertType.OkCancel:
                        intresult = MessageBox(GetWindowHandle(), content, title, MB_OKCANCEL);
                        break;
                    case  WindowsAlertType.RetryCancel:
                        intresult = MessageBox(GetWindowHandle(), content, title, MB_RETRYCANCEL);
                        break;
                    case  WindowsAlertType.YesNo:
                        intresult = MessageBox(GetWindowHandle(), content, title, MB_YESNO);
                        break;
                    case  WindowsAlertType.YesNoCancel:
                        intresult = MessageBox(GetWindowHandle(), content, title, MB_YESNOCANCEL);
                        break;
    
                }

                switch (intresult)
                {
                    
                    case 1:
                        result = WindowsAlertResult.OK;
                        break;
                    case 2:
                        result = WindowsAlertResult.CANCEL;
                        break;
                    case 3:
                        result = WindowsAlertResult.ABORT;
                        break;
                    case 4:
                        result = WindowsAlertResult.RETRY;
                        break;
                    case 5:
                        result = WindowsAlertResult.IGNORE;
                        break;
                    case 6:
                        result = WindowsAlertResult.YES;
                        break;
                    case 7:
                        result = WindowsAlertResult.NO;
                        break;
                    case 10:
                        result = WindowsAlertResult.TRY_AGAIN;
                        break;
                    default:
                        result = WindowsAlertResult.OK;
                        break;

                }
                return result;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return LogikedWindowsAlert.WindowsAlertResult.Unkown;
            }
        }
    }
#endif


}

