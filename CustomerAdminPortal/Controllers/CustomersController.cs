
using CustomerAdminPortal.Data;
using CustomerAdminPortal.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace CustomerAdminPortal.Controllers
{
    [Authorize]
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public CustomersController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Customer list, search and pagination
        public async Task<IActionResult> Index(string? search, int page = 1)
        {
            const int pageSize = 5;
            page = Math.Max(1, page);

            var query = _dbContext.Customers.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c =>
                    c.Name.Contains(search) ||
                    c.Email.Contains(search));
            }

            var totalCustomers = await query.CountAsync();

            var customers = await query
                .OrderBy(c => c.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Search = search;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages =
                (int)Math.Ceiling(totalCustomers / (double)pageSize);
            ViewBag.TotalCustomers = totalCustomers;

            return View(customers);
        }

        // Add customer page
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Name,Email,Phone,Salary")] Customer customer)
        {
            if (!ModelState.IsValid)
                return View(customer);

            _dbContext.Customers.Add(customer);
            await _dbContext.SaveChangesAsync();

            TempData["Success"] = "Customer added successfully!";
            return RedirectToAction(nameof(Index));
        }

        // Edit customer page
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var customer = await _dbContext.Customers.FindAsync(id);

            if (customer == null)
                return NotFound();

            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,Name,Email,Phone,Salary")] Customer input)
        {
            if (id != input.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(input);

            var customer = await _dbContext.Customers.FindAsync(id);

            if (customer == null)
                return NotFound();

            customer.Name = input.Name;
            customer.Email = input.Email;
            customer.Phone = input.Phone;
            customer.Salary = input.Salary;

            await _dbContext.SaveChangesAsync();

            TempData["Success"] = "Customer updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        // Delete confirmation page
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var customer = await _dbContext.Customers.FindAsync(id);

            if (customer == null)
                return NotFound();

            return View(customer);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _dbContext.Customers.FindAsync(id);

            if (customer == null)
                return NotFound();

            _dbContext.Customers.Remove(customer);
            await _dbContext.SaveChangesAsync();

            TempData["Success"] = "Customer deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
