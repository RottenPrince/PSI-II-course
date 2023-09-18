using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecondApiController : ControllerBase
    {
        private static Dictionary<int, string> _db = new Dictionary<int, string>();

        [HttpGet]
        public string Get()
        {
            return "foo";
        }

        [HttpGet("{id}")]
        public APIResultWithData<string> Get(int id)
        {
            if (_db.TryGetValue(id, out var value)) return APIResultWithData<string>.CreateSuccess(value);
            else return APIResultWithData<string>.CreateFailure("ID not found");
        }

        [HttpPost]
        public APIResult Post(NewString data)
        {
            if (_db.TryAdd(data.id, data.value)) return APIResult.CreateSuccess();
            else return APIResult.CreateFailure("ID already taken");
        }

        [HttpPut]
        public APIResult Put(NewString data)
        {
            if (_db.ContainsKey(data.id)) { _db[data.id] = data.value; return APIResult.CreateSuccess(); }
            else { return APIResult.CreateFailure("ID not found"); }
        }

        [HttpDelete("{id}")]
        public APIResult Delete(int id)
        {
            if (_db.Remove(id)) return APIResult.CreateSuccess();
            else return APIResult.CreateFailure("ID not found");
        }

        [HttpGet("GetAll")]
        public Dictionary<int, string> GetAll()
        {
            return _db;
        }
    }
}
