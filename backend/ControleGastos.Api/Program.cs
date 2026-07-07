// Ponto de entrada da API de Controle de Gastos Residenciais.
// A configuração é mantida enxuta aqui; as regras de negócio ficarão nos
// controllers/serviços e o acesso a dados no DbContext (EF Core + SQLite).
using System.Text.Json.Serialization;
using ControleGastos.Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Política de CORS para permitir que o front-end (React) consuma a API
// diretamente. As origens permitidas são configuráveis via appsettings
// ("Cors:AllowedOrigins"), com um padrão para o dev server do Vite.
const string PoliticaCorsFrontend = "Frontend";
var origensPermitidas = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? new[] { "http://localhost:5173" };

builder.Services.AddCors(options =>
    options.AddPolicy(PoliticaCorsFrontend, policy =>
        policy.WithOrigins(origensPermitidas)
              .AllowAnyHeader()
              .AllowAnyMethod()));

// Registra os controllers (MVC/attribute routing) que exporão os endpoints REST.
// O JsonStringEnumConverter faz o tipo da transação trafegar como texto
// ("Despesa"/"Receita") em vez de número, deixando a API mais legível.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Persistência com SQLite via EF Core. A connection string vem do appsettings.json,
// garantindo que os dados sobrevivam ao fechamento da aplicação (arquivo .db em disco).
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Swagger/OpenAPI para documentar e testar a API durante o desenvolvimento.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Aplica as migrations pendentes na inicialização, criando/atualizando o banco
// automaticamente. Simplifica a execução do teste técnico (sem passos manuais).
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Swagger disponível apenas em ambiente de desenvolvimento.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(PoliticaCorsFrontend);
app.UseAuthorization();
app.MapControllers();

app.Run();
