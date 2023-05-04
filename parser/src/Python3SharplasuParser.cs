using Antlr4.Runtime;
using ExtensionMethods;
using Strumenta.Sharplasu.Parsing;
using Strumenta.Sharplasu.Validation;

namespace Strumenta.Python3Parser;

public class Python3SharplasuParser : SharpLasuParser<CompilationUnit, Python3Parser, Python3Parser.File_inputContext>
{
    public override Lexer InstantiateLexer(ICharStream charStream)
    {
        return new Python3Lexer(charStream);
    }

    public override Python3Parser InstantiateParser(ITokenStream tokenStream)
    {
        return new Python3Parser(tokenStream);
    }

    protected override CompilationUnit ParseTreeToAst(Python3Parser.File_inputContext parseTreeRoot, bool considerPosition = true, List<Issue> issues = null)
    {
        var cu = parseTreeRoot.ToAst(issues);
        cu.AssignParents();
        return cu;
    }

    protected override Python3Parser.File_inputContext InvokeRootRule(Python3Parser parser)
    {
        return parser.file_input();
    }
}
