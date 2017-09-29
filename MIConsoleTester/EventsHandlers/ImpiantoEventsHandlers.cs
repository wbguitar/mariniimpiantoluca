using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Libreria Marini!!!
using MariniImpiantoDataModel;
// Libreria per mettere a disposizione le strutture per PropertyChanged event.
using System.ComponentModel;
// Libreria per il Log. Magari provare a toglierla dall'handler e spostare tutto su main con throw try catch???
using log4net;
// Libreria per usare l'oggetto MethodBase in Log4Net
using System.Reflection;

namespace MIConsoleTester.EventsHandlers
{
    public class ImpiantoEventsHandlers : IMariniEventsHandlers
    {
        protected static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void ImpiantoEventHandler(object sender, PropertyChangedEventArgs e)
        {
            
            Console.WriteLine("Sono in ImpiantoEventsHandlers->ImpiantoEventsHandlers, il sender e' {0} e la proprieta' e' : {1}!!!!", (sender as MariniGenericObject).id, e.PropertyName);
            
            //methodToBeCalledWhenPropertyIsSet();
        }

    }
}
