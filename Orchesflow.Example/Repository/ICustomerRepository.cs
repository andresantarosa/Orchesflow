using Orchesflow.Example.Models;

namespace Orchesflow.Example.Repository;

public interface ICustomerRepository
{
    Task Add(Customer customer);
    Task<List<Customer>> List();
}