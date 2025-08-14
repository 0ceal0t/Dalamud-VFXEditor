using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VfxEditor.FileBrowser;
using VfxEditor.Interop;
using VfxEditor.Ui.Components.SplitViews;
using VfxEditor.Ui.Interfaces;
using static VfxEditor.Utils.ShaderUtils;

namespace VfxEditor.Formats.ShpkFormat.Shaders {
    public class ShpkShader : IUiItem {
        public static string TempCso => Path.Combine( Plugin.Configuration.WriteLocation, "temp_cso.cso" ).Replace( '\\', '/' );
        public static string TempDxbc => Path.Combine( Plugin.Configuration.WriteLocation, "temp_dxbc.dxbc" ).Replace( '\\', '/' );

        public readonly ShaderStage Stage;
        public readonly DX DxVersion;
        public readonly bool IsV7;
        public string Extension => DxVersion == DX.DX11 ? "dxbc" : "cso";
        public readonly bool HasResources;

        public readonly ShaderFileType Type;

        private readonly int TempOffset;
        private readonly int TempSize;

        private readonly List<ShpkParameterInfo> Constants = [];
        private readonly List<ShpkParameterInfo> Samplers = [];
        private readonly List<ShpkParameterInfo> Resources = [];
        private readonly List<ShpkParameterInfo> Textures = [];

        private readonly CommandSplitView<ShpkParameterInfo> ConstantView;
        private readonly CommandSplitView<ShpkParameterInfo> SamplerView;
        private readonly CommandSplitView<ShpkParameterInfo> ResourceView;
        private readonly CommandSplitView<ShpkParameterInfo> TextureView;

        private byte[] ExtraData = [];
        private byte[] Data = [];

        private int ExtraSize => Stage switch {
            ShaderStage.Vertex => DxVersion == DX.DX9 ? 4 : 8,
            _ => 0
        };

        private string Prefix => Stage switch {
            ShaderStage.Vertex => "vs",
            ShaderStage.Pixel => "ps",
            _ => ""
        };

        private bool BinLoaded = false;
        private string BinDump = "";

        public ShpkShader( ShaderStage stage, DX dxVersion, bool hasResources, ShaderFileType type, bool isV7 ) {
            Stage = stage;
            HasResources = hasResources;
            DxVersion = dxVersion;
            Type = type;
            IsV7 = isV7;

            ConstantView = new( "Constant", Constants, false, ( ShpkParameterInfo item, int idx ) => item.GetText(), () => new( type ) );
            SamplerView = new( "Sampler", Samplers, false, ( ShpkParameterInfo item, int idx ) => item.GetText(), () => new( type ) );
            if( HasResources ) {
                ResourceView = new( "Resource", Resources, false, ( ShpkParameterInfo item, int idx ) => item.GetText(), () => new( type ) );
                TextureView = new( "Texture", Textures, false, ( ShpkParameterInfo item, int idx ) => item.GetText(), () => new( type ) );
            }
        }

        public ShpkShader( BinaryReader reader, ShaderStage stage, DX dxVersion, bool hasResources, ShaderFileType type, bool isV7 ) : this( stage, dxVersion, hasResources, type, isV7 ) {
            TempOffset = reader.ReadInt32();
            TempSize = reader.ReadInt32();

            var numConstants = reader.ReadInt16();
            var numSamplers = reader.ReadInt16();
            var numRw = 0;
            var numTextures = 0;
            if( HasResources ) {
                numRw = reader.ReadInt16();
                numTextures = reader.ReadInt16();
            }

            for( var i = 0; i < numConstants; i++ ) Constants.Add( new( reader, Type ) );
            for( var i = 0; i < numSamplers; i++ ) Samplers.Add( new( reader, Type ) );
            if( HasResources ) {
                for( var i = 0; i < numRw; i++ ) Resources.Add( new( reader, Type ) );
                for( var i = 0; i < numTextures; i++ ) Textures.Add( new( reader, Type ) );
            }
        }

        public void Read( BinaryReader reader, uint parameterOffset, uint shaderOffset ) {
            Constants.ForEach( x => x.Read( reader, parameterOffset ) );
            Samplers.ForEach( x => x.Read( reader, parameterOffset ) );
            Resources.ForEach( x => x.Read( reader, parameterOffset ) );
            Textures.ForEach( x => x.Read( reader, parameterOffset ) );

            reader.BaseStream.Position = shaderOffset + TempOffset;
            ExtraData = reader.ReadBytes( ExtraSize );
            Data = reader.ReadBytes( TempSize - ExtraSize );
        }

        public void Write( BinaryWriter writer, List<(long, string)> stringPositions, List<(long, ShpkShader)> shaderPositions ) {
            shaderPositions.Add( (writer.BaseStream.Position, this) );
            writer.Write( 0 ); // placeholder
            writer.Write( Data.Length + ExtraData.Length );

            writer.Write( ( short )Constants.Count );
            writer.Write( ( short )Samplers.Count );
            if( HasResources ) {
                writer.Write( ( short )Resources.Count );
                writer.Write( ( short )Textures.Count );
            }

            Constants.ForEach( x => x.Write( writer, stringPositions ) );
            Samplers.ForEach( x => x.Write( writer, stringPositions ) );
            if( HasResources ) {
                Resources.ForEach( x => x.Write( writer, stringPositions ) );
                Textures.ForEach( x => x.Write( writer, stringPositions ) );
            }
        }

        public void WriteByteCode( BinaryWriter writer, long shaderOffset, long placeholderPosition ) {
            var offset = writer.BaseStream.Position - shaderOffset;

            var savePos = writer.BaseStream.Position;
            writer.BaseStream.Position = placeholderPosition;
            writer.Write( ( uint )offset );
            writer.BaseStream.Position = savePos;

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

            if( !HasResources ) return;

            using( var tab = ImRaii.TabItem( "Resources" ) ) {
                if( tab ) ResourceView.Draw();
            }

            if( IsV7 ) {
                using var tab = ImRaii.TabItem( "Textures" );
                if( tab ) TextureView.Draw();
            }
        }

        private void DrawCode() {
            using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing );

            if( ImGui.Button( "Export" ) ) {
                FileBrowserManager.SaveFileDialog( "Select a Save Location", $".{Extension}", "shader_" + Prefix, Extension, ( bool ok, string res ) => {
                    if( !ok ) return;
                    File.WriteAllBytes( res, Data );
                } );
            }

            ImGui.SameLine();
            if( ImGui.Button( "Replace" ) ) {
                FileBrowserManager.OpenFileDialog( "Select a File", DxVersion == DX.DX11 ? "Shader{.hlsl,." + Extension + "},.*" : $".{Extension},.*", ( bool ok, string res ) => {
                    if( !ok ) return;

                    if( Path.GetExtension( res ) == ".hlsl" ) {
                        InteropUtils.Run( "d3d/fxc.exe", $"/T {Prefix}_5_0 \"{res}\" /Fo \"{TempDxbc}\" /O3", true, out var output );
                        Dalamud.Log( output );
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
