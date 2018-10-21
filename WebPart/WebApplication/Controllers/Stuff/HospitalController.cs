using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Models.HospitalModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication.Controllers.Stuff
{
    public class HospitalController : Controller
    {
        
        protected Guid HospitalId =>
            Guid.TryParse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value, out var id) ? id : throw new ArgumentException();

        
    }
}
