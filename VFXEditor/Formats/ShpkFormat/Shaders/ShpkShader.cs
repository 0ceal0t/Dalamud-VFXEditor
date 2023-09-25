using Dalamud.Interface;
using Dalamud.Logging;
using ImGuiFileDialog;
using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Interop;
using VfxEditor.Ui.Components.SplitViews;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.ShpkFormat.Shaders {
    public class ShpkShader : IUiItem {
        public static string TempCso => Path.Combine( Plugin.Configuration.WriteLocation, $"temp_cso.cso" ).Replace( '\\', '/' );
        public static string TempDxil => Path.Combine( Plugin.Configuration.WriteLocation, $"temp_dxil.dxil" ).Replace( '\\', '/' );
        public static string TempDxbc => Path.Combine( Plugin.Configuration.WriteLocation, $"temp_dxbc.dxbc" ).Replace( '\\', '/' );

        public readonly DX DxVersion;
        public string Extension => DxVersion == DX.DX11 ? "dxbc" : "cso";

        public readonly bool Vertex;

        private readonly int TempOffset;
        private readonly int TempSize;

        private readonly List<ShpkParameterInfo> Constants = new();
        private readonly List<ShpkParameterInfo> Samplers = new();
        private readonly List<ShpkParameterInfo> Resources = new();

        private readonly CommandSplitView<ShpkParameterInfo> ConstantView;
        private readonly CommandSplitView<ShpkParameterInfo> SamplerView;
        private readonly CommandSplitView<ShpkParameterInfo> ResourceView;

        private byte[] ExtraData = [];
        private byte[] Data = [];
        private int ExtraSize => Vertex ? ( DxVersion == DX.DX9 ? 4 : 8 ) : 0;

        private bool BinLoaded = false;
        private string BinDump = "";

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

        public void Write( BinaryWriter writer, List<(long, string)> stringPositions, List<(long, ShpkShader)> shaderPositions ) {
            shaderPositions.Add( (writer.BaseStream.Position, this) );
            writer.Write( 0 ); // placeholder
            writer.Write( Data.Length + ExtraData.Length );

            writer.Write( ( short )Constants.Count );
            writer.Write( ( short )Samplers.Count );
            writer.Write( ( short )Resources.Count );
            writer.Write( ( short )0 );
            Constants.ForEach( x => x.Write( writer, stringPositions ) );
            Samplers.ForEach( x => x.Write( writer, stringPositions ) );
            Resources.ForEach( x => x.Write( writer, stringPositions ) );
        }

        public void WriteByteCode( BinaryWriter writer, long shaderOffset, long placeholderPosition ) {
            var offset = writer.BaseStream.Position - shaderOffset;

            var savePos = writer.BaseStream.Position;
            writer.BaseStream.Seek( placeholderPosition, SeekOrigin.Begin );
            writer.Write( ( uint )offset );
            writer.BaseStream.Seek( savePos, SeekOrigin.Begin );

            writer.Write( ExtraData );
            writer.Write( Data );
        }

        public void Draw() {
            using var _ = ImRaii.PushId( "Shader" );

            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            using( var tab = ImRaii.TabItem( "Code" ) ) {
                if( tab ) DrawCode();
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
        }

        private void DrawCode() {
            using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing );

            if( ImGui.Button( "Export" ) ) {
                FileDialogManager.SaveFileDialog( "Select a Save Location", $".{Extension}", "shader_" + ( Vertex ? "vs" : "ps" ), Extension, ( bool ok, string res ) => {
                    if( !ok ) return;
                    File.WriteAllBytes( res, Data );
                } );
            }

            ImGui.SameLine();
            if( ImGui.Button( "Replace" ) ) {
                FileDialogManager.OpenFileDialog( "Select a File", $".{Extension},.*", ( bool ok, string res ) => {
                    if( !ok ) return;
                    Data = File.ReadAllBytes( res );
                    BinLoaded = false;
                } );
            }

            if( !BinLoaded ) {
                BinLoaded = true;
                RefreshBin();
            }

            using var font = ImRaii.PushFont( UiBuilder.MonoFont );
            ImGui.InputTextMultiline( "##BinDump", ref BinDump, 100000, new( -1, -1 ), ImGuiInputTextFlags.ReadOnly );
        }

        private void RefreshBin() {
            if( DxVersion == DX.DX11 ) {
                File.WriteAllBytes( TempDxbc, Data );
                InteropUtils.Run( "d3d/dxbc2dxil.exe", $"\"{TempDxbc}\" /o \"{TempDxil}\"", false, out var _ );
                InteropUtils.Run( "d3d/dxc.exe", $"-dumpbin \"{TempDxil}\"", true, out BinDump );
            }
            else {
                File.WriteAllBytes( TempCso, Data );
                InteropUtils.Run( "d3d/fxc.exe", $"/nologo /dumpbin \"{TempCso}\"", true, out BinDump );
            }
        }
    }
}
