using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVFXLib.AVFX;
using Newtonsoft.Json.Linq;

namespace AVFXLib.Models
{
    public abstract class Base
    {
        public bool Assigned { get; set; } = false;
        public string JSONPath { get; set; }
        public string AVFXName { get; set; }

        public Base()
        {
            JSONPath = "";
            AVFXName = "";
        }
        public Base(string jsonPath, string avfxName)
        {
            JSONPath = jsonPath;
            AVFXName = avfxName;
        }

        public abstract AVFXNode toAVFX();
        public abstract JToken toJSON();
        public abstract void read(AVFXNode node);

        // ====== DEFAULT =======
        public virtual void toDefault() { } // TEMP
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
            attribute.toDefault();
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
            foreach (Base attribute in attributes)
            {
                ReadAVFX(attribute, node);
            }
        }
        public static void ReadAVFX(Base attribute, AVFXNode node)
        {
            if (attribute == null || node == null) return;
            foreach (AVFXNode item in node.Children)
            {
                if(item.Name == attribute.AVFXName)
                {
                    if (attribute is LiteralBase)
                    {
                        LiteralBase literal = (LiteralBase)attribute;
                        literal.read((AVFXLeaf)item);
                    }
                    else
                    {
                        attribute.read(item);
                    }
                    break;
                }
            }
        }
        // ===== EXPORT JSON =======
        public static void PutJSON(JObject obj, List<Base> sources)
        {
            if (obj == null || sources == null) return;
            foreach (Base b in sources)
            {
                PutJSON(obj, b);
            }
        }
        public static void PutJSON(JObject obj, Base source)
        {
            if (obj == null || source == null) return;
            if (!source.Assigned) return;
            obj[source.JSONPath] = source.toJSON();
        }
        // ======= EXPORT AVFX ========
        public static void PutAVFX(AVFXNode destination, List<Base> sources)
        {
            if (destination == null || sources == null) return;
            foreach (Base b in sources)
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
                return b.toAVFX();
            }
            return new AVFXBlank();
        }
    }
}
