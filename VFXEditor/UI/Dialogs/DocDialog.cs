using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Dalamud.Interface;
using Dalamud.Plugin;
using ImGuiNET;
using VFXEditor.Data;
using VFXEditor.UI.VFX;

namespace VFXEditor.UI {
    public class DocDialog : GenericDialog {
        public DocDialog(Plugin plugin) : base(plugin, "Documents" ) { }

        public ReplaceDoc SelectedDoc = null;
        public override void OnDraw() {
            var id = "##Doc";
            float footerHeight = ImGui.GetStyle().ItemSpacing.Y + ImGui.GetFrameHeightWithSpacing();

            ImGui.BeginChild( id + "/Child", new Vector2( 0, -footerHeight ), true );

            ImGui.Columns( 2, id + "/Columns", false );

            int idx = 0;
            foreach( var doc in Plugin.DocManager.Docs ) {
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

            foreach( var doc in Plugin.DocManager.Docs ) {
                ImGui.Text( doc.Replace.DisplayString );
            }

            ImGui.Columns(1);
            ImGui.EndChild();

            if( ImGui.Button( "+ NEW" + id ) ) {
                Plugin.DocManager.NewDoc();
            }

            if(SelectedDoc != null ) {
                bool deleteDisabled = ( Plugin.DocManager.Docs.Count == 1 );

                ImGui.SameLine( ImGui.GetWindowWidth() - 105 );
                if(ImGui.Button("Select" + id ) ) {
                    Plugin.DocManager.SelectDoc( SelectedDoc );
                }
                if( !deleteDisabled ) {
                    ImGui.SameLine( ImGui.GetWindowWidth() - 55 );
                    if( UIUtils.RemoveButton( "Delete" + id ) ) {
                        Plugin.DocManager.RemoveDoc( SelectedDoc );
                        SelectedDoc = Plugin.DocManager.ActiveDoc;
                    }
                }
            }
        }
    }
}
