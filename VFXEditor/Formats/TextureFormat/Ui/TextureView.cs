using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VfxEditor.Formats.TextureFormat.Textures;
using VfxEditor.Select;
using VfxEditor.Select.Formats;
using VfxEditor.Ui.Components.SplitViews;
using VfxEditor.Utils;

namespace VfxEditor.Formats.TextureFormat.Ui {
    public class TextureView : SplitView<TextureReplace> {
        private readonly TexSelectDialog ExtractSelect;
        private readonly TexSelectDialog ImportSelect;

        private TextureReplace DraggingItem;
        private string SearchText = "";
        private ExtractFileType ExtractType = ExtractFileType.ATEX_TEX;

        private static readonly TextureFormat[] ValidPngFormat = [
            TextureFormat.DXT5,
            TextureFormat.DXT3,
            TextureFormat.DXT1,
            TextureFormat.A8R8G8B8,
            TextureFormat.BC5,
        ];

        private enum ExtractFileType {
            ATEX_TEX,
            PNG,
            DDS,
        }

        private static readonly ExtractFileType[] ExtractTypes = [
            ExtractFileType.ATEX_TEX,
            ExtractFileType.PNG,
            ExtractFileType.DDS
        ];

        public TextureView( TextureManager manager, List<TextureReplace> textures ) : base( "Textures", textures ) {
            InitialWidth = 300;
            ExtractSelect = new( "Texture Extract", manager, true, Extract );
            ImportSelect = new( "Texture Import", manager, false, Import );
        }

        // ==============

        public override void Draw() {
            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                if( ImGui.Button( FontAwesomeIcon.Plus.ToIconString() ) ) ImGui.OpenPopup( "ImportTex" );
            }

            using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing ) ) {
                ImGui.SameLine();
            }

            if( UiUtils.IconButton( FontAwesomeIcon.Download, "Extract" ) ) ImGui.OpenPopup( "ExtractTex" );

            ImGui.SameLine();
            ImGui.InputTextWithHint( "##Search", "Search", ref SearchText, 255 );

            // ==== IMPORT ==========

            using( var border = ImRaii.PushStyle( ImGuiStyleVar.PopupBorderSize, 1 ) )
            using( var popup = ImRaii.Popup( "ImportTex" ) ) {
                if( popup ) {
                    using var child = ImRaii.Child( "Child", new Vector2( 500, 500 ) );

                    DrawPngSettings();

                    ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );
                    ImGui.Separator();
                    ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );
                    ImportSelect.DrawBody();
                }
            }

            // ====== EXTRACT =========

            using( var border = ImRaii.PushStyle( ImGuiStyleVar.PopupBorderSize, 1 ) )
            using( var popup = ImRaii.Popup( "ExtractTex" ) ) {
                if( popup ) {
                    using var child = ImRaii.Child( "Child", new Vector2( 500, 500 ) );

                    ImGui.SetNextItemWidth( 150 );
                    if( UiUtils.EnumComboBox( "Format", ExtractTypes, ExtractType, out var newExtractType ) ) {
                        ExtractType = newExtractType;
                    }

                    ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );
                    ImGui.Separator();
                    ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );
                    ExtractSelect.DrawBody();
                }
            }

            // ====================

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );
            ImGui.Separator();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );

            if( !Items.Contains( Selected ) ) ClearSelected();

            base.Draw();
        }

        protected override void DrawPreLeft() { }

        protected override void DrawLeftColumn() {
            if( Items.Count == 0 ) {
                ImGui.TextDisabled( "No textures have been replaced..." );
                return;
            }

            for( var idx = 0; idx < Items.Count; idx++ ) {
                var item = Items[idx];
                if( !string.IsNullOrEmpty( SearchText ) && !item.Matches( SearchText ) ) continue;
                var name = item.GetExportReplace();

                using( var _ = ImRaii.PushId( idx ) ) {
                    using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing );

                    if( ImGui.Selectable( "##{Name}", item == Selected, ImGuiSelectableFlags.SpanAllColumns ) ) Selected = item;
                }

                if( UiUtils.DrawDragDrop( Items, item, item.GetExportReplace(), ref DraggingItem, $"TEXTUREVIEW-SPLIT", false ) ) break;

                using( var _ = ImRaii.PushId( idx ) ) {
                    ImGui.SameLine();
                    DrawHd( item.IsHd() );
                    ImGui.SameLine();
                    ImGui.Text( name );
                }
            }

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );
        }

        protected override void DrawRightColumn() => Selected?.DrawBody();

        public static void DrawHd( bool isHd ) {
            var pos = ImGui.GetCursorScreenPos() + new Vector2( 0, 4 );
            ImGui.Dummy( new Vector2( 15, 10 ) );

            if( !isHd ) return;

            var drawList = ImGui.GetWindowDrawList();
            drawList.AddRectFilled( pos, pos + new Vector2( 15, 12 ), ImGui.GetColorU32( ImGuiCol.Text ), 2f );
            drawList.AddText( UiBuilder.DefaultFont, 12, pos + new Vector2( 1, -1 ), 0xFF000000, "HD" );
        }


        public void Extract( SelectResult result ) {
            var file = result.Type == SelectResultType.Local ?
                ( Path.Exists( result.Path ) ? TextureDataFile.LoadFromLocal( result.Path ) : null ) :
                ( Dalamud.DataManager.FileExists( result.Path ) ? Dalamud.DataManager.GetFile<TextureDataFile>( result.Path ) : null );

            if( file == null ) return;

            if( ExtractType == ExtractFileType.DDS ) file.SaveDdsDialog();
            else if( ExtractType == ExtractFileType.PNG ) file.SavePngDialog();
            else file.SaveTexDialog( result.Path.Split( '.' )[^1].ToLower() );
        }

        public static void Import( SelectResult result ) => Plugin.TextureManager.Import( result );

        public static void DrawPngSettings() {
            ImGui.SetNextItemWidth( 150 );
            if( ImGui.InputInt( "Mips", ref Plugin.Configuration.PngMips ) ) Plugin.Configuration.Save();
            using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing ) ) {
                ImGui.SameLine();
            }
            ImGui.TextDisabled( "(.png)" );

            ImGui.SetNextItemWidth( 150 );
            if( UiUtils.EnumComboBox( "Format", ValidPngFormat, Plugin.Configuration.PngFormat, out var newPngFormat ) ) {
                Plugin.Configuration.PngFormat = newPngFormat;
                Plugin.Configuration.Save();
            }
            using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing ) ) {
                ImGui.SameLine();
            }
            ImGui.TextDisabled( "(.png)" );
        }
    }
}
