using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Optiviera.Models;
using Optiviera.Data;
using Microsoft.EntityFrameworkCore;
using Optiviera.Models.Enums;

namespace Optiviera.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var reportData = new ReportsViewModel();

            // Toplam iş emirleri
            reportData.TotalTickets = await _context.Tickets.CountAsync();

            // Durum bazında iş emirleri
            reportData.OpenTickets = await _context.Tickets.CountAsync(t => t.Status == TicketStatus.Open);
            reportData.InProgressTickets = await _context.Tickets.CountAsync(t => t.Status == TicketStatus.InProgress);
            reportData.CompletedTickets = await _context.Tickets.CountAsync(t => t.Status == TicketStatus.Completed);
            reportData.ClosedTickets = await _context.Tickets.CountAsync(t => t.Status == TicketStatus.Closed);

            // Son 30 günlük iş emirleri - SQLite uyumlu
            var thirtyDaysAgo = DateTime.Now.AddDays(-30);
            var allTickets = await _context.Tickets.ToListAsync();
            reportData.RecentTickets = allTickets.Count(t => t.Created >= thirtyDaysAgo);

            ViewBag.ReportData = reportData;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}