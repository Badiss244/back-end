using System.Security.Claims;
using Domain.Commands;
using Domain.DTOs.Admin;
using Domain.DTOs.Audit;
using Domain.DTOs.Auditor;
using Domain.DTOs.Factory;
using Domain.DTOs.Rapport;
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
    [Authorize(Roles = "Auditor")]
    public class AuditorController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IMediator _mediator;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IEmailSender _emailSender;

        public AuditorController(IEmailSender emailSender, UserManager<AppUser> userManager, IConfiguration configuration, IMediator mediator, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _userManager = userManager;
            _configuration = configuration;
            _mediator = mediator;
            _roleManager = roleManager;
            _emailSender = emailSender;
        }

        [HttpPost("createAudit")]
        [Authorize]
        public async Task<IActionResult> CreateAudit([FromBody] CreateAuditDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var auditorIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(auditorIdStr) || !Guid.TryParse(auditorIdStr, out Guid auditorId))
            {
                return Unauthorized("Invalid auditor id.");
            }
            var factory = await _mediator.Send(new GetGenericQuery<Factory, Guid>(model.FKfactory));
            if (factory == null)
            {
                return BadRequest("The provided Factory id does not exist.");
            }
            var audit = new Audit
            {
                IdAudit = Guid.NewGuid(),
                PlanDate = model.PlanDate,
                FKfactory = model.FKfactory,
                FKauditor = auditorId
            };
            var command = new AddGenericCommand<Audit>(audit);
            var result = await _mediator.Send(command);
            if (result != null)
            {
                var user = (await _mediator.Send(new FindGenericQuery<AppUser>(
                    u => u.Factory != null && u.Factory.IdFactory == factory.IdFactory)))
                    .FirstOrDefault();
                if (user == null)
                {
                    return Ok(new { Message = "Audit created successfully, but no factory manager was found."});
                }
                var notification = new Nofication
                {
                    IdNofication = Guid.NewGuid(),
                    IdDes = user.Id,
                    Type = "Info",
                    Message = $"Tu as un nouvel audit planifié le {audit.CreatedAt}",
                    FKappuser = Guid.Parse(auditorIdStr)
                };
                var command2 = new AddGenericCommand<Nofication>(notification);
                var result2 = await _mediator.Send(command2);
                if (result2 == null)
                {
                    return BadRequest(new { Message = "Failed to create Notification." });
                }
                var auditDtoCreated = new AuditDTO
                {
                    IdAudit = audit.IdAudit,
                    CreatedAt = audit.CreatedAt,
                    PlanDate = audit.PlanDate,
                    Status = audit.Status
                };
                return Ok(new { Message = "Audit created successfully.", Audit = auditDtoCreated });
            }
            else
            {
                return BadRequest(new { Message = "Failed to create audit." });
            }
        }
        [HttpGet("audits")]
        [Authorize]
        public async Task<IActionResult> GetAuditsForAuditor()
        {
            var auditorIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(auditorIdStr) || !Guid.TryParse(auditorIdStr, out Guid auditorId))
            {
                return Unauthorized("Invalid auditor id.");
            }

            var audits = await _mediator.Send(new FindGenericQuery<Audit>(
                a => a.FKauditor.HasValue && a.FKauditor.Value == auditorId && a.IsOld == false));
            foreach (var audit in audits)
            {
                DateTime planDateTime = audit.PlanDate.ToDateTime(new TimeOnly(0, 0));
                bool shouldUpdate = false;
                if (audit.Status == "Completed" || audit.Status == "Canceled")
                {
                    audit.IsOld = true;
                    shouldUpdate = true;
                }
                if (DateTime.Now > planDateTime.AddDays(2))
                {
                    audit.IsOld = true;
                    shouldUpdate = true;
                }
                if (shouldUpdate)
                {
                    await _mediator.Send(new PutGenericCommand<Audit>(audit));
                }

            }

            var auditDtoList = new List<AuditDTO>();

            foreach (var n in audits)
            {
                string factoryName = string.Empty;
                string addressName = string.Empty;
                string filialeName = string.Empty;

                if (n.FKfactory.HasValue)
                {
                    var factory = await _mediator.Send(new GetGenericQuery<Factory, Guid>(n.FKfactory.Value));
                    if (factory != null)
                    {
                        factoryName = factory.Name;
                        addressName = factory.Address;
                        if (factory.Filiale != null)
                            filialeName = factory.Filiale.Name;
                        else if (factory.FKfiliale != Guid.Empty)
                        {
                            var filiale = await _mediator.Send(new GetGenericQuery<Filiale, Guid>(factory.FKfiliale));
                            filialeName = filiale?.Name ?? string.Empty;
                        }
                    }
                }

                auditDtoList.Add(new AuditDTO
                {
                    IdAudit = n.IdAudit,
                    CreatedAt = n.CreatedAt,
                    PlanDate = n.PlanDate,
                    Status = n.Status,
                    Factory = factoryName + " | " + addressName,
                    Filiale = filialeName
                });
            }

            return Ok(auditDtoList);
        }

        [HttpGet("Historique_audits")]
        [Authorize]
        public async Task<IActionResult> GetHAudits()
        {
            var auditorIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(auditorIdStr) || !Guid.TryParse(auditorIdStr, out Guid auditorId))
            {
                return Unauthorized("Invalid auditor id.");
            }

            var audits = await _mediator.Send(new FindGenericQuery<Audit>(
                a => a.FKauditor.HasValue && a.FKauditor.Value == auditorId && a.IsOld == true));
            var auditDtoList = new List<AuditDTO>();
            foreach (var n in audits)
            {
                string factoryName = string.Empty;
                string addressName = string.Empty;
                string filialeName = string.Empty;

                if (n.FKfactory.HasValue)
                {
                    var factory = await _mediator.Send(new GetGenericQuery<Factory, Guid>(n.FKfactory.Value));
                    if (factory != null)
                    {
                        factoryName = factory.Name;
                        addressName = factory.Address;
                        if (factory.Filiale != null)
                            filialeName = factory.Filiale.Name;
                        else if (factory.FKfiliale != Guid.Empty)
                        {
                            var filiale = await _mediator.Send(new GetGenericQuery<Filiale, Guid>(factory.FKfiliale));
                            filialeName = filiale?.Name ?? string.Empty;
                        }
                    }
                }

                auditDtoList.Add(new AuditDTO
                {
                    IdAudit = n.IdAudit,
                    CreatedAt = n.CreatedAt,
                    PlanDate = n.PlanDate,
                    Status = n.Status,
                    Factory = factoryName + " | " + addressName,
                    Filiale = filialeName
                });
            }

            return Ok(auditDtoList);
        }


        [HttpPut("cancelAudit")]
        [Authorize]
        public async Task<IActionResult> Cancle([FromQuery] Guid id)
        {
            var audit = await _mediator.Send(new GetGenericQuery<Audit, Guid>(id));
            if (audit == null)
            {
                return NotFound("Audit not found.");
            }

            audit.Status = "Canceled";
            var updatedAudit = await _mediator.Send(new PutGenericCommand<Audit>(audit));
            if (updatedAudit != null)
            {
                return Ok(new { Message = "Audit Canceled successfully." });
            }
            else
            {
                return BadRequest(new { Message = "Failed to cancel audit." });
            }
        }
        [HttpPut("CompleteAudit")]
        [Authorize]
        public async Task<IActionResult> COmplete([FromQuery] Guid id)
        {
            var auditorIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(auditorIdStr) || !Guid.TryParse(auditorIdStr, out Guid auditorId))
            {
                return Unauthorized("Invalid auditor id.");
            }
            var audit = await _mediator.Send(new GetGenericQuery<Audit, Guid>(id));
            if (audit == null)
            {
                return NotFound("Audit not found.");
            }

            audit.Status = "Completed";
            var updatedAudit = await _mediator.Send(new PutGenericCommand<Audit>(audit));
            if (updatedAudit != null)
            {
                
                return Ok(new { Message = "Audit completed successfully."});
            }
            else
            {
                return BadRequest(new { Message = "Failed to complete audit." });
            }
        }
        //================================================================================
        //================================================================================//================================================================================
        [HttpPost("rapport")]
        public async Task<IActionResult> CreateRapport([FromBody] CreateRapportDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var auditorIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(auditorIdStr) || !Guid.TryParse(auditorIdStr, out Guid auditorId))
                return Unauthorized("Invalid Token");
            if (dto.IdFactory == Guid.Empty)
                return BadRequest("A valid Factory id is required.");

            var factory = await _mediator.Send(new GetGenericQuery<Factory, Guid>(dto.IdFactory));
            if (factory == null)
                return BadRequest("The provided Factory id does not exist.");

            var rapport = new Rapport
            {
                IdRapport = Guid.NewGuid(),
                Description = dto.Description,
                FKauditor = auditorId,
                FKfactory = dto.IdFactory
            };

            foreach (var pic in dto.Pictures.Where(p => !string.IsNullOrEmpty(p)))
            {
                try
                {
                    rapport.Evidence.Add(new Evidence
                    {
                        IdEvidence = Guid.NewGuid(),
                        Picture = Convert.FromBase64String(pic),
                        FKrapport = rapport.IdRapport
                    });
                }
                catch (FormatException)
                {
                    return BadRequest(new { Message = "Invalid Base64 format in one of the pictures." });
                }
            }

            var createdRapport = await _mediator.Send(new AddGenericCommand<Rapport>(rapport));
            if (createdRapport == null)
                return BadRequest(new { Message = "Failed to create rapport." });

            foreach (var sc in dto.Scores)
            {
                var critere = await _mediator.Send(new GetGenericQuery<Critaire, Guid>(sc.CritereId));
                if (critere == null)
                    return BadRequest(new { Message = $"Critère with id {sc.CritereId} not found." });

                critere.Score = (float)sc.Score;
                await _mediator.Send(new PutGenericCommand<Critaire>(critere));
            }

            // 
            //var historique = new Historique(
            //    Guid.NewGuid(),
            //    (await _mediator.Send(new GetGenericQuery<AppUser, Guid>(auditorId))).First_name
            //      + " " +
            //      (await _mediator.Send(new GetGenericQuery<AppUser, Guid>(auditorId))).Last_name,
            //    dto.Description,
            //    dto.IdFactory,
            //    dto.Scores.Where(s =>  true)
            //              .Select(s => (int)s.Score).ToList(),
            //    
            //    new List<int>(), new List<int>(), new List<int>()
            //);
            //await _mediator.Send(new AddGenericCommand<Historique>(historique));

            return Ok(new { Message = "Rapport created and scores updated successfully." });
        }

        //================================================================================//================================================================================


        [HttpGet("rapports")]
        [Authorize]
        public async Task<IActionResult> GetAuditorRapports()
        {
            var auditorIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(auditorIdStr) || !Guid.TryParse(auditorIdStr, out Guid auditorId))
                return Unauthorized("Invalid auditor id.");

            var rapports = await _mediator.Send(new FindGenericQuery<Rapport>(
                a => a.FKauditor.HasValue
                     && a.FKauditor.Value == auditorId
                     && a.FKfactory != Guid.Empty && a.FKfactory!=null));

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


        [HttpGet("audit-stats")]
        public async Task<IActionResult> GetAuditStats()
        {
            var auditorIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(auditorIdStr) || !Guid.TryParse(auditorIdStr, out Guid auditorId))
            {
                return Unauthorized("error jwt.");
            }

            var allAudits = await _mediator.Send(new FindGenericQuery<Audit>(h => h.FKauditor == auditorId));
            var ayoub = await _mediator.Send(new FindGenericQuery<Audit>(h => h.FKauditor == auditorId && h.Status== "Underway"));
            var rapports = await _mediator.Send(new FindGenericQuery<Rapport>(h => h.FKauditor == auditorId));
            var audits = await _mediator.Send(new GetListGenericQuery<Audit>());
            var dates = ayoub
                .Select(a => DateOnly.FromDateTime(a.CreatedAt))
                .Distinct()
                .Select(date => new DatePlanifierDTO { PlanDate = date })
                .ToList();
            var N = 0;
            foreach (var rapport in rapports)
            {
                var evi = await _mediator.Send(new FindGenericQuery<Evidence>(r => r.FKrapport == rapport.IdRapport));
                if (evi != null)
                {
                    N += evi.Count();
                }
            }
            int totalAudits = allAudits.Count();
            int completedAudits = allAudits.Count(a => a.Status == "Completed");
            int canceledAudits = allAudits.Count(a => a.Status == "Canceled");
            int inProgressAudits = allAudits.Count(a => a.Status == "Underway");
            int Rapports = rapports.Count();

            var resultDto = new AuditStatsDTO
            {
                TotalAudits = totalAudits,
                CompletedAudits = completedAudits,
                CanceledAudits = canceledAudits,
                InProgressAudits = inProgressAudits,
                TotalEvidence = N,
                Rapports = Rapports,
                Dates = dates
            };

            return Ok(resultDto);
        }


        [HttpGet("C-by-factory")]
        public async Task<IActionResult> GetCriteresByFactory([FromQuery] Guid factoryId)
        {
            if (factoryId == Guid.Empty)
                return BadRequest("give factoryId sala7.");

            var sxs = (await _mediator.Send(new FindGenericQuery<Sx>(sx => sx.FKfactory == factoryId)))
                          .OrderBy(sx => sx.NumberS)
                          .ToList();

            if (!sxs.Any())
                return NotFound($"Aucun critère n'existe pour cet identifiant  {factoryId}.");

            var result = new List<SxWithCriteresDTO>();

            foreach (var sx in sxs)
            {
                var crits = (await _mediator.Send(new FindGenericQuery<Critaire>(c => c.FKsx == sx.IdSx)))
                                .OrderBy(c => c.Name)
                                .ToList();

                var critDtos = crits.Select(c => new CritereDTO
                {
                    Id = c.IdCritaire,
                    Name = c.Name,
                    Score = c.Score
                })
                .ToList();

                result.Add(new SxWithCriteresDTO
                {
                    SxId = sx.IdSx,
                    NameEnglish = sx.NameEnglish,
                    NameJaponaise = sx.NameJaponaise,
                    Criteres = critDtos
                });
            }

            return Ok(result);
        }

        public class SxWithCriteresDTO
        {
            public Guid SxId { get; set; }
            public string NameEnglish { get; set; }
            public string NameJaponaise { get; set; }
            public List<CritereDTO> Criteres { get; set; }
        }

        public class CritereDTO
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public float Score { get; set; }
        }


    }
}

