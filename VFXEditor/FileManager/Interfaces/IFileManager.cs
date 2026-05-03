using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace VfxEditor.FileManager.Interfaces {
    public enum ResetType {
        Reset,
        PluginClosing,
        ToDefault
    }

    public interface IFileManager : IFileManagerSelect {
        public void WorkspaceImport( JObject meta, string loadLocation, string key, string path );

        public IEnumerable<IFileDocument> GetDocuments();

        public void Show();

        public void Draw();

        public void Reset( ResetType type );
    }
}
