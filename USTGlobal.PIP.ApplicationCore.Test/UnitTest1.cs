using FluentAssertions;
using NSubstitute;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Helpers;
using USTGlobal.PIP.ApplicationCore.Interfaces;
using USTGlobal.PIP.ApplicationCore.Services;
using Xunit;

namespace USTGlobal.PIP.ApplicationCore.Test
{
    public class UnitTest1
    {
        //[Fact]
        //public void Test1()
        //{
        //    var userDTO = new UserRoleDTO();
        //    userDTO.Email = "a@b.c";

        //    var repo = Substitute.For<IUserRepository>();
        //    repo.GetUserData("a@b.c").Returns(userDTO);

        //    var service = new UserService(repo);
            
        //    service.GetUserData("a@b.c").Should().Be(userDTO);            
        //}

        //[Fact]
        //public void ToXml_Should_Convert_Object_ToXml()
        //{
        //    var userDTO = new UserRoleDTO();
        //    userDTO.Email = "a@b.c";

        //    var xml = userDTO.ToXml();
            
        //    xml.Should().NotBeEmpty();
        //}

    }
}
