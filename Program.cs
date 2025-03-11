using Microsoft.EntityFrameworkCore; 
using TodoApi.Models; 

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySQL(builder.Configuration.GetConnectionString("ToDoDB")));

builder.Services.AddEndpointsApiExplorer();  
builder.Services.AddSwaggerGen(); 

// הוספת CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("http://localhost:3000") 
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); 
    });
});

var app = builder.Build();

app.UseCors("AllowSpecificOrigin");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();  
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ToDo API V1");
        c.RoutePrefix = string.Empty; 
    });
}

// 1. שליפת כל הפריטים
app.MapGet("/items", async (ApplicationDbContext db) =>
{
    var items = await db.Items.ToListAsync();  
    return Results.Ok(items);
});

// 2. שליפת פריט לפי ID
app.MapGet("/items/{id}", async (int id, ApplicationDbContext db) =>
{
    var item = await db.Items.FindAsync(id);  
    
    return item is not null ? Results.Ok(item) : Results.NotFound();
});

// 3. הוספת פריט חדש
app.MapPost("/items", async (Item item, ApplicationDbContext db) =>
{
    db.Items.Add(item);  // הוספת פריט ישירות דרך DbContext
    await db.SaveChangesAsync();  // שמירת השינויים
    return Results.Created($"/items/{item.Id}", item);
});

// 4. עדכון פריט לפי ID
app.MapPut("/items/{id}", async (int id, Item updatedItem, ApplicationDbContext db) =>
{
    var item = await db.Items.FindAsync(id);  // קריאה ישירה ל-DbContext
    // if (item is null) return Results.NotFound();

    item.Name = updatedItem.Name;
    item.IsComplete = updatedItem.IsComplete;

    await db.SaveChangesAsync();  // שמירת השינויים
    return Results.Ok(item);
});

// 5. מחיקת פריט לפי ID
app.MapDelete("/items/{id}", async (int id, ApplicationDbContext db) =>
{
    var item = await db.Items.FindAsync(id);  // קריאה ישירה ל-DbContext
    if (item is null) return Results.NotFound();

    db.Items.Remove(item);  // מחיקת פריט
    await db.SaveChangesAsync();  // שמירת השינויים
    return Results.NoContent();
});

// הפעלת השרת
app.Run();


