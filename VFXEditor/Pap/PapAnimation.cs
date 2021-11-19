using ImGuiFileDialog;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFXEditor.Helper;
using VFXEditor.Interop;
using VFXEditor.Tmb;

namespace VFXEditor.Pap {
    public class PapAnimation {
        private readonly string HkxTempLocation;

        private string Name = ""; // padded to 32 bytes (INCLUDING NULL)
        private short Unknown1 = 0;
        private int Unknown2 = 0;
        private readonly short HavokIndex;

        private TmbFile Tmb;

        private int ImportedHavokIndex = 0;

        public PapAnimation( BinaryReader reader, string hkxPath ) {
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
            Tmb = new TmbFile( reader, false );
        }

        public byte[] GetTmbBytes() => Tmb.ToBytes();

        public string GetName() => Name;

        public void Draw( string id ) {
            ImGui.InputText( $"Name{id}", ref Name, 31 );
            FileHelper.ShortInput( $"Unknown 1{id}", ref Unknown1 );
            ImGui.InputInt( $"Unknown 2{id}", ref Unknown2 );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            // =====================
            if( ImGui.BeginTabBar( $"Tabs{id}", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton ) ) {
                if( ImGui.BeginTabItem( $"Havok{id}/Tab" ) ) {
                    DrawHavok( id );
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( $"Tmb{id}/Tab" ) ) {
                    DrawTmb( id );
                    ImGui.EndTabItem();
                }
                ImGui.EndTabBar();
            }
        }

        private void DrawHavok( string id ) {
            if( ImGui.Button( $"Export Havok{id}" ) ) {
                FileDialogManager.SaveFileDialog( "Select a Save Location", ".hkx", "", "hkx", ( bool ok, string res ) => {
                    if( ok ) File.Copy( HkxTempLocation, res, true );
                } );
            }
            ImGui.SameLine();
            if( ImGui.Button( $"Import Havok{id}" ) ) {
                FileDialogManager.OpenFileDialog( "Select a File", ".hkx,.*", ( bool ok, string res ) => {
                    if( ok ) HavokInterop.ReplaceHavokAnimation( HkxTempLocation, HavokIndex, res, ImportedHavokIndex, HkxTempLocation );
                } );
            }
            ImGui.SameLine();
            ImGui.SetNextItemWidth( 100f );
            ImGui.InputInt( $"Imported Havok Index{id}", ref ImportedHavokIndex );

            ImGui.Text( $"Havok Index: {HavokIndex}" );
        }

        private void DrawTmb( string id ) {
            Tmb.Draw( id + "/Tmb" );
        }
    }
}
