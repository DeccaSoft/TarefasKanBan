using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TarefasKanBan.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string Login { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string EMail { get; set; }
        [Required]
        public string CPF { get; set; }
        public string Phone { get; set; }
        public DateTime Birthday { get; set; }
        public string Role { get; set; }

        public List<Tarefa>? Tarefas { get; set; } = null;
    }
}