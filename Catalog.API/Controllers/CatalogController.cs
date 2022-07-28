using Catalog.API.Infrastructure;
using Catalog.API.Infrastructure.Repository;
using Catalog.API.Model;
using Catalog.API.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Catalog.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CatalogController : ControllerBase
    {

        private readonly IUnitOfWork _uow;
        public CatalogController(IUnitOfWork uow)
        {
            _uow = uow;
            
        }

        [HttpGet]
        [Authorize]
        [Route("[action]/{pageSize:int}/{pageIndex:int}")]
        [ProducesResponseType(typeof(PaginatedItemsViewModel<Product>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Products([FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0)
        {
            var totalItem = await _uow.Repository<Product>().LongCountAsync();
            var itemsOnPage = await _uow.Repository<Product>().GetAll().OrderBy(c => c.Name).Skip(pageSize * pageIndex).Take(pageSize).ToListAsync();
            var model = new PaginatedItemsViewModel<Product>(pageIndex, pageSize, totalItem, itemsOnPage);
            return Ok(model);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> AddBrand()
        {
            var item = new Brand
            {
                
                BrandName = "Samsung",
                Description = "Samsung android phone"
            };

            await _uow.Repository<Brand>().Add(item);
            await _uow.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> AddProduct()
        {
            var item = new Product
            {
                Name = "Samsung A72",
                Description = "Samsung A72 android phone",
                Price = 20000,
                BrandId = 1
            };

            await _uow.Repository<Product>().Add(item);
            await _uow.SaveChangesAsync();
            return Ok();
        }
     
    }
}