using CQRS.Core.Commands;
using System.Reflection.Metadata.Ecma335;

namespace CQRS.Core.Infrastructure
{
    public interface ICommandDispatcher
    {
        void RegisterHandler<T>(Func<T, Task> handler) where T : BaseCommand;

        Task SendAsync(BaseCommand command);
    }
}
