using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using System;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Data.Command.ListCommands;
using VfxEditor.FileBrowser;
using VfxEditor.PhybFormat;
using VfxEditor.PhybFormat.Simulator;
using VfxEditor.Ui.Components;

namespace VfxEditor.Formats.PhybFormat.Simulator {
    public class PhybSimulatorDropdown : CommandDropdown<PhybSimulator> {
        private readonly PhybSimulation Simulation;
        private readonly PhybFile File;

        public PhybSimulatorDropdown( PhybFile file, PhybSimulation simulation, List<PhybSimulator> simulators ) :
            base( "Simulator", simulators, null, () => new PhybSimulator( file ) ) {

            File = file;
            Simulation = simulation;
        }

        protected override void DrawControls() {
            using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing );

            DrawNewControl( OnNew );

            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                ImGui.SameLine();
                if( ImGui.Button( FontAwesomeIcon.Upload.ToIconString() ) ) {
                    FileBrowserManager.OpenFileDialog( "Select a File", "PHYB simulator{.phybsim},.*", ( ok, res ) => {
                        if( !ok ) return;
                        try {
                            using var ms = new MemoryStream( System.IO.File.ReadAllBytes( res ) );
                            using var reader = new BinaryReader( ms );
                            reader.ReadInt32(); // 1
                            CommandManager.Add( new ListAddCommand<PhybSimulator>( Items, new PhybSimulator( File, reader, 0 ), OnChangeAction ) );
                        }
                        catch( Exception e ) {
                            Dalamud.Error( e, "Could not import data" );
                        }
                    } );
                }

                ImGui.SameLine();
                using var disabled = ImRaii.Disabled( Selected == null );
                if( ImGui.Button( FontAwesomeIcon.Save.ToIconString() ) ) {
                    FileBrowserManager.SaveFileDialog( "Select a Save Location", ".phybsim,.*", "ExportedSimulator", "phybsim", ( ok, res ) => {
                        if( ok ) System.IO.File.WriteAllBytes( res, Selected.ToBytes() );
                    } );
                }
            }

            DrawDeleteControl( OnDelete );
        }
    }
}
