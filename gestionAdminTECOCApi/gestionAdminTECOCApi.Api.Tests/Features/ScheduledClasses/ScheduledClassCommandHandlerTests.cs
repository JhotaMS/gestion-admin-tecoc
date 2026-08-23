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

    private static ScheduledClassCommand ValidCommand() => new(
        "2026-09-01"
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
        Assert.Equal( "ScheduledClass.DateFormatNotAllowed", result.Error.Code );
        await _repository
            .DidNotReceive()
            .AddAsync( Arg.Any<ScheduledClass>(), Arg.Any<CancellationToken>() );
    }

    [Fact]
    public async Task Handle_HoraConFormatoInvalido_RetornaFailureSinGuardar() {
        Result<ScheduledClassCommandResponse> result = await _handler.Handle(
            ValidCommand() with { ScheduledTime = "2:30 pm" }
            , CancellationToken.None
        );

        Assert.True( result.IsFailure );
        Assert.Equal( "ScheduledClass.TimeFormatNotAllowed", result.Error.Code );
        await _repository
            .DidNotReceive()
            .AddAsync( Arg.Any<ScheduledClass>(), Arg.Any<CancellationToken>() );
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
        await _repository
            .DidNotReceive()
            .AddAsync( Arg.Any<ScheduledClass>(), Arg.Any<CancellationToken>() );
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
                    scheduledClass.ScheduledDate == new DateOnly( 2026, 9, 1 )
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
                "  2026-09-01  "
                , "  14:30  "
                , "  Ecuaciones diferenciales de primer orden  "
                , "  Unidad 3  "
            )
            , CancellationToken.None
        );

        Assert.True( result.IsSuccess );
        Assert.Equal( "2026-09-01", result.Value.ScheduledDate );
        Assert.Equal( "14:30", result.Value.ScheduledTime );
        Assert.Equal( "Ecuaciones diferenciales de primer orden", result.Value.Topic );
        Assert.Equal( "Unidad 3", result.Value.CourseLevel );
    }

    private void ExisteProgramacion( bool existe )
        => _repository
        .Exitst( Arg.Any<Expression<Func<ScheduledClass, bool>>>(), Arg.Any<CancellationToken>() )
        .Returns( existe );
}
