using AVFXLib.Models;
using Dalamud.Logging;
using Dalamud.Plugin;
using System;
using VFXEditor.Data;
using VFXEditor.UI.VFX;
using VFXSelect.UI;

namespace VFXEditor {
    public partial class Plugin {
        private DateTime LastSelect = DateTime.Now;

        public void SetSourceVFX( VFXSelectResult selectResult) {
            SetSourceVFX( selectResult, true );
        }
        public void SetSourceVFX( VFXSelectResult selectResult, bool addToRecent ) {
            if( ( DateTime.Now - LastSelect ).TotalSeconds < 0.5 ) return;
            LastSelect = DateTime.Now;

            switch( selectResult.Type ) {
                case VFXSelectType.Local: // LOCAL
                    var localResult = DataManager.GetLocalFile( selectResult.Path, out var localAvfx );
                    if( localResult ) {
                        LoadCurrentVFX( localAvfx );
                    }
                    else {
                        PluginLog.Error( "Could not get file: " + selectResult.Path );
                        return;
                    }
                    break;
                default: // EVERYTHING ELSE: GAME FILES
                    var gameResult = DataManager.GetGameFile( selectResult.Path, out var gameAvfx );
                    if( gameResult ) {
                        LoadCurrentVFX( gameAvfx );
                    }
                    else {
                        PluginLog.Error( "Could not get file: " + selectResult.Path );
                        return;
                    }
                    break;
            }
            if( addToRecent ) Configuration.Config.AddRecent( selectResult );
            DocumentManager.Manager.UpdateSource( selectResult );
            DocumentManager.Manager.Save();
        }

        public static void RemoveSourceVFX() {
            DocumentManager.Manager.UpdateSource( VFXSelectResult.None() );
            DocumentManager.CurrentActiveDoc.Dispose();
        }

        public static void SetReplaceVFX( VFXSelectResult replaceResult) {
            SetReplaceVFX( replaceResult, true );
        }
        public static void SetReplaceVFX( VFXSelectResult replaceResult, bool addToRecent ) {
            if( addToRecent ) Configuration.Config.AddRecent( replaceResult );
            DocumentManager.Manager.UpdateReplace( replaceResult );
        }

        public static void RemoveReplaceVFX() {
            DocumentManager.Manager.UpdateReplace( VFXSelectResult.None() );
        }

        public static void LoadCurrentVFX( AVFXBase avfx ) {
            if( avfx == null ) return;
            DocumentManager.CurrentActiveDoc.SetAVFX( avfx );

            if( Configuration.Config.VerifyOnLoad ) {
                var node = avfx.ToAVFX();
                var verifyResult = DataManager.LastImportNode.CheckEquals( node, out var messages );
                DocumentManager.CurrentActiveDoc.Main.Verified = verifyResult ? VerifiedStatus.OK : VerifiedStatus.ISSUE;
                PluginLog.Log( $"[VERIFY RESULT]: {verifyResult}" );
                foreach( var m in messages ) {
                    PluginLog.Warning( m );
                }
            }
        }
    }
}
