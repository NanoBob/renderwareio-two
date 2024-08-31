
//var inputPath = "C:\\code\\csharp\\RenderWareIoTwo\\RenderWareIoTwo.Tests\\Files\\Dff\\large-input.dff";
//var outputPath = "output.col";

//Examples.GenerateLargeCol(inputPath, outputPath);

//var inputs = @"C:\Program Files (x86)\MTA San Andreas 1.6\server\mods\deathmatch\resources\chunks\chunks";
//foreach (var dffPath in Directory.GetFiles(inputs, "*.dff"))
//{
//    var colPath = Path.Join(Path.GetDirectoryName(dffPath), $"{Path.GetFileNameWithoutExtension(dffPath)}.col");
//    if (File.Exists(colPath))
//        File.Delete(colPath);

//    Examples.GenerateCol1(dffPath, colPath);

//    Console.WriteLine(colPath);
//}

//var files = new Dictionary<string, string>()
//{
//    ["C:\\code\\csharp\\RenderWareIoTwo\\RenderWareIoTwo.Tests\\Files\\Dff\\cube.dff"] = "C:\\code\\csharp\\RenderWareIoTwo\\RenderWareIoTwo.Tests\\Files\\Col",
//    ["C:\\code\\csharp\\RenderWareIoTwo\\RenderWareIoTwo.Tests\\Files\\Dff\\monkey.dff"] = "C:\\code\\csharp\\RenderWareIoTwo\\RenderWareIoTwo.Tests\\Files\\Col",
//    ["C:\\code\\csharp\\RenderWareIoTwo\\RenderWareIoTwo.Tests\\Files\\Dff\\donut.dff"] = "C:\\code\\csharp\\RenderWareIoTwo\\RenderWareIoTwo.Tests\\Files\\Col",
//};

//foreach (var (source, targetDirectory) in files)
//{
//    var colPath1 = Path.Join(targetDirectory, $"{Path.GetFileNameWithoutExtension(source)}-1.col");
//    var colPath3 = Path.Join(targetDirectory, $"{Path.GetFileNameWithoutExtension(source)}-3.col");
//    if (File.Exists(colPath1))
//        File.Delete(colPath1);
//    if (File.Exists(colPath3))
//        File.Delete(colPath3);

//    Examples.GenerateCol1(source, colPath1);
//    Examples.GenerateCol3(source, colPath3);

//    Console.WriteLine(source);
//}

Examples.GenerateEmptyImg("C:\\code\\csharp\\RenderWareIoTwo\\RenderWareIoTwo.Tests\\Files\\Img\\input.img");