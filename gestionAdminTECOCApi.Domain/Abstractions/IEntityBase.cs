namespace gestionAdminTECOCApi.Domain.Abstractions;

public interface IEntityBase<T> {
    T Id { get; init; }
}
