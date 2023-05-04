using Antlr4.Runtime;
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
           // TODO
           return new List<Statement>();
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
}
