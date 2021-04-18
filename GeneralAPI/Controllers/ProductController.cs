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
    public class ProductController : ApiController
    {
       private readonly  ApplicationDbContext _context = new ApplicationDbContext();
        [HttpPost]
        public async Task<IHttpActionResult> CreateProduct([FromBody] Product model)
        {
            if(model is null)
            {
                return BadRequest("Model cannot be Null");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _context.Products.Add(model);
            await _context.SaveChangesAsync();
            return Ok("Product addeded successfully");
        }
        [HttpGet]
        public async Task<IHttpActionResult> GetAllProduct()
        {
            List<Product> products = await _context.Products.ToListAsync();
            return Ok(products);

        }
        [HttpGet]
        public async Task<IHttpActionResult> GetProductById([FromUri] string id)
        {
            var Product = await _context.Products.FindAsync(id);
            if (Product is null)
            {
                return BadRequest("The target Product do not match the id ");
            }
            return Ok(Product);

        }
        [HttpPut]
        public async  Task<IHttpActionResult> UpdateProduct([FromBody] Product model, [FromUri] string id)
        {
            if (id.ToUpper() != model?.SKU.ToUpper())
            {
                return BadRequest("Id do  not  match");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var product = await _context.Products.FindAsync(id);
            if(product is null)
            {
                return NotFound();

            }
            product.Cost = model.Cost;
            product.Name = model.Name;
            product.NumberInInventory = model.NumberInInventory;
            //should both old product and the updated product maintain the  same SkU.
            await _context.SaveChangesAsync();
            return Ok("Product updated succesfully");
            
        }
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteProduct([FromUri] string id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product is null)
                return NotFound();
            _context.Products.Remove(product);
            bool Isdeleted = await _context.SaveChangesAsync() == 1;
            if (Isdeleted)
                return Ok("Product  successfully deleted");
            return InternalServerError();
        }
    }
}
