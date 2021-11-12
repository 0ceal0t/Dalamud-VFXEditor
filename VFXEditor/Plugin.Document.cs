using AVFXLib.Models;
using Dalamud.Logging;

using System;

using VFXEditor.Document;
using VFXEditor.Helper;
using VFXSelect;

namespace VFXEditor {
    public partial class Plugin {
        private DateTime LastSelect = DateTime.Now;

        public void SetSourceVFX( SelectResult selectResult) {
            SetSourceVFX( selectResult, true );
        }
        public void SetSourceVFX( SelectResult selectResult, bool addToRecent ) {
            if( ( DateTime.Now - LastSelect ).TotalSeconds < 0.5 ) return;
            LastSelect = DateTime.Now;

            switch( selectResult.Type ) {
                case SelectResultType.Local: // LOCAL
                    var localResult = AvfxHelper.GetLocalFile( selectResult.Path, out var localAvfx );
                    if( localResult ) {
                        LoadCurrentVFX( localAvfx );
                    }
                    else {
                        PluginLog.Error( "Could not get file: " + selectResult.Path );
                        return;
                    }
                    break;
                default: // EVERYTHING ELSE: GAME FILES
                    var gameResult = AvfxHelper.GetGameFile( selectResult.Path, out var gameAvfx );
                    if( gameResult ) {
                        LoadCurrentVFX( gameAvfx );
                    }
                    else {
                        PluginLog.Error( "Could not get file: " + selectResult.Path );
                        return;
                    }
                    break;
            }
            if( addToRecent ) Configuration.AddRecent( selectResult );
            DocumentManager.UpdateSource( selectResult );
            DocumentManager.Save();
        }

        public static void RemoveSourceVFX() {
            DocumentManager.UpdateSource( SelectResult.None() );
            DocumentManager.ActiveDocument.Dispose();
        }

        public static void SetReplaceVFX( SelectResult replaceResult) {
            SetReplaceVFX( replaceResult, true );
        }
        public static void SetReplaceVFX( SelectResult replaceResult, bool addToRecent ) {
            if( addToRecent ) Configuration.AddRecent( replaceResult );
            DocumentManager.UpdateReplace( replaceResult );
        }

        public static void RemoveReplaceVFX() {
            DocumentManager.UpdateReplace( SelectResult.None() );
        }

        public static void LoadCurrentVFX( AVFXBase avfx ) {
            if( avfx == null ) return;
            DocumentManager.ActiveDocument.SetAVFX( avfx );

            if( Configuration.VerifyOnLoad ) {
                var node = avfx.ToAVFX();
                var verifyResult = AvfxHelper.LastImportNode.CheckEquals( node, out var messages );
                DocumentManager.ActiveDocument.Main.Verified = verifyResult ? VerifiedStatus.OK : VerifiedStatus.ISSUE;
                PluginLog.Log( $"[VERIFY RESULT]: {verifyResult}" );
                foreach( var m in messages ) {
                    PluginLog.Warning( m );
                }
            }
        }
    }
}
