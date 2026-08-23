using gestionAdminTECOCApi.Application.Features.Users.CreateUser;
using gestionAdminTECOCApi.Domain.Abstractions;
using gestionAdminTECOCApi.Domain.Ports;
using gestionAdminTECOCApi.Domain.Users;
using NSubstitute;
using System.Linq.Expressions;

namespace gestionAdminTECOCApi.Api.Tests.Features.Users;

public class UserCommandHandlerTests {
    private readonly IAsyncRepository<User> _repository = Substitute.For<IAsyncRepository<User>>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly UserCommandHandler _handler;

    public UserCommandHandlerTests() {
        _unitOfWork.Repository<User>().Returns( _repository );
        _handler = new UserCommandHandler( new UserService( _unitOfWork ) );
    }

    private static UserCommand ValidCommand() => new(
        "Juan Camilo Tamayo"
        , "CC"
        , "1094567890"
        , "Analista de desarrollo"
    );

    [Fact]
    public async Task Handle_TipoDocumentoNoConfigurado_RetornaFailureSinGuardar() {
        Result<UserCommandResponse> result = await _handler.Handle(
            ValidCommand() with { DocumentType = "XX" }
            , CancellationToken.None
        );

        Assert.True( result.IsFailure );
        Assert.Equal( "User.DocumentTypeNotAllowed", result.Error.Code );
        await _repository
            .DidNotReceive()
            .AddAsync( Arg.Any<User>(), Arg.Any<CancellationToken>() );
    }

    [Fact]
    public async Task Handle_DocumentoYaRegistrado_RetornaFailureSinGuardar() {
        ExisteDocumento( true );

        Result<UserCommandResponse> result = await _handler.Handle(
            ValidCommand()
            , CancellationToken.None
        );

        Assert.True( result.IsFailure );
        Assert.Equal( "User.DocumentAlreadyRegistered", result.Error.Code );
        await _repository
            .DidNotReceive()
            .AddAsync( Arg.Any<User>(), Arg.Any<CancellationToken>() );
    }

    [Fact]
    public async Task Handle_DatosValidos_GuardaElRegistroYRetornaLaInformacionCreada() {
        ExisteDocumento( false );
        UserCommand command = ValidCommand();

        Result<UserCommandResponse> result = await _handler.Handle(
            command
            , CancellationToken.None
        );

        Assert.True( result.IsSuccess );
        Assert.NotEqual( Guid.Empty, result.Value.Id );
        Assert.Equal( command.FullName, result.Value.FullName );
        Assert.Equal( command.DocumentType, result.Value.DocumentType );
        Assert.Equal( command.DocumentNumber, result.Value.DocumentNumber );
        Assert.Equal( command.Position, result.Value.Position );

        await _repository
            .Received( 1 )
            .AddAsync(
                Arg.Is<User>( user =>
                    user.FullName == command.FullName
                    && user.DocumentType == DocumentType.CedulaCiudadania
                    && user.DocumentNumber == command.DocumentNumber
                    && user.Position == command.Position
                )
                , Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task Handle_DatosValidos_RetornaElMismoIdentificadorDelRegistroGuardado() {
        ExisteDocumento( false );
        User? guardado = null;
        await _repository
            .AddAsync( Arg.Do<User>( user => guardado = user ), Arg.Any<CancellationToken>() );

        Result<UserCommandResponse> result = await _handler.Handle(
            ValidCommand()
            , CancellationToken.None
        );

        Assert.NotNull( guardado );
        Assert.Equal( guardado.Id, result.Value.Id );
    }

    [Fact]
    public async Task Handle_TipoDocumentoEnMinuscula_LoNormalizaEnLaRespuesta() {
        ExisteDocumento( false );

        Result<UserCommandResponse> result = await _handler.Handle(
            ValidCommand() with { DocumentType = "cc" }
            , CancellationToken.None
        );

        Assert.True( result.IsSuccess );
        Assert.Equal( "CC", result.Value.DocumentType );
    }

    [Fact]
    public async Task Handle_CamposConEspacios_LosRecortaAntesDeGuardar() {
        ExisteDocumento( false );

        Result<UserCommandResponse> result = await _handler.Handle(
            new UserCommand(
                "  Juan Camilo Tamayo  "
                , "CC"
                , "  1094567890  "
                , "  Analista de desarrollo  "
            )
            , CancellationToken.None
        );

        Assert.True( result.IsSuccess );
        Assert.Equal( "Juan Camilo Tamayo", result.Value.FullName );
        Assert.Equal( "1094567890", result.Value.DocumentNumber );
        Assert.Equal( "Analista de desarrollo", result.Value.Position );
    }

    private void ExisteDocumento( bool existe )
        => _repository
        .Exitst( Arg.Any<Expression<Func<User, bool>>>(), Arg.Any<CancellationToken>() )
        .Returns( existe );
}
