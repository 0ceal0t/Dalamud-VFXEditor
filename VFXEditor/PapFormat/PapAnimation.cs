using ImGuiFileDialog;
using ImGuiNET;
using OtterGui.Raii;
using System.IO;
using VfxEditor.Interop;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat;
using VfxEditor.Utils;

namespace VfxEditor.PapFormat {
    public class PapAnimation {
        public short HavokIndex = 0;

        private readonly string HkxTempLocation;
        private readonly ParsedString Name = new( "Name", "cbbm_replace_this", 31 );
        private readonly ParsedShort Unk1 = new( "Unknown 1" );
        private readonly ParsedInt Unk2 = new( "Unknown 2" );
        public TmbFile Tmb;

        public PapAnimation( string hkxPath ) {
            HkxTempLocation = hkxPath;
        }

        public PapAnimation( BinaryReader reader, string hkxPath ) {
            HkxTempLocation = hkxPath;
            Name.Read( reader );
            Name.Pad( reader, 32 );
            Unk1.Read( reader );
            HavokIndex = reader.ReadInt16();
            Unk2.Read( reader );
        }

        public void Write( BinaryWriter writer ) {
            Name.Write( writer );
            Name.Pad( writer, 32 );
            Unk1.Write( writer );
            writer.Write( HavokIndex );
            Unk2.Write( writer );
        }

        public void ReadTmb( BinaryReader reader, CommandManager manager ) {
            Tmb = new TmbFile( reader, manager, true );
        }

        public void ReadTmb( string path, CommandManager manager ) {
            Tmb = TmbFile.FromPapEmbedded( path, manager );
        }

        public byte[] GetTmbBytes() => Tmb.ToBytes();

        public string GetName() => Name.Value;

        public void Draw( int modelId, SkeletonType modelType ) {
            Name.Draw( CommandManager.Pap );
            Unk1.Draw( CommandManager.Pap );
            Unk2.Draw( CommandManager.Pap );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            ImGui.Text( $"This animation has Havok index: {HavokIndex}" );

            if( ImGui.Button( $"Replace Havok data" ) ) {
                FileDialogManager.OpenFileDialog( "Select a File", ".hkx,.*", ( bool ok, string res ) => {
                    if( ok ) {
                        Plugin.PapManager.IndexDialog.OnOk = ( int idx ) => {
                            CommandManager.Pap.Add( new PapHavokFileCommand( HkxTempLocation, () => {
                                HavokInterop.ReplaceHavokAnimation( HkxTempLocation, HavokIndex, res, idx, HkxTempLocation );
                            } ) );
                            UiUtils.OkNotification( "Havok data replaced" );
                        };
                        Plugin.PapManager.IndexDialog.Show();
                    }
                } );
            }

            using var tabBar = ImRaii.TabBar( "AnimationTabs" );
            if( !tabBar ) return;

            DrawTmb();
            DrawAnimation3D( modelId, modelType );
        }

        private void DrawTmb() {
            using var tabItem = ImRaii.TabItem( "TMB" );
            if( !tabItem ) return;

            if( ImGui.Button( "Replace TMB" ) ) {
                FileDialogManager.OpenFileDialog( "Select a File", ".tmb,.*", ( bool ok, string res ) => {
                    if( ok ) {
                        CommandManager.Pap.Add( new PapReplaceTmbCommand( this, TmbFile.FromPapEmbedded( res, CommandManager.Pap ) ) );
                        UiUtils.OkNotification( "TMB data imported" );
                    }
                } );
            }

            ImGui.SameLine();
            if( ImGui.Button( "Export TMB" ) ) UiUtils.WriteBytesDialog( ".tmb", Tmb.ToBytes(), "tmb" );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            using var _ = ImRaii.PushId( "Tmb" );
            Tmb.Draw();
        }

        private void DrawAnimation3D( int modelId, SkeletonType modelType ) {
            using var tabItem = ImRaii.TabItem( "3D View" );
            if( !tabItem ) return;

            Plugin.AnimationManager.Draw( this, HkxTempLocation, HavokIndex, modelId, modelType );
        }
    }
}
