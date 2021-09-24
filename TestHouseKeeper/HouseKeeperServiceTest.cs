using HouseKeeper.Application;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TestHouseKeeper
{
    public class HouseKeeperServiceTest
    {
        private HouseKeeperService _service;
        private Housekeeper _housekepper;
        private Mock<IStatementGenerator> _statementGenerator;
        private Mock<IEmailSender> _emailSender;
        private Mock<IXtraMessageBox> _messageBox;
        private DateTime _statementDate = new DateTime(2021, 8, 8);
        private string _statementFileName;

        [SetUp]
        public void Setup()
        {
            _housekepper = new Housekeeper { Email = "S", FullName = "Hai Son", Oid = 1, StatementEmailBody = "ko biet" };
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(a => a.Query<Housekeeper>()).Returns(new List<Housekeeper>
            {
                _housekepper
            }.AsQueryable());

            _statementFileName = "filename";
            _statementGenerator = new Mock<IStatementGenerator>();
            _statementGenerator.Setup(x => x.SaveStatement(_housekepper.Oid, _housekepper.FullName
                                              , (_statementDate))).Returns(() => _statementFileName);

            _emailSender = new Mock<IEmailSender>();
            _messageBox = new Mock<IXtraMessageBox>();

            _service = new HouseKeeperService(unitOfWork.Object, _statementGenerator.Object,
                                                _emailSender.Object, _messageBox.Object);
        }

        [Test]
        public void SendStatementEmails_WhenCalled_EmailTheStatement()
        {
            _statementGenerator.Setup(x => x.SaveStatement(_housekepper.Oid, _housekepper.FullName
                                                , (_statementDate))).Returns(_statementFileName);
            _service.SendStatementEmails(_statementDate);
            _emailSender.Verify(x => x.EmailFile(_housekepper.Email,
                                                 _housekepper.StatementEmailBody,
                                                 _statementFileName,
                                                 It.IsAny<string>()));
        }

        [Test]
        public void SendStatementEmails_StatementFileNameIsNull_ShouldNotEmailStatement()
        {
            _statementFileName = null;
            _service.SendStatementEmails(_statementDate);
            _emailSender.Verify(x => x.EmailFile(It.IsAny<string>(),
                                                 It.IsAny<string>(),
                                                 It.IsAny<string>(),
                                                 It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void SendStatementEmails_StatementFileNameIsEmptyString_ShouldNotEmailStatement()
        {
            _statementFileName = "";
           _service.SendStatementEmails(_statementDate);
            _emailSender.Verify(x => x.EmailFile(It.IsAny<string>(),
                                                 It.IsAny<string>(),
                                                 It.IsAny<string>(),
                                                 It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void SendStatementEmails_StatementFileNameIsWhiteSpace_ShouldNotEmailStatement()
        {
            _statementFileName = " ";
            _service.SendStatementEmails(_statementDate);
            _emailSender.Verify(x => x.EmailFile(It.IsAny<string>(),
                                                 It.IsAny<string>(),
                                                 It.IsAny<string>(),
                                                 It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void SendStatementEmails_EmailSendingFail_DisplayMessageBox()
        {
            _emailSender.Setup(e => e.EmailFile(It.IsAny<string>(),
                                                It.IsAny<string>(),
                                                It.IsAny<string>(),
                                                It.IsAny<string>())).Throws<Exception>();
            _service.SendStatementEmails(_statementDate);
            _messageBox.Verify(m => m.Show(It.IsAny<string>(), It.IsAny<string>(), MessageBoxButtons.OK));
        }
    }
}