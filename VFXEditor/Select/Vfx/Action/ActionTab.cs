using ImGuiNET;
using OtterGui.Raii;
using System.Linq;
using VfxEditor.Select.Shared;

namespace VfxEditor.Select.Vfx.Action {
    public class ActionTab : SelectTab<ActionRow, ParseAvfx> {
        public ActionTab( SelectDialog dialog, string name ) : this( dialog, name, "Vfx-Action" ) { }

        public ActionTab( SelectDialog dialog, string name, string stateId ) : base( dialog, name, stateId, SelectResultType.GameAction ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var sheet = Dalamud.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>()
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

        protected override void DrawSelected() {
            SelectUiUtils.DrawIcon( Icon );

            if( !string.IsNullOrEmpty( Loaded.OriginalPath ) ) {
                using( var _ = ImRaii.PushId( "CopyTmb" ) ) {
                    SelectUiUtils.Copy( Loaded.OriginalPath );
                }

                ImGui.SameLine();
                ImGui.Text( "TMB:" );
                ImGui.SameLine();
                SelectUiUtils.DisplayPath( Loaded.OriginalPath );
            }

            DrawPath( "Cast", Selected.CastVfxPath, $"{Selected.Name} Cast", true );
            DrawPath( "Start", Selected.StartVfxPath, $"{Selected.Name} Start", true );
            if( !string.IsNullOrEmpty( Loaded.OriginalPath ) ) {
                DrawPaths( "VFX", Loaded.VfxPaths, Selected.Name, true );
            }
        }

        protected override string GetName( ActionRow item ) => item.Name;
    }
}
