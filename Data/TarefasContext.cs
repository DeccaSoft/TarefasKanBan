//using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TarefasKanBan.Models;

namespace TarefasKanBan.Data
{
    public class TarefasContext : DbContext
    {
        public TarefasContext(DbContextOptions<TarefasContext> options) : base(options){ }

        // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        // {
        //     //optionsBuilder.UseSqlite(new SqliteConnection("Filename=TarefasContext.db"));
        //     //optionsBuilder.UseMySql(new MySqlConnection("server=localhost;database=DBContext;uid=root;pwd=MS1778amA"));
        // }

        public DbSet<Tarefa> Tarefas {get; set;}
        public DbSet<User> Users {get; set;}
    }
}