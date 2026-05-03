using Dalamud.Interface.Windowing;

namespace VfxEditor.FileManager {
    public abstract class FileManagerGroupBase {
        public readonly string FormatName;
        public readonly string Title;
        public readonly string Extension;
        public readonly string WorkspaceKey;
        public readonly string WorkspacePath;

        public readonly WindowSystem WindowSystem = new();

        protected FileManagerGroupBase( string title, string formatName, string extension, string workspaceKey, string workspacePath ) {
            Title = title;
            Extension = extension;
            WorkspaceKey = workspaceKey;
            WorkspacePath = workspacePath;
            FormatName = formatName;
        }

        public string GetId() => FormatName;

        public string GetName() => FormatName.ToLower();

        public WindowSystem GetWindowSystem() => WindowSystem;

        public abstract void SetLastFocusedManager( FileManagerBase manager );

        public abstract void Draw();
    }
}
