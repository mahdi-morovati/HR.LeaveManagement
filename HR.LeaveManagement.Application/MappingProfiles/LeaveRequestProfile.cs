using AutoMapper;
using HR.LeaveManagement.Application.Features.LeaveRequest.Commands.CreateLeaveRequest;
using HR.LeaveManagement.Application.Features.LeaveRequest.Commands.UpdateLeaveRequest;
using HR.LeaveManagement.Application.Features.LeaveRequest.Queries.GetLeaveRequestDetail;
using HR.LeaveManagement.Application.Features.LeaveRequest.Queries.GetLeaveRequestList;
using HR.LeaveManagement.Application.Features.LeaveRequest.Queries.GetLeaveRequestsByUser;
using HR.LeaveManagement.Domain;

namespace HR.LeaveManagement.Application.MappingProfiles;

public class LeaveRequestProfile : Profile
{
    public LeaveRequestProfile()
    {
        CreateMap<LeaveRequestListDto, LeaveRequest>().ReverseMap();
        CreateMap<LeaveRequestDetailsDto, LeaveRequest>().ReverseMap();
        CreateMap<LeaveRequest, LeaveRequestListDto>()
            .ForMember(dest => dest.LeaveType, opt => opt.MapFrom(src => src.LeaveType));
        CreateMap<LeaveRequest, LeaveRequestDetailsDto>();
        CreateMap<CreateLeaveRequestCommand, LeaveRequest>();
        CreateMap<LeaveRequest, LeaveRequestsByUserDto>();
        CreateMap<UpdateLeaveRequestCommand, LeaveRequest>();
    }
}