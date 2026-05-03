using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VfxEditor.FileManager.Interfaces;
using VfxEditor.Utils;

namespace VfxEditor.FileManager {
    public abstract class FileManagerGroup<M, D, F, S> : FileManagerGroupBase, IFileManagerGroup where M : FileManager<D, F, S> where D : FileManagerDocument<F, S> where F : FileManagerFile {
        public readonly List<M> Managers = [];
        public List<D> Documents => [.. Managers.SelectMany( x => x.Documents )];

        public M LastFocusedManager { get; protected set; } = null;
        private bool Visible = true;

        public FileManagerGroup( string title, string formatName ) :
            this( title, formatName, formatName.ToLower(), formatName, formatName ) { }

        public FileManagerGroup( string title, string formatName, string extension, string workspaceKey, string workspacePath ) :
            base( title, formatName, extension, workspaceKey, workspacePath ) {

            LastFocusedManager = AddManager();
        }

        protected abstract M GetNewManager();

        public M AddManager() {
            var newManager = GetNewManager();
            Managers.Add( newManager );
            return newManager;
        }

        public override void NewWindow() {
            AddManager().Show();
        }

        public override void ToNewWindow( FileManagerBase currentManager, IFileDocument document ) {
            if( ( currentManager is M m ) && ( document is D d ) ) {
                m.MoveDocumentOut( d );
                var newManager = AddManager();
                newManager.Show();
                newManager.RemoveDocument( newManager.Documents[0] ); // remove the default
                newManager.MoveDocumentIn( d );
            }
        }

        public override void OnClose( FileManagerBase manager ) {
            if( Managers.Contains( manager ) ) RemoveManager( ( M )manager );
        }

        public void RemoveManager( M manager ) {
            if( Managers.Count == 1 ) return; // can't remove the last manager
            if( LastFocusedManager == manager ) LastFocusedManager = null;
            manager.Reset( ResetType.PluginClosing );
            Managers.Remove( manager );
            WindowSystem.RemoveWindow( manager );
        }

        public IEnumerable<IFileDocument> GetDocuments() => Managers.Select( x => x.GetDocuments() ).SelectMany( x => x);

        public void WorkspaceImport( JObject meta, string loadLocation ) {
            Managers[0].WorkspaceImport( meta, loadLocation, WorkspaceKey, WorkspacePath );
        }

        public void WorkspaceExport( Dictionary<string, string> meta, string saveLocation ) {
            var rootPath = Path.Combine( saveLocation, WorkspacePath );
            Directory.CreateDirectory( rootPath );

            List<S> documentMeta = [];
            var idx = 0;
            foreach( var manager in Managers ) {
                foreach( var document in manager.Documents ) {
                    document.WorkspaceExport( documentMeta, rootPath, $"{FormatName}Temp{idx}.{Extension}" );
                    idx++;
                }
            }

            WorkspaceUtils.WriteToMeta( meta, documentMeta.ToArray(), WorkspaceKey );
        }

        public bool DoDebug( string path ) => path.Contains( $".{Extension}" );

        public bool FileExists( string path ) => IFileManagerGroup.FileExist( this, path );

        public bool GetReplacePath( string path, out string replacePath ) => IFileManagerGroup.GetReplacePath( this, path, out replacePath );

        public override void SetLastFocusedManager( FileManagerBase manager ) {
            if( manager is M m ) LastFocusedManager = m;
        }

        public virtual void Reset( ResetType type ) {
            LastFocusedManager = null;
            Managers[1..].ForEach( x => x.Reset( ResetType.PluginClosing ) );
            Managers[0].Reset( type );
            Managers.RemoveRange( 1, Managers.Count - 1 );
        }

        public void Show() {
            Visible = true;
            Managers.ForEach( x => x.Show() );
        }

        public override void Draw() {
            if( !Visible ) return;
            WindowSystem.Draw();
        }

        public void Hide() {
            Visible = false;
        }
    }
}
