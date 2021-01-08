using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dalamud.Interface;
using Dalamud.Plugin;
using ImGuiNET;

namespace VFXEditor.UI
{

    public class PenumbraDialog
    {
        public Plugin _plugin;
        public bool Visible = false;

        public PenumbraDialog( Plugin plugin ) {
            _plugin = plugin;
        }
        public void Show() {
            Visible = true;
        }

        public string Name = "";
        public string Author = "";
        public string SaveLocation = "";
        public string VFXPath = "";

        public bool DrawOnce = false;
        public void Draw() {
            if( !Visible )
                return;
            if( !DrawOnce )
            {
                ImGui.SetNextWindowSize( new Vector2( 500, 190 ) );
                DrawOnce = true;
            }
            // ================
            var ret = ImGui.Begin( "Penumbra", ref Visible );
            if( !ret )
                return;

            var id = "##Penumbra";
            float footerHeight = ImGui.GetStyle().ItemSpacing.Y + ImGui.GetFrameHeightWithSpacing();

            ImGui.BeginChild( id + "/Child", new Vector2( 0, -footerHeight ), false );
            ImGui.InputText( "Mod Name" + id, ref Name, 255 );
            ImGui.InputText( "Mod Author" + id, ref Author, 255 );
            ImGui.InputText( "Save Location" + id, ref SaveLocation, 255 );
            //ImGui.SameLine();
            //if( ImGui.Button( "BROWSE" + id ) )
            //{
            //    SaveDialog();
            //}
            ImGui.InputText( "VFX Path" + id, ref VFXPath, 255 );
            ImGui.SameLine();
            if( ImGui.Button( "Use Replace" + id ) )
            {
                VFXPath = _plugin.ReplaceAVFXPath;
            }
            ImGui.EndChild();
            ImGui.Separator();
            if( ImGui.Button( "EXPORT" + id ) )
            {
                _plugin.PenumbraManager.Export( Name, Author, VFXPath, SaveLocation, _plugin.AVFX );
                Visible = false;
            }
            ImGui.End();
        }

        public void SaveDialog() // TODO: this is broken
        {
            Task.Run( async () => {
                var picker = new FolderBrowserDialog
                {
                    Description = "Select folder output"
                };
                var result = await picker.ShowDialogAsync();
                if( result == DialogResult.OK ) {
                    try {
                        SaveLocation = picker.SelectedPath;
                    }
                    catch( Exception ex ) {
                        PluginLog.LogError( ex, "Could not select a mod location" );
                    }
                }
            } );
        }
    }
}
