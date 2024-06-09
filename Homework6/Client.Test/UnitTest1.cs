using System.Data.SqlTypes;
using System.Text.Json.Nodes;

namespace Client.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void MessageDeserializeTest()
        {
            string json = "{\"Text\":\"Test message\",\"Sender\":\"Sender\",\"IsRead\":false}";

            Message msg = Message.Deserialize(json);
            Assert.IsNotNull(msg);
            Assert.AreEqual("Test message",msg.Text);
            Assert.AreEqual("Sender", msg.Sender);
            Assert.IsFalse(msg.IsRead);
        }

        [TestMethod]
        public void MessageSerializeTest()
        {
            Message msg = new MessageBuilder().SetDateTime(DateTime.Now)
                .SetText("Test text")
                .SetSender("Sender")
                .SetReceiver("Server")
                .SetIsRead(false)
                .Build();

            Assert.IsNotNull(msg);

            string json = msg.Serialize();

            Assert.IsNotNull(json);

            Assert.IsTrue(json.Contains("Test text"));
            Assert.IsTrue(json.Contains("Sender"));
            Assert.IsFalse(json.Contains("true"));
        }        
    }
}