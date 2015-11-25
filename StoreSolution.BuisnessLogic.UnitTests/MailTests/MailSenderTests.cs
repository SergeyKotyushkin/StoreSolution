using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StoreSolution.BusinessLogic.Lang.Contracts;
using StoreSolution.BusinessLogic.Mail;
using StoreSolution.BusinessLogic.Models;

namespace StoreSolution.BuisnessLogic.UnitTests.MailTests
{
    [TestClass]
    public class MailSenderTests
    {
        [TestMethod]
        public void CheckIsMessageCreated_NotCreatingMessage_ReturnFalse()
        {
            var mailSender = new MailSender(GetMailService());

            const bool expected = false;

            var actual = mailSender.CheckIsMessageCreated();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CheckIsMessageCreated_AfterCreatingMessage_ReturnTrue()
        {
            var mailSender = new MailSender(GetMailService());

            const bool expected = true;

            mailSender.Create("from", "to", "subject", new List<OrderItem>(), false, new CultureInfo("ru-RU"));
            var actual = mailSender.CheckIsMessageCreated();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Create_CheckIsMessageCreatedBeforeAndAfterCreate_ReturnEqualCheckIsMessageCreated()
        {
            var mailSender = new MailSender(GetMailService());

            const bool expectedBeforeCreate = false;
            const bool expectedAfterCreate = true;

            var actualBeforeCreate = mailSender.CheckIsMessageCreated();

            mailSender.Create("from", "to", "subject", new List<OrderItem>(), false, new CultureInfo("ru-RU"));

            var actualAfterCreate = mailSender.CheckIsMessageCreated();

            Assert.AreEqual(expectedBeforeCreate, actualBeforeCreate);
            Assert.AreEqual(expectedAfterCreate, actualAfterCreate);
        }

        [TestMethod]
        public void Send_SendMailBeforeCreateMessage_ReturnNullReferenceException()
        {
            var mailSender = new MailSender(GetMailService());

            try
            {
                mailSender.Send();
                Assert.Fail("Must be threw NullReferenceException.");
            }
            catch (NullReferenceException)
            {
            }
        }

        [TestMethod]
        public void Send_SendMailAfterCreateMessage_ReturnSuccessfullySendBeforeSendFalse()
        {
            var mailSender = new MailSender(GetMailService());

            mailSender.Create("from@mail.ru", "to@mail.ru", "subject", new List<OrderItem>(), false,
                new CultureInfo("ru-RU"));

            const bool expectedBeforeSend = false;

            try
            {
                var actualBeforeSend = mailSender.SuccessfulySend;
                mailSender.Send();

                Assert.AreEqual(expectedBeforeSend, actualBeforeSend);
            }
            catch (NullReferenceException e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        public void Send_SendMailAfterCreateMessage_ReturnSuccessfullySendAfterSendTrue()
        {
            var mailSender = new MailSender(GetMailService());

            mailSender.Create("from@mail.ru", "to@mail.ru", "subject", new List<OrderItem>(), false,
                new CultureInfo("ru-RU"));

            const bool expectedAfterSend = true;

            try
            {
                mailSender.Send();
                var actualAfterSend = mailSender.SuccessfulySend;

                Assert.AreEqual(expectedAfterSend, actualAfterSend);
            }
            catch (NullReferenceException e)
            {
                Assert.Fail(e.Message);
            }
        }


        [TestMethod]
        public void Send_SendMailAfterCreateMessage_ReturnNoExceptions()
        {
            var mailSender = new MailSender(GetMailService());

            mailSender.Create("from@mail.ru", "to@mail.ru", "subject", new List<OrderItem>(), false,
                new CultureInfo("ru-RU"));
            
            try
            {
                mailSender.Send();
            }
            catch (NullReferenceException ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        private static MailService GetMailService()
        {
            var langSetterMock = new Mock<ILangSetter>();
            const string mailString = "{0}-{1}-{2}<br/>";

            langSetterMock.Setup(m => m.Set(It.IsAny<string>())).Returns(mailString);

            return new MailService(langSetterMock.Object);
        }
    }
}