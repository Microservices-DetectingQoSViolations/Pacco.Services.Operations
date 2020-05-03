﻿using Pacco.Services.Operations.Api.Services;
using Pacco.Services.Operations.Api.Services.TimeProviding;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Convey.QoS.Violation;
using Convey.QoS.Violation.Act;
using Microsoft.Extensions.Logging;
using Pacco.Services.Operations.Api.Infrastructure;

namespace Pacco.Services.Operations.Api.QoS.Job
{
    public class CheckRequestStatusJob : IJob
    {
        private readonly IPendingOperationsService _pendingOperationsService;
        private readonly IQoSViolateRaiser _qoSViolateRaiser;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly int _handlingRequestsOvertimeInSeconds;
        private readonly ILogger<CheckRequestStatusJob> _logger;
        private readonly ICollection<string> _overtimeOperations = new List<string>();

        public CheckRequestStatusJob(IPendingOperationsService pendingOperationsService, IDateTimeProvider dateTimeProvider, 
            RequestsOptions requestsOptions, ILogger<CheckRequestStatusJob> logger, IQoSViolateRaiser qoSViolateRaiser)
        {
            _pendingOperationsService = pendingOperationsService;
            _dateTimeProvider = dateTimeProvider;
            _logger = logger;
            _qoSViolateRaiser = qoSViolateRaiser;

            _handlingRequestsOvertimeInSeconds = requestsOptions.MaxHandlingOperationSeconds;
        }

        public Task Execute(IJobExecutionContext context)
        {
            var currentDate = _dateTimeProvider.Now();
            var pendingOperations = _pendingOperationsService.GetAll();

            foreach (var (id, (operationName, startOperationTime)) in pendingOperations)
            {
                if (DidHandlingRequestOvertime(currentDate, startOperationTime))
                {
                    _logger.LogWarning($"Request with Id {id} and name {operationName} still is not full handled.");
                    _qoSViolateRaiser.Raise(ViolationType.NotFullyHandledOperation);
                    _overtimeOperations.Add(id);
                }
            }

            if (_overtimeOperations.Any())
            {
                foreach (var operationId in _overtimeOperations)
                {
                    pendingOperations.Remove(operationId);
                }
                _overtimeOperations.Clear();
            }

            return Task.CompletedTask;
        }

        private bool DidHandlingRequestOvertime(DateTime currentTime, DateTime pendingOperationStartTime)
            => (currentTime - pendingOperationStartTime).TotalSeconds > _handlingRequestsOvertimeInSeconds;
    }
}
