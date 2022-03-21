using System.ComponentModel.DataAnnotations;

namespace Orchesflow.Example.Models;

public class Customer
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}