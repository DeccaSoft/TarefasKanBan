using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TarefasKanBan.Data;
using TarefasKanBan.Models;
using TarefasKanBan.Models.Enums;

namespace TarefasKanBan.Controllers
{
    [ApiController]
    [Route("Tarefas")]
    //[Authorize(Roles = "Adm")]
    public class TarefasController : ControllerBase
    {
        private readonly TarefasContext _tarefasContext;
        public TarefasController(TarefasContext tarefasContext)
        {
            _tarefasContext = tarefasContext;
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
            _tarefasContext.Tarefas.Add(tarefa);
            _tarefasContext.SaveChanges();
            return Ok(tarefa);
        }

        [HttpGet]
        public IActionResult GetTasks()
        {
            if(_tarefasContext.Tarefas.ToList().Count() == 0) {return Ok("Ainda NÃO há Tarefas Cadastradas!");}
            return Ok(_tarefasContext.Tarefas.ToList());
        }

        [HttpGet("{id}")]
        public IActionResult GetTaskById(int id)
        {
            var tarefaModel = _tarefasContext.Tarefas.Find(id);
            if(tarefaModel is null) {return BadRequest("Tarefa NÃO Cadastrada!");}
            return Ok(tarefaModel);
        }

        [HttpPut]
        public IActionResult UpdateTask([FromBody] Tarefa tarefa)
        {
            if(!ModelState.IsValid) {return BadRequest(ModelState);}
            var tarefaModel = _tarefasContext.Tarefas.Find(tarefa.Id);
            if(tarefaModel is null) {return BadRequest("Tarefa NÃO Cadastrada!");}
            tarefaModel.Name = tarefa.Name;
            tarefaModel.Description = tarefa.Description;
            tarefaModel.Priority = tarefa.Priority;
            _tarefasContext.SaveChanges();
            return Ok(tarefaModel);
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
            _tarefasContext.Update(tarefaModel);
            _tarefasContext.SaveChanges();
            return Ok(tarefaModel);
        }

        [HttpDelete]
        public IActionResult DeleteTask(int id)
        {
            var tarefaModel = _tarefasContext.Tarefas.Find(id);
            if(tarefaModel is null) {return Ok(BadRequest("Tarefa NÃO Cadastrada!"));}
            _tarefasContext.Tarefas.Remove(tarefaModel);
            _tarefasContext.SaveChanges();
            return Ok(tarefaModel);
        }
    }
}