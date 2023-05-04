using Strumenta.Python3Parser;
using Strumenta.Sharplasu.Validation;
using static Strumenta.Python3Parser.Python3Parser;

namespace ExtensionMethods;

public static class ParseTreeToASTExtensions
{
    public static CompilationUnit ToAst(this File_inputContext context, List<Issue> issues)
    {
        return new CompilationUnit
        {
            Statements = context.stmt().Select(s => s.ToAst(issues)).ToList()
        };
    }


    public static Statement ToAst(this StmtContext context, List<Issue> issues)
    {
        return new DummyStatement
        {
            Content = context.GetText()
        };
    }
}
