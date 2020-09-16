using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium;
using System.Reflection;

namespace UITests
{
   [TestClass]
   public class UnitTest1
   {
      protected const string WindowsApplicationDriverUrl = "http://127.0.0.1:1000";

      protected static WindowsDriver<WindowsElement> session;
      
      [TestInitialize]
      public void Setup()
      {
         // Launch a new instance of wpf app
         if (session == null)
         {
            string assemblyFolder = Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location );
            string WPFSameDirAppLocation = Path.Combine( assemblyFolder, @".\MyWPFTestApp.exe" );
            // Create a new session to launch WPF application
            DesiredCapabilities appCapabilities = new DesiredCapabilities();
            appCapabilities.SetCapability( "app", WPFSameDirAppLocation );
            session = new WindowsDriver<WindowsElement>( new Uri( WindowsApplicationDriverUrl ), appCapabilities );
            Assert.IsNotNull( session );
            Assert.IsNotNull( session.SessionId );

            // Verify that WPF app is started
            Assert.AreEqual( "MainWindow", session.Title );

            // Set implicit timeout to 1.5 seconds to make element search to retry every 500 ms for at most three times
            session.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds( 1.5 );
         }
      }

      [TestCleanup]
      public void Cleanup()
      {
         // Close the application and delete the session
         if (session != null)
         {
            session.Close();
            session.Quit();
            session = null;
         }
      }


      [TestMethod]
      public void CopyText_CopiesFromTextBoxToLabel()
      {
         WindowsElement textbox = session.FindElementByAccessibilityId( "textBox" );
         WindowsElement copyTextButton = session.FindElementByAccessibilityId( "copyTextButton" );
         WindowsElement label = session.FindElementByAccessibilityId( "label" );

         Assert.IsNotNull( textbox );
         Assert.IsNotNull( copyTextButton );
         Assert.IsNotNull( label );

         string testData = "Test123Test123";

         // Select all text and delete to clear the edit box
         textbox.SendKeys( Keys.Control + "a" );
         textbox.SendKeys( Keys.Delete );
         textbox.SendKeys( testData );
         copyTextButton.Click();

         Assert.IsTrue( label.Text.Equals( testData ) );
      }
   }
}
