using Microsoft.AspNetCore.Identity;
using Moq;

namespace WritingPlatformApi.Core.Tests
{
    public class RoleManagerDecorator
    {
        public Mock<RoleManager<IdentityRole>> RoleManagerMock { get; private set; }

        public RoleManagerDecorator()
        {
            var roleStore = new Mock<IRoleStore<IdentityRole>>();
            RoleManagerMock = new Mock<RoleManager<IdentityRole>>(
                roleStore.Object, null, null, null, null);
        }

        public void SetupRoleExistsAsync(bool exists)
        {
            RoleManagerMock.Setup(x => x.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(exists);
        }

        public void SetupCreateAsync(IdentityResult result)
        {
            RoleManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(result);
        }
    }
}
