﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Libreria per la gestione dei file XML. Permette di usare classi come XmlAttributeCollection, XmlNode, ...;
using System.Xml;
// Libreria per usare l'oggetto MethodInfo e tutti gli altri metodi reflection usati per accoppiare gli handlers con gli eventi
using System.Reflection;


namespace MariniImpiantoDataModel
{
    /// <summary>
    /// Classe singleton con la quale accedere al modello dell'impianto Marini. Crea dinamicamente il modello
    /// partendo da un file xml. Contiene alcuni metodi di supporto per la gestione del modello.
    /// </summary>
    public class MariniImpiantoDataManager
    {
        
        private MariniBaseObject _dataTree;
        /// <summary>
        /// Gets the actual MariniImpiantoDataTree
        /// </summary>
        public MariniBaseObject DataTree
        {
            get
            {
                return _dataTree;
            }
        }

        private Dictionary<string, MariniGenericObject> _pathObjectsDictionary = new Dictionary<string,MariniGenericObject>();
        /// <summary>
        /// Gets the dictionary of all object that composed MariniImpianto with path key
        /// </summary>
        public Dictionary<string, MariniGenericObject> PathObjectsDictionary
        {
            get
            {
                return _pathObjectsDictionary;
            }
        }

        private List<IMariniEventsHandlers> _eventsHandlersList = new List<IMariniEventsHandlers>();
        /// <summary>
        /// Gets the class with event handler methods
        /// </summary>
        public List<IMariniEventsHandlers> EventsHandlersList
        {
            get
            {
                return _eventsHandlersList;
            }
        }

        //private IMariniEventsHandlers _eventsHandlers;
        ///// <summary>
        ///// Gets the class with event handler methods
        ///// </summary>
        //public IMariniEventsHandlers EventsHandlers
        //{
        //    get
        //    {
        //        return _eventsHandlers;
        //    }
        //}

        private IMariniSerializer _serializer;
        /// <summary>
        /// Gets the actual MariniImpiantoDataTree
        /// </summary>
        public IMariniSerializer Serializer
        {
            get
            {
                return _serializer;
            }
        }

        //Nei costruttori faccio Dependency Injection / Inversion of Control, ovvero sposto il codice su oggetti esterni.
        public MariniImpiantoDataManager(MariniBaseObject dataTree, IMariniSerializer serializer, List <IMariniEventsHandlers> eventsHandlersList)
        {
            _Initialize(dataTree, serializer, eventsHandlersList);   
        }

        public MariniImpiantoDataManager(string filename, IMariniSerializer serializer, List<IMariniEventsHandlers> eventsHandlersList)
        {
            // TODO: Una qualche validazione del filename? Magari con metodo apposito, o oggetto validatore esterno
            // TODO: una qualche validazione dell'XML? Magari con metodo apposito o con oggetto validatore esterno (vedi XSD)
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);
            XmlNode root = doc.SelectSingleNode("*");
            MariniBaseObject dataTree = (MariniBaseObject)MariniObjectCreator.CreateMariniObject(root);

            _Initialize(dataTree, serializer, eventsHandlersList);
        }
        
        /// <summary>
        /// Initialize the MariniImpiantoDataManager. Sets the mariniImpiantoDataTree and populate the Dictionaries
        /// </summary>
        /// <param name="mariniImpiantoDataTree"></param>
        private void _Initialize(MariniBaseObject dataTree, IMariniSerializer serializer, List<IMariniEventsHandlers> eventsHandlersList)
        {
            // TODO Faccio cosi' o uso un setter???
            this._dataTree = dataTree;
            this._serializer = serializer;
            this._eventsHandlersList = eventsHandlersList;
            _InitializeDictionaries();
            foreach (IMariniEventsHandlers mariniEventsHandler in this._eventsHandlersList)
            {
                _SubscribeEvents(mariniEventsHandler);
            }
        }

        /// <summary>
        /// Initialize and populate the dictionaries of MariniImpiantoDataManager.
        /// </summary>
        /// <param name="mariniImpiantoDataTree"></param>
        private void _InitializeDictionaries()
        {
            // TODO: sviluppare un metodo GetChildDictionaryByParam che chieda in ingresso anche il parametro
            // di MariniGenericObject da usare come key del dizionario, per non fare 2 funzioni che fanno la stessa
            // cosa. Uso Reflection?
            _pathObjectsDictionary = new Dictionary<string, MariniGenericObject>();
            _populatePathObjectsDictionary(this._dataTree);
        }

        // Qua cerco di agganciare un handler caricato dal file XML e presente in _mariniImpiantoEventHandlers
        // Se l'handler nell'XML trova una corrispondenza nei metodi della classe MariniImpiantoEventHandler
        // allora viene usato allo scatenarsi dell'evento
        /// <summary>
        /// Try to subscribe handlers in _eventsHandlers to the PropertyChanged events raised by some properties
        /// of the _dataTree
        /// </summary>
        /// <param name="mariniImpiantoDataTree"></param>
        private void _SubscribeEvents(IMariniEventsHandlers mariniEventsHandler)
        {
            Console.WriteLine("\n\n\n========== INIZIO Sottoscrizione Eventi {0} ==========", mariniEventsHandler.ToString());
            MethodInfo[] methods = mariniEventsHandler.GetType().GetMethods();
            foreach (MariniGenericObject mgo in this._pathObjectsDictionary.Values)
            {
                Console.WriteLine("\n\n\t-----> Oggetto Marini {0} chiede la sottoscrizione dell'handler {1}", mgo.path, mgo.handler);
                if (mgo.handler == "NO_HANDLER")
                {
                    Console.WriteLine("\n\tNessun handler richiesto");
                    //Logger.DebugFormat("{0} - Nessuna richiesta di handler", mgo.id);
                }
                else
                {
                    int methodCounter = 0;
                    bool bHandlerFound = false;
                    foreach (MethodInfo handlerInfo in methods)
                    {
                        methodCounter++;
                        //Console.WriteLine("\n\t\tTentativo {0}: metodo {1}", methodCounter, handlerInfo.Name);
                        Type t_mgo = mgo.GetType();
                        //foreach (var prop in t_mgo.GetProperties())
                        //{
                        //    Console.WriteLine("{0}={1}", prop.Name, prop.GetValue(mgo, null));
                        //}
                        EventInfo ei = t_mgo.GetEvent("PropertyChanged");
                        //Console.WriteLine("\t\tEvent name      : {0}", ei.Name);
                        MethodInfo mi = null;
                        //Console.WriteLine("\t\thandlerInfo.Name: {0}", handlerInfo.Name);
                        //Console.WriteLine("\t\tmgo.handler     : {0}", mgo.handler);
                        //Logger.DebugFormat("{0} handler cercato: {1} - handler trovato: {2}", mgo.id, handlerInfo.Name, mgo.handler);
                        if (handlerInfo.Name == mgo.handler)
                        {
                            Console.WriteLine("\t\tTROVATA CORRISPONDENZA - handlerInfo.Name {0} == mgo.handler {1}", handlerInfo.Name, mgo.handler);
                            bHandlerFound = true;
                            //Logger.DebugFormat("{0} - Trovato handler {1}", mgo.id, handlerInfo.Name);
                            //MethodInfo mi = _mariniImpiantoEventHandlers.GetType().GetMethod("MyHandler");
                            mi = mariniEventsHandler.GetType().GetMethod(handlerInfo.Name);
                            //Console.WriteLine("{0}", mi.Name);

                            //Delegate dg = Delegate.CreateDelegate(typeof(PropertyChangedEventHandler), value, mi);
                            Delegate dg = Delegate.CreateDelegate(ei.EventHandlerType, mariniEventsHandler, mi);

                            ei.AddEventHandler(mgo, dg);
                        }
                    }
                    if (!bHandlerFound)
                    {
                        //Logger.DebugFormat("{0} - Nessun handler trovato", mgo.id);
                        Console.WriteLine("\n\tNessun handler trovato");
                    }
                }
                // Qua cerco di agganciare l'handler per tutte le proprieta' con plctag associato, in modo da fare il bind
                // tra il valore del plctag e la proprieta' dell'oggetto che contiene il plctag.
                if (mgo.GetType() == typeof(MariniProperty))
                {
                    //mgo.PropertyChanged+=_mariniImpiantoEventHandlers.MyPropertyHandler;
                    //(mgo as MariniProperty).MariniPropertyChanged += _mariniImpiantoEventHandlers.MariniPropertyHandler;
                }
            }
        }

        /// <summary>
        /// Retrieve a dictionary of MariniGenericObject children with Path key
        /// </summary>
        /// <returns>The children dictionary</returns>
        public void _populatePathObjectsDictionary(MariniGenericObject mgo)
        {
            _pathObjectsDictionary.Add(mgo.path, mgo);
            if (mgo.ChildList.Count > 0)
            {
                foreach (MariniGenericObject child in mgo.ChildList)
                {
                    _populatePathObjectsDictionary(child);
                }
            }
            return;
        }

        /// <summary>
        /// Gets a specific object of MariniImpianto.
        /// </summary>
        /// <param name="path">Path of the object</param>
        /// <returns>The <c>MariniGenericObject</c> with the given Path, <c>null</c> if not found.</returns>
        public MariniGenericObject GetObjectByPath(string path)
        {
            MariniGenericObject mgo = null;
            if (this.PathObjectsDictionary.TryGetValue(path, out mgo))
            {
                return mgo;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Serialize a specific object of MariniImpianto.
        /// </summary>
        /// <param name="path">the path of the object.</param>
        /// <returns>A string that contains the serialized object</returns>
        public string SerializeObject(string path)
        {
            return Serializer.Serialize(GetObjectByPath(path));
        }
    }
}
