using AutoMapper;
using LogiKnow.Application.Common.DTOs;
using LogiKnow.Domain.Entities;
using System.Text.Json;

namespace LogiKnow.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Term mappings
        CreateMap<Term, TermDto>()
            .ForMember(d => d.Tags, opt => opt.MapFrom(s => s.Tags.Select(t => t.Name).ToList()));

        CreateMap<CreateTermRequest, Term>()
            .ForMember(d => d.Tags, opt => opt.Ignore());

        CreateMap<UpdateTermRequest, Term>()
            .ForMember(d => d.Tags, opt => opt.Ignore())
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.CreatedAt, opt => opt.Ignore());

        // Book mappings
        CreateMap<Book, BookDto>()
            .ForMember(d => d.Authors, opt => opt.MapFrom<BookAuthorsResolver>());

        CreateMap<AddBookRequest, Book>()
            .ForMember(d => d.Authors, opt => opt.MapFrom<AddBookAuthorsResolver>());

        // AcademicEntry mappings
        CreateMap<AcademicEntry, AcademicEntryDto>()
            .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Type.ToString()))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.Tags, opt => opt.MapFrom(s => s.Tags.Select(t => t.Name).ToList()));

        CreateMap<SubmitAcademicEntryRequest, AcademicEntry>()
            .ForMember(d => d.Tags, opt => opt.Ignore())
            .ForMember(d => d.Status, opt => opt.Ignore());

        // Submission mappings
        CreateMap<Submission, SubmissionDto>()
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()));

        // Search result mappings
        CreateMap<LogiKnow.Domain.Interfaces.SearchResult, SearchResultDto>();
        CreateMap<LogiKnow.Domain.Interfaces.QuoteSearchResult, QuoteSearchResultDto>();
    }
}

public class BookAuthorsResolver : IValueResolver<Book, BookDto, List<string>>
{
    public List<string> Resolve(Book source, BookDto destination, List<string> destMember, ResolutionContext context)
    {
        if (string.IsNullOrEmpty(source.Authors)) return new List<string>();
        try { return JsonSerializer.Deserialize<List<string>>(source.Authors) ?? new List<string>(); }
        catch { return new List<string> { source.Authors }; }
    }
}

public class AddBookAuthorsResolver : IValueResolver<AddBookRequest, Book, string>
{
    public string Resolve(AddBookRequest source, Book destination, string destMember, ResolutionContext context)
    {
        return JsonSerializer.Serialize(source.Authors);
    }
}
