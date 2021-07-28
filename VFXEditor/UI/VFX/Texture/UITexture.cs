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
            if( TextureManager.Manager.PathToTexturePreview.ContainsKey( currentPathValue ) ) {
                var t = TextureManager.Manager.PathToTexturePreview[currentPathValue];
                ImGui.BeginTooltip();
                ImGui.Image( t.Wrap.ImGuiHandle, new Vector2( t.Width, t.Height ) );
                ImGui.EndTooltip();
            }
        }

        public static void ImportDialog(string newPath) {
            Plugin.DialogManager.OpenFileDialog( "Select a File", "Image files{.png,.atex,.dds},.*", ( bool ok, string res ) =>
            {
                if( !ok ) return;
                try {
                    if( !TextureManager.Manager.ImportReplaceTexture( res, newPath ) ) {
                        PluginLog.Log( $"Could not import" );
                    }
                }
                catch( Exception e ) {
                    PluginLog.LogError( "Could not import data", e );
                }
            } );
        }

        public static void SavePngDialog(string texPath) {
            Plugin.DialogManager.SaveFileDialog( "Select a Save Location", ".png", "ExportedTexture", "png", ( bool ok, string res ) =>
            {
                if( !ok ) return;
                var texFile = TextureManager.Manager.GetTexture( texPath );
                texFile.SaveAsPng( res );
            } );
        }

        public static void SaveDDSDialog(string texPath ) {
            Plugin.DialogManager.SaveFileDialog( "Select a Save Location", ".dds", "ExportedTexture", "dds", ( bool ok, string res ) =>
            {
                if( !ok ) return;
                var texFile = TextureManager.Manager.GetTexture( texPath );
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
