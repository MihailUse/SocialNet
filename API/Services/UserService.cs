using API.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class UserService
{
    private readonly IMapper _mapper;
    private readonly DataContext _context;

    public UserService(IMapper mapper, DataContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task CreateUser(CreateUserModel model)
    {
        var user = _mapper.Map<DAL.Entities.User>(model);
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }
    
    public async Task UpdateUser(UpdateUserModel model)
    {
        var user = _mapper.Map<DAL.Entities.User>(model);
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task<UserModel> GetUser(Guid id)
    {
        User user = await _context.Users.FindAsync(id);
        return _mapper.Map<UserModel>(user);
    }
}