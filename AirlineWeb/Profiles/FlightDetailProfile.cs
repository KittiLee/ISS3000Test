using AirlineWeb.Models;
using AirlineWeb.Dtos;
using AutoMapper;

namespace AirlineWeb.Profiles
{
    public class FlightDetailProfile : Profile
    {
        public FlightDetailProfile()
        {
            CreateMap<FlightDetail,FlightDetailReadDto>();
            CreateMap<FlightDetailCreateDto,FlightDetail>();           
            CreateMap<FlightDetailUpdateDto, FlightDetail>();
        }

    }
}