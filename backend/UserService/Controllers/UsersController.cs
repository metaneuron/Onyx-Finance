using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.IdentityModel.Tokens.Jwt;
using WebApi.Helpers;
using Microsoft.Extensions.Options;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using WebApi.Services;
using WebApi.Dtos;
using WebApi.Entities;

/*
ASP.NET core users controllers defines and handles all routes/endpoints for the api 
that is related to users, including authentication, registration and standard CRUD operations.

Controller actions are secured with JWT using the [Authorize] attribute, with the exception
of Authentication and Register methods which allow public access by overriding [Authorize]
attribute on the controller with [AllowAnonymous] attributes on each action method.
*/
namespace WebApi.Controllers {
    [Authorize]
    [ApiController]
    [Route("[controller")]
    public class UsersController : ControllerBase {
        private IUserService _userService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public UsersController(
            IUserService userService, 
            IMapper mapper, 
            IOptions<AppSettings> appSettings) 
        {
            _userService = userService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        } 
    }

    /*
    On successful authentication, Authenticate generates a JWT (JSON Web Token) using
    the JwtSecurityTokenHandler class that generates a token that is signed using
    a secret key stored in appsettings.json. The JWT token is returned to the client
    which the includes it in HTTP Authorization header of subsequent web api requests.
     */
    [AllowAnonymous]
    [HttpPost("Authenticate")]
    public IActionResult Authenticate([FromBody]UserDto userDto) {
        var user = _userService.Authenticate(userDto.Username, userDto.Password);

        if(user == null)
            return BadRequest(new {message = "Username or password is incorrect"});

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor {
            Subject = new ClaimsIdentity(new Claim[]{
                new Claim(ClaimTypes.Name, user.Id.toString())
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        // return basic user info (w/o pass) and token to store client side
        return Ok(new {
            Id = user.Id,
            Username = user.Username,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Token = tokenString
        });
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public IActionResult Register([FromBody]UserDto userDto) {
        // map dto to entity
        var user = _mapper.Map<User>(userDto);
        try {
            // saving user
            _userService.Create(user, userDto.Password);
            return Ok();
        } catch (AppException ex) {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    public IActionResult GetAll() {
        var users = _userService.GetAll();
        var userDtos = _mapper.Map<IList<userDto>>(users);
        return Ok(userDtos);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id) {
        var user = _userService.GetById(id);
        var userDto = _mapper.Map<UserDto>(user);
        return Ok(userDto);
    }

    [HttpGet("{id}")]
    public IActionResult Update(int id, [FromBody]UserDto userDto) {
        // map dto to entity and set id
        var user = _mapper.Map<User>(userDto);
        user.Id = id;

        try {
            // save
            _userService.Update(user, userDto.Password);
            return Ok();
        } catch(AppException ex) {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id) {
        _userService.Delete(id);
        return Ok();
    }
}