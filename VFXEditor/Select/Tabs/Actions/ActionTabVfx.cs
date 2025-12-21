using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using Lumina.Excel.Sheets;
using System.Collections.Generic;
using System.Linq;

namespace VfxEditor.Select.Tabs.Actions {
    public class ActionTabVfx : SelectTab<ActionRowVfx, ParsedPaths> {
        public ActionTabVfx( SelectDialog dialog, string name ) : this( dialog, name, "Action-Vfx" ) { }

        public ActionTabVfx( SelectDialog dialog, string name, string stateId ) : base( dialog, name, stateId ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var sheet = Dalamud.DataManager.GetExcelSheet<Action>()
                .Where( x => !string.IsNullOrEmpty( x.Name.ExtractText() ) && ( x.IsPlayerAction || x.ClassJob.ValueNullable != null ) );
            foreach( var item in sheet ) {
                var action = new ActionRowVfx( item );
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
            if( !string.IsNullOrEmpty( Loaded.OriginalPath ) ) {
                using( var _ = ImRaii.PushId( "CopyTmb" ) ) {
                    SelectUiUtils.Copy( Loaded.OriginalPath );
                }

                ImGui.SameLine();
                ImGui.Text( "TMB:" );
                ImGui.SameLine();
                SelectUiUtils.DisplayPath( Loaded.OriginalPath );
            }

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            Dialog.DrawPaths( new Dictionary<string, string>() {
                { "Cast", Selected.CastVfxPath },
                { "Start", Selected.StartVfxPath }
            }, string.IsNullOrEmpty( Loaded.OriginalPath ) ? [] : Loaded.Paths, Selected.Name, SelectResultType.GameAction );
        }
    }
}