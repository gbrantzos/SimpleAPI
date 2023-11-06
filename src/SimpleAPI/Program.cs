using SimpleAPI.Infrastructure;

Console.WriteLine(@"
|      ______  __                       __                ______  _______  ______ 
|     /      \|  \                     |  \              /      \|       \|      \
|    |  ▓▓▓▓▓▓\\▓▓______ ____   ______ | ▓▓ ______      |  ▓▓▓▓▓▓\ ▓▓▓▓▓▓▓\\▓▓▓▓▓▓
|    | ▓▓___\▓▓  \      \    \ /      \| ▓▓/      \     | ▓▓__| ▓▓ ▓▓__/ ▓▓ | ▓▓  
|     \▓▓    \| ▓▓ ▓▓▓▓▓▓\▓▓▓▓\  ▓▓▓▓▓▓\ ▓▓  ▓▓▓▓▓▓\    | ▓▓    ▓▓ ▓▓    ▓▓ | ▓▓  
|     _\▓▓▓▓▓▓\ ▓▓ ▓▓ | ▓▓ | ▓▓ ▓▓  | ▓▓ ▓▓ ▓▓    ▓▓    | ▓▓▓▓▓▓▓▓ ▓▓▓▓▓▓▓  | ▓▓  
|    |  \__| ▓▓ ▓▓ ▓▓ | ▓▓ | ▓▓ ▓▓__/ ▓▓ ▓▓ ▓▓▓▓▓▓▓▓    | ▓▓  | ▓▓ ▓▓      _| ▓▓_ 
|     \▓▓    ▓▓ ▓▓ ▓▓ | ▓▓ | ▓▓ ▓▓    ▓▓ ▓▓\▓▓     \    | ▓▓  | ▓▓ ▓▓     |   ▓▓ \
|      \▓▓▓▓▓▓ \▓▓\▓▓  \▓▓  \▓▓ ▓▓▓▓▓▓▓ \▓▓ \▓▓▓▓▓▓▓     \▓▓   \▓▓\▓▓      \▓▓▓▓▓▓
|                             | ▓▓                                                
|                             | ▓▓                                                
|                              \▓▓                                                
|
|    SimpleAPI - version 0.1.0
                              
");
var builder = WebApplication.CreateBuilder(args);
builder.Host
    .PrepareHost(builder.Configuration);
builder.Services
    .AddSystemServices()
    .AddApplicationServices()
    .AddInfrastructure();

var app = builder.Build();
app.ConfigurePipeline(app.Environment);
app.MapEndpoints();

app.Run();
