using HR.LeaveManagement.Application.Contracts.Logging;
using HR.LeaveManagement.Application.Features.LeaveType.Commands.CreateLeaveType;
using HR.LeaveManagement.Application.Features.LeaveType.Commands.DeleteLeaveType;
using HR.LeaveManagement.Application.Features.LeaveType.Commands.UpdateLeaveType;
using HR.LeaveManagement.Application.Features.LeaveType.Queries.GetAllLeaveTypes;
using HR.LeaveManagement.Application.Features.LeaveType.Queries.GetLeaveTypeDetails;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR.LeaveManagement.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class LeaveTypesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IAppLogger<LeaveTypesController> _logger;

    public LeaveTypesController(IMediator mediator, IAppLogger<LeaveTypesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    // Get: api/[LeaveTypesController]
    [HttpGet]
    public async Task<ActionResult<List<LeaveTypeDto>>> Get()
    {
        var leaveTypes = await _mediator.Send(new GetLeaveTypesQuery());
        return Ok(leaveTypes);
    }

    // GET: api/[LeaveTypesController]/5
    [HttpGet("{id}")]
    public async Task<ActionResult<LeaveTypeDto>> Get(int id)
    {
        var leaveType = await _mediator.Send(new GetLeaveTypeDetailsQuery(id));
        return Ok(leaveType);
    }

    // POST: api/[LeaveTypesController]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Post(CreateLeaveTypeCommand leaveType)
    {
        var response = await _mediator.Send(leaveType);
        var result = new { id = response, message = "Leave type created successfully" };
        return CreatedAtAction(nameof(Get), new { id = response }, result);
    }

    // PUT: api/[LeaveTypesController]/5
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> Put(UpdateLeaveTypeCommand leaveType)
    {
        await _mediator.Send(leaveType);
        return NoContent();
    }

    // DELETE: api/[LeaveTypesController]/5
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> Delete(int id)
    {
        var command = new DeleteLeaveTypeCommand { Id = id };
        await _mediator.Send(command);
        return NoContent();
    }
}