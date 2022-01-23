using System.Collections.Generic;
using System.Linq;
using Aula6.Services;
using Microsoft.AspNetCore.Mvc;
using TarefasKanBan.Data;
using TarefasKanBan.Models;

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
            _tarefasContext.Users.Add(user);
            _tarefasContext.SaveChanges();
            return Ok(user);
        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            if(_tarefasContext.Users.ToList().Count() == 0) {return Ok("Ainda NÃO há Usuários Cadastrados!");}
            return Ok(_tarefasContext.Users.ToList());
        }

        [HttpGet("id/{id}")]
        public IActionResult GetUserById(int id)
        {
            var userModel = _tarefasContext.Users.Find(id);
            if(userModel is null) {return BadRequest("Usuário NÃO Cadastrado!");}
            return Ok(_tarefasContext.Users.Find(id));
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
            var userModel = _tarefasContext.Users.Find(user.Id);
            if(userModel is null) {return BadRequest("Usuário NÃO Cadastrado!");}
            userModel.Login = user.Login;
            userModel.Name = user.Name;
            userModel.EMail = user.EMail;
            userModel.CPF = user.CPF;
            userModel.Phone = user.Phone;
            userModel.Birthday = user.Birthday;
            userModel.Role = user.Role;
            //_tarefasContext.Update(user);
            _tarefasContext.SaveChanges();
            return Ok(userModel);
        }

        [HttpDelete]
        public IActionResult DeleteUser(int id)
        {
            var userModel = _tarefasContext.Users.Find(id);
            if(userModel is null) {return Ok(BadRequest("Usuário NÃO Cadastrado!"));}
            _tarefasContext.Users.Remove(userModel);
            _tarefasContext.SaveChanges();
            return Ok(userModel);

            // var userModel = GetUserById(id);
            // _tarefasContext.Users.Remove(userModel);
            // _tarefasContext.SaveChanges();
            // return userModel;
        }
    }
}