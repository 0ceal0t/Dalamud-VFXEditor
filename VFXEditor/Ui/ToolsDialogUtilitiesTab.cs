using Dalamud.Logging;
using ImGuiFileDialog;
using ImGuiNET;
using System;
using VfxEditor.Utils;
using VfxEditor.TextureFormat;
using System.Numerics;

namespace VfxEditor.Ui {
    public class ToolsDialogUtilitiesTab {
        private string ExtractPath = "";

        public void Draw() {
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );
            ImGui.TextDisabled( "Extract Raw Game File" );
            ImGui.Indent();

            ImGui.PushStyleVar( ImGuiStyleVar.ItemSpacing, new Vector2( 4, 3 ) );
            ImGui.InputTextWithHint( "##Extract", "Game Path", ref ExtractPath, 255 );

            ImGui.SameLine();
            if( ImGui.Button( "Extract" ) ) {
                var cleanedPath = ExtractPath.Replace( "\\", "/" );
                if( Plugin.DataManager.FileExists( cleanedPath ) ) {
                    try {
                        var ext = cleanedPath.Contains( '.' ) ? cleanedPath.Split( "." )[1] : "bin";
                        var file = Plugin.DataManager.GetFile( cleanedPath );
                        UiUtils.WriteBytesDialog( $".{ext}", file.Data, ext );
                    }
                    catch( Exception e ) {
                        PluginLog.Error( "Could not read file", e );
                    }
                }
            }

            ImGui.PopStyleVar( 1 );

            ImGui.Unindent();

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ImGui.TextDisabled( "Image Conversion" );
            ImGui.Indent();

            if( ImGui.Button( ".atex to PNG" ) ) {
                FileDialogManager.OpenFileDialog( "Select a File", ".atex,.*", ( bool ok, string res ) => {
                    if( !ok ) return;
                    var texFile = TextureFile.LoadFromLocal( res );
                    texFile.SaveAsPng( res + ".png" );
                } );
            }

            ImGui.SameLine();
            if( ImGui.Button( ".atex to DDS" ) ) {
                FileDialogManager.OpenFileDialog( "Select a File", ".atex,.*", ( bool ok, string res ) => {
                    if( !ok ) return;
                    var texFile = TextureFile.LoadFromLocal( res );
                    texFile.SaveAsDds( res + ".dds" );
                } );
            }

            ImGui.Unindent();
        }

        /*
         * Skill Swapping
         * 
         * Get TMB and PAP for both
         *  - split into end/start/hit
         *  - skip basic destinations (normal_hit, etc)
         *  
         *  get unique animation ids (intersection of animation ids in pap and animation ids used in tmb)
         *  
         *  in TMB, replace C010 with new placeholders (only replace the ones from modified .pap files)
         *  
         *  race edge case
         *  - source exists / dest doesn't -> skip
         *  - dest exists / source doesn't -> use closest available (midlander m?)
         *  
         *  action.pap
         *  - both in action.pap -> do nothing (don't even need unique animation ids)
         *  - neither in action.pap -> normal
         *  - source in action.pap, dest not
         *      - don't need to do anything, can just use them
         *  - source not in action.pap dest is [RISK OF MULTIPLE MODIFYING ACTION.PAP]
         *      - just add them to the end
         *      - what about TMFC?
         */
    }
}
