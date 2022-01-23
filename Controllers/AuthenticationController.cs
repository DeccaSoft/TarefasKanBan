using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TarefasKanBan.Data;
using TarefasKanBan.Models;
using TarefasKanBan.Requests;

namespace TarefasKanBan.Controllers
{
    [ApiController]
    [Route("authentication")]
    public class AuthenticationController : ControllerBase
    {
        private readonly TarefasContext _tarefasContext;

        public AuthenticationController(TarefasContext tarefasContext)
        {
            _tarefasContext = tarefasContext;
        }

        [HttpPost]
        public IActionResult Login(LoginRequest loginRequest)   //Recebe E-mail e Senha
        {
            //var user = new User{Email = loginRequest.Email, Password = loginRequest.Password}; => Aqui teríamos o usuário Disponível
            var user = _tarefasContext.Users.FirstOrDefault(u => u.EMail == loginRequest.EMail && u.Password == loginRequest.Password); //Checa E-Mail e Senha e Retorna o Usuário

            if (user is null || !BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.Password))
            {
                return BadRequest("Usuário ou senha inválido(s)");
            }
            
            var token = GenerateToken(user);    //Caso usuário exista, Gera o Token

            return Ok(token);                   //Retorna o Token ao usuário
        }

        public static string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();               //Objeto JWT para o acesso ao Token
            var key = Encoding.ASCII.GetBytes("ChaveTarefasKanBan");    //Transformaa Chave de String para Bytes (Array)
            var tokenDescriptor = new SecurityTokenDescriptor
            {                                                               //Informações do Token
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.EMail),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddHours(1),                      //Tempo de Validade do Token (1Hora)
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature) //Tipo de Algorítimo da Assinatura - Sha256
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);          //Cria o Token
            return tokenHandler.WriteToken(token);                          //Transforma o Token em String para ser Retornado
        }
    }
}