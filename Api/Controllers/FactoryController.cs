using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography;
using Domain.Commands;
using Domain.DTOs.Admin;
using Domain.DTOs.Audit;
using Domain.DTOs.Factory;
using Domain.DTOs.Filiale;
using Domain.DTOs.Notification;
using Domain.DTOs.PlanAction;
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
    [Authorize(Roles = "FactoryM")]
    public class FactoryController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IMediator _mediator;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IEmailSender _emailSender;

        public FactoryController(IEmailSender emailSender, UserManager<AppUser> userManager, IConfiguration configuration, IMediator mediator, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _userManager = userManager;
            _configuration = configuration;
            _mediator = mediator;
            _roleManager = roleManager;
            _emailSender = emailSender;
        }



        [HttpGet("planActions")]
        [Authorize]
        public async Task<IActionResult> GetPlanActionsForFactoryM()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out Guid userId))
            {
                return Unauthorized("Invalid user id.");
            }

            var currentUser = await _userManager.FindByIdAsync(userIdStr);
            if (currentUser == null)
            {
                return Unauthorized("User not found.");
            }

            if (currentUser.FKfactory == null)
            {
                return BadRequest("No factory information found for the current user.");
            }

            Guid factoryId = (Guid)currentUser.FKfactory;

            var planActions = await _mediator.Send(new FindGenericQuery<PlanAction>(
                pa => pa.FKfactory == factoryId
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

            var planActionDtos = planActions.Select(pa => new PlanActionDTO
            {
                Id = pa.IdPlanAction,
                Name = pa.Name,
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
            }).ToList();

            return Ok(planActionDtos);
        }



        //[HttpGet("rapports")]
        //[Authorize]
        //public async Task<IActionResult> GetRapports()
        //{
        //    var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //    if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out Guid userId))
        //    {
        //        return Unauthorized("Invalid user id.");
        //    }
        //    var user0 = await _userManager.FindByIdAsync(userIdStr);
        //    var rapports = await _mediator.Send(new FindGenericQuery<Rapport>(
        //        a => a.FKauditor.HasValue
        //             && a.FKfactory == user0.FKfactory
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
        [HttpGet("rapports")]
        [Authorize]
        public async Task<IActionResult> GetAuditorRapports()
        {
            var auditorIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(auditorIdStr) || !Guid.TryParse(auditorIdStr, out Guid auditorId))
                return Unauthorized("Invalid auditor id.");
            var user0 = await _mediator.Send(new GetGenericQuery<AppUser, Guid>(auditorId));
            var rapports = await _mediator.Send(new FindGenericQuery<Rapport>(
                    a => a.FKauditor.HasValue
                         && a.FKfactory == user0.FKfactory
                         && a.FKfactory != Guid.Empty && a.FKfactory != null));

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


        [HttpPut("completeTache")]
        public async Task<IActionResult> CompleteTache([FromQuery] Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Invalid tache id.");
            }

            var tache = await _mediator.Send(new GetGenericQuery<Tache, Guid>(id));
            if (tache == null)
            {
                return NotFound("Tache not found.");
            }

            tache.IsDone = true;

            var updateCommand = new PutGenericCommand<Tache>(tache);
            var updatedTache = await _mediator.Send(updateCommand);

            if (updatedTache != null)
            {
                return Ok("Tache completed successfully.");
            }
            else
            {
                return BadRequest(new { Message = "Failed to update tache." });
            }
        }


        [HttpPut("completePlanAction")]
        public async Task<IActionResult> CompletePlanAction([FromQuery] Guid id)
        {

            if (id == Guid.Empty)
            {
                return BadRequest("Invalid planAction id.");
            }


            var planAction = await _mediator.Send(new GetGenericQuery<PlanAction, Guid>(id));
            if (planAction == null)
            {
                return NotFound("PlanAction not found.");
            }


            if (planAction.Taches == null || !planAction.Taches.Any())
            {

                var taches = await _mediator.Send(new FindGenericQuery<Tache>(t => t.FKplanaction == planAction.IdPlanAction));
                planAction.Taches = taches.ToList();
            }

            bool allTasksCompleted = planAction.Taches.All(t => t.IsDone);
            if (!allTasksCompleted)
            {
                return BadRequest("Not all tasks are completed. Please complete all tasks before marking the planAction as complete.");
            }

            planAction.IsDone = true;
            var updateCommand = new PutGenericCommand<PlanAction>(planAction);
            var updatedPlanAction = await _mediator.Send(updateCommand);

            if (updatedPlanAction != null)
            {
                return Ok("PlanAction marked as complete successfully.");
            }
            else
            {
                return BadRequest("Failed to update planAction.");
            }
        }
        [HttpGet("factory-users-ranking")]
        public async Task<IActionResult> GetFactoryUsersRanking()
        {
            var factoryUsers = await _userManager.GetUsersInRoleAsync("FactoryM");
            if (factoryUsers == null || !factoryUsers.Any())
            {
                return NotFound("No users with the role 'FactoryM' were found.");
            }

            var rankingList = new List<UserRankingDTO>();

            foreach (var user in factoryUsers)
            {
                
                if (user.FKfactory == null || user.FKfactory == Guid.Empty)
                    continue;
                
                var factory = await _mediator.Send(new GetGenericQuery<Factory, Guid>((Guid)user.FKfactory));
                if (factory == null)
                    continue;
                Console.WriteLine(user.UserName);
                Console.WriteLine(user.UserName);
                Console.WriteLine(user.UserName);
                Console.WriteLine(user.UserName);
                Console.WriteLine(user.UserName);

                var sxs = await _mediator.Send(new FindGenericQuery<Sx>(s => s.FKfactory == factory.IdFactory));
                var sxIds = sxs.Select(s => s.IdSx).ToList();

                var criteres = await _mediator.Send(new FindGenericQuery<Critaire>(c => c.FKsx.HasValue && sxIds.Contains(c.FKsx.Value)));

                double averageScore = criteres.Any() ? criteres.Average(c => c.Score) : 0;

                rankingList.Add(new UserRankingDTO
                {
                    UserId = user.Id,
                    UserName = $"{user.First_name} {user.Last_name}",
                    FactoryName = factory.Name,
                    AverageScore = averageScore
                });
            }

            return Ok(rankingList);
        }


        [HttpGet("factory-stats")]
        [Authorize(Roles = "FactoryM")]
        public async Task<IActionResult> GetFactoryStats()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out Guid userId))
            {
                return Unauthorized("jwt error .");
            }

            var user = await _userManager.FindByIdAsync(userIdStr);
            if (user == null)
            {
                return Unauthorized("user not exist .");
            }

            if (user.IdFactory == null)
            {
                return BadRequest("you don t have factory w t7ib 3al stat ?.");
            }

            var factory = await _mediator.Send(new GetGenericQuery<Factory, Guid>((Guid)user.FKfactory));
            if (factory == null)
            {
                return NotFound("factory not exist you can t resist.");
            }

            var rapports = await _mediator.Send(new FindGenericQuery<Rapport>(r => r.FKfactory == factory.IdFactory));
            

            var planActions = await _mediator.Send(new FindGenericQuery<PlanAction>(p => p.FKfactory == factory.IdFactory));
            var planActionIds = planActions.Select(pa => pa.IdPlanAction).ToList();
            var taches = await _mediator.Send(new FindGenericQuery<Tache>(t => planActionIds.Contains(t.FKplanaction)));
            int totalTasks = taches.Count();
            int doneTasks = taches.Count(t => t.IsDone);
            int notDoneTasks = taches.Count(t => !t.IsDone);

            var criteres = await _mediator.Send(new FindGenericQuery<Critaire>(c => c.Key == factory.IdFactory));
            double averageScore = criteres.Any() ? criteres.Average(c => c.Score) : 0;

            var result = new StatfactoryDTO
            {
                FactoryName = factory.Name,
                FactoryAddress = factory.Address,
                Rapports = rapports.Count(),
                TotalPlanActions = planActions.Count(),
                AverageScore = averageScore,
                TotalTasksToDo = totalTasks,
                TotalTasksCompleted = doneTasks,
                TotalTasksNotCompleted = notDoneTasks
            };

            return Ok(result);
        }


        //[HttpGet("factory-Evolution")]
        //[Authorize(Roles = "FactoryM")]
        //public async Task<IActionResult> GetFactoryE()
        //{
        //    var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out Guid userId))
        //    {
        //        return Unauthorized("jwt error .");
        //    }
        //    var user = await _userManager.FindByIdAsync(userIdStr);
        //    if (user == null)
        //    {
        //        return Unauthorized("user not exist .");
        //    }
        //    if (user.IdFactory == null)
        //    {
        //        return BadRequest("you don t have factory w t7ib 3al stat ?.");
        //    }
        //    var factory = await _mediator.Send(new GetGenericQuery<Factory, Guid>((Guid)user.FKfactory));
        //    if (factory == null)
        //    {
        //        return NotFound("factory not exist you can t resist.");
        //    }

        //    var rapports = await _mediator.Send(new FindGenericQuery<Historique>(r => r.FKfactory == factory.IdFactory));
        //    var RapportDtos = new List<EvolutionDTO>();
        //    foreach (var r in rapports)
        //    {
        //        var score = (r.s1.Average() + r.s2.Average() + r.s3.Average() + r.s4.Average() + r.s5.Average())/5;
        //        RapportDtos.Add(new EvolutionDTO
        //        {
        //            CreatedTtime = r.CreatedAt,
        //            ScoreM5S = score
        //        });
        //    }


        //    return Ok(RapportDtos);
        //}

        [HttpPut("tache/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateTache(Guid id, [FromBody] UpdateTacheDTO2 dto)
        {
            var tache = await _mediator.Send(new GetGenericQuery<Tache, Guid>(id));
            if (tache == null)
                return NotFound(new { Message = "Tache non trouvée." });

            if (dto.Pictures != null && dto.Pictures.Any())
            {
                foreach (var b64 in dto.Pictures)
                {
                    if (string.IsNullOrWhiteSpace(b64))
                        continue;

                    try
                    {
                        var bytes = Convert.FromBase64String(b64);
                        tache.Pictures.Add(bytes);
                    }
                    catch (FormatException)
                    {
                        return BadRequest(new { Message = "Format Base64 invalide dans l'une des images." });
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(dto.Commantaire))
            {
                tache.Commantaire = dto.Commantaire;
            }

            var updated = await _mediator.Send(new PutGenericCommand<Tache>(tache));
            if (updated == null)
                return BadRequest(new { Message = "Échec de la mise à jour de la tâche." });

            return Ok(new
            {
                Message = "Tâche mise à jour avec succès.",
                Tache = new
                {
                    updated.IdTache,
                    updated.Name,
                    updated.IsDone,
                    PicturesCount = updated.Pictures.Count,
                    updated.Commantaire
                }
            });
        }




    }
}

