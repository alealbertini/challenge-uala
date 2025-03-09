using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TwitterUala.Application.Contracts.Applicaction;
using TwitterUala.Application.Contracts.Infrastructure;
using TwitterUala.Application.UseCases;
using TwitterUala.Infrastructure;
using TwitterUala.Infrastructure.Impl;
using TwitterUala.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<TwitterDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("Default"), 
    b =>
    {
        b.MigrationsAssembly(nameof(TwitterUala));
    }));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<DbContext, TwitterDbContext>();

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
