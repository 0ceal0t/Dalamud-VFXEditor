using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Numerics;
using Dalamud.Logging;
using VFXEditor.Texture;
using ImGuiFileDialog;

namespace VFXEditor.UI.VFX
{
    public class UITexture : UINode {
        public UIMain Main;
        public AVFXTexture Texture;

        public string lastValue;
        public UIString Path;
        public UINodeGraphView NodeView;

        public UITexture(UIMain main, AVFXTexture texture ) : base( UINodeGroup.TextureColor, false ) {
            Main = main;
            Texture = texture;
            NodeView = new UINodeGraphView( this );

            Path = new UIString( "Path", Texture.Path);
            lastValue = Texture.Path.Value;
            TextureManager.Manager.LoadPreviewTexture( Texture.Path.Value );
            HasDependencies = false; // if imported, all set now
        }

        public string LoadTex() {
            var currentPathValue = Path.Literal.Value;
            if( currentPathValue != lastValue ) {
                lastValue = currentPathValue;
                TextureManager.Manager.LoadPreviewTexture( currentPathValue );
            }
            return currentPathValue;
        }

        public override void DrawBody( string parentId ) {
            var id = parentId + "/Texture";
            NodeView.Draw( id );
            DrawRename( id );
            Path.Draw( id );

            var currentPathValue = LoadTex();

            if (TextureManager.Manager.GetTexturePreview(currentPathValue, out var t)) {
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
                    ImGui.TextColored( UIUtils.RED_COLOR, "Replaced with imported texture" );
                    ImGui.SameLine();
                    if( UIUtils.RemoveButton( "Remove" + id, small: true ) ) {
                        TextureManager.Manager.RemoveReplaceTexture( currentPathValue.Trim( '\0' ) );
                        TextureManager.Manager.RefreshPreviewTexture( currentPathValue.Trim( '\0' ) );
                    }
                }
            }
        }

        public override void ShowTooltip() {
            var currentPathValue = LoadTex();

            if( TextureManager.Manager.GetTexturePreview( currentPathValue, out var t ) ) {
                ImGui.BeginTooltip();
                ImGui.Image( t.Wrap.ImGuiHandle, new Vector2( t.Width, t.Height ) );
                ImGui.EndTooltip();
            }
        }

        public static void ImportDialog(string newPath) {
            FileDialogManager.OpenFileDialog( "Select a File", "Image files{.png,.atex,.dds},.*", ( bool ok, string res ) =>
            {
                if( !ok ) return;
                try {
                    if( !TextureManager.Manager.AddReplaceTexture( res, newPath ) ) {
                        PluginLog.Error( $"Could not import" );
                    }
                }
                catch( Exception e ) {
                    PluginLog.Error( "Could not import data", e );
                }
            } );
        }

        public static void SavePngDialog(string texPath) {
            FileDialogManager.SaveFileDialog( "Select a Save Location", ".png", "ExportedTexture", "png", ( bool ok, string res ) =>
            {
                if( !ok ) return;
                var texFile = TextureManager.Manager.GetRawTexture( texPath );
                texFile.SaveAsPNG( res );
            } );
        }

        public static void SaveDDSDialog(string texPath ) {
            FileDialogManager.SaveFileDialog( "Select a Save Location", ".dds", "ExportedTexture", "dds", ( bool ok, string res ) =>
            {
                if( !ok ) return;
                var texFile = TextureManager.Manager.GetRawTexture( texPath );
                texFile.SaveAsDDS( res );
            } );
        }

        public override string GetDefaultText() {
            return "Texture " + Idx;
        }

        public override string GetWorkspaceId() {
            return $"Tex{Idx}";
        }

        public override byte[] ToBytes() {
            return Texture.ToAVFX().ToBytes();
        }
    }
}
