using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using VfxEditor.FileManager;
using VfxEditor.Formats.ShpkFormat.Keys;
using VfxEditor.Formats.ShpkFormat.Materials;
using VfxEditor.Formats.ShpkFormat.Nodes;
using VfxEditor.Formats.ShpkFormat.Shaders;
using VfxEditor.Parsing;
using VfxEditor.Ui.Components;
using VfxEditor.Ui.Components.SplitViews;
using VfxEditor.Utils;
using static VfxEditor.Utils.ShaderUtils;

namespace VfxEditor.Formats.ShpkFormat {
    // Based on https://github.com/Ottermandias/Penumbra.GameData/blob/15ae65921468a2407ecdd068ca79947e596e24be/Files/ShpkFile.cs#L6
    // And other work by Ny

    public class ShpkFile : FileManagerFile {
        public const uint MaterialParamsConstantId = 0x64D12851u;
        public const uint TableSamplerId = 0x2005679Fu;

        public const uint HULL_DOMAIN_GEO_SHADERS_VERSION = 0x0D01;
        public const uint NODE_ALIAS_CLUSTER_VERSION = 0x0E01;

        private readonly uint Version;
        private readonly uint DxMagic;
        public DX DxVersion => GetDxVersion( DxMagic );
        public readonly bool IsLegacy;

        private readonly List<ShpkShader> VertexShaders = [];
        private readonly List<ShpkShader> PixelShaders = [];
        private readonly List<ShpkShader> HullShaders = [];
        private readonly List<ShpkShader> DomainShaders = [];
        private readonly List<ShpkShader> GeometryShaders = [];

        public readonly ParsedBool HasDefaultMaterialValues = new( "Default Values" );
        public readonly List<ShpkMaterialParmeter> MaterialParameters = [];
        public readonly List<ShpkParameterInfo> Constants = [];
        public readonly List<ShpkParameterInfo> Samplers = [];
        public readonly List<ShpkParameterInfo> Textures = [];
        public readonly List<ShpkParameterInfo> Resources = [];

        public readonly List<ShpkKey> SystemKeys = [];
        public readonly List<ShpkKey> SceneKeys = [];
        public readonly List<ShpkKey> MaterialKeys = [];
        public readonly List<ShpkKey> SubViewKeys = [];

        private readonly List<ShpkNode> Nodes = [];
        private readonly List<ShpkAlias> Aliases = [];
        private readonly List<ShpkNodeAliasCluster> NodeAliasClusters = [];

        private readonly CommandDropdown<ShpkShader> VertexView;
        private readonly CommandDropdown<ShpkShader> PixelView;
        private readonly CommandDropdown<ShpkShader> HullView;
        private readonly CommandDropdown<ShpkShader> DomainView;
        private readonly CommandDropdown<ShpkShader> GeometryView;

        private readonly CommandSplitView<ShpkMaterialParmeter> MaterialParameterView;
        private readonly CommandSplitView<ShpkParameterInfo> ConstantView;
        private readonly CommandSplitView<ShpkParameterInfo> SamplerView;
        private readonly CommandSplitView<ShpkParameterInfo> TextureView;
        private readonly CommandSplitView<ShpkParameterInfo> ResourceView;

        private readonly CommandSplitView<ShpkKey> SystemKeyView;
        private readonly CommandSplitView<ShpkKey> SceneKeyView;
        private readonly CommandSplitView<ShpkKey> MaterialKeyView;
        private readonly CommandSplitView<ShpkKey> SubViewKeyView;

        private readonly CommandDropdown<ShpkNode> NodeView;
        private readonly CommandSplitView<ShpkAlias> AliasView;
        private readonly CommandDropdown<ShpkNodeAliasCluster> ClusterView;

        public ShpkFile( BinaryReader reader, bool verify ) : this( reader, null, verify ) { }

        public ShpkFile( BinaryReader reader, CommandManager manager, bool verify ) : base( manager ) {
            reader.ReadInt32(); // Magic
            Version = reader.ReadUInt32();
            DxMagic = reader.ReadUInt32();

            reader.ReadInt32(); // File length
            var shaderOffset = reader.ReadUInt32();
            var parameterOffset = reader.ReadUInt32();

            var numVertex = reader.ReadUInt32();
            var numPixel = reader.ReadUInt32();

            var materialParamsSize = reader.ReadUInt32(); // Material parameters size
            var numMaterialParams = reader.ReadUInt16();
            HasDefaultMaterialValues.Value = reader.ReadUInt16() != 0;

            var numConstants = reader.ReadUInt16();
            var unk1 = reader.ReadUInt16();

            var numSamplers = reader.ReadUInt16();
            var numTextures = reader.ReadUInt16();

            var numResources = reader.ReadUInt16();
            var unk2 = reader.ReadUInt16();

            IsLegacy = Version < HULL_DOMAIN_GEO_SHADERS_VERSION && !HasDefaultMaterialValues.Value && numTextures == 0;

            var numSystemKey = reader.ReadUInt32();
            var numSceneKey = reader.ReadUInt32();
            var numMaterialKey = reader.ReadUInt32();

            var numNode = reader.ReadUInt32();
            var numAlias = reader.ReadUInt32();

            var numHull = 0u;
            var numDomain = 0u;
            var numGeo = 0u;

            if( Version >= HULL_DOMAIN_GEO_SHADERS_VERSION ) {
                numHull = reader.ReadUInt32();
                numDomain = reader.ReadUInt32();
                numGeo = reader.ReadUInt32();
            }

            var nodeAliasClusterCount = Version >= NODE_ALIAS_CLUSTER_VERSION ? reader.ReadUInt32() : 0;

            for( var i = 0; i < numVertex; i++ ) VertexShaders.Add( new( reader, ShaderStage.Vertex, Version, DxVersion, true, ShaderFileType.Shpk, IsLegacy ) );
            for( var i = 0; i < numPixel; i++ ) PixelShaders.Add( new( reader, ShaderStage.Pixel, Version, DxVersion, true, ShaderFileType.Shpk, IsLegacy ) );
            for( var i = 0; i < numHull; i++ ) HullShaders.Add( new( reader, ShaderStage.Hull, Version, DxVersion, true, ShaderFileType.Shpk, IsLegacy ) );
            for( var i = 0; i < numDomain; i++ ) DomainShaders.Add( new( reader, ShaderStage.Domain, Version, DxVersion, true, ShaderFileType.Shpk, IsLegacy ) );
            for( var i = 0; i < numGeo; i++ ) GeometryShaders.Add( new( reader, ShaderStage.Geometry, Version, DxVersion, true, ShaderFileType.Shpk, IsLegacy ) );

            for( var i = 0; i < numMaterialParams; i++ ) MaterialParameters.Add( new( this, reader ) );

            if( HasDefaultMaterialValues.Value ) {
                var defaultStart = reader.BaseStream.Position;
                foreach( var param in MaterialParameters ) {
                    reader.BaseStream.Position = defaultStart + param.Offset.Value;
                    param.DefaultValue.Read( reader );
                }
                reader.BaseStream.Position = defaultStart + materialParamsSize;
            }
            for( var i = 0; i < numConstants; i++ ) Constants.Add( new( reader, ShaderFileType.Shpk ) );
            for( var i = 0; i < numSamplers; i++ ) Samplers.Add( new( reader, ShaderFileType.Shpk ) );
            for( var i = 0; i < numTextures; i++ ) Textures.Add( new( reader, ShaderFileType.Shpk ) );
            for( var i = 0; i < numResources; i++ ) Resources.Add( new( reader, ShaderFileType.Shpk ) );

            for( var i = 0; i < numSystemKey; i++ ) SystemKeys.Add( new( reader ) );
            for( var i = 0; i < numSceneKey; i++ ) SceneKeys.Add( new( reader ) );
            for( var i = 0; i < numMaterialKey; i++ ) MaterialKeys.Add( new( reader ) );

            SubViewKeys.Add( new( 1, reader.ReadUInt32() ) );
            SubViewKeys.Add( new( 2, reader.ReadUInt32() ) );

            for( var i = 0; i < numNode; i++ ) Nodes.Add( new( reader, SystemKeys.Count, SceneKeys.Count, MaterialKeys.Count, SubViewKeys.Count ) );
            for( var i = 0; i < numAlias; i++ ) Aliases.Add( new( reader ) );
            for( var i = 0; i < nodeAliasClusterCount; i++ ) NodeAliasClusters.Add( new( reader ) );

            // ======= POPULATE ==========

            VertexShaders.ForEach( x => x.Read( reader, parameterOffset, shaderOffset ) );
            PixelShaders.ForEach( x => x.Read( reader, parameterOffset, shaderOffset ) );
            HullShaders.ForEach( x => x.Read( reader, parameterOffset, shaderOffset ) );
            DomainShaders.ForEach( x => x.Read( reader, parameterOffset, shaderOffset ) );
            GeometryShaders.ForEach( x => x.Read( reader, parameterOffset, shaderOffset ) );

            Constants.ForEach( x => x.Read( reader, parameterOffset ) );
            Samplers.ForEach( x => x.Read( reader, parameterOffset ) );
            Textures.ForEach( x => x.Read( reader, parameterOffset ) );
            Resources.ForEach( x => x.Read( reader, parameterOffset ) );

            // ====== CONSTRUCT VIEWS ==========

            VertexView = new( "Vertex Shader", VertexShaders, null, () => new( ShaderStage.Vertex, Version, DxVersion, true, ShaderFileType.Shpk, IsLegacy ) );
            PixelView = new( "Pixel Shader", PixelShaders, null, () => new( ShaderStage.Pixel, Version, DxVersion, true, ShaderFileType.Shpk, IsLegacy ) );
            HullView = new( "Hull Shader", HullShaders, null, () => new( ShaderStage.Hull, Version, DxVersion, true, ShaderFileType.Shpk, IsLegacy ) );
            DomainView = new( "Domain Shader", DomainShaders, null, () => new( ShaderStage.Domain, Version, DxVersion, true, ShaderFileType.Shpk, IsLegacy ) );
            GeometryView = new( "Geometry Shader", GeometryShaders, null, () => new( ShaderStage.Geometry, Version, DxVersion, true, ShaderFileType.Shpk, IsLegacy ) );

            MaterialParameterView = new( "Parameter", MaterialParameters, false, null, () => new( this ) );

            ConstantView = new( "Constant", Constants, false, ( item, idx ) => item.GetText(), () => new( ShaderFileType.Shpk ) );
            SamplerView = new( "Sampler", Samplers, false, ( item, idx ) => item.GetText(), () => new( ShaderFileType.Shpk ) );
            TextureView = new( "Texture", Textures, false, ( item, idx ) => item.GetText(), () => new( ShaderFileType.Shpk ) );
            ResourceView = new( "Resource", Resources, false, ( item, idx ) => item.GetText(), () => new( ShaderFileType.Shpk ) );

            SystemKeyView = new( "System Key", SystemKeys, false, ( item, idx ) => item.GetText( idx ), () => new() );
            SceneKeyView = new( "Scene Key", SceneKeys, false, ( item, idx ) => item.GetText( idx ), () => new() );
            MaterialKeyView = new( "Material Key", MaterialKeys, false, ( item, idx ) => item.GetText( idx ), () => new() );
            SubViewKeyView = new( "Sub-View Key", SubViewKeys, false, ( item, idx ) => item.GetText( idx ), () => new() );

            NodeView = new( "Node", Nodes, null, () => new() );
            AliasView = new( "Alias", Aliases, false, null, () => new() );
            ClusterView = new( "Cluster", NodeAliasClusters, null, () => new() );

            // TODO: don't be dumb when adding keys, actually update selectors and stuff
            // TOOD: when adding keys, make sure to do it everywhere

            if( verify ) Verified = FileUtils.Verify( reader, ToBytes() );
        }


        public override void Write( BinaryWriter writer ) {
            writer.Write( 0x6B506853u ); // Magic
            writer.Write( Version );
            writer.Write( DxMagic );

            var placeholderPos = writer.BaseStream.Position;
            writer.Write( 0 ); // size
            writer.Write( 0 ); // shader offset
            writer.Write( 0 ); // parameter offset

            writer.Write( VertexShaders.Count );
            writer.Write( PixelShaders.Count );

            var materialParamsSize = Constants.FirstOrDefault( x => x.Id == MaterialParamsConstantId )?.DataSize ?? 0u;
            foreach( var param in MaterialParameters ) {
                materialParamsSize = ( uint )Math.Max( materialParamsSize, ( uint )param.Offset.Value + ( int )param.Size.Value );
            }
            materialParamsSize = ( materialParamsSize + 0xFu ) & ~0xFu;
            writer.Write( materialParamsSize );
            writer.Write( ( ushort )MaterialParameters.Count );
            writer.Write( ( ushort )( HasDefaultMaterialValues.Value ? 1 : 0 ) );

            writer.Write( Constants.Count );

            writer.Write( ( ushort )Samplers.Count );
            writer.Write( ( ushort )Textures.Count );

            writer.Write( Resources.Count );

            writer.Write( SystemKeys.Count );
            writer.Write( SceneKeys.Count );
            writer.Write( MaterialKeys.Count );

            writer.Write( Nodes.Count );
            writer.Write( Aliases.Count );

            if( Version >= HULL_DOMAIN_GEO_SHADERS_VERSION ) {
                writer.Write( HullShaders.Count );
                writer.Write( DomainShaders.Count );
                writer.Write( GeometryShaders.Count );
            }

            if( Version >= NODE_ALIAS_CLUSTER_VERSION ) writer.Write( NodeAliasClusters.Count );

            var stringPositions = new List<(long, string)>();
            var shaderPositions = new List<(long, ShpkShader)>();

            VertexShaders.ForEach( x => x.Write( writer, stringPositions, shaderPositions ) );
            PixelShaders.ForEach( x => x.Write( writer, stringPositions, shaderPositions ) );
            HullShaders.ForEach( x => x.Write( writer, stringPositions, shaderPositions ) );
            DomainShaders.ForEach( x => x.Write( writer, stringPositions, shaderPositions ) );
            GeometryShaders.ForEach( x => x.Write( writer, stringPositions, shaderPositions ) );

            MaterialParameters.ForEach( x => x.Write( writer ) );

            if( HasDefaultMaterialValues.Value ) {
                var defaultStart = writer.BaseStream.Position;
                FileUtils.Pad( writer, materialParamsSize );
                foreach( var param in MaterialParameters ) {
                    writer.BaseStream.Position = defaultStart + param.Offset.Value;
                    param.DefaultValue.Write( writer );
                }
                writer.BaseStream.Position = defaultStart + materialParamsSize;
            }

            Constants.ForEach( x => x.Write( writer, stringPositions ) );
            Samplers.ForEach( x => x.Write( writer, stringPositions ) );
            Textures.ForEach( x => x.Write( writer, stringPositions ) );
            Resources.ForEach( x => x.Write( writer, stringPositions ) );

            SystemKeys.ForEach( x => x.Write( writer ) );
            SceneKeys.ForEach( x => x.Write( writer ) );
            MaterialKeys.ForEach( x => x.Write( writer ) );

            SubViewKeys.ForEach( x => writer.Write( x.DefaultValue.Value ) );

            Nodes.ForEach( x => x.Write( writer ) );
            Aliases.ForEach( x => x.Write( writer ) );
            NodeAliasClusters.ForEach( x => x.Write( writer ) );

            WriteOffsetsSHPK( writer, placeholderPos, stringPositions, shaderPositions );
        }

        public override void Draw() {
            ImGui.Separator();
            ImGui.TextDisabled( $"Version: 0x{Version:X4} DirectX: {DxVersion}" );

            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            using( var tab = ImRaii.TabItem( "Vertex Shaders" ) ) {
                if( tab ) VertexView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Pixel Shaders" ) ) {
                if( tab ) PixelView.Draw();
            }

            if( Version >= HULL_DOMAIN_GEO_SHADERS_VERSION ) {
                using( var tab = ImRaii.TabItem( "Hull Shaders" ) ) {
                    if( tab ) HullView.Draw();
                }

                using( var tab = ImRaii.TabItem( "Domain Shaders" ) ) {
                    if( tab ) DomainView.Draw();
                }

                using( var tab = ImRaii.TabItem( "Geometry Shaders" ) ) {
                    if( tab ) GeometryView.Draw();
                }
            }

            using( var tab = ImRaii.TabItem( "Material Parameters" ) ) {
                if( tab ) {
                    if( !IsLegacy ) HasDefaultMaterialValues.Draw();
                    DrawMaterialTable();
                    ImGui.Separator();
                    MaterialParameterView.Draw();
                }
            }

            using( var tab = ImRaii.TabItem( "Constants" ) ) {
                if( tab ) ConstantView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Samplers" ) ) {
                if( tab ) SamplerView.Draw();
            }

            if( !IsLegacy ) {
                using var tab = ImRaii.TabItem( "Textures" );
                if( tab ) TextureView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Resources" ) ) {
                if( tab ) ResourceView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Keys" ) ) {
                if( tab ) DrawKeys();
            }

            using( var tab = ImRaii.TabItem( "Nodes" ) ) {
                if( tab ) NodeView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Aliases" ) ) {
                if( tab ) AliasView.Draw();
            }

            if( Version >= NODE_ALIAS_CLUSTER_VERSION ) {
                using var tab = ImRaii.TabItem( "Clusters" );
                if( tab ) ClusterView.Draw();
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

        private void DrawMaterialTable() {
            using var _ = ImRaii.PushId( "MaterialParameters" );

            ImGui.Dummy( Vector2.One );
            using var table = ImRaii.Table( "Table", 5, ImGuiTableFlags.RowBg | ImGuiTableFlags.ScrollY | ImGuiTableFlags.NoSavedSettings, new( ImGui.GetContentRegionAvail().X, 200 ) );
            if( !table ) return;

            using( var font = ImRaii.PushFont( UiBuilder.MonoFont ) ) {
                ImGui.TableSetupScrollFreeze( 0, 1 );
                ImGui.TableSetupColumn( string.Empty, ImGuiTableColumnFlags.WidthFixed, 50 );
                ImGui.TableSetupColumn( "x", ImGuiTableColumnFlags.WidthStretch );
                ImGui.TableSetupColumn( "y", ImGuiTableColumnFlags.WidthStretch );
                ImGui.TableSetupColumn( "z", ImGuiTableColumnFlags.WidthStretch );
                ImGui.TableSetupColumn( "w", ImGuiTableColumnFlags.WidthStretch );
                ImGui.TableHeadersRow();
            }

            var rows = MaterialParameters.Count == 0 ? 0 : ( int )MaterialParameters.Select( x => Math.Ceiling( ( float )x.EndSlot / 4 ) ).Max();

            for( var i = 0; i < rows; i++ ) {
                ImGui.TableNextColumn();

                using( var font = ImRaii.PushFont( UiBuilder.MonoFont ) ) {
                    ImGui.TableHeader( $" [{i}]" );
                    UiUtils.Tooltip( $"g_MaterialParameter[{i}]" );
                }

                for( var j = 0; j < 4; j++ ) {
                    var slot = ( 4 * i ) + j;
                    var parameters = MaterialParameters.FindAll( x => slot >= x.StartSlot && slot < x.EndSlot );
                    var parameter = parameters.FirstOrDefault();

                    ImGui.TableNextColumn();

                    using var disabled = ImRaii.Disabled( parameter == null || slot != parameter.StartSlot );
                    using var none = ImRaii.PushColor( ImGuiCol.Text, UiUtils.RED_COLOR, parameter == null );
                    using var selected = ImRaii.PushColor( ImGuiCol.Text, UiUtils.PARSED_GREEN, parameter != null && parameter == MaterialParameterView.GetSelected() );
                    using var multiple = ImRaii.PushColor( ImGuiCol.Text, UiUtils.DALAMUD_ORANGE, parameters.Count > 1 );

                    if( ImGui.Selectable( parameter == null ? "[NONE]" : $"Parameter {MaterialParameters.IndexOf( parameter )}" ) && parameter != null ) {
                        MaterialParameterView.SetSelected( parameter );
                    }
                }
            }
        }
    }
}
