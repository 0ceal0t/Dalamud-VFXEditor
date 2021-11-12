using Dalamud.Interface;
using Dalamud.Logging;
using ImGuiNET;

using System;
using System.IO;
using System.Numerics;

using VFXEditor.Helper;
using VFXEditor.UI;

using VFXSelect;
using VFXSelect.VFX;

namespace VFXEditor.Tmb {
    public partial class TmbManager : GenericDialog {
        private static string LocalPath => Path.Combine( Plugin.Configuration.WriteLocation, "TEMP_TMB.tmb" );
        private static TmbSelectDialog SourceSelect;
        private static TmbSelectDialog ReplaceSelect;

        public static void Setup() {
            SourceSelect = new TmbSelectDialog(
                "Tmb Select [SOURCE]",
                null,
                true,
                ( SelectResult result ) => SetSourceGlobal( result )
            );

            ReplaceSelect = new TmbSelectDialog(
                "Tmb Select [TARGET]",
                null,
                false,
                ( SelectResult result ) => SetReplaceGlobal( result )
            );
        }

        public static void SetSourceGlobal( SelectResult result ) {
            Plugin.TmbManager?.SetSource( result );
        }

        public static void SetReplaceGlobal( SelectResult result ) {
            Plugin.TmbManager?.SetReplace( result );
        }

        // =====================

        private SelectResult TmbSource = SelectResult.None();
        private SelectResult TmbReplace = SelectResult.None();

        private TmbFile CurrentFile;
        private VerifiedStatus Verified = VerifiedStatus.UNKNOWN;

        public TmbManager() : base("Tmb Tester") {
            Size = new Vector2( 600, 400 );
        }

        public void SetSource( SelectResult result ) {
            switch( result.Type ) {
                case SelectResultType.Local: // LOCAL
                    LoadLocalTmb( result.Path );
                    break;
                default: // EVERYTHING ELSE: GAME FILES
                    LoadGameTmb( result.Path );
                    break;
            }
            TmbSource = result;
            if (CurrentFile != null) {
                Verified = CurrentFile.Verified ? VerifiedStatus.OK : VerifiedStatus.ISSUE;
                Update();
            }
        }

        public void RemoveSource() {
            CurrentFile = null;
            TmbSource = SelectResult.None();
        }

        public void SetReplace( SelectResult result ) {
            TmbReplace = result;
        }

        public void RemoveReplace() {
            TmbReplace = SelectResult.None();
        }

        public bool GetReplacePath( string path, out string replacePath ) {
            replacePath = TmbReplace.Path.Equals( path ) ? LocalPath : null;
            return !string.IsNullOrEmpty( replacePath );
        }

        public void ImportLocalTmb( string localPath, SelectResult source, SelectResult replace ) {
            TmbSource = source;
            TmbReplace = replace;
            LoadLocalTmb( localPath );
        }

        private void LoadLocalTmb( string localPath ) {
            if( !File.Exists( localPath ) ) return;
            using BinaryReader br = new( File.Open( localPath, FileMode.Open ) );
            CurrentFile = new( br );
        }

        private void LoadGameTmb( string gamePath ) {
            if( !Plugin.DataManager.FileExists( gamePath ) ) return;
            var file = Plugin.DataManager.GetFile( gamePath );
            using var ms = new MemoryStream( file.Data );
            using var br = new BinaryReader( ms );
            CurrentFile = new TmbFile( br );
        }

        public void Dispose() {
            SourceSelect.Hide();
            ReplaceSelect.Hide();
        }

        public override void OnDraw() {
            SourceSelect.Draw();
            ReplaceSelect.Draw();

            ImGui.Columns( 2, "TmbManager-Columns", false );

            // ======== INPUT TEXT =========
            ImGui.SetColumnWidth( 0, 140 );
            ImGui.Text( "Loaded TMB" );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ImGui.Text( "TMB Being Replaced" );
            ImGui.NextColumn();

            // ======= SEARCH BARS =========
            var sourceString = TmbSource.DisplayString;
            var previewString = TmbReplace.DisplayString;
            ImGui.SetColumnWidth( 1, ImGui.GetWindowWidth() - 140 );
            ImGui.PushItemWidth( ImGui.GetColumnWidth() - 100 );

            ImGui.InputText( "##TmbManager-Source", ref sourceString, 255, ImGuiInputTextFlags.ReadOnly );

            ImGui.SameLine();
            ImGui.PushFont( UiBuilder.IconFont );
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 5 );
            if( ImGui.Button( $"{( char )FontAwesomeIcon.Search}", new Vector2( 30, 23 ) ) ) {
                SourceSelect.Show();
            }
            ImGui.SameLine();
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 5 );
            ImGui.PushStyleColor( ImGuiCol.Button, UiHelper.RED_COLOR );
            if( ImGui.Button( $"{( char )FontAwesomeIcon.Times}##TmbManager-SourceRemove", new Vector2( 30, 23 ) ) ) {
                RemoveSource();
            }
            ImGui.PopStyleColor();
            ImGui.PopFont();

            ImGui.InputText( "##TmbManager-Preview", ref previewString, 255, ImGuiInputTextFlags.ReadOnly );

            ImGui.SameLine();
            ImGui.PushFont( UiBuilder.IconFont );
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 5 );
            if( ImGui.Button( $"{( char )FontAwesomeIcon.Search}##TmbManager-PreviewSelect", new Vector2( 30, 23 ) ) ) {
                ReplaceSelect.Show();
            }
            ImGui.SameLine();
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 5 );
            ImGui.PushStyleColor( ImGuiCol.Button, UiHelper.RED_COLOR );
            if( ImGui.Button( $"{( char )FontAwesomeIcon.Times}##TmbManager-PreviewRemove", new Vector2( 30, 23 ) ) ) {
                RemoveReplace();
            }
            ImGui.PopStyleColor();
            ImGui.PopFont();

            ImGui.PopItemWidth();
            ImGui.Columns( 1 );

            // ===============

            ImGui.PushStyleColor( ImGuiCol.Text, new Vector4( 0.9f, 0.1f, 0.1f, 1.0f ) );
            ImGui.TextWrapped( "DO NOT modify movement abilities (dashes, backflips, etc.)" );
            ImGui.PopStyleColor();

            ImGui.TextWrapped( "Also note that changing animation paths to that of a different job may not work without swapping the .pap files as well" );

            ImGui.Separator();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( CurrentFile != null ) {
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
            Plugin.ResourceLoader.ReloadPath( TmbReplace.Path, LocalPath );
        }
    }
}
