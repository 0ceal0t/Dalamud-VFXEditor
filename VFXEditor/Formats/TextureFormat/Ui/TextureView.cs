using Dalamud.Interface;
using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using System.Numerics;
using VfxEditor.Formats.TextureFormat.Textures;
using VfxEditor.Ui.Components.SplitViews;
using VfxEditor.Utils;

namespace VfxEditor.Formats.TextureFormat.Ui {
    public class TextureView : SplitView<TextureReplace> {
        public readonly List<TextureReplace> Textures;
        private string SearchText = "";

        public TextureView( List<TextureReplace> textures ) : base( "Textures" ) {
            Textures = textures;
            InitialWidth = 300;
        }

        // ==============

        public override void Draw() {
            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                if( ImGui.Button( FontAwesomeIcon.Plus.ToIconString() ) ) {
                    // TODO
                }
            }

            using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing ) ) {
                ImGui.SameLine();
            }

            if( UiUtils.IconButton( FontAwesomeIcon.Download, "Extract" ) ) {
                // TODO
            }

            ImGui.SameLine();
            ImGui.InputTextWithHint( "##Search", "Search", ref SearchText, 255 );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );
            ImGui.Separator();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );

            base.Draw();
        }

        protected override void DrawPreLeft() { }

        protected override void DrawLeftColumn() {
            if( Textures.Count == 0 ) {
                ImGui.TextDisabled( "No textures have been replaced..." );
                return;
            }

            for( var idx = 0; idx < Textures.Count; idx++ ) {
                using var _ = ImRaii.PushId( idx );
                using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing );

                var item = Textures[idx];
                if( !string.IsNullOrEmpty( SearchText ) && !item.Matches( SearchText ) ) continue;
                var name = item.GetExportReplace();

                if( ImGui.Selectable( "##{Name}", item == Selected, ImGuiSelectableFlags.SpanAllColumns ) ) Selected = item;
                ImGui.SameLine();
                DrawHd( item.IsHd() );
                ImGui.SameLine();
                ImGui.Text( name );
            }

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );
        }

        protected override void DrawRightColumn() => Selected?.DrawBody();

        public void ClearSelected() { Selected = null; }

        private static void DrawHd( bool isHd ) {
            var pos = ImGui.GetCursorScreenPos() + new Vector2( 0, 4 );
            ImGui.Dummy( new Vector2( 15, 10 ) );

            if( !isHd ) return;

            var drawList = ImGui.GetWindowDrawList();
            drawList.AddRectFilled( pos, pos + new Vector2( 15, 12 ), ImGui.GetColorU32( ImGuiCol.Text ), 2f );
            drawList.AddText( UiBuilder.DefaultFont, 12, pos + new Vector2( 1, -1 ), 0xFF000000, "HD" );
        }
    }
}
