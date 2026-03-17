using AutoMapper;
using LogiKnow.Application.Common.DTOs;
using LogiKnow.Domain.Entities;
using LogiKnow.Domain.Interfaces;
using MediatR;
using System.Text.Json;

namespace LogiKnow.Application.Books.Commands;

public record AddBookCommand(AddBookRequest Data) : IRequest<BookDto>;

public class AddBookHandler : IRequestHandler<AddBookCommand, BookDto>
{
    private readonly IBookRepository _repo;
    private readonly IMapper _mapper;

    public AddBookHandler(IBookRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<BookDto> Handle(AddBookCommand request, CancellationToken ct)
    {
        var book = _mapper.Map<Book>(request.Data);
        var created = await _repo.CreateAsync(book, ct);
        return _mapper.Map<BookDto>(created);
    }
}
