using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication1.Models;
using WebApplication1.Data;
using WebApplication1.Models;
using static System.Net.WebRequestMethods;
using System.Collections;
using System.Net.Http;
using Newtonsoft.Json;
using System.Xml.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<ActionResult<string>>  Index()
        {
            var url = $"https://localhost:7018/api/EmployeeData/GetEmpData";
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);
            string responseResult = await response.Content.ReadAsStringAsync();
            ViewBag.deserializedlistobj = JsonConvert.DeserializeObject<List<Employee>>(responseResult);
            return View();
        }
        public async Task<ActionResult> CreateOrUpdate(Employee employee)
        {
            var url = "https://localhost:7018/api/EmployeeData/CreateEmpData";
            var httpClient = new HttpClient();
            var loadData = JObject.Parse(JsonConvert.SerializeObject(employee)).ToString();
            var requestContent = new StringContent(loadData, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(url, requestContent);
            var content = await response.Content.ReadAsStringAsync();

            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<ActionResult> GetIdToDelete(Employee employee)
        {
            var url = $"https://localhost:7018/api/EmployeeData/DeleteEmpData/{employee.Id}";
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);
            var Result= await response.Content.ReadAsStringAsync();
            
            if (Result == "404")
            {
                //ViewBag.error = "Id not Found.";
                TempData["Error"] = "Id not Found.";
                return RedirectToAction("Error");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public async Task<JsonResult> GetEditDetails(Employee employee)
        {
            var Result = "";
            try
            {
                var url = $"https://localhost:7018/api/EmployeeData/GetEditEmpData/{employee.Id}";
                var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(url);
                Result = await response.Content.ReadAsStringAsync();
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
            return Json(Result);
        }
        [HttpPost]
        public JsonResult Edit(Employee employee)
        {
            var task = Task.Run(async () => await GetEditDetails(employee));
            var result = task.Result;
            return Json(result.Value);
        }
        public IActionResult Privacy()
        {
            return View();
        }
        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            if(TempData["Error"].ToString()!= null)
            {
                string lsError = TempData["Error"].ToString();
                ViewBag.error = lsError;
            }
            
            //return View(Content(TempData["ErrorTempData"].ToString()));
            return View();
        }

    }
}