using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TarefasKanBan.Data;
using TarefasKanBan.Models;
using TarefasKanBan.Services;

namespace TarefasKanBan.Controllers
{
    [ApiController]
    [Route("Users")]
    //[Authorize(Roles = "Adm")]
    public class UsersController : ControllerBase
    {
        private readonly TarefasContext _tarefasContext;
        private readonly UsersServices _usersServices;

        public UsersController(TarefasContext tarefasContext, UsersServices usersServices)
        {
            _tarefasContext = tarefasContext;
            _usersServices = usersServices;

        }
        
        [HttpPost]
        public IActionResult RegisterUser([FromBody] User user)
        {            
            if(!ModelState.IsValid){return BadRequest(ModelState);}
            bool alreadyExists = ((_tarefasContext.Users.Find(user.Id) != null) 
                || (_tarefasContext.Users.FirstOrDefault(u => u.Login == user.Login) != null)
                || (_tarefasContext.Users.FirstOrDefault(u => u.CPF == user.CPF) != null));
            if(alreadyExists)
            {
                ModelState.AddModelError("Status", "Usuário já Cadastrado!");
                return BadRequest(ModelState);
            }

            return Ok(_usersServices.RegisterUser(user));
        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            if(_tarefasContext.Users.ToList().Count() == 0) {return Ok("Ainda NÃO há Usuários Cadastrados!");}

            return Ok(_usersServices.GetUsers());
        }

        [HttpGet("id/{id}")]
        public IActionResult GetUserById(int id)
        {
            if(_usersServices.GetUserById(id) is null) {return BadRequest("Usuário NÃO Cadastrado!");}
            return Ok(_usersServices.GetUserById(id));
        }

        [HttpGet("search")]
        public IActionResult SearchUsers([FromQuery]string searchTerm, int initialRecord = 0, int limitPerPage = 10) 
        {
            if(_usersServices.SearchUsers(searchTerm, initialRecord, limitPerPage).Count == 0)
            {
                return Ok("Nenhum Usuário encontrado para essa Pesquisa!");
            }
            return Ok(_usersServices.SearchUsers(searchTerm, initialRecord, limitPerPage));
        }

        [HttpPut]
        public IActionResult UpdateUser([FromBody] User user)
        {
            if(!ModelState.IsValid) {return BadRequest(ModelState);}
            if(_usersServices.UpdateUser(user) is null) {return BadRequest("Usuário NÃO Cadastrado!");}
            return Ok(_usersServices.UpdateUser(user));
        }

        [HttpDelete]
        public IActionResult DeleteUser(int id)
        {
            if(_usersServices.DeleteUser(id) is null) {return Ok(BadRequest("Usuário NÃO Cadastrado!"));}
            return Ok(_usersServices.DeleteUser(id));
        }
    }
}