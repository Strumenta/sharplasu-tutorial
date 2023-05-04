using ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Strumenta.Sharplasu.Testing;
using static Strumenta.Sharplasu.Testing.Asserts;

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
        
        // Test the function declaration: _plates_simulation
        var expectedPlatesSimulation = new FunctionDefinition
        {
            Name = "_plates_simulation",
            Parameters = new IgnoreChildren<ParameterDeclaration>()
        };
        AssertASTsAreEqual(expectedPlatesSimulation, platesSimulationFuncDef);


        var expectedTempsArgument = new ParameterDeclaration
        {
            Name = "temps",
            DefaultValue = new ArrayLiteral
            {
                Value = "[.874,.765,.594,.439,.366,.124]",
                Elements = new IgnoreChildren<Expression>()
            }
        };
        AssertASTsAreEqual(expectedTempsArgument, temps);
    }
    
    [TestMethod]
    public void TestAST()
    {
        var expectedAST = new CompilationUnit
        {
            Statements = new List<Statement>
            {
                new ImportStatement { Name = "platec" },
                new ImportStatement { Name = "time" },
                new ImportStatement { Name = "numpy" },
                new ImportStatement
                {
                    Name =
                        "Step,add_noise_to_elevation,center_land,generate_world,get_verbose,initialize_ocean_and_thresholds,place_oceans_at_map_borders",
                    From = "worldengine.generation"
                },
                new ImportStatement
                {
                    Name = "World,Size,GenerationParameters",
                    From = "worldengine.model.world"
                },
                new FunctionDefinition
                {
                    Name = "generate_plates_simulation",
                    Parameters = new IgnoreChildren<ParameterDeclaration>()
                },
                new FunctionDefinition
                {
                    Name = "_plates_simulation",
                    Parameters = new IgnoreChildren<ParameterDeclaration>()
                },
                new FunctionDefinition
                {
                    Name = "world_gen",
                    Parameters = new IgnoreChildren<ParameterDeclaration>()
                }
            }
        };
  
        var parser = new Python3SharplasuParser();
        var result = parser.GetTreeForText(GetExamplePythonFileContent());
        AssertASTsAreEqual(expectedAST, result.Root);
    }
    
    [TestMethod]
    public void TestTraversing()
    {
        var parser = new Python3SharplasuParser();
        var result = parser.GetTreeForText(GetExamplePythonFileContent());
      
        // Search all function parameters named "height"
        var heightParameters = result.Root
            .SearchByType<ParameterDeclaration>()
            .Where(p => p.Name == "height")
            .ToList();
        Assert.AreEqual(3, heightParameters.Count);
        
        // We can search the closest ancestor of a given type.
        // For each of the "height" parameters we just found,
        // we expect the closest FunctionDefinition to be respectively:
        // generate_plates_simulation, _plates_simulation and world_gen
        var functionDefinitionNames = heightParameters
            .Select(param => param.FindAncestorOfType<FunctionDefinition>())
            .Select(funcDef => funcDef.Name)
            .ToList();
        Assert.IsTrue(
            new [] { "generate_plates_simulation", "_plates_simulation", "world_gen" }
                .SequenceEqual(functionDefinitionNames));
    }
}
