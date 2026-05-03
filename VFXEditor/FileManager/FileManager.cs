using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Data.Copy;
using VfxEditor.FileManager.Interfaces;
using VfxEditor.Select;
using VfxEditor.Ui.Export;
using VfxEditor.Utils;

namespace VfxEditor.FileManager {
    public abstract partial class FileManager<D, F, S> : FileManagerBase, IFileManager where D : FileManagerDocument<F, S> where F : FileManagerFile {
        public D ActiveDocument { get; protected set; }
        public F? ActiveFile => ActiveDocument?.File;

        private int DOC_ID = 0;
        public override string NewWriteLocation => Path.Combine( Plugin.Configuration.WriteLocation, $"{FormatName}Temp{DOC_ID++}.{Extension}" ).Replace( '\\', '/' );

        private readonly FileManagerDocumentWindow<D, F, S> DocumentWindow;
        public readonly List<D> Documents = [];

        public FileManager( FileManagerGroupBase group ) : base( group ) {
            AddDocument();
            DocumentWindow = new( Title, this );
        }

        // ===================

        public override void SetSource( SelectResult result ) {
            ActiveDocument?.SetSource( result );
            Plugin.Configuration.AddRecent( Configuration.RecentItems, result );
        }

        public override void SetReplace( SelectResult result ) {
            ActiveDocument?.SetReplace( result );
            Plugin.Configuration.AddRecent( Configuration.RecentItems, result );
        }

        private void CheckKeybinds() {
            if( !ImGui.IsWindowFocused( ImGuiFocusedFlags.RootAndChildWindows ) ) return;
            if( Plugin.Configuration.CopyKeybind.KeyPressed() ) CopyManager.Copy();
            if( Plugin.Configuration.PasteKeybind.KeyPressed() ) CopyManager.Paste();
            if( Plugin.Configuration.UndoKeybind.KeyPressed() ) CommandManager.Undo();
            if( Plugin.Configuration.RedoKeybind.KeyPressed() ) CommandManager.Redo();
            ActiveDocument?.CheckKeybinds();
        }

        // ====================

        protected abstract D GetNewDocument();

        public void AddDocument() {
            ActiveDocument = GetNewDocument();
            Documents.Add( ActiveDocument );
        }

        public void SelectDocument( D document ) {
            ActiveDocument = document;
        }

        public bool RemoveDocument( D document ) {
            DraggingItem = null;
            DocumentWindow.Reset();

            Documents.Remove( document );
            document.Dispose();
            ExportDialog.RemoveDocument( document );

            if( document == ActiveDocument && Documents.Count > 0 ) {
                ActiveDocument = Documents[0];
                return true;
            }
            return false;
        }

        // Document is being moved somewhere else
        public void MoveDocumentOut( D document ) {
            DraggingItem = null;
            DocumentWindow.Reset();

            Documents.Remove( document );
            if( Documents.Count == 0 ) AddDocument(); // There needs to be at least 1 document
            if( document == ActiveDocument ) ActiveDocument = Documents[0];
        }

        public void MoveDocumentIn( D document ) {
            Documents.Add( document );
            ActiveDocument = document;
        }

        public IEnumerable<IFileDocument> GetDocuments() => Documents;

        public void WorkspaceImport( JObject meta, string loadLocation, string key, string path ) {
            var items = WorkspaceUtils.ReadFromMeta<S>( meta, key );
            if( items == null || items.Length == 0 ) {
                AddDocument();
                return;
            }
            foreach( var item in items ) {
                var newDocument = GetWorkspaceDocument( item, Path.Combine( loadLocation, path ) );
                ActiveDocument = newDocument;
                Documents.Add( newDocument );
            }
        }

        protected abstract D GetWorkspaceDocument( S data, string localPath );

        public virtual void Reset( ResetType type ) {
            Documents.ForEach( x => x.Dispose() );
            Documents.Clear();
            SourceSelect?.Hide();
            ReplaceSelect?.Hide();

            ActiveDocument = null;
            DraggingItem = null;
            DocumentWindow.Reset();

            if( type == ResetType.ToDefault ) AddDocument(); // Default document
        }
    }
}
