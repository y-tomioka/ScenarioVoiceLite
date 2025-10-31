using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;

namespace STVoice
{
    public static class ATControl
    {
        /*
        public static void changeOntei(ref byte[] wav, int ontei)
        {
            if(ontei == 100)
            {
                return;
            }
            double onteiat = (double)ontei * 0.01;

            // 音程変更
            double samplingRate = BitConverter.ToUInt32(wav, 24);
            double dataRate = BitConverter.ToUInt32(wav, 28);
            samplingRate *= onteiat;
            dataRate *= onteiat;
            byte[] samplingRateBytes = BitConverter.GetBytes((uint)samplingRate);
            byte[] dataRateBytes = BitConverter.GetBytes((uint)dataRate);
            Array.Copy(samplingRateBytes, 0, wav, 24, 4);
            Array.Copy(dataRateBytes, 0, wav, 28, 4);
        }
        */
        
        public static void changeOntei(ref byte[] wav, int ontei, bool snapToStandard = false, double snapTolPercent = 0.5)
        {
            if (ontei == 100) return;

            double ratio = ontei * 0.01;

            // 典型的な PCM WAV 前提: [24..27]=SampleRate, [28..31]=ByteRate, [32..33]=BlockAlign
            // ※AquesTalkの出力が素直なPCMならこの位置でOK
            uint origSr = BitConverter.ToUInt32(wav, 24);
            ushort blockAlign = BitConverter.ToUInt16(wav, 32);

            uint newSr = (uint)Math.Round(origSr * ratio);

            if (snapToStandard)
            {
                // "とても近い時だけ"標準SRへ寄せる（既定±0.5%）
                uint[] std = { 8000, 11025, 12000, 16000, 22050, 24000, 32000, 44100, 48000 };
                uint snapped = Nearest(newSr, std);
                double diffPct = Math.Abs((double)snapped - newSr) / newSr * 100.0;
                if (diffPct <= snapTolPercent) newSr = snapped;
            }

            uint newByteRate = (uint)(newSr * (uint)blockAlign);

            Array.Copy(BitConverter.GetBytes(newSr), 0, wav, 24, 4);
            Array.Copy(BitConverter.GetBytes(newByteRate), 0, wav, 28, 4);

            static uint Nearest(uint v, uint[] cs)
            {
                uint best = cs[0]; uint bestDiff = (uint)Math.Abs((int)v - (int)best);
                foreach (var c in cs)
                {
                    uint d = (uint)Math.Abs((int)v - (int)c);
                    if (d < bestDiff) { bestDiff = d; best = c; }
                }
                return best;
            }
        }
        
    }


    public static class ATF1
    {
        const string dllPath = ".\\AquesTalk\\bin\\f1\\AquesTalk.dll";

        [DllImport(dllPath)]
        private static extern IntPtr AquesTalk_Synthe(string koe, int iSpeed, ref int size);

        [DllImport(dllPath)]
        private static extern void AquesTalk_FreeWave(IntPtr wavPtr);

        public static bool start(string path, string text, int speed, int ontei = 100)
        {
            int iSpeed = speed;
            string koe = text;

            int size = 0;
            IntPtr wavPtr = IntPtr.Zero;
            try
            {
                wavPtr = AquesTalk_Synthe(koe, iSpeed, ref size);

                if (wavPtr == IntPtr.Zero)
                {
                    return false ;
                }
            }
            catch (Exception /*exception*/)
            {
                 return false;
            }

            byte[] wav = new byte[size];
            Marshal.Copy(wavPtr, wav, 0, size);

            AquesTalk_FreeWave(wavPtr);

            ATControl.changeOntei(ref wav, ontei);

            var ms = new MemoryStream(wav);
            using (var fileStream = System.IO.File.Create(path))
            {
                ms.CopyTo(fileStream);
                ms.Flush();
            }
            return true;
        }
    }

    public static class ATF2
    {
        const string dllPath = ".\\AquesTalk\\bin\\f2\\AquesTalk.dll";

        [DllImport(dllPath)]
        private static extern IntPtr AquesTalk_Synthe(string koe, int iSpeed, ref int size);

        [DllImport(dllPath)]
        private static extern void AquesTalk_FreeWave(IntPtr wavPtr);

        public static bool start(string path, string text, int speed, int ontei = 100)
        {
            int iSpeed = speed;
            string koe = text;

            int size = 0;
            IntPtr wavPtr = IntPtr.Zero;
            try
            {
                wavPtr = AquesTalk_Synthe(koe, iSpeed, ref size);

                if (wavPtr == IntPtr.Zero)
                {
                    return false;
                }
            }
            catch (Exception /*exception*/)
            {
                return false;
            }

            byte[] wav = new byte[size];
            Marshal.Copy(wavPtr, wav, 0, size);

            AquesTalk_FreeWave(wavPtr);

            ATControl.changeOntei(ref wav, ontei);

            var ms = new MemoryStream(wav);
            using (var fileStream = System.IO.File.Create(path))
            {
                ms.CopyTo(fileStream);
                ms.Flush();
            }
            return true;
        }
    }

    public static class ATM1
    {
        const string dllPath = ".\\AquesTalk\\bin\\m1\\AquesTalk.dll";

        [DllImport(dllPath)]
        private static extern IntPtr AquesTalk_Synthe(string koe, int iSpeed, ref int size);

        [DllImport(dllPath)]
        private static extern void AquesTalk_FreeWave(IntPtr wavPtr);

        public static bool start(string path, string text, int speed, int ontei = 100)
        {
            int iSpeed = speed;
            string koe = text;

            int size = 0;
            IntPtr wavPtr = IntPtr.Zero;
            try
            {
                wavPtr = AquesTalk_Synthe(koe, iSpeed, ref size);

                if (wavPtr == IntPtr.Zero)
                {
                    return false;
                }
            }
            catch (Exception /*exception*/)
            {
                return false;
            }

            byte[] wav = new byte[size];
            Marshal.Copy(wavPtr, wav, 0, size);

            AquesTalk_FreeWave(wavPtr);

            ATControl.changeOntei(ref wav, ontei);

            var ms = new MemoryStream(wav);
            using (var fileStream = System.IO.File.Create(path))
            {
                ms.CopyTo(fileStream);
                ms.Flush();
            }
            return true;
        }
    }

    public static class ATM2
    {
        const string dllPath = ".\\AquesTalk\\bin\\m2\\AquesTalk.dll";

        [DllImport(dllPath)]
        private static extern IntPtr AquesTalk_Synthe(string koe, int iSpeed, ref int size);

        [DllImport(dllPath)]
        private static extern void AquesTalk_FreeWave(IntPtr wavPtr);

        public static bool start(string path, string text, int speed, int ontei = 100)
        {
            int iSpeed = speed;
            string koe = text;

            int size = 0;
            IntPtr wavPtr = IntPtr.Zero;
            try
            {
                wavPtr = AquesTalk_Synthe(koe, iSpeed, ref size);

                if (wavPtr == IntPtr.Zero)
                {
                    return false;
                }
            }
            catch (Exception /*exception*/)
            {
                return false;
            }

            byte[] wav = new byte[size];
            Marshal.Copy(wavPtr, wav, 0, size);

            AquesTalk_FreeWave(wavPtr);

            ATControl.changeOntei(ref wav, ontei);

            var ms = new MemoryStream(wav);
            using (var fileStream = System.IO.File.Create(path))
            {
                ms.CopyTo(fileStream);
                ms.Flush();
            }
            return true;
        }
    }

    public static class ATDVD
    {
        const string dllPath = ".\\AquesTalk\\bin\\dvd\\AquesTalk.dll";

        [DllImport(dllPath)]
        private static extern IntPtr AquesTalk_Synthe(string koe, int iSpeed, ref int size);

        [DllImport(dllPath)]
        private static extern void AquesTalk_FreeWave(IntPtr wavPtr);

        public static bool start(string path, string text, int speed, int ontei = 100)
        {
            int iSpeed = speed;
            string koe = text;

            int size = 0;
            IntPtr wavPtr = IntPtr.Zero;
            try
            {
                wavPtr = AquesTalk_Synthe(koe, iSpeed, ref size);

                if (wavPtr == IntPtr.Zero)
                {
                    return false;
                }
            }
            catch (Exception /*exception*/)
            {
                return false;
            }

            byte[] wav = new byte[size];
            Marshal.Copy(wavPtr, wav, 0, size);

            AquesTalk_FreeWave(wavPtr);

            ATControl.changeOntei(ref wav, ontei);

            var ms = new MemoryStream(wav);
            using (var fileStream = System.IO.File.Create(path))
            {
                ms.CopyTo(fileStream);
                ms.Flush();
            }
            return true;
        }
    }

    public static class ATIMD1
    {
        const string dllPath = ".\\AquesTalk\\bin\\imd1\\AquesTalk.dll";

        [DllImport(dllPath)]
        private static extern IntPtr AquesTalk_Synthe(string koe, int iSpeed, ref int size);

        [DllImport(dllPath)]
        private static extern void AquesTalk_FreeWave(IntPtr wavPtr);

        public static bool start(string path, string text, int speed, int ontei = 100)
        {
            int iSpeed = speed;
            string koe = text;

            int size = 0;
            IntPtr wavPtr = IntPtr.Zero;
            try
            {
                wavPtr = AquesTalk_Synthe(koe, iSpeed, ref size);

                if (wavPtr == IntPtr.Zero)
                {
                    return false;
                }
            }
            catch (Exception /*exception*/)
            {
                return false;
            }

            byte[] wav = new byte[size];
            Marshal.Copy(wavPtr, wav, 0, size);

            AquesTalk_FreeWave(wavPtr);

            ATControl.changeOntei(ref wav, ontei);

            var ms = new MemoryStream(wav);
            using (var fileStream = System.IO.File.Create(path))
            {
                ms.CopyTo(fileStream);
                ms.Flush();
            }
            return true;
        }
    }

    public static class ATJGR
    {
        const string dllPath = ".\\AquesTalk\\bin\\jgr\\AquesTalk.dll";

        [DllImport(dllPath)]
        private static extern IntPtr AquesTalk_Synthe(string koe, int iSpeed, ref int size);

        [DllImport(dllPath)]
        private static extern void AquesTalk_FreeWave(IntPtr wavPtr);

        public static bool start(string path, string text, int speed, int ontei = 100)
        {
            int iSpeed = speed;
            string koe = text;

            int size = 0;
            IntPtr wavPtr = IntPtr.Zero;
            try
            {
                wavPtr = AquesTalk_Synthe(koe, iSpeed, ref size);

                if (wavPtr == IntPtr.Zero)
                {
                    return false;
                }
            }
            catch (Exception /*exception*/)
            {
                return false;
            }

            byte[] wav = new byte[size];
            Marshal.Copy(wavPtr, wav, 0, size);

            AquesTalk_FreeWave(wavPtr);

            ATControl.changeOntei(ref wav, ontei);

            var ms = new MemoryStream(wav);
            using (var fileStream = System.IO.File.Create(path))
            {
                ms.CopyTo(fileStream);
                ms.Flush();
            }
            return true;
        }
    }

    public static class ATR1
    {
        const string dllPath = ".\\AquesTalk\\bin\\r1\\AquesTalk.dll";

        [DllImport(dllPath)]
        private static extern IntPtr AquesTalk_Synthe(string koe, int iSpeed, ref int size);

        [DllImport(dllPath)]
        private static extern void AquesTalk_FreeWave(IntPtr wavPtr);

        public static bool start(string path, string text, int speed, int ontei = 100)
        {
            int iSpeed = speed;
            string koe = text;

            int size = 0;
            IntPtr wavPtr = IntPtr.Zero;
            try
            {
                wavPtr = AquesTalk_Synthe(koe, iSpeed, ref size);

                if (wavPtr == IntPtr.Zero)
                {
                    return false;
                }
            }
            catch (Exception /*exception*/)
            {
                return false;
            }

            byte[] wav = new byte[size];
            Marshal.Copy(wavPtr, wav, 0, size);

            AquesTalk_FreeWave(wavPtr);

            ATControl.changeOntei(ref wav, ontei);

            var ms = new MemoryStream(wav);
            using (var fileStream = System.IO.File.Create(path))
            {
                ms.CopyTo(fileStream);
                ms.Flush();
            }
            return true;
        }
    }
}
