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
}
