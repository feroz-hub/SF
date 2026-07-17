using FluentAssertions;
using Xunit;
using Zentra.Domain.Models.Endpoint;
using Zentra.DomainServices;
using Zentra.Infrastructure.Data;
using Zentra.Service.Implementation.Endpoint;

namespace Zentra.ArchitectureTests;

public class LayerDependencyTests
{
    [Fact]
    public void DomainAssembly_MustNotDependOnApplicationOrInfrastructure()
    {
        var references = typeof(ClientsModel).Assembly
            .GetReferencedAssemblies()
            .Select(reference => reference.Name)
            .ToArray();

        references.Should().NotContain(name => name != null && name.StartsWith("Zentra.Service"));
        references.Should().NotContain(name => name != null && name.StartsWith("Zentra.Infrastructure"));
    }

    [Fact]
    public void DomainServicesAssembly_MustNotDependOnApplicationOrInfrastructure()
    {
        var references = typeof(IRepository<>).Assembly
            .GetReferencedAssemblies()
            .Select(reference => reference.Name)
            .ToArray();

        references.Should().NotContain(name => name != null && name.StartsWith("Zentra.Service"));
        references.Should().NotContain(name => name != null && name.StartsWith("Zentra.Infrastructure"));
    }

    [Fact]
    public void ApplicationAssembly_MustNotDependOnPersistence()
    {
        var references = typeof(TokenEndpoint).Assembly
            .GetReferencedAssemblies()
            .Select(reference => reference.Name)
            .ToArray();

        references.Should().NotContain(name => name != null && name == "Zentra.Infrastructure.Data");
    }

    [Fact]
    public void PersistenceAssembly_MustNotDependOnApplicationAssembly()
    {
        var references = typeof(ApplicationDbContext).Assembly
            .GetReferencedAssemblies()
            .Select(reference => reference.Name)
            .ToArray();

        references.Should().NotContain(name => name != null && name == "Zentra.Service");
    }

    [Fact]
    public void ApiLayer_MustNotReferenceInfrastructureOutsideCompositionRoot()
    {
        var repositoryRoot = GetRepositoryRoot();
        var apiRoot = Path.Combine(repositoryRoot, "src", "Identity", "Zentra.Identity.API");
        var allowedCompositionFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            Path.Combine(apiRoot, "Program.cs"),
            Path.Combine(apiRoot, "Extensions", "ZentraExtension.cs")
        };

        var violatingFiles = Directory
            .GetFiles(apiRoot, "*.cs", SearchOption.AllDirectories)
            .Where(file => !file.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}",
                               StringComparison.OrdinalIgnoreCase))
            .Where(file => !file.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}",
                               StringComparison.OrdinalIgnoreCase))
            .Where(file => !allowedCompositionFiles.Contains(file))
            .Where(file => File.ReadAllText(file).Contains("Zentra.Infrastructure.", StringComparison.Ordinal))
            .Select(file => Path.GetRelativePath(repositoryRoot, file))
            .ToArray();

        violatingFiles.Should().BeEmpty("only composition root files may reference infrastructure assemblies");
    }

    [Fact]
    public void DomainLayer_MustNotReferenceInfrastructureNamespaces()
    {
        var repositoryRoot = GetRepositoryRoot();
        var domainRoot = Path.Combine(repositoryRoot, "src", "Identity", "Zentra.Identity.Domain");

        var violatingFiles = Directory
            .GetFiles(domainRoot, "*.cs", SearchOption.AllDirectories)
            .Where(file => !file.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}",
                               StringComparison.OrdinalIgnoreCase))
            .Where(file => !file.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}",
                               StringComparison.OrdinalIgnoreCase))
            .Where(file => File.ReadAllText(file).Contains("Zentra.Infrastructure.", StringComparison.Ordinal))
            .Select(file => Path.GetRelativePath(repositoryRoot, file))
            .ToArray();

        violatingFiles.Should().BeEmpty("domain layer must remain independent from infrastructure namespaces");
    }

    private static string GetRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory != null && !File.Exists(Path.Combine(directory.FullName, "Zentra.sln")))
            directory = directory.Parent;

        if (directory == null)
            throw new InvalidOperationException("Unable to locate repository root from test output directory.");

        return directory.FullName;
    }
}
