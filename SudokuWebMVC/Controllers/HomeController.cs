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
using SudokuWebMVC.Services;
using SudokuWebMVC.Validations;

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

        public async Task<JsonResult> NewGame(int level)
        {
            var NewBoard = await new SudokuGenerator().LoadFromFileAsync().ConfigureAwait(false);
            NewBoard = await new SudokuGenerator().PrepareBoardAsync((Difficulty)level, NewBoard).ConfigureAwait(false);
            //use newtonsoft because can serialize bidimensional array
            var jsonoutPut = JsonConvert.SerializeObject(NewBoard);
            return Json(jsonoutPut);
        }

        [HttpPost]
        public async Task<JsonResult> ValidateBoard(string jsonMatrix)
        {
            var Matrix = JsonConvert.DeserializeObject<int[,]>(jsonMatrix);
            return Json(await new SudokuValidations().MatrixIsDoneAsync(Matrix));
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
