using System.Web.Http;

namespace OsuLoader.Api;

public class GameController : ApiController
{
    public IHttpActionResult Get()
    {
        return Ok(OsuManager.GetScoreData());
    }
}