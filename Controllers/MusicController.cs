using System.Collections.Generic;
using System.Linq;
using Bminus.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Bminus.Controllers
{
    [Authorize]
    public class MusicController : Controller
    {
        public MusicController()
        {
            if (System.IO.File.Exists(Constants.MUSIC_INFO_FILE_NAME) == false)
            {
                using System.IO.FileStream fs = System.IO.File.Create(Constants.MUSIC_INFO_FILE_NAME);
            }         
        }

        public IActionResult Index(Genre? genre, string text)
        {
            Request.Cookies.TryGetValue("userName", out var userName);
            ViewData["user"] = userName;
            Request.Cookies.TryGetValue("Premium", out var premium);
            ViewData["premium"] = premium;
            var jsonData = System.IO.File.ReadAllText(Constants.MUSIC_INFO_FILE_NAME);
            var music = JsonConvert.DeserializeObject<List<Song>>(jsonData) ?? new List<Song>();
            if (genre != null)
            {
                music = music.Where(x => x.Genre == genre).ToList();
            }
            if (!string.IsNullOrEmpty(text))
            {
                music = music.Where(x => $"{x.Artist.Title} {x.Title}".Contains(text)).ToList();
            }

            return View(music);
        }

        [HttpGet]
        public IActionResult Create()
        {
            Request.Cookies.TryGetValue("userName", out var userName);
            ViewData["user"] = userName;
            Request.Cookies.TryGetValue("Premium", out var premium);
            ViewData["premium"] = premium;
            return View();
        }

        [HttpPost]
        public IActionResult Create(Song song)
        {
            Request.Cookies.TryGetValue("userName", out var userName);
            ViewData["user"] = userName;
            Request.Cookies.TryGetValue("Premium", out var premium);
            ViewData["premium"] = premium;
            if (ModelState.IsValid)
            {
                var jsonData = System.IO.File.ReadAllText(Constants.MUSIC_INFO_FILE_NAME);
                var music = JsonConvert.DeserializeObject<List<Song>>(jsonData) ?? new List<Song>();
                music.Add(song);
                jsonData = JsonConvert.SerializeObject(music);
                System.IO.File.WriteAllText(Constants.MUSIC_INFO_FILE_NAME, jsonData);

                return RedirectToAction("Index");
            }

            return View(song);
        }
    }
}