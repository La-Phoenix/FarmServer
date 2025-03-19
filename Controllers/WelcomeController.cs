using Microsoft.AspNetCore.Mvc;

namespace FarmServer.Controllers
{
    public class WelcomeController : Controller
    {
        

        [HttpGet]
        public IActionResult Welcome()
        {
            string postmanLink = "https://red-star-5463.postman.co/workspace/Personal~0dd7abf0-ba43-4be5-b08b-f4ccc6d6bbeb/collection/40959837-e67c1cea-03a7-4dc4-99eb-1c5c144914cb?action=share&creator=40959837";
            return View("Welcome", postmanLink);
        }
    }
}
