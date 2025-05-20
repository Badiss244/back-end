using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography;
using Domain.Commands;
using Domain.DTOs.Audit;
using Domain.DTOs.Factory;
using Domain.DTOs.Filiale;
using Domain.DTOs.Notification;
using Domain.DTOs.PlanAction;
using Domain.DTOs.QualityM;
using Domain.DTOs.Rapport;
using Domain.DTOs.Tache;
using Domain.Interface;
using Domain.Models;
using Domain.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "QualityM")]
    public class QualityController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IMediator _mediator;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IEmailSender _emailSender;

        public QualityController(IEmailSender emailSender, UserManager<AppUser> userManager, IConfiguration configuration, IMediator mediator, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _userManager = userManager;
            _configuration = configuration;
            _mediator = mediator;
            _roleManager = roleManager;
            _emailSender = emailSender;
        }


        [HttpPost("planAction")]
        public async Task<IActionResult> CreatePlanAction([FromBody] CreatePlanActionDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var qualityIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(qualityIdStr) || !Guid.TryParse(qualityIdStr, out Guid qualityId))
            {
                return Unauthorized("Invalid Token");
            }

            if (dto.IdFactory == Guid.Empty)
                return BadRequest("A valid Factory id is required.");

            var factory = await _mediator.Send(new GetGenericQuery<Factory, Guid>(dto.IdFactory));
            var rapport0 = await _mediator.Send(new GetGenericQuery<Rapport, Guid>(dto.IdRapport));
            if (factory == null)
                return BadRequest("The provided Factory id does not exist.");
            if (rapport0 == null)
                return BadRequest("The provided Rapport id does not exist.");

            var appUsers = await _mediator.Send(new FindGenericQuery<AppUser>(
            u => u.Factory != null && u.Factory.IdFactory == factory.IdFactory
            ));
            var appUser = appUsers.FirstOrDefault();

            if (appUser == null)
            {
                return NotFound("No AppUser found for the provided factory.");
            }

            Rapport rapport = null;
            if (dto.IdRapport != Guid.Empty)
            {
                rapport = await _mediator.Send(new GetGenericQuery<Rapport, Guid>(dto.IdRapport));
                if (rapport == null)
                    return BadRequest("The provided Rapport id does not exist.");
            }

            var taches = new List<Tache>();
            if (dto.Taches != null)
            {
                foreach (var tacheDto in dto.Taches)
                {
                    taches.Add(new Tache
                    {
                        Name = tacheDto.Name,
                        NameS = tacheDto.NameS
                    });
                }
            }

            var planAction = new PlanAction
            {
                IdPlanAction = Guid.NewGuid(),
                Name = dto.Name,
                FKqualitym = qualityId,
                FKfactory = factory.IdFactory,
                FKrapport = dto.IdRapport,
                Taches = taches
            };

            var command = new AddGenericCommand<PlanAction>(planAction);
            var result = await _mediator.Send(command);

            if (result != null)
            {
                rapport0.X = qualityId;
                rapport0.IsDone = true;
                var updatedRapport = await _mediator.Send(new PutGenericCommand<Rapport>(rapport0));
                var notification = new Nofication
                {
                    IdNofication = Guid.NewGuid(),
                    IdDes = appUser.Id,
                    Type = "Info",
                    Message = $" 3andik planAction jdid '{dto.Name}' tzad (MSG hatha taw nbadlo)",
                    FKappuser = Guid.Parse(qualityIdStr)
                };
                var notificationResult = await _mediator.Send(new AddGenericCommand<Nofication>(notification));
                return Ok(new { Message = "PlanAction created successfully." });
            }
            else
            {
                return BadRequest(new { Message = "Failed to create PlanAction." });
            }
        }




        [HttpGet("planActions")]
        [Authorize]
        public async Task<IActionResult> GetPlanActionsForCurrentUser()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out Guid userId))
            {
                return Unauthorized("Invalid user id.");
            }

            var planActions = await _mediator.Send(new FindGenericQuery<PlanAction>(
                pa => pa.FKqualitym == userId
            ));

            foreach (var pa in planActions)
            {
                if (pa.Taches == null || !pa.Taches.Any())
                {
                    var taches = await _mediator.Send(new FindGenericQuery<Tache>(
                        t => t.FKplanaction == pa.IdPlanAction
                    ));
                    pa.Taches = taches.ToList();
                }
            }

            var planActionDtos = new List<PlanActionDTO>();
            foreach (var pa in planActions)
            {
                var factory = await _mediator.Send(new GetGenericQuery<Factory, Guid>(pa.FKfactory));
                var factoryName = factory != null ? factory.Name : "N/A";
                var factoryAddress = factory != null ? factory.Address : "N/A";
                
                var dto = new PlanActionDTO
                {
                    Id = pa.IdPlanAction,
                    Name = pa.Name,
                    factory = factoryName  + " | " + factoryAddress,
                    IsDone = pa.IsDone,
                    taches = pa.Taches.Select(t => new TacheDTO
                    {
                        Id = t.IdTache,
                        Name = t.Name,
                        NameS = t.NameS,
                        IsDone = t.IsDone,
                        Commantaire = t.Commantaire,
                        Pictures = t.Pictures?
                            .Select(b => Convert.ToBase64String(b))
                            .ToList()
                            ?? new List<string>()
                    }).ToList()
                };
                planActionDtos.Add(dto);
            }

            return Ok(planActionDtos);
        }



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
            return Ok(factoryDtos);
        }


        [HttpGet("rapports")]
        [Authorize]
        public async Task<IActionResult> GetAuditorRapports()
        {
            var auditorIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(auditorIdStr) || !Guid.TryParse(auditorIdStr, out Guid auditorId))
                return Unauthorized("Invalid auditor id.");

            var rapports = await _mediator.Send(new FindGenericQuery<Rapport>(
                a => a.FKauditor.HasValue
                     && (a.IsDone == false || a.X == auditorId)
                     && (a.FKfactory != Guid.Empty  && a.FKfactory != null)));

            var result = new List<RapportDTO>();

            foreach (var rapport in rapports)
            {
                if (rapport.Evidence == null || !rapport.Evidence.Any())
                    rapport.Evidence = (await _mediator.Send(new FindGenericQuery<Evidence>(
                        e => e.FKrapport == rapport.IdRapport))).ToList();

                var pictures = rapport.Evidence
                    .Where(e => e.Picture?.Length > 0)
                    .Select(e => Convert.ToBase64String(e.Picture))
                    .ToList();

                var factory = await _mediator.Send(new GetGenericQuery<Factory, Guid>(rapport.FKfactory.Value));
                var factoryName = factory?.Name ?? "N/A";
                var factoryAddress = factory?.Address ?? "N/A";

                var sxs = (await _mediator.Send(new FindGenericQuery<Sx>(
                    sx => sx.FKfactory == rapport.FKfactory)))
                    .OrderBy(sx => sx.NumberS)
                    .ToList();

                var sxIds = sxs.Select(sx => sx.IdSx).ToList();

                var criteres = await _mediator.Send(new FindGenericQuery<Critaire>(
                    c => c.FKsx.HasValue && sxIds.Contains(c.FKsx.Value)));

                var scores = new List<ScoreCritereDTO>();
                foreach (var sx in sxs)
                {
                    var critsForThisS = criteres
                        .Where(c => c.FKsx == sx.IdSx)
                        .ToList();

                    foreach (var c in critsForThisS)
                    {
                        scores.Add(new ScoreCritereDTO
                        {
                            CritereId = c.IdCritaire,
                            Name = c.Name,
                            Score = c.Score,
                            SName = $"{sx.NameEnglish} ({sx.NameJaponaise})"
                        });
                    }
                }

                var user = await _mediator.Send(new GetGenericQuery<AppUser, Guid>(rapport.FKauditor.Value));
                var auditorName = user?.UserName ?? "Unknown";

                result.Add(new RapportDTO
                {
                    Id = rapport.IdRapport,
                    AuditorName = auditorName,
                    Factory = factoryName + " | " + factoryAddress,
                    Description = rapport.Description,
                    CreatedDate = rapport.CreatedAt,
                    Pictures = pictures,
                    Scores = scores
                });
            }

            return Ok(result);
        }

        //[HttpGet("rapports")]
        //[Authorize]
        //public async Task<IActionResult> GetAuditorRapports()
        //{
        //    var IdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    if (string.IsNullOrEmpty(IdStr) || !Guid.TryParse(IdStr, out Guid auditorId))
        //    {
        //        return Unauthorized("Invalid Token");
        //    }
        //    var rapports = await _mediator.Send(new FindGenericQuery<Rapport>(
        //        a => a.FKauditor.HasValue
        //             && (a.IsDone == false || a.X == auditorId)
        //             && a.FKfactory != Guid.Empty));

        //    var rapportDtos = new List<RapportDTO>();

        //    foreach (var rapport in rapports)
        //    {
        //        if (rapport.FKfactory == null)
        //            continue;

        //        string auditorName = "Unknown";
        //        if (rapport.FKauditor.HasValue)
        //        {
        //            var user = await _mediator.Send(new GetGenericQuery<AppUser, Guid>(rapport.FKauditor.Value));
        //            if (user != null)
        //            {
        //                auditorName = user.UserName;
        //            }
        //        }

        //        if (rapport.Evidence == null || !rapport.Evidence.Any())
        //        {
        //            var evidences = await _mediator.Send(new FindGenericQuery<Evidence>(
        //                e => e.FKrapport == rapport.IdRapport));
        //            rapport.Evidence = evidences.ToList();
        //        }

        //        var pictures = rapport.Evidence?
        //            .Where(e => e.Picture != null && e.Picture.Length > 0)
        //            .Select(e => Convert.ToBase64String(e.Picture))
        //            .ToList() ?? new List<string>();

        //        var criteres = await _mediator.Send(new FindGenericQuery<Critaire>(
        //            c => c.Key == rapport.FKfactory));
        //        List<int> combinedList = new List<int>();
        //        var factory = await _mediator.Send(new GetGenericQuery<Factory, Guid>((Guid)rapport.FKfactory));
        //        foreach (var crit in criteres)
        //        {
        //            combinedList.Add((int)crit.Score);
        //        }

        //        List<int> s1 = combinedList.Count >= 2 ? new List<int> { combinedList[0], combinedList[1] } : new List<int>();
        //        List<int> s2 = combinedList.Count >= 5 ? new List<int> { combinedList[2], combinedList[3], combinedList[4] } : new List<int>();
        //        List<int> s3 = combinedList.Count >= 8 ? new List<int> { combinedList[5], combinedList[6], combinedList[7] } : new List<int>();
        //        List<int> s4 = combinedList.Count >= 11 ? new List<int> { combinedList[8], combinedList[9], combinedList[10] } : new List<int>();
        //        List<int> s5 = combinedList.Count >= 15 ? new List<int> { combinedList[11], combinedList[12], combinedList[13], combinedList[14] } : new List<int>();

        //        var dto = new RapportDTO
        //        {
        //            Id = rapport.IdRapport,
        //            AuditorName = auditorName,
        //            Description = rapport.Description,
        //            Factory = factory?.Name ?? "N/A",
        //            Pictures = pictures,
        //            CreatedDate = rapport.CreatedAt,
        //            s1 = s1,
        //            s2 = s2,
        //            s3 = s3,
        //            s4 = s4,
        //            s5 = s5
        //        };

        //        rapportDtos.Add(dto);
        //    }

        //    return Ok(rapportDtos);
        //}

        [HttpDelete("planAction/{id}")]
        [Authorize]
        public async Task<IActionResult> DeletePlanAction(Guid id)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out Guid userId))
            {
                return Unauthorized("Invalid user id in token.");
            }

            var planAction = await _mediator.Send(new GetGenericQuery<PlanAction, Guid>(id));
            if (planAction == null)
            {
                return NotFound("PlanAction not found.");
            }
            if (planAction.FKqualitym != userId)
            {
                return Forbid("You are not authorized to delete this PlanAction.");
            }
            bool deletionResult = await _mediator.Send(new DeleteGenericCommand<PlanAction, Guid>(id));
            if (deletionResult)
            {
                return Ok(new { Message = "PlanAction deleted successfully." });
            }
            else
            {
                return BadRequest(new { Message = "Failed to delete PlanAction." });
            }
        }


        [HttpPut("planAction/name/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdatePlanActionName(Guid id, [FromBody] PutPlanActionDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if(id != dto.Id)
            {
                return BadRequest("id query w id put mch kifkif");
            }
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out Guid qualityId))
            {
                return Unauthorized("Invalid or missing token.");
            }

            var existingPlanAction = await _mediator.Send(new GetGenericQuery<PlanAction, Guid>(id));
            if (existingPlanAction == null)
            {
                return NotFound("PlanAction not found.");
            }

            if (existingPlanAction.FKqualitym != qualityId)
            {
                return Forbid("You are not authorized to update this PlanAction.");
            }

            existingPlanAction.Name = dto.Name;

            var updateCommand = new PutGenericCommand<PlanAction>(existingPlanAction);
            var updatedPlanAction = await _mediator.Send(updateCommand);

            if (updatedPlanAction != null)
            {
                var resultDto = new PlanActionDTO
                {
                    Id = updatedPlanAction.IdPlanAction,
                    Name = updatedPlanAction.Name,
                    factory = (await _mediator.Send(new GetGenericQuery<Factory, Guid>(updatedPlanAction.FKfactory)))?.Name ?? "N/A",
                    taches = updatedPlanAction.Taches.Select(t => new TacheDTO
                    {
                        Id = t.IdTache,
                        Name = t.Name,
                        IsDone = t.IsDone
                    }).ToList()
                };

                return Ok(new { Message = "PlanAction name updated successfully.", PlanAction = resultDto });
            }
            else
            {
                return BadRequest(new { Message = "Failed to update PlanAction name." });
            }
        }

        [HttpPut("tache/{id}")]
        [Authorize(Roles = "QualityM")]
        public async Task<IActionResult> UpdateTache(Guid id, [FromBody] UpdateTacheDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (id != dto.Id)
            {
                return BadRequest("id query w id put mch kifkif");
            }
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out Guid qualityId))
            {
                return Unauthorized("Invalid or missing token.");
            }

            var existingTache = await _mediator.Send(new GetGenericQuery<Tache, Guid>(id));
            if (existingTache == null)
            {
                return NotFound("Tache not found.");
            }
            if (!string.IsNullOrEmpty(dto.Name))
            {
                existingTache.Name = dto.Name;
            }
            if (!string.IsNullOrEmpty(dto.NameS))
            {
                existingTache.NameS = dto.Name;
            }

            var updateCommand = new PutGenericCommand<Tache>(existingTache);
            var updatedTache = await _mediator.Send(updateCommand);

            if (updatedTache != null)
            {
                var resultDto = new TacheDTO
                {
                    Id = updatedTache.IdTache,
                    Name = updatedTache.Name
                };

                return Ok(new { Message = "Tache updated successfully.", Tache = resultDto });
            }
            else
            {
                return BadRequest(new { Message = "Failed to update Tache." });
            }
        }


        [HttpDelete("tache/{id}")]
        [Authorize(Roles = "QualityM")]
        public async Task<IActionResult> DeleteTache(Guid id)
        {

            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out Guid qualityId))
            {
                return Unauthorized("Invalid or missing token.");
            }

            var existingTache = await _mediator.Send(new GetGenericQuery<Tache, Guid>(id));
            if (existingTache == null)
            {
                return NotFound("Tache not found.");
            }

            var deleteCommand = new DeleteGenericCommand<Tache, Guid>(id);
            bool deleted = await _mediator.Send(deleteCommand);

            if (deleted)
            {
                return Ok(new { Message = "Tache deleted successfully." });
            }
            else
            {
                return BadRequest(new { Message = "Failed to delete Tache." });
            }
        }


        [HttpPost("planAction/NTache")]
        [Authorize]
        public async Task<IActionResult> AddTacheToPlanAction([FromBody] AddTacheToPa dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var planAction = await _mediator.Send(new GetGenericQuery<PlanAction, Guid>(dto.IdPlanAction));
            if (planAction == null)
                return NotFound("PlanAction not found.");

            var tache = new Tache
            {
                IdTache = Guid.NewGuid(),
                Name = dto.Name,
                IsDone = false,
                NameS = dto.NameS,
                FKplanaction = dto.IdPlanAction
            };

            var command = new AddGenericCommand<Tache>(tache);
            var addedTache = await _mediator.Send(command);

            if (addedTache != null)
            {
                return Ok(new { Message = "Tache added to PlanAction successfully." });
            }
            else
            {
                return BadRequest(new { Message = "Failed to add Tache to PlanAction." });
            }
        }

        [HttpGet("dashboard-stats")]
        [Authorize(Roles = "QualityM")]
        public async Task<IActionResult> GetQualityDashboardStats()
        {
            var qualityIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(qualityIdStr) || !Guid.TryParse(qualityIdStr, out Guid qualityId))
            {
                return Unauthorized("error jwt.");
            }

            var planActions = await _mediator.Send(new FindGenericQuery<PlanAction>(pa => pa.FKqualitym == qualityId));
            int totalPlanActions = planActions.Count();

            var planActionIds = planActions.Select(pa => pa.IdPlanAction).ToList();
            var taches = await _mediator.Send(new FindGenericQuery<Tache>(t => planActionIds.Contains(t.FKplanaction)));
            int totalTasks = taches.Count();
            int doneTasks = taches.Count(t => t.IsDone);
            int notDoneTasks = taches.Count(t => !t.IsDone);

            var rapports = await _mediator.Send(new FindGenericQuery<Rapport>(r => r.IsDone==false));
            int totalRapports = rapports.Count();

            var auditors = await _userManager.GetUsersInRoleAsync("Auditor");
            int totalAuditors = auditors.Count;

            var factories = await _mediator.Send(new GetListGenericQuery<Factory>());
            int totalFactories = factories.Count();
            var result = new StatQualityDTO
            {
                TotalPlanActions = totalPlanActions,
                TotalTasks = totalTasks,
                DoneTasks = doneTasks,
                NotDoneTasks = notDoneTasks,
                TotalRapports = totalRapports,
                TotalAuditors = totalAuditors,
                TotalFactories = totalFactories
            };

            return Ok(result);
        }


        [HttpGet("sdefinitions")]
        [Authorize]
        public async Task<IActionResult> GetSDefinitions()
        {
            var sDefinitions = await _mediator.Send(new GetListGenericQuery<SDefinition>());

            var result = sDefinitions
                .Select(s => new
                {
                    Id = s.IdSDefinition,
                    NameEnglish = s.NameEnglish,
                    NameJaponaise = s.NameJaponaise
                })
                .ToList();

            return Ok(result);
        }
    }




}


