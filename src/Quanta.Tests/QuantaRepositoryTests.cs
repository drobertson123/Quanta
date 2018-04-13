using System;
using Xunit;

namespace Quanta.Tests
{
    public class QuantaRepositoryTests 
    {

        [Fact]
        public void TestMethod1()
        {
            // Arrange


            // Act
            QuantaRepository quantaRepository = this.CreateQuantaRepository();


            // Assert

        }

        private QuantaRepository CreateQuantaRepository()
        {
            return new QuantaRepository();
        }
    }
}
