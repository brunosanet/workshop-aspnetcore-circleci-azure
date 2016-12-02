using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnitTests.Configuration;
using WebAPIApplication.Models;
using Xunit;

namespace UnitTests.Controllers
{
    public class PessoasControllerIntegrationTest : BaseIntegrationTest
    {
        private const string BaseUrl = "api/pessoas";

        public PessoasControllerIntegrationTest(BaseTestFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task DeveRetornarListaDePessoasVazias()
        {
            var response = await Client.GetAsync(BaseUrl);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<List<Pessoa>>(responseString);

            Assert.Equal(data.Count, 0);
        }

        [Fact]
        public async Task DeveRetornarListaDePessoas()
        {
            var pessoa = new Pessoa
            {
                Nome = "Bruno",
                Twitter = "brunosanet"
            };

            await TestDataContext.AddAsync(pessoa);
            await TestDataContext.SaveChangesAsync();

            var response = await Client.GetAsync(BaseUrl);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<List<Pessoa>>(responseString);

            Assert.Equal(data.Count, 1);
            Assert.Contains(data, x => x.Nome == pessoa.Nome);
        }

        [Fact]
        public async Task DeveCriarPessoa()
        {
            var pessoa = new Pessoa
            {
                Nome = "Bruno",
                Twitter = "brunosanet"
            };

            var strPessoa = JsonConvert.SerializeObject(pessoa);
            var httpContent = new StringContent(strPessoa, Encoding.UTF8, "application/json");

            var response = await Client.PostAsync(BaseUrl, httpContent);
            response.EnsureSuccessStatusCode();

            Assert.Equal("OK", response.StatusCode.ToString());

            var data = TestDataContext.Pessoas.Where(c => c.Nome == pessoa.Nome && c.Twitter == pessoa.Twitter ).FirstOrDefault();  
            Assert.NotNull(data);           
        }

        [Fact]
        public async Task DeveAtualizarPessoa()
        {
            var pessoa = new Pessoa
            {
                Nome = "nova pessoa",
                Twitter = "novo twitter"
            };

            await TestDataContext.AddAsync(pessoa);
            await TestDataContext.SaveChangesAsync();

            var data = TestDataContext.Pessoas.Where(c => c.Nome == pessoa.Nome && c.Twitter == pessoa.Twitter).FirstOrDefault();
            data.Nome = "pessoa alterada";
            data.Twitter = "twitter alterado";

            var strPessoa = JsonConvert.SerializeObject(data);
            var httpContent = new StringContent(strPessoa, Encoding.UTF8, "application/json");

            var response = await Client.PutAsync(BaseUrl, httpContent);
            response.EnsureSuccessStatusCode();

            Assert.Equal("OK", response.StatusCode.ToString());

            var pessoaAlterada = TestDataContext.Pessoas.Where(c => c.Nome == data.Nome && c.Twitter == data.Twitter).FirstOrDefault();
            Assert.NotNull(pessoaAlterada);
        }

    }
}
