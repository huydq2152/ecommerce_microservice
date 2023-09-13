namespace EventBus.Messages;

public interface IIntergrationEvent
{
    DateTime CreationDate { get; }

    Guid Id { get; set; }
}