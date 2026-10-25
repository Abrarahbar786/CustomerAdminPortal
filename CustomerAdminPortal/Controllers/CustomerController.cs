using CustomerAdminPortal.Data;
using CustomerAdminPortal.Models;
using CustomerAdminPortal.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CustomerAdminPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public CustomerController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // 1. Get all customers
        [HttpGet]
        public async Task<IActionResult> GetAllCustomers()
        {
            var customers = await _dbContext.Customers
                .AsNoTracking()
                .ToListAsync();

            return Ok(customers);
        }

        // 2. Get customer by ID
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            var customer = await _dbContext.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
            {
                return NotFound(new
                {
                    message = $"Customer with ID {id} was not found."
                });
            }

            return Ok(customer);
        }

        // 3. Add new customer
        [HttpPost]
        public async Task<IActionResult> AddCustomer(
            [FromBody] AddCustomerDto addCustomerDto)
        {
            var customer = new Customer
            {
                Name = addCustomerDto.Name,
                Email = addCustomerDto.Email,
                Phone = addCustomerDto.Phone,
                Salary = addCustomerDto.Salary
            };

            await _dbContext.Customers.AddAsync(customer);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetCustomerById),
                new { id = customer.Id },
                customer);
        }

        // 4. Update customer
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCustomer(
            int id,
            [FromBody] UpdateCustomerDto updateCustomerDto)
        {
            var customer = await _dbContext.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound(new
                {
                    message = $"Customer with ID {id} was not found."
                });
            }

            customer.Name = updateCustomerDto.Name;
            customer.Email = updateCustomerDto.Email;
            customer.Phone = updateCustomerDto.Phone;
            customer.Salary = updateCustomerDto.Salary;

            await _dbContext.SaveChangesAsync();

            return Ok(customer);
        }

        // 5. Delete customer
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _dbContext.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound(new
                {
                    message = $"Customer with ID {id} was not found."
                });
            }

            _dbContext.Customers.Remove(customer);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}