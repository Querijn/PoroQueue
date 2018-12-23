using Newtonsoft.Json;
using System.Threading.Tasks;

namespace PoroQueue
{
    public static class JSONRequest
    {
        public static async Task<T> Get<T>(string URL)
        {
            return JsonConvert.DeserializeObject<T>(await Request.Get(URL));
        }
    }
}
