using CommunityToolkit.Mvvm.Messaging;

namespace IDE.Core.Presentation.Messages;

/// <summary>
/// an adapter for shared app messenger
/// </summary>
public static class Messenger
{
    public static void Send<TMessage>(TMessage message) where TMessage : class
    {
        StrongReferenceMessenger.Default.Send(message);
    }

    public static void Register<TRecipient, TMessage>(TRecipient recipient, MessageHandler<TRecipient, TMessage> handler)
        where TRecipient : class
        where TMessage : class
    {
        StrongReferenceMessenger.Default.Register(recipient, handler);
    }
}
