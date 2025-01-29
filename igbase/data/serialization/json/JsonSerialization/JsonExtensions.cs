#if NETFRAMEWORK
using System.Web.Script.Serialization;
#else
using System.Text.Json;
#endif

/*
$$$$
REMARK: Corrections had to be maded in this file in order to compile the IGLib project
for .NET 6 and higher.
See conditional compilation directives in the code with "if NETFRAMEWORK".
Additional corrections were made in the project file (PackageReference Include="System.Text.Json")...
*/

namespace JsonPrettyPrinterPlus.JsonSerialization
{
    public static class JsonExtensions
    {
        public static string ToJSON(this object graph)
        {
            return graph.ToJSON(false);
        }

        public static string ToJSON(this object graph, bool prettyPrint)
        {
#if NETFRAMEWORK
            var unprettyJson = new JavaScriptSerializer().Serialize(graph);
#else
            var unprettyJson = JsonSerializer.Serialize(graph);
#endif
            if (!prettyPrint)
                return unprettyJson;

            return unprettyJson.PrettyPrintJson();
        }

        public static T DeserializeFromJson<T>(this string json)
        {
#if NETFRAMEWORK
            return new JavaScriptSerializer().Deserialize<T>(json);
#else
            return JsonSerializer.Deserialize<T>(json);
#endif
        }

    }
}