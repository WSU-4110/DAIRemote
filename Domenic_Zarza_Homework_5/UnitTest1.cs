using UDPServerManagerForm;

namespace Domenic_Zarza_Homework_5
{
    public class UnitTest1
    {
        [Fact]
        public void ExtractName()
        {
            var obj = new UDPServerHost();
            string input = "Connection requested by xyz";
            string expected = "xyz";
            string actual = obj.ExtractDeviceName(input);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ClientCheck()
        {
            var obj1 = new UDPServerHost();
            string ip = "123";
            bool expected = false;
            bool actual = obj1.IsClient(ip);
            Assert.Equal(expected, actual);
        }
    }
}