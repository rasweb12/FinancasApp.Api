using CommunityToolkit.Mvvm.Messaging.Messages;

namespace FinancasApp.Mobile;

public sealed class LogoutMessage : ValueChangedMessage<bool>
{
    public LogoutMessage() : base(false) { }
}
