﻿using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.OpenApi.Models;
using Yardarm.Helpers;
using Yardarm.Spec;

namespace Yardarm.Enrichment.Schema.Internal
{
    internal class RequiredPropertyEnricher : IOpenApiSyntaxNodeEnricher<PropertyDeclarationSyntax, OpenApiSchema>
    {
        public int Priority => 0;

        public PropertyDeclarationSyntax Enrich(PropertyDeclarationSyntax syntax, OpenApiEnrichmentContext<OpenApiSchema> context)
        {
            bool isRequired =
                context.LocatedElement.Parents.FirstOrDefault() is LocatedOpenApiElement<OpenApiSchema> parentSchema &&
                parentSchema.Element.Required.Contains(context.LocatedElement.Key);

            return isRequired
                ? AddRequiredAttribute(syntax, context.Compilation)
                : syntax.MakeNullable();
        }

        private PropertyDeclarationSyntax AddRequiredAttribute(PropertyDeclarationSyntax syntax, CSharpCompilation compilation)
        {
            var semanticModel = compilation.GetSemanticModel(syntax.SyntaxTree);

            var typeInfo = semanticModel.GetTypeInfo(syntax.Type);

            syntax = syntax.AddAttributeLists(SyntaxFactory.AttributeList().AddAttributes(
                SyntaxFactory.Attribute(WellKnownTypes.RequiredAttribute())));

            if (typeInfo.Type?.IsReferenceType ?? false)
            {
                // Always mark reference types as nullable on schemas, even if they're required
                // This will encourage SDK consumers to check for nulls and prevent NREs

                syntax = syntax.MakeNullable();
            }

            return syntax;
        }
    }
}
