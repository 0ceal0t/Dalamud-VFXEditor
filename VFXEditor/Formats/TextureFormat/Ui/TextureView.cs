using Dalamud.Interface;
using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using System.Numerics;
using VfxEditor.Formats.TextureFormat.Textures;
using VfxEditor.Select;
using VfxEditor.Select.Tex;
using VfxEditor.Ui.Components.SplitViews;
using VfxEditor.Ui.Interfaces;
using VfxEditor.Utils;

namespace VfxEditor.Formats.TextureFormat.Ui {
    public class TextureView : SplitView<TextureReplace>, IDraggableList<TextureReplace> {
        public readonly List<TextureReplace> Textures;
        private readonly TexSelectDialog ExtractSelect;
        private readonly TexSelectDialog ImportSelect;

        private TextureReplace DraggingItem;
        private string SearchText = "";
        private ExtractFileType ExtractType = ExtractFileType.Atex_Tex;

        private static readonly TextureFormat[] ValidPngFormat = new[] {
            TextureFormat.DXT5,
            TextureFormat.DXT3,
            TextureFormat.DXT1,
            TextureFormat.A8R8G8B8,
        };

        private enum ExtractFileType {
            Atex_Tex,
            Png,
            Dds,
        }

        private static readonly ExtractFileType[] ExtractTypes = new[] {
            ExtractFileType.Atex_Tex,
            ExtractFileType.Png,
            ExtractFileType.Dds
        };

        public TextureView( TextureManager manager, List<TextureReplace> textures ) : base( "Textures" ) {
            Textures = textures;
            InitialWidth = 300;
            ExtractSelect = new( "Texture Extract", manager, false, Extract );
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

            using( var popup = ImRaii.Popup( "ImportTex" ) ) {
                if( popup ) {
                    using var child = ImRaii.Child( "Child", new Vector2( 500, 500 ) );

                    if( ImGui.InputInt( ".png Mips", ref Plugin.Configuration.PngMips ) ) Plugin.Configuration.Save();
                    if( UiUtils.EnumComboBox( ".png Format", ValidPngFormat, Plugin.Configuration.PngFormat, out var newPngFormat ) ) {
                        Plugin.Configuration.PngFormat = newPngFormat;
                        Plugin.Configuration.Save();
                    }

                    ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );
                    ImGui.Separator();
                    ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );
                    ImportSelect.DrawBody();
                }
            }

            // ====== EXTRACT =========

            using( var popup = ImRaii.Popup( "ExtractTex" ) ) {
                if( popup ) {
                    using var child = ImRaii.Child( "Child", new Vector2( 500, 500 ) );

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

            base.Draw();
        }

        protected override void DrawPreLeft() { }

        protected override void DrawLeftColumn() {
            if( Textures.Count == 0 ) {
                ImGui.TextDisabled( "No textures have been replaced..." );
                return;
            }

            for( var idx = 0; idx < Textures.Count; idx++ ) {
                var item = Textures[idx];
                if( !string.IsNullOrEmpty( SearchText ) && !item.Matches( SearchText ) ) continue;
                var name = item.GetExportReplace();

                using( var _ = ImRaii.PushId( idx ) ) {
                    using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing );

                    if( ImGui.Selectable( "##{Name}", item == Selected, ImGuiSelectableFlags.SpanAllColumns ) ) Selected = item;
                }

                if( IDraggableList<TextureReplace>.DrawDragDrop( this, item, $"TEXTUREVIEW-SPLIT" ) ) break;

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

        public void ClearSelected() { Selected = null; }

        public static void DrawHd( bool isHd ) {
            var pos = ImGui.GetCursorScreenPos() + new Vector2( 0, 4 );
            ImGui.Dummy( new Vector2( 15, 10 ) );

            if( !isHd ) return;

            var drawList = ImGui.GetWindowDrawList();
            drawList.AddRectFilled( pos, pos + new Vector2( 15, 12 ), ImGui.GetColorU32( ImGuiCol.Text ), 2f );
            drawList.AddText( UiBuilder.DefaultFont, 12, pos + new Vector2( 1, -1 ), 0xFF000000, "HD" );
        }


        public void Extract( SelectResult result ) {
            if( !Plugin.DataManager.FileExists( result.Path ) ) return;
            var ext = result.Path.Split( '.' )[^1].ToLower();
            var file = Plugin.DataManager.GetFile<TextureDataFile>( result.Path );

            Plugin.TextureManager.AddRecent( result );

            if( ExtractType == ExtractFileType.Dds ) file.SaveDdsDialog();
            else if( ExtractType == ExtractFileType.Png ) file.SavePngDialog();
            else file.SaveTexDialog( ext );
        }

        public static void Import( SelectResult result ) => Plugin.TextureManager.Import( result );

        // ===== DRAG/DROP =======

        TextureReplace IDraggableList<TextureReplace>.GetDraggingItem() => DraggingItem;

        void IDraggableList<TextureReplace>.SetDraggingItem( TextureReplace item ) => DraggingItem = item;

        List<TextureReplace> IDraggableList<TextureReplace>.GetItems() => Textures;

        string IDraggableList<TextureReplace>.GetDraggingText( TextureReplace item ) => item.GetExportReplace();
    }
}
