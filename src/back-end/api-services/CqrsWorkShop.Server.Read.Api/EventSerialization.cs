using System;
using System.Collections.Generic;
using System.Text;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace Hse.CqrsWorkShop.Api
{
    public class EventSerialization
    {
        public static string EventClrTypeHeader = "EventClrTypeName";

        public static object DeserializeEvent(RecordedEvent originalEvent)
        {
            if (originalEvent.Metadata != null)
            {
                try
                {
                    var metadata = DeserializeObject<Dictionary<string, string>>(originalEvent.Metadata);
                    if (metadata != null && metadata.ContainsKey(EventClrTypeHeader))
                    {
                        var eventData = DeserializeObject(originalEvent.Data, metadata[EventClrTypeHeader]);
                        return eventData;
                    }
                }
                catch (Exception)
                {
                    
                }
            }
            return null;
        }

        private static T DeserializeObject<T>(byte[] data)
        {
            return (T)(DeserializeObject(data, typeof(T).AssemblyQualifiedName));
        }

        private static object DeserializeObject(byte[] data, string typeName)
        {
            var jsonString = Encoding.UTF8.GetString(data);
            try
            {
                return JsonConvert.DeserializeObject(jsonString, Type.GetType(typeName));
            }
            catch (JsonReaderException)
            {
                return null;
            }
        }
    }
}