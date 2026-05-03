using LeisnerCare.Core.Entities;
using LeisnerCare.Core.Interfaces;

namespace LeisnerCare.Application.Services;

public class ObservationService
{
    private readonly IObservationRepository _repository;
    private readonly IAuditService _auditService;

    public ObservationService(IObservationRepository repository, IAuditService auditService)
    {
        _repository = repository;
        _auditService = auditService;
    }

    public Task<List<Observation>> GetByPatientAsync(int patientId) =>
        _repository.GetByPatientIdAsync(patientId);

    public Task<List<Observation>> GetAllAsync() =>
        _repository.GetAllAsync();

    public async Task<Observation> CreateAsync(Observation observation)
    {
        var result = await _repository.AddAsync(observation);

        // Audit log
        await _auditService.LogAsync(
            "Created",
            "Observation",
            result.Id,
            observation.AuthorId,
            observation.AuthorName,
            $"Klinisk: {observation.IsClinical}");

        return result;
    }
}