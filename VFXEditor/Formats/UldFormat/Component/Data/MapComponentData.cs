using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Component.Data {
    public class MapComponentData : UldGenericData {
        public MapComponentData() {
            Parsed.AddRange( [
                new ParsedUInt( "Unknown Node Id 1" ),
                new ParsedUInt( "Unknown Node Id 2" ),
                new ParsedUInt( "Unknown Node Id 3" ),
                new ParsedUInt( "Unknown Node Id 4" ),
                new ParsedUInt( "Unknown Node Id 5" ),
                new ParsedUInt( "Unknown Node Id 6" ),
                new ParsedUInt( "Unknown Node Id 7" ),
                new ParsedUInt( "Unknown Node Id 8" ),
                new ParsedUInt( "Unknown Node Id 9" ),
                new ParsedUInt( "Unknown Node Id 10"),
                new ParsedUInt( "Unknown Node Id 11"),
            ] );
        }
    }
}
