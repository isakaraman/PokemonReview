using AutoMapper;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.AutoMapper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Pokemon,PokemonDto>();  
            CreateMap<Category,CategoryDto>();
            CreateMap<CategoryDto, Category>();
            CreateMap<Country,CountryDto>();
            CreateMap<CountryDto, Country>();
            CreateMap<Owner,OwnerDto>();
            CreateMap<OwnerDto, Owner>();
            CreateMap<PokemonDto, Pokemon>();
            CreateMap<Review,ReviewDto>();
            CreateMap<ReviewDto,Review>();
            CreateMap<Reviewer,ReviewerDto>();
            CreateMap<ReviewerDto, Reviewer>();
        }
    }
}
