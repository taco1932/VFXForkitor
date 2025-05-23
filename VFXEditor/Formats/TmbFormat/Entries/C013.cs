using System.Collections.Generic;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;
using VfxEditor.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public class C013 : TmbEntry {
        public const string MAGIC = "C013";
        public const string DISPLAY_NAME = "Model Animation";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x1C;
        public override int ExtraSize => 0;
        public override DangerLevel Danger => DangerLevel.Yellow;

        private readonly ParsedInt Duration = new( "Duration" );
        private readonly ParsedInt Unk2 = new( "Unknown 2" );
        private readonly ParsedInt TmfcId = new( "F-Curve Id" );
        private readonly ParsedInt Placement = new( "Placement" );

        public C013( TmbFile file ) : base( file ) { }

        public C013( TmbFile file, TmbReader reader ) : base( file, reader ) { }

        protected override List<ParsedBase> GetParsed() => [
            Duration,
            Unk2,
            TmfcId,
            Placement
        ];
    }
}
