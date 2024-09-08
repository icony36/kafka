

using Post.Common.Events;

namespace Post.Query.Infrastructure.Handlers
{
    public interface IEventHandler
    {
        Task On(PostCreatedEvent @event); // handling method on consumption
        Task On(MessageUpdatedEvent @event);
        Task On(PostLikedEvent @event);
        Task On(CommentAddedEvent @event);
        Task On(CommentUpdatedEvent @event);
        Task On(CommentRemovedEvent @event);
        Task On(PostRemovedEvent @event);
    }
}
