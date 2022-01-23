using System.Collections.Generic;
using System.Linq;
using TarefasKanBan.Data;
using TarefasKanBan.Models;

namespace TarefasKanBan.Services
{
    public class TasksServices
    {
        private readonly TarefasContext _tarefasContext;
        public TasksServices(TarefasContext tarefasContext)
        {
            _tarefasContext = tarefasContext;
            //_dbContext.Database.EnsureCreated();
        }

        public Tarefa CreateTask(Tarefa tarefa)
        {
            _tarefasContext.Tarefas.Add(tarefa);
            var userModel = _tarefasContext.Users.Find(tarefa.UserId);
            userModel.Tarefas.Add(tarefa);
            _tarefasContext.SaveChanges();
            return tarefa;
        }

        public List<Tarefa> GetTasks()
        {
            return _tarefasContext.Tarefas.ToList();
        }

        public Tarefa GetTaskById(int id)
        {
            var tarefaModel = _tarefasContext.Tarefas.Find(id);
            if(tarefaModel is null) {return null;}
            return tarefaModel;
        }

        public Tarefa UpdateTask(Tarefa tarefa)
        {
            var tarefaModel = _tarefasContext.Tarefas.Find(tarefa.Id);
            if(tarefaModel is null) {return null;}
            tarefaModel.Name = tarefa.Name;
            tarefaModel.Description = tarefa.Description;
            tarefaModel.Priority = tarefa.Priority;
            _tarefasContext.SaveChanges();
            return tarefaModel;
        }

        public Tarefa DeleteTask(int id)
        {
            Tarefa tarefaModel = _tarefasContext.Tarefas.Find(id);
            if(tarefaModel is null) {return null;}
            _tarefasContext.Tarefas.Remove(tarefaModel);
            _tarefasContext.SaveChanges();
            return tarefaModel;
        }
    }
}