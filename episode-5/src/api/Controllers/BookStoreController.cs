using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BookStoreApp.WebApi.Models;

namespace BookStoreApp.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookStoreController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var bookStore = new List<BookStore>{
                new BookStore {
                    Name = "Waterstones",
                    AddressLine1 = "The Dolphin & Anchor",
                    AddressLine2 = "West Street",
                    City = "Chichester",
                    PostCode = "PO19 1QD",
                    TelephoneNumber = "01234 773030"
                }
            };
            
            return await Task.Run(() => new JsonResult(bookStore));
        }
    }
}