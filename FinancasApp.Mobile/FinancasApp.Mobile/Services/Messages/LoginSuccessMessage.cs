using CommunityToolkit.Mvvm.Messaging.Messages;

public sealed class LoginSuccessMessage : ValueChangedMessage<bool>
{
    public LoginSuccessMessage() : base(true) { }
}

public sealed class LogoutMessage : ValueChangedMessage<bool>
{
    public LogoutMessage() : base(false) { }
}
