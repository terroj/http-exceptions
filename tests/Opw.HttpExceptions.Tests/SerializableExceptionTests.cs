using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Opw.HttpExceptions
{
    public class SerializableExceptionTests
    {
        [Fact]
        public void Constructor_Should_CreateSerializableException()
        {
            var exception = new SerializableException(new ApplicationException("ApplicationException", new ArgumentNullException("param", "ArgumentNullException")));

            exception.Type.Should().Be("ApplicationException");
            exception.Message.Should().Be("ApplicationException");
            exception.InnerException.Type.Should().StartWith("ArgumentNullException");
            exception.InnerException.Data["ParamName"].Should().Be("param");
        }

        [Fact]
        public void Serialization_Should_SerializeAndDeserialize()
        {
            var exception = new SerializableException(new ApplicationException("ApplicationException", new ArgumentNullException("param", "ArgumentNullException")));
            exception = SerializationHelper.SerializeDeserialize(exception);

            exception.Type.Should().Be("ApplicationException");
            exception.Message.Should().Be("ApplicationException");
            exception.InnerException.Type.Should().StartWith("ArgumentNullException");
            exception.InnerException.Data["ParamName"].Should().Be("param");
        }
        
        [Fact]
        public void Serialization_Should_SerializeWithMultiplyPropertiesWithSameKey()
        {
            var somePropertyValue = "SomePropertyValue";
            var overriddenException = new OverriddenException("OverriddenException")
            {
                SomeProperty = somePropertyValue
            };
            ((BaseException)overriddenException).SomeProperty = "another string";

            var exception = new SerializableException(overriddenException);
            exception = SerializationHelper.SerializeDeserialize(exception);

            exception.Type.Should().Be("OverriddenException");
            exception.Message.Should().Be("OverriddenException");
            exception.Data["SomeProperty"].Should().Be(somePropertyValue);
        }

        private class BaseException : Exception
        {
            public string SomeProperty { get; set; }

            public BaseException(string message) : base(message)
            {
            }
        }

        private class OverriddenException : BaseException
        {
            public new object SomeProperty { get; set; }

            public OverriddenException(string message) : base(message)
            {
            }
        }
    }
}
