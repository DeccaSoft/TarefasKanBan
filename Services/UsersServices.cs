using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TarefasKanBan.Data;
using TarefasKanBan.Models;

namespace TarefasKanBan.Services
{
    public class UsersServices
    {
        private readonly TarefasContext _tarefasContext;
        public UsersServices(TarefasContext tarefasContext)
        {
            _tarefasContext = tarefasContext;
            //_dbContext.Database.EnsureCreated();
        }

        public User RegisterUser(User user){
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            _tarefasContext.Users.Add(user);
            _tarefasContext.SaveChanges();
            return user;
        }

        public List<User> GetUsers()
        {
            return _tarefasContext.Users.Include(t => t.Tarefas).ToList();
        }

        public User GetUserById(int id)
        {
            var userModel = _tarefasContext.Users.Find(id);
            if(userModel is null){return null;}
            return _tarefasContext.Users.Include(t => t.Tarefas).FirstOrDefault(u => u.Id == userModel.Id);
        }

        //Lista usuários que contenham o 'term' em seu Nome, Login, CPF ou Email... Paginando a partir do usuário 'offset', listando de 'limit' em 'limit' usuários
        public List<User> SearchUsers([FromQuery] string searchTerm, int initialRecord, int limitPerPage)
        {
            List<User> users = _tarefasContext.Users.Where(u => u.Name.Contains(searchTerm) || u.Login.Contains(searchTerm) || u.EMail.Contains(searchTerm) || u.CPF.Contains(searchTerm)).Skip(initialRecord).Take(limitPerPage).ToList();
            return users;
        }

        public User UpdateUser(User user)
        {
            var userModel = _tarefasContext.Users.Find(user.Id);
            if(userModel is null) {return null;}
            userModel.Login = user.Login;
            userModel.Name = user.Name;
            userModel.EMail = user.EMail;
            userModel.CPF = user.CPF;
            userModel.Phone = user.Phone;
            userModel.Birthday = user.Birthday;
            userModel.Role = user.Role;
            //_tarefasContext.Update(user);
            _tarefasContext.SaveChanges();
            return userModel;
        }

        public User DeleteUser(int id)
        {
            var userModel = _tarefasContext.Users.Include(t => t.Tarefas).FirstOrDefault(u => u.Id == id);
            if(userModel is null) {return null;}
            var ListTarefas = _tarefasContext.Tarefas.FirstOrDefault(t => t.UserId == id);
            if(ListTarefas != null){
                _tarefasContext.Tarefas.Remove(ListTarefas);
            }
            _tarefasContext.Users.Remove(userModel);
            _tarefasContext.SaveChanges();
            return userModel;
        }
    }
}
