using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TrainingAPI.Models;
using Microsoft.Extensions.Configuration;
using TrainingAPI.Utilities;
using Microsoft.EntityFrameworkCore;

namespace TrainingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly TrainingDbContext _context;
        private readonly IConfiguration Configuration;

        public UsersController(TrainingDbContext context, IConfiguration configuration)
        {
            _context = context;
            Configuration = configuration;
        }

        // Post: api/Users/Login
        [HttpPost("Login")]
        public ActionResult<ResponseData<LoginInfo>> Login([FromForm] string userId, [FromForm] string password)
        {
            try
            {
                //Thông tin đăng nhập admin từ file
                string infoPath = string.Format("{0}\\{1}", AppDomain.CurrentDomain.BaseDirectory, Configuration["LoginFile"]);
                string infoText = System.IO.File.ReadAllText(infoPath);
                JObject admInfo = JsonConvert.DeserializeObject<JObject>(infoText);

                if (string.Compare(userId, admInfo["UserID"].ToString(), false) != 0 || string.Compare(password, admInfo["Password"].ToString(), false) != 0)
                {
                    return new ResponseData<LoginInfo> { Message = "LoginID hoặc Password bị sai!"};
                }

                //Token đăng nhập của user
                var login = new LoginInfo()
                {
                    Token = Guid.NewGuid().ToString(),
                    UserId = userId
                };
                return new ResponseData<LoginInfo> { Message = "", Data = login };
            }
            catch (Exception ex)
            {
                return new ResponseData<LoginInfo> { Message = ex.Message};
            }
        }

        //Post: api/Users/List
        [HttpPost("List")]
        public ActionResult<ResponseData<PaginationSet<User>>> GetPagingUsers([FromForm] int page, [FromForm] string keyword)
        {
            List<User> listUsers;

            try
            {
                //Tìm kiếm theo keyword đã bỏ dấu
                if (!string.IsNullOrEmpty(keyword))
                {
                    var unsignkeyword = WordsUtil.ChuyenTiengVietKhongDau(keyword).ToLower();
                    listUsers = _context.Users.Where(c => c.Name.ToLower().Contains(keyword.ToLower()) || c.UnsignedName.ToLower().Contains(unsignkeyword)).ToList();
                }
                else
                {
                    listUsers = _context.Users.ToList();
                }

                //Phân trang nếu có danh sách
                if (listUsers.Count > 0)
                {
                    int countInPage;
                    if (!int.TryParse(Configuration["ItemsCountInPage"], out countInPage))
                    {
                        countInPage = 10;
                    }
                    var result = new PaginationSet<User>()
                    {
                        Page = page,
                        TotalPage = (int)Math.Ceiling((decimal)listUsers.Count / countInPage),
                        Items = listUsers.Skip((page - 1) * countInPage).Take(countInPage).ToList(),
                    };
                    return new ResponseData<PaginationSet<User>> { Message = "", Data = result };
                }
                return new ResponseData<PaginationSet<User>> { Message = "Không tìm thấy user!" };
            }
            catch (Exception ex)
            {
                return new ResponseData<PaginationSet<User>> { Message = ex.Message};
            }

        }

        //Get: api/Users/1
        [HttpGet("{id}")]
        public ActionResult<ResponseData<User>> GetUserDetail(int id)
        {
            try
            {
                //Get theo id
                var user = _context.Users.Find(id);
                if (user == null)
                {
                    return new ResponseData<User> { Message = "Không tìm thấy user thích hợp!" };
                }
                return new ResponseData<User> { Message = "", Data = user };
            }
            catch (Exception ex)
            {
                return new ResponseData<User> { Message = ex.Message };
            }
        }

        //Post: api/Users/Edit
        [HttpPut("Edit")]
        public ActionResult<ResponseData<User>> EditUser(User user)
        {
            try
            {
                //Get theo id
                var existUser = _context.Users.Find(user.ID);

                if (existUser == null)
                {
                    return new ResponseData<User> { Message = "Không tìm thấy user thích hợp!" };
                }
                else
                {
                    //Check trường email đã tồn tại
                    if (_context.Users.Any(c => c.ID != user.ID && string.Compare(c.Email, user.Email) == 0))
                    {
                        return new ResponseData<User> { Message = "Địa chỉ email đã tồn tại!" };
                    }

                    existUser.Name = user.Name;
                    existUser.UnsignedName = WordsUtil.ChuyenTiengVietKhongDau(user.Name);
                    existUser.Email = user.Email;
                    existUser.Tel = user.Tel;

                    //Cập nhật vào db
                    _context.SaveChanges();

                    return new ResponseData<User> { Message = "Cập nhật thông tin thành công!" };
                }
            }
            catch (Exception ex)
            {
                return new ResponseData<User> { Message = ex.Message };
            }
        }

        //Post: api/Users/Create
        [HttpPost("Create")]
        public ActionResult<ResponseData<User>> CreateUser(User user)
        {
            try
            {
                //Check trường email đã tồn tại
                if (_context.Users.Any(c => string.Compare(c.Email, user.Email) == 0))
                {
                    return new ResponseData<User> { Message = "Địa chỉ email đã tồn tại!" };
                }

                User newUser = new User
                {
                    Name = user.Name,
                    UnsignedName = WordsUtil.ChuyenTiengVietKhongDau(user.Name),
                    Email = user.Email,
                    Tel = user.Tel
                };
                _context.Users.Add(newUser);

                //Cập nhật vào db
                _context.SaveChanges();

                return new ResponseData<User> { Message = "Đăng ký thông tin thành công!" };
            }
            catch (Exception ex)
            {
                return new ResponseData<User> { Message = ex.Message };
            }
        }

        [HttpDelete("{id}")]
        public ActionResult<ResponseData<User>> DeleteUser(int id)
        {
            try
            {
                //Get theo id
                var user = _context.Users.Find(id);
                if (user == null)
                {
                    return new ResponseData<User> { Message = "Không tìm thấy user thích hợp!" };
                }

                _context.Users.Remove(user);

                //Cập nhật vào db
                _context.SaveChanges();

                return new ResponseData<User> { Message = "Xóa thông tin thành công!" };
            }
            catch (Exception ex)
            {
                return new ResponseData<User> { Message = ex.Message };
            }

        }
    }
}
