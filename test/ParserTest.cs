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
    }
}
