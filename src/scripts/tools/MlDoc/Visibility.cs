using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MlDoc;

/// <summary>
/// Determina si un miembro forma parte de la superficie publica del ensamblado,
/// que es exactamente el criterio con el que el compilador emite CS1591.
/// </summary>
internal static class Visibility
{
    public static bool IsExternallyVisible(MemberDeclarationSyntax member)
    {
        if (!IsDocumentable(member)) return false;
        if (!IsPublicOrProtected(member, out _)) return false;

        // todos los contenedores deben ser visibles tambien
        foreach (var anc in member.Ancestors().OfType<MemberDeclarationSyntax>())
        {
            if (anc is BaseNamespaceDeclarationSyntax) continue;
            if (!IsPublicOrProtected(anc, out _)) return false;
        }

        return true;
    }

    private static bool IsDocumentable(MemberDeclarationSyntax member) => member switch
    {
        ClassDeclarationSyntax        => true,
        StructDeclarationSyntax       => true,
        InterfaceDeclarationSyntax    => true,
        RecordDeclarationSyntax       => true,
        EnumDeclarationSyntax         => true,
        DelegateDeclarationSyntax     => true,
        MethodDeclarationSyntax       => true,
        ConstructorDeclarationSyntax  => true,
        PropertyDeclarationSyntax     => true,
        IndexerDeclarationSyntax      => true,
        EventDeclarationSyntax        => true,
        EventFieldDeclarationSyntax   => true,
        FieldDeclarationSyntax        => true,
        OperatorDeclarationSyntax     => true,
        ConversionOperatorDeclarationSyntax => true,
        EnumMemberDeclarationSyntax   => true,
        _                             => false,
    };

    /// <summary>
    /// Indica si el miembro es publico o protegido, teniendo en cuenta la visibilidad
    /// por defecto de cada clase de contenedor.
    /// </summary>
    private static bool IsPublicOrProtected(MemberDeclarationSyntax member, out bool isProtected)
    {
        isProtected = false;
        var mods = member.Modifiers;

        if (mods.Any(SyntaxKind.PrivateKeyword) && !mods.Any(SyntaxKind.ProtectedKeyword)) return false;
        if (mods.Any(SyntaxKind.InternalKeyword) && !mods.Any(SyntaxKind.ProtectedKeyword)) return false;

        if (mods.Any(SyntaxKind.PublicKeyword)) return true;

        if (mods.Any(SyntaxKind.ProtectedKeyword))
        {
            // protected internal -> visible; private protected -> no
            if (mods.Any(SyntaxKind.PrivateKeyword)) return false;
            isProtected = true;
            return true;
        }

        // sin modificador explicito: los miembros de interfaz son publicos por defecto
        var parent = member.Parent;
        if (parent is InterfaceDeclarationSyntax) return true;

        // los valores de una enumeracion tienen la visibilidad de la propia enumeracion
        if (member is EnumMemberDeclarationSyntax && parent is EnumDeclarationSyntax) return true;

        // los tipos de nivel superior sin modificador son internal
        return false;
    }

    /// <summary>Indica si la declaracion lleva el atributo <c>[Obsolete]</c>.</summary>
    public static bool IsObsolete(MemberDeclarationSyntax member)
        => member.AttributeLists
                 .SelectMany(a => a.Attributes)
                 .Any(a =>
                 {
                     var n = a.Name.ToString();
                     return n is "Obsolete" or "ObsoleteAttribute"
                         || n.EndsWith(".Obsolete", StringComparison.Ordinal)
                         || n.EndsWith(".ObsoleteAttribute", StringComparison.Ordinal);
                 });
}
