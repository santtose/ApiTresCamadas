using DevIO.Business.Interfaces.Repository;
using DevIO.Business.Models;
using DevIO.Business.Models.Validations;
using DevIO.Business.Services;
using Moq;
using Moq.AutoMock;
using System.Linq.Expressions;

namespace DevIO.Business.Tests
{
    public class FornecedorTests
    {
        private readonly AutoMocker _mocker;
        private readonly FornecedorService _fornecedorService;
        private readonly FornecedorValidation _fornecedorValidation;
        private readonly EnderecoValidation _enderecoValidation;

        public FornecedorTests()
        {
            _mocker = new AutoMocker();
            _fornecedorService = _mocker.CreateInstance<FornecedorService>();
            _fornecedorValidation = _mocker.CreateInstance<FornecedorValidation>();
            _enderecoValidation = _mocker.CreateInstance<EnderecoValidation>();
        }

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

            // Act
            await _fornecedorService.Adicionar(fornecedor);

            // Assert
            Assert.True(fornecedor.EhValido());
            _mocker.GetMock<IFornecedorRepository>().Verify(r => r.Adicionar(fornecedor), Times.Once);
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

            // Act
            await _fornecedorService.Adicionar(fornecedor);

            // Assert
            Assert.False(fornecedor.EhValido());
            _mocker.GetMock<IFornecedorRepository>().Verify(r => r.Adicionar(fornecedor), Times.Never);
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

            // Garante que não existe outro fornecedor com o mesmo documento e ID diferente
            _mocker.GetMock<IFornecedorRepository>()
                .Setup(r => r.Buscar(It.IsAny<Expression<Func<Fornecedor, bool>>>()))
                .ReturnsAsync(new List<Fornecedor>());

            // Act
            await _fornecedorService.Atualizar(fornecedor);

            // Assert
            _mocker.GetMock<IFornecedorRepository>().Verify(r => r.Atualizar(fornecedor), Times.Once);
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

            // Garante que não existe outro fornecedor com o mesmo documento e ID diferente
            _mocker.GetMock<IFornecedorRepository>()
                .Setup(r => r.Buscar(It.IsAny<Expression<Func<Fornecedor, bool>>>()))
                .ReturnsAsync(new List<Fornecedor> { outroFornecedor });

            // Act
            await _fornecedorService.Atualizar(fornecedor);

            // Assert
            _mocker.GetMock<IFornecedorRepository>().Verify(r => r.Atualizar(fornecedor), Times.Never);
        }

        [Fact(DisplayName = "Validar Fornecedor com Falha")]
        [Trait("Categoria", "Fornecedor Service Tests")]
        public void FornecedorService_ValidarFornecedor_DeveFalharDevidoFaltaDePreenchimento()
        {
            // Arrange
            var fornecedor = new Fornecedor
            {
                Nome = "",
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
            // Act
            var resultadoFornecedor = _fornecedorValidation.Validate(fornecedor);
            // Assert
            Assert.False(resultadoFornecedor.IsValid);
        }

        [Fact(DisplayName = "Validar Endereco com Falha")]
        [Trait("Categoria", "Fornecedor Service Tests")]
        public void FornecedorService_ValidarEndereco_DeveFalharDevidoFaltaDePreenchimento()
        {
            // Arrange
            var fornecedor = new Fornecedor
            {
                Nome = "Teste",
                Documento = "12345678944",
                TipoFornecedor = TipoFornecedor.PessoaFisica,
                Ativo = true,
                Endereco = new Endereco
                {
                    Logradouro = "Rua Teste",
                    Numero = "123",
                    Bairro = "",
                    Cidade = "Cidade Teste",
                    Estado = "Estado Teste",
                    Cep = "12345678"
                }
            };
            // Act
            var resultadoEndereco = _enderecoValidation.Validate(fornecedor.Endereco);
            // Assert
            Assert.False(resultadoEndereco.IsValid);
        }
    }
}
