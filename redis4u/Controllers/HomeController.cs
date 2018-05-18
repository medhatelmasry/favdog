using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using redis4u.Models;

/*
Strings
Lists
Hashes
Sets
Sorted Sets
HyperLog
*/
namespace redis4u.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDistributedCache _cache;
        private IHostingEnvironment _env;
        IConfiguration _config;

        public HomeController(IDistributedCache cache, 
            IHostingEnvironment env,
            IConfiguration configuration)
        {
            _cache = cache;
            _env = env;
            _config = configuration;
        }

        public IActionResult Index()
        {
            string[] files = Utils.GetFiles(_env);

            List<Animal> dogs = Utils.GetAnimals(files, _env);

            var cacheKey = "TheTime";

            string existingTime = string.Empty; ;
            try
            {
                existingTime = _cache.GetString(cacheKey);
            } catch(Exception ex) {
                return View("RedisDown");
            }

            if (!string.IsNullOrEmpty(existingTime))
            {
                ViewData["DistCache"] = existingTime + " [FROM CACHE]";
            }
            else
            {
                existingTime = DateTime.UtcNow.ToString();
                _cache.SetString(cacheKey, existingTime);
                ViewData["DistCache"] = existingTime;
            }

            //string strCols = _config.GetValue<string>("table:columns");
            string strCols = _config["table:columns"];
            if (string.IsNullOrEmpty(strCols))
                ViewBag.Columns = "2";
            else
                ViewBag.Columns = strCols;

            return View(dogs);
        }

        public IActionResult Submit(string id)
        {
            if (!string.IsNullOrEmpty(id)) {
                var strVote = _cache.GetString(id);
                int intVote = 0;
                if (!string.IsNullOrEmpty(strVote))
                {
                    intVote = Convert.ToInt32(strVote);
                }
                intVote++;
                _cache.SetString(id, intVote.ToString());
            }

            string[] files = Utils.GetFiles(_env);
            List<Animal> dogs = Utils.GetAnimals(files, _env);

            List<AnimalVote> votes = new List<AnimalVote>();
            foreach (var item in dogs)
            {
                var strVote = _cache.GetString(item.Name);
                int intVote = 0;
                if (!string.IsNullOrEmpty(strVote))
                {
                    intVote = Convert.ToInt32(strVote);
                }

                AnimalVote vote = new AnimalVote()
                {
                    Name=item.Name,
                    Count=intVote,
                    PictureUrl=item.PictureUrl,
                };

                votes.Add(vote);
            }

            int totalCount = votes.Sum(v => v.Count);

            foreach(var item in votes)
            {
                item.Percent = item.Count * 100 / totalCount;
            }

            string strCols = _config.GetValue<string>("table:columns");
            if (string.IsNullOrEmpty(strCols))
                ViewBag.Columns = "2";
            else
                ViewBag.Columns = strCols;

            return View(votes);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
