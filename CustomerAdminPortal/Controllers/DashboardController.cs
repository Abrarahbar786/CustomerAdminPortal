using Microsoft.AspNetCore.Authorization;
using CustomerAdminPortal.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CustomerAdminPortal.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public DashboardController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var customers = await _dbContext.Customers
                .AsNoTracking()
                .OrderByDescending(c => c.Id)
                .Take(5)
                .ToListAsync();

            ViewBag.TotalCustomers =
                await _dbContext.Customers.CountAsync();

            ViewBag.TotalSalary =
                await _dbContext.Customers.SumAsync(c => (decimal?)c.Salary) ?? 0m;

            return View(customers);
        }
    }
}
