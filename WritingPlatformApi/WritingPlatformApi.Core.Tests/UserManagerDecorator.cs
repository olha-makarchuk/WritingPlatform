using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace WritingPlatformApi.Core.Tests
{
    public class UserManagerDecorator
    {
        public Mock<UserManager<ApplicationUser>> UserManagerMock { get; private set; }
        public Dictionary<string, string> MyConfiguration { get; private set; }

        public UserManagerDecorator()
        {
            MyConfiguration = new()
            {
                {"JWTKey:Secret", "super_secret_key_1234567890qwerty"},
                {"JWTKey:ValidIssuer", "testIssuer"},
                {"JWTKey:ValidAudience", "testAudience"},
                {"JWTKey:TokenExpiryTimeInHour", "1"},
                {"JWTKey:RefreshTokenExpiryTimeInDay", "7"}
            };

            var store = new Mock<IUserStore<ApplicationUser>>();
            UserManagerMock = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
        }

        public void SetupFindByNameAsync(ApplicationUser user)
        {
            UserManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(user);
        }

        public void SetupFindByNameAsyncTwice(ApplicationUser userFirst, ApplicationUser userSecond)
        {
            UserManagerMock.SetupSequence(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(userFirst)
                .ReturnsAsync(userSecond);
        }

        public void SetupCreateAsync(IdentityResult result)
        {
            UserManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(result);
        }

        public void SetupCheckPasswordAsync(bool result)
        {
            UserManagerMock.Setup(x => x.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(result);
        }

        public void SetupGetRolesAsync(IList<string> roles)
        {
            UserManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(roles);
        }

        public void SetupUpdateAsync(IdentityResult result)
        {
            UserManagerMock.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(result);
        }
        public void SetupFindByIdAsync(ApplicationUser user)
        {
            UserManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
        }
    }
}
