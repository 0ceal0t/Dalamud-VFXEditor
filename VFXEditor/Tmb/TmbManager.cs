using Dalamud.Interface;
using Dalamud.Logging;
using ImGuiNET;
using System;
using System.IO;
using System.Numerics;

using VFXEditor.Helper;
using VFXEditor.UI;

namespace VFXEditor.Tmb {

    public partial class TmbManager : GenericDialog {
        public string TmbSourcePath = "";
        public string TmbReplacePath = "";
        public TmbFile CurrentFile;

        private static string LocalPath => Path.Combine(Plugin.Configuration.WriteLocation, "TEMP_TMB.tmb" );
        private VerifiedStatus Verified = VerifiedStatus.UNKNOWN;

        public TmbManager() : base("Tmb Tester") {
            Size = new Vector2( 600, 400 );
        }

        public void Dispose() {
        }

        public bool GetReplacePath( string path, out string replacePath ) {
            replacePath = TmbReplacePath.Equals( path ) ? LocalPath : null;
            return !string.IsNullOrEmpty( replacePath );
        }

        public override void OnDraw() {
            ImGui.InputText( "Loaded TMB##Tmb", ref TmbSourcePath, 255 );
            ImGui.SameLine();
            if( ImGui.Button( "Load##Tmb" ) ) {
                var result = !string.IsNullOrEmpty(TmbSourcePath) && Plugin.DataManager.FileExists( TmbSourcePath );
                if( result ) {
                    var file = Plugin.DataManager.GetFile( TmbSourcePath );
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

            ImGui.InputText( "TMB Being Replaced##Tmb", ref TmbReplacePath, 255 );

            if( CurrentFile != null ) {
                ImGui.Separator();
                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

                if( UiHelper.OkButton( "UPDATE" ) ) Update();

                ImGui.SameLine();
                if( ImGui.Button( "Reload" ) ) Reload();
                ImGui.SameLine();
                UiHelper.HelpMarker( "Manually reload the resource" );

                ImGui.SameLine();
                ImGui.PushFont( UiBuilder.IconFont );
                if( ImGui.Button( $"{( char )FontAwesomeIcon.FileDownload}" ) ) {
                    ImGui.PopFont();
                    Plugin.WriteBytesDialog( ".tmb", CurrentFile.ToBytes(), "tmb" );
                }
                else ImGui.PopFont();

                ImGui.SameLine();
                UiHelper.ShowVerifiedStatus( Verified );

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
            Plugin.ResourceLoader.ReloadPath( TmbReplacePath, false );
        }
    }
}
