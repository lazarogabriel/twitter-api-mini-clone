using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using twitter.api.domain.Models;

namespace twitter.api.web.Controllers
{
    [ApiController]
    [Route("Tweets")]
    public class TweetsController : ControllerBase
    {
        #region Fields

        private readonly IMapper _mapper;

        #endregion

        #region Constructor

        public TweetsController(IMapper mapper)
        {
            _mapper = mapper;
        }

        #endregion

        #region Endpoints

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var tweets = new List<Tweet>();

            var user = new User(Guid.NewGuid(), "lazavecchi");

            tweets.AddRange(new Tweet(content: "coso", author: user), new Tweet(content: "aaaaaaaaaaa uwu", author: user));

            return Ok(tweets);
        }

        #endregion
    }
}
