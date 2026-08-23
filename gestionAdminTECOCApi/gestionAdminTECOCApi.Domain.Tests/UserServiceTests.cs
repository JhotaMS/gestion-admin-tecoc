using gestionAdminTECOCApi.Domain.Ports;
using gestionAdminTECOCApi.Domain.Users;
using NSubstitute;
using System.Linq.Expressions;

namespace gestionAdminTECOCApi.Domain.Tests;

[TestClass]
public class UserServiceTests {
    private IAsyncRepository<User> _repository = default!;
    private IUnitOfWork _unitOfWork = default!;
    private UserService _userService = default!;

    [TestInitialize]
    public void Initialize() {
        _repository = Substitute.For<IAsyncRepository<User>>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _unitOfWork.Repository<User>().Returns( _repository );
        _userService = new UserService( _unitOfWork );
    }

    private static User NewUser() => User.Create(
        "Juan Camilo Tamayo"
        , DocumentType.CedulaCiudadania
        , "1094567890"
        , "Analista de desarrollo"
    );

    [TestMethod]
    public async Task CreateUserAsync_UsuarioValido_AgregaAlRepositorioYRetornaElId() {
        //Arrange
        User user = NewUser();

        //Act
        Guid id = await _userService.CreateUserAsync( user, CancellationToken.None );

        //Assert
        Assert.AreEqual( user.Id, id );
        await _repository
            .Received( 1 )
            .AddAsync( user, Arg.Any<CancellationToken>() );
    }

    [TestMethod]
    public async Task CreateUserAsync_UsuarioNulo_LanzaArgumentNullException() {
        //Arrange

        //Act

        //Assert
        await Assert.ThrowsExceptionAsync<ArgumentNullException>(
            () => _userService.CreateUserAsync( null!, CancellationToken.None )
        );
    }

    [TestMethod]
    public async Task ExistsByDocumentAsync_DocumentoRegistrado_RetornaTrue() {
        //Arrange
        _repository
            .Exitst( Arg.Any<Expression<Func<User, bool>>>(), Arg.Any<CancellationToken>() )
            .Returns( true );

        //Act
        bool result = await _userService.ExistsByDocumentAsync(
            DocumentType.CedulaCiudadania
            , "1094567890"
            , CancellationToken.None
        );

        //Assert
        Assert.IsTrue( result );
    }

    [TestMethod]
    public async Task ExistsByDocumentAsync_DocumentoNoRegistrado_RetornaFalse() {
        //Arrange
        _repository
            .Exitst( Arg.Any<Expression<Func<User, bool>>>(), Arg.Any<CancellationToken>() )
            .Returns( false );

        //Act
        bool result = await _userService.ExistsByDocumentAsync(
            DocumentType.CedulaCiudadania
            , "1094567890"
            , CancellationToken.None
        );

        //Assert
        Assert.IsFalse( result );
    }

    [TestMethod]
    public async Task ExistsByDocumentAsync_FiltraPorTipoYNumeroDeDocumento() {
        //Arrange
        Expression<Func<User, bool>>? predicate = null;
        _repository
            .Exitst(
                Arg.Do<Expression<Func<User, bool>>>( filter => predicate = filter )
                , Arg.Any<CancellationToken>()
            )
            .Returns( false );

        //Act
        await _userService.ExistsByDocumentAsync(
            DocumentType.CedulaCiudadania
            , "1094567890"
            , CancellationToken.None
        );

        //Assert
        Assert.IsNotNull( predicate );
        Func<User, bool> filtro = predicate.Compile();
        Assert.IsTrue( filtro( NewUser() ) );
        Assert.IsFalse( filtro( User.Create( "Otra persona", DocumentType.CedulaExtranjeria, "1094567890", "Analista" ) ) );
        Assert.IsFalse( filtro( User.Create( "Otra persona", DocumentType.CedulaCiudadania, "1000000000", "Analista" ) ) );
    }
}
