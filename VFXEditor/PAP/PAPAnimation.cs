using ImGuiFileDialog;
using ImGuiNET;
using System.IO;
using VFXEditor.Helper;
using VFXEditor.Interop;
using VFXEditor.TMB;

namespace VFXEditor.PAP {
    public class PAPAnimation {
        public short HavokIndex = 0;

        private readonly string HkxTempLocation;
        private string Name = "cbbm_replace_this"; // padded to 32 bytes (INCLUDING NULL)
        private short Unknown1 = 0;
        private int Unknown2 = 0;
        private TMBFile Tmb;

        public PAPAnimation( string hkxPath ) {
            HkxTempLocation = hkxPath;
        }

        public PAPAnimation( BinaryReader reader, string hkxPath ) {
            HkxTempLocation = hkxPath;
            Name = FileHelper.ReadString( reader );
            reader.ReadBytes( 32 - Name.Length - 1 );
            Unknown1 = reader.ReadInt16();
            HavokIndex = reader.ReadInt16();
            Unknown2 = reader.ReadInt32();
        }

        public void Write( BinaryWriter writer ) {
            FileHelper.WriteString( writer, Name, true );
            for( var i = 0; i < ( 32 - Name.Length - 1 ); i++ ) {
                writer.Write( ( byte )0 );
            }
            writer.Write( Unknown1 );
            writer.Write( HavokIndex );
            writer.Write( Unknown2 );
        }

        public void ReadTmb( BinaryReader reader ) {
            Tmb = new TMBFile( reader, false );
        }

        public void ReadTmb( string path ) {
            Tmb = TMBFile.FromLocalFile( path );
        }

        public byte[] GetTmbBytes() => Tmb.ToBytes();

        public string GetName() => Name;

        public void Draw( string id ) {
            ImGui.InputText( $"Name{id}", ref Name, 31 );
            FileHelper.ShortInput( $"Unknown 1{id}", ref Unknown1 );
            ImGui.InputInt( $"Unknown 2{id}", ref Unknown2 );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            // Havok  ================

            ImGui.Text( $"This animation has Havok index: {HavokIndex}" );

            if( ImGui.Button( $"Replace Havok data{id}" ) ) {
                FileDialogManager.OpenFileDialog( "Select a File", ".hkx,.*", ( bool ok, string res ) => {
                    if( ok ) {
                        PAPManager.IndexDialog.OnOk = ( int idx ) => {
                            HavokInterop.ReplaceHavokAnimation( HkxTempLocation, HavokIndex, res, idx, HkxTempLocation );
                            UIHelper.OkNotification( "Havok data replaced" );
                        };
                        PAPManager.IndexDialog.Show();
                    }
                } );
            }

            // Tmb ==============

            // Export Tmb

            if( ImGui.Button( $"Export TMB{id}" ) ) {
                UIHelper.WriteBytesDialog( ".tmb", Tmb.ToBytes(), "tmb" );
            }

            // Import Tmb

            ImGui.SameLine();
            if( ImGui.Button( $"Import TMB{id}" ) ) {
                FileDialogManager.OpenFileDialog( "Select a File", ".tmb,.*", ( bool ok, string res ) => {
                    if( ok ) {
                        Tmb = TMBFile.FromLocalFile( res );
                        UIHelper.OkNotification( "TMB data imported" );
                    }
                } );
            }

            // Draw Tmb

            if( ImGui.BeginTabBar( $"Tabs{id}", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton ) ) {
                if( ImGui.BeginTabItem( $"Tmb{id}/Tab" ) ) {
                    Tmb.Draw( id + "/Tmb" );
                    ImGui.EndTabItem();
                }
                ImGui.EndTabBar();
            }
        }
    }
}
