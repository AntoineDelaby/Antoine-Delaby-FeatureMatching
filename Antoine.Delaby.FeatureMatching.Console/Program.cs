using System.Reflection;
using System.Text.Json;

namespace Antoine.Delaby.FeatureMatching.Console;

class PrintProgram
{
    // J'ai enlevé les chemins complets vers mes fichiers images de la configuration de build du projet
    // Car cette solution sera disponible en public sur GitHub
    
    public static void Main(string[] args)
    {
        // Lecture des images
        var executingPath = GetExecutingPath();
        var imageScenesData = new List<byte[]>();
        foreach (var imagePath in Directory.EnumerateFiles(Path.Combine(executingPath,
                     args[1])))
        {
            var imageBytes = File.ReadAllBytes(imagePath);
            imageScenesData.Add(imageBytes);
        }
        var objectImageData = File.ReadAllBytes(Path.Combine(executingPath,
            args[0]));
        
        // Utilisation de la library
        var results = new ObjectDetection().DetectObjectInScenesAsync(objectImageData, imageScenesData);
        results.Wait();
        var detectObjectInScenesResults = results.Result;
        
        // Affichage des résultats
        foreach (var objectDetectionResult in detectObjectInScenesResults)
        {
            System.Console.WriteLine($"Points:{JsonSerializer.Serialize(objectDetectionResult.Points)}");
        }
    }
    
    private static string GetExecutingPath()
    {
        var executingAssemblyPath = Assembly.GetExecutingAssembly().Location;
        var executingPath = Path.GetDirectoryName(executingAssemblyPath);
        return executingPath;
    } 
}