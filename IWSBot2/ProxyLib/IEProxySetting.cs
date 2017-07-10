using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ProxyLib
{
    public struct Struct_INTERNET_PROXY_INFO
    {
        public int dwAccessType;
        public IntPtr proxy;
        public IntPtr proxyBypass;
    }; 

    public class IEProxySetting
    {

        const int INTERNET_OPTION_PROXY = 38;
        const int INTERNET_OPEN_TYPE_PROXY = 3;
        const int INTERNET_OPEN_TYPE_DIRECT = 1;

        [DllImport("wininet.dll", SetLastError = true)]
        private static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int lpdwBufferLength);
        public void RefreshIESettings(string strProxy)//strProxy为代理IP:端口 
        {
            Struct_INTERNET_PROXY_INFO struct_IPI;
            // Filling in structure 
            struct_IPI.dwAccessType = INTERNET_OPEN_TYPE_PROXY;
            struct_IPI.proxy = Marshal.StringToHGlobalAnsi(strProxy);
            struct_IPI.proxyBypass = Marshal.StringToHGlobalAnsi("local");
            // Allocating memory 
            IntPtr intptrStruct = Marshal.AllocCoTaskMem(Marshal.SizeOf(struct_IPI));
            if (string.IsNullOrEmpty(strProxy) || strProxy.Trim().Length == 0)
            {
                strProxy = string.Empty;
                struct_IPI.dwAccessType = INTERNET_OPEN_TYPE_DIRECT;
            }
            // Converting structure to IntPtr 
            Marshal.StructureToPtr(struct_IPI, intptrStruct, true);
            bool iReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_PROXY, intptrStruct, Marshal.SizeOf(struct_IPI));
        }

        public void DisableIEProxy()
        {
            RefreshIESettings(string.Empty);
        } 
    }
}
