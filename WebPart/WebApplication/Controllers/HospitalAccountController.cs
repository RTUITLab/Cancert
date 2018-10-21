using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PublicAPI.Responses;
using WebApplication.DataBase;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    public class HospitalAccountController : Controller
    {
        private readonly DataBaseContext dbContext;
        private readonly IConfiguration configuration;
        private readonly IMapper mapper;

        public HospitalAccountController(
            DataBaseContext dbContext, 
            IConfiguration configuration,
            IMapper mapper)
        {
            this.dbContext = dbContext;
            this.configuration = configuration;
            this.mapper = mapper;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody]string accessToken)
        {
            var targetSubscription = await dbContext
                .Subscriptions
                .Include(s => s.Hospital)
                .Where(s => s.AccessToken == accessToken)
                .SingleOrDefaultAsync();
            if (targetSubscription == null)
                return NotFound();

            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                    notBefore: now,
                    claims: new Claim[] { new Claim (ClaimTypes.NameIdentifier, targetSubscription.Hospital.Id.ToString())},
                    expires: now.Add(TimeSpan.FromDays(30)),
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration.GetValue<string>("JWTKey"))), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return Json(new
            {
                jwt = encodedJwt,
                me = mapper.Map<HospitalView>(targetSubscription.Hospital)
            });
        }
    }
}
