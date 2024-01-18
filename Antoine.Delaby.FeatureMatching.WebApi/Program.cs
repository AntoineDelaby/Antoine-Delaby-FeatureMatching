using Microsoft.AspNetCore.Mvc;
using Antoine.Delaby.FeatureMatching;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Méthode post de l'API
app.MapPost("/FeatureMatching", async ([FromForm] IFormFileCollection files) =>
{
    if (files.Count != 2)
        return Results.BadRequest();
    using var objectSourceStream = files[0].OpenReadStream();
    using var objectMemoryStream = new MemoryStream();
    objectSourceStream.CopyTo(objectMemoryStream);
    var imageObjectData = objectMemoryStream.ToArray();
    using var sceneSourceStream = files[1].OpenReadStream();
    using var sceneMemoryStream = new MemoryStream();
    sceneSourceStream.CopyTo(sceneMemoryStream);
    var imageSceneData = sceneMemoryStream.ToArray();
    
    // Your implementation code
    // Notre library a besoin d'une liste de tableaux d'octets.
    // Comme on ne demande qu'une image avec l'API Web, on ajoute cette image dans une liste
    IList<byte[]> imageSceneDataList = new List<byte[]>();
    imageSceneDataList.Add(imageSceneData);
    
    var results = new ObjectDetection().DetectObjectInScenesAsync(imageObjectData, imageSceneDataList);
    results.Wait();
    var result = results.Result;
    
    // La méthode ci-dessous permet de retourner une image depuis un tableau de bytes,
    var imageData = new byte();
    return Results.File(result[0].ImageData, "image/png");
}).DisableAntiforgery(); 

app.Run();