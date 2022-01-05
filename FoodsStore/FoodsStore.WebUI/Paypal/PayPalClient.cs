using System;
using PayPalCheckoutSdk.Core;
using PayPalHttp;

using System.IO;
using System.Text;
using System.Runtime.Serialization.Json;

namespace FoodsStore.WebUI.Paypal
{
    public class PayPalClient
    {
        /**
           Setting up PayPal environment with credentials with sandbox cerdentails. 
           For Live, this should be LiveEnvironment Instance. 
        */
        public static PayPalEnvironment environment()
        {
            return new SandboxEnvironment("Aa7AndcgAFRz297Bs1DHn0KW-w8eaIqen3hxRGwDZL3hmGHfdsMtL8GipWlpfu7svoBA8AgaqObgHlM8", "ECQRpY9NeXK4apOoM6lTXjVy1EoLzWHFqkmw7fQCG2j6VTlk8g9RQxb1RGLI0cYIIUQjZVIgVCZdZ66u");
        }

        /**
            Returns PayPalHttpClient instance which can be used to invoke PayPal API's.
         */
        public static HttpClient client()
        {
            return new PayPalHttpClient(environment());
        }

        public static HttpClient client(string refreshToken)
        {
            return new PayPalHttpClient(environment(), refreshToken);
        }

        /**
            This method can be used to Serialize Object to JSON string.
        */
        public static String ObjectToJSONString(Object serializableObject)
        {
            MemoryStream memoryStream = new MemoryStream();
            var writer = JsonReaderWriterFactory.CreateJsonWriter(
                        memoryStream, Encoding.UTF8, true, true, "  ");
            DataContractJsonSerializer ser = new DataContractJsonSerializer(serializableObject.GetType(), new DataContractJsonSerializerSettings { UseSimpleDictionaryFormat = true });
            ser.WriteObject(writer, serializableObject);
            memoryStream.Position = 0;
            StreamReader sr = new StreamReader(memoryStream);
            return sr.ReadToEnd();
        }
    }
}