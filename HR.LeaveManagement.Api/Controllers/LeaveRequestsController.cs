using HR.LeaveManagement.Application.Features.LeaveRequest.Queries.GetLeaveRequestDetail;
using HR.LeaveManagement.Application.Features.LeaveRequest.Queries.GetLeaveRequestList;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HR.LeaveManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeaveRequestsController : ControllerBase
{
    private readonly IMediator _mediator;

    public LeaveRequestsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Get: api/[LeaveRequestsController]
    [HttpGet]
    public async Task<ActionResult<List<LeaveRequestListDto>>> Get()
    {
        var leaveRequests = await _mediator.Send(new GetLeaveRequestListQuery());
        return Ok(leaveRequests);
    }

    // Get: api/[LeaveRequestsController]/5
    [HttpGet("{id}")]
    public async Task<ActionResult<LeaveRequestDetailsDto>> Get(int id)
    {
        var leaveRequest = await _mediator.Send(new GetLeaveRequestDetailQuery()
        {
            Id = id
        });
        return Ok(leaveRequest);
    }
}