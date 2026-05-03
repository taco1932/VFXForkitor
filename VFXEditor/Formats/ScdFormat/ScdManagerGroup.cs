using System.IO;
using VfxEditor.FileManager;
using VfxEditor.FileManager.Interfaces;
using VfxEditor.Formats.ScdFormat.Utils;
using VfxEditor.ScdFormat;
using VfxEditor.Utils;

namespace VfxEditor.Formats.ScdFormat {
    public class ScdManagerGroup : FileManagerGroup<ScdManager, ScdDocument, ScdFile, WorkspaceMetaBasic> {
        public static string ConvertWav => Path.Combine( Plugin.Configuration.WriteLocation, "temp_out.wav" ).Replace( '\\', '/' );
        public static string ConvertOgg => Path.Combine( Plugin.Configuration.WriteLocation, "temp_out.ogg" ).Replace( '\\', '/' );

        public ScdManagerGroup() : base( "Scd Editor", "Scd" ) { }

        protected override ScdManager GetNewManager() => new( this );

        public override void Reset( ResetType type ) {
            base.Reset( type );
            ScdUtils.Cleanup();
        }
    }
}
