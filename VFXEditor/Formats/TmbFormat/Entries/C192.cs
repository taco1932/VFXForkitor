using System.Collections.Generic;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;
using VfxEditor.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public class C192 : TmbEntry {
        public const string MAGIC = "C192";
        public const string DISPLAY_NAME = "";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;
        public override DangerLevel Danger => DangerLevel.Yellow;

        public override int Size => 0x38;
        public override int ExtraSize => 0;

        private readonly ParsedInt Unk1 = new( "Unknown 1" );
        private readonly ParsedInt Unk2 = new( "Unknown 2" );
        private readonly ParsedInt Unk3 = new( "Unknown 3" ); //not voiceline, but similar format
        private readonly ParsedInt Unk4 = new( "Unknown 4" );
        private readonly ParsedInt Unk5 = new( "Unknown 5" );
        private readonly ParsedInt Unk6 = new( "Unknown 6" );
        private readonly ParsedInt Unk7 = new( "Unknown 7" );
        private readonly ParsedInt Unk8 = new( "Unknown 8" );
        private readonly ParsedInt Unk9 = new( "Unknown 9" );
        private readonly ParsedInt Unk10 = new( "Unknown 10" );
        private readonly ParsedInt Unk11 = new( "Unknown 11" );

        public C192( TmbFile file ) : base( file ) { }

        public C192( TmbFile file, TmbReader reader ) : base( file, reader ) { }

        protected override List<ParsedBase> GetParsed() => [
            Unk1,
            Unk2,
            Unk3,
            Unk4,
            Unk5,
            Unk6,
            Unk7,
            Unk8,
            Unk9,
            Unk10,
            Unk11
        ];
    }
}
