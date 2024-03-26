using Scraper.API;
using Scraper.API.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5124", $"http://{Essentials.Net.Dns.GetLocalIPAddress()}:5124");

builder.Services.AddDataStorage(builder.Configuration);
builder.Services.AddCacheDataStorage(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddCors(option =>
{
    option.AddPolicy(LC.ALLOWED_ORIGIN_KEY,
        builder => builder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
        );
});
builder.Services.AddCustomAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseCors(LC.ALLOWED_ORIGIN_KEY);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
