using Domain.Commands;
using Domain.DTOs;
using Domain.DTOs.Account;
using Domain.DTOs.Admin;
using Domain.DTOs.Notification;
using Domain.Interface;
using Domain.Models;
using Domain.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IMediator _mediator;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        public AccountController(UserManager<AppUser> userManager, IConfiguration configuration , IMediator mediator, IEmailSender emailSender, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _userManager = userManager;
            _configuration = configuration;
            _mediator = mediator;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            Guid a = Guid.Parse("00000000-0000-0000-0000-000000000001");
            var inMaintenance0 = await _mediator.Send(new GetGenericQuery<Parametres, Guid>(a));
            bool inMaintenance = inMaintenance0.Maintenance;
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var roles = await _userManager.GetRolesAsync(user);
                var roleString = roles.Any() ? roles.First() : string.Empty;
                if (inMaintenance && roleString != "Admin")
                {
                    return BadRequest("Our website is temporarily unavailable due to maintenance. Thank you for your patience.");
                }
                else
                {
                    var token = await GenerateJwtToken(user);
                    return Ok(new { Token = token });
                }
                
            }
            return Unauthorized("Invalid credentials");
        }


        [HttpGet("users")]
        [Authorize]
        public async Task<IActionResult> GetUsers([FromQuery] string? search)
        {
            IEnumerable<AppUser> users;

            if (string.IsNullOrEmpty(search))
            {
                //CQRS
                var query = new GetListGenericQuery<AppUser>();
                users = await _mediator.Send(query);
            }
            else
            {
                //UserManager
                users = await _userManager.Users
                .Where(u => EF.Functions.Like(u.UserName, $"%{search}%"))
                .ToListAsync();
            }

            var userDtos = new List<UserDTO>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                string role = roles.Any() ? roles.First() : string.Empty;

                userDtos.Add(new UserDTO
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    Phone = user.PhoneNumber,
                    Role = role,
                    Created = user.Created,
                    FirstName = user.First_name,
                    LastName = user.Last_name

                });
            }

            return Ok(userDtos);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Ok(new { Message = "Si le compte existe, un lien de réinitialisation du mot de passe sera envoyé" });
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = System.Net.WebUtility.UrlEncode(token);


            var resetLink = $"https://localhost:4200/reset-password?email={model.Email}&token={encodedToken}";


            await _emailSender.SendEmailAsync(model.Email,
    "Réinitialisation de votre mot de passe",
    $@"<html>
<head>
  <meta http-equiv='Content-Type' content='text/html; charset=UTF-8' />
  <meta name='viewport' content='width=device-width, initial-scale=1.0' />
  <title>Réinitialiser Votre Mot de Passe</title>
  <style>
    body {{
      margin: 0;
      padding: 0;
      background-color: #f6f6f6;
      font-family: Arial, sans-serif;
      color: #333333;
    }}

    .email-container {{
      width: 100%;
      margin: 0 auto;
      background-color: #f6f6f6;
      padding: 20px 0;
    }}

    .logo-container {{
      text-align: center;
      padding: 20px 0;
    }}

    .logo-container img {{
      max-width: 150px;
    }}

    .content {{
      max-width: 600px;
      margin: 0 auto;
      background-color: #ffffff;
      border-radius: 5px;
      overflow: hidden;
      box-shadow: 0 0 10px rgba(0,0,0,0.1);
    }}

    .content-header {{
      background: linear-gradient(90deg, #4A90E2, #005bea);
      color: #ffffff;
      text-align: center;
      padding: 20px;
    }}

    .content-header h1 {{
      margin: 0;
      font-size: 24px;
      font-weight: 400;
    }}

    .content-body {{
      padding: 20px 30px;
    }}

    .content-body h2 {{
      font-size: 20px;
      margin-top: 0;
    }}

    .button {{
      display: inline-block;
      background-color: #4A90E2;
      color: #ffffff;
      text-decoration: none;
      padding: 12px 25px;
      border-radius: 4px;
      margin-top: 20px;
      font-weight: bold;
    }}

    .content-footer {{
      background-color: #f2f2f2;
      padding: 15px 30px;
      text-align: center;
      font-size: 12px;
      color: #666666;
    }}

    .footer-text {{
      margin: 0;
      line-height: 1.5;
    }}

    @media only screen and (max-width: 600px) {{
      .content-body, .content-header, .content-footer {{
        padding: 15px;
      }}
      .button {{
        padding: 10px 20px;
      }}
    }}
  </style>
</head>
<body>
  <div class='email-container'>
 
    <div class='logo-container'>
      <img src='https://cdn.brandfetch.io/idg4eYjY1W/w/387/h/150/theme/dark/logo.png?c=1dxbfHSJFAPEGdCLU4o5B' alt='Company Logo' />
    </div>

    <div class='content'>
   
      <div class='content-header'>
        <h1>Réinitialisation de Mot de Passe</h1>
      </div>

      <div class='content-body'>
        <h2>Bonjour,</h2>
        <p>
          Vous avez demandé à réinitialiser votre mot de passe. 
          Veuillez cliquer sur le bouton ci-dessous pour le réinitialiser :
        </p>

        <div style='text-align:center;'>
          <a href='{resetLink}' style='color:white' class='button'>Réinitialiser le mot de passe</a>
        </div>

        <p style='margin-top: 20px;'>
          Si vous n'êtes pas à l'origine de cette demande, vous pouvez ignorer ce message.
        </p>
      </div>

      <div class='content-footer'>
        <p class='footer-text'>
          &copy; 2025 Votre Société. Tous droits réservés.<br/>
          Ceci est un e-mail automatique, merci de ne pas y répondre.
        </p>
      </div>
    </div>
  </div>
</body>
</html>");


            return Ok(new { Message = "Si le compte existe, un lien de réinitialisation du mot de passe sera envoye.", Token = token });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Ok(new { Message = "Si le compte existe, un lien de réinitialisation du mot de passe sera envoye" });
            }

            var decodedToken = System.Net.WebUtility.UrlDecode(model.Token);
            var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.NewPassword);
            if (result.Succeeded)
            {
                return Ok(new { Message = "Votre mot de passe a ete reinitialise avec succes" });
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }





        private async Task<string> GenerateJwtToken(AppUser user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["Secret"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var expiryInMinutes = int.Parse(jwtSettings["ExpiryInMinutes"]);

            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryInMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }




        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (model.NewPassword != model.ConfirmNewPassword)
            {
                return BadRequest(new { Message = "La nouvelle mot de passe et sa confirmation ne correspondent pas." });
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Unauthorized();
            }

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {
                return Ok(new { Message = "Votre mot de passe a été modifié avec succès." });
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }


        [HttpGet("profile")]
        [Authorize] 
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByIdAsync(userId);
            var roles = await _userManager.GetRolesAsync(user);
            var roleString = roles.Any() ? roles.First() : string.Empty;
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var profile = new ProfileDTO
            {
                Username = user.UserName,
                Email = user.Email,
                Phone = user.PhoneNumber,
                Role = roleString,
                Created = user.Created,
                FirstName = user.First_name,
                LastName = user.Last_name
            };

            return Ok(profile);
        }

        [HttpPut("profile")]
        [Authorize] 
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDTO model)
        {
            if(!string.IsNullOrEmpty(model.Email))
            {
                var user0 = await _userManager.FindByEmailAsync(model.Email);
                if (user0 != null)
                {
                    return BadRequest("Email existe déjà");
                }
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found.");

            if (!string.IsNullOrEmpty(model.Username))
                user.UserName = model.Username;
            if (!string.IsNullOrEmpty(model.Email))
                user.Email = model.Email;
            if (!string.IsNullOrEmpty(model.Phone))
                user.PhoneNumber = model.Phone;
            if (!string.IsNullOrEmpty(model.FirstName))
                user.First_name = model.FirstName;
            if (!string.IsNullOrEmpty(model.LastName))
                user.Last_name = model.LastName;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                return Ok(new { Message = "Profile updated successfully." });
            else
                return BadRequest(result.Errors);
        }

        [HttpGet("get-picture")]
        [Authorize]
        public async Task<IActionResult> GetPicture()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.Picture == null)
            {
                return NotFound("No picture found.");
            }


            string base64String = Convert.ToBase64String(user.Picture);
            return Ok($"data:image/png;base64,{base64String}");
        }


        [HttpPost("upload-picture")]
        [Authorize]
        public async Task<IActionResult> UploadPicture(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            byte[] pictureBytes;
            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                pictureBytes = ms.ToArray();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.Picture = pictureBytes;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Ok("Picture uploaded successfully.");
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpGet("get-picture-u")]
        [Authorize]
        public async Task<IActionResult> GetPictureu(string? Username)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            if (string.IsNullOrEmpty(Username))
            {
                return NotFound("hat username fi URL se3a");
            }

            var user = await _userManager.FindByNameAsync(Username);
            if (user == null || user.Picture == null)
            {
                return NotFound("No picture found.");
            }

            string base64String = Convert.ToBase64String(user.Picture);
            return Ok($"data:image/png;base64,{base64String}");
        }
        //===============================================================================

        [HttpPost("CreateNotification")]
        public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var senderIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(senderIdStr) || !Guid.TryParse(senderIdStr, out Guid senderId))
            {
                return Unauthorized("Invalid token.");
            }

            var notificationsCreated = new List<Nofication>();

            if (!string.IsNullOrWhiteSpace(dto.RoleDes))
            {
                var notification = new Nofication
                {
                    IdNofication = Guid.NewGuid(),
                    RoleDes = dto.RoleDes,
                    IdDes = Guid.Empty,
                    Type = dto.Type,
                    CreatedAt = DateTime.Now,
                    Message = dto.Message,
                    FKappuser = senderId
                };

                var command = new AddGenericCommand<Nofication>(notification);
                var result = await _mediator.Send(command);
                if (result != null)
                {
                    notificationsCreated.Add(result);
                }
            }
            else if (dto.Usernames != null && dto.Usernames.Any(u => !string.IsNullOrWhiteSpace(u)))
            {
                foreach (var username in dto.Usernames.Where(u => !string.IsNullOrWhiteSpace(u)))
                {
                    var targetUser = await _userManager.FindByNameAsync(username);
                    if (targetUser != null)
                    {
                        var notification = new Nofication
                        {
                            IdNofication = Guid.NewGuid(),
                            RoleDes = dto.RoleDes,
                            IdDes = targetUser.Id,
                            Type = dto.Type,
                            CreatedAt = DateTime.Now,
                            Message = dto.Message,
                            FKappuser = senderId
                        };

                        var command = new AddGenericCommand<Nofication>(notification);
                        var result = await _mediator.Send(command);
                        if (result != null)
                        {
                            notificationsCreated.Add(result);
                        }
                    }
                }
            }
            else
            {
                var notification = new Nofication
                {
                    IdNofication = Guid.NewGuid(),
                    RoleDes = dto.RoleDes,
                    IdDes = Guid.Empty,
                    Type = dto.Type,
                    CreatedAt = DateTime.Now,
                    Message = dto.Message,
                    FKappuser = senderId
                };

                var command = new AddGenericCommand<Nofication>(notification);
                var result = await _mediator.Send(command);
                if (result != null)
                {
                    notificationsCreated.Add(result);
                }
            }

            if (notificationsCreated.Any())
            {
                return Ok(new { Message = $"Notifications created successfully for {notificationsCreated.Count} case(s)." });
            }
            else
            {
                return BadRequest(new { Message = "Failed to create any notifications." });
            }
        }


        [HttpGet("notifications")]
        [Authorize]
        public async Task<IActionResult> GetNotifications()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            if (!Guid.TryParse(userId, out Guid parsedUserId))
                return BadRequest("Invalid user id.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found.");

            var roles = await _userManager.GetRolesAsync(user);
            string role = roles.FirstOrDefault() ?? string.Empty;

            var notifications = await _mediator.Send(new FindGenericQuery<Nofication>(
                n => (n.IdDes.HasValue && n.IdDes.Value != Guid.Empty && n.IdDes.Value == parsedUserId)
                     || (!string.IsNullOrEmpty(n.RoleDes) && n.RoleDes == role)
                     || (string.IsNullOrEmpty(n.RoleDes) && (n.IdDes == Guid.Empty || n.IdDes == null))
            ));

            var notificationDtos = new List<NotificationDTO>();
            foreach (var n in notifications)
            {
                var username = await UsernameAsync(n.FKappuser);
                notificationDtos.Add(new NotificationDTO
                {
                    Id = n.IdNofication,
                    Type = n.Type,
                    CreatedAt = n.CreatedAt,
                    Message = n.Message,
                    Username = username,
                    Read = n.Read
                });
            }

            return Ok(notificationDtos);
        }

        private async Task<string> UsernameAsync(Guid fk)
        {
            var user = await _userManager.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == fk);
            return user?.UserName ?? "Unknown";
        }





        [HttpGet("markAsRead")]
        public async Task<IActionResult> MarkNotificationAsRead([FromQuery] Guid id)
        {
            var notification = await _mediator.Send(new GetGenericQuery<Nofication, Guid>(id));
            if (notification == null)
            {
                return NotFound("Notification not found.");
            }

            notification.Read = true;

            var command = new PutGenericCommand<Nofication>(notification);
            var result = await _mediator.Send(command);
            if (result == null)
            {
                return BadRequest("Failed to update notification.");
            }

            return Ok("Notification marked as read.");
        }

        [HttpDelete("notification/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteNotification(Guid id)
        {
            var command = new DeleteGenericCommand<Nofication, Guid>(id);
            var result = await _mediator.Send(command);

            if (result)
            {
                return Ok(new { Message = "Notification deleted successfully." });
            }
            else
            {
                return BadRequest(new { Message = "Failed to delete notification." });
            }
        }


        [HttpGet("statistics")]
        [Authorize]
        public async Task<IActionResult> GetDashboardStatistics()
        {
            var adminCount = (await _userManager.GetUsersInRoleAsync("Admin")).Count;
            var factoryMCount = (await _userManager.GetUsersInRoleAsync("FactoryM")).Count;
            var qualityMCount = (await _userManager.GetUsersInRoleAsync("QualityM")).Count;
            var auditorCount = (await _userManager.GetUsersInRoleAsync("Auditor")).Count;

            var factories = await _mediator.Send(new GetListGenericQuery<Factory>());
            var filiales = await _mediator.Send(new GetListGenericQuery<Filiale>());
            var planActions = await _mediator.Send(new GetListGenericQuery<PlanAction>());
            var rapports = await _mediator.Send(new GetListGenericQuery<Rapport>());

            int usineCount = factories?.Count() ?? 0;
            int filialeCount = filiales?.Count() ?? 0;
            int planActionCount = planActions?.Count() ?? 0;
            int rapportCount = rapports?.Count() ?? 0;

            var criteres = await _mediator.Send(new GetListGenericQuery<Critaire>());
            var sxs = await _mediator.Send(new GetListGenericQuery<Sx>());

            var sxNameById = sxs.ToDictionary(s => s.IdSx, s => s.NameEnglish);

            double globalAverage5s = criteres.Any()
                ? criteres.Average(c => c.Score)
                : 0;

            var averagesByS = criteres
                .Where(c => c.FKsx.HasValue && sxNameById.ContainsKey(c.FKsx.Value))
                .GroupBy(c => sxNameById[c.FKsx.Value])
                .ToDictionary(
                    grp => grp.Key,
                    grp => grp.Average(c => c.Score)
                );


            var statsDto = new DashboardStatisticsDTO
            {
                AdminCount = adminCount,
                FactoryMCount = factoryMCount,
                QualityMCount = qualityMCount,
                AuditorCount = auditorCount,
                UsineCount = usineCount,
                FilialeCount = filialeCount,
                PlanActionCount = planActionCount,
                RapportCount = rapportCount,
                GlobalAverage5s = globalAverage5s,
            };

            return Ok(statsDto);
        }



        [HttpGet("ismaintenance")]
        public async Task<IActionResult> IsMaintenance()
        {
            Guid a = Guid.Parse("00000000-0000-0000-0000-000000000001");
            var param = await _mediator.Send(new GetGenericQuery<Parametres, Guid>(a));

            if (param == null)
            {
                return Ok(new { IsMaintenance = false });
            }

            return Ok(new { IsMaintenance = param.Maintenance });
        }



    }



}



