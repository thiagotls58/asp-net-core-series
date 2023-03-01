using AutoMapper;
using Contracts.Log;
using Contracts.Repositories;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AccountOwnerServer.Controllers;

[Route("api/owner")]
[ApiController]
public class OwnerController : ControllerBase
{
    private readonly ILoggerManager _logger;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;

    public OwnerController(ILoggerManager logger,
        IRepositoryWrapper repositoryWrapper,
        IMapper mapper)
    {
        _logger = logger;
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
    }

    [HttpGet]
    public IActionResult GetOwners([FromQuery] OwnerParameters ownerParameters)
    {
        if (!ownerParameters.ValidYearRange)
        {
            return BadRequest("Max year of birth cannot be less than min year of birth");
        }

        var owners = _repositoryWrapper.Owner.GetOwners(ownerParameters);

        _logger.LogInfo($"Returned {owners.Count()} owners from database");

        var metaData = new
        {
            owners.TotalCount,
            owners.PageSize,
            owners.CurrentPage,
            owners.TotalPages,
            owners.HasNext,
            owners.HasPrevius
        };

        Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metaData));

        return Ok(metaData);
    }

    [HttpGet]
    public IActionResult GetAllOwners()
    {
        try
        {
            var owners = _repositoryWrapper.Owner.GetAllOwners();

            _logger.LogInfo("Returned all owners from database");

            var ownersResult = _mapper.Map<IEnumerable<OwnerDto>>(owners);

            return Ok(ownersResult);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Something went wrong inside GetAllOwners action: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}", Name = "OwnerById")]
    public IActionResult GetOwnerById(Guid id)
    {
        try
        {
            var owner = _repositoryWrapper.Owner.GetOwnerById(id);

            if (owner is null)
            {
                _logger.LogError($"Owner with id: {id}, hasn't been found in db.");
                return NotFound();
            }
            else
            {
                _logger.LogInfo($"Returned owner with id: {id}");

                var ownerResult = _mapper.Map<OwnerDto>(owner);
                return Ok(ownerResult);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Something went wrong inside GetOwnerById action: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}/account")]
    public IActionResult GetOwnerWithDetails(Guid id)
    {
        try
        {
            var owner = _repositoryWrapper.Owner.GetOwnerWithDetails(id);

            if (owner == null)
            {
                _logger.LogError($"Owner with id: {id}, hasn't been found in db.");
                return NotFound();
            }
            else
            {
                _logger.LogInfo($"Returned owner with details for id: {id}");

                var ownerResult = _mapper.Map<OwnerDto>(owner);
                return Ok(ownerResult);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Something went wrong inside GetOwnerWithDetails action: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    public IActionResult CreateOwner([FromBody] OwnerForCreationDto owner)
    {
        try
        {
            if (owner is null)
            {
                _logger.LogError("Owner object sent from client is null.");
                return BadRequest("Owner object is null");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid owner object sent from client.");
                return BadRequest("Invalid model object");
            }

            var ownerEntity = _mapper.Map<Owner>(owner);

            _repositoryWrapper.Owner.CreateOwner(ownerEntity);
            _repositoryWrapper.Save();

            var createdOwner = _mapper.Map<OwnerDto>(ownerEntity);

            return CreatedAtRoute("OwnerById", new { id = createdOwner.Id }, createdOwner);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}")]
    public IActionResult UpdateOwner(Guid id, [FromBody] OwnerForUpdateDto owner)
    {
        try
        {
            if (owner is null)
            {
                _logger.LogError("Owner object sent from client is null.");
                return BadRequest("Owner object is null");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid owner object sent from client.");
                return BadRequest("Invalid model object");
            }

            var ownerEntity = _repositoryWrapper.Owner.GetOwnerById(id);
            if (ownerEntity is null)
            {
                _logger.LogError($"Owner with id: {id}, hasn't been found in db.");
                return NotFound();
            }

            _mapper.Map(owner, ownerEntity);

            _repositoryWrapper.Owner.UpdateOwner(ownerEntity);
            _repositoryWrapper.Save();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Something went wrong inside UpdateOwner action: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteOwner(Guid id)
    {
        try
        {
            var owner = _repositoryWrapper.Owner.GetOwnerById(id);
            if (owner == null)
            {
                _logger.LogError($"Owner with id: {id}, hasn't been found in db.");
                return NotFound();
            }

            if (_repositoryWrapper.Account.AccountsByOwner(id).Any())
            {
                _logger.LogError($"Cannot delete owner with id: {id}. It has related accounts. Delete those accounts first");
                return BadRequest("Cannot delete owner. It has related accounts. Delete those accounts first");
            }

            _repositoryWrapper.Owner.DeleteOwner(owner);
            _repositoryWrapper.Save();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Something went wrong inside DeleteOwner action: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }
}
