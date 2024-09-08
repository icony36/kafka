

using CQRS.Core.Events;

namespace CQRS.Core.Domain
{
    public abstract class AggregateRoot
    {
        protected Guid _id;

        private readonly List<BaseEvent> _changes = new();

        public Guid Id { get { return _id; } }

        public int Version { get; set; } = -1;

        public IEnumerable<BaseEvent> GetUncommittedChanges()
        {
            return _changes;
        }

        public void MarkChangesAsCommitted()
        {
            _changes.Clear();
        }

        private void ApplyChange(BaseEvent @event, bool isNew)
        {
            var method = GetType().GetMethod("Apply", [@event.GetType()]); // get the Apply method with the event type as parameter from the concrete class that inherit from this class

            if (method == null )
            {
                throw new ArgumentNullException(nameof(method), $"The Apply method was not found in the aggregate for {@event.GetType().Name}.");
            }
            else
            {
                method.Invoke(this, [@event]);
            }

            if (isNew) // add new event to uncommitted changes as if they come from event store they have already committed before
            {
                _changes.Add(@event);
            }
        }

        protected void RaiseEvent(BaseEvent @event)
        {
            ApplyChange(@event, true);
        }

        public void ReplayEvents(IEnumerable<BaseEvent> events)
        {
            foreach(var @event in events)
            {
                ApplyChange(@event, false);
            }
        }
    }
}
