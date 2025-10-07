using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Optiviera.Data;
using Optiviera.Models;
using Optiviera.Models.Enums;

namespace Optiviera.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var reportData = new ReportsViewModel();

            // Toplam iş emirleri
            reportData.TotalTickets = await _context.Tickets.CountAsync();

            // Durum bazında iş emirleri - gerçek veriler
            reportData.OpenTickets = await _context.Tickets.CountAsync(t => t.Status == TicketStatus.Open);
            reportData.InProgressTickets = await _context.Tickets.CountAsync(t => t.Status == TicketStatus.InProgress);
            reportData.CompletedTickets = await _context.Tickets.CountAsync(t => t.Status == TicketStatus.Completed);
            reportData.ClosedTickets = await _context.Tickets.CountAsync(t => t.Status == TicketStatus.Closed);

            // Öncelik bazında iş emirleri
            reportData.HighPriorityTickets = await _context.Tickets.CountAsync(t => t.Priority.Name == "Yüksek");
            reportData.MediumPriorityTickets = await _context.Tickets.CountAsync(t => t.Priority.Name == "Orta");
            reportData.LowPriorityTickets = await _context.Tickets.CountAsync(t => t.Priority.Name == "Düşük");
            reportData.UrgentTickets = await _context.Tickets.CountAsync(t => t.Priority.Name == "Acil");

            // Kullanıcı istatistikleri
            reportData.TotalUsers = await _context.Users.CountAsync();
            reportData.AdminUsers = await _context.UserRoles.CountAsync(ur => ur.RoleId == _context.Roles.FirstOrDefault(r => r.Name == "Admin").Id);
            reportData.ManagerUsers = await _context.UserRoles.CountAsync(ur => ur.RoleId == _context.Roles.FirstOrDefault(r => r.Name == "Manager").Id);
            reportData.EmployeeUsers = await _context.UserRoles.CountAsync(ur => ur.RoleId == _context.Roles.FirstOrDefault(r => r.Name == "Employee").Id);

            // Son 30 günlük iş emirleri - SQLite uyumlu
            var thirtyDaysAgo = DateTime.Now.AddDays(-30);
            var allTickets = await _context.Tickets.ToListAsync();
            reportData.RecentTickets = allTickets.Count(t => t.Created >= thirtyDaysAgo);

            // En aktif kullanıcılar (en çok iş emri alan)
            reportData.TopTechnicians = await _context.Tickets
                .Where(t => t.TechnicianId != null)
                .GroupBy(t => t.TechnicianId)
                .Select(g => new TechnicianStats
                {
                    TechnicianName = g.First().Technician.FirstName + " " + g.First().Technician.LastName,
                    TicketCount = g.Count()
                })
                .OrderByDescending(t => t.TicketCount)
                .Take(5)
                .ToListAsync();

            return View(reportData);
        }
    }

    public class ReportsViewModel
    {
        public int TotalTickets { get; set; }
        public int OpenTickets { get; set; }
        public int InProgressTickets { get; set; }
        public int CompletedTickets { get; set; }
        public int ClosedTickets { get; set; }
        public int HighPriorityTickets { get; set; }
        public int MediumPriorityTickets { get; set; }
        public int LowPriorityTickets { get; set; }
        public int UrgentTickets { get; set; }
        public int TotalUsers { get; set; }
        public int AdminUsers { get; set; }
        public int ManagerUsers { get; set; }
        public int EmployeeUsers { get; set; }
        public int RecentTickets { get; set; }
        public List<TechnicianStats> TopTechnicians { get; set; } = new List<TechnicianStats>();
    }

    public class TechnicianStats
    {
        public string TechnicianName { get; set; }
        public int TicketCount { get; set; }
    }
}
