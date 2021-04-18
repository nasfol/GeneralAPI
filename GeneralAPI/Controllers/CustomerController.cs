using GeneralAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace GeneralAPI.Controllers
{
    public class CustomerController : ApiController
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();

        [HttpPost]
        public async Task<IHttpActionResult> CreateCustomer([FromBody] Customer model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if(model is null)
            {
                return BadRequest("You cannot pass in a null model");

            }
            _context.Customers.Add(model);
            await _context.SaveChangesAsync();
            return Ok("Customer Added Successfully");

        }
        [HttpGet]
        public async Task<IHttpActionResult> GetAllCustomers()
        {
            List<Customer> customers = await _context.Customers.ToListAsync();
            return Ok(customers);
        }
        [HttpGet]
        public async Task<IHttpActionResult> GetCustomerById([FromUri] int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
                return Ok(customer);
            return NotFound();
        }
        [HttpPut]
        public async Task<IHttpActionResult> UpdateCustomerById([FromBody] Customer updatedCustomer,[FromUri] int id)
        {
            if (id != updatedCustomer?.Id)
            {
                return BadRequest("Id do not match");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var customer = await _context.Customers.FindAsync(id);
            if(customer is null)
            {
                return NotFound();
            }
            customer.FirstName = updatedCustomer.FirstName;
            customer.LastName = updatedCustomer.LastName;
            await _context.SaveChangesAsync();
            return Ok("Customer Updated succesfully");
            
        }
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteCustomerById([FromUri] int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer is null)
                return BadRequest($"There is no customer with this {id} ");
            _context.Customers.Remove(customer);
            bool isdeleted = (await _context.SaveChangesAsync() == 1);
            if (isdeleted)
                return Ok("Customer deleted successfully");
            return InternalServerError();

        }
    }

}
