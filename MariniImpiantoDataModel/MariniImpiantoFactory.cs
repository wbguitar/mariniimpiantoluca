using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Libreria per poter lavorare coi file
using System.IO;
// Libreria per la gestione dei file XML. Permette di usare classi come XmlAttributeCollection, XmlNode, ...;
using System.Xml;

namespace MariniImpiantoDataModel
{

    ///// <summary>
    ///// The 'Creator' abstract class
    ///// </summary>
    //public abstract class MariniGenericDataTreeFactory
    //{
    //    private MariniBaseObject _mbo = new MariniBaseObject();

    //    // Constructor calls abstract Factory method
    //    public MariniGenericDataTreeFactory(string filename)
    //    {
    //        this.CreateDataTree(filename);
    //    }

    //    public MariniBaseObject Mbo
    //    {
    //        get { return _mbo; }
    //        set { Mbo = value; }
    //    }
    //    // Factory Method
    //    public abstract void CreateDataTree(string filename);
    //}

    //public class MariniImpiantoFactory : MariniGenericDataTreeFactory
    //{
    //    public override void CreateDataTree(string filename)
    //    {
    //        XmlDocument doc = new XmlDocument();
    //        try 
    //        {	        
    //            doc.Load(filename);
    //            XmlNode root = doc.SelectSingleNode("*");
    //            Mbo = (MariniBaseObject)MariniObjectCreator.CreateMariniObject(root);
    //        }
    //        catch (Exception e)
    //        {
    //            Console.WriteLine("MariniImpiantoFactory: error on creating a Marini Data Tree from XML file {0}", filename);
    //            throw e;
    //        }
            
    //    }
    //}
}
