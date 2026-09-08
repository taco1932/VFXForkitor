using System.Collections.Generic;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public enum C043Type {
        Invalid = 0x00,
        Remove_Attribute = 0x01,
        Apply_Attribute = 0x02,
        Use_Internal_Id = 0x03,
        Dead_Pose_Unknown = 0x05,
        Use_Weapon_Id = 0x08
    }

    public class C043 : TmbEntry {
        public const string MAGIC = "C043";
        public const string DISPLAY_NAME = "Summon Weapon";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x20;
        public override int ExtraSize => 0;

        private readonly ParsedInt Duration = new( "Duration" );
        private readonly ParsedInt Unk1 = new( "Unknown 1" );
        private readonly ParsedEnum<C043Type> Type = new( "Type" );
        private readonly ParsedShort WeaponId = new( "Weapon Id" );
        private readonly ParsedShort BodyId = new( "Body Id" );
        private readonly ParsedInt VariantId = new( "Variant Id" );

        public C043( TmbFile file ) : base( file ) { }

        public C043( TmbFile file, TmbReader reader ) : base( file, reader ) { }

        protected override List<ParsedBase> GetParsed() => [
            Duration,
            Unk1,
            Type,
            WeaponId,
            BodyId,
            VariantId
        ];
    }
}
