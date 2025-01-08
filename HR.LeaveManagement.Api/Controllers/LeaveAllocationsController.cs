using HR.LeaveManagement.Application.Features.LeaveAllocation.Commands.CreateLeaveAllocation;
using HR.LeaveManagement.Application.Features.LeaveAllocation.Queries.GetLeaveAllocationDetails;
using HR.LeaveManagement.Application.Features.LeaveAllocation.Queries.GetLeaveAllocations;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HR.LeaveManagement.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LeaveAllocationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public LeaveAllocationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Get: api/[LeaveAllocationsController]
    [HttpGet]
    public async Task<ActionResult<List<LeaveAllocationDto>>> Get()
    {
        var leaveAllocations = await _mediator.Send(new GetLeaveAllocationListQuery());
        return Ok(leaveAllocations);
    }

    // Get: api/[LeaveAllocationsController]/5
    [HttpGet("id")]
    public async Task<ActionResult<LeaveAllocationDto>> Get(int id)
    {
        var leaveAllocation = await _mediator.Send(new GetLeaveAllocationDetailsQuery() { Id = id });
        return Ok(leaveAllocation);
    }

    // POST: api/[LeaveAllocationsController]
    [HttpPost]
    public async Task<ActionResult> Post(CreateLeaveAllocationCommand leaveAllocation)
    {
        var response = await _mediator.Send(leaveAllocation);
        var result = new { Id = response, message = "Leave Allocation created successfully" };
        return CreatedAtAction(nameof(Get), new { id = response }, result);
    }
}