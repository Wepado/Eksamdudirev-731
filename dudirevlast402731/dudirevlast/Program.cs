using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();


List<Triangle> repo = new List<Triangle>()
{
    new Triangle(3, 4, 5) 
};


app.MapGet("/triangles", () => repo);


app.MapGet("/triangle/{id}", (int id) =>
{
    var triangle = repo.Find(t => t.Id == id);
    return triangle is not null ? Results.Ok(new
    {
        triangle.Id,
        Sides = triangle.GetSides(),
        Perimeter = triangle.PerimeterInfo() 
    }) : Results.NotFound();
});


app.MapPost("/triangle", (Triangle t) =>
{
    
    if (t.SideA <= 0 || t.SideB <= 0 || t.SideC <= 0)
    {
        return Results.BadRequest("Стороны треугольника не могут быть отрицательными или равными нулю.");
    }

    
    if (t.SideA + t.SideB <= t.SideC || t.SideA + t.SideC <= t.SideB || t.SideB + t.SideC <= t.SideA)
    {
        return Results.BadRequest("Две Стороны одного треугольника не могут быть больше третьей стороны.");
    }

    
    repo.Add(t);
    return Results.Created($"/triangle/{t.Id}", t); 
});


app.MapGet("/triangle/{id}/info", (int id) =>
{
    var triangle = repo.Find(t => t.Id == id);
    if (triangle == null)
    {
        return Results.NotFound();
    }
    
  
    return Results.Ok(new
    {
        triangle.Id,
        Sides = triangle.GetSides(),
        Perimeter = triangle.PerimeterInfo() 
    });
});


app.MapDelete("/triangle/{id}", (int id) =>
{
    var triangle = repo.Find(t => t.Id == id);
    if (triangle != null)
    {
        repo.Remove(triangle); 
        return Results.NoContent(); 
    }
    return Results.NotFound(); 
});

app.Run();


class Triangle
{
    private static int _idCounter = 1;

    public Triangle(double sideA, double sideB, double sideC)
    {
        Id = _idCounter++;
        SideA = sideA;
        SideB = sideB;
        SideC = sideC;
    }

    public int Id { get; }
    public double SideA { get; set; }
    public double SideB { get; set; }
    public double SideC { get; set; }

      public double Perimeter() => SideA + SideB + SideC;

        public string GetSides()
    {
        return $"Side A: {SideA}, Side B: {SideB}, Side C: {SideC}";
    }

    
    public string PerimeterInfo()
    {
        double perimeter = Perimeter();
        return $"Perimeter: {perimeter}";
    }
}
