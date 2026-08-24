using Dalamud.Interface.Utility.Raii;
using System;
using System.Collections.Generic;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    [Flags]
    public enum AnimationFlags {
        Time_Control_Enabled = 0x01,
        Unknown_Flag_2 = 0x02,
        Unknown_Flag_3 = 0x04,
        Unknown_Flag_4 = 0x08,
        Unknown_Flag_5 = 0x10,
        Unknown_Flag_6 = 0x20,
        Unknown_Flag_7 = 0x40,
        Unknown_Flag_8 = 0x80
    }

    public class C010 : TmbEntry {
        public const string MAGIC = "C010";
        public const string DISPLAY_NAME = "Animation";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x28;
        public override int ExtraSize => 0;

        private readonly ParsedInt Duration = new( "Duration", value: 50 );
        private readonly ParsedInt Unk1 = new( "Unknown 1" );
        private readonly ParsedFlag<AnimationFlags> Flags = new( "Flags", size: 1 );
        private readonly ParsedByte Unk3 = new ( "Unknown 3" ); //1-6
        private readonly ParsedByte Unk4 = new ( "Unknown 4" ); //0
        private readonly ParsedByte Unk5 = new ( "Unknown 5" ); //8
        private readonly ParsedFloat AnimationStart = new( "Animation Start Frame" );
        private readonly ParsedFloat AnimationEnd = new( "Animation End Frame" );
        private readonly TmbOffsetString Path = new( "Path" );
        private readonly ParsedInt Unk2 = new( "Unknown 2" );

        public C010( TmbFile file ) : base( file ) { }

        public C010( TmbFile file, TmbReader reader ) : base( file, reader ) { }

        protected override List<ParsedBase> GetParsed() => [
            Duration,
            Unk1,
            Flags,
            Unk3,
            Unk4,
            Unk5,
            AnimationStart,
            AnimationEnd,
            Path,
            Unk2
        ];

        public override void DrawBody() {
            DrawHeader();

            Flags.Draw();

            using( var disabled = ImRaii.Disabled( !Flags.HasFlag( AnimationFlags.Time_Control_Enabled ) ) ) {
                Duration.Draw();
                AnimationStart.Draw();
                AnimationEnd.Draw();
            }

            Unk1.Draw();
            Path.Draw();
            Unk2.Draw();
            Unk3.Draw();
            Unk4.Draw();
            Unk5.Draw();
        }
    }
}
