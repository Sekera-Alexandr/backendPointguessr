using backendPointguessr.Classes;
using Microsoft.AspNetCore.Mvc;


namespace backendPointguessr.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : Controller
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public UsersController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Endpoint vytvoření uživatele
        /// </summary>
        /// <param name="user">Uživatel</param>
        /// <returns>Zaregistrovaný uživatel</returns>
        [HttpPost]
        public Users? CreateUser([FromBody] Users user)
        {
            Database db = new Database(_configuration);
            return db.SignUp(user);
        }

        /// <summary>
        /// Endpoint přihlášení uživatele
        /// </summary>
        /// <param name="user">Uživatel</param>
        /// <returns>Přihlášený uživatel</returns>
        [HttpPost("login")]
        public Users? LoginUser([FromBody] Users user)
        {
            Database db = new Database(_configuration);
            return db.Login(user);
        }

        /// <summary>
        /// Endpoint vytvoření záznamu mapy
        /// </summary>
        /// <param name="mapa">Mapa</param>
        /// <returns>true=úspěšně zaznamenáno</returns>
        [HttpPost("maps")]
        public IActionResult CreateMap([FromBody] Maps mapa) 
        {
            Database db = new Database(_configuration);
            return Ok(db.SaveImage(mapa));
        }

        /// <summary>
        /// Endpoint získání záznamu mapy
        /// </summary>
        /// <returns>Mapa</returns>
        [HttpGet("maps")]
        public Maps? GetMap() 
        {
            Database db = new Database(_configuration);
            return db.GetMap();
        }
    }
}