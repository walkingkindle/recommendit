using AutoMapper;
using DataRetriever.Models;
using Recommendit.Infrastructure;

namespace DataRetriever
{
    public class ShowMappingProfile : Profile
    {
        public ShowMappingProfile()
        {
            CreateMap<ShowData, Show>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => ExtractName(src.Name)))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Overview))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Image))
                .ForMember(dest => dest.FinalEpisodeAired, opt => opt.MapFrom(src=> ParseDate(src.LastAired)))
                .ForMember(dest => dest.Score, opt => opt.MapFrom(src => src.Score))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.Name ?? "Unknown"))
                .ForMember(dest => dest.OriginalCountry, opt => opt.MapFrom(src => src.OriginalCountry))
                .ForMember(dest => dest.OriginalLanguage, opt => opt.MapFrom(src => src.OriginalLanguage))
                .ForMember(dest => dest.ReleaseYear, opt => opt.MapFrom(src => 
                    !string.IsNullOrEmpty(src.Year) ? int.Parse(src.Year) : (int?)null))
                .ForMember(dest => dest.ShowInfo, opt => opt.Ignore());
        }

        private string ExtractName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return "Unknown";
            }

            return name;
        }

        private DateTime? ParseDate(string dateStr)
        {
            if (string.IsNullOrEmpty(dateStr) || !DateTime.TryParse(dateStr, out DateTime result))
            {
                return null;
            }
            return result;
        }
    }
}
    
