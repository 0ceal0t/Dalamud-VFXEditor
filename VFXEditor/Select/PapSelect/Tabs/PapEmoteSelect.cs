using ImGuiNET;
using System.Numerics;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.PapSelect {
    public class PapEmoteSelect : PapSelectTab<XivEmotePap, XivEmotePapSelected> {
        private ImGuiScene.TextureWrap Icon;

        public PapEmoteSelect( string parentId, string tabId, PapSelectDialog dialog ) :
            base( parentId, tabId, SheetManager.EmotePap, dialog ) {
        }

        protected override bool CheckMatch( XivEmotePap item, string searchInput ) => Matches( item.Name, searchInput );

        protected override void OnSelect() {
            LoadIcon( Selected.Icon, ref Icon );
        }

        protected override void DrawSelected( XivEmotePapSelected loadedItem ) {
            if( loadedItem == null ) { return; }
            ImGui.Text( loadedItem.EmotePap.Name );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            DrawIcon( Icon );

            ImGui.BeginTabBar( "EmotePapTabs" );
            foreach( var action in loadedItem.AllPaps ) {
                if( !ImGui.BeginTabItem( $"{action.Key}##EmotePap" ) ) continue;
                ImGui.BeginChild( "EmotePapChild", new Vector2( -1 ), false );

                if( action.Value.TryGetValue( "Action", out var actionDict ) ) {
                    DrawPapDict( actionDict, "", $"{loadedItem.EmotePap.Name} {action.Key}" );
                }
                else {
                    foreach( var subItem in action.Value ) {
                        if( subItem.Value.Count == 0 ) continue;
                        if( ImGui.CollapsingHeader( subItem.Key ) ) {
                            ImGui.Indent();
                            DrawPapDict( subItem.Value, "", $"{loadedItem.EmotePap.Name} {action.Key} {subItem.Key}" );
                            ImGui.Unindent();
                        }
                    }
                }

                ImGui.EndChild();
                ImGui.EndTabItem();
            }
            ImGui.EndTabBar();
        }

        protected override string UniqueRowTitle( XivEmotePap item ) => $"{item.Name}##{item.RowId}";
    }
}
