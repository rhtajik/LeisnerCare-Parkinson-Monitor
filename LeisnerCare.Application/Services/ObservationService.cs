using LeisnerCare.Core.Entities;
using LeisnerCare.Core.Interfaces;

namespace LeisnerCare.Application.Services;

public class ObservationService
{
    private readonly IObservationRepository _repository;

    public ObservationService(IObservationRepository repository)
    {
        _repository = repository;
    }

    public Task<List<Observation>> GetByPatientAsync(int patientId) =>
        _repository.GetByPatientIdAsync(patientId);

    public Task<List<Observation>> GetAllAsync() =>
        _repository.GetAllAsync();

    public Task<Observation> CreateAsync(Observation observation) =>
        _repository.AddAsync(observation);
}