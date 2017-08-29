using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using AutoMapper;

using DSMS.Web.Models;
using DSMS.Data;
using DSMS.Data.Models;

namespace DSMS.Web.Controllers
{
    public class HomeController : Controller
    {
        private DSMSDBContext context;
        private IMapper mapper;

        public HomeController(DSMSDBContext Context, IMapper Mapper)
        {
            this.context = Context;
            this.mapper = Mapper;
        }
        public async Task<IActionResult> Index()
        {
            var sm = await context.SystemMessages.Where(m => (m.StartDate == null || m.StartDate < DateTime.Now) && (m.EndDate == null || m.EndDate > DateTime.Now)).ToListAsync();
            var vis = await context.Visitors.Include(v => v.ResponsibleParty).ToListAsync().ConvertEach<Visitor, VisitorViewModel>();
            var tuple = new Tuple<List<SystemMessage>, List<VisitorViewModel>>(sm, vis);
            //ViewData["Message"] = "XXX";
            return View(tuple);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
