using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using Dalamud.Interface.Utility.Raii;
using System.Linq;

namespace VfxEditor.Select.Tabs.Actions {
    public class ActionTabVfx : SelectTab<ActionRowVfx, ParsedPaths> {
        public ActionTabVfx( SelectDialog dialog, string name ) : this( dialog, name, "Action-Vfx" ) { }

        public ActionTabVfx( SelectDialog dialog, string name, string stateId ) : base( dialog, name, stateId, SelectResultType.GameAction ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var sheet = Dalamud.DataManager.GetExcelSheet<Action>()
                .Where( x => !string.IsNullOrEmpty( x.Name ) && ( x.IsPlayerAction || x.ClassJob.Value != null ) );
            foreach( var item in sheet ) {
                var action = new ActionRowVfx( item, false );
                Items.Add( action );
                if( action.HitAction != null ) Items.Add( action.HitAction );
            }
        }

        public override void LoadSelection( ActionRowVfx item, out ParsedPaths loaded ) {
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

        protected override string GetName( ActionRowVfx item ) => item.Name;
    }
}