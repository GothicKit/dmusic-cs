using System;
using System.Runtime.InteropServices;
using DirectMusic.Util;

namespace DirectMusic
{
    public class RandomNumberGenerator
    {
        private static GCHandle? _handler;
        private static readonly Native.DmRng NativeHandler = _nativeCallbackHandler;

        /// <summary>
        /// 
        /// </summary>
        public delegate int Callback();

        /// <summary>
        /// 
        /// </summary>
        public static void Set(Callback cb)
        {
            var handler = GCHandle.Alloc(cb);
            Native.Dm_setRandomNumberGenerator(NativeHandler, GCHandle.ToIntPtr(handler));

            _handler?.Free();
            _handler = handler;
        }

        [MonoPInvokeCallback]
        private static uint _nativeCallbackHandler(IntPtr ctx)
        {
            var gcHandle = GCHandle.FromIntPtr(ctx);
            var cb = (Callback)gcHandle.Target;
            return (uint) (cb() % uint.MaxValue);
        }
    }
}