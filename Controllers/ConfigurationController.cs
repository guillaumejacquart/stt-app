using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using api.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using api.ViewModels;

namespace api.Controllers
{
    [Authorize]
    [Route("api/configuration")]
    public class ConfigurationController : Controller
    {
        private ApiContext ApiContext;
        private const string API_AI_URL = "https://api.api.ai/v1";

        public ConfigurationController(ApiContext context)
        {
            ApiContext = context;
        }

        // GET api/configuration
        [HttpGet]
        public ActionResult Get()
        {
            var confs = ApiContext.Configurations.Select(c => new
            {
                Id = c.ConfigurationId,
                Name = c.Name
            });
            return Ok(confs);
        }

        // GET api/configuration/5
        [HttpGet("{id}")]
        public async Task<Configuration> Get(int id)
        {
            var conf = await ApiContext.Configurations.SingleOrDefaultAsync(c => c.ConfigurationId == id);
            return conf;
        }

        // POST api/configuration
        [HttpPost]
        public async Task Post([FromBody]ApiAiConfiguration conf)
        {
            var action = Guid.NewGuid().ToString();
            var user = await ApiContext.Users.SingleOrDefaultAsync(u => 
                u.Email == AccountController.GetCurrentUsername(HttpContext));

            var jsonInString = JsonConvert.SerializeObject(new
            {
                name = conf.Name,
                auto = true,
                templates = new string[] { conf.UserSay },
                userSays = new[]
                {
                    new {
                        data = new[]
                        {
                            new {
                                text = conf.UserSay
                            }
                        },
                        isTemplate = false,
                        count = 0
                    }
                },
                responses = new[]
                {
                    new{
                        resetContexts = false,
                        action = action
                    }
                },
                priority = 500000
            });
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {user.ApiAiDeveloperToken}");

            var response = await client.PostAsync(
                requestUri: $"{API_AI_URL}/intents",
                content: new StringContent(jsonInString, Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                var newConf = new Configuration()
                {
                    Action = action,
                    Name = conf.Name
                };
                user.Configurations.Add(newConf);
                ApiContext.Configurations.Add(newConf);

                await ApiContext.SaveChangesAsync();
            }
        }

        // PUT api/configuration/5
        [HttpPut("{id}")]
        public async Task Put(int id, [FromBody]Configuration conf)
        {
            var confDb = await ApiContext.Configurations.SingleOrDefaultAsync(c => c.ConfigurationId == id);
            confDb.Name = conf.Name;
            await ApiContext.SaveChangesAsync();
        }

        // DELETE api/configuration/5
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            var confDb = await ApiContext.Configurations.SingleOrDefaultAsync(c => c.ConfigurationId == id);
            ApiContext.Configurations.Remove(confDb);
            await ApiContext.SaveChangesAsync();
        }
    }
}
