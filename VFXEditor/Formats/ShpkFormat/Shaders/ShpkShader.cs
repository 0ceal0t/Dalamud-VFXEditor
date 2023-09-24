using Dalamud.Logging;
using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Ui.Components.SplitViews;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.ShpkFormat.Shaders {
    public class ShpkShader : IUiItem {
        public readonly DX DxVersion;
        public readonly bool Vertex;

        private readonly int TempOffset;
        private readonly int TempSize;

        private readonly List<ShpkParameterInfo> Constants = new();
        private readonly List<ShpkParameterInfo> Samplers = new();
        private readonly List<ShpkParameterInfo> Resources = new();

        private readonly CommandSplitView<ShpkParameterInfo> ConstantView;
        private readonly CommandSplitView<ShpkParameterInfo> SamplerView;
        private readonly CommandSplitView<ShpkParameterInfo> ResourceView;

        private byte[] ExtraData;
        private byte[] Data;

        private int ExtraSize => Vertex ? ( DxVersion == DX.DX9 ? 4 : 8 ) : 0;

        public ShpkShader( bool vertex, DX dxVersion ) {
            Vertex = vertex;
            DxVersion = dxVersion;

            ConstantView = new( "Constant", Constants, false, ( ShpkParameterInfo item, int idx ) => item.GetText(), () => new(), () => CommandManager.Shpk );
            SamplerView = new( "Sampler", Samplers, false, ( ShpkParameterInfo item, int idx ) => item.GetText(), () => new(), () => CommandManager.Shpk );
            ResourceView = new( "Resource", Resources, false, ( ShpkParameterInfo item, int idx ) => item.GetText(), () => new(), () => CommandManager.Shpk );
        }

        public ShpkShader( BinaryReader reader, bool vertex, DX dxVersion ) : this( vertex, dxVersion ) {
            TempOffset = reader.ReadInt32();
            TempSize = reader.ReadInt32();

            var numConstants = reader.ReadInt16();
            var numSamplers = reader.ReadInt16();
            var numRw = reader.ReadInt16();
            var numUnknown = reader.ReadInt16();

            for( var i = 0; i < numConstants; i++ ) Constants.Add( new( reader ) );
            for( var i = 0; i < numSamplers; i++ ) Samplers.Add( new( reader ) );
            for( var i = 0; i < numRw; i++ ) Resources.Add( new( reader ) );

            if( numUnknown != 0 ) PluginLog.Error( "Unknown data" );
        }

        public void Read( BinaryReader reader, uint parameterOffset, uint shaderOffset ) {
            Constants.ForEach( x => x.Read( reader, parameterOffset ) );
            Samplers.ForEach( x => x.Read( reader, parameterOffset ) );
            Resources.ForEach( x => x.Read( reader, parameterOffset ) );

            reader.BaseStream.Seek( shaderOffset + TempOffset, SeekOrigin.Begin );
            ExtraData = reader.ReadBytes( ExtraSize );
            Data = reader.ReadBytes( TempSize - ExtraSize );
        }

        public void Write( BinaryWriter writer ) {

        }

        public void Draw() {
            using var _ = ImRaii.PushId( "Shader" );

            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            using( var tab = ImRaii.TabItem( "Constants" ) ) {
                if( tab ) ConstantView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Samplers" ) ) {
                if( tab ) SamplerView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Resources" ) ) {
                if( tab ) ResourceView.Draw();
            }
        }
    }
}
