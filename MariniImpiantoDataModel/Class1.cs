using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Libreria per far funzionare InotifyPropertyChanged.
// InotifyPropertyChanged è l'interfaccia per notificare a clienti (in generale binding clients) che una
// proprietà è cambiata
using System.ComponentModel;
// Libreria per far funzionare la serializzazione. Ad esmpio per decorare le proprietà da
// serializzare con [XmlElement("oggettobase", Type = typeof(MariniOggettoBase))]
using System.Xml.Serialization;
// Libreria per far funzionare l'attributo [CallerMemberName] presente nella OnPropertyChanged(…).
// In questo modo non ci si deve più preoccupare di rinominare le proprietà.
using System.Runtime.CompilerServices;

namespace MariniImpiantoDataModel
{
    /// <summary>
    /// Represents a generic object of MariniImpianto
    /// </summary>
    public abstract class MariniGenericObject : INotifyPropertyChanged
    {

        #region properties

        

        private MariniGenericObject _parent;
        /// <summary>
        /// Gets and Sets the parent of an object
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public MariniGenericObject parent { get { return _parent; } set { _parent = value; } }

        //[System.Xml.Serialization.XmlElementAttribute("type")]
        private string _type;
        /// <summary>
        /// Gets and Sets the type of an object
        /// </summary>
        [System.Xml.Serialization.XmlAttribute]
        public string type { get { return _type; } set { _type = value; } }

        //[System.Xml.Serialization.XmlElementAttribute("id")]
        private string _id;
        /// <summary>
        /// Gets and Sets the ID of an object
        /// </summary>
        [System.Xml.Serialization.XmlAttribute]
        public string id { get { return _id; } set { _id = value; } }

        private string _name;
        /// <summary>
        /// Gets and Sets the name of an object
        /// </summary>
        [System.Xml.Serialization.XmlAttribute]
        public string name { get { return _name; } set { SetField(ref _name, value); } }

        //[System.Xml.Serialization.XmlElementAttribute("id")]
        private string _path;
        /// <summary>
        /// Gets and Sets the path of an object
        /// </summary>
        [System.Xml.Serialization.XmlAttribute]
        public string path { get { return _path; } set { _path = value; } }

        private string _description;
        /// <summary>
        /// Gets and Sets the description of an object
        /// </summary>
        [System.Xml.Serialization.XmlAttribute]
        public string description { get { lock (synchro) { return _description; } } set { SetField(ref _description, value); } }
        
        private string _handler;
        /// <summary>
        /// Gets and Sets the handler method of an object propertychanged event
        /// </summary>
        [System.Xml.Serialization.XmlAttribute]
        public string handler { get { return _handler; } set { SetField(ref _handler, value); } }

        private readonly List<MariniGenericObject> _listaGenericObject = new List<MariniGenericObject>();
        /// <summary>
        /// Gets the list of children objects
        /// </summary>
        /*
        [XmlElement("Impianto", Type = typeof(MariniImpianto))]
        [XmlElement("ZonaPredosaggio", Type = typeof(MariniZonaPredosaggio))]
        [XmlElement("Predosatore", Type = typeof(MariniPredosatore))]
        [XmlElement("Bilancia", Type = typeof(MariniBilancia))]
        [XmlElement("Nastro", Type = typeof(MariniNastro))]
        */
        [XmlElement("Property", Type = typeof(MariniProperty))]
        [XmlElement("oggettobase", Type = typeof(MariniOggettoBase))]
        public List<MariniGenericObject> ListaGenericObject { get { return _listaGenericObject; } }

        /// <summary>
        /// Threadsafe implementation of the event <see cref="MariniGenericObject.OnPropertyChanged"/>
        /// </summary>
        readonly object synchro = new object();

        #endregion

        #region events
        
        

        // By default the event implementation is not threadsafe.
        // We will do a thread-safe implementation of the event
        // So this is the old implementation
        /*
        public event PropertyChangedEventHandler PropertyChanged;
        */
        // And this is the new implementation
        private PropertyChangedEventHandler internalPropertyChanged;
        /// <summary>
        /// Occurs when a property is changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                lock (synchro)
                {
                    internalPropertyChanged += value;
                }
            }
            remove
            {
                lock (synchro)
                {
                    internalPropertyChanged -= value;
                }
            }
        }

        private void OnPropertyChanged(string property)
        {
            //Send notification without acquiring the synchro object
            PropertyChangedEventHandler localCopy;
            lock (synchro)
            {
                localCopy = internalPropertyChanged != null ? (PropertyChangedEventHandler)internalPropertyChanged.Clone() : null;
            }

            if (localCopy != null)
            {
                localCopy(this, new PropertyChangedEventArgs(property));
            }
        }


        //protected bool SetField<T>(ref T field, T value, string propertyName)
        /// <summary>
        /// Used in every property set to launch <see cref="MariniGenericObject.OnPropertyChanged"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns><c>true</c> if the property is effectively changed; otherwise, <c>false</c>.</returns>
        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            lock (synchro)
            {
                if (EqualityComparer<T>.Default.Equals(field, value))
                {
                    //Console.WriteLine("Sono nella SetField e la proprieta' non e' cambiata");
                    return false;
                }
                field = value;
            }

            //Console.WriteLine("Sono nella SetField e lancio OnPropertyChanged(propertyName)");
            OnPropertyChanged(propertyName);

            return true;
        }

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MariniGenericObject"/> class.
        /// </summary>
        /// <param name="parent">MariniGenericObject parent</param>
        /// <param name="id">MariniGenericObject ID</param>
        /// <param name="name">MariniGenericObject name</param>
        /// <param name="description">MariniGenericObject description</param>
        /// <param name="handler">MariniGenericObject method name to handle the PropertyChange event</param>
        protected MariniGenericObject(MariniGenericObject parent, string id, string name, string description, string handler)
        {
            this.parent = parent;
            this.id = id;
            this.name = name;
            this.description = description;
            this.handler = handler;

            SetObjPath(parent);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MariniGenericObject"/> class.
        /// </summary>
        /// <param name="parent">MariniGenericObject parent</param>
        /// <param name="id">MariniGenericObject ID</param>
        /// <param name="name">MariniGenericObject name</param>
        /// <param name="description"></param>
        protected MariniGenericObject(MariniGenericObject parent, string id, string name, string description)
            : this(parent, id, name, description, "NO_HANDLER")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MariniGenericObject"/> class.
        /// </summary>
        /// <param name="parent">MariniGenericObject parent</param>
        /// <param name="id">MariniGenericObject ID</param>
        /// <param name="name">MariniGenericObject name</param>
        protected MariniGenericObject(MariniGenericObject parent, string id, string name)
            : this(parent, id, name, "NO_DESCRIPTION")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MariniGenericObject"/> class.
        /// </summary>
        /// <param name="parent">MariniGenericObject parent</param>
        /// <param name="id">MariniGenericObject ID</param>
        protected MariniGenericObject(MariniGenericObject parent, string id)
            : this(parent, id, "NO_NAME")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MariniGenericObject"/> class.
        /// </summary>
        /// <param name="parent">MariniGenericObject parent</param>
        protected MariniGenericObject(MariniGenericObject parent)
            : this(parent, "NO_ID")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MariniGenericObject"/> class.
        /// </summary>
        protected MariniGenericObject()
            : this(null, "NO_ID")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MariniGenericObject"/> class.
        /// </summary>
        /// <param name="parent">MariniGenericObject parent</param>
        /// <param name="node">An Xml node from which to construct the object</param>
        protected MariniGenericObject(MariniGenericObject parent, XmlNode node)
            : this(parent)
        {
            if (node.Attributes != null)
            {
                XmlAttributeCollection attrs = node.Attributes;
                foreach (XmlAttribute attr in attrs)
                {
                    //Console.WriteLine("Attribute Name = " + attr.Name + "; Attribute Value = " + attr.Value);

                    switch (attr.Name)
                    {
                        case "id":
                            id = attr.Value;
                            break;
                        case "name":
                            name = attr.Value;
                            break;
                        case "description":
                            description = attr.Value;
                            break;
                        case "handler":
                            handler = attr.Value;
                            break;
                        //default:
                        //    throw new ApplicationException(string.Format("MariniObject '{0}' cannot be created", mgo));
                    }
                }
            }

            SetObjPath(parent);

        }

        private void SetObjPath(MariniGenericObject parent)
        {
            if (parent == null)
            {
                path = id;
            }
            else
            {
                path = parent.path + "." + id;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MariniGenericObject"/> class.
        /// </summary>
        /// <param name="node">An Xml node from which to construct the object</param>
        protected MariniGenericObject(XmlNode node)
            : this(null, node)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Plain description of the MariniGenericObject.
        /// </summary>
        public abstract void ToPlainText();

        /// <summary>
        /// Plain description of the MariniGenericObject and, recursively, it's children.
        /// </summary>
        public void ToPlainTextRecursive()
        {
            ToPlainText();
            foreach (MariniGenericObject mgo in _listaGenericObject)
            {
                mgo.ToPlainTextRecursive();
            }
            return;
        }

        /// <summary>
        /// Retrieve the MariniPlctag bound to the property prop_name
        /// </summary>
        /// <param name="prop_name">the property bound to the plctag</param>
        /// <returns></returns>
        public MariniPlctag GetPropertyBoundPlctagObject(string prop_name)
        {

            //MariniPlctag mplctag = ListaGenericObject
            //    .Where(mgo => mgo.GetType() == typeof(MariniPlctag))
            //    .Cast<MariniPlctag>()
            //    .FirstOrDefault(mp => mp.parent_property_bind == prop_name);

            return ListaGenericObject
                .Where(mgo => mgo.GetType() == typeof(MariniPlctag))
                .Cast<MariniPlctag>()
                .FirstOrDefault(mp => mp.parent_property_bind == prop_name);
        }



        /// <summary>
        /// Retrieve the child with the given ID
        /// </summary>
        /// <param name="id">MariniGenericObject ID</param>
        /// <returns>The MariniGenericObject or null if not found</returns>
        public MariniGenericObject GetObjectById(string id)
        {
            MariniGenericObject mgo = null;
            _GetObjectById(id, ref mgo);
            return mgo;
        }

        private void _GetObjectById(string id, ref MariniGenericObject mgo)
        {
            if (this.id == id)
            {
                mgo = this;
                return;
            }
            else
            {
                if (_listaGenericObject.Count > 0)
                {
                    foreach (MariniGenericObject child in _listaGenericObject)
                    {
                        child._GetObjectById(id, ref mgo);
                    }
                }
            }
        }

        /// <summary>
        /// Retrieve the child with the given ID
        /// </summary>
        /// <param name="id">MariniGenericObject ID</param>
        /// <returns>The MariniGenericObject or null if not found</returns>
        public MariniGenericObject GetObjectByPath(string path)
        {
            MariniGenericObject mgo = null;
            _GetObjectByPath(path, ref mgo);
            return mgo;
        }

        private void _GetObjectByPath(string path, ref MariniGenericObject mgo)
        {
            if (this.path == path)
            {
                mgo = this;
                return;
            }
            else
            {
                if (_listaGenericObject.Count > 0)
                {
                    foreach (MariniGenericObject child in _listaGenericObject)
                    {
                        child._GetObjectByPath(path, ref mgo);
                    }
                }
            }
        }






        /// <summary>
        /// Retrieve a list of MariniGenericObject child of a specific type
        /// </summary>
        /// <param name="type">The type of MariniGenericObject</param>
        /// <returns>A list of MariniGenericObject</returns>
        public List<MariniGenericObject> GetObjectListByType(Type type)
        {
            List<MariniGenericObject> mgoList = new List<MariniGenericObject>();
            _GetObjectListByType(type, ref mgoList);
            return mgoList;
        }

        private void _GetObjectListByType(Type type, ref List<MariniGenericObject> mgoList)
        {
            if (this.GetType() == type)
            {
                mgoList.Add(this);
                //return;
            }

            if (_listaGenericObject.Count > 0)
            {
                foreach (MariniGenericObject child in _listaGenericObject)
                {
                    child._GetObjectListByType(type, ref  mgoList);
                }
            }
        }

        /// <summary>
        /// Retrieve a dictionary of MariniGenericObject children
        /// </summary>
        /// <returns>The children dictionary</returns>
        public Dictionary<string, MariniGenericObject> GetIdChildDictionary()
        {
            Dictionary<string, MariniGenericObject> md = new Dictionary<string, MariniGenericObject>();
            _GetIdChildDictionary(ref md);
            return md;
        }

        private void _GetIdChildDictionary(ref Dictionary<string, MariniGenericObject> md)
        {
            md.Add(this.id, this);
            if (_listaGenericObject.Count > 0)
            {
                foreach (MariniGenericObject child in _listaGenericObject)
                {
                    child._GetIdChildDictionary(ref md);
                }
            }
        }




        /// <summary>
        /// Retrieve a dictionary of MariniGenericObject children
        /// </summary>
        /// <returns>The children dictionary</returns>
        public Dictionary<string, MariniGenericObject> GetPathChildDictionary()
        {
            Dictionary<string, MariniGenericObject> md = new Dictionary<string, MariniGenericObject>();
            _GetPathChildDictionary(ref md);
            return md;
        }

        private void _GetPathChildDictionary(ref Dictionary<string, MariniGenericObject> md)
        {
            md.Add(this.path, this);
            if (_listaGenericObject.Count > 0)
            {
                foreach (MariniGenericObject child in _listaGenericObject)
                {
                    child._GetPathChildDictionary(ref md);
                }
            }
        }




        #endregion
    }

}
