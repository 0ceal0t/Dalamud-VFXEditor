using ImGuiNET;
using System.Collections.Generic;
using VfxEditor.Select.Sheets;

namespace VfxEditor.Select.PapSelect {
    public abstract class PapSelectTab<T, S> : SelectTab<T, S> where T : class where S : class {
        protected readonly PapSelectDialog Dialog;

        public PapSelectTab( string parentId, string tabId, SheetLoader<T, S> loader, PapSelectDialog dialog ) : base( parentId, tabId, loader ) {
            Dialog = dialog;
        }

        public void DrawPapDict( Dictionary<string, string> items, string label, string name ) {
            foreach( var item in items ) {
                var skeleton = item.Key;
                var path = item.Value;

                ImGui.Text( $"{label} ({skeleton}): " );
                ImGui.SameLine();
                if( path.Contains( "action.pap" ) || path.Contains( "face.pap" ) ) {
                    DisplayPathWarning( path, "Be careful about modifying this file, as it contains dozens of animations for every job" );
                }
                else {
                    DisplayPath( path );
                }

                DrawPath( "", path, $"{Id}-{label}-{skeleton}", Dialog, SelectResultType.GameAction, "ACTION", $"{name} {label} ({skeleton})" );
            }
        }
    }
}