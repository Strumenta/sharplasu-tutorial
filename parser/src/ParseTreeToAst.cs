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
            .Select(p => new KeyValuePair<TfpdefContext, TestContext?>(
                p,
                defaultValuesByParameter.ContainsKey(p) ? defaultValuesByParameter[p] : null));
  
        return parametersWithDefaultValues
            .Select(pair => new ParameterDeclaration
            {
                Name = pair.Key.name().GetText(),
                DefaultValue = pair.Value?.ToAst(issues) ?? null
            })
            .ToList();
    }
    
    public static Expression ToAst(this TestContext context, List<Issue> issues)
    {
        if (context.test() == null && context.or_test().Length == 1)
            return context.or_test()[0].ToAst(issues);
  
        // TODO
        throw new NotImplementedException($"{context}");
    }

    public static Expression ToAst(this Or_testContext context, List<Issue> issues)
    {
        if (context.and_test().Length == 1)
            return context.and_test()[0].ToAst(issues);
  
        // TODO
        throw new NotImplementedException($"{context}");
    }

    public static Expression ToAst(this And_testContext context, List<Issue> issues)
    {
        if (context.not_test().Length == 1)
            return context.not_test()[0].ToAst(issues);
  
        // TODO
        throw new NotImplementedException($"{context}");
    }

    public static Expression ToAst(this Not_testContext context, List<Issue> issues)
    {
        if (context.comparison() != null)
            return context.comparison().ToAst(issues);
  
        // TODO
        throw new NotImplementedException($"{context}");
    }

    public static Expression ToAst(this ComparisonContext context, List<Issue> issues)
    {
        if (context.expr().Length == 1)
            return context.expr()[0].ToAst(issues);
  
        // TODO
        throw new NotImplementedException($"{context}");
    }


    public static Expression ToAst(this ExprContext context, List<Issue> issues)
    {
        if (context.xor_expr().Length == 1)
            return context.xor_expr()[0].ToAst(issues);
  
        // TODO
        throw new NotImplementedException($"{context}");
    }

    public static Expression ToAst(this Xor_exprContext context, List<Issue> issues)
    {
        if (context.and_expr().Length == 1)
            return context.and_expr()[0].ToAst(issues);
  
        // TODO
        throw new NotImplementedException($"{context}");
    }

    public static Expression ToAst(this And_exprContext context, List<Issue> issues)
    {
        if (context.shift_expr().Length == 1)
            return context.shift_expr()[0].ToAst(issues);
  
        // TODO
        throw new NotImplementedException($"{context}");
    }

    public static Expression ToAst(this Shift_exprContext context, List<Issue> issues)
    {
        if (context.arith_expr().Length == 1)
            return context.arith_expr()[0].ToAst(issues);
  
        // TODO
        throw new NotImplementedException($"{context}");
    }

    public static Expression ToAst(this Arith_exprContext context, List<Issue> issues)
    {
        if (context.term().Length == 1 &&
            context.term()[0].factor().Length == 1 &&
            context.term()[0].factor()[0].power() != null)
            return context.term()[0].factor()[0].power().atom_expr().atom().ToAst(issues);
  
        // TODO
        throw new NotImplementedException($"{context}");
    }
    
    public static Expression ToAst(this AtomContext context, List<Issue> issues)
    {
        if (context.NUMBER() != null)
            return new NumberLiteral { Value = context.NUMBER().GetText() };
        if (context.STRING() != null && context.STRING().Length > 0)
            return new StringLiteral { Value = string.Join("", context.STRING().Select(s => s.GetText())) };
        if (context.TRUE() != null || context.FALSE() != null)
            return new BooleanLiteral { Value = context.TRUE() != null ? context.TRUE().GetText() : context.FALSE().GetText() };
        if (context.name() != null)
            return new ReferenceExpression { Reference = context.name().GetText() };
        if (context.OPEN_BRACK() != null)
            return new ArrayLiteral
            {
                Value = context.GetText(),
                Elements = context.testlist_comp().test()
                    .Select(t => t.ToAst(issues)).ToList()
            };

        // TODO
        throw new NotImplementedException($"{context}");
    }
}