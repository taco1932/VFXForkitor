using Dalamud.Interface.Utility.Raii;
using System;
using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.ScdFormat.Sound.Data {
    [Flags]
    public enum SoundExtraUnk2 {
        Unknown_01 = 0x01,
        DualSense_1 = 0x02,
        Is_Controller_Only = 0x04,
        DualSense_2 = 0x08,
        Unknown_05 = 0x10,
        Use_Low_Pass_Filter = 0x20,
        Unknown_07 = 0x40,
        Unknown_08 = 0x80 //RandomWind
    }
    public class SoundExtra {
        public readonly ParsedByte Version = new( "Version" );
        private readonly ParsedByte Unknown1 = new( "Unknown 1" ); //Reserve 1
        private ushort Size = 0x10;
        public readonly ParsedInt PlayTimeLength = new( "Play Time Length" );
        private readonly ParsedFlag<SoundExtraUnk2> Unknown2 = new( "Unknown 2" );
        private readonly ParsedFloat Unknown3 = new( "Unknown 3" ); //0.0 or 999.0

        public void Read( BinaryReader reader ) {
            Version.Read( reader );
            Unknown1.Read( reader );
            Size = reader.ReadUInt16();
            PlayTimeLength.Read( reader );
            Unknown2.Read( reader );
            Unknown3.Read( reader );
        }

        public void Write( BinaryWriter writer ) {
            Version.Write( writer );
            Unknown1.Write( writer );
            writer.Write( Size );
            PlayTimeLength.Write( writer );
            Unknown2.Write( writer );
            Unknown3.Write( writer );
        }

        public void Draw() {
            using var _ = ImRaii.PushId( "Extra" );

            Version.Draw();
            Unknown1.Draw();
            PlayTimeLength.Draw();
            Unknown2.Draw();
            Unknown3.Draw();
        }
    }
}
