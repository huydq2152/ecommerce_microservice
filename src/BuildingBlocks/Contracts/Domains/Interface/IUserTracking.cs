namespace Contracts.Domains.Interface;

public interface IUserTracking
{
    string CreatedBy { get; set; }

    string LastModifiedBy { get; set; }
}