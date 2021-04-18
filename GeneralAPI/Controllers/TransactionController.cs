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
    public class TransactionController : ApiController
    {
        ApplicationDbContext _context = new ApplicationDbContext();
        [HttpPost]
        public async Task<IHttpActionResult> CreateTransaction([FromBody] Transaction model)
        {
            if (model is null)
            {
                return BadRequest("Model cannot  be Null");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var product = _context.Products.Find(model.ProductSKU);
            if (product is null)
                return NotFound();
            if (!product.IsInStock)
            {
                return BadRequest("Stock is empty");
            }
            if (model.ItemCount > product.NumberInInventory)
            {
                return BadRequest("There is not enough nstock for this transaction");

            }
            // _context.Products.Remove(product);
            product.NumberInInventory = product.NumberInInventory - model.ItemCount;
            _context.SaveChanges();

            _context.Transactions.Add(model);
            await _context.SaveChangesAsync();
            return Ok("Transaction posted successfully");


        }
        [HttpGet]
        public async Task<IHttpActionResult> GetAllTransaction()
        {
            List<Transaction> Transaction = await _context.Transactions.ToListAsync();
            return Ok(Transaction);

        }
        [HttpGet]
        [Route("api/Transaction/CustomerId/{Id}")]
        public async Task<IHttpActionResult> GetAllTransactionByCustomerId([FromUri] int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer.Id != id)
            {
                return BadRequest("There is no customer with this id");
            }
            
            List<Transaction> Transaction = await _context.Transactions.ToListAsync();
            foreach (var T in Transaction)
            {
                if (T.CustomerId!= id)
                {
                    return NotFound();
                }
                List<Transaction> customerList = new List<Transaction>();
                customerList.Add(T);
                return Ok(customerList);

            }
            return Ok();
        }
        [HttpGet]
        // [Route("api/Transaction/{TransactionId}")]
        public async Task<IHttpActionResult> GetAllTransactionByTransactionID([FromUri] int id)
        {
            var Transaction = await _context.Transactions.FindAsync(id);
            if (Transaction is null)
            {
                return BadRequest($"There is no transaction associated with this id:{id}");
            }
            return Ok(Transaction);

        }
        [HttpPut]
        public async Task<IHttpActionResult> UpdateTransactionById([FromUri] int id, [FromBody] Transaction model)
        {
            if (id != model?.Id)
            {
                return BadRequest("Id do not match");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var product = await _context.Products.FindAsync(model.ProductSKU);
            if (!product.IsInStock)
            {
                return BadRequest("Stock is empty");
            }
            if (model.ItemCount > product.NumberInInventory)
            {
                return BadRequest("There is not enough nstock for this transaction");
            }
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction is null)
            {
                return BadRequest("Transaction not found");
            }
            product.NumberInInventory += transaction.ItemCount;
            transaction.DateOfTransaction = model.DateOfTransaction;
            transaction.ItemCount = model.ItemCount;
            transaction.ProductSKU = model.ProductSKU;

            product.NumberInInventory = product.NumberInInventory - model.ItemCount;
            await _context.SaveChangesAsync();

            return Ok("Transaction Updated successfully");
        }
        [HttpDelete]
        public async Task<IHttpActionResult> Delete([FromUri] int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction is null)
            {
                return BadRequest("Transaction not found");
            }
            var product = _context.Transactions.Where(P => P.ProductSKU == transaction.ProductSKU).Select(p => p.Product);
            foreach (var p in product)
            {
                p.NumberInInventory += transaction.ItemCount;
            }
            _context.Transactions.Remove(transaction);

            bool Isdeleted = await _context.SaveChangesAsync() == 2;
            if (Isdeleted)
            {

                return Ok("Transaction succesfully deleted");
            }
            else
            {
                return BadRequest("Transaction  cannot be deleted");
            }
                
        }
    }

}
