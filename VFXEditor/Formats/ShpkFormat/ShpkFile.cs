using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Formats.ShpkFormat.Keys;
using VfxEditor.Formats.ShpkFormat.Materials;
using VfxEditor.Formats.ShpkFormat.Nodes;
using VfxEditor.Formats.ShpkFormat.Shaders;
using VfxEditor.Ui.Components;
using VfxEditor.Ui.Components.SplitViews;

namespace VfxEditor.Formats.ShpkFormat {
    // Based on https://github.com/Ottermandias/Penumbra.GameData/blob/15ae65921468a2407ecdd068ca79947e596e24be/Files/ShpkFile.cs#L6

    public enum DX {
        DX9,
        DX11,
        UNKNOWN
    }

    public class ShpkFile : FileManagerFile {
        private readonly uint Version;
        private readonly uint DxMagic;
        public DX DxVersion => DxMagic switch {
            0x00395844u => DX.DX9,
            0x31315844u => DX.DX11,
            _ => DX.UNKNOWN
        };

        private readonly List<ShpkShader> VertexShaders = new();
        private readonly List<ShpkShader> PixelShaders = new();

        private readonly List<ShpkMaterialParmeter> MaterialParameters = new();

        private readonly List<ShpkParameterInfo> Constants = new();
        private readonly List<ShpkParameterInfo> Samplers = new();
        private readonly List<ShpkParameterInfo> Resources = new();

        private readonly List<ShpkKey> SystemKeys = new();
        private readonly List<ShpkKey> SceneKeys = new();
        private readonly List<ShpkKey> MaterialKeys = new();
        private readonly List<ShpkKey> SubViewKeys = new();

        private readonly List<ShpkNode> Nodes = new();
        private readonly List<ShpkAlias> Aliases = new();

        private readonly CommandDropdown<ShpkShader> VertexView;
        private readonly CommandDropdown<ShpkShader> PixelView;
        private readonly CommandSplitView<ShpkMaterialParmeter> MaterialParameterView;
        private readonly CommandSplitView<ShpkParameterInfo> ConstantView;
        private readonly CommandSplitView<ShpkParameterInfo> SamplerView;
        private readonly CommandSplitView<ShpkParameterInfo> ResourceView;

        private readonly CommandSplitView<ShpkKey> SystemKeyView;
        private readonly CommandSplitView<ShpkKey> SceneKeyView;
        private readonly CommandSplitView<ShpkKey> MaterialKeyView;
        private readonly CommandSplitView<ShpkKey> SubViewKeyView;

        private readonly CommandDropdown<ShpkNode> NodeView;

        public ShpkFile( BinaryReader reader, bool verify ) : base( new( Plugin.ShpkManager ) ) {
            reader.ReadInt32(); // Magic
            Version = reader.ReadUInt32();
            DxMagic = reader.ReadUInt32();
            reader.ReadInt32(); // File length

            var shaderOffset = reader.ReadUInt32();
            var parameterOffset = reader.ReadUInt32();

            var numVertex = reader.ReadUInt32();
            var numPixel = reader.ReadUInt32();

            var materialParamSize = reader.ReadUInt32();
            var numMaterialParams = reader.ReadUInt32();

            var numConstants = reader.ReadUInt32();
            var numSamplers = reader.ReadUInt32();
            var numResources = reader.ReadUInt32();

            var numSystemKey = reader.ReadUInt32();
            var numSceneKey = reader.ReadUInt32();
            var numMaterialKey = reader.ReadUInt32();

            var numNode = reader.ReadUInt32();
            var numAlias = reader.ReadUInt32();

            for( var i = 0; i < numVertex; i++ ) VertexShaders.Add( new( reader, true, DxVersion ) );
            for( var i = 0; i < numPixel; i++ ) PixelShaders.Add( new( reader, false, DxVersion ) );

            for( var i = 0; i < numMaterialParams; i++ ) MaterialParameters.Add( new( reader ) );

            for( var i = 0; i < numConstants; i++ ) Constants.Add( new( reader ) );
            for( var i = 0; i < numSamplers; i++ ) Samplers.Add( new( reader ) );
            for( var i = 0; i < numResources; i++ ) Resources.Add( new( reader ) );

            for( var i = 0; i < numSystemKey; i++ ) SystemKeys.Add( new( reader ) );
            for( var i = 0; i < numSceneKey; i++ ) SceneKeys.Add( new( reader ) );
            for( var i = 0; i < numMaterialKey; i++ ) MaterialKeys.Add( new( reader ) );

            var subViewKey1Default = reader.ReadUInt32();
            var subViewKey2Default = reader.ReadUInt32();
            SubViewKeys.Add( new( 1, subViewKey1Default ) );
            SubViewKeys.Add( new( 2, subViewKey2Default ) );

            for( var i = 0; i < numNode; i++ ) Nodes.Add( new( reader, SystemKeys.Count, SceneKeys.Count, MaterialKeys.Count, SubViewKeys.Count ) );
            for( var i = 0; i < numAlias; i++ ) Aliases.Add( new( reader ) );

            // ======= POPULATE ==========

            VertexShaders.ForEach( x => x.Read( reader, parameterOffset, shaderOffset ) );
            PixelShaders.ForEach( x => x.Read( reader, parameterOffset, shaderOffset ) );
            Constants.ForEach( x => x.Read( reader, parameterOffset ) );
            Samplers.ForEach( x => x.Read( reader, parameterOffset ) );
            Resources.ForEach( x => x.Read( reader, parameterOffset ) );

            // ====== CONSTRUCT VIEWS ==========

            VertexView = new( "Vertex Shader", VertexShaders, null, () => new( true, DxVersion ), () => CommandManager.Shpk );
            PixelView = new( "Pixel Shader", PixelShaders, null, () => new( false, DxVersion ), () => CommandManager.Shpk );

            MaterialParameterView = new( "Material Parameter", MaterialParameters, false, null, () => new(), () => CommandManager.Shpk );

            ConstantView = new( "Constant", Constants, false, ( ShpkParameterInfo item, int idx ) => item.GetText(), () => new(), () => CommandManager.Shpk );
            SamplerView = new( "Sampler", Samplers, false, ( ShpkParameterInfo item, int idx ) => item.GetText(), () => new(), () => CommandManager.Shpk );
            ResourceView = new( "Resource", Resources, false, ( ShpkParameterInfo item, int idx ) => item.GetText(), () => new(), () => CommandManager.Shpk );

            SystemKeyView = new( "System Key", SystemKeys, false, ( ShpkKey item, int idx ) => item.GetText( idx ), () => new(), () => CommandManager.Shpk );
            SceneKeyView = new( "Scene Key", SceneKeys, false, ( ShpkKey item, int idx ) => item.GetText( idx ), () => new(), () => CommandManager.Shpk );
            MaterialKeyView = new( "Material Key", MaterialKeys, false, ( ShpkKey item, int idx ) => item.GetText( idx ), () => new(), () => CommandManager.Shpk );
            SubViewKeyView = new( "Sub-View Key", SubViewKeys, false, ( ShpkKey item, int idx ) => item.GetText( idx ), () => new(), () => CommandManager.Shpk );

            NodeView = new( "Node", Nodes, null, () => new(), () => CommandManager.Shpk );
        }

        public override void Write( BinaryWriter writer ) {
            // TODO

            /*
             *         // Ceil required size to a multiple of 16 bytes.
        // Offsets can be skipped, MaterialParamsConstantId's size is the count.
        MaterialParamsSize = (GetConstantById(MaterialParamsConstantId)?.Size ?? 0u) << 4;
        foreach (var param in MaterialParams)
            MaterialParamsSize = Math.Max(MaterialParamsSize, (uint)param.ByteOffset + param.ByteSize);
        MaterialParamsSize = (MaterialParamsSize + 0xFu) & ~0xFu;
             */
        }

        public override void Draw() {
            ImGui.Separator();

            ImGui.TextDisabled( $"Version: {Version} DirectX: {DxVersion}" );

            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            using( var tab = ImRaii.TabItem( "Vertex Shaders" ) ) {
                if( tab ) VertexView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Pixel Shaders" ) ) {
                if( tab ) PixelView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Material Parameters" ) ) {
                if( tab ) MaterialParameterView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Constants" ) ) {
                if( tab ) ConstantView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Samplers" ) ) {
                if( tab ) SamplerView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Resources" ) ) {
                if( tab ) ResourceView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Nodes" ) ) {
                if( tab ) NodeView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Keys" ) ) {
                if( tab ) DrawKeys();
            }
        }

        private void DrawKeys() {
            using var _ = ImRaii.PushId( "Keys" );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );

            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            using( var tab = ImRaii.TabItem( "System" ) ) {
                if( tab ) SystemKeyView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Scene" ) ) {
                if( tab ) SceneKeyView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Material" ) ) {
                if( tab ) MaterialKeyView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Sub-View" ) ) {
                if( tab ) SubViewKeyView.Draw();
            }
        }
    }
}
