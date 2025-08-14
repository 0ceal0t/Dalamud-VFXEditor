using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VfxEditor.Formats.ShpkFormat.Materials;
using VfxEditor.Parsing;
using VfxEditor.Parsing.Int;
using VfxEditor.Ui.Components;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.MtrlFormat.Shader {
    public class MtrlMaterialParameter : IUiItem {
        private readonly MtrlFile File;

        public readonly ParsedUIntPicker<ShpkMaterialParmeter> Id;
        public readonly List<ParsedFloat> Values = [];

        private readonly ushort TempOffset;
        private readonly ushort TempSize;

        private readonly CommandListView<ParsedFloat> ValueView;

        public MtrlMaterialParameter( MtrlFile file ) {
            File = file;
            ValueView = new( Values, () => new( "##Value" ), true );
            Id = new( "Parameter",
                () => File.ShaderFile?.MaterialParameters,
                ( ShpkMaterialParmeter item, int _ ) => item.GetText(),
                ( ShpkMaterialParmeter item ) => item.Id.Value
            );
        }

        public MtrlMaterialParameter( MtrlFile file, BinaryReader reader ) : this( file ) {
            Id.Read( reader );
            TempOffset = reader.ReadUInt16();
            TempSize = reader.ReadUInt16();
        }

        public void PickValues( List<float> values ) {
            foreach( var item in values.GetRange( TempOffset / 4, TempSize / 4 ) ) {
                Values.Add( new ParsedFloat( "##Value", item ) );
            }
        }

        public void Write( BinaryWriter writer, List<long> constantPositions ) {
            Id.Write( writer );
            constantPositions.Add( writer.BaseStream.Position );
            writer.Write( ( ushort )0 ); // placeholder
            writer.Write( ( ushort )( Values.Count * 4 ) );
        }

        public void Draw() {
            Id.Draw();
            using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 4, 2 ) ) ) {
                ImGui.SameLine();
            }
            ImGui.TextDisabled( $"[{File.ShaderFilePath.Split( '/' )[^1]}]" );

            ValueView.Draw();
        }
    }
}
