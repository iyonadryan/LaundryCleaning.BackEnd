using LaundryCleaning.Scheduler;
using LaundryCleaning.Scheduler.Config;
using LaundryCleaning.Scheduler.Extensions;
using LaundryCleaning.Service.Data;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

// Load YAML
var schedulerConfig = YamlConfigLoader.Load("cron-value.yaml");
builder.Services.AddSingleton(schedulerConfig);

builder.Services.AddProjectServices(); // Add Service Registration

builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
