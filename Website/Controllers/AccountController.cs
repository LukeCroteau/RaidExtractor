using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Website.Models;
using Website.Repositories;

namespace Website.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountDumpRepository _accountDumpRepository;

        public AccountController(IAccountDumpRepository accountDumpRepository)
        {
            _accountDumpRepository = accountDumpRepository;
        }

        [HttpPost]
        public string Upload(AccountDump accountDump)
        {
            return _accountDumpRepository.Store(accountDump);
        }

        [HttpGet]
        public AccountDump Get(string id)
        {
            return _accountDumpRepository.Get(id);
        }
    }
}
