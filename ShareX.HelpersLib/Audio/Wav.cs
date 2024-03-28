using System;
using System.IO;

namespace ShareX.HelpersLib.Audio
{
    // The fields and their sizes were created according to WAV specification
    // See: http://soundfile.sapp.org/doc/WaveFormat/
    public class Wav
    {
        private const int Riff = 1179011410;
        public readonly int Size;
        private const int Format = 1163280727;

        private const int FormatChunkId = 544501094;
        public readonly int FormatSize;
        public readonly short AudioFormat;
        public readonly short Channels;
        public readonly int SampleRate;
        public readonly int ByteRate;
        public readonly short BlockAlign;
        public readonly short BitsPerSample;

        private const int DataChunkId = 1635017060;
        public readonly int DataSize;
        public readonly byte[] SoundData;

        public Wav(Stream stream)
        {
            var reader = new BinaryReader(stream);

            if (reader.ReadInt32() != Riff)
                throw new Exception("Invalid WAV file: wrong RIFF chunk ID");

            Size = reader.ReadInt32();

            if (reader.ReadInt32() != Format)
                throw new Exception("Invalid WAV file: wrong RIFF chunk format");
            if (reader.ReadInt32() != FormatChunkId)
                throw new Exception("Invalid WAV file: wrong fmt subchunk ID");

            FormatSize = reader.ReadInt32();
            AudioFormat = reader.ReadInt16();
            Channels = reader.ReadInt16();
            SampleRate = reader.ReadInt32();
            ByteRate = reader.ReadInt32();
            BlockAlign = reader.ReadInt16();
            BitsPerSample = reader.ReadInt16();

            if (reader.ReadInt32() != DataChunkId)
                throw new Exception("Invalid WAV file: wrong data subchunk ID");

            DataSize = reader.ReadInt32();
            SoundData = reader.ReadBytes(DataSize);
        }

        public Stream ToStream()
        {
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);

            writer.Write(Riff);
            writer.Write(Size);
            writer.Write(Format);
            writer.Write(FormatChunkId);
            writer.Write(FormatSize);
            writer.Write(AudioFormat);
            writer.Write(Channels);
            writer.Write(SampleRate);
            writer.Write(ByteRate);
            writer.Write(BlockAlign);
            writer.Write(BitsPerSample);
            writer.Write(DataChunkId);
            writer.Write(DataSize);
            writer.Write(SoundData);

            stream.Position = 0;
            return stream;
        }

        public Wav ChangeVolume(float factor)
        {
            for (var i = 0; i < DataSize; i += BitsPerSample/8)
            {
                var int16 = BitConverter.ToInt16(SoundData, i);
                int16 = (short)Math.Round(int16 * factor);
                BitConverter.GetBytes(int16).CopyTo(SoundData, i);
            }

            return this;
        }
    }
}