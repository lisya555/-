using Microsoft.AspNetCore.Builder;
using System;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();


List<Order> repo = new List<Order>()
{
   new Order(1, "Принтер", 3657, "Завис", "Зубенок Павел Олегович", 89558906754, "Принят", "Виксик")
};

bool isUpdeteStatus = false; //статус обновленя  
string message = "";

app.MapGet("/", () =>
{
    if (isUpdeteStatus)
    {
        string buffer = message;
        isUpdeteStatus = false;
        string mesage = "";
        return Results.Json(new OrderUpdateStatusDTO(repo, buffer));
    }
    else
        return Results.Json(repo);
});

app.MapPost("/", (Order o) => repo.Add(o));

app.MapPut("/{number}", (int number, OrderUpdateDTO dto) =>

{
Order buffer = repo.Find(o => o.Number == number);
if (buffer != null)
{
    buffer.Status = dto.Status;
    isUpdeteStatus = true;
    message += "статус заявки номер " + buffer.Number + "изменен/n";
    if (buffer.Status == "завершено")
        buffer.EndDate = DateTime.Now;
}

{
    Order buffer = repo.Find(o => o.Number == number);
    if (buffer != null)
        return Results.NotFound("такого нет");
    if(buffer.Status != dto.Status)
        buffer.Status = dto.Status;
    if (buffer.Description != dto.Description)
        buffer.Description = dto.Description;
    if (buffer.Master != dto.Master)
        buffer.Master = dto.Master;
    if (dto.Comment != null || dto.Comment != "")
        buffer.Comment.Add(dto.Comment);
    return Results.Json(buffer);
});

app.MapGet("/{number}", (int number) => repo.Find(o => o.Number == number));
app.MapGet("/filter/{param}", (string param) => repo.FindAll(o =>

o.Status == param ||
o.Master == param ||
o.View == param ||
o.Description == param));

app.MapGet("/stat/complCount", () => repo.FindAll(o => o.Status == "завершено"));

app.MapGet("/stat/avrg", () =>
{
    double timeSum = 0;
    int oCount = 0;
    foreach (var o in repo)

        if (o.Status == "зфвершено")
        {
            timeSum += o.TimeExecution;  
            oCount++;
        }
    if (oCount > 0)
        return timeSum / oCount;
    return 0;
});

app.Run();


record class OrderUpdateStatusDTO(List<Order> repo, string massege);

class OrderUpdateDTO
{
    string status;
    string description;
    string master;
    string comment;

    public string Status { get => status; set => status = value; }
    public string Description { get => description; set => description = value; }
    public string Master { get => master; set => master = value; }
    public string Comment { get => comment; set => comment = value; }
}

class Order
{
    int number;
    string view;
    int model;
    string description;
    string fio;
    long phone;
    string status;
    string master;

    public Order(int number, string view, int model, string description, string fio, long phone, string status, string master)
    {
        Number = number;
        View = view;
        Model = model;
        Description = description;
        Fio = fio;
        Phone = phone;
        Status = status;
        Master = "не назначен";
        StartDate = new DateTime();
        EndDate = null;
    }

    public int Number { get => number; set => number = value; }
    public DateTime StartDate
    {
        get; set;
    }
    public DateTime? EndDate { get; set; }
    public double TimeExecution { get => (EndDate - StartDate).Value.TotalDays; }
    public string View { get => view; set => view = value; }
    public int Model { get => model; set => model = value; }
    public string Description { get => description; set => description = value; }
    public string Fio { get => fio; set => fio = value; }
    public long Phone { get => phone; set => phone = value; }
    public string Status { get => status; set => status = value; }
    public string Master { get => master; set => master = value; }
    public List<string> Comment { get; set; } = [];
}


