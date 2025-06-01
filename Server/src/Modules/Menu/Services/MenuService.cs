using AutoMapper;
using Menu.DTOs;
using Menu.Repositories;
using Shared.Result;

namespace Menu.Services;

public class MenuService : IMenuService
{
    private readonly IMenuRepository _repository;
    private readonly IMapper _mapper;

    public MenuService(IMenuRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
}
