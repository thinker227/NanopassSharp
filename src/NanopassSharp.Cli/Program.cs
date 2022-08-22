using NanopassSharp.Builders;
using NanopassSharp.Cli;
using Spectre.Console.Cli;

AstNodeHierarchyBuilder hb = new();

var ast = hb.AddRoot("Ast", "An AST node");

var stmt = ast.AddChild("Stmt", "A statement");

var assignment = stmt.AddChild("Assignment", "An assignment statement");
assignment.AddMember("Identifier", type: "string");
assignment.AddMember("Expression", type: "Expr");

var expr = ast.AddChild("Expr", "An expression");

var unary = expr.AddChild("Unary", "A unary expression");
unary.AddMember("Op", type: "UnaryOperator");
unary.AddMember("Operand", type: "Expr");

var binary = expr.AddChild("Binary", "A binary expression");
binary.AddMember("Left", type: "Expr");
binary.AddMember("Op", type: "BinaryOperator");
binary.AddMember("Right", type: "Expr");

var literal = expr.AddChild("Literal", "A literal value");
literal.AddMember("Value", type: "object");

var h = hb.Build();

var l = h.Roots[0].Children["Expr"].Children["Literal"];
var lb = AstNodeBuilder.FromNode(l);

var hb2 = AstNodeHierarchyBuilder.FromHierarchy(h);

//CommandApp<RunCommand> app = new();
//app.Configure(config => config
//    .SetApplicationName("Nanopass#")
//    .SetApplicationVersion("1.0.0"));
//return await app.RunAsync(args);
return 0;
