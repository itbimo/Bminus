using Bminus.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Bminus.Controllers
{
    public class AuthorizationController : Controller
    {
        public AuthorizationController()
        {
            if (System.IO.File.Exists(Constants.USER_INFO_FILE_NAME) == false)
            {
                using System.IO.FileStream fs = System.IO.File.Create(Constants.USER_INFO_FILE_NAME);
            }
        }

        [HttpGet]
        public IActionResult Registration(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            var user = new User();
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Registration(User user, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var jsonData = System.IO.File.ReadAllText(Constants.USER_INFO_FILE_NAME);
                var users = JsonConvert.DeserializeObject<List<User>>(jsonData) ?? new List<User>();
                var oldUser = users.Where(x => x.Email == user.Email);

                if (oldUser.Any())
                {
                    user.ErrorMessage = "Пользователь уже существует.";
                    return View(user);
                }
                users.Add(user);
                jsonData = JsonConvert.SerializeObject(users);
                System.IO.File.WriteAllText(Constants.USER_INFO_FILE_NAME, jsonData);
                //Авторизация
                var claims = new List<Claim>
                {
                    new Claim("user", user.Email),
                    new Claim("role", "Member")
                };
                await HttpContext.SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies", "user", "role")));
                Response.Cookies.Delete("userName");
                Response.Cookies.Append("userName", user.Nickname);
                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return Redirect("/");
                }
            }
            return View(user);
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            var user = new User();
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Login(User user, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            //Проверка валидности логина и пароля
            var jsonData = System.IO.File.ReadAllText(Constants.USER_INFO_FILE_NAME);
            var users = JsonConvert.DeserializeObject<List<User>>(jsonData) ?? new List<User>();
            var currentUser = users.Where(x => x.Email == user.Email && x.Password == user.Password);

            if (currentUser.Any())
            {
                //Авторизация
                var claims = new List<Claim>
                {
                    new Claim("user", user.Email),
                    new Claim("role", "Member")
                };
                await HttpContext.SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies", "user", "role")));
                Response.Cookies.Delete("userName");
                Response.Cookies.Append("userName", currentUser.First().Nickname);
                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return Redirect("/");
                }
            }
            user.ErrorMessage = "Неверный email или пароль";
            return View(user);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            Response.Cookies.Delete("userName");
            Response.Cookies.Delete("Premium");
            return RedirectToAction("Login");
        }

        public IActionResult GetPremium()
        {
            Response.Cookies.Append("Premium", "true");
            return RedirectToAction("Index", "Music");
        }
    }
}