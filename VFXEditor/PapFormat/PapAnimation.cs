using ImGuiFileDialog;
using ImGuiNET;
using System.IO;
using VfxEditor.Utils;
using VfxEditor.Interop;
using VfxEditor.TmbFormat;

namespace VfxEditor.PapFormat {
    public class PapAnimation {
        public short HavokIndex = 0;

        private readonly string HkxTempLocation;
        private string Name = "cbbm_replace_this"; // padded to 32 bytes (INCLUDING NULL)
        private short Unknown1 = 0;
        private int Unknown2 = 0;
        private TmbFile Tmb;

        public PapAnimation( string hkxPath ) {
            HkxTempLocation = hkxPath;
        }

        public PapAnimation( BinaryReader reader, string hkxPath ) {
            HkxTempLocation = hkxPath;
            Name = FileUtils.ReadString( reader );
            reader.ReadBytes( 32 - Name.Length - 1 ); // name padded to 32 bytes. also account for trailing null
            Unknown1 = reader.ReadInt16();
            HavokIndex = reader.ReadInt16();
            Unknown2 = reader.ReadInt32();
        }

        public void Write( BinaryWriter writer ) {
            FileUtils.WriteString( writer, Name, true );
            for( var i = 0; i < ( 32 - Name.Length - 1 ); i++ ) {
                writer.Write( ( byte )0 );
            }
            writer.Write( Unknown1 );
            writer.Write( HavokIndex );
            writer.Write( Unknown2 );
        }

        public void ReadTmb( BinaryReader reader ) {
            Tmb = new TmbFile( reader, true );
        }

        public void ReadTmb( string path ) {
            Tmb = TmbFile.FromLocalFile( path, true );
        }

        public byte[] GetTmbBytes() => Tmb.ToBytes();

        public string GetName() => Name;

        public void Draw( string parentId, short modelId, SkeletonType modelType, byte variant ) {
            ImGui.InputText( $"Name{parentId}", ref Name, 31 );
            FileUtils.ShortInput( $"Unknown 1{parentId}", ref Unknown1 );
            ImGui.InputInt( $"Unknown 2{parentId}", ref Unknown2 );   

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            ImGui.Text( $"This animation has Havok index: {HavokIndex}" );

            if( ImGui.Button( $"Replace Havok data{parentId}" ) ) {
                FileDialogManager.OpenFileDialog( "Select a File", ".hkx,.*", ( bool ok, string res ) => {
                    if( ok ) {
                        PapManager.IndexDialog.OnOk = ( int idx ) => {
                            HavokInterop.ReplaceHavokAnimation( HkxTempLocation, HavokIndex, res, idx, HkxTempLocation );
                            UiUtils.OkNotification( "Havok data replaced" );
                        };
                        PapManager.IndexDialog.Show();
                    }
                } );
            }

            ImGui.BeginTabBar( "AnimationTabls" );
            DrawTmb( parentId );
            DrawAnimation3D( parentId, modelId, modelType, variant );
            ImGui.EndTabBar();
        }

        private void DrawTmb( string parentId ) {
            if( !ImGui.BeginTabItem( "TMB" + parentId ) ) return;

            if( ImGui.Button( $"Import TMB{parentId}" ) ) {
                FileDialogManager.OpenFileDialog( "Select a File", ".tmb,.*", ( bool ok, string res ) => {
                    if( ok ) {
                        Tmb = TmbFile.FromLocalFile( res, true );
                        UiUtils.OkNotification( "TMB data imported" );
                    }
                } );
            }

            ImGui.SameLine();
            if( ImGui.Button( $"Export TMB{parentId}" ) ) {
                UiUtils.WriteBytesDialog( ".tmb", Tmb.ToBytes(), "tmb" );
            }

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            Tmb.Draw( parentId + "/Tmb" );

            ImGui.EndTabItem();
        }

        private void DrawAnimation3D( string parentId, short modelId, SkeletonType modelType, byte variant ) {
            if( !ImGui.BeginTabItem( "3D View" + parentId ) ) return;
            Plugin.AnimationManager.Draw( this, HkxTempLocation, HavokIndex, modelId, modelType, variant );
            ImGui.EndTabItem();
        }
    }
}
