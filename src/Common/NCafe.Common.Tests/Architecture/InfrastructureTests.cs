using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnit;
using ArchUnitNET.Fluent;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace NCafe.Common.Tests.Architecture;
public class InfrastructureTests
{
#pragma warning disable IDE1006 // Naming Styles
    private readonly static System.Reflection.Assembly InfrastructureAssembly =
        System.Reflection.Assembly.Load("NCafe.Infrastructure");

    private readonly static ArchUnitNET.Domain.Architecture Architecture =
        new ArchLoader().LoadAssemblies(
            InfrastructureAssembly
        ).Build();

    private readonly IObjectProvider<IType> CommonInfrastructure =
        Types().That().ResideInAssembly(InfrastructureAssembly)
        .As("Infrastructure Layer");

    private readonly IObjectProvider<IType> DependencyRegistrationClass =
        Types().That().HaveName("DependencyRegistration")
        .As("DependencyRegistration class");

    [Fact]
    public void Infrastructure_Project_Types_Should_Be_Internal()
    {
        IArchRule typesShouldBeInternal = Types().That().Are(CommonInfrastructure)
            .And().AreNot(DependencyRegistrationClass).Should().BeInternal()
            .Because("Other layers should not depend on implementation details, only interfaces.");

        typesShouldBeInternal.Check(Architecture);
    }
#pragma warning restore IDE1006 // Naming Styles
}
