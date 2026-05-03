using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace VfxEditor.FileManager.Interfaces {
    public interface IFileManagerGroup {
        public bool DoDebug( string path );

        public bool GetReplacePath( string gamePath, out string replacePath );

        public bool FileExists( string path );

        public void WorkspaceImport( JObject meta, string loadLocation );

        public void WorkspaceExport( Dictionary<string, string> meta, string saveLocation );

        public IEnumerable<IFileDocument> GetDocuments();

        public string GetName();

        public string GetId();

        public void Show();

        public void Draw();

        public void Hide();

        public void Reset( ResetType type );

        public static bool FileExist( IFileManagerGroup manager, string path ) =>
    Dalamud.GameFileExists( path ) || Plugin.PenumbraIpc.PenumbraFileExists( path, out var _ ) || manager.GetReplacePath( path, out var _ );

        public static bool GetReplacePath( IFileManagerGroup manager, string path, out string replacePath ) {
            replacePath = null;
            foreach( var document in manager.GetDocuments() ) {
                if( document.GetReplacePath( path, out var documentReplacePath ) ) {
                    replacePath = documentReplacePath;
                    return true;
                }
            }
            return false;
        }
    }
}
