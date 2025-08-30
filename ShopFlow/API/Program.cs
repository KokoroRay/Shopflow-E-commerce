var builder = WebApplication.CreateBuilder(args);

// --- API essentials ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApiVersioning(opt =>
{
    opt.AssumeDefaultVersionWhenUnspecified = true;
    opt.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
});

// --- Application/Infrastructure wiring ---
builder.Services.AddApplication();    // extension method ở Application (nếu dùng)
builder.Services.AddInfrastructure(builder.Configuration); // đăng ký DbContext, repo, v.v.

// Validation, AutoMapper
builder.Services.AddAutoMapper(typeof(Program)); // hoặc typeof(SomeProfile).Assembly
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// CORS (ví dụ)
builder.Services.AddCors(opt =>
    opt.AddPolicy("Default", p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseCors("Default");
app.UseAuthorization();
app.MapControllers();

// (Optional) apply migrations at startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}
app.Run();
