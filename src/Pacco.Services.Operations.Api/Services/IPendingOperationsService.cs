using System;
using System.Collections.Generic;
using Pacco.Services.Operations.Api.DTO;

namespace Pacco.Services.Operations.Api.Services
{
    public interface IPendingOperationsService
    {
        void AddOrUpdate(OperationDto operation);
        void Remove(string operationId);
        IDictionary<string, (string OperationName, DateTime StartOperationTime)> GetAll();
    }
}