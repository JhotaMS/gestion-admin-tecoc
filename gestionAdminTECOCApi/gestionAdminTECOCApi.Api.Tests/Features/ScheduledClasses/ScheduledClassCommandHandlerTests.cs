using gestionAdminTECOCApi.Application.Features.ScheduledClasses.CreateScheduledClass;
using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.Ports;
using gestionAdminTECOCApi.Domain.ScheduledClasses;
using NSubstitute;
using System.Linq.Expressions;

namespace gestionAdminTECOCApi.Api.Tests.Features.ScheduledClasses;

public class ScheduledClassCommandHandlerTests {
    private readonly IAsyncRepository<ScheduledClass> _repository = Substitute.For<IAsyncRepository<ScheduledClass>>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ScheduledClassCommandHandler _handler;

    public ScheduledClassCommandHandlerTests() {
        _unitOfWork.Repository<ScheduledClass>().Returns( _repository );
        _handler = new ScheduledClassCommandHandler( new ScheduledClassService( _unitOfWork ) );
    }

    private static readonly DateOnly _fecha = new( 2026, 9, 15 );

    private static string FechaTexto()
        => _fecha.ToString( "yyyy-MM-dd" );

    private static ScheduledClassCommand ValidCommand() => new(
        FechaTexto()
        , "14:30"
        , "Ecuaciones diferenciales de primer orden"
        , "Unidad 3"
    );

    [Fact]
    public async Task Handle_FechaConFormatoInvalido_RetornaFailureSinGuardar() {
        Result<ScheduledClassCommandResponse> result = await _handler.Handle(
            ValidCommand() with { ScheduledDate = "01/09/2026" }
            , CancellationToken.None
        );

        Assert.True( result.IsFailure );
        Assert.Equal( "ScheduledClass.ValidationFailed", result.Error.Code );
        Assert.Contains( "yyyy-MM-dd", result.Error.Name, StringComparison.Ordinal );
        await NoSeGuardoNada();
    }

    [Fact]
    public async Task Handle_HoraConFormatoInvalido_RetornaFailureSinGuardar() {
        Result<ScheduledClassCommandResponse> result = await _handler.Handle(
            ValidCommand() with { ScheduledTime = "2:30 pm" }
            , CancellationToken.None
        );

        Assert.True( result.IsFailure );
        Assert.Equal( "ScheduledClass.ValidationFailed", result.Error.Code );
        Assert.Contains( "HH:mm", result.Error.Name, StringComparison.Ordinal );
        await NoSeGuardoNada();
    }

    [Fact]
    public async Task Handle_CamposObligatoriosVacios_RetornaFailureConTodosLosMensajes() {
        Result<ScheduledClassCommandResponse> result = await _handler.Handle(
            new ScheduledClassCommand( string.Empty, string.Empty, string.Empty, string.Empty )
            , CancellationToken.None
        );

        Assert.True( result.IsFailure );
        Assert.Equal( "ScheduledClass.ValidationFailed", result.Error.Code );
        Assert.Contains( "La fecha de la clase es obligatoria", result.Error.Name, StringComparison.Ordinal );
        Assert.Contains( "La hora de la clase es obligatoria", result.Error.Name, StringComparison.Ordinal );
        Assert.Contains( "El tema de la clase es obligatorio", result.Error.Name, StringComparison.Ordinal );
        Assert.Contains( "El nivel o unidad del curso es obligatorio", result.Error.Name, StringComparison.Ordinal );
        await NoSeGuardoNada();
    }

    [Fact]
    public async Task Handle_FechaAnteriorAHoy_LaAceptaYGuardaElRegistro() {
        ExisteProgramacion( false );

        Result<ScheduledClassCommandResponse> result = await _handler.Handle(
            ValidCommand() with { ScheduledDate = "2020-03-15" }
            , CancellationToken.None
        );

        Assert.True( result.IsSuccess );
        Assert.Equal( "2020-03-15", result.Value.ScheduledDate );
    }

    [Fact]
    public async Task Handle_ClaseYaProgramadaEnLaMismaFechaYHora_RetornaFailureSinGuardar() {
        ExisteProgramacion( true );

        Result<ScheduledClassCommandResponse> result = await _handler.Handle(
            ValidCommand()
            , CancellationToken.None
        );

        Assert.True( result.IsFailure );
        Assert.Equal( "ScheduledClass.ScheduleAlreadyTaken", result.Error.Code );
        await NoSeGuardoNada();
    }

    [Fact]
    public async Task Handle_DatosValidos_GuardaElRegistroYRetornaLaInformacionCreada() {
        ExisteProgramacion( false );
        ScheduledClassCommand command = ValidCommand();

        Result<ScheduledClassCommandResponse> result = await _handler.Handle(
            command
            , CancellationToken.None
        );

        Assert.True( result.IsSuccess );
        Assert.NotEqual( Guid.Empty, result.Value.Id );
        Assert.Equal( command.ScheduledDate, result.Value.ScheduledDate );
        Assert.Equal( command.ScheduledTime, result.Value.ScheduledTime );
        Assert.Equal( command.Topic, result.Value.Topic );
        Assert.Equal( command.CourseLevel, result.Value.CourseLevel );

        await _repository
            .Received( 1 )
            .AddAsync(
                Arg.Is<ScheduledClass>( scheduledClass =>
                    scheduledClass.ScheduledDate == _fecha
                    && scheduledClass.ScheduledTime == new TimeOnly( 14, 30 )
                    && scheduledClass.Topic == command.Topic
                    && scheduledClass.CourseLevel == command.CourseLevel
                )
                , Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task Handle_DatosValidos_RetornaElMismoIdentificadorDelRegistroGuardado() {
        ExisteProgramacion( false );
        ScheduledClass? guardado = null;
        await _repository
            .AddAsync( Arg.Do<ScheduledClass>( scheduledClass => guardado = scheduledClass ), Arg.Any<CancellationToken>() );

        Result<ScheduledClassCommandResponse> result = await _handler.Handle(
            ValidCommand()
            , CancellationToken.None
        );

        Assert.NotNull( guardado );
        Assert.Equal( guardado.Id, result.Value.Id );
    }

    [Fact]
    public async Task Handle_HoraConSegundos_LaNormalizaEnLaRespuesta() {
        ExisteProgramacion( false );

        Result<ScheduledClassCommandResponse> result = await _handler.Handle(
            ValidCommand() with { ScheduledTime = "14:30:00" }
            , CancellationToken.None
        );

        Assert.True( result.IsSuccess );
        Assert.Equal( "14:30", result.Value.ScheduledTime );
    }

    [Fact]
    public async Task Handle_CamposConEspacios_LosRecortaAntesDeGuardar() {
        ExisteProgramacion( false );

        Result<ScheduledClassCommandResponse> result = await _handler.Handle(
            new ScheduledClassCommand(
                "  " + FechaTexto() + "  "
                , "  14:30  "
                , "  Ecuaciones diferenciales de primer orden  "
                , "  Unidad 3  "
            )
            , CancellationToken.None
        );

        Assert.True( result.IsSuccess );
        Assert.Equal( FechaTexto(), result.Value.ScheduledDate );
        Assert.Equal( "14:30", result.Value.ScheduledTime );
        Assert.Equal( "Ecuaciones diferenciales de primer orden", result.Value.Topic );
        Assert.Equal( "Unidad 3", result.Value.CourseLevel );
    }

    private async Task NoSeGuardoNada()
        => await _repository
        .DidNotReceive()
        .AddAsync( Arg.Any<ScheduledClass>(), Arg.Any<CancellationToken>() );

    private void ExisteProgramacion( bool existe )
        => _repository
        .Exitst( Arg.Any<Expression<Func<ScheduledClass, bool>>>(), Arg.Any<CancellationToken>() )
        .Returns( existe );
}
