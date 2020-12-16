using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Caching;
using System.Web.Http;
using System;
using System.IO;
using System.Collections;
using System.Xml.Serialization;
using System.Web.Services;
using WebApi.OutputCache.V2;
using Newtonsoft.Json;
using System.Web.Http.Results;

namespace IpPool.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        [WebMethod(Description = "Test", CacheDuration = 3)]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        static Random random = new Random(100);
        // GET api/values/5

        //[CacheOutput(ClientTimeSpan = 60, ServerTimeSpan = 60)]

        [CacheOutput(ServerTimeSpan = 6, ExcludeQueryStringFromCacheKey = true)]
        public string Get(int id)
        {            
            return JsonConvert.SerializeObject(random.Next(300).ToString());
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
