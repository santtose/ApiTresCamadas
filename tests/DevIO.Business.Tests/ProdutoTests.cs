using DevIO.Business.Interfaces.Repository;
using DevIO.Business.Models;
using DevIO.Business.Services;
using Moq;
using Moq.AutoMock;

namespace DevIO.Business.Tests
{
    public class ProdutoTests
    {
        private readonly AutoMocker _mocker;
        private readonly ProdutoService _produtoService;

        public ProdutoTests()
        {
            _mocker = new AutoMocker();
            _produtoService = _mocker.CreateInstance<ProdutoService>();
        }

        [Fact(DisplayName = "Adicionar Produto com Sucesso")]
        [Trait("Categoria", "Produto Service Tests")]
        public async Task ProdutoService_Adicionar_DeveExecutarComSucesso()
        {
            var idGuid = Guid.NewGuid();
            // Arrange
            var produto = new Produto
            {
                Id = idGuid,
                Nome = "Produto Teste",
                Descricao = "Descricao Teste",
                Valor = 100,
                Ativo = true,
                FornecedorId = Guid.NewGuid()
            };

            // Act
            await _produtoService.Adicionar(produto);

            // Assert
            Assert.True(produto.EhValido());
            _mocker.GetMock<IProdutoRepository>().Verify(r => r.Adicionar(produto), Times.Once);
        }

        [Fact(DisplayName = "Adicionar Produto com Falha")]
        [Trait("Categoria", "Produto Service Tests")]
        public async Task ProdutoService_Adicionar_DeveExecutarComFalha()
        {
            var idGuid = Guid.NewGuid();
            // Arrange
            var produto = new Produto
            {
                Id = idGuid,
                Nome = "",
                Descricao = "Descricao Teste",
                Valor = 100,
                Ativo = true,
                FornecedorId = Guid.NewGuid()
            };

            // Act
            await _produtoService.Adicionar(produto);

            // Assert
            Assert.False(produto.EhValido());
            _mocker.GetMock<IProdutoRepository>().Verify(r => r.Adicionar(produto), Times.Never);
        }
    }
}
