using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Website.Models;

namespace Website.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        [HttpPost]
        public string Upload(AccountDump accountDump)
        {
            return string.Empty;
        }

        [HttpGet]
        public AccountDump Get(string id)
        {
            return new AccountDump();
        }
    }
}
