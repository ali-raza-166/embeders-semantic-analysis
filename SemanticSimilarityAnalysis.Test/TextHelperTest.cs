using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class TextHelperTests
{
    private TextHelper _textHelper;

    [TestInitialize]
    public void Initialize()
    {
        _textHelper = new TextHelper();
    }
}