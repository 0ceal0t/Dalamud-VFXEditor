using ImGuiFileDialog;
using ImGuiNET;
using System.IO;
using VFXEditor.Utils;
using VFXEditor.Interop;
using VFXEditor.TmbFormat;

namespace VFXEditor.PapFormat {
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
            reader.ReadBytes( 32 - Name.Length - 1 );
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
            Tmb = new TmbFile( reader, false );
        }

        public void ReadTmb( string path ) {
            Tmb = TmbFile.FromLocalFile( path );
        }

        public byte[] GetTmbBytes() => Tmb.ToBytes();

        public string GetName() => Name;

        public void Draw( string id ) {
            ImGui.InputText( $"Name{id}", ref Name, 31 );
            FileUtils.ShortInput( $"Unknown 1{id}", ref Unknown1 );
            ImGui.InputInt( $"Unknown 2{id}", ref Unknown2 );   

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            // ====== HAVOK ========

            ImGui.Text( $"This animation has Havok index: {HavokIndex}" );

            if( ImGui.Button( $"Replace Havok data{id}" ) ) {
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

            // ===== TMB ========

            ImGui.SameLine();
            if( ImGui.Button( $"Import TMB{id}" ) ) {
                FileDialogManager.OpenFileDialog( "Select a File", ".tmb,.*", ( bool ok, string res ) => {
                    if( ok ) {
                        Tmb = TmbFile.FromLocalFile( res );
                        UiUtils.OkNotification( "TMB data imported" );
                    }
                } );
            }

            ImGui.SameLine();
            if( ImGui.Button( $"Export TMB{id}" ) ) {
                UiUtils.WriteBytesDialog( ".tmb", Tmb.ToBytes(), "tmb" );
            }

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 10 );

            ImGui.TextDisabled( "Embedded TMB:" );
            Tmb.Draw( id + "/Tmb" );
        }
    }
}
