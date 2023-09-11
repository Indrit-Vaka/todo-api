using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using indrit.vaka.todo_api.models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace todo_api;

public static class todo_api
{
   
    private static List<Todo> _items = new();

    [FunctionName("CreateTodo")]
    public static async Task<IActionResult> CreateTodo(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todo")]
        HttpRequest req, ILogger log)
    {
        log.LogInformation("Creating a new todo list item");
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var input = JsonConvert.DeserializeObject<TodoCreateModel>(requestBody);

        var todo = new Todo { Description = input.Description };
        _items.Add(todo);
        return new OkObjectResult(todo);
    }

    [FunctionName("GetTodos")]
    public static IActionResult GetTodos(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo")]
        HttpRequest req, ILogger log)
    {
        log.LogInformation("Getting todo list items");
        return new OkObjectResult(_items);
    }

    [FunctionName("GetTodoById")]
    public static IActionResult GetTodoById(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo/{id}")]
        HttpRequest req, ILogger log, string id)
    {
        log.LogInformation("Getting todo item");

        var todo = _items.Find(t => t.Id == id);
        if (todo == null) return new NotFoundResult();
        return new OkObjectResult(todo);
    }

    [FunctionName("UpdateTodoById")]
    public static async Task<IActionResult> UpdateTodoById(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "todo/{id}")]
        HttpRequest req, ILogger log, string id)
    {
        log.LogInformation("Updating todo item");

        var todo = _items.Find(t => t.Id == id);
        if (todo == null) return new NotFoundResult();

        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var updated = JsonConvert.DeserializeObject<TodoUpdateModel>(requestBody);
        todo.IsCompleted = updated.IsCompleted;

        if (!string.IsNullOrEmpty(updated.Description))
        {
            todo.Description = updated.Description;
        }

        return new OkObjectResult(todo);
    }

    [FunctionName("DeleteTodoById")]
    public static async Task<IActionResult> DeleteTodoById(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "todo/{id}")]
        HttpRequest req, ILogger log, string id)
    {
        log.LogInformation("Deleting item");

        var todo = _items.Find(t => t.Id == id);
        if (todo == null) return new NotFoundResult();
        _items.Remove(todo);

        return new OkResult();
    }

    [FunctionName("CreatOrUpdate")]
    public static async Task<IActionResult> CreatOrUpdate(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "todo")]
        HttpRequest req, ILogger log)
    {
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var tods = JsonConvert.DeserializeObject<TodoCreateUpdateModel[]>(requestBody);
        var updatedTodos = tods.Select(todo => todo.Id == null ? CreateTodo(todo) : UpdateTodo(todo))
            .Where(todo => todo != null);
        return new OkObjectResult(updatedTodos);
    }

    private static Todo UpdateTodo(TodoCreateUpdateModel updated)
    {
        var todo = _items.Find(t => t.Id == updated.Id);
        if (todo == null) return null;
        todo.IsCompleted = updated.IsCompleted;
        todo.Title = updated.Title;
        todo.Description = updated.Description;

        return todo;
    }

    private static Todo CreateTodo(TodoCreateUpdateModel todo)
    {
        var newTodo = new Todo
        {
            Title = todo.Title,
            Description = todo.Description
        };
        _items.Add(newTodo);
        return newTodo;
    }
}