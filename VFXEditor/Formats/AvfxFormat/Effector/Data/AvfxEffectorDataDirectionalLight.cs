using VFXEditor.Formats.AvfxFormat.Curve;

namespace VfxEditor.AvfxFormat {
    public class AvfxEffectorDataDirectionalLight : AvfxData {
        public readonly AvfxCurveColor Ambient = new( "Ambient", "Amb" );
        public readonly AvfxCurveColor Color = new( "Color" );
        public readonly AvfxCurve1Axis Power = new( "Power", "Pow" );
        public readonly AvfxCurve1Axis PowerRandom = new( "Power Random", "PowR" );
        public readonly AvfxCurve3Axis Rotation = new( "Rotation", "Rot", CurveType.Angle );

        public AvfxEffectorDataDirectionalLight() : base() {
            Parsed = [
                Ambient,
                Color,
                Power,
                PowerRandom,
                Rotation
            ];

            Tabs.Add( Ambient );
            Tabs.Add( Color );
            Tabs.Add( Power );
            Tabs.Add( PowerRandom );
            Tabs.Add( Rotation );
        }
    }
}
