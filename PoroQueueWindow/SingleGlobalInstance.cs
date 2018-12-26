using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;

namespace PoroQueueWindow
{
    class SingleGlobalInstance : IDisposable
    {
        public bool HasHandle = false;
        private Mutex Mutex;

        private void InitMutex()
        {
            string GUID = ((GuidAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(GuidAttribute), false).GetValue(0)).Value.ToString();
            string MutexID = string.Format("Global\\{{{0}}}", GUID);
            Mutex = new Mutex(false, MutexID);

            var AllowEveryone = new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), MutexRights.FullControl, AccessControlType.Allow);
            var SecuritySettings = new MutexSecurity();
            SecuritySettings.AddAccessRule(AllowEveryone);
            Mutex.SetAccessControl(SecuritySettings);
        }

        public SingleGlobalInstance(int TimeOut)
        {
            InitMutex();
            try
            {
                HasHandle = Mutex.WaitOne(TimeOut <= 0 ? Timeout.Infinite : TimeOut, false);

                if (!HasHandle)
                    throw new TimeoutException("Timeout waiting for exclusive access on SingleInstance");
            }
            catch (AbandonedMutexException)
            {
                HasHandle = true;
            }
        }

        public void Dispose()
        {
            if (Mutex != null)
            {
                if (HasHandle)
                    Mutex.ReleaseMutex();
                Mutex.Dispose();
            }
        }
    }
}
