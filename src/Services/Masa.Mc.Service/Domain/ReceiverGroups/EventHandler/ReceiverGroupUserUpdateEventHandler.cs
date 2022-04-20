namespace Masa.Mc.Service.Admin.Domain.ReceiverGroups.EventHandler;

public class ReceiverGroupUserUpdateEventHandler
{
    public ReceiverGroupUserUpdateEventHandler()
    {

    }

    [EventHandler]
    public void HandleEvent(ReceiverGroupItemChangedDomainEvent @event)
    {
        //Waiting to dock Masa Auth
    }
}
