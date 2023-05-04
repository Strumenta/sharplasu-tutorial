using Strumenta.Sharplasu.Model;

namespace Strumenta.Python3Parser;

public class CompilationUnit : Node
{
    public List<Statement> Statements { get; set; }
}

public abstract class Statement : Node {}

public class DummyStatement : Statement
{
    public string Content { get; set; }
}
