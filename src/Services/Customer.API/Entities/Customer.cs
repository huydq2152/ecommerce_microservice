using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Contracts.Domains;

namespace Customer.API.Entities;

public class Customer: EntityBase<int>
{
    public string UserName { get; set; }
    
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
}