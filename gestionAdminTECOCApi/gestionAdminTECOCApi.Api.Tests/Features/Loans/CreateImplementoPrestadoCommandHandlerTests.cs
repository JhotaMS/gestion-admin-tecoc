using gestionAdminTECOCApi.Application.Features.Loans.CreateImplementoPrestado;
using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.Loans;
using gestionAdminTECOCApi.Domain.Ports;
using gestionAdminTECOCApi.Domain.Users;
using NSubstitute;
using System.Linq.Expressions;

namespace gestionAdminTECOCApi.Api.Tests.Features.Loans;

public class CreateImplementoPrestadoCommandHandlerTests {
    private readonly IAsyncRepository<ImplementoPrestado> _prestamoRepo = Substitute.For<IAsyncRepository<ImplementoPrestado>>();
    private readonly IAsyncRepository<User> _userRepo = Substitute.For<IAsyncRepository<User>>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly CreateImplementoPrestadoCommandHandler _handler;

    public CreateImplementoPrestadoCommandHandlerTests() {
        _unitOfWork.Repository<ImplementoPrestado>().Returns( _prestamoRepo );
        _unitOfWork.Repository<User>().Returns( _userRepo );
        _handler = new CreateImplementoPrestadoCommandHandler( _unitOfWork );
    }

    private static CreateImplementoPrestadoCommand ValidCommand() => new(
        UserId: Guid.NewGuid(),
        ImplementoId: Guid.NewGuid(),
        TipoRevisionId: 1,
        EstadoTipo: EstadoTipoImplemento.Bueno,
        FechaInicio: DateTime.UtcNow,
        FechaFin: DateTime.UtcNow.AddDays( 2 ),
        Observacion: "Implemento entregado en óptimas condiciones."
    );

    [Fact]
    public async Task Handle_FechaFinMenorAFechaInicio_RetornaFailureSinGuardar() {
        var invalidCommand = ValidCommand() with {
            FechaInicio = DateTime.UtcNow.AddDays( 3 ),
            FechaFin = DateTime.UtcNow
        };

        Result<CreateImplementoPrestadoResponse> result = await _handler.Handle(
            invalidCommand,
            CancellationToken.None
        );

        Assert.True( result.IsFailure );
        Assert.Equal( "ImplementoPrestado.InvalidDateRange", result.Error.Code );
        await _prestamoRepo
            .DidNotReceive()
            .AddAsync( Arg.Any<ImplementoPrestado>(), Arg.Any<CancellationToken>() );
    }

    [Fact]
    public async Task Handle_UsuarioNoExiste_RetornaFailureSinGuardar() {
        _userRepo
            .Exitst( Arg.Any<Expression<Func<User, bool>>>(), Arg.Any<CancellationToken>() )
            .Returns( false );

        Result<CreateImplementoPrestadoResponse> result = await _handler.Handle(
            ValidCommand(),
            CancellationToken.None
        );

        Assert.True( result.IsFailure );
        Assert.Equal( "ImplementoPrestado.UserNotFound", result.Error.Code );
        await _prestamoRepo
            .DidNotReceive()
            .AddAsync( Arg.Any<ImplementoPrestado>(), Arg.Any<CancellationToken>() );
    }

    [Fact]
    public async Task Handle_DatosValidos_GuardaRegistroYRetornaRespuestaExitosa() {
        _userRepo
            .Exitst( Arg.Any<Expression<Func<User, bool>>>(), Arg.Any<CancellationToken>() )
            .Returns( true );

        var command = ValidCommand();

        Result<CreateImplementoPrestadoResponse> result = await _handler.Handle(
            command,
            CancellationToken.None
        );

        Assert.True( result.IsSuccess );
        Assert.NotEqual( Guid.Empty, result.Value.Id );
        Assert.Equal( command.UserId, result.Value.UserId );
        Assert.Equal( command.ImplementoId, result.Value.ImplementoId );
        Assert.Equal( command.TipoRevisionId, result.Value.TipoRevisionId );
        Assert.Equal( command.EstadoTipo.ToString(), result.Value.EstadoTipo );
        Assert.Equal( command.FechaInicio, result.Value.FechaInicio );
        Assert.Equal( command.FechaFin, result.Value.FechaFin );
        Assert.Equal( command.Observacion, result.Value.Observacion );

        await _prestamoRepo
            .Received( 1 )
            .AddAsync( Arg.Any<ImplementoPrestado>(), Arg.Any<CancellationToken>() );
        await _unitOfWork
            .Received( 1 )
            .SaveChangesAsync();
    }
}

