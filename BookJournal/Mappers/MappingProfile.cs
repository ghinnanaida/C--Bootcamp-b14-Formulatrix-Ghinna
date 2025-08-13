using AutoMapper;
using BookJournal.DTOs;
using BookJournal.Models;

namespace BookJournal.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ProgressTracker, ProgressTrackerDTO>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Book.Title))
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Book.Author))
                .ForMember(dest => dest.Genres, opt => opt.MapFrom(src => src.Book.Genres.Select(g => g.Name).ToList()))
                .ForMember(dest => dest.ProgressPercentage, opt => opt.MapFrom(src =>
                    src.TotalValue > 0 ? (src.CurrentValue / src.TotalValue) * 100 : 0));

            CreateMap<BookCreateDTO, Book>()
                .ForMember(dest => dest.Genres, opt => opt.Ignore());

            CreateMap<ProgressTrackerCreateDTO, ProgressTracker>()
                .ForMember(dest => dest.LastStatusChangeDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<ProgressTrackerUpdateDTO, ProgressTracker>()
                .ForMember(dest => dest.LastStatusChangeDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
