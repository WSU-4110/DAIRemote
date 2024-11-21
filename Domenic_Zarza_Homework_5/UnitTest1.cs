using UDPServerManagerForm;

namespace Domenic_Zarza_Homework_5
{
    public class UnitTest1
    {
        [Fact]
        public void ExtractName()
        {
            var obj = new UDPServerHost();
            string input = "Connection established by xyz";
            string expected = "xyz";
            string actual = obj.ExtractDeviceName(input);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ClientCheck()
        {

        }
    }
}