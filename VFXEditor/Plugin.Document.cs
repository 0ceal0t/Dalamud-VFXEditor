using AVFXLib.Models;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFXSelect.UI;

namespace VFXEditor {
    public partial class Plugin {
        public AVFXBase AVFX {
            get { return Doc.ActiveDoc.AVFX; }
            set { Doc.ActiveDoc.AVFX = value; }
        }
        public string ReplaceAVFXPath => Doc.ActiveDoc.Replace.Path;
        public string SourceString => Doc.ActiveDoc.Source.DisplayString;
        public string ReplaceString => Doc.ActiveDoc.Replace.DisplayString;

        private DateTime LastSelect = DateTime.Now;

        public void SetSourceVFX( VFXSelectResult selectResult) {
            SetSourceVFX( selectResult, true );
        }
        public void SetSourceVFX( VFXSelectResult selectResult, bool addToRecent ) {
            if( ( DateTime.Now - LastSelect ).TotalSeconds < 0.5 ) return;
            LastSelect = DateTime.Now;

            switch( selectResult.Type ) {
                case VFXSelectType.Local: // LOCAL
                    bool localResult = GetLocalFile( selectResult.Path, out var localAvfx );
                    if( localResult ) {
                        LoadVFX( localAvfx );
                    }
                    else {
                        PluginLog.Log( "Could not get file: " + selectResult.Path );
                        return;
                    }
                    break;
                default: // EVERYTHING ELSE: GAME FILES
                    bool gameResult = GetGameFile( selectResult.Path, out var gameAvfx );
                    if( gameResult ) {
                        LoadVFX( gameAvfx );
                    }
                    else {
                        PluginLog.Log( "Could not get file: " + selectResult.Path );
                        return;
                    }
                    break;
            }
            if( addToRecent ) Configuration.AddRecent( selectResult );
            Doc.UpdateSource( selectResult );
        }

        public void RemoveSourceVFX() {
            Doc.UpdateSource( VFXSelectResult.None() );
            Doc.ResetDoc();
            UnloadAVFX();
        }

        public void SetReplaceVFX( VFXSelectResult replaceResult) {
            SetReplaceVFX( replaceResult, true );
        }
        public void SetReplaceVFX( VFXSelectResult replaceResult, bool addToRecent ) {
            if( addToRecent ) Configuration.AddRecent( replaceResult );
            Doc.UpdateReplace( replaceResult );
        }

        public void RemoveReplaceVFX() {
            Doc.UpdateReplace( VFXSelectResult.None() );
        }

        public void RefreshSelectedDocUI() {
            if( Doc.HasVFX() ) {
                RefreshVFXUI();
            }
            else {
                UnloadAVFX();
            }
        }

        public void LoadVFX( AVFXBase avfx ) {
            if( avfx == null ) return;
            AVFX = avfx;
            // ===============
            if( Configuration.VerifyOnLoad ) {
                var node = AVFX.ToAVFX();
                bool verifyResult = LastImportNode.CheckEquals( node, out List<string> messages );
                SetStatus( verifyResult );
                PluginLog.Log( $"[VERIFY RESULT]: {verifyResult}" );
                foreach( var m in messages ) {
                    PluginLog.Log( m );
                }
            }
            TexManager.Reset();
            RefreshVFXUI();
        }

        public void UnloadAVFX() {
            AVFX = null;
            CurrentVFXUI = null;
        }
    }
}
