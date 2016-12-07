namespace Integra.Space.LanguageUnitTests.TestObject
{
    public class TestObject2 : InputBase
    {
        public TestObject2(string messageType = "0100", string primaryAccountNumber = "9999941616073663", string processingCode = "302000", decimal transactionAmount = 2m, string dateTimeTransmission = "0508152549", string systemTraceAuditNumber = "212868", string localTransactionTime = "152549"
            , string localTransactionDate = "0508", string settlementDate = "0508", string merchantType = "6011", string acquiringInstitutionCountryCode = "320", string pointOfServiceEntryMode = "051", string pointOfServiceConditionCode = "02", string acquiringInstitutionIdentificationCode = "491381", string track2Data = "9999941616073663D18022011583036900000"
            , string retrievalReferenceNumber = "412815212868", string cardAcceptorTerminalIdentification = "2906", string cardAcceptorIdentificationCode = "Shell El Rodeo ", string cardAcceptorNameLocation = "Shell El Rodeo2HONDURAS     HN", string transactionCurrencyCode = "320", string accountIdentification1 = "00001613000000000001"
            , int campo104 = -1, uint campo105 = 1)
        {
            this.MessageType = messageType;
            this.PrimaryAccountNumber = primaryAccountNumber;
            this.ProcessingCode = processingCode;
            this.TransactionAmount = transactionAmount;
            this.DateTimeTransmission = dateTimeTransmission;
            this.SystemTraceAuditNumber = systemTraceAuditNumber;
            this.LocalTransactionTime = localTransactionTime;
            this.LocalTransactionDate = localTransactionDate;
            this.SettlementDate = settlementDate;
            this.MerchantType = merchantType;
            this.AcquiringInstitutionCountryCode = acquiringInstitutionCountryCode;
            this.PointOfServiceConditionCode = pointOfServiceEntryMode;
            this.PointOfServiceConditionCode = pointOfServiceConditionCode;
            this.AcquiringInstitutionIdentificationCode = acquiringInstitutionIdentificationCode;
            this.Track2Data = track2Data;
            this.RetrievalReferenceNumber = retrievalReferenceNumber;
            this.CardAcceptorTerminalIdentification = cardAcceptorTerminalIdentification;
            this.CardAcceptorIdentificationCode = cardAcceptorIdentificationCode;
            this.CardAcceptorNameLocation = cardAcceptorNameLocation;
            this.TransactionCurrencyCode = transactionCurrencyCode;
            this.AccountIdentification1 = accountIdentification1;            
            this.Campo104 = Campo104;
            this.Campo105 = Campo105;
        }

        public string MessageType { get; private set; }
        public string PrimaryAccountNumber { get; private set; }
        public string ProcessingCode { get; private set; }
        public decimal TransactionAmount { get; private set; }
        public string DateTimeTransmission { get; private set; }
        public string SystemTraceAuditNumber { get; private set; }
        public string LocalTransactionTime { get; private set; }
        public string LocalTransactionDate { get; private set; }
        public string SettlementDate { get; private set; }
        public string MerchantType { get; private set; }
        public string AcquiringInstitutionCountryCode { get; private set; }
        public string PointOfServiceEntryMode { get; private set; }
        public string PointOfServiceConditionCode { get; private set; }
        public string AcquiringInstitutionIdentificationCode { get; private set; }
        public string Track2Data { get; private set; }
        public string RetrievalReferenceNumber { get; private set; }
        public string CardAcceptorTerminalIdentification { get; private set; }
        public string CardAcceptorIdentificationCode { get; private set; }
        public string CardAcceptorNameLocation { get; private set; }
        public string TransactionCurrencyCode { get; private set; }
        public string AccountIdentification1 { get; private set; }
        public int Campo104 { get; private set; }
        public uint Campo105 { get; private set; }
    }
}
