// Ponto de entrada da API de Controle de Gastos Residenciais.
// A configuração é mantida enxuta aqui; as regras de negócio ficarão nos
// controllers/serviços e o acesso a dados no DbContext (EF Core + SQLite).
var builder = WebApplication.CreateBuilder(args);

// Registra os controllers (MVC/attribute routing) que exporão os endpoints REST.
builder.Services.AddControllers();

// Swagger/OpenAPI para documentar e testar a API durante o desenvolvimento.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger disponível apenas em ambiente de desenvolvimento.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
