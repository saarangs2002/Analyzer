using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dashboard;
using Authenticator;

namespace AuthenticationTest
{
    [TestClass]
    public class UnitTest1
    {
        private Authenticator viewModel;
        [TestMethod]
        public AuthenticationViewModelUnitTest()
        {
            viewModel = new Authenticator();
        }
        //[Fact]
        public async void WebPageTimeout()
        {
            // Giving a timeout of 50 ms to reduce the wait time
            var returnval = await viewModel.AuthenticateUser( 50 );
            // Assert
            Assert.Equals("false" , returnval[0] );
        }

    }
}
