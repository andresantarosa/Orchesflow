using Microsoft.EntityFrameworkCore;
using Orchesflow.Example.Models;
using Orchesflow.Example.Persistence;

namespace Orchesflow.Example.Repository;

public class CustomerRepository : ICustomerRepository
{
    private DbSet<Customer> _dbSet;

    public CustomerRepository(ApplicationDbContext context)
    {
        _dbSet = context.Set<Customer>();
    }

    public async Task Add(Customer customer) =>
        await _dbSet.AddAsync(customer);

    public async Task<List<Customer>> List() =>
        await _dbSet.ToListAsync();
}