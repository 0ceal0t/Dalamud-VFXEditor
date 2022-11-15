using Dalamud.Logging;
using ImGuiFileDialog;
using ImGuiNET;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using VfxEditor.AvfxFormat2.Model;
using VfxEditor.Utils;
using static VfxEditor.DirectX.ModelPreview;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxModel : AvfxNode {
        public const string NAME = "Modl";

        public readonly AvfxVertexes Vertexes = new();
        public readonly AvfxIndexes Indexes = new();
        public readonly AvfxEmitVertexes EmitVertexes = new();
        public readonly AvfxEmitVertexNumbers EmitVertexNumbers = new();

        public readonly List<UiEmitVertex> CombinedEmitVertexes = new();

        private readonly List<AvfxBase> Parsed;

        public readonly UiModelEmitSplitView EmitSplitDisplay;
        public readonly UiNodeGraphView NodeView;

        private int Mode = ( int )RenderMode.Color;
        private bool Refresh = false;
        private readonly UiModelUvView UvView;

        public AvfxModel() : base( NAME, UiNodeGroup.ModelColor, false ) {
            Parsed = new() {
                EmitVertexNumbers,
                EmitVertexes,
                Vertexes,
                Indexes
            };

            NodeView = new( this );
            UvView = new UiModelUvView();
            EmitSplitDisplay = new( CombinedEmitVertexes );
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            ReadNested( reader, Parsed, size );
            if( EmitVertexes.EmitVertexes.Count != EmitVertexNumbers.VertexNumbers.Count ) {
                PluginLog.Error( $"Mismatched emit vertex counts {EmitVertexes.EmitVertexes.Count} {EmitVertexNumbers.VertexNumbers.Count}" );
            }
            for( var i = 0; i < Math.Min( EmitVertexes.EmitVertexes.Count, EmitVertexNumbers.VertexNumbers.Count ); i++ ) {
                CombinedEmitVertexes.Add( new UiEmitVertex( EmitVertexes.EmitVertexes[i], EmitVertexNumbers.VertexNumbers[i] ) );
            }
            EmitSplitDisplay.UpdateIdx();

            DepedencyImportInProgress = false;
        }

        protected override void RecurseChildrenAssigned( bool assigned ) => RecurseAssigned( Parsed, assigned );

        protected override void WriteContents( BinaryWriter writer ) {
            EmitVertexes.EmitVertexes.Clear();
            EmitVertexes.EmitVertexes.AddRange( CombinedEmitVertexes.Select( x => x.Vertex ) );
            EmitVertexNumbers.VertexNumbers.Clear();
            EmitVertexNumbers.VertexNumbers.AddRange( CombinedEmitVertexes.Select( x => x.Number ) );

            if( EmitVertexNumbers.VertexNumbers.Count > 0 ) EmitVertexNumbers.Write( writer );
            if( EmitVertexes.EmitVertexes.Count > 0 ) EmitVertexes.Write( writer );
            if( Vertexes.Vertexes.Count > 0 ) Vertexes.Write( writer );
            if( Indexes.Indexes.Count > 0 ) Indexes.Write( writer );
        }

        public override void Draw( string parentId ) {
            var id = parentId + "/Model";
            NodeView.Draw( id );
            DrawRename( id );
            ImGui.Text( "Vertices: " +Vertexes.Vertexes.Count + " " + "Indexes: " + Indexes.Indexes.Count );
            if( ImGui.Button( "Export" + id ) ) {
                ImGui.OpenPopup( "Save_Popup" + id );
            }

            if( ImGui.BeginPopup( "Save_Popup" + id ) ) {
                if( ImGui.Selectable( "GLTF" + id ) ) {
                    ExportDialog();
                }
                if( ImGui.Selectable( "AVFX" + id ) ) {
                    Plugin.AvfxManager.ShowExportDialog( this );
                }
                ImGui.EndPopup();
            }

            ImGui.SameLine();

            if( ImGui.Button( "Replace" + id ) ) ImportDialog();

            ImGui.Text( "Notes on exporting GLTF models:" );
            ImGui.SameLine();
            if( ImGui.SmallButton( "Here" ) ) UiUtils.OpenUrl( "https://github.com/0ceal0t/Dalamud-VFXEditor/wiki/Replacing-textures-and-models#models" );

            ImGui.BeginTabBar( "ModelTabs" );
            DrawModel3D( id );
            DrawUvView( id );
            DrawEmitterVerts( id );
            ImGui.EndTabBar();
        }

        public void OnSelect() {
            Plugin.DirectXManager.ModelPreview.LoadModel( this, RenderMode.Color );
            UvView.LoadModel( this );
        }

        private void DrawModel3D( string parentId ) {
            if( !ImGui.BeginTabItem( "3D View" + parentId ) ) return;
            if( Refresh ) {
                Plugin.DirectXManager.ModelPreview.LoadModel( this, ( RenderMode )Mode );
                UvView.LoadModel( this );
                Refresh = false;
            }

            var wireframe = Plugin.DirectXManager.ModelPreview.IsWireframe;
            if( ImGui.Checkbox( "Wireframe##3DModel", ref wireframe ) ) {
                Plugin.DirectXManager.ModelPreview.IsWireframe = wireframe;
                Plugin.DirectXManager.ModelPreview.RefreshRasterizeState();
                Plugin.DirectXManager.ModelPreview.Draw();
            }
            ImGui.SameLine();
            if( ImGui.Checkbox( "Show Edges##3DModel", ref Plugin.DirectXManager.ModelPreview.ShowEdges ) ) {
                Plugin.DirectXManager.ModelPreview.Draw();
            }
            ImGui.SameLine();
            if( ImGui.Checkbox( "Show Emitter Vertices##3DModel", ref Plugin.DirectXManager.ModelPreview.ShowEmitter ) ) {
                Plugin.DirectXManager.ModelPreview.Draw();
            }
            if( ImGui.RadioButton( "Color", ref Mode, ( int )RenderMode.Color ) ) {
                Plugin.DirectXManager.ModelPreview.LoadModel( this, RenderMode.Color );
            }
            ImGui.SameLine();
            if( ImGui.RadioButton( "UV 1", ref Mode, ( int )RenderMode.Uv1 ) ) {
                Plugin.DirectXManager.ModelPreview.LoadModel( this, RenderMode.Uv1 );
            }
            ImGui.SameLine();
            if( ImGui.RadioButton( "UV 2", ref Mode, ( int )RenderMode.Uv2 ) ) {
                Plugin.DirectXManager.ModelPreview.LoadModel( this, RenderMode.Uv2 );
            }

            Plugin.DirectXManager.ModelPreview.DrawInline();

            ImGui.EndTabItem();
        }

        private void DrawUvView( string parentId ) {
            if( !ImGui.BeginTabItem( "UV View" + parentId ) ) return;
            UvView.Draw( parentId );
            ImGui.EndTabItem();
        }

        private void DrawEmitterVerts( string parentId ) {
            if( !ImGui.BeginTabItem( "Emitter Vertices" + parentId ) ) return;
            EmitSplitDisplay.Draw( parentId );
            ImGui.EndTabItem();
        }

        private void ImportDialog() {
            FileDialogManager.OpenFileDialog( "Select a File", ".gltf,.*", ( bool ok, string res ) => {
                if( !ok ) return;
                try {
                    if( GltfUtils.ImportModel( res, out var newVertexes, out var newIndexes ) ) {
                        CommandManager.Avfx.Add( new AvfxModelImportCommand( this, newIndexes, newVertexes ) );
                    }
                }
                catch( Exception e ) {
                    PluginLog.Error( e, "Could not import data" );
                }
            } );
        }

        private void ExportDialog() {
            FileDialogManager.SaveFileDialog( "Select a Save Location", ".gltf", "model", "gltf", ( bool ok, string res ) => {
                if( !ok ) return;
                GltfUtils.ExportModel( this, res );
            } );
        }

        public void RefreshModelPreview() { Refresh = true; }

        public override string GetDefaultText() => $"Model {GetIdx()}";

        public override string GetWorkspaceId() => $"Mdl{GetIdx()}";
    }
}
