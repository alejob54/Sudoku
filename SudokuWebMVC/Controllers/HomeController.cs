using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SudokuWebMVC.Enum;
using SudokuWebMVC.Helpers;
using SudokuWebMVC.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SudokuWebMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        //public IActionResult NewGame()
        //{

        //    return View();
        //}

        public async Task<JsonResult> NewGame()
        {
            var NewBoard = new SudokuGenerator().LoadFromFile();
            NewBoard = new SudokuGenerator().PrepareBoard((Difficulty)2, NewBoard);
            //use newtonsoft because can serialize bidimensional array
            var jsonoutPut = JsonConvert.SerializeObject(NewBoard);
            return Json(jsonoutPut);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
