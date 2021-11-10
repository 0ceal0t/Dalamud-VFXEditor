using Dalamud.Interface;
using Dalamud.Logging;
using ImGuiNET;
using System;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using VFXEditor.UI;

namespace VFXEditor.Tmb {

    public class TmbManager : GenericDialog {
        private static string LocalPath => Path.Combine(Plugin.Configuration.WriteLocation, "TEMP_TMB.tmb" );
        private string TmbPath = "";
        private TmbFile CurrentFile;
        private VerifiedStatus Verified = VerifiedStatus.UNKNOWN;

        public TmbManager() : base("Tmb Tester") {
            Size = new Vector2( 600, 400 );
        }

        public void Dispose() {
        }

        public bool GetReplacePath( string path, out string replacePath ) {
            replacePath = null;
            if( TmbPath.Equals( path ) ) {
                replacePath = LocalPath;
                return true;
            }
            return false;
        }

        public override void OnDraw() {
            ImGui.InputText( "Path##Tmb", ref TmbPath, 255 );
            ImGui.SameLine();
            if( ImGui.Button( "Load##Tmb" ) ) {
                var result = Plugin.DataManager.FileExists( TmbPath );
                if( result ) {
                    var file = Plugin.DataManager.GetFile( TmbPath );
                    using var ms = new MemoryStream( file.Data );
                    using var br = new BinaryReader( ms );

                    CurrentFile = new TmbFile( br );
                    Update();

                    var output = CurrentFile.ToBytes();
                    Verified = CurrentFile.EntriesOk ? VerifiedStatus.OK : VerifiedStatus.ISSUE;
                    for( var i = 0; i < Math.Min( output.Length, file.Data.Length ); i++ ) {
                        if( output[i] != file.Data[i] ) {
                            PluginLog.Log( $"FILES DO NOT MATCH: {i}" );
                            Verified = VerifiedStatus.ISSUE;
                        }
                    }
                }
            }

            if( CurrentFile != null ) {
                ImGui.Separator();
                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

                if( UIUtils.OkButton( "UPDATE" ) ) Update();

                ImGui.SameLine();
                if( ImGui.Button( "Reload" ) ) Reload();
                ImGui.SameLine();
                UIUtils.HelpMarker( "Manually reload the resource" );

                ImGui.SameLine();
                ImGui.PushFont( UiBuilder.IconFont );
                if( ImGui.Button( $"{( char )FontAwesomeIcon.FileDownload}" ) ) {
                    ImGui.PopFont();
                    Plugin.WriteBytesDialog( ".tmb", CurrentFile.ToBytes(), "tmb" );
                }
                else ImGui.PopFont();

                ImGui.SameLine();
                UIUtils.ShowVerifiedStatus( Verified );

                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );
                CurrentFile.Draw( "##Tmb" );
            }
        }

        private void Update() {
            if( CurrentFile == null ) return;
            File.WriteAllBytes( LocalPath, CurrentFile.ToBytes() );
        }

        private void Reload() {
            if( CurrentFile == null ) return;
            Plugin.ResourceLoader.ReloadPath( TmbPath, false );
        }
    }
}
