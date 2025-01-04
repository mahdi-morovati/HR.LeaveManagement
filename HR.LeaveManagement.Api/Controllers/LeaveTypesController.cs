using HR.LeaveManagement.Application.Features.LeaveRequest.Commands.UpdateLeaveRequest;
using HR.LeaveManagement.Application.Features.LeaveType.Commands.CreateLeaveType;
using HR.LeaveManagement.Application.Features.LeaveType.Queries.GetAllLeaveTypes;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HR.LeaveManagement.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LeaveTypesController : ControllerBase
{
    private readonly IMediator _mediator;

    public LeaveTypesController(IMediator mediator)
    {
        _mediator = mediator;
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
        throw new NotImplementedException();
    }
    
    // POST: api/[LeaveTypesController]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Post(CreateLeaveTypeCommand leaveType)
    {
        throw new NotImplementedException();
    }
    
    // PUT: api/[LeaveTypesController]/5
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> Put(UpdateLeaveRequestCommand levaeType)
    {
        throw new NotImplementedException();
    }
    
    // DELETE: api/[LeaveTypesController]/5
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> Delete(int id)
    {
        throw new NotImplementedException();
    }

}