/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using FluentAssertions;
using Xunit;
using HCL.CS.Domain.Models.Endpoint;
using HCL.CS.DomainServices;
using HCL.CS.Infrastructure.Data;
using HCL.CS.Service.Implementation.Endpoint;

namespace HCL.CS.ArchitectureTests;

public class LayerDependencyTests
{
    [Fact]
    public void DomainAssembly_MustNotDependOnApplicationOrInfrastructure()
    {
        var references = typeof(ClientsModel).Assembly
            .GetReferencedAssemblies()
            .Select(reference => reference.Name)
            .ToArray();

        references.Should().NotContain(name => name != null && name.StartsWith("HCL.CS.Service"));
        references.Should().NotContain(name => name != null && name.StartsWith("HCL.CS.Infrastructure"));
    }

    [Fact]
    public void DomainServicesAssembly_MustNotDependOnApplicationOrInfrastructure()
    {
        var references = typeof(IRepository<>).Assembly
            .GetReferencedAssemblies()
            .Select(reference => reference.Name)
            .ToArray();

        references.Should().NotContain(name => name != null && name.StartsWith("HCL.CS.Service"));
        references.Should().NotContain(name => name != null && name.StartsWith("HCL.CS.Infrastructure"));
    }

    [Fact]
    public void ApplicationAssembly_MustNotDependOnPersistence()
    {
        var references = typeof(TokenEndpoint).Assembly
            .GetReferencedAssemblies()
            .Select(reference => reference.Name)
            .ToArray();

        references.Should().NotContain(name => name != null && name == "HCL.CS.Infrastructure.Data");
    }

    [Fact]
    public void PersistenceAssembly_MustNotDependOnApplicationAssembly()
    {
        var references = typeof(ApplicationDbContext).Assembly
            .GetReferencedAssemblies()
            .Select(reference => reference.Name)
            .ToArray();

        references.Should().NotContain(name => name != null && name == "HCL.CS.Service");
    }

    [Fact]
    public void ApiLayer_MustNotReferenceInfrastructureOutsideCompositionRoot()
    {
        var repositoryRoot = GetRepositoryRoot();
        var apiRoot = Path.Combine(repositoryRoot, "src", "Identity", "HCL.CS.Identity.API");
        var allowedCompositionFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            Path.Combine(apiRoot, "Program.cs"),
            Path.Combine(apiRoot, "Extensions", "HclCsExtension.cs")
        };

        var violatingFiles = Directory
            .GetFiles(apiRoot, "*.cs", SearchOption.AllDirectories)
            .Where(file => !file.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}",
                               StringComparison.OrdinalIgnoreCase))
            .Where(file => !file.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}",
                               StringComparison.OrdinalIgnoreCase))
            .Where(file => !allowedCompositionFiles.Contains(file))
            .Where(file => File.ReadAllText(file).Contains("HCL.CS.Infrastructure.", StringComparison.Ordinal))
            .Select(file => Path.GetRelativePath(repositoryRoot, file))
            .ToArray();

        violatingFiles.Should().BeEmpty("only composition root files may reference infrastructure assemblies");
    }

    [Fact]
    public void DomainLayer_MustNotReferenceInfrastructureNamespaces()
    {
        var repositoryRoot = GetRepositoryRoot();
        var domainRoot = Path.Combine(repositoryRoot, "src", "Identity", "HCL.CS.Identity.Domain");

        var violatingFiles = Directory
            .GetFiles(domainRoot, "*.cs", SearchOption.AllDirectories)
            .Where(file => !file.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}",
                               StringComparison.OrdinalIgnoreCase))
            .Where(file => !file.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}",
                               StringComparison.OrdinalIgnoreCase))
            .Where(file => File.ReadAllText(file).Contains("HCL.CS.Infrastructure.", StringComparison.Ordinal))
            .Select(file => Path.GetRelativePath(repositoryRoot, file))
            .ToArray();

        violatingFiles.Should().BeEmpty("domain layer must remain independent from infrastructure namespaces");
    }

    private static string GetRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory != null && !File.Exists(Path.Combine(directory.FullName, "HCL.CS.sln")))
            directory = directory.Parent;

        if (directory == null)
            throw new InvalidOperationException("Unable to locate repository root from test output directory.");

        return directory.FullName;
    }
}
