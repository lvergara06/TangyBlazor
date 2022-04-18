using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Tangy_Common;
using Tangy_Models;
using TangyWeb_API.Helper;

namespace TangyWeb_API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : Controller
    {
        
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly APISettings _aPISettings;

        public AccountController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<APISettings> options)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _aPISettings = options.Value;
        }


        [HttpPost]
        public async Task<IActionResult> SignIn([FromBody] SignInRequestDTO SignInRequestDTO)
        {
           if(SignInRequestDTO == null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            // Call the sign in manager with user password combo
            var result = await _signInManager.PasswordSignInAsync(SignInRequestDTO.UserName, SignInRequestDTO.Password, false, false);

            // Check the results of the create
            if (!result.Succeeded)
            {
                return BadRequest(new SignInResponseDTO
                {
                    IsAuthSuccessful = false,
                    ErrorMessage = "User Not Found"
                });
            }

            IdentityUser user = await _userManager.FindByNameAsync(SignInRequestDTO.UserName);
            
            if(user == null)
            {
                return BadRequest(new SignInResponseDTO
                {
                    IsAuthSuccessful = false,
                    ErrorMessage = "Could not find user"
                });
            }

            // Here we have to give tokens to the user when it successfully logs in
            var signingCredentials = GetSigningCredentials();
            var claims = await GetClaims(user);
            var tokenOptions = new JwtSecurityToken(
                issuer: _aPISettings.ValidIssuer,
                audience: _aPISettings.ValidAudience,
                claims: claims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: signingCredentials);

            var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            return Ok(new SignInResponseDTO()
            {
                Token = token,
                IsAuthSuccessful = true,
                UserDTO = new UserDTO()
                {
                    Name = user.UserName,
                    Id = user.Id,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber
                }
            });
        }

        [HttpPost]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequestDTO SignUpRequestDTO)
        {
            if (SignUpRequestDTO == null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            // Create a user based on the SignUp Request
            var user = new IdentityUser()
            {
                UserName = SignUpRequestDTO.Email,
                Email = SignUpRequestDTO.Email,
                PhoneNumber = SignUpRequestDTO.PhoneNumber,
                EmailConfirmed = true
            };

            // Create the new user using _userManager
            var result = await _userManager.CreateAsync(user, SignUpRequestDTO.Password);

            // Check the results of the create
            if (!result.Succeeded)
            {
                return BadRequest(new SignUpResponseDTO()
                {
                    IsRegisterationSuccessful = false,
                    Errors = result.Errors.Select(i => i.Description)
                });
            }

            // Now that user is created successfully add it to a role
            var roleResult = await _userManager.AddToRoleAsync(user, SD.Role_Customer);

            if (!roleResult.Succeeded)
            {
                return BadRequest(new SignUpResponseDTO()
                {
                    IsRegisterationSuccessful = false,
                    Errors = result.Errors.Select(i => i.Description)
                });
            }

            //Valid entry
            return StatusCode(201);
        }

        private SigningCredentials GetSigningCredentials()
        {
            var secret = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_aPISettings.SecretKey));
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        private async Task<List<Claim>> GetClaims(IdentityUser user)
        {
            // Claims get saved in your tokens
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("Id", user.Id)
            };

            // Get the role for the user --> Find the user by email first
            var roles = await _userManager.GetRolesAsync(await _userManager.FindByEmailAsync(user.Email));

            foreach(var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }

    }
}
