﻿using System;
using System.Collections.Generic;
using System.Net.Mail;
using StoreSolution.BusinessLogic.Mail.Contracts;
using StoreSolution.BusinessLogic.Models;

namespace StoreSolution.BusinessLogic.Mail
{
    public class MailSender : IMailSender
    {
        private readonly IMailService _mailService;

        private string _from;
        private string _to;
        private string _subject;
        private string _body;
        private bool _isBodyHtml;

        public MailSender(IMailService mailService)
        {
            _mailService = mailService;
            SuccessfulySend = false;
        }

        public void Send()
        {
            SuccessfulySend = false;

            if (!CheckIsMessageCreated()) throw new NullReferenceException();

            using (var mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress(_from);
                mailMessage.To.Add(new MailAddress(_to));
                mailMessage.CC.Add(new MailAddress(_to));
                mailMessage.Subject = _subject;
                mailMessage.Body = _body;
                mailMessage.IsBodyHtml = _isBodyHtml;

                using (var client = new SmtpClient())
                {
                    client.Send(mailMessage);
                }
            }

            SuccessfulySend = true;
        }

        public void Create(string @from, string to, string subject, IEnumerable<OrderItem> orderItemsBody,
            bool isBodyHtml, IFormatProvider culture)
        {
            SuccessfulySend = false;

            _from = @from;
            _to = to;
            _subject = subject;
            _body = _mailService.GetBody(orderItemsBody, culture);
            _isBodyHtml = isBodyHtml;
        }

        public bool CheckIsMessageCreated()
        {
            SuccessfulySend = false;

            return !string.IsNullOrEmpty(_from) && 
                   !string.IsNullOrEmpty(_to) && 
                   !string.IsNullOrEmpty(_subject) &&
                   !string.IsNullOrEmpty(_body);
        }

        public bool SuccessfulySend { get; private set; }
    }
}