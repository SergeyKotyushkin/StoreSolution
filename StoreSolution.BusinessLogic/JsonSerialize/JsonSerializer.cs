using System.Web.Script.Serialization;
using StoreSolution.BusinessLogic.JsonSerialize.Contracts;

namespace StoreSolution.BusinessLogic.JsonSerialize
{
    public class JsonSerializer : IJsonSerializer
    {
        public string Serialize(object inputObject)
        {
            var jsonSerialiser = new JavaScriptSerializer();
            return jsonSerialiser.Serialize(inputObject);
        }

        public T Deserialize<T>(string inputString)
        {
            var jss = new JavaScriptSerializer();
            return jss.Deserialize<T>(inputString);
        }
    }
}