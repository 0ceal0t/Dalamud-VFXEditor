using ImGuiNET;
using OtterGui.Raii;
using System.Linq;
using VfxEditor.Select.Shared;

namespace VfxEditor.Select.Vfx.Action {
    public class ActionTab : SelectTab<ActionRow, ParsedPaths> {
        public ActionTab( SelectDialog dialog, string name ) : this( dialog, name, "Vfx-Action" ) { }

        public ActionTab( SelectDialog dialog, string name, string stateId ) : base( dialog, name, stateId, SelectResultType.GameAction ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var sheet = Dalamud.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>()
                .Where( x => !string.IsNullOrEmpty( x.Name ) && ( x.IsPlayerAction || x.ClassJob.Value != null ) );
            foreach( var item in sheet ) {
                var action = new ActionRow( item, false );
                Items.Add( action );
                if( action.HitAction != null ) Items.Add( action.HitAction );
            }
        }

        public override void LoadSelection( ActionRow item, out ParsedPaths loaded ) {
            if( string.IsNullOrEmpty( item.TmbPath ) ) { // no need to get the file
                loaded = new ParsedPaths();
                return;
            }

            ParsedPaths.ReadFile( item.TmbPath, SelectDataUtils.AvfxRegex, out loaded );
        }

        // ===== DRAWING ======

        protected override void DrawSelected() {
            DrawIcon( Selected.Icon );
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
                DrawPaths( "VFX", Loaded.Paths, Selected.Name, true );
            }
        }

        protected override string GetName( ActionRow item ) => item.Name;
    }
}
