using Server.Shared;
using Xunit;

namespace Server.UnitTest.Shared;

public class TestBusinessException
{
    private static void ExEmpy() { throw new BusinessException(); }
    private static void ExMsg() { throw new BusinessException("fubar"); }
    private static void ExInner() { throw new BusinessException("fubar", new DivideByZeroException("fubar2")); }

    [Fact]
    public void BusinessExecption_Without_Message()
    {
        // Arrange

        // Act
        var exception = Record.Exception(ExEmpy);

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<BusinessException>(exception);
        Assert.Equal("Exception of type 'Server.Shared.BusinessException' was thrown.", exception.Message);
    }

    [Fact]
    public void BusinessExecption_With_Message()
    {
        // Arrange

        // Act
        var exception = Record.Exception(ExMsg);

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<BusinessException>(exception);
        Assert.Equal("fubar", exception.Message);
    }

    [Fact]
    public void BusinessExecption_With_InnerException()
    {
        // Arrange

        // Act
        var exception = Record.Exception(ExInner);

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<BusinessException>(exception);
        Assert.Equal("fubar", exception.Message);
        Assert.NotNull(exception.InnerException);
        Assert.IsType<DivideByZeroException>(exception.InnerException);
        Assert.Equal("fubar2", exception.InnerException.Message);
    }
}
