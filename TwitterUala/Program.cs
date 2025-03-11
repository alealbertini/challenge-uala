using Microsoft.EntityFrameworkCore;
using TwitterUala.Application.Contracts.Applicaction;
using TwitterUala.Application.Contracts.Infrastructure;
using TwitterUala.Application.UseCases;
using TwitterUala.Infrastructure;
using TwitterUala.Infrastructure.Database;
using TwitterUala.Infrastructure.Handlers;
using TwitterUala.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContextFactory<TwitterDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("Default"), 
    b =>
    {
        b.MigrationsAssembly(nameof(TwitterUala));
    }));

builder.Services.AddSingleton<IUnitOfWork, UnitOfWork>();
builder.Services.AddSingleton(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddSingleton<DbContext, TwitterDbContext>();

builder.Services.AddScoped<IFollowUserService, FollowUserService>();
builder.Services.AddScoped<IPublishTweetService, PublishTweetService>();
builder.Services.AddScoped<ITimelineService, TimelineService>();
builder.Services.AddScoped<ICreateUserService, CreateUserService>();
builder.Services.AddScoped<IFollowingRepository, FollowingRepository>();

builder.Services.AddExceptionHandler<ExceptionHandler>();

var app = builder.Build();
using (var sp = app.Services.CreateScope())
{
    sp.ServiceProvider.GetRequiredService<TwitterDbContext>().Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(_ => { });

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
