using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Numerics;
using Dalamud.Plugin;
using VFXEditor.Data.Texture;

namespace VFXEditor.UI.VFX
{
    public class UITexture : UINode {
        public UIMain Main;
        public AVFXTexture Texture;
        //===========================
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
            string id = parentId + "/Texture";
            NodeView.Draw( id );
            DrawRename( id );
            Path.Draw( id );

            var currentPathValue = LoadTex();

            if( TextureManager.Manager.PathToTexturePreview.ContainsKey( currentPathValue ) ) {
                var t = TextureManager.Manager.PathToTexturePreview[currentPathValue];
                ImGui.Image( t.Wrap.ImGuiHandle, new Vector2( t.Width, t.Height ) );
                ImGui.Text( $"Format: {t.Format}  MIPS: {t.MipLevels}  SIZE: {t.Width}x{t.Height}" );
                if( ImGui.SmallButton( "Export" + id ) ) {
                    ImGui.OpenPopup( "Tex_Export" + id );
                }
                ImGui.SameLine();
                if( ImGui.SmallButton( "Replace" + id ) ) {
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
                    ImGui.TextColored( new Vector4( 1.0f, 0.0f, 0.0f, 1.0f ), "Replaced with imported texture" );
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
            if( TextureManager.Manager.PathToTexturePreview.ContainsKey( currentPathValue ) ) {
                var t = TextureManager.Manager.PathToTexturePreview[currentPathValue];
                ImGui.BeginTooltip();
                ImGui.Image( t.Wrap.ImGuiHandle, new Vector2( t.Width, t.Height ) );
                ImGui.EndTooltip();
            }
        }

        public static void ImportDialog(string newPath) {
            Plugin.ImportFileDialog( "DDS, ATEX, or PNG File (*.png;*.atex;*.dds)|*.png*;*.atex;*.dds*|All files (*.*)|*.*", "Select Image File.",
                ( string path ) => {
                    try {
                        if( !TextureManager.Manager.ImportReplaceTexture( path, newPath ) ) {
                            PluginLog.Log( $"Could not import" );
                        }
                    }
                    catch( Exception ex ) {
                        PluginLog.LogError( ex, "Could not select an image location" );
                    }
                }
            );
        }

        public static void SavePngDialog(string texPath) {
            Plugin.SaveFileDialog( "PNG Image (*.png)|*.png*|All files (*.*)|*.*", "Select a Save Location.", "png",
                ( string path ) => {
                    try {
                        var texFile = TextureManager.Manager.GetTexture( texPath );
                        texFile.SaveAsPng( path );
                    }
                    catch( Exception ex ) {
                        PluginLog.LogError( ex, "Could not select an image location" );
                    }
                }
            );
        }

        public static void SaveDDSDialog(string texPath ) {
            Plugin.SaveFileDialog( "DDS Image (*.dds)|*.dds*|All files (*.*)|*.*", "Select a Save Location.", "dds",
                ( string path ) => {
                    try {
                        var texFile = TextureManager.Manager.GetTexture( texPath );
                        texFile.SaveAsDDS( path );
                    }
                    catch( Exception ex ) {
                        PluginLog.LogError( ex, "Could not select an image location" );
                    }
                }
            );
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
