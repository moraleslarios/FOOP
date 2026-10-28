using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MlDoc;

/// <summary>
/// Construye, linea a linea, el bloque de documentacion XML que se insertara
/// delante de un miembro, respetando su sangria exacta.
/// </summary>
internal static class DocBlock
{
    public static List<string> Build(MemberDeclarationSyntax member, string indent, Options opt)
    {
        var lines = new List<string>();

        // Los miembros que redefinen o implementan otro heredan su documentacion.
        if (InheritsDocumentation(member))
        {
            lines.Add(indent + "/// <inheritdoc/>");
            return lines;
        }

        switch (member)
        {
            case MethodDeclarationSyntax m:              BuildMethod(m, indent, lines, opt);      break;
            case ConstructorDeclarationSyntax c:         BuildConstructor(c, indent, lines);      break;
            case PropertyDeclarationSyntax p:            BuildProperty(p, indent, lines);         break;
            case IndexerDeclarationSyntax ix:            BuildIndexer(ix, indent, lines);         break;
            case DelegateDeclarationSyntax d:            BuildDelegate(d, indent, lines);         break;
            case OperatorDeclarationSyntax o:            BuildOperator(o, indent, lines);         break;
            case ConversionOperatorDeclarationSyntax co: BuildConversion(co, indent, lines);      break;
            case EnumDeclarationSyntax e:                Summary(lines, indent, SummaryBuilder.ForEnum(e.Identifier.ValueText)); break;
            case EnumMemberDeclarationSyntax em:         Summary(lines, indent, SummaryBuilder.ForEnumMember(em.Identifier.ValueText)); break;
            case TypeDeclarationSyntax t:                BuildType(t, indent, lines);             break;
            case EventDeclarationSyntax ev:              Summary(lines, indent, "Se produce cuando cambia " + SummaryBuilder.Code(ev.Identifier.ValueText) + "."); break;
            case EventFieldDeclarationSyntax evf:        BuildEventField(evf, indent, lines);     break;
            case FieldDeclarationSyntax f:               BuildField(f, indent, lines);            break;
        }

        // ultimo paso: reponer las tildes del castellano en la prosa generada
        for (var i = 0; i < lines.Count; i++) lines[i] = Accents.Fix(lines[i]);

        return lines;
    }

    /// <summary>
    /// Indica si el miembro redefine (<c>override</c>) o implementa explicitamente otro,
    /// caso en el que basta con <c>&lt;inheritdoc/&gt;</c>.
    /// </summary>
    private static bool InheritsDocumentation(MemberDeclarationSyntax member)
    {
        if (member.Modifiers.Any(SyntaxKind.OverrideKeyword)) return true;

        return member switch
        {
            MethodDeclarationSyntax m   => m.ExplicitInterfaceSpecifier is not null,
            PropertyDeclarationSyntax p => p.ExplicitInterfaceSpecifier is not null,
            EventDeclarationSyntax e    => e.ExplicitInterfaceSpecifier is not null,
            IndexerDeclarationSyntax i  => i.ExplicitInterfaceSpecifier is not null,
            _                           => false,
        };
    }

    // ------------------------------------------------------------------ metodos

    private static void BuildMethod(MethodDeclarationSyntax m, string indent, List<string> lines, Options opt)
    {
        var name = m.Identifier.ValueText;
        var isAsync = m.Modifiers.Any(SyntaxKind.AsyncKeyword) || IsAwaitable(m.ReturnType);
        var isExtension = m.ParameterList.Parameters.FirstOrDefault()?.Modifiers.Any(SyntaxKind.ThisKeyword) == true;
        var returnsMlResult = ReturnsMlResult(m.ReturnType);

        Summary(lines, indent, SummaryBuilder.ForMethod(name, isAsync, isExtension, returnsMlResult));

        if (Visibility.IsObsolete(m))
            Remarks(lines, indent, "Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo <c>Obsolete</c>.");

        TypeParams(lines, indent, m.TypeParameterList);
        Params(lines, indent, m.ParameterList, isExtension);
        Returns(lines, indent, m.ReturnType, isAsync);
    }

    private static void BuildConstructor(ConstructorDeclarationSyntax c, string indent, List<string> lines)
    {
        Summary(lines, indent, SummaryBuilder.ForConstructor(TypeRef(c)));
        Params(lines, indent, c.ParameterList, false);
    }

    private static void BuildOperator(OperatorDeclarationSyntax o, string indent, List<string> lines)
    {
        var token = o.OperatorToken.ValueText;
        Summary(lines, indent, "Implementa el operador <c>" + Escape(token) + "</c>.");
        Params(lines, indent, o.ParameterList, false);
        Returns(lines, indent, o.ReturnType, false);
    }

    private static void BuildConversion(ConversionOperatorDeclarationSyntax co, string indent, List<string> lines)
    {
        var kind = co.ImplicitOrExplicitKeyword.IsKind(SyntaxKind.ImplicitKeyword) ? "implicita" : "explicita";
        Summary(lines, indent, "Realiza la conversion " + kind + " a <c>" + Escape(co.Type.ToString()) + "</c>.");
        Params(lines, indent, co.ParameterList, false);
        Returns(lines, indent, co.Type, false);
    }

    private static void BuildDelegate(DelegateDeclarationSyntax d, string indent, List<string> lines)
    {
        Summary(lines, indent, "Representa el delegado " + Lower(Vocabulary.Humanize(d.Identifier.ValueText)) + ".");
        TypeParams(lines, indent, d.TypeParameterList);
        Params(lines, indent, d.ParameterList, false);
        Returns(lines, indent, d.ReturnType, false);
    }

    // ------------------------------------------------------------------ miembros

    private static void BuildProperty(PropertyDeclarationSyntax p, string indent, List<string> lines)
    {
        var isBool = p.Type.ToString() is "bool" or "bool?" or "Boolean" or "Boolean?";
        var hasSetter = p.AccessorList?.Accessors.Any(a =>
                              a.IsKind(SyntaxKind.SetAccessorDeclaration)
                           || a.IsKind(SyntaxKind.InitAccessorDeclaration)) == true;

        var text = SummaryBuilder.ForProperty(p.Identifier.ValueText, isBool);
        if (hasSetter && text.StartsWith("Obtiene ", StringComparison.Ordinal))
            text = "Obtiene o establece " + text["Obtiene ".Length..];

        Summary(lines, indent, text);
    }

    private static void BuildIndexer(IndexerDeclarationSyntax ix, string indent, List<string> lines)
    {
        Summary(lines, indent, "Obtiene el elemento situado en la posicion indicada.");
        Params(lines, indent, ix.ParameterList, false);
        lines.Add(indent + "/// <returns>Elemento correspondiente a la posicion indicada.</returns>");
    }

    private static void BuildField(FieldDeclarationSyntax f, string indent, List<string> lines)
    {
        var name = f.Declaration.Variables.FirstOrDefault()?.Identifier.ValueText;
        if (string.IsNullOrEmpty(name)) return;

        var isConst = f.Modifiers.Any(SyntaxKind.ConstKeyword);
        var text = isConst
            ? "Constante con " + Lower(Vocabulary.Humanize(name)) + "."
            : SummaryBuilder.ForField(name);

        Summary(lines, indent, text);
    }

    private static void BuildEventField(EventFieldDeclarationSyntax evf, string indent, List<string> lines)
    {
        var name = evf.Declaration.Variables.FirstOrDefault()?.Identifier.ValueText;
        if (string.IsNullOrEmpty(name)) return;
        Summary(lines, indent, "Se produce cuando " + Lower(Vocabulary.Humanize(name)) + ".");
    }

    private static void BuildType(TypeDeclarationSyntax t, string indent, List<string> lines)
    {
        Summary(lines, indent, SummaryBuilder.ForType(t));

        if (Visibility.IsObsolete(t))
            Remarks(lines, indent, "Tipo obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo <c>Obsolete</c>.");

        TypeParams(lines, indent, t.TypeParameterList);

        // record posicional: documentar sus parametros
        if (t is RecordDeclarationSyntax { ParameterList: not null } rec)
            Params(lines, indent, rec.ParameterList, false);
    }

    // ------------------------------------------------------------------ etiquetas

    private static void Summary(List<string> lines, string indent, string text)
    {
        lines.Add(indent + "/// <summary>");
        foreach (var l in Wrap(text, 100))
            lines.Add(indent + "/// " + l);
        lines.Add(indent + "/// </summary>");
    }

    private static void Remarks(List<string> lines, string indent, string text)
    {
        lines.Add(indent + "/// <remarks>");
        foreach (var l in Wrap(text, 100))
            lines.Add(indent + "/// " + l);
        lines.Add(indent + "/// </remarks>");
    }

    private static void TypeParams(List<string> lines, string indent, TypeParameterListSyntax? list)
    {
        if (list is null) return;
        foreach (var tp in list.Parameters)
        {
            var n = tp.Identifier.ValueText;
            lines.Add(indent + "/// <typeparam name=\"" + n + "\">" + Vocabulary.TypeParamText(n) + "</typeparam>");
        }
    }

    private static void Params(List<string> lines, string indent, BaseParameterListSyntax list, bool isExtension)
    {
        var first = true;
        foreach (var p in list.Parameters)
        {
            var n = p.Identifier.ValueText;
            if (string.IsNullOrEmpty(n)) { first = false; continue; }

            var text = isExtension && first
                ? ExtensionSourceText(p)
                : ParamText(p);

            lines.Add(indent + "/// <param name=\"" + n + "\">" + text + "</param>");
            first = false;
        }
    }

    private static string ExtensionSourceText(ParameterSyntax p)
    {
        var type = p.Type?.ToString() ?? string.Empty;
        if (type.StartsWith("MlResult", StringComparison.Ordinal))
            return "Resultado de origen sobre el que se aplica la operacion.";
        if (type.StartsWith("Task<MlResult", StringComparison.Ordinal) || type.StartsWith("ValueTask<MlResult", StringComparison.Ordinal))
            return "Tarea que produce el resultado de origen.";
        return Vocabulary.ParamText(p.Identifier.ValueText);
    }

    private static string ParamText(ParameterSyntax p)
    {
        var name = p.Identifier.ValueText;
        var known = Vocabulary.ParamText(name);

        // si el vocabulario ha dado una descripcion especifica, se respeta
        if (!known.Equals(Vocabulary.Humanize(name) + ".", StringComparison.Ordinal)) return known;

        var type = p.Type?.ToString() ?? string.Empty;

        if (type.StartsWith("Func<", StringComparison.Ordinal))   return "Funcion a aplicar.";
        if (type.StartsWith("Action", StringComparison.Ordinal))  return "Accion a ejecutar.";
        if (type.StartsWith("MlResult", StringComparison.Ordinal))return "Resultado sobre el que se opera.";
        if (type is "CancellationToken")                          return "Token para cancelar la operacion.";
        if (type is "bool" or "bool?")                            return "Valor que activa o desactiva " + SummaryBuilder.Code(name) + ".";
        if (IsCollection(type))                                   return "Coleccion de " + SummaryBuilder.Code(name) + ".";

        return "Valor de " + SummaryBuilder.Code(name) + ".";
    }

    private static void Returns(List<string> lines, string indent, TypeSyntax returnType, bool isAsync)
    {
        var t = returnType.ToString();
        if (t is "void") return;

        if (t is "Task" or "ValueTask")
        {
            lines.Add(indent + "/// <returns>Tarea que representa la operacion asincrona.</returns>");
            return;
        }

        var inner = Unwrap(t);
        var wasAwaitable = inner.Length != t.Length;

        string text;
        if (inner.StartsWith("MlResult", StringComparison.Ordinal))
            text = "resultado valido con el valor obtenido, o resultado fallido con los detalles del error";
        else if (inner is "bool" or "bool?")
            text = "<c>true</c> si la condicion se cumple; en caso contrario, <c>false</c>";
        else if (inner is "string" or "string?")
            text = "representacion textual obtenida";
        else if (IsCollection(inner))
            text = "coleccion con los elementos obtenidos";
        else
            text = "valor de tipo " + Code(inner) + " obtenido por la operacion";

        text = wasAwaitable
            ? "Tarea que produce el " + text + "."
            : char.ToUpperInvariant(text[0]) + text[1..] + ".";

        lines.Add(indent + "/// <returns>" + text + "</returns>");
    }

    private static bool IsCollection(string type)
        => type.EndsWith("[]", StringComparison.Ordinal)
        || type.StartsWith("IEnumerable<", StringComparison.Ordinal)
        || type.StartsWith("IQueryable<", StringComparison.Ordinal)
        || type.StartsWith("IReadOnlyCollection<", StringComparison.Ordinal)
        || type.StartsWith("IReadOnlyList<", StringComparison.Ordinal)
        || type.StartsWith("ICollection<", StringComparison.Ordinal)
        || type.StartsWith("IList<", StringComparison.Ordinal)
        || type.StartsWith("List<", StringComparison.Ordinal)
        || type.StartsWith("IAsyncEnumerable<", StringComparison.Ordinal);

    private static string Code(string type)
        => "<c>" + Escape(type) + "</c>";

    // ------------------------------------------------------------------ utiles

    private static string Unwrap(string type)
    {
        foreach (var w in new[] { "Task<", "ValueTask<" })
            if (type.StartsWith(w, StringComparison.Ordinal) && type.EndsWith('>'))
                return type[w.Length..^1];
        return type;
    }

    private static bool IsAwaitable(TypeSyntax t)
    {
        var s = t.ToString();
        return s is "Task" or "ValueTask"
            || s.StartsWith("Task<", StringComparison.Ordinal)
            || s.StartsWith("ValueTask<", StringComparison.Ordinal)
            || s.StartsWith("IAsyncEnumerable<", StringComparison.Ordinal);
    }

    private static bool ReturnsMlResult(TypeSyntax t)
        => Unwrap(t.ToString()).StartsWith("MlResult", StringComparison.Ordinal);

    private static string TypeRef(SyntaxNode node)
    {
        var owner = node.Ancestors().OfType<TypeDeclarationSyntax>().FirstOrDefault();
        if (owner is null) return "T";

        var name = owner.Identifier.ValueText;
        var tp = owner.TypeParameterList;
        if (tp is null || tp.Parameters.Count == 0) return name;

        return name + "{" + string.Join(", ", tp.Parameters.Select(p => p.Identifier.ValueText)) + "}";
    }

    private static string Lower(string text)
        => text.Length == 0 ? text : char.ToLowerInvariant(text[0]) + text[1..];

    private static string Escape(string text)
        => text.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");

    /// <summary>Parte el texto en lineas de longitud maxima, cortando por espacios.</summary>
    private static List<string> Wrap(string text, int width)
    {
        var result = new List<string>();
        var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var current = new System.Text.StringBuilder();

        foreach (var w in words)
        {
            if (current.Length > 0 && current.Length + 1 + w.Length > width)
            {
                result.Add(current.ToString());
                current.Clear();
            }
            if (current.Length > 0) current.Append(' ');
            current.Append(w);
        }
        if (current.Length > 0) result.Add(current.ToString());
        if (result.Count == 0) result.Add(text);
        return result;
    }
}
