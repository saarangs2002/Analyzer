//using System;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Dashboard.Authentication;
//namespace AuthenticationTest
//{
//    [TestClass]
//    public class UnitTest1
//    {
//        [TestMethod]
//        private Authenticator viewModel;
//        public AuthenticationViewModelUnitTest()
//        {
//            viewModel = new Authenticator();
//        }

//        /// <summary>
//        /// This is used to test any kind of timeout scenario
//        /// 1. If the browser window is closed and never reopened
//        /// 2. Internet is not working
//        /// 3. Not entering credentials for too long
//        /// </summary>
//        [Fact]
//        public async void WebPageTimeout()
//        {
//            // Giving a timeout of 50 ms to reduce the wait time
//            var returnval = await viewModel.AuthenticateUser(50);
//            // Assert
//            Assert.Equal("false", returnval[0]);
//        }

//        // Commenting this out since this is 
//        // semi automatic

//        /// <summary>
//        /// In case all information have been provided succesfully
//        /// </summary>
//        //[Fact]
//        //public async void ValidData()
//        //{
//        //    var returnVal = await viewModel.Authenticate();
//        //    // Assert
//        //    Assert.Equal("true", returnVal[0]);
//        //    Assert.NotEmpty(returnVal[1]);
//        //    Assert.NotEmpty(returnVal[2]);
//        //    Assert.NotEmpty(returnVal[3]);
//        //}
//    }
//}
