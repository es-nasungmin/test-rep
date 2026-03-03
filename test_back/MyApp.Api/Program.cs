using MyApp.Api.Endpoints;
using MyApp.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// AI 요약 서비스 등록
builder.Services.AddSingleton<ISummaryService, SummaryService>();

// CORS 추가
builder.Services.AddCors(options =>
{
    options.AddPolicy("vue",
        policy =>
        {       
            policy.WithOrigins("http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("vue");

// AI 요약 엔드포인트
app.MapSummaryEndpoints();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching", "Windy", "Rainy", "Cloudy"
};

var todos = new Dictionary<int, TodoItem>();
var nextTodoId = 0;

app.MapGet("/", () => Results.Redirect("/weatherforecast"));

app.MapGet("/weatherforecast", () =>
{           
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapGet("/todos", () =>
{
    var items = todos.Values.OrderBy(todo => todo.Id);
    return Results.Ok(items);
})
.WithName("GetTodos");

app.MapGet("/todos/{id:int}", (int id) =>
{
    return todos.TryGetValue(id, out var todo)
        ? Results.Ok(todo)
        : Results.NotFound();
})
.WithName("GetTodoById");

app.MapPost("/todos", (CreateTodoRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.Title))
    {
        return Results.BadRequest(new { message = "Title is required." });
    }

    var id = Interlocked.Increment(ref nextTodoId);
    var todo = new TodoItem(id, request.Title.Trim(), false, DateTime.UtcNow);
    todos[id] = todo;

    return Results.Created($"/todos/{id}", todo);
})
.WithName("CreateTodo");

app.MapPut("/todos/{id:int}", (int id, UpdateTodoRequest request) =>
{
    if (!todos.TryGetValue(id, out var existing))
    {
        return Results.NotFound();
    }

    if (string.IsNullOrWhiteSpace(request.Title))
    {
        return Results.BadRequest(new { message = "Title is required." });
    }

    var updated = existing with
    {
        Title = request.Title.Trim(),
        IsCompleted = request.IsCompleted
    };

    todos[id] = updated;
    return Results.Ok(updated);
})
.WithName("UpdateTodo");

app.MapDelete("/todos/{id:int}", (int id) =>
{
    return todos.Remove(id)
        ? Results.NoContent()
        : Results.NotFound();
})
.WithName("DeleteTodo");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

record TodoItem(int Id, string Title, bool IsCompleted, DateTime CreatedAt);

record CreateTodoRequest(string Title);

record UpdateTodoRequest(string Title, bool IsCompleted);