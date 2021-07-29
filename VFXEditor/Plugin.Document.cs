using AVFXLib.Models;
using Dalamud.Plugin;
using System;
using VFXEditor.Data;
using VFXEditor.UI.VFX;
using VFXSelect.UI;

namespace VFXEditor {
    public partial class Plugin {
        public ReplaceDoc CurrentDocument => DocManager.ActiveDoc;
        private DateTime LastSelect = DateTime.Now;

        public void SetSourceVFX( VFXSelectResult selectResult) {
            SetSourceVFX( selectResult, true );
        }
        public void SetSourceVFX( VFXSelectResult selectResult, bool addToRecent ) {
            if( ( DateTime.Now - LastSelect ).TotalSeconds < 0.5 ) return;
            LastSelect = DateTime.Now;

            switch( selectResult.Type ) {
                case VFXSelectType.Local: // LOCAL
                    var localResult = GetLocalFile( selectResult.Path, out var localAvfx );
                    if( localResult ) {
                        LoadCurrentVFX( localAvfx );
                    }
                    else {
                        PluginLog.Log( "Could not get file: " + selectResult.Path );
                        return;
                    }
                    break;
                default: // EVERYTHING ELSE: GAME FILES
                    var gameResult = GetGameFile( selectResult.Path, out var gameAvfx );
                    if( gameResult ) {
                        LoadCurrentVFX( gameAvfx );
                    }
                    else {
                        PluginLog.Log( "Could not get file: " + selectResult.Path );
                        return;
                    }
                    break;
            }
            if( addToRecent ) Configuration.Config.AddRecent( selectResult );
            DocManager.UpdateSource( selectResult );
            DocManager.Save();
        }

        public void RemoveSourceVFX() {
            DocManager.UpdateSource( VFXSelectResult.None() );
            CurrentDocument.Dispose();
        }

        public void SetReplaceVFX( VFXSelectResult replaceResult) {
            SetReplaceVFX( replaceResult, true );
        }
        public void SetReplaceVFX( VFXSelectResult replaceResult, bool addToRecent ) {
            if( addToRecent ) Configuration.Config.AddRecent( replaceResult );
            DocManager.UpdateReplace( replaceResult );
        }

        public void RemoveReplaceVFX() {
            DocManager.UpdateReplace( VFXSelectResult.None() );
        }

        public void LoadCurrentVFX( AVFXBase avfx ) {
            if( avfx == null ) return;
            CurrentDocument.SetAVFX( avfx );

            if( Configuration.Config.VerifyOnLoad ) {
                var node = avfx.ToAVFX();
                var verifyResult = LastImportNode.CheckEquals( node, out var messages );
                CurrentDocument.Main.Verified = verifyResult ? VerifiedStatus.OK : VerifiedStatus.ISSUE;
                PluginLog.Log( $"[VERIFY RESULT]: {verifyResult}" );
                foreach( var m in messages ) {
                    PluginLog.Log( m );
                }
            }
        }
    }
}
