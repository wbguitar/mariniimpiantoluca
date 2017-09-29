using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Libreria per la gestione dei file XML. Permette di usare classi come XmlAttributeCollection, XmlNode, ...;
using System.Xml;


namespace MariniImpiantoDataModel
{

    public class MariniBaseObject : MariniGenericObject
    {

        public MariniBaseObject(MariniGenericObject parent, string type, string id, string name, string description)
            : base(parent, type, id, name, description)
        {
        }

        public MariniBaseObject(MariniGenericObject parent, string type, string id, string name)
            : base(parent, type, id, name)
        {
        }

        public MariniBaseObject(MariniGenericObject parent, string type, string id)
            : base(parent, type, id)
        {
        }

        public MariniBaseObject(MariniGenericObject parent, string type)
            : base(parent, type)
        {
        }

        public MariniBaseObject(MariniGenericObject parent)
            : base(parent)
        {
        }

        public MariniBaseObject()
            : base()
        {
        }

        public MariniBaseObject(MariniGenericObject parent, XmlNode node)
            : base(parent, node)
        {
        }

        public MariniBaseObject(XmlNode node)
            : base(node)
        {
        }

        public override void ToPlainText()
        {
            Console.WriteLine("Sono un oggetto base id: {0} name: {1} description: {2} path: {3}", id, name, description, path);
        }
    }
}
