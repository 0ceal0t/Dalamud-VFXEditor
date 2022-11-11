using Dalamud.Logging;
using ImGuiFileDialog;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VfxEditor.AvfxFormat.Vfx.Model;
using VfxEditor.AVFXLib.Model;
using VfxEditor.Utils;
using static VfxEditor.DirectX.ModelPreview;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiModel : UiNode {
        public AVFXModel Model;
        public List<UiModelEmitterVertex> EmitterVerts;
        public UiModelEmitSplitView EmitSplit;
        public UiNodeGraphView NodeView;

        private int Mode = (int)RenderMode.Color;
        private bool Refresh = false;
        private readonly UiModelUvView UvView;

        public UiModel( AVFXModel model ) : base( UiNodeGroup.ModelColor, false ) {
            Model = model;
            NodeView = new UiNodeGraphView( this );

            UvView = new UiModelUvView(); // don't load until actually necessary

            EmitterVerts = new List<UiModelEmitterVertex>();
            for( var i = 0; i < Math.Min( Model.VertexNumbers.VertexNumbers.Count, Model.EmitVertexes.EmitVertexes.Count ); i++ ) {
                EmitterVerts.Add( new UiModelEmitterVertex( Model.VertexNumbers.VertexNumbers[i], Model.EmitVertexes.EmitVertexes[i], this ) );
            }
            EmitSplit = new UiModelEmitSplitView( EmitterVerts, this );
            HasDependencies = false; // if imported, all set now
        }

        public override void DrawInline( string parentId ) {
            var id = parentId + "/Model";
            NodeView.DrawInline( id );
            DrawRename( id );
            ImGui.Text( "Vertices: " + Model.Vertexes.Vertexes.Count + " " + "Indexes: " + Model.Indexes.Indexes.Count );
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
            Plugin.DirectXManager.ModelPreview.LoadModel( Model, RenderMode.Color );
            UvView.LoadModel( Model );
        }

        private void DrawModel3D( string parentId ) {
            if( !ImGui.BeginTabItem( "3D View" + parentId ) ) return;
            if( Refresh ) {
                Plugin.DirectXManager.ModelPreview.LoadModel( Model, (RenderMode)Mode );
                UvView.LoadModel( Model );
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
            if( ImGui.RadioButton( "Color", ref Mode, (int)RenderMode.Color ) ) {
                Plugin.DirectXManager.ModelPreview.LoadModel( Model, RenderMode.Color );
            }
            ImGui.SameLine();
            if( ImGui.RadioButton( "UV 1", ref Mode, ( int )RenderMode.Uv1 ) ) {
                Plugin.DirectXManager.ModelPreview.LoadModel( Model,RenderMode.Uv1 );
            }
            ImGui.SameLine();
            if( ImGui.RadioButton( "UV 2", ref Mode, ( int )RenderMode.Uv2 ) ) {
                Plugin.DirectXManager.ModelPreview.LoadModel( Model, RenderMode.Uv2 );
            }

            Plugin.DirectXManager.ModelPreview.DrawInline();

            ImGui.EndTabItem();
        }

        private void DrawUvView( string parentId ) {
            if( !ImGui.BeginTabItem( "UV View" + parentId ) ) return;
            UvView.DrawInline( parentId );
            ImGui.EndTabItem();
        }

        private void DrawEmitterVerts( string parentId ) {
            if( !ImGui.BeginTabItem( "Emitter Vertices" + parentId ) ) return;
            EmitSplit.DrawInline( parentId );
            ImGui.EndTabItem();
        }

        private void ImportDialog() {
            FileDialogManager.OpenFileDialog( "Select a File", ".gltf,.*", ( bool ok, string res ) => {
                if( !ok ) return;
                try {
                    if( GltfUtils.ImportModel( res, out var newVertexes, out var newIndexes ) ) {
                        Model.Vertexes.Vertexes.Clear();
                        Model.Vertexes.Vertexes.AddRange( newVertexes );

                        Model.Indexes.Indexes.Clear();
                        Model.Indexes.Indexes.AddRange( newIndexes );
                        Refresh = true;
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
                GltfUtils.ExportModel( Model, res );
            } );
        }

        public override string GetDefaultText() => $"Model {Idx}";

        public override string GetWorkspaceId() => $"Mdl{Idx}";

        public override void Write( BinaryWriter writer ) => Model.Write( writer );
    }
}
