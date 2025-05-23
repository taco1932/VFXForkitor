using Dalamud.Game.ClientState.Conditions;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using Penumbra.String;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using VfxEditor.Structs;
using VfxEditor.Utils;

namespace VfxEditor.Interop {
    public unsafe partial class ResourceLoader {
        private const int INVIS_FLAG = ( 1 << 1 ) | ( 1 << 11 );

        // ====== REDRAW =======

        private enum RedrawState {
            None,
            Start,
            Invisible,
            Visible
        }

        private RedrawState CurrentRedrawState = RedrawState.None;
        private int WaitFrames = 0;

        // ========= MISC ==============

        public delegate IntPtr GetMatrixSingletonDelegate();

        public GetMatrixSingletonDelegate GetMatrixSingleton { get; private set; }

        public delegate IntPtr GetFileManagerDelegate();

        private readonly GetFileManagerDelegate GetFileManager;

        private readonly GetFileManagerDelegate GetFileManager2;

        [UnmanagedFunctionPointer( CallingConvention.ThisCall )]
        public delegate byte DecRefDelegate( IntPtr resource );

        private readonly DecRefDelegate DecRef;

        [UnmanagedFunctionPointer( CallingConvention.ThisCall )]
        private delegate void* RequestFileDelegate( IntPtr a1, IntPtr a2, IntPtr a3, byte a4 );

        private readonly RequestFileDelegate RequestFile;

        public void ReRender() {
            if( CurrentRedrawState != RedrawState.None || Plugin.PlayerObject == null || Dalamud.Condition[ConditionFlag.Fishing] ) return;
            CurrentRedrawState = RedrawState.Start;
            Dalamud.Framework.Update += OnUpdateEvent;
        }

        private void OnUpdateEvent( object framework ) {
            if( Plugin.PlayerObject == null ) return;
            var gameObject = ( GameObject* )Plugin.PlayerObject.Address;

            switch( CurrentRedrawState ) {
                case RedrawState.Start:
                    gameObject->RenderFlags |= INVIS_FLAG;
                    CurrentRedrawState = RedrawState.Invisible;
                    WaitFrames = 15;
                    break;
                case RedrawState.Invisible:
                    if( WaitFrames == 0 ) {
                        gameObject->RenderFlags &= ~INVIS_FLAG;
                        CurrentRedrawState = RedrawState.Visible;
                    }
                    else WaitFrames--;
                    break;
                case RedrawState.Visible:
                default:
                    CurrentRedrawState = RedrawState.None;
                    Dalamud.Framework.Update -= OnUpdateEvent;
                    break;
            }
        }

        private static bool ProcessPenumbraPath( string path, out string outPath ) {
            outPath = path;
            if( string.IsNullOrEmpty( path ) ) return false;
            if( !path.StartsWith( '|' ) ) return false;

            var split = path.Split( "|" );
            if( split.Length != 3 ) return false;

            outPath = split[2];
            return true;
        }

        private static bool GetReplacePath( string gamePath, out string localPath ) {
            localPath = null;

            if( Plugin.State != WorkspaceState.None ) return false;

            foreach( var manager in Plugin.Managers.Where( x => x != null ) ) {
                if( manager.GetReplacePath( gamePath, out var managerLocalPath ) ) {
                    localPath = managerLocalPath;
                    return true;
                }
            }

            return GetCustomPathBackup( gamePath, out localPath );
        }

        private static bool GetCustomPathBackup( string gamePath, out string localPath ) {
            localPath = null;
            if( Dalamud.GameFileExists( gamePath ) ) return false; // not custom path
            return Plugin.CustomPathBackups.TryGetValue( gamePath.ToLower(), out localPath );
        }

        public void ReloadPath( string gamePath, string localPath, List<string> papIds, List<short> papTypes ) {
            if( string.IsNullOrEmpty( gamePath ) ) return;

            var gameResource = GetResource( gamePath, true );
            if( Plugin.Configuration?.LogDebug == true && DoDebug( gamePath ) ) Dalamud.Log( $"[ReloadPath] {gamePath} / {localPath} -> " + gameResource.ToString( "X8" ) );

            if( gameResource != IntPtr.Zero ) {
                InteropUtils.PrepPap( gameResource, papIds, papTypes );
                RequestFile( GetFileManager2(), gameResource + Constants.GameResourceOffset, gameResource, 1 );
                InteropUtils.WritePapIds( gameResource, papIds, papTypes );
            }

            if( string.IsNullOrEmpty( localPath ) ) return;

            var localGameResource = GetResource( gamePath, false ); // get local path resource
            if( Plugin.Configuration?.LogDebug == true && DoDebug( gamePath ) ) Dalamud.Log( $"[ReloadPath] {gamePath} / {localPath} -> " + localGameResource.ToString( "X8" ) );

            if( localGameResource != IntPtr.Zero ) {
                if( gamePath.EndsWith( ".pbd" ) ) ReplacePbd( ( ResourceHandle* )localGameResource );

                InteropUtils.PrepPap( localGameResource, papIds, papTypes );
                RequestFile( GetFileManager2(), localGameResource + Constants.GameResourceOffset, localGameResource, 1 );
                InteropUtils.WritePapIds( localGameResource, papIds, papTypes );
            }
        }

        private IntPtr GetResource( string path, bool original, bool decRef = true ) {
            var extension = FileUtils.Reverse( path.Split( '.' )[1] );
            var typeBytes = Encoding.ASCII.GetBytes( extension );
            var bType = stackalloc byte[typeBytes.Length + 1];
            Marshal.Copy( typeBytes, 0, new IntPtr( bType ), typeBytes.Length );
            var pResourceType = ( ResourceType* )bType;

            // Category
            var split = path.Split( '/' );
            var categoryString = split[0];
            var categoryBytes = categoryString switch {
                "bgcommon" => BitConverter.GetBytes( 1u ),
                "cur" => InteropUtils.GetDatCategory( 3u, split[1] ),
                "chara" => BitConverter.GetBytes( 4u ),
                "shader" => BitConverter.GetBytes( 5u ),
                "ui" => BitConverter.GetBytes( 6u ),
                "sound" => BitConverter.GetBytes( 7u ),
                "vfx" => BitConverter.GetBytes( 8u ),
                "bg" => InteropUtils.GetBgCategory( split[1], split[2] ),
                "music" => InteropUtils.GetDatCategory( 12u, split[1] ),
                _ => BitConverter.GetBytes( 0u )
            };
            var bCategory = stackalloc byte[categoryBytes.Length + 1];
            Marshal.Copy( categoryBytes, 0, new IntPtr( bCategory ), categoryBytes.Length );
            var pCategoryId = ( uint* )bCategory;

            ByteString.FromString( path, out var resolvedPath );
            var hash = resolvedPath.GetHashCode();

            var hashBytes = BitConverter.GetBytes( hash );
            var bHash = stackalloc byte[hashBytes.Length + 1];
            Marshal.Copy( hashBytes, 0, new IntPtr( bHash ), hashBytes.Length );
            var pResourceHash = ( int* )bHash;

            var resource = original ? new IntPtr( GetResourceSyncHook.Original( GetFileManager(), pCategoryId, pResourceType, pResourceHash, resolvedPath.Path, null ) ) :
                new IntPtr( GetResourceSyncDetour( GetFileManager(), pCategoryId, pResourceType, pResourceHash, resolvedPath.Path, null ) );
            if( decRef ) DecRef( resource );

            return resource;
        }

        private static bool DoDebug( string path ) => Plugin.State == WorkspaceState.None && Plugin.Managers.Where( x => x != null && x.DoDebug( path ) ).Any();
    }
}
