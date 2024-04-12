using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.InteropServices;

namespace VfxEditor.Interop.Penumbra {
    [StructLayout( LayoutKind.Explicit, Pack = 1, Size = 16 )]
    public readonly struct MetaManipulation {
        public enum Type : byte {
            Unknown = 0,
            Imc = 1,
            Eqdp = 2,
            Eqp = 3,
            Est = 4,
            Gmp = 5,
            Rsp = 6,
        }

        [FieldOffset( 0 )]
        [JsonIgnore]
        public readonly ImcManipulation Imc = default;

        [FieldOffset( 15 )]
        [JsonConverter( typeof( StringEnumConverter ) )]
        [JsonProperty( "Type" )]
        public readonly Type ManipulationType;

        public readonly object Manipulation {
            get => ManipulationType switch {
                Type.Unknown => null,
                Type.Imc => Imc,
                _ => null
            };
            init {
                switch( value ) {
                    case ImcManipulation m:
                        Imc = m;
                        ManipulationType = Type.Imc;
                        return;
                }
            }
        }

        public MetaManipulation() { }
    }
}
