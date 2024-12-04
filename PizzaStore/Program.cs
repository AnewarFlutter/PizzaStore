using Microsoft.OpenApi.Models; 
using Microsoft.EntityFrameworkCore;
using PizzaStore.Models;

var builder = WebApplication.CreateBuilder(args);

// Ajout de la chaîne de connexion
var connectionString = builder.Configuration.GetConnectionString("Pizzas") ?? "Data Source=Pizzas.db";

// Ajout du contexte de base de données en mémoire
builder.Services.AddDbContext<PizzaDb>(options => options.UseInMemoryDatabase("items"));

builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen(c =>  
{ 
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "API PizzaStore", 
        Description = "Faire les pizzas que vous aimez", 
        Version = "v1" 
    }); 
}); 
var app = builder.Build(); 
app.UseSwagger(); 
app.UseSwaggerUI(c => 
{ 
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PizzaStore API V1"); 
}); 

// Ajout de l'itinéraire "/pizzas"
app.MapGet("/pizzas", async (PizzaDb db) => await db.Pizzas.ToListAsync());

// Ajout de l'itinéraire POST "/pizza"
app.MapPost("/pizza", async (PizzaDb db, Pizza pizza) =>
{
    await db.Pizzas.AddAsync(pizza);
    await db.SaveChangesAsync();
    return Results.Created($"/pizza/{pizza.Id}", pizza);
});

// Nouvel endpoint pour obtenir une pizza par ID
app.MapGet("/pizza/{id}", async (PizzaDb db, int id) => await db.Pizzas.FindAsync(id));

// Nouvel endpoint pour mettre à jour une pizza
app.MapPut("/pizza/{id}", async (PizzaDb db, Pizza updatepizza, int id) =>
{
    var pizza = await db.Pizzas.FindAsync(id);
    if (pizza is null) return Results.NotFound();
    pizza.Nom = updatepizza.Nom;
    pizza.Description = updatepizza.Description;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Nouvel endpoint pour supprimer une pizza
app.MapDelete("/pizza/{id}", async (PizzaDb db, int id) =>
{
    var pizza = await db.Pizzas.FindAsync(id);
    if (pizza is null)
    {
        return Results.NotFound();
    }
    db.Pizzas.Remove(pizza);
    await db.SaveChangesAsync();
    return Results.Ok();
});

app.MapGet("/", () => "Bonjour Sénégal!"); 
app.Run();