using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using connections.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace connections.Controllers
{
    public class ConnectionController : Controller
    {
        private Context _context { get; set; }

        public ConnectionController (Context c)
        {
            _context = c;
        }

        // TODO - move methods here

    }
}