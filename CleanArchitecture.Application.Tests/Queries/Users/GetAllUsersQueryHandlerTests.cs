using System.Linq;
using System.Threading.Tasks;
using CleanArchitecture.Application.Queries.Users.GetAll;
using CleanArchitecture.Application.Tests.Fixtures.Queries.Users;
using CleanArchitecture.Application.ViewModels;
using FluentAssertions;
using Xunit;

namespace CleanArchitecture.Application.Tests.Queries.Users;

public sealed class GetAllUsersQueryHandlerTests
{
    private readonly GetAllUsersTestFixture _fixture = new();

    [Fact]
    public async Task Should_Get_All_Users()
    {
        var user = _fixture.SetupUserAsync();

        var query = new PageQuery
        {
            PageSize = 1,
            Page = 1
        };

        var result = await _fixture.Handler.Handle(
            new GetAllUsersQuery(query, user.Email),
            default);

        _fixture.VerifyNoDomainNotification();

        result.PageSize.Should().Be(query.PageSize);
        result.Page.Should().Be(query.Page);
        result.Count.Should().Be(1);

        var userViewModels = result.Items.ToArray();
        userViewModels.Should().NotBeNull();
        userViewModels.Should().ContainSingle();
        userViewModels.FirstOrDefault()!.Id.Should().Be(_fixture.ExistingUserId);
    }

    [Fact]
    public async Task Should_Not_Get_Deleted_Users()
    {
        _fixture.SetupDeletedUserAsync();

        var query = new PageQuery
        {
            PageSize = 10,
            Page = 1
        };

        var result = await _fixture.Handler.Handle(
            new GetAllUsersQuery(query),
            default);

        _fixture.VerifyNoDomainNotification();

        result.PageSize.Should().Be(query.PageSize);
        result.Page.Should().Be(query.Page);
        result.Count.Should().Be(0);

        result.Items.Should().BeEmpty();
    }
}