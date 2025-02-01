using AutoMapper;
using HR.LeaveManagement.BlazorUI.Contracts;
using HR.LeaveManagement.BlazorUI.Models.LeaveTypes;
using HR.LeaveManagement.BlazorUI.Pages;
using HR.LeaveManagement.BlazorUI.Services.Base;

namespace HR.LeaveManagement.BlazorUI.Services;

public class LeaveTypeService : BaseHttpService, ILeaveTypeService
{
    private readonly IMapper _mapper;

    public LeaveTypeService(IClient client, IMapper mapper) : base(client)
    {
        _mapper = mapper;
    }

    public async Task<List<LeaveTypeVM>> GetLeaveTypes()
    {
        var leaveTypes = await Client.LeaveTypesAllAsync();
        return _mapper.Map<List<LeaveTypeVM>>(leaveTypes);
    }

    public async Task<LeaveTypeVM> GetLeaveTypeDetails(int id)
    {
        var leaveType = await Client.LeaveTypesGETAsync(id);
        return _mapper.Map<LeaveTypeVM>(leaveType);
    }

    public async Task<Response<Guid>> CreateLeaveType(LeaveTypeVM leaveType)
    {
        try
        {
            var createLeaveTypeCommand = _mapper.Map<CreateLeaveTypeCommand>(leaveType);
            await Client.LeaveTypesPOSTAsync(createLeaveTypeCommand);
            return new Response<Guid>
            {
                Success = true
            };
        }
        catch (ApiException ex)
        {
            return ConvertApiException<Guid>(ex);
        }
    }

    public async Task<Response<Guid>> UpdateLeaveType(int id, LeaveTypeVM leaveType)
    {
        try
        {
            var updateLeaveTypeCommand = _mapper.Map<UpdateLeaveTypeCommand>(leaveType);
            await Client.LeaveTypesPUTAsync(id.ToString(), updateLeaveTypeCommand);
            return new Response<Guid>
            {
                Success = true,
            };
        }
        catch (ApiException ex)
        {
            return ConvertApiException<Guid>(ex);
        }
    }

    public async Task<Response<Guid>> DeleteLeaveType(int id)
    {
        try
        { 
            await Client.LeaveTypesDELETEAsync(id);
            return new Response<Guid>
            {
                Success = true
            };
        }
        catch (ApiException ex)
        {
            return ConvertApiException<Guid>(ex);
        }
    }
}