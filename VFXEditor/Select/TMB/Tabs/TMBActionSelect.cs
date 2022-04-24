using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using Dalamud.Logging;
using ImGuiNET;
using VFXSelect.Select.Rows;

namespace VFXSelect.TMB {
    public class TMBActionSelect : TMBSelectTab<XivActionTmb, XivActionTmb> {
        private ImGuiScene.TextureWrap Icon;

        public TMBActionSelect( string parentId, string tabId, TMBSelectDialog dialog ) :
            base( parentId, tabId, SheetManager.ActionTmb, dialog ) {
        }

        protected override bool CheckMatch( XivActionTmb item, string searchInput ) {
            return Matches( item.Name, searchInput );
        }

        protected override void OnSelect() {
            LoadIcon( Selected.Icon, ref Icon );
        }

        protected override void DrawSelected( XivActionTmb loadedItem ) {
            if( loadedItem == null ) { return; }
            ImGui.Text( loadedItem.Name );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            DrawIcon( Icon );

            DrawPath( "Start Tmb Path", loadedItem.StartTmb, Id + "Start", Dialog, SelectResultType.GameAction, "ACTION", loadedItem.Name + " Start" );

            DrawPath( "End Tmb Path", loadedItem.EndTmb, Id + "End", Dialog, SelectResultType.GameAction, "ACTION", loadedItem.Name + " End" );

            DrawPath( "Hit Tmb Path", loadedItem.HitTmb, Id + "Hit", Dialog, SelectResultType.GameAction, "ACTION", loadedItem.Name + " Hit" );

            DrawPath( "Weapon Tmb Path", loadedItem.WeaponTmb, Id + "Weapon", Dialog, SelectResultType.GameAction, "ACTION", loadedItem.Name + " Weapon" );
        }

        protected override string UniqueRowTitle( XivActionTmb item ) {
            return item.Name + "##" + item.RowId;
        }
    }
}
