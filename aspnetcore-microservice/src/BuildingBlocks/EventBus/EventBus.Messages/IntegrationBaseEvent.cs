namespace EventBus.Messages;

public record IntegrationBaseEvent(): IIntergrationEvent
{
    public DateTime CreationDate { get; } = DateTime.UtcNow;
    public Guid Id { get; set; }
}