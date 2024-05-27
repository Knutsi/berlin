using System.Reflection;
using System.Text;
using Berlin.Library.MethodExecution;

namespace Berlin.Library.MethodListing;

public class RpcListRenderer(RpcMethodCache methodCache)
{
    public string RenderRpcList()
    {
        var template = LoadEmbeddedTemplate();
        var methods = methodCache.AllRpcs();
        var rows = new StringBuilder();


        foreach (var method in methods)
        {
            var link = $"/api/rpc/{method.ServiceName}.{method.Name}";
            rows.Append($"""
                 <tr class=border-t border-gray-200 hover:bg-gray-100>
                     <td class=px-4 py-2>{method.ServiceName}</td>
                     <td class=px-4 py-2>
                        <a href="#" onclick="callMethod('{link}')" class="text-blue-500 hover:text-blue-800">{method.Name}</a>
                        <a href="{link}">(url)</a>
                     </td>
                     <td class=px-4 py-2>-</td>
                     <td class=px-4 py-2>-</td>
                 </tr>
             """);
        }


        return template.Replace("{rows}", rows.ToString());
    }

    public string LoadEmbeddedTemplate()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = "Berlin.Library.MethodListing.Embedded.list.html";

        using (var stream = assembly.GetManifestResourceStream(resourceName))
        using (var reader = new StreamReader(stream))
        {
            var result = reader.ReadToEnd();
            return result;
        }
    }
}