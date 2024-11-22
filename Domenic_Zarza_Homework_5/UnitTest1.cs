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
            var obj = new UDPServerHost();
            string ip = "123";
            bool expected = false;
            bool actual = obj.IsClient(ip);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SetHeartbeat()
        {
            var obj = new UDPServerHost();
            var expected = DateTime.Now;
            obj.SetLastHeartbeat(expected);
            Assert.Equal(expected, obj.GetLastHeartbeat());
        }

        [Fact]
        public void GetHeartbeat()
        {
            var obj = new UDPServerHost();
            var actual = obj.GetLastHeartbeat();
            Assert.Equal(default, actual);
        }
    }
}