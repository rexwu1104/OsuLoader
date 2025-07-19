using System.Collections.Generic;
using System.Web.Http;

namespace OsuLoader.Api;

public class StateController : ApiController
{
    public IHttpActionResult Get()
    {
        return Ok(new Dictionary<string, Dictionary<string, string>>
        {
            { "random", new Dictionary<string, string>() },
            { "play", new Dictionary<string, string>() },
            { "move", new Dictionary<string, string>
            {
                { "x", "(0, 512)" },
                { "y", "(0, 384)" }
            } },
            { "press", new Dictionary<string, string>
            {
                { "binding", "[OsuLeft, OsuRight]" }
            } },
            { "release", new Dictionary<string, string>
            {
                { "binding", "[OsuLeft, OsuRight]" }
            } },
            { "restart", new Dictionary<string, string>() }
        });
    }
}