using System.Text;
using Berlin.Library.MethodExecution;

namespace Berlin.Library.MethodListing;

public class RpcListRenderer(RpcMethodCache methodCache)
{
   public string RenderRpcList()
{
    var methods = methodCache.AllRpcs();
    var rows = new StringBuilder();

    foreach (var method in methods)
    {
        var link = $"/api/rpc/{method.ServiceName}.{method.Name}";
        rows.Append($"""
            <tr class=\"border-t border-gray-200 hover:bg-gray-100\">
                <td class=\"px-4 py-2\">{method.ServiceName}</td>
                <td class=\"px-4 py-2\"><a href=\"#\" onclick=\"callMethod('{link}')\" class=\"text-blue-500 hover:text-blue-800\">{method.Name}</a></td>
                <td class=\"px-4 py-2\">-</td>
                <td class=\"px-4 py-2\">-</td>
            </tr>
        """);
    }

    var script = """
        async function callMethod(url) {
            const response = await fetch(url, { method: 'POST' });
            const data = await response.json();
            document.getElementById('result').textContent = JSON.stringify(data, null, 2);
        }
    """;

    var template = $$"""
        <!DOCTYPE html>
        <html>
        <head>
            <title>RPC Methods</title>
            <link href=\"https://fonts.googleapis.com/css2?family=Roboto:wght@400;700&display=swap\" rel=\"stylesheet\">
            <link href=\"https://cdn.jsdelivr.net/npm/tailwindcss@2.2.16/dist/tailwind.min.css\" rel=\"stylesheet\">
            <style>
                body {font-family: 'Roboto', sans-serif;}
            </style>
            <script>
                {script}
            </script>
        </head>
        <body class=\"p-6 bg-gray-50\">
            <table class=\"min-w-full divide-y divide-gray-200 shadow-sm rounded-lg overflow-hidden\">
                <thead class=\"bg-gray-200\">
                    <tr>
                        <th class=\"px-4 py-2\">ServiceName</th>
                        <th class=\"px-4 py-2\">Method</th>
                        <th class=\"px-4 py-2\">Params</th>
                        <th class=\"px-4 py-2\">Returns</th>
                    </tr>
                </thead>
                <tbody class=\"bg-white divide-y divide-gray-200\">
                    {{rows.ToString()}}
                </tbody>
            </table>
            <pre id=\"result\" class=\"p-6 mt-6 bg-white rounded shadow-sm\"></pre>
        </body>
        </html>
    """;

    return template;
}
}