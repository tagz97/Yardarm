﻿using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.OpenApi.Models;
using Yardarm.Generation;

namespace Yardarm.Enrichment.Requests
{
    public interface IOperationParameterPropertyEnricher : IEnricher<PropertyDeclarationSyntax, LocatedOpenApiElement<OpenApiParameter>>
    {
    }
}
