using AutoMapper;
using FarmServer.Domain.Entities;
using FarmServer.DTOs.Farm;
using FarmServer.DTOs.Farmer;
using FarmServer.DTOs.Field;

namespace FarmServer.Mappings
{
    public class MappingProfile :Profile
    {
        public MappingProfile()
        {

            //Field

            // Map CreateFieldDTO to Field and generate a new Guid
            CreateMap<CreateFieldDTO, Field>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()));
            // Map FielDTO to Field and assign the Id to a new Guid
            CreateMap<FieldDTO, Field>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()));
            // Map Field to FieldDTO
            CreateMap<Field, FieldDTO>()
                .ForMember(dest => dest.Farm, opt => opt.MapFrom(src => new PartialFarmUpdateDTO
                {
                    Id = src.Farm.Id,
                    Name = src.Farm.Name,
                    Location = src.Farm.Location
                }));

            // Farm
            //Farm to FarmDTO
            CreateMap<Farm, FarmDTO>();
            //FarmDTO to Farm and generate a new Guid
            CreateMap<FarmDTO, Farm>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()));


            //Farmer

            //Map FarmerDTO to Farmer
            CreateMap<CreateFarmerDTO, Farmer>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name ?? string.Empty))//Handle null values
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Location ?? string.Empty));

            //Map Farmer to FarmerDTO
            CreateMap<Farmer, FarmerDTO>();
            //Map FarmerDTO to Farmer
            CreateMap<FarmerDTO, Farmer>();
        }
    }
}
