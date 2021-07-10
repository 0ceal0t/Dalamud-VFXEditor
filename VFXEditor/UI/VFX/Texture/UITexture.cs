using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Threading.Tasks;
using System.Numerics;
using Dalamud.Plugin;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
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
                        SavePngDialog( t );
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
            Plugin.ImportFileDialog( "DDS or PNG File (*.png;*.dds)|*.png*;*.dds*|All files (*.*)|*.*", "Select Image File.",
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

        public static void SavePngDialog(TexData t) {
            Plugin.SaveFileDialog( "PNG Image (*.png)|*.png*|All files (*.*)|*.*", "Select a Save Location.", "png",
                ( string path ) => {
                    try {
                        Bitmap bmp = new Bitmap( t.Width, t.Height );
                        for( int i = 0; i < t.Height; i++ ) {
                            for( int j = 0; j < t.Width; j++ ) {
                                int _idx = ( i * t.Width + j ) * 4;
                                int r = t.Data[_idx];
                                int g = t.Data[_idx + 1];
                                int b = t.Data[_idx + 2];
                                int a = t.Data[_idx + 3];
                                bmp.SetPixel( j, i, System.Drawing.Color.FromArgb( a, r, g, b ) );
                            }
                        }
                        bmp.Save( path, System.Drawing.Imaging.ImageFormat.Png );
                    }
                    catch( Exception ex ) {
                        PluginLog.LogError( ex, "Could not select an image location" );
                    }
                }
            );
        }

        public static void SaveDDSDialog(string newPath ) {
            Plugin.SaveFileDialog( "DDS Image (*.dds)|*.dds*|All files (*.*)|*.*", "Select a Save Location.", "dds",
                ( string path ) => {
                    try {
                        var texFile = TextureManager.Manager.GetTexture( newPath );
                        byte[] header = IOUtil.CreateDDSHeader( texFile.Header.Width, texFile.Header.Height, texFile.Header.Format, texFile.Header.Depth, texFile.Header.MipLevels );
                        byte[] data = texFile.GetDDSData();
                        byte[] writeData = new byte[header.Length + data.Length];
                        Buffer.BlockCopy( header, 0, writeData, 0, header.Length );
                        Buffer.BlockCopy( data, 0, writeData, header.Length, data.Length );
                        File.WriteAllBytes( path, writeData );
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
