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

public class ImportStatement : Statement
{
    public string? From { get; set; }
    public string Name { get; set; }
}
public class FunctionDefinition : Statement
{
    public string Name { get; set; }
    public List<ParameterDeclaration> Parameters { get; set; }
    public List<Statement> Body { get; set; }
}

public class ParameterDeclaration : Statement
{
    public string Name { get; set; }
    public Expression DefaultValue { get; set; }
}
public abstract class Expression : Node {}

public abstract class LiteralExpression : Expression
{
    public string Value { get; set; }
}

public class NumberLiteral : LiteralExpression {}
public class BooleanLiteral : LiteralExpression {}
public class StringLiteral : LiteralExpression {}
