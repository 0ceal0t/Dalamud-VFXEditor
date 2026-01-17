using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using VfxEditor.AvfxFormat.Model;
using VfxEditor.FileBrowser;
using VfxEditor.Ui.Components.Tables;
using VfxEditor.Utils;
using VfxEditor.Utils.Gltf;
using VfxEditor.DirectX;
using static VfxEditor.DirectX.ModelPreview;

namespace VfxEditor.AvfxFormat {
    public class AvfxModel : AvfxNode {
        public const string NAME = "Modl";

        public readonly AvfxFile File;

        public readonly AvfxVertexes Vertexes = new();
        public readonly AvfxIndexes Indexes = new();
        public readonly AvfxEmitVertexes EmitVertexes = new();
        public readonly AvfxEmitVertexNumbers EmitVertexNumbers = new();

        public readonly List<UiEmitVertex> AllEmitVertexes = [];
        public readonly List<UiVertexNumber> AllVertexNumbers = [];

        private readonly List<AvfxBase> Parsed;

        private readonly UiNodeGraphView NodeView;
        private readonly CommandTable<UiEmitVertex> VertexTable;
        private readonly CommandTable<UiVertexNumber> VertexNumberTable;

        public readonly int RenderId = RenderInstance.NewId;
        private bool NeedsRender = false;

        private int Mode = ( int )RenderMode.Color;
        private readonly UiModelUvView UvView;

        public AvfxModel( AvfxFile file ) : base( NAME, AvfxNodeGroupSet.ModelColor ) {
            File = file;

            Parsed = [
                EmitVertexNumbers,
                EmitVertexes,
                Vertexes,
                Indexes
            ];

            NodeView = new( this );
            UvView = new UiModelUvView();

            VertexNumberTable = new( "Number", true, true, AllVertexNumbers, [
                ( "Number", ImGuiTableColumnFlags.None, -1 )
            ],
            () => new( new() ), ( item, add ) => Updated() );

            VertexTable = new( "Emit", true, true, AllEmitVertexes, [
                ( "Position", ImGuiTableColumnFlags.None, -1 ),
                ( "Normal", ImGuiTableColumnFlags.None, -1 ),
                ( "Color", ImGuiTableColumnFlags.None, - 1),
            ],
            () => new( this, new() ), ( item, add ) => Updated() );
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            ReadNested( reader, Parsed, size );
            if( EmitVertexes.EmitVertexes.Count != EmitVertexNumbers.VertexNumbers.Count )
            {
                Dalamud.Error( $"Mismatched emit vertex counts {EmitVertexes.EmitVertexes.Count} {EmitVertexNumbers.VertexNumbers.Count}" );
            }
            for( var i = 0; i < EmitVertexes.EmitVertexes.Count; i++ )
            {
                AllEmitVertexes.Add( new UiEmitVertex( this, EmitVertexes.EmitVertexes[i] ) );
            }
            for( var i = 0; i < EmitVertexNumbers.VertexNumbers.Count; i++ )
            {
                AllVertexNumbers.Add( new UiVertexNumber( EmitVertexNumbers.VertexNumbers[i] ) );
            }
        }

        public override void WriteContents( BinaryWriter writer ) {
            EmitVertexNumbers.VertexNumbers.Clear();
            EmitVertexNumbers.VertexNumbers.AddRange( AllVertexNumbers.Select( x => x.Number ) );

            EmitVertexes.EmitVertexes.Clear();
            EmitVertexes.EmitVertexes.AddRange( AllEmitVertexes.Select( x => x.Vertex ) );

            if( EmitVertexNumbers.VertexNumbers.Count > 0 ) {
                EmitVertexNumbers.SetAssigned( true );
                EmitVertexNumbers.Write( writer );
            }

            if( EmitVertexes.EmitVertexes.Count > 0 ) {
                EmitVertexes.SetAssigned( true );
                EmitVertexes.Write( writer );
            }

            if( Vertexes.Vertexes.Count > 0 ) {
                Vertexes.SetAssigned( true );
                Vertexes.Write( writer );
            }

            if( Indexes.Indexes.Count > 0 ) {
                Indexes.SetAssigned( true );
                Indexes.Write( writer );
            }
        }

        protected override IEnumerable<AvfxBase> GetChildren() {
            foreach( var item in Parsed ) yield return item;
        }

        public override void Draw() {
            using var _ = ImRaii.PushId( "Model" );
            NodeView.Draw();
            DrawRename();

            ImGui.TextDisabled( $"Vertices: {Vertexes.Vertexes.Count} Indexes: {Indexes.Indexes.Count}" );

            using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing ) ) {
                if( ImGui.Button( "Export" ) ) ImGui.OpenPopup( "ExportPopup" );

                using( var popup = ImRaii.Popup( "ExportPopup" ) ) {
                    if( popup ) {
                        if( ImGui.Selectable( ".gltf" ) ) ExportDialog();
                        if( ImGui.Selectable( ".avfx" ) ) Plugin.AvfxManager.ShowExportDialog( this );
                    }
                }

                ImGui.SameLine();
                if( ImGui.Button( "Replace" ) ) ImportDialog();

                ImGui.SameLine();
                UiUtils.WikiButton( "https://github.com/0ceal0t/Dalamud-VFXEditor/wiki/Replacing-textures-and-models#models" );
            }

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 4 );

            using var tabBar = ImRaii.TabBar( "ModelTabs" );
            if( !tabBar ) return;

            using( var tab = ImRaii.TabItem( "3D View" ) ) {
                if( tab ) DrawModel3D();
            }

            using( var tab = ImRaii.TabItem( "UV View" ) ) {
                if( tab ) UvView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Vertex Order" ) ) {
                if( tab ) DrawVertexNumbers();
            }

            using( var tab = ImRaii.TabItem( "Emitter Vertices" ) ) {
                if( tab ) DrawEmitterVertices();
            }
        }

        private void DrawEmitterVertices() {
            var size = Plugin.Configuration.EmitterVertexSplitOpen ?
                new Vector2( ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y / 2f ) :
                new Vector2( ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y - UiUtils.AngleUpDownSize );

            VertexTable.Draw( size );

            if( UiUtils.DrawAngleUpDown( ref Plugin.Configuration.EmitterVertexSplitOpen ) ) Plugin.Configuration.Save();

            if( Plugin.Configuration.EmitterVertexSplitOpen ) {
                if( NeedsRender ) UpdateRender();
                Plugin.DirectXManager.ModelRenderer.DrawTexture( RenderId, File.ModelInstance, UpdateRender );
            }
        }

        private void DrawVertexNumbers() {
            VertexNumberTable.Draw();
        }

        private void DrawModel3D() {
            using var _ = ImRaii.PushId( "3DModel" );

            if( ImGui.Checkbox( "Show Edges", ref Plugin.Configuration.ModelShowEdges ) ) Plugin.DirectXManager.Redraw();

            ImGui.SameLine();
            if( ImGui.Checkbox( "Show Emitter Vertices", ref Plugin.Configuration.ModelShowEmitters ) ) Plugin.DirectXManager.Redraw();

            if( ImGui.RadioButton( "Color", ref Mode, ( int )RenderMode.Color ) ) UpdateRender();

            ImGui.SameLine();
            if( ImGui.RadioButton( "UV 1", ref Mode, ( int )RenderMode.Uv1 ) ) UpdateRender();

            ImGui.SameLine();
            if( ImGui.RadioButton( "UV 2", ref Mode, ( int )RenderMode.Uv2 ) ) UpdateRender();

            ImGui.SameLine();
            if( ImGui.RadioButton( "UV 3", ref Mode, ( int )RenderMode.Uv3 ) ) UpdateRender();

            ImGui.SameLine();
            if( ImGui.RadioButton( "UV 4", ref Mode, ( int )RenderMode.Uv4 ) ) UpdateRender();

            ImGui.SameLine();
            if( ImGui.RadioButton( "Normal", ref Mode, ( int )RenderMode.Normal ) ) UpdateRender();

            if( NeedsRender ) UpdateRender();
            Plugin.DirectXManager.ModelRenderer.DrawTexture( RenderId, File.ModelInstance, UpdateRender );
        }

        public void UpdateRender() {
            Plugin.DirectXManager.ModelRenderer.SetModel( RenderId, File.ModelInstance, this, ( RenderMode )Mode );
            UvView.LoadModel( this );
            NeedsRender = false;
        }

        private void ImportDialog() {
            FileBrowserManager.OpenFileDialog( "Select a File", "GLTF{.gltf,.glb},.*", ( ok, res ) => {
                if( !ok ) return;
                try {
                    if( GltfModel.ImportModel( res, out var newVertexes, out var newIndexes ) ) {
                        CommandManager.Add( new AvfxModelImportCommand( this, newIndexes, newVertexes ) );
                    }
                }
                catch( Exception e ) {
                    Dalamud.Error( e, "Could not import data" );
                }
            } );
        }

        private void ExportDialog() {
            FileBrowserManager.SaveFileDialog( "Select a Save Location", ".gltf", "model", "gltf", ( ok, res ) => {
                if( !ok ) return;
                GltfModel.ExportModel( this, res );
            } );
        }

        public void Updated() { NeedsRender = true; }

        public override string GetDefaultText() => $"Model {GetIdx()}";

        public override string GetWorkspaceId() => $"Mdl{GetIdx()}";
    }
}
