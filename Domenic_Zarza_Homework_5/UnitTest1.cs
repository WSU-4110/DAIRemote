using UDPServerManagerForm;

namespace Domenic_Zarza_Homework_5
{
    public class UnitTest1
    {
        [Fact]
        public void ExtractName()
        {
            using var obj = new UDPServerHost();
            string input = "Connection requested by xyz";
            string expected = "xyz";
            string actual = obj.ExtractDeviceName(input);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ClientCheck()
        {
            using var obj = new UDPServerHost();
            string ip = "123";
            bool expected = false;
            bool actual = obj.IsClient(ip);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SetHeartbeat()
        {
            using var obj = new UDPServerHost();
            var expected = DateTime.Now;
            obj.SetLastHeartbeat(expected);
            Assert.Equal(expected, obj.GetLastHeartbeat());
        }

        [Fact]
        public void GetHeartbeat()
        {
            using var obj = new UDPServerHost();
            var actual = obj.GetLastHeartbeat();
            Assert.Equal(default, actual);
        }

        [Fact]
        public void SetTimeout()
        {
            using var obj = new UDPServerHost();
            var expected = TimeSpan.FromSeconds(10);
            obj.SetHeartbeatTimeout(expected);
            Assert.Equal(expected, obj.GetHeartbeatTimeout());
        }

        [Fact]
        public void GetTimeout()
        {
            using var obj = new UDPServerHost();
            var actual = obj.GetHeartbeatTimeout();
            Assert.Equal(default, actual);
        }
    }
}