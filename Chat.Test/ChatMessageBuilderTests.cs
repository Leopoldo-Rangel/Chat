using Chat.Shared.Helpers;

namespace Chat.Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void IsBot_Is_False()
        {
            var response = ChatMessageBuilder.FormatMessage("Hi", new DateTime(2022, 11, 3, 12, 25, 40),
                false, "Test");
            Console.WriteLine(response);
            Assert.IsTrue(response.Equals("(11/03/2022 12:25:40) Test says: Hi"));
        }

        [Test]
        public void IsBot_Is_True()
        {
            var response = ChatMessageBuilder.FormatMessage("Hi", new DateTime(2022, 11, 3, 12, 25, 40),
                true);
            Console.WriteLine(response);
            Assert.IsTrue(response.Equals("(11/03/2022 12:25:40) Quote Bot says: Hi"));
        }

        [Test]
        public void UserName_Should_Have_Value_If_IsBot_Is_False()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                ChatMessageBuilder.FormatMessage("Test", DateTime.Now, false);
            });
        }

        [Test]
        public void UserName_Should_Not_Have_Value_If_IsBot_Is_True()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                ChatMessageBuilder.FormatMessage("Hi", DateTime.Now, true, "Test");
            });
        }


    }
}