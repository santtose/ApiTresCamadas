using DevIO.Business.Interfaces.Repository;
using DevIO.Business.Models;
using DevIO.Business.Services;
using Moq;
using Moq.AutoMock;
using System.Linq.Expressions;

namespace DevIO.Business.Tests
{
    public class FornecedorTests
    {
        [Fact(DisplayName = "Adicionar Fornecedor com Sucesso")]
        [Trait("Categoria", "Fornecedor Service Tests")]
        public async Task FornecedorService_Adicionar_DeveExecutarComSucesso()
        {
            //  Arrange
            var idGuid = Guid.NewGuid();
            var fornecedor = new Fornecedor
            {
                Id = idGuid,
                Nome = "Fornecedor Teste",
                Documento = "45922445081",
                TipoFornecedor = TipoFornecedor.PessoaFisica,
                Ativo = true,
                Endereco = new Endereco
                {
                    Logradouro = "Rua Teste",
                    Numero = "123",
                    Bairro = "Bairro Teste",
                    Cidade = "Cidade Teste",
                    Estado = "Estado Teste",
                    Cep = "12345678"
                }
            };

            var mocker = new AutoMocker();
            var fornecedorService = mocker.CreateInstance<FornecedorService>();

            // Act
            await fornecedorService.Adicionar(fornecedor);

            // Assert
            Assert.True(fornecedor.EhValido());
            mocker.GetMock<IFornecedorRepository>().Verify(r => r.Adicionar(fornecedor), Times.Once);
        }

        [Fact(DisplayName = "Adicionar Fornecedor com Falha")]
        [Trait("Categoria", "Fornecedor Service Tests")]
        public async Task FornecedorService_Adicionar_DeveFalhar()
        {
            //  Arrange
            var idGuid = Guid.NewGuid();
            var fornecedor = new Fornecedor
            {
                Id = idGuid,
                Nome = "Fornecedor Teste",
                Documento = "12345678944",
                TipoFornecedor = TipoFornecedor.PessoaFisica,
                Ativo = true,
                Endereco = new Endereco
                {
                    Logradouro = "Rua Teste",
                    Numero = "123",
                    Bairro = "Bairro Teste",
                    Cidade = "Cidade Teste",
                    Estado = "Estado Teste",
                    Cep = "12345678"
                }
            };

            var mocker = new AutoMocker();
            var fornecedorService = mocker.CreateInstance<FornecedorService>();

            // Act
            await fornecedorService.Adicionar(fornecedor);

            // Assert
            Assert.False(fornecedor.EhValido());
            mocker.GetMock<IFornecedorRepository>().Verify(r => r.Adicionar(fornecedor), Times.Never);
        }

        [Fact(DisplayName = "Atualizar Fornecedor com Sucesso")]
        [Trait("Categoria", "Fornecedor Service Tests")]
        public async Task FornecedorService_Atualizar_DeveExecutarComSucesso()
        {
            // Arrange
            var idGuid = Guid.NewGuid();
            var fornecedor = new Fornecedor
            {
                Id = idGuid,
                Nome = "Fornecedor Atualizado",
                Documento = "45922445081", // CPF válido
                TipoFornecedor = TipoFornecedor.PessoaFisica,
                Ativo = true,
                Endereco = new Endereco
                {
                    Logradouro = "Rua Nova",
                    Numero = "456",
                    Bairro = "Bairro Novo",
                    Cidade = "Cidade Nova",
                    Estado = "Estado Novo",
                    Cep = "87654321"
                }
            };

            var mocker = new AutoMocker();
            var fornecedorService = mocker.CreateInstance<FornecedorService>();
            // Garante que não existe outro fornecedor com o mesmo documento e ID diferente
            mocker.GetMock<IFornecedorRepository>()
                .Setup(r => r.Buscar(It.IsAny<Expression<Func<Fornecedor, bool>>>()))
                .ReturnsAsync(new List<Fornecedor>());            

            // Act
            await fornecedorService.Atualizar(fornecedor);

            // Assert
            mocker.GetMock<IFornecedorRepository>().Verify(r => r.Atualizar(fornecedor), Times.Once);
        }

        [Fact(DisplayName = "Atualizar Fornecedor com Falha")]
        [Trait("Categoria", "Fornecedor Service Tests")]
        public async Task FornecedorService_Atualizar_DeveFalhar_QuandoDocumentoJaExiste()
        {
            // Arrange            
            var idGuid = Guid.NewGuid();
            var fornecedor = new Fornecedor
            {
                Id = idGuid,
                Nome = "Fornecedor Atualizado",
                Documento = "45922445081", // CPF válido
                TipoFornecedor = TipoFornecedor.PessoaFisica,
                Ativo = true,
                Endereco = new Endereco
                {
                    Logradouro = "Rua Nova",
                    Numero = "456",
                    Bairro = "Bairro Novo",
                    Cidade = "Cidade Nova",
                    Estado = "Estado Novo",
                    Cep = "87654321"
                }
            };

            var outroFornecedor = new Fornecedor
            {
                Id = Guid.NewGuid(), // ID diferente
                Documento = "45922445081",
                TipoFornecedor = TipoFornecedor.PessoaFisica,
                Ativo = true,
                Endereco = new Endereco
                {
                    Logradouro = "Rua Nova",
                    Numero = "456",
                    Bairro = "Bairro Novo",
                    Cidade = "Cidade Nova",
                    Estado = "Estado Novo",
                    Cep = "87654321"
                }
            };

            var mocker = new AutoMocker();
            var fornecedorService = mocker.CreateInstance<FornecedorService>();
            // Garante que não existe outro fornecedor com o mesmo documento e ID diferente
            mocker.GetMock<IFornecedorRepository>()
                .Setup(r => r.Buscar(It.IsAny<Expression<Func<Fornecedor, bool>>>()))
                .ReturnsAsync(new List<Fornecedor> { outroFornecedor });

            // Act
            await fornecedorService.Atualizar(fornecedor);

            // Assert
            mocker.GetMock<IFornecedorRepository>().Verify(r => r.Atualizar(fornecedor), Times.Never);
        }
    }
}
