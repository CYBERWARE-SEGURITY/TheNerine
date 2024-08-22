using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MandelbrotFractal
{
    public class Beat1 : WaveProvider32
    {
        private int t = 0;
        private bool switchSound = true;

        public Beat1()
        {
            this.SetWaveFormat(8000, 1);
        }

        public override int Read(float[] buffer, int offset, int sampleCount)
        {
            for (int i = 0; i < sampleCount; i++)
            {
                byte soundByte = switchSound ? GenerateBytebeatStrong(t) : GenerateBytebeatWeak(t);
                buffer[i + offset] = soundByte / 255f;
                t++;
            }

            switchSound = !switchSound;

            return sampleCount;
        }

        private byte GenerateBytebeatStrong(int t)
        {

            return (byte)(64 * (t >> 3 | t >> 4 | t >> 9) + (t >> 11 & t << 2) ^ 2 * (t >> 16 | t | t >> 7) + 32 * (t >> t & 32));
        }

        private byte GenerateBytebeatWeak(int t)
        {
            return (byte)(64 * (t >> 3 | t >> 4 | t >> 9) + (t >> 11 & t << 2) ^ 2 * (t >> 16 | t | t >> 7) + 32 * (t >> t & 32));
        }
    }

    public class Beat2 : WaveProvider32
    {
        private int t = 0;
        private bool switchSound = true;

        public Beat2()
        {
            this.SetWaveFormat(8000, 1);
        }

        public override int Read(float[] buffer, int offset, int sampleCount)
        {
            for (int i = 0; i < sampleCount; i++)
            {
                byte soundByte = switchSound ? GenerateBytebeatStrong(t) : GenerateBytebeatWeak(t);
                buffer[i + offset] = soundByte / 255f;
                t++;
            }

            switchSound = !switchSound;

            return sampleCount;
        }

        private byte GenerateBytebeatStrong(int t)
        {

            return (byte)((t * (t >> 13 | t >> 8) | t >> 16 ^ t) - 64);
        }

        private byte GenerateBytebeatWeak(int t)
        {
            return (byte)((t * (t >> 13 | t >> 8) | t >> 16 ^ t) - 64);
        }

    }

    public class Beat3 : WaveProvider32
    {
        private int t = 0;
        private bool switchSound = true;

        public Beat3()
        {
            this.SetWaveFormat(8000, 1);
        }

        public override int Read(float[] buffer, int offset, int sampleCount)
        {
            for (int i = 0; i < sampleCount; i++)
            {
                byte soundByte = switchSound ? GenerateBytebeatStrong(t) : GenerateBytebeatWeak(t);
                buffer[i + offset] = soundByte / 255f;
                t++;
            }

            switchSound = !switchSound;

            return sampleCount;
        }

        private byte GenerateBytebeatStrong(int t)
        {
            return (byte)((t / 4 & 244) >> t / ((t >> 14 & 3) + 4));
        }

        private byte GenerateBytebeatWeak(int t)
        {
            return (byte)((t / 4 & 244) >> t / ((t >> 14 & 3) + 4));
        }

    }

    public class Beat4 : WaveProvider32
    {
        private int t = 0;
        private bool switchSound = true;

        public Beat4()
        {
            this.SetWaveFormat(8000, 1);
        }

        public override int Read(float[] buffer, int offset, int sampleCount)
        {
            for (int i = 0; i < sampleCount; i++)
            {
                byte soundByte = switchSound ? GenerateBytebeatStrong(t) : GenerateBytebeatWeak(t);
                buffer[i + offset] = soundByte / 255f;
                t++;
            }

            switchSound = !switchSound;

            return sampleCount;
        }

        private byte GenerateBytebeatStrong(int t)
        {
            return (byte)(20 * t * t * (t >> 11) / 7);
        }

        private byte GenerateBytebeatWeak(int t)
        {
            return (byte)(20 * t * t * (t >> 11) / 7);
        }

    }

    public class BeatEnd : WaveProvider32
    {
        private int t = 0;
        private bool switchSound = true;

        public BeatEnd()
        {
            this.SetWaveFormat(8000, 1);
        }

        public override int Read(float[] buffer, int offset, int sampleCount)
        {
            for (int i = 0; i < sampleCount; i++)
            {
                byte soundByte = switchSound ? GenerateBytebeatStrong(t) : GenerateBytebeatWeak(t);
                buffer[i + offset] = soundByte / 255f;
                t++;
            }

            switchSound = !switchSound;

            return sampleCount;
        }

        private byte GenerateBytebeatStrong(int t)
        {
            return (byte)(2 * (1 - (t + 10 >> (t >> 9 & t >> 14) & t >> 4 & -2)) * ((t >> 10 ^ t + (t >> 6 & 127) >> 10) & 1) * 32 + 128);
        }

        private byte GenerateBytebeatWeak(int t)
        {
            return (byte)(2 * (1 - (t + 10 >> (t >> 9 & t >> 14) & t >> 4 & -2)) * ((t >> 10 ^ t + (t >> 6 & 127) >> 10) & 1) * 32 + 128);
        }

    }


}
