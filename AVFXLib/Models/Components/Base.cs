using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVFXLib.AVFX;

namespace AVFXLib.Models
{
    public abstract class Base {
        public bool Assigned { get; set; } = false;
        public string AVFXName { get; set; }

        public Base()
        {
            AVFXName = "";
        }
        public Base(string avfxName)
        {
            AVFXName = avfxName;
        }

        public abstract AVFXNode ToAVFX();
        public abstract void Read(AVFXNode node);

        // ====== DEFAULT =======
        public virtual void ToDefault() { } // TEMP
        public static void SetDefault(List<Base> attributes)
        {
            foreach (var attribute in attributes)
            {
                SetDefault(attribute);
            }
        }
        public static void SetDefault(Base attribute)
        {
            if (attribute == null) return;
            attribute.ToDefault();
        }
        public static void SetUnAssigned(List<Base> attributes)
        {
            foreach (var attribute in attributes)
            {
                SetUnAssigned(attribute);
            }
        }
        public static void SetUnAssigned(Base attribute)
        {
            if (attribute == null) return;
            attribute.Assigned = false;
        }
        // ====== READ =========
        public static void ReadAVFX(List<Base> attributes, AVFXNode node)
        {
            if (attributes == null || node == null) return;
            foreach (var attribute in attributes)
            {
                ReadAVFX(attribute, node);
            }
        }
        public static void ReadAVFX(Base attribute, AVFXNode node)
        {
            if (attribute == null || node == null) return;
            foreach (var item in node.Children)
            {
                if(item.Name == attribute.AVFXName)
                {
                    if (attribute is LiteralBase)
                    {
                        var literal = (LiteralBase)attribute;
                        literal.read( (AVFXLeaf)item);
                    }
                    else
                    {
                        attribute.Read( item);
                    }
                    break;
                }
            }
        }
        // ======= EXPORT AVFX ========
        public static void PutAVFX(AVFXNode destination, List<Base> sources)
        {
            if (destination == null || sources == null) return;
            foreach (var b in sources)
            {
                PutAVFX(destination, b);
            }
        }
        public static void PutAVFX(AVFXNode destination, Base source)
        {
            if (destination == null || source == null) return;
            destination.Children.Add(GetAVFX(source));
        }
        public static AVFXNode GetAVFX(Base b)
        {
            if (b == null) return null;
            if (b.Assigned)
            {
                return b.ToAVFX();
            }
            return new AVFXBlank();
        }
    }
}
