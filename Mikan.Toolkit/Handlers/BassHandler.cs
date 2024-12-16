using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagedBass;

namespace Mikan.Toolkit.Handlers
{
    internal static class BassHandler
    {
        private static bool _isBASSInitialized = false;

        public static bool IsBASSInitialized => _isBASSInitialized;
        public static void CheckBassInit()
        {
            if (!Bass.Init())
            {
                if (Bass.LastError == Errors.Already)
                {
                    Debug.WriteLine($"[ManagedBass] BASS already initialized");
                    return;
                }

                Debug.WriteLine($"[ManagedBass] Failed to initialize BASS: {Bass.LastError}");
            }

            Debug.WriteLine($"[ManagedBass] BASS initialized successfully");
            _isBASSInitialized = true;

            AppDomain.CurrentDomain.ProcessExit += (s, e) =>
            {
                EndBass();
            };
        }

        public static void EndBass()
        {
            if (!_isBASSInitialized)
                return;

            Bass.Free();
            _isBASSInitialized = false;
            Debug.WriteLine($"[ManagedBass] BASS resources were freed");
        }
    }
}
