using System;
using System.ComponentModel.DataAnnotations;
using TarefasKanBan.Models.Enums;

namespace TarefasKanBan.Models
{
    public class Tarefa
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Informe o Nome da Tarefa")]
        public int UserId { get; set; }
        public string Name { get; set; }
        [MaxLength(250, ErrorMessage = "Tamanho Máximo de 250 Caracteres")]
        public string Description { get; set; }
        public Priority Priority { get; set; } = Priority.Normal;
        public Status Status { get; set; } = Status.Created;
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public DateTime? IniciationDate {get; set;} = null;
        public DateTime? FinalizationDate { get; set; } = null;
        public DateTime? CancellationDate { get; set; } = null;
        public bool Reopened { get; set; } = false;
        //public User User { get; set; }
    }
}