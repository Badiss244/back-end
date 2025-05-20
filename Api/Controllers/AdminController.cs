using System.Reflection.Metadata;
using System.Security.Claims;
using Domain.Commands;
using Domain.DTOs.Admin;
using Domain.DTOs.Factory;
using Domain.DTOs.Filiale;
using Domain.Interface;
using Domain.Models;
using Domain.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IMediator _mediator;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IEmailSender _emailSender;

        public AdminController(IEmailSender emailSender, UserManager<AppUser> userManager, IConfiguration configuration, IMediator mediator, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _userManager = userManager;
            _configuration = configuration;
            _mediator = mediator;
            _roleManager = roleManager;
            _emailSender = emailSender;
        }


        [HttpGet("users")]
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

        [HttpPost("createUser")]
        public async Task<IActionResult> CreateUser([FromBody] RegisterDTO model)
        {
            var user0 = await _userManager.FindByEmailAsync(model.Email);
            if (user0 != null)
            {
                return BadRequest("Mail already exist ya b10");
            }
            bool isAllDigits = model.Phone.All(char.IsDigit);
            if (model.Phone.Length != 8 || !isAllDigits)
            {
                return BadRequest("mochkla fi nomro ya 7oma");
            }
            var user = new AppUser { UserName = model.Username, Email = model.Email, PhoneNumber = model.Phone, First_name = model.FirstName, Last_name = model.LastName };
            if (!await _roleManager.RoleExistsAsync(model.Role))
            {
                return BadRequest($"The role '{model.Role}' does not exist.");
            }
            if (model.Role == "FactoryM")
            {
                if (model.FactoryId == Guid.Empty)
                {
                    return BadRequest("Factory id is required for role FactoryM.");
                }

                var factory = await _mediator.Send(new GetGenericQuery<Factory, Guid>((Guid)model.FactoryId));
                if (factory == null)
                {
                    return BadRequest("The provided Factory id does not exist.");
                }

                var existingManagers = await _userManager.Users
                    .Where(u => u.Factory != null && u.Factory.IdFactory == model.FactoryId)
                    .ToListAsync();

                if (existingManagers.Any())
                {
                    return BadRequest("This factory already has a factory manager assigned.");
                }

                user.Factory = factory;
            }
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);


            await _userManager.AddToRoleAsync(user, model.Role);
            await _emailSender.SendEmailAsync(model.Email, "Création de compte", $@"
<!DOCTYPE html>
<html>
<head>
    <meta http-equiv='Content-Type' content='text/html; charset=UTF-8' />
    <meta name='viewport' content='width=device-width, initial-scale=1.0' />
    <title>Informations de Connexion</title>
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
        .info-box {{
            background-color: #f2f2f2;
            padding: 15px;
            border-radius: 5px;
            margin: 20px 0;
        }}
        .info-box p {{
            margin: 0;
            line-height: 1.6;
        }}
        .highlight {{
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
                <h1>Informations de Connexion</h1>
            </div>

            
            <div class='content-body'>
                <h2>Bonjour {model.FirstName},</h2>
                <p>
                    Votre compte a été créé avec succès. Vous trouverez ci-dessous vos identifiants de connexion :
                </p>

                <div class='info-box'>
                    <p><span class='highlight'>Nom d'utilisateur :</span> {model.Username}</p>
                    <p><span class='highlight'>Mot de passe :</span> {model.Password}</p>
                </div>

                <p>
                    Nous vous recommandons  changer votre mot de passe dès la première connexion
                </p>

                <p>
                    Pour vous connecter, veuillez visiter notre site web ou cliquer sur le bouton ci-dessous :
                </p>

                <p style='text-align:center; margin: 30px 0;'>
                    <a 
                      href='https://localhost:4200/' 
                      style='
                          display: inline-block;
                          background-color: #4A90E2;
                          color: #ffffff;
                          text-decoration: none;
                          padding: 12px 25px;
                          border-radius: 4px;
                          font-weight: bold;
                      '
                    >
                        Se Connecter
                    </a>
                </p>
            </div>

            <div class='content-footer'>
                <p class='footer-text'>
                    &copy; 2025 Poulina Holding Group. Tous droits réservés.<br/>
                    Ceci est un e-mail automatique, merci de ne pas y répondre.
                </p>
            </div>

        </div>
    </div>
</body>
</html>
");

            return Ok("User created and role assigned successfully");
        }

        [HttpDelete("user/{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound(new { Message = "User not found." });
            }


            // XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX lazimni ntestiha
            if (await _userManager.IsInRoleAsync(user, "QualityM"))
            {
                var planActions = await _mediator.Send(new FindGenericQuery<PlanAction>(
                    pa => pa.FKqualitym == user.Id));


                foreach (var planAction in planActions)
                {
                    planAction.FKqualitym = null;
                    await _mediator.Send(new PutGenericCommand<PlanAction>(planAction));
                }
            }

            if (await _userManager.IsInRoleAsync(user, "Auditor"))
            {
                var audits = await _mediator.Send(new FindGenericQuery<Audit>(
                    pa => pa.FKauditor == user.Id));
                var rapports = await _mediator.Send(new FindGenericQuery<Rapport>(
                    pa => pa.FKauditor == user.Id));

                foreach (var audit in audits)
                {
                    audit.FKauditor = null;
                    audit.FKfactory = null;
                    await _mediator.Send(new PutGenericCommand<Audit>(audit));
                    await _mediator.Send(new DeleteGenericCommand<Audit, Guid>(audit.IdAudit));
                }
                foreach (var rapport in rapports)
                {
                    rapport.FKauditor = null;
                    await _mediator.Send(new PutGenericCommand<Rapport>(rapport));
                }
            }
            // XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
            var notifications = await _mediator.Send(new FindGenericQuery<Nofication>(n => n.FKappuser == user.Id));
            foreach (var notification in notifications)
            {
                await _mediator.Send(new DeleteGenericCommand<Nofication, Guid>(notification.IdNofication));
            }
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return Ok(new { Message = "User deleted successfully." });
            }
            else
            {
                return BadRequest(result.Errors); 
            }
        }


        //====================================================================================================================

        [HttpGet("filiales")]
        public async Task<IActionResult> GetFiliales([FromQuery] string? search)
        {
            IEnumerable<Filiale> filiales;

            if (string.IsNullOrEmpty(search))
            {
                var query = new GetListGenericQuery<Filiale>();
                filiales = await _mediator.Send(query);
            }
            else
            {
                filiales = await _mediator.Send(new FindGenericQuery<Filiale>(f => EF.Functions.Like(f.Name, $"%{search}%")));
            }

            var filialeDtos = new List<FilialeDTO>();

            foreach (var filiale in filiales)
            {
                var factories = await _mediator.Send(new FindGenericQuery<Factory>(f => f.FKfiliale == filiale.IdFiliale));

                var factoryDtos = factories.Select(f => new FactoryDTO
                {
                    Id = f.IdFactory,
                    Name = f.Name,
                    Address = f.Address,
                    FilialeId = f.FKfiliale,
                    FilialeName = f.Filiale != null ? f.Filiale.Name : string.Empty,
                    ManagerFactory = f.AppUser != null ? f.AppUser.UserName : string.Empty
                }).ToList();

                var filialeDto = new FilialeDTO
                {
                    Id = filiale.IdFiliale,
                    Name = filiale.Name,
                    FactoryCount = factoryDtos.Count,
                    Factories = factoryDtos
                };

                filialeDtos.Add(filialeDto);
            }

            return Ok(filialeDtos);
        }





        [HttpDelete("filiale/{id}")]
        public async Task<IActionResult> DeleteFiliale(Guid id)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr))
            {
                return NotFound("JWT error: user id not found.");
            }

            var filiale = await _mediator.Send(new GetGenericQuery<Filiale, Guid>(id));
            if (filiale == null)
            {
                return NotFound("Filiale not found; nothing to delete.");
            }
            string oldName = filiale.Name;

            // Delete the filiale
            var deleteFilialeCommand = new DeleteGenericCommand<Filiale, Guid>(id);
            bool deleted = await _mediator.Send(deleteFilialeCommand);

            var notification = new Nofication
            {
                IdNofication = Guid.NewGuid(),
                Type = "Info",
                Message = $"La filiale '{oldName}' a été supprimée",
                FKappuser = Guid.Parse(userIdStr),
            };

            await _mediator.Send(new AddGenericCommand<Nofication>(notification));

            if (deleted)
            {
                return Ok(new { Message = "Filiale deleted successfully." });
            }
            else
            {
                return BadRequest(new { Message = "Failed to delete filiale." });
            }
        }



        [HttpPut("filiale/{id}")]
        public async Task<IActionResult> UpdateFiliale(Guid id, [FromBody] CreateFiliale2DTO dto)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr))
            {
                return NotFound("JWT error: user id not found.");
            }

            if (id != dto.Id)
            {
                return BadRequest(new { Message = "The provided Filiale id does not match the payload id." });
            }

            var existingFiliale = await _mediator.Send(new GetGenericQuery<Filiale, Guid>(id));
            if (existingFiliale == null)
            {
                return NotFound("The provided Filiale id does not exist.");
            }

            var oldName = existingFiliale.Name;

            existingFiliale.Name = dto.Name;

            var updateCommand = new PutGenericCommand<Filiale>(existingFiliale);

            var notification = new Nofication
            {
                IdNofication = Guid.NewGuid(),
                Type = "Info",
                Message = $"La filiale '{oldName}' a été modifiée avec le nouveau nom '{dto.Name}'",
                FKappuser = Guid.Parse(userIdStr)
            };

            var notificationResult = await _mediator.Send(new AddGenericCommand<Nofication>(notification));

            var updatedFiliale = await _mediator.Send(updateCommand);
            if (updatedFiliale != null)
            {
                return Ok(new { Message = "Filiale updated successfully.", Filiale = updatedFiliale });
            }
            else
            {
                return BadRequest(new { Message = "Failed to update filiale." });
            }
        }




        [HttpPost("filiale")]
        public async Task<IActionResult> CreateFiliale([FromBody] CreateFilialeDTO dto)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr))
            {
                return NotFound("JWT error: user id not found.");
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var filiale = new Filiale
            {
                IdFiliale = Guid.NewGuid(),
                Name = dto.Name
            };

            var command = new AddGenericCommand<Filiale>(filiale);

            var result = await _mediator.Send(command);

            if (result != null)
            {
                var notification = new Nofication
                {
                    IdNofication = Guid.NewGuid(),
                    Type = "Info",
                    Message = $"Nouvelle filiale  '{dto.Name}' a été créée",
                    FKappuser = Guid.Parse(userIdStr)
                };
                var notificationResult = await _mediator.Send(new AddGenericCommand<Nofication>(notification));
                return Ok(new { Message = "Filiale created successfully.", Filiale = result });
            }
            else
            {
                return BadRequest(new { Message = "Failed to create Filiale." });
            }
        }

        //====================================================================================================================X
        [HttpGet("factories")]
        public async Task<IActionResult> GetFactories([FromQuery] string? search)
        {
            IEnumerable<Factory> factories;
            if (string.IsNullOrEmpty(search))
            {
                var query = new GetListGenericQuery<Factory>();
                factories = await _mediator.Send(query);
            }
            else
            {
                factories = await _mediator.Send(new FindGenericQuery<Factory>(f => EF.Functions.Like(f.Name, $"%{search}%")));
            }

            var factoryDtos = factories.Select(f => new FactoryDTO
            {
                Id = f.IdFactory,
                Name = f.Name,
                Address = f.Address,
                FilialeId = f.FKfiliale,
                FilialeName = f.Filiale != null ? f.Filiale.Name : string.Empty,
                ManagerFactory = f.AppUser != null ? f.AppUser.UserName : string.Empty
            });
            foreach (Factory i in factories)
            {
                Console.WriteLine("XXXXXXXXXXXXXXXXXXXXXXXXXX");
                Console.WriteLine(i.CalculerMoyenneGlobal());
                Console.WriteLine(i.Sx.Count);
                foreach (Sx j in i.Sx)
                {
                    Console.WriteLine(j.NameEnglish);
                    Console.WriteLine(j.Critaires.Count);
                }
                Console.WriteLine("XXXXXXXXXXXXXXXXXXXXXXXXXX");
            }
            return Ok(factoryDtos);
        }



        [HttpPost("factory")]
        public async Task<IActionResult> CreateFactory([FromBody] CreateFactoryDTO dto)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr))
                return NotFound("JWT error: user id not found.");

            var filiale = await _mediator.Send(new GetGenericQuery<Filiale, Guid>(dto.FilialeId));
            if (filiale == null)
                return BadRequest("The provided Filiale id does not exist.");

            var factory = new Factory
            {
                IdFactory = Guid.NewGuid(),
                Name = dto.Name,
                Address = dto.Address,
                FKfiliale = dto.FilialeId
            };
            var createdFactory = await _mediator.Send(new AddGenericCommand<Factory>(factory));
            if (createdFactory == null)
                return BadRequest("Failed to create factory.");

            var sDefinitions = await _mediator.Send(new GetListGenericQuery<SDefinition>());

            foreach (var sDef in sDefinitions)
            {
                var sx = new Sx
                {
                    IdSx = Guid.NewGuid(),
                    NameEnglish = sDef.NameEnglish,
                    NameJaponaise = sDef.NameJaponaise,
                    FKfactory = createdFactory.IdFactory,
                    NumberS = sDef.NumberS
                };
                var createdSx = await _mediator.Send(new AddGenericCommand<Sx>(sx));

                var critereDefs = await _mediator.Send(new FindGenericQuery<CritereDefinition>(
                    cd => cd.FKsxDefinition == sDef.IdSDefinition));

                foreach (var cd in critereDefs)
                {
                    var critaire = new Critaire
                    {
                        IdCritaire = Guid.NewGuid(),
                        Name = cd.Name,
                        Score = 0f,  
                        FKsx = createdSx.IdSx,
                        Key = factory.IdFactory
                    };
                    await _mediator.Send(new AddGenericCommand<Critaire>(critaire));
                }
            }

            var notification = new Nofication
            {
                IdNofication = Guid.NewGuid(),
                Type = "Info",
                Message = $"Nouvelle usine  '{createdFactory.Name}' a été créée.",
                FKappuser = Guid.Parse(userIdStr)
            };
            await _mediator.Send(new AddGenericCommand<Nofication>(notification));

            var factoryDto = new FactoryDTO
            {
                Id = createdFactory.IdFactory,
                Name = createdFactory.Name,
                Address = createdFactory.Address,
                FilialeId = createdFactory.FKfiliale,
                FilialeName = filiale.Name
            };
            return Ok(new { Message = "Factory created successfully with S & Critaire.", Factory = factoryDto });
        }


        [HttpPut("factory/{id}")]
        public async Task<IActionResult> UpdateFactory(Guid id, [FromBody] UpdateFactoryDTO dto)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr))
            {
                return NotFound("JWT error: user id not found.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dto.Id)
            {
                return BadRequest(new { Message = "The id in the route does not match the id in the data." });
            }

            var existingFactory = await _mediator.Send(new GetGenericQuery<Factory, Guid>(id));
            if (existingFactory == null)
            {
                return NotFound("Factory not found.");
            }

            var oldName = existingFactory.Name;

            if (!string.IsNullOrEmpty(dto.Name))
            {
                existingFactory.Name = dto.Name;
            }
            if (!string.IsNullOrEmpty(dto.Address))
            {
                existingFactory.Address = dto.Address;
            }
            if (dto.FilialeId != null && dto.FilialeId != Guid.Empty)
            {
                existingFactory.FKfiliale = (Guid)dto.FilialeId;
            }

            var notification = new Nofication
            {
                IdNofication = Guid.NewGuid(),
                Type = "Info",
                Message = $"La filiale '{oldName}' a été modifiée avec le nouveau nom  '{existingFactory.Name}'",
                FKappuser = Guid.Parse(userIdStr)
            };

            var notificationCommand = new AddGenericCommand<Nofication>(notification);
            var notificationResult = await _mediator.Send(notificationCommand);

            var updateCommand = new PutGenericCommand<Factory>(existingFactory);
            var updatedFactory = await _mediator.Send(updateCommand);

            string filialeName = string.Empty;
            if (updatedFactory.FKfiliale != Guid.Empty)
            {
                var filiale = await _mediator.Send(new GetGenericQuery<Filiale, Guid>(updatedFactory.FKfiliale));
                filialeName = filiale?.Name ?? string.Empty;
            }

            if (updatedFactory != null)
            {
                var factoryDto = new FactoryDTO
                {
                    Id = updatedFactory.IdFactory,
                    Name = updatedFactory.Name,
                    Address = updatedFactory.Address,
                    FilialeId = updatedFactory.FKfiliale,
                    FilialeName = filialeName
                };

                return Ok(new { Message = "Factory updated successfully.", Factory = factoryDto });
            }
            else
            {
                return BadRequest(new { Message = "Failed to update factory." });
            }
        }





        [HttpDelete("factory/{id}")]
        public async Task<IActionResult> DeleteFactory(Guid id)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr))
            {
                return NotFound("JWT error: user id not found.");
            }
            if (!Guid.TryParse(userIdStr, out Guid senderId))
            {
                return Unauthorized("Invalid sender id in token.");
            }

            var factory = await _mediator.Send(new GetGenericQuery<Factory, Guid>(id));
            if (factory == null)
            {
                return BadRequest(new { Message = "Factory not found; failed to delete factory." });
            }

            var planActions = await _mediator.Send(new FindGenericQuery<PlanAction>(p => p.FKfactory == id));
            foreach (var plan in planActions)
            {
                var taches = await _mediator.Send(new FindGenericQuery<Tache>(t => t.FKplanaction == plan.IdPlanAction));
                foreach (var t in taches)
                {
                    await _mediator.Send(new DeleteGenericCommand<Tache, Guid>(t.IdTache));
                }

                await _mediator.Send(new DeleteGenericCommand<PlanAction, Guid>(plan.IdPlanAction));
            }

            var sxList = await _mediator.Send(new FindGenericQuery<Sx>(sx => sx.FKfactory == id));
            foreach (var sx in sxList)
            {
                await _mediator.Send(new DeleteGenericCommand<Sx, Guid>(sx.IdSx));
            }

            var audits = await _mediator.Send(new FindGenericQuery<Audit>(a => a.FKfactory == id));
            foreach (var audit in audits)
            {
                await _mediator.Send(new DeleteGenericCommand<Audit, Guid>(audit.IdAudit));
            }

            var deleteFactoryCommand = new DeleteGenericCommand<Factory, Guid>(id);
            bool factoryDeleted = await _mediator.Send(deleteFactoryCommand);
            if (!factoryDeleted)
            {
                return BadRequest(new { Message = "Failed to delete factory." });
            }

            var notification = new Nofication
            {
                IdNofication = Guid.NewGuid(),
                RoleDes = string.Empty,
                IdDes = Guid.Empty,
                Type = "Info",
                CreatedAt = DateTime.Now,
                Message = $"L usine '{factory.Name}' a été supprimée.",
                FKappuser = senderId
            };

            var notificationCommand = new AddGenericCommand<Nofication>(notification);
            await _mediator.Send(notificationCommand);

            return Ok(new { Message = "Factory and all related data deleted successfully. Notification created." });
        }




        [HttpPut("changeRole")]
        public async Task<IActionResult> ChangeUserRole([FromBody] UpdateRoleDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByIdAsync(dto.Userid.ToString());
            if (user == null)
                return NotFound("User not found.");

            if (!await _roleManager.RoleExistsAsync(dto.NewRole))
            {
                return BadRequest($"The role '{dto.NewRole}' does not exist.");
            }

            if (dto.NewRole == "FactoryM")
            {
                if (dto.FactoryId == Guid.Empty)
                {
                    return BadRequest("Factory id is required for role FactoryM.");
                }

                var factory = await _mediator.Send(new GetGenericQuery<Factory, Guid>((Guid)dto.FactoryId));
                if (factory == null)
                {
                    return BadRequest("The provided Factory id does not exist.");
                }

                var existingManagers = await _mediator.Send(new FindGenericQuery<AppUser>(
                    u => u.Factory != null && u.Factory.IdFactory == dto.FactoryId));
                if (existingManagers.Any())
                {
                    return BadRequest("This factory already has a factory manager assigned.");
                }
                user.FKfactory = dto.FactoryId;
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    return BadRequest(updateResult.Errors);
                }
            }


            var currentRoles = await _userManager.GetRolesAsync(user);

            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                return BadRequest(removeResult.Errors);
            }

            //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
            if (currentRoles.Contains("QualityM"))
            {
                var planActions = await _mediator.Send(new FindGenericQuery<PlanAction>(
                    pa => pa.FKqualitym == user.Id));


                foreach (var planAction in planActions)
                {
                    planAction.FKqualitym = null;
                    await _mediator.Send(new PutGenericCommand<PlanAction>(planAction));
                }
            }

            if (await _userManager.IsInRoleAsync(user, "Auditor"))
            {
                var audits = await _mediator.Send(new FindGenericQuery<Audit>(
                    pa => pa.FKauditor == user.Id));
                var rapports = await _mediator.Send(new FindGenericQuery<Rapport>(
                    pa => pa.FKauditor == user.Id));

                foreach (var audit in audits)
                {
                    audit.FKauditor = null;
                    audit.FKfactory = null;
                    await _mediator.Send(new PutGenericCommand<Audit>(audit));
                    await _mediator.Send(new DeleteGenericCommand<Audit, Guid>(audit.IdAudit));
                }
                foreach (var rapport in rapports)
                {
                    rapport.FKauditor = null;
                    await _mediator.Send(new PutGenericCommand<Rapport>(rapport));
                }
            }
            //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX

            var addResult = await _userManager.AddToRoleAsync(user, dto.NewRole);
            if (!addResult.Succeeded)
            {
                return BadRequest(addResult.Errors);
            }

            return Ok(new { Message = "User role updated successfully." });
        }

        //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX//XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX//XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX

        [HttpGet("S")]
        public async Task<IActionResult> GetAllS()
        {
            var list = await _mediator.Send(new GetListGenericQuery<SDefinition>());
            var sortedList = list.OrderBy(s => s.NumberS).ToList();
            return Ok(sortedList);
        }

        [HttpPost("S")]
        public async Task<IActionResult> Create([FromBody] CreateSDefinitionDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var sDef = new SDefinition
            {
                IdSDefinition = Guid.NewGuid(),
                NameEnglish = dto.NameEnglish,
                NameJaponaise = dto.NameJaponaise
            };

            var result = await _mediator.Send(new AddGenericCommand<SDefinition>(sDef));
            if (result == null)
                return BadRequest(new { Message = "error" });

            return Ok(new { Message = "done", Definition = sDef });
        }

        [HttpPut("S/{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSDefinitionDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (id != dto.Id)
                return BadRequest(new { Message = "not some" });

            var existing = await _mediator.Send(new GetGenericQuery<SDefinition, Guid>(id));
            if (existing == null)
                return NotFound(new { Message = "not exist" });

            if (!string.IsNullOrEmpty(dto.NameEnglish))
                existing.NameEnglish = dto.NameEnglish;
            if (!string.IsNullOrEmpty(dto.NameJaponaise))
                existing.NameJaponaise = dto.NameJaponaise;

            var updated = await _mediator.Send(new PutGenericCommand<SDefinition>(existing));
            if (updated == null)
                return BadRequest(new { Message = "error" });

            return Ok(new { Message = "done bi naja7", Definition = updated });
        }

        [HttpDelete("S/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _mediator.Send(new DeleteGenericCommand<SDefinition, Guid>(id));
            if (!deleted)
                return BadRequest(new { Message = "error" });

            return Ok(new { Message = "doen bi naja7." });
        }


    [HttpGet("Critaire")]
        public async Task<IActionResult> GetAllC()
        {
            var list = await _mediator.Send(new GetListGenericQuery<CritereDefinition>());
            return Ok(list);
        }

        [HttpPost("Critaire")]
        public async Task<IActionResult> Create([FromBody] CreateCritereDefinitionDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var critDef = new CritereDefinition
            {
                IdCritereDefinition = Guid.NewGuid(),
                Name = dto.Name,
                FKsxDefinition = dto.SDefinitionId
            };

            var result = await _mediator.Send(new AddGenericCommand<CritereDefinition>(critDef));
            if (result == null)
                return BadRequest(new { Message = "error" });

            return Ok(new { Message = "tama incha ta3rif", Definition = critDef });
        }


        [HttpPut("Critaire/{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCritereDefinitionDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (id != dto.Id)
                return BadRequest(new { Message = "error ya bro." });

            var existing = await _mediator.Send(new GetGenericQuery<CritereDefinition, Guid>(id));
            if (existing == null)
                return NotFound(new { Message = "not exist" });

            if (!string.IsNullOrEmpty(dto.Name))
                existing.Name = dto.Name;
            if (dto.SxId.HasValue)
                existing.FKsxDefinition = dto.SxId.Value;

            var updated = await _mediator.Send(new PutGenericCommand<CritereDefinition>(existing));
            if (updated == null)
                return BadRequest(new { Message = "error " });

            return Ok(new { Message = "put bi naja7.", Definition = updated });
        }

        [HttpDelete("Critaire/{id:guid}")]
        public async Task<IActionResult> DeleteC(Guid id)
        {
            var deleted = await _mediator.Send(new DeleteGenericCommand<CritereDefinition, Guid>(id));
            if (!deleted)
                return BadRequest(new { Message = "error delete." });

            return Ok(new { Message = "delete critaire bi naja7." });
        }




        [HttpPost("synchronize-factories")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SynchronizeFactoriesPreserveScores()
        {
            var sDefinitions = await _mediator.Send(new GetListGenericQuery<SDefinition>());
            var critereDefinitions = await _mediator.Send(new GetListGenericQuery<CritereDefinition>());

            var factories = await _mediator.Send(new GetListGenericQuery<Factory>());

            foreach (var factory in factories)
            {
                var existingSxList = await _mediator.Send(new FindGenericQuery<Sx>(sx => sx.FKfactory == factory.IdFactory));

                foreach (var sDef in sDefinitions)
                {
                    var sx = existingSxList
                        .FirstOrDefault(x => x.NameEnglish == sDef.NameEnglish);

                    if (sx == null)
                    {
                        sx = new Sx
                        {
                            IdSx = Guid.NewGuid(),
                            NameEnglish = sDef.NameEnglish,
                            NameJaponaise = sDef.NameJaponaise,
                            FKfactory = factory.IdFactory,
                            NumberS = sDef.NumberS
                        };
                        sx = await _mediator.Send(new AddGenericCommand<Sx>(sx));
                    }
                    else
                    {
                        sx.NameJaponaise = sDef.NameJaponaise;
                        await _mediator.Send(new PutGenericCommand<Sx>(sx));
                    }

                    var existingCritList = await _mediator.Send(
                        new FindGenericQuery<Critaire>(c => c.FKsx == sx.IdSx)
                    );

                    var relatedCritDefs = critereDefinitions
                        .Where(cd => cd.FKsxDefinition == sDef.IdSDefinition)
                        .ToList();

                    foreach (var cd in relatedCritDefs)
                    {
                        var crit = existingCritList.FirstOrDefault(c => c.Name == cd.Name);
                        if (crit == null)
                        {
                            var newCrit = new Critaire
                            {
                                IdCritaire = Guid.NewGuid(),
                                Name = cd.Name,
                                Score = 0f,
                                FKsx = sx.IdSx,
                                Key = factory.IdFactory
                            };
                            await _mediator.Send(new AddGenericCommand<Critaire>(newCrit));
                        }
                    }

                    var keptNames = relatedCritDefs.Select(cd => cd.Name).ToHashSet();
                    foreach (var oldCrit in existingCritList)
                    {
                        if (!keptNames.Contains(oldCrit.Name))
                            await _mediator.Send(new DeleteGenericCommand<Critaire, Guid>(oldCrit.IdCritaire));
                    }
                }

                var keptSNames = sDefinitions.Select(sd => sd.NameEnglish).ToHashSet();
                foreach (var oldSx in existingSxList)
                {
                    if (!keptSNames.Contains(oldSx.NameEnglish))
                        await _mediator.Send(new DeleteGenericCommand<Sx, Guid>(oldSx.IdSx));
                }
            }

            return Ok(new { Message = "Synced while preserving old grades." });
        }



        [HttpPut("maintenance")]
        public async Task<IActionResult> SetMaintenanceMode([FromBody] MaintenanceDTO dto)
        {
            Guid a = Guid.Parse("00000000-0000-0000-0000-000000000001");
            var param = await _mediator.Send(new GetGenericQuery<Parametres, Guid>(a));
        

            
            param.Maintenance = dto.Maintenance;
            await _mediator.Send(new PutGenericCommand<Parametres>(param));

            return Ok(new
            {
                Message = "tama ta7dith bi naja7",
                Maintenance = param.Maintenance
            });
        }

        








        public class GlobalScoreDTO
        {
            public string SName { get; set; }  
            public double AverageScore { get; set; }  
        }


        [HttpGet("global-scores")]
        [Authorize]
        public async Task<IActionResult> GetGlobalScores()
        {
            var allSx = await _mediator.Send(new GetListGenericQuery<Sx>());
            var allCrits = await _mediator.Send(new GetListGenericQuery<Critaire>());

            var sxNameById = allSx.ToDictionary(s => s.IdSx, s => s.NameEnglish);

            var globalAverages = allCrits
                .Where(c => c.FKsx.HasValue && sxNameById.ContainsKey(c.FKsx.Value))
                .GroupBy(c => sxNameById[c.FKsx.Value])
                .Select(g => new GlobalScoreDTO
                {
                    SName = g.Key,
                    AverageScore = g.Average(c => c.Score)
                })
                .ToList();

            return Ok(globalAverages);
        }

    }
}
