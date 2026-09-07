using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace NEA_prototype_wpf
{
    public class WaveFileProcess
    {

        //private int samplerate = 0;
        //private int channels;
        //private string format;





        //public static string[]
    }

    public class WaveFileProcessor
    {
        public int sample_count;
        public int samplerate;
        public int channels;
        public int bitspersample;
        public int bytesPerInstantaneousAmplitude;
        private BinaryReader file;
        public byte[] samples;


        public WaveFileProcessor(string filename)
        {
            file = new BinaryReader(File.Open(filename, FileMode.Open));
        }
        
        public void closeFile()
        {
            file.Close();
        }

        public byte[] getNextSample()
        {

            byte[] sample_byte_pair = file.ReadBytes((bitspersample/8) * channels);

            return sample_byte_pair;
        }
        public void populateSamplesByteArray()
        {
            samples = file.ReadBytes(sample_count*2);
        }

        public byte[] getSampleFromArray(int offset)
        {
            byte[] sample_byte_pair = new byte[bytesPerInstantaneousAmplitude];

            sample_byte_pair = new byte[] { samples[offset * 2], samples[(offset * 2) + 1] };

            return sample_byte_pair;
        }

        public void loadFileHeader()
        {
            byte[] header = new byte[44]; // wave file header is always 44 bytes
            const int headerOffset = 44;

            /*
            *  Getting header information - sample rate, channels, bit depth, format type (mulaw, 16bit int, 32bit float etc)
            *  Throw error if format is wrong
            *
            *
            *
            */

            header = file.ReadBytes(headerOffset);

            for (int i = 0; i < 4; i++)
            {   
                samplerate += (header[i + 24]) << (i * 8); // little endian, 4 bytes to store sample rate
            }
            channels = header[22] + (header[23] << 8);
            bitspersample = header[34] + (header[35] << 8);
            sample_count = ((header[40] + (header[41] << 8) + (header[42] << 16) + (header[43] << 24)) * 8)/bitspersample;
            samples = new byte[sample_count*2];
            bytesPerInstantaneousAmplitude = (bitspersample / 8) * channels;

            Console.WriteLine(channels);
            Console.WriteLine(samplerate);
            Console.WriteLine(bitspersample/8);
            Console.WriteLine(header[20] + (header[21] << 8));
            Console.WriteLine(sample_count);

            

        }
    }
}
