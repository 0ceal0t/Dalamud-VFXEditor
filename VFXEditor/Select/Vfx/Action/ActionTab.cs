using Dalamud.Logging;
using ImGuiNET;
using System;
using System.Linq;
using VfxEditor.Select.Shared;

namespace VfxEditor.Select.Vfx.Action {
    public class ActionTab : SelectTab<ActionRow, ParseAvfx> {
        public ActionTab( SelectDialog dialog, string name, string stateId ) : base( dialog, name, stateId ) { }
        public ActionTab( SelectDialog dialog, string name ) : base( dialog, name, "Vfx-Action" ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var sheet = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>()
                .Where( x => !string.IsNullOrEmpty( x.Name ) && ( x.IsPlayerAction || x.ClassJob.Value != null ) );

            foreach( var item in sheet ) {
                var actionItem = new ActionRow( item, false );
                if( actionItem.HasVfx ) Items.Add( actionItem );
                if( actionItem.HitAction != null ) Items.Add( actionItem.HitAction );
            }
        }

        public override void LoadSelection( ActionRow item, out ParseAvfx loaded ) {
            if( string.IsNullOrEmpty( item.SelfTmbKey ) ) { // no need to get the file
                loaded = new ParseAvfx();
                return;
            }

            ParseAvfx.ReadFile( item.TmbPath, out loaded );
        }

        // ===== DRAWING ======

        protected override void OnSelect() => LoadIcon( Selected.Icon );

        protected override void DrawSelected( string parentId ) {
            SelectTabUtils.DrawIcon( Icon );

            if( !string.IsNullOrEmpty( Loaded.OriginalPath ) ) {
                SelectTabUtils.Copy( Loaded.OriginalPath, $"{parentId}/CopyTmb" );
                ImGui.SameLine();
                ImGui.Text( "TMB:" );
                ImGui.SameLine();
                SelectTabUtils.DisplayPath( Loaded.OriginalPath );
            }

            Dialog.DrawPath( "Cast", Selected.CastVfxPath, $"{parentId}/Cast", SelectResultType.GameAction, $"{Selected.Name} Cast", true );

            if( !string.IsNullOrEmpty( Loaded.OriginalPath ) ) {
                Dialog.DrawPath( "VFX", Loaded.VfxPaths, parentId, SelectResultType.GameAction, Selected.Name, true );
            }
        }

        protected override string GetName( ActionRow item ) => item.Name;
    }
}
