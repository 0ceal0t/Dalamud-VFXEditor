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

        public ReplaceDoc SelectedDoc = null;
        public override void OnDraw() {
            var id = "##Doc";
            float footerHeight = ImGui.GetStyle().ItemSpacing.Y + ImGui.GetFrameHeightWithSpacing();

            ImGui.BeginChild( id + "/Child", new Vector2( 0, -footerHeight ), true );

            ImGui.Columns( 2, id + "/Columns", false );

            int idx = 0;
            foreach( var doc in _plugin.Doc.Docs ) {
                if(ImGui.Selectable(doc.Source.DisplayString + id + idx, doc == SelectedDoc, ImGuiSelectableFlags.SpanAllColumns ) ) {
                    SelectedDoc = doc;
                }
                if( ImGui.IsItemHovered() ) {
                    ImGui.BeginTooltip();
                    ImGui.Text( "Replace path: " + doc.Replace.Path );
                    ImGui.Text( "Write path: " + doc.WriteLocation );
                    ImGui.EndTooltip();
                }
                idx++;
            }
            ImGui.NextColumn();

            foreach( var doc in _plugin.Doc.Docs ) {
                ImGui.Text( doc.Replace.DisplayString );
            }

            ImGui.Columns(1);
            ImGui.EndChild();

            if( ImGui.Button( "+ NEW" + id ) ) {
                _plugin.Doc.NewDoc();
                _plugin.RefreshDoc();
            }

            if(SelectedDoc != null ) {
                bool deleteDisabled = ( _plugin.Doc.Docs.Count == 1 );

                ImGui.SameLine( ImGui.GetWindowWidth() - 105 );
                if(ImGui.Button("Select" + id ) ) {
                    _plugin.Doc.SelectDoc( SelectedDoc );
                    _plugin.RefreshDoc();
                }
                if( !deleteDisabled ) {
                    ImGui.SameLine( ImGui.GetWindowWidth() - 55 );
                    if( UIUtils.RemoveButton( "Delete" + id ) ) {
                        if( _plugin.Doc.RemoveDoc( SelectedDoc ) ) {
                            _plugin.RefreshDoc();
                        }
                        SelectedDoc = _plugin.Doc.ActiveDoc;
                    }
                }
            }
        }
    }
}
