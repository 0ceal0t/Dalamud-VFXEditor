using ImGuiNET;
using System;
using System.Numerics;
using Dalamud.Logging;
using ImGuiFileDialog;
using VFXEditor.Helper;
using VFXEditor.AVFXLib.Texture;
using System.IO;

namespace VFXEditor.Avfx.Vfx {
    public class UITexture : UINode {
        public AvfxFile Main;
        public AVFXTexture Texture;
        public string lastValue;
        public UIString Path;
        public UINodeGraphView NodeView;

        public UITexture( AvfxFile main, AVFXTexture texture ) : base( UINodeGroup.TextureColor, false ) {
            Main = main;
            Texture = texture;
            NodeView = new UINodeGraphView( this );

            Path = new UIString( "Path", Texture.Path );
            lastValue = Texture.Path.GetValue();
            Plugin.TextureManager.LoadPreviewTexture( Texture.Path.GetValue() );
            HasDependencies = false; // if imported, all set now
        }

        public string LoadTex() {
            var currentPathValue = Path.Literal.GetValue();
            if( currentPathValue != lastValue ) {
                lastValue = currentPathValue;
                Plugin.TextureManager.LoadPreviewTexture( currentPathValue );
            }
            return currentPathValue;
        }

        public override void DrawBody( string parentId ) {
            var id = parentId + "/Texture";
            NodeView.Draw( id );
            DrawRename( id );
            Path.Draw( id );

            var currentPathValue = LoadTex();

            if( Plugin.TextureManager.GetTexturePreview( currentPathValue, out var t ) ) {
                ImGui.Image( t.Wrap.ImGuiHandle, new Vector2( t.Width, t.Height ) );
                ImGui.Text( $"Format: {t.Format}  MIPS: {t.MipLevels}  SIZE: {t.Width}x{t.Height}" );
                if( ImGui.Button( "Export" + id ) ) {
                    ImGui.OpenPopup( "Tex_Export" + id );
                }
                ImGui.SameLine();
                if( ImGui.Button( "Replace" + id ) ) {
                    ImportDialog( currentPathValue.Trim( '\0' ) );
                }
                if( ImGui.BeginPopup( "Tex_Export" + id ) ) {
                    if( ImGui.Selectable( "Png" + id ) ) {
                        SavePngDialog( currentPathValue.Trim( '\0' ) );
                    }
                    if( ImGui.Selectable( "DDS" + id ) ) {
                        SaveDDSDialog( currentPathValue.Trim( '\0' ) );
                    }
                    ImGui.EndPopup();
                }

                // ===== IMPORTED TEXTURE =======
                if( t.IsReplaced ) {
                    ImGui.TextColored( UiHelper.RED_COLOR, "Replaced with imported texture" );
                    ImGui.SameLine();
                    if( UiHelper.RemoveButton( "Remove" + id, small: true ) ) {
                        Plugin.TextureManager.RemoveReplaceTexture( currentPathValue.Trim( '\0' ) );
                        Plugin.TextureManager.RefreshPreviewTexture( currentPathValue.Trim( '\0' ) );
                    }
                }
            }
        }

        public override void ShowTooltip() {
            var currentPathValue = LoadTex();

            if( Plugin.TextureManager.GetTexturePreview( currentPathValue, out var t ) ) {
                ImGui.BeginTooltip();
                ImGui.Image( t.Wrap.ImGuiHandle, new Vector2( t.Width, t.Height ) );
                ImGui.EndTooltip();
            }
        }

        private static void ImportDialog( string newPath ) {
            FileDialogManager.OpenFileDialog( "Select a File", "Image files{.png,.atex,.dds},.*", ( bool ok, string res ) => {
                if( !ok ) return;
                try {
                    if( !Plugin.TextureManager.AddReplaceTexture( res, newPath ) ) {
                        PluginLog.Error( $"Could not import" );
                    }
                }
                catch( Exception e ) {
                    PluginLog.Error( "Could not import data", e );
                }
            } );
        }

        private static void SavePngDialog( string texPath ) {
            FileDialogManager.SaveFileDialog( "Select a Save Location", ".png", "ExportedTexture", "png", ( bool ok, string res ) => {
                if( !ok ) return;
                var texFile = Plugin.TextureManager.GetRawTexture( texPath );
                texFile.SaveAsPNG( res );
            } );
        }

        private static void SaveDDSDialog( string texPath ) {
            FileDialogManager.SaveFileDialog( "Select a Save Location", ".dds", "ExportedTexture", "dds", ( bool ok, string res ) => {
                if( !ok ) return;
                var texFile = Plugin.TextureManager.GetRawTexture( texPath );
                texFile.SaveAsDDS( res );
            } );
        }

        public override string GetDefaultText() => $"Texture {Idx}";

        public override string GetWorkspaceId() => $"Tex{Idx}";

        public override void Write( BinaryWriter writer ) => Texture.Write( writer );
    }
}
