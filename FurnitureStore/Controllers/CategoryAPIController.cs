using FurnitureStore.Models;
using System.Linq;
using System.Web.Http;

namespace FurnitureStore.Controllers
{
    public class CategoryAPIController : ApiController
    {
        private FurnitureDB context = new FurnitureDB();
        [HttpGet]
        public IHttpActionResult GetByCategoryId(int id)
        {

            var products = context.Products.Where(p => p.category_id == id).ToList();
            return Ok(products);

        }

    }
}
