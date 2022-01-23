using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TarefasKanBan.Data;
using TarefasKanBan.Models;
using TarefasKanBan.Models.Enums;
using TarefasKanBan.Services;

namespace TarefasKanBan.Controllers
{
    [ApiController]
    [Route("Tarefas")]
    //[Authorize(Roles = "Adm")]
    public class TarefasController : ControllerBase
    {
        private readonly TarefasContext _tarefasContext;
        private readonly TasksServices _tasksServices;
        public TarefasController(TarefasContext tarefasContext, TasksServices tasksServices)
        {
            _tarefasContext = tarefasContext;
            _tasksServices = tasksServices;
        }
        
        [HttpPost]
        public IActionResult CreateTask([FromBody] Tarefa tarefa)
        {
            if(!ModelState.IsValid) {return BadRequest(ModelState);}
            if(tarefa.Status != Status.Created)
            {
                ModelState.AddModelError("Status", "Uma Tarefa só pode ser criada com Status 'Created (0)'");
                return BadRequest(ModelState);
            }
            if (tarefa.IniciationDate != null || tarefa.FinalizationDate != null || tarefa.CancellationDate != null || tarefa.Reopened != false)
            {
                ModelState.AddModelError("IniciationDate", "Uma Tarefa NÃO pode ser criada como Iniciada!");
                ModelState.AddModelError("FinalizationDate", "Uma Tarefa NÃO pode ser criada como Finalizada!");
                ModelState.AddModelError("CancellationDate", "Uma Tarefa NÃO pode ser criada como Cancelada!");
                ModelState.AddModelError("Reopened", "Uma Tarefa NÃO pode ser criada como Reaberta!");
                return BadRequest(ModelState);
            }
            bool alreadyExists = (_tarefasContext.Tarefas.Find(tarefa.Id) != null);
            if(alreadyExists)
            {
                ModelState.AddModelError("Status", "Tarefa já Cadastrada!");
                return BadRequest(ModelState);
            } 

            return Ok(_tasksServices.CreateTask(tarefa));
        }

        [HttpGet]
        public IActionResult GetTasks()
        {
            if(_tasksServices.GetTasks().Count() == 0) {return Ok("Ainda NÃO há Tarefas Cadastradas!");}
            return Ok(_tasksServices.GetTasks());
        }

        [HttpGet("{id}")]
        public IActionResult GetTaskById(int id)
        {
            //var tarefaModel = _tarefasContext.Tarefas.Find(id);
            if(_tasksServices.GetTaskById(id) is null) {return BadRequest("Tarefa NÃO Cadastrada!");}
            return Ok(_tasksServices.GetTaskById(id));
        }

        [HttpPut]
        public IActionResult UpdateTask([FromBody] Tarefa tarefa)
        {
            if(!ModelState.IsValid) {return BadRequest(ModelState);}
            if(_tasksServices.UpdateTask(tarefa) is null) {return BadRequest("Tarefa NÃO Cadastrada!");}
            return Ok(_tasksServices.UpdateTask(tarefa));
        }

        [HttpDelete]
        public IActionResult DeleteTask(int id)
        {
            if(_tasksServices.DeleteTask(id) is null) {return Ok(BadRequest("Tarefa NÃO Cadastrada!"));}
            return Ok(_tasksServices.DeleteTask(id));
        }

        //Iniciar uma Tarefa
        [HttpPut("Start/{id}")]
        public IActionResult StartTask(int id)
        {
            var tarefaModel = _tarefasContext.Tarefas.Find(id);
            if(tarefaModel is null) {return BadRequest("Tarefa NÃO Cadastrada!");}
            if(tarefaModel.Status == Status.Initiated) {return BadRequest("Tarefa Já Iniciada!");}
            if(tarefaModel.Status == Status.Finished) {return BadRequest("Não se pode Iniciar uma Tarefa Já Finalizada, mas você pode Reabri-la...");}
            if(tarefaModel.Status == Status.Canceled) {return BadRequest("Não se pode Iniciar uma Tarefa Cancelada!");}
            tarefaModel.Status = Status.Initiated;
            tarefaModel.IniciationDate = DateTime.Now;
            _tarefasContext.Update(tarefaModel);
            _tarefasContext.SaveChanges();
            return Ok(tarefaModel);
        }

        //Finalizar uma Tarefa
        [HttpPut("Finish/{id}")]
        public IActionResult FinishTask(int id)
        {
            var tarefaModel = _tarefasContext.Tarefas.Find(id);
            if(tarefaModel is null) {return BadRequest("Tarefa NÃO Cadastrada!");}
            if(tarefaModel.Status == Status.Finished) {return BadRequest("Tarefa Já Finalizada!");}
            if(tarefaModel.Status == Status.Created) {return BadRequest("Não se pode Finalizar uma Tarefa ainda não Iniciada, mas você pode Cancela-la...");}
            if(tarefaModel.Status == Status.Canceled) {return BadRequest("Não se pode Finalizar uma Tarefa Cancelada!");}
            tarefaModel.Status = Status.Finished;
            tarefaModel.FinalizationDate = DateTime.Now;
            _tarefasContext.Update(tarefaModel);
            _tarefasContext.SaveChanges();
            return Ok(tarefaModel);
        }

        //Cancelar uma Tarefa
        [HttpPut("Cancel/{id}")]
        public IActionResult CancelTask(int id)
        {
            var tarefaModel = _tarefasContext.Tarefas.Find(id);
            if(tarefaModel is null) {return BadRequest("Tarefa NÃO Cadastrada!");}
            if(tarefaModel.Status == Status.Canceled) {return BadRequest("Tarefa Já Cancelada!");}
            if(tarefaModel.Status == Status.Finished) {return BadRequest("Não se pode Cancelar uma Tarefa Já Finalizada, mas você pode Raebri-la...");}
            tarefaModel.Status = Status.Canceled;
            tarefaModel.CancellationDate = DateTime.Now;
            _tarefasContext.Update(tarefaModel);
            _tarefasContext.SaveChanges();
            return Ok(tarefaModel);
        }

        //Reabrir uma Tarefa
        [HttpPut("Reopen/{id}")]
        public IActionResult ReopenTask(int id)
        {
            var tarefaModel = _tarefasContext.Tarefas.Find(id);
            if(tarefaModel is null) {return BadRequest("Tarefa NÃO Cadastrada!");}
            if(tarefaModel.Status == Status.Initiated) {return BadRequest("Tarefa Já está Aberta!");}
            if(tarefaModel.Status == Status.Created) 
            {
                return BadRequest("Não se pode Reabrir uma Tarefa que nunca Iniciada!!");
            }
            tarefaModel.Status = Status.Initiated;
            tarefaModel.Reopened = true;
            tarefaModel.IniciationDate = DateTime.Now;
            tarefaModel.FinalizationDate = null;
            tarefaModel.CancellationDate = null;
            _tarefasContext.Update(tarefaModel);
            _tarefasContext.SaveChanges();
            return Ok(tarefaModel);
        }
    }
}