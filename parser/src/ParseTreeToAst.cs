using Antlr4.Runtime;
using Strumenta.Python3Parser;
using Strumenta.Sharplasu.Parsing;
using Strumenta.Sharplasu.Validation;
using static Strumenta.Python3Parser.Python3Parser;

namespace ExtensionMethods;

public static class ParseTreeToASTExtensions
{
    public static CompilationUnit ToAst(this File_inputContext context, List<Issue> issues)
    {
        return new CompilationUnit
        {
            Statements = context.stmt().SelectMany(s => s.ToAst(issues)).ToList()
        };
    }

    public static List<Statement> ToAst(this StmtContext context, List<Issue> issues)
    {
        if (context.simple_stmts() != null && context.simple_stmts().simple_stmt() != null)
        {
            return context.simple_stmts().simple_stmt().Select(s => s.ToAst(issues)).ToList();
        }
        else if (context.compound_stmt() != null)
        {
            return new List<Statement> { context.compound_stmt().ToAst(issues) };
        }
      
        issues.Add(new Issue(
            IssueType.SYNTACTIC,
            $"Unexpected node: {context}",
            null,
            IssueSeverity.Error));
        return new List<Statement>();
    }

    public static Statement ToAst(this Simple_stmtContext context, List<Issue> issues)
    {
        if (context.import_stmt() != null)
        {
            return context.import_stmt().ToAst(issues);
        }
      
        throw new NotImplementedException($"{context}");
    }

    public static ImportStatement ToAst(this Import_stmtContext context, List<Issue> issues)
    {
        if (context.import_name() != null)
        {
            return new ImportStatement
            {
                Name = context.import_name().dotted_as_names().
                    GetText()
            };
        }
        else if (context.import_from() != null)
        {
            return new ImportStatement
            {
                From = context.import_from().dotted_name().GetText(),
                Name = context.import_from().import_as_names().GetText()
            };
        }
      
        throw new NotImplementedException($"{context}");
    }
   
    public static Statement ToAst(this Compound_stmtContext context, List<Issue> issues)
    {
        if (context.funcdef() != null)
        {
            return context.funcdef().ToAst(issues);
        }
  
        // TODO
        throw new NotImplementedException($"{context}");
    }
   
    public static FunctionDefinition ToAst(this FuncdefContext context, List<Issue> issues)
    {
        return new FunctionDefinition
        {
            Name = context.name().GetText(),
            Parameters = context.parameters().ToAst(issues),
            // TODO: Body = context.block().ToAst(issues)
        };
    }

    public static List<ParameterDeclaration> ToAst(this ParametersContext context, List<Issue> issues)
    {
        return context.typedargslist() == null ? new List<ParameterDeclaration>() : context.typedargslist().ToAst(issues);
    }
   
    public static List<ParameterDeclaration> ToAst(this TypedargslistContext context, List<Issue> issues)
    {
        var parameters = context.tfpdef().OrderBy(t => t.Position().Start).ToList();
        var defaultValues = context.test().OrderBy(v => v.Position().Start).ToList();
        var defaultValuesByParameter = new Dictionary<TfpdefContext, TestContext>();
  
        foreach (var defaultValue in defaultValues)
        {
            var parameter = parameters.Last(p =>
                p.Position().End < defaultValue.Position().Start);
            if (parameter != null)
                defaultValuesByParameter.Add(parameter, defaultValue);
        }
  
        var parametersWithDefaultValues = parameters
            .Select(p => new KeyValuePair<TfpdefContext, TestContext>(
                p,
                defaultValuesByParameter.ContainsKey(p) ? defaultValuesByParameter[p] : null));
  
        return parametersWithDefaultValues
            .Select(pair => new ParameterDeclaration
            {
                Name = pair.Key.name().GetText(),
                DefaultValue = new StringLiteral { Value = pair.Value?.GetText() }
                // TODO: DefaultValue = pair.Value.ToAst(issues)
            })
            .ToList();
    }
}