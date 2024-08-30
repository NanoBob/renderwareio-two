using RenderWareIoTwo.Formats.Dff;

var inputPath = "input.dff";
var outputPath = "output.dff";

using var input = File.OpenRead(inputPath);
var dff = new DffFile(input);
input.Close();

Console.WriteLine(dff);

using var output = File.OpenWrite(outputPath);
dff.WriteTo(output);
output.Close();


var inputBuffer = File.ReadAllBytes(inputPath);
var outputBuffer = File.ReadAllBytes(outputPath);

for (int i = 0; i < inputBuffer.Length; i++)
{
    if (inputBuffer[i] != outputBuffer[i])
    {
        throw new Exception($"Files do not match at {i}");
    }
}

Console.WriteLine("Files match.");
