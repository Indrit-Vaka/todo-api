using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace indrit.vaka.todo_api.models;

public class Todo
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsCompleted { get; set; }
}

public class TodoCreateModel
{
    public string Title { get; set; }
    public string Description { get; set; }
}

public class TodoUpdateModel
{
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsCompleted { get; set; }
}
public class TodoCreateUpdateModel
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsCompleted { get; set; }
}


