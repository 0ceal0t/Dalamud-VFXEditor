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
using VFXEditor.UI.VFX;

namespace VFXEditor.UI {
    public class DocDialog : GenericDialog {
        public DocDialog(Plugin plugin) : base(plugin, "Documents" ) {
            Size = new Vector2( 500, 400 );
        }

        public override void OnDraw() {
            var id = "##Doc";
            float footerHeight = ImGui.GetStyle().ItemSpacing.Y + ImGui.GetFrameHeightWithSpacing();

            ImGui.BeginChild( id + "/Child", new Vector2( 0, -footerHeight ), false );

            ImGui.Columns( 3, id + "/Columns", false );

            foreach( var doc in _plugin.Doc.Docs ) {
                ImGui.Text( doc.Source.DisplayString );
            }
            ImGui.NextColumn();

            foreach( var doc in _plugin.Doc.Docs ) {
                ImGui.Text( doc.Replace.DisplayString );
            }
            ImGui.NextColumn();

            var idx = 0;
            foreach(var doc in _plugin.Doc.Docs ) {
                if( ImGui.SmallButton("Select" + id + idx) ) {
                    _plugin.Doc.SelectDoc( doc );
                    _plugin.RefreshDoc();
                }
                ImGui.SameLine();
                if( _plugin.Doc.Docs.Count > 1 ) {
                    if( UIUtils.RemoveButton( "Delete" + id + idx, small:true ) ) {
                        if(_plugin.Doc.RemoveDoc( doc ) ) {
                            _plugin.RefreshDoc();
                        }
                        return;
                    }
                }
                idx++;
            }

            ImGui.Columns(1);

            ImGui.EndChild();
            ImGui.Separator();
            if( ImGui.Button( "+ NEW" + id ) ) {
                _plugin.Doc.NewDoc();
                _plugin.RefreshDoc();
            }
        }
    }
}
