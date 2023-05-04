using ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Strumenta.Python3Parser.Test;

[TestClass]
public class ParserTest
{
    private string GetExamplePythonFileContent()
    {
        return File.ReadAllText(@"data/plates.py");
    }
  
    [TestMethod]
    public void TestFirstStageParsing()
    {
        var parser = new Python3SharplasuParser();
        var result = parser.ParseFirstStage(GetExamplePythonFileContent());
        Assert.IsTrue(result.Correct);
    }
    
    [TestMethod]
    public void TestParsing()
    {
        var parser = new Python3SharplasuParser();
        var result = parser.GetTreeForText(GetExamplePythonFileContent());
        Assert.IsTrue(result.Correct);
        // We expect 8 statements (the top level statements in the file)
        Assert.AreEqual(8, result.Root.Statements.Count);
        // We expect the Compilation Unit to have 8 children, exactly 1 for each Statement
        Assert.AreEqual(8, result.Root.Children.Count);
        
        // We expect a function definition for _plates_simulation
        var platesSimulationFuncDef = result.Root
            .SearchByType<FunctionDefinition>()
            .FirstOrDefault(funcDef => funcDef.Name == "_plates_simulation");
        Assert.IsNotNull(platesSimulationFuncDef);
        
        // We expect _plates_simulation to contain a definition of an array called temps
        var temps = platesSimulationFuncDef
            .Parameters
            .FirstOrDefault(p => p.Name == "temps");
        Assert.IsNotNull(temps);
        // We also test the expected value of the temps parameter
        Assert.IsTrue(temps.DefaultValue is ArrayLiteral);
        Assert.IsTrue(((ArrayLiteral) temps.DefaultValue).Elements.SequenceEqual(
            new List<Expression>()
            {
                new NumberLiteral { Value = ".874" },
                new NumberLiteral { Value = ".765" },
                new NumberLiteral { Value = ".594" },
                new NumberLiteral { Value = ".439" },
                new NumberLiteral { Value = ".366" },
                new NumberLiteral { Value = ".124" }
            }));
    }
}
