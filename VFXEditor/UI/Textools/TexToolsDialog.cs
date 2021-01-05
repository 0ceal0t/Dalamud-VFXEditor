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

    public class TexToolsDialog
    {
        public Plugin _plugin;
        public bool Visible = false;

        public TexToolsDialog( Plugin plugin )
        {
            _plugin = plugin;
        }

        public void Show(
        )
        {
            Visible = true;
        }

        public string Name = "";
        public string Author = "";
        public string SaveLocation = "";
        public string VFXPath = "";

        public bool DrawOnce = false;
        public void Draw()
        {
            if( !Visible )
                return;
            if( !DrawOnce )
            {
                ImGui.SetNextWindowSize( new Vector2( 500, 300 ) );
                DrawOnce = true;
            }
            // ================
            var ret = ImGui.Begin( "TexTools", ref Visible );
            if( !ret )
                return;

            var id = "##Textools";

            ImGui.InputText( "Mod Name" + id, ref Name, 255 );
            ImGui.InputText( "Mod Author" + id, ref Author, 255 );
            ImGui.InputText( "Save Location" + id, ref SaveLocation, 255 );
            ImGui.SameLine();
            if( ImGui.Button( "BROWSE" + id ) )
            {
                SaveDialog();
            }
            ImGui.InputText( "VFX Path" + id, ref VFXPath, 255 );
            ImGui.SameLine();
            if(ImGui.Button("Use Replace" + id ) )
            {
                VFXPath = _plugin.ReplaceAVFXPath;
            }
            ImGui.Separator();
            if(ImGui.Button("EXPORT" + id ) )
            {
                _plugin.TexToolsManager.Export( Name, Author, VFXPath, SaveLocation, _plugin.AVFX );
                Visible = false;
            }

            ImGui.End();
        }

        public void SaveDialog()
        {
            Task.Run( async () =>
            {
                var picker = new SaveFileDialog
                {
                    Filter = "TexTools Mod (*.ttmp2)|*.ttmp2*|All files (*.*)|*.*",
                    Title = "Select a Save Location.",
                    DefaultExt = "ttmp2",
                    AddExtension = true
                };
                var result = await picker.ShowDialogAsync();
                if( result == DialogResult.OK )
                {
                    try
                    {
                        SaveLocation = picker.FileName;
                    }
                    catch( Exception ex )
                    {
                        PluginLog.LogError( ex, "Could not select a mod location");
                    }
                }
            } );
        }
    }
}
