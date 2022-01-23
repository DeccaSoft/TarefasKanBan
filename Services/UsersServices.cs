using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TarefasKanBan.Data;
using TarefasKanBan.Models;

namespace Aula6.Services
{
    public class UsersServices
    {
        private readonly TarefasContext _tarefasContext;
        public UsersServices(TarefasContext tarefasContext)
        {
            _tarefasContext = tarefasContext;
            //_dbContext.Database.EnsureCreated();
        }

        //Lista usuários que contenham o 'term' em seu Nome, Login, CPF ou Email... Paginando a partir do usuário 'offset', listando de 'limit' em 'limit' usuários
        public List<User> SearchUsers([FromQuery] string searchTerm, int initialRecord, int limitPerPage)
        {
            List<User> users = _tarefasContext.Users.Where(u => u.Name.Contains(searchTerm) || u.Login.Contains(searchTerm) || u.EMail.Contains(searchTerm) || u.CPF.Contains(searchTerm)).Skip(initialRecord).Take(limitPerPage).ToList();
            return users;
        }
    }
}
