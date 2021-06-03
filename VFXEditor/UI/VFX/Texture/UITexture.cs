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
    public class UITexture : UINode
    {
        public TextureManager Manager;
        public UITextureView View;
        public AVFXTexture Texture;
        public string lastValue;
        public UIString Path;
        public UINodeGraphView NodeView;

        public UITexture(AVFXTexture texture, UITextureView view ) : base( UINodeGroup.TextureColor, false ) {
            View = view;
            Texture = texture;
            NodeView = new UINodeGraphView( this );
            Manager = view.Manager;
            // ================
            Path = new UIString( "Path", Texture.Path);
            lastValue = Texture.Path.Value;
            if( view.GetPreviewTexture() ) {
                Manager.LoadTexture( Texture.Path.Value );
            }
            HasDependencies = false; // if imported, all set now
        }

        public string LoadTex() {
            var currentPathValue = Path.Literal.Value;
            if( currentPathValue != lastValue ) {
                lastValue = currentPathValue;
                if( View.GetPreviewTexture() ) {
                    Manager.LoadTexture( currentPathValue );
                }
            }
            return currentPathValue;
        }

        public override void DrawBody( string parentId )
        {
            string id = parentId + "/Texture";
            NodeView.Draw( id );
            Path.Draw( id );

            var currentPathValue = LoadTex();

            if( View.GetPreviewTexture() ) {
                if( Manager.PathToTex.ContainsKey( currentPathValue ) ) {
                    var t = Manager.PathToTex[currentPathValue];
                    ImGui.Image( t.Wrap.ImGuiHandle, new Vector2( t.Width, t.Height ) );
                    ImGui.Text( $"Format: {t.Format}  MIPS: {t.MipLevels}  SIZE: {t.Width}x{t.Height}" );
                    if(ImGui.SmallButton("Export" + id ) ) {
                        ImGui.OpenPopup( "Tex_Export" + id );
                    }
                    ImGui.SameLine();
                    if( ImGui.SmallButton( "Replace" + id ) ) {
                        ImportDialog(Manager, currentPathValue.Trim('\0') );
                    }
                    if( ImGui.BeginPopup( "Tex_Export" + id ) ) {
                        if( ImGui.Selectable( "Png" + id ) ) {
                            SavePngDialog( t );
                        }
                        if( ImGui.Selectable( "DDS" + id ) ) {
                            SaveDDSDialog( Manager._plugin, currentPathValue.Trim( '\0' ) );
                        }
                        ImGui.EndPopup();
                    }
                    // ===== IMPORTED TEXTURE =======
                    if( t.IsReplaced ) {
                        ImGui.TextColored( new Vector4( 1.0f, 0.0f, 0.0f, 1.0f ), "Replaced with imported texture" );
                        ImGui.SameLine();
                        if( UIUtils.RemoveButton( "Remove" + id, small: true ) ) {
                            Manager.RemoveReplace( currentPathValue.Trim( '\0' ) );
                            Manager.RefreshPreview( currentPathValue.Trim( '\0' ) );
                        }
                    }
                }
            }
        }

        public override void ShowTooltip() {
            var currentPathValue = LoadTex();
            if( View.GetPreviewTexture() ) {
                if( Manager.PathToTex.ContainsKey( currentPathValue ) ) {
                    var t = Manager.PathToTex[currentPathValue];
                    ImGui.BeginTooltip();
                    ImGui.Image( t.Wrap.ImGuiHandle, new Vector2( t.Width, t.Height ) );
                    ImGui.EndTooltip();
                }
            }
        }

        public static void ImportDialog(TextureManager manager, string path) {
            Task.Run( async () => {
                var picker = new OpenFileDialog {
                    Filter = "DDS or PNG File (*.png;*.dds)|*.png*;*.dds*|All files (*.*)|*.*",
                    CheckFileExists = true,
                    Title = "Select Image File."
                };
                var result = await picker.ShowDialogAsync();
                if( result == DialogResult.OK ) {
                    try {
                        if( !manager.ImportTexture( picker.FileName, path ) ) {
                            PluginLog.Log( $"Could not import" );
                        }
                    }
                    catch( Exception ex ) {
                        PluginLog.LogError( ex, "Could not select an image location" );
                    }
                }
            } );
        }

        public static void SavePngDialog(TexData t) {
            Task.Run( async () => {
                var picker = new SaveFileDialog {
                    Filter = "PNG Image (*.png)|*.png*|All files (*.*)|*.*",
                    Title = "Select a Save Location.",
                    DefaultExt = "png",
                    AddExtension = true
                };
                var result = await picker.ShowDialogAsync();
                if( result == DialogResult.OK ) {
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
                        bmp.Save( picker.FileName, System.Drawing.Imaging.ImageFormat.Png );
                    }
                    catch( Exception ex ) {
                        PluginLog.LogError( ex, "Could not select an image location" );
                    }
                }
            });
        }

        public static void SaveDDSDialog(Plugin _plugin, string path ) {
            Task.Run( async () => {
                var picker = new SaveFileDialog
                {
                    Filter = "DDS Image (*.dds)|*.dds*|All files (*.*)|*.*",
                    Title = "Select a Save Location.",
                    DefaultExt = "dds",
                    AddExtension = true
                };
                var result = await picker.ShowDialogAsync();
                if( result == DialogResult.OK ) {
                    try {
                        var texFile = _plugin.Manager.TexManager.GetTexture( path );
                        byte[] header = Data.Texture.IOUtil.CreateDDSHeader( texFile.Header.Width, texFile.Header.Height, texFile.Header.Format, texFile.Header.Depth, texFile.Header.MipLevels );
                        byte[] data = texFile.GetDDSData();
                        byte[] writeData = new byte[header.Length + data.Length];
                        Buffer.BlockCopy( header, 0, writeData, 0, header.Length );
                        Buffer.BlockCopy( data, 0, writeData, header.Length, data.Length );
                        File.WriteAllBytes( picker.FileName, writeData );
                    }
                    catch( Exception ex ) {
                        PluginLog.LogError( ex, "Could not select an image location" );
                    }
                }
            } );
        }

        public override string GetText() {
            return "Texture " + Idx;
        }

        public override byte[] ToBytes() {
            return Texture.ToAVFX().ToBytes();
        }
    }
}
