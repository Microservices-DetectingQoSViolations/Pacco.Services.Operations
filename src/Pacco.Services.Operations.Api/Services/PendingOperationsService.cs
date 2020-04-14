using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Pacco.Services.Operations.Api.DTO;
using Pacco.Services.Operations.Api.Services.TimeProviding;

namespace Pacco.Services.Operations.Api.Services
{
    public class PendingOperationsService : IPendingOperationsService
    {
        private readonly ConcurrentDictionary<string, (string OperationName, DateTime StartOperationTime)> _pendingOperations = 
            new ConcurrentDictionary<string, (string, DateTime)>();

        private readonly IDateTimeProvider _dateTimeProvider;

        public PendingOperationsService(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        public void AddOrUpdate(OperationDto operation)
            => _pendingOperations.AddOrUpdate(operation.Id, 
                _ => (operation.Name, _dateTimeProvider.Now()), 
                (_, __) => (operation.Name, _dateTimeProvider.Now()));

        public void Remove(string operationId)
            => _pendingOperations.TryRemove(operationId, out _);

        public IDictionary<string, (string OperationName, DateTime StartOperationTime)> GetAll() => _pendingOperations;
    }
}
