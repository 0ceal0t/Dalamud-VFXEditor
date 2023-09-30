using Dalamud.Interface;
using Dalamud.Logging;
using ImGuiFileDialog;
using ImGuiNET;
using OtterGui.Raii;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VfxEditor.Interop;
using VfxEditor.Ui.Components.SplitViews;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.ShpkFormat.Shaders {
    public class ShpkShader : IUiItem {
        public static string TempCso => Path.Combine( Plugin.Configuration.WriteLocation, $"temp_cso.cso" ).Replace( '\\', '/' );
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

        private byte[] ExtraData = Array.Empty<byte>();
        private byte[] Data = Array.Empty<byte>();
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
                FileDialogManager.OpenFileDialog( "Select a File", DxVersion == DX.DX11 ? "Shader{.hlsl,." + Extension + "},.*" : $".{Extension},.*", ( bool ok, string res ) => {
                    if( !ok ) return;

                    if( Path.GetExtension( res ) == ".hlsl" ) {
                        var target = Vertex ? "vs_5_0" : "ps_5_0";
                        InteropUtils.Run( "d3d/fxc.exe", $"/T {target} \"{res}\" /Fo \"{TempDxbc}\" /O3", true, out var output );
                        PluginLog.Log( output );
                        if( !output.Contains( "compilation failed" ) ) {
                            Data = File.ReadAllBytes( TempDxbc );
                            BinLoaded = false;
                        }
                    }
                    else {
                        Data = File.ReadAllBytes( res );
                        BinLoaded = false;
                    }
                } );
            }

            if( !BinLoaded ) {
                BinLoaded = true;
                RefreshBin();
            }

            using var font = ImRaii.PushFont( UiBuilder.MonoFont );
            using var childColor = ImRaii.PushColor( ImGuiCol.ChildBg, 0x8A202020 );
            using var child = ImRaii.Child( "Child", new( -1, -1 ), true );
            using var spacing = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 0, 0 ) );

            if( string.IsNullOrEmpty( BinDump ) ) return;

            var lines = BinDump.Split( '\n' );
            foreach( var line in lines ) {
                var split = line.Split( "//", 2 );
                var preComment = split[0];
                var code = preComment.TrimStart();
                var whitespace = preComment.Length - code.Length;

                ImGui.Text( preComment[0..whitespace] );

                DrawCode( code );

                if( split.Length > 1 ) {
                    using var commentColor = ImRaii.PushColor( ImGuiCol.Text, ImGui.GetColorU32( ImGuiCol.TextDisabled ) );
                    ImGui.SameLine();
                    ImGui.Text( "//" );
                    ImGui.SameLine();
                    ImGui.Text( split[1] );
                }
            }
        }

        private static void DrawCode( string code ) {
            if( string.IsNullOrEmpty( code ) ) return;

            var split = code.Split( " ", 2 );

            using( var color = ImRaii.PushColor( ImGuiCol.Text, 0xFF66FFFF ) ) {
                ImGui.SameLine();
                ImGui.Text( split[0] );
            }

            if( split.Length > 1 ) {
                ImGui.SameLine();
                ImGui.Text( $" {split[1]}" );
            }
        }

        private void RefreshBin() {
            if( DxVersion == DX.DX11 ) {
                BinDump = D3DCompiler.Disassemble( Data, D3DCompiler.DisassembleFlags.DisableDebugInfo ).ToString();
            }
            else {
                File.WriteAllBytes( TempCso, Data );
                InteropUtils.Run( "d3d/fxc.exe", $"/nologo /dumpbin \"{TempCso}\"", true, out BinDump );
            }
        }
    }
}
