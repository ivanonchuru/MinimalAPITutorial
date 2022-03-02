global using Microsoft.EntityFrameworkCore;
using MinimalAPITutorial;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

async Task<List<SuperHero>> GetAllHeros(DataContext context) =>
    await context.SuperHeroes.ToListAsync();

app.MapGet("/", () => "Welcome to the Super Hero DB");

app.MapGet("/superhero", async (DataContext context) =>
    await context.SuperHeroes.ToListAsync());

app.MapGet("/superhero/{id}", async (DataContext context, int id) =>
    await context.SuperHeroes.FindAsync(id) is SuperHero hero ?
    Results.Ok(hero) :
    Results.NotFound("Sorry, Hero not found. :/"));

app.MapPost("/superhero", async (DataContext context, SuperHero hero) =>
{
    context.SuperHeroes.Add(hero);
    await context.SaveChangesAsync();
    return Results.Ok(await GetAllHeros(context));
});
app.MapPut("/superhero/{id}", async (DataContext context, SuperHero hero, int id) =>
 {
     var dbHero = await context.SuperHeroes.FindAsync(id);
     if (dbHero == null) return Results.NotFound("No Hero found. :/");

     dbHero.FirstName = hero.FirstName;
     dbHero.LastName = hero.LastName;
     dbHero.HeroName = hero.HeroName;
     await context.SaveChangesAsync();

     return Results.Ok(await GetAllHeros(context));
 });

app.MapDelete("/superhero/{id}", async (DataContext context, int id) =>
{
    var dbHero = await context.SuperHeroes.FindAsync(id);
    if (dbHero == null) return Results.NotFound("No Hero found. :/");

    context.SuperHeroes.Remove(dbHero);
    await context.SaveChangesAsync();

    return Results.Ok(await GetAllHeros(context));

});

app.Run();



