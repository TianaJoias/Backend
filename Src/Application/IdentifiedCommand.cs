using System;
using Application.Common;

namespace Application
{
    public class IdentifiedCommand<T> : ICommand
        where T : ICommand
    {
        public T Command { get; }
        public Guid Id { get; }
        public IdentifiedCommand(T command, Guid id)
        {
            Command = command;
            Id = id;
        }

        /// <summary>
        /// Returns an instance of the builder to start the fluent creation of the object.
        /// </summary>
        public static IdentifiedCommand<T> New(T command, Guid id)
        {
            return new IdentifiedCommand<T>(command, id);
        }
    }
}
