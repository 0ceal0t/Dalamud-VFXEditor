using ImGuiNET;
using System.Numerics;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.PapSelect {
    public class PapEmoteSelect : SelectTab<XivEmotePap, XivEmotePapSelected> {
        private ImGuiScene.TextureWrap Icon;

        public PapEmoteSelect( string tabId, PapSelectDialog dialog ) : base( tabId, SheetManager.EmotePap, dialog ) { }

        protected override void OnSelect() => LoadIcon( Selected.Icon, ref Icon );

        protected override void DrawSelected( string parentId ) {
            DrawIcon( Icon );

            ImGui.BeginTabBar( "EmotePapTabs" );
            foreach( var action in Loaded.AllPaps ) {
                if( !ImGui.BeginTabItem( $"{action.Key}{parentId}/Emote" ) ) continue;
                ImGui.BeginChild( "EmotePapChild", new Vector2( -1 ), false );

                if( action.Value.TryGetValue( "Action", out var actionDict ) ) DrawPapDict( actionDict, "", $"{Loaded.EmotePap.Name} {action.Key}", parentId );
                else {
                    foreach( var subItem in action.Value ) {
                        if( subItem.Value.Count == 0 ) continue;
                        if( ImGui.CollapsingHeader( subItem.Key ) ) {
                            ImGui.Indent();
                            DrawPapDict( subItem.Value, "", $"{Loaded.EmotePap.Name} {action.Key} {subItem.Key}", parentId );
                            ImGui.Unindent();
                        }
                    }
                }

                ImGui.EndChild();
                ImGui.EndTabItem();
            }
            ImGui.EndTabBar();
        }

        protected override string GetName( XivEmotePap item ) => item.Name;
    }
}
