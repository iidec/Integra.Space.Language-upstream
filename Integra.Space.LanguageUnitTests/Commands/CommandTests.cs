using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Integra.Space.Language;
using Integra.Space.Common;

namespace Integra.Space.LanguageUnitTests.Commands
{
    [TestClass]
    public class CommandTests
    {
        public void Process(string command)
        {
            CommandParser cp = new CommandParser(command);
            var spaceCommands = cp.Evaluate();

            Console.WriteLine();
        }

        #region insert

        [TestMethod]
        public void InsertEvent()
        {
            string command = @"insert into SourceParaPruebas
                                (
                                MessageType,
                                PrimaryAccountNumber,
                                ProcessingCode,
                                TransactionAmount,
                                DateTimeTransmission,
                                SystemTraceAuditNumber,
                                LocalTransactionTime,
                                LocalTransactionDate,
                                SettlementDate,
                                MerchantType,
                                AcquiringInstitutionCountryCode,
                                PointOfServiceEntryMode,
                                PointOfServiceConditionCode,
                                AcquiringInstitutionIdentificationCode,
                                Track2Data,
                                RetrievalReferenceNumber,
                                CardAcceptorTerminalIdentification,
                                CardAcceptorIdentificationCode,
                                CardAcceptorNameLocation,
                                TransactionCurrencyCode,
                                AccountIdentification1,
                                Campo104,
                                Campo105
                                )
                                values
                                (
                                ""0100"",
                                ""9999941616073663"",
                                ""302000"",
                                1m,
                                ""0508152549"",
                                ""212868"",
                                ""152549"",
                                ""0508"",
                                ""0508"",
                                ""6011"",
                                ""320"",
                                ""051"",
                                ""02"",
                                ""491381"",
                                ""9999941616073663D18022011583036900000"",
                                ""412815212868"",
                                ""2906    "",
                                ""Shell El Rodeo "",
                                ""Shell El Rodeo1GUATEMALA    GT"",
                                ""320"",
                                ""00001613000000000001"",
                                -1,
                                1
                                )";
            this.Process(command);
        }

        #endregion insert

        #region take ownership

        [TestMethod]
        public void TakeOwnershipOnEndpoint()
        {
            string command = "take ownership on endpoint Endpoint1";
            this.Process(command);
        }

        [TestMethod]
        public void TakeOwnershipOnDatabase()
        {
            string command = "use Database1; take ownership on database Database1";
            this.Process(command);
        }

        [TestMethod]
        public void TakeOwnershipOnDbRole()
        {
            string command = "take ownership on role Role1";
            this.Process(command);
        }

        [TestMethod]
        public void TakeOwnershipOnSchema()
        {
            string command = "take ownership on schema Schema1";
            this.Process(command);
        }

        [TestMethod]
        public void TakeOwnershipOnSource()
        {
            string command = "take ownership on source Source1";
            this.Process(command);
        }

        [TestMethod]
        public void TakeOwnershipOnStream()
        {
            string command = "take ownership on stream Stream1";
            this.Process(command);
        }

        [TestMethod]
        public void TakeOwnershipOnView()
        {
            string command = "take ownership on view View1";
            this.Process(command);
        }

        #endregion take ownership

        #region use

        [TestMethod]
        public void UseCommandTest()
        {
            string command = "use Database1";
            this.Process(command);
        }

        #endregion use

        #region drop

        [TestMethod]
        public void DropEndpoint()
        {
            string command = "drop endpoint Enpoint1";
            this.Process(command);
        }

        [TestMethod]
        public void DropEndpoints()
        {
            string command = "drop endpoint Enpoint1, endpoint2, endpoint3";
            this.Process(command);
        }

        [TestMethod]
        public void DropLogin()
        {
            string command = "drop login Login1";
            this.Process(command);
        }

        [TestMethod]
        public void DropLogins()
        {
            string command = "drop login Login1, login2, login3";
            this.Process(command);
        }

        [TestMethod]
        public void DropDatabase()
        {
            string command = "drop database database1";
            this.Process(command);
        }

        [TestMethod]
        public void DropDatabases()
        {
            string command = "drop database database1, database2, database3";
            this.Process(command);
        }

        [TestMethod]
        public void DropUser()
        {
            string command = "drop user user1";
            this.Process(command);
        }

        [TestMethod]
        public void DropUsers()
        {
            string command = "drop user user1, user2, user3";
            this.Process(command);
        }

        [TestMethod]
        public void DropDatabaseRole()
        {
            string command = "drop role role1";
            this.Process(command);
        }

        [TestMethod]
        public void DropDatabaseRoles()
        {
            string command = "drop role role1, role2, role3";
            this.Process(command);
        }

        [TestMethod]
        public void DropSchema()
        {
            string command = "drop schema schema1";
            this.Process(command);
        }

        [TestMethod]
        public void DropSchemas()
        {
            string command = "drop schema schema1, schema2, schema3";
            this.Process(command);
        }

        [TestMethod]
        public void DropSource()
        {
            string command = "drop source Source1";
            this.Process(command);
        }

        [TestMethod]
        public void DropSources()
        {
            string command = "drop source Source1, source2, source3";
            this.Process(command);
        }

        [TestMethod]
        public void DropStream()
        {
            string command = "drop stream stream1";
            this.Process(command);
        }

        [TestMethod]
        public void DropStreams()
        {
            string command = "drop stream stream1, stream2, stream3";
            this.Process(command);
        }

        [TestMethod]
        public void DropView()
        {
            string command = "drop view view1";
            this.Process(command);
        }

        [TestMethod]
        public void DropViews()
        {
            string command = "drop view view1, view2, view3";
            this.Process(command);
        }

        #endregion drop

        #region create

        [TestMethod]
        public void CreateEndpoint()
        {
            string command = "create endpoint endpoint1";
            this.Process(command);
        }

        [TestMethod]
        public void CreateEndpointWithStatusOn()
        {
            string command = "create endpoint endpoint1 with status = on";
            this.Process(command);
        }

        [TestMethod]
        public void CreateEndpointWithStatusOff()
        {
            string command = "create endpoint endpoint1 with status = off";
            this.Process(command);
        }

        [TestMethod]
        public void CreateLogin()
        {
            string command = "create login endpoint1 with password = \"pass123\"";
            this.Process(command);
        }

        [TestMethod]
        public void CreateLoginWithStatusOn()
        {
            string command = "create login login1 with password = \"pass123\", status = on";
            this.Process(command);
        }

        [TestMethod]
        public void CreateLoginWithStatusOff()
        {
            string command = "create login login2 with password = \"pass123\", status = off";
            this.Process(command);
        }

        [TestMethod]
        public void CreateLoginWithDefaultDatabase()
        {
            string command = "create login login2 with password = \"pass123\", default_database = db1";
            this.Process(command);
        }

        [TestMethod]
        public void CreateLoginWithDefaultDatabaseAndStatusOn()
        {
            string command = "create login login2 with password = \"pass123\", default_database = db1, status = on";
            this.Process(command);
        }

        [TestMethod]
        public void CreateLoginWithDefaultDatabaseAndStatusOff()
        {
            string command = "create login login2 with password = \"pass123\", default_database = db1, status = off";
            this.Process(command);
        }

        [TestMethod]
        public void CreateDatabase()
        {
            string command = "create database databaese1";
            this.Process(command);
        }

        [TestMethod]
        public void CreateDatabaseWithStatusOn()
        {
            string command = "create database databaese1 with status = on";
            this.Process(command);
        }

        [TestMethod]
        public void CreateDatabaseWithStatusOff()
        {
            string command = "create database databaese1 with status = off";
            this.Process(command);
        }

        [TestMethod]
        public void CreateUser()
        {
            string command = "create user user1";
            this.Process(command);
        }

        [TestMethod]
        public void CreateUserWithDefaultSchema()
        {
            string command = "create user user1 with default_schema = schema1";
            this.Process(command);
        }

        [TestMethod]
        public void CreateUserWithLogin()
        {
            string command = "create user user1 with login = login1";
            this.Process(command);
        }

        [TestMethod]
        public void CreateUserWithStatusOn()
        {
            string command = "create user user1 with status = on";
            this.Process(command);
        }

        [TestMethod]
        public void CreateUserWithStatusOff()
        {
            string command = "create user user1 with status = off";
            this.Process(command);
        }

        [TestMethod]
        public void CreateUserWithDefaultSchemaLoginStatusOff()
        {
            string command = "create user user1 with default_schema = schema1, login = login1, status = off";
            this.Process(command);
        }

        [TestMethod]
        public void CreateUserWithDefaultSchemaLoginStatusOon()
        {
            string command = "create user user1 with default_schema = schema1, login = login1, status = on";
            this.Process(command);
        }

        [TestMethod]
        public void CreateRole()
        {
            string command = "create role role1";
            this.Process(command);
        }

        [TestMethod]
        public void CreateRoleWithStatusOn()
        {
            string command = "create role role1 with status = on";
            this.Process(command);
        }

        [TestMethod]
        public void CreateRoleWithStatusOff()
        {
            string command = "create role role1 with status = off";
            this.Process(command);
        }

        [TestMethod]
        public void CreateRoleAddUser()
        {
            string command = $"Create role role1 with add = User1";
            this.Process(command);
        }

        [TestMethod]
        public void CreateRoleAddUsers()
        {
            string command = $"Create role role1 with add = User1 User2 user3";
            this.Process(command);
        }
        
        [TestMethod]
        public void CreateSchema()
        {
            string command = "create schema schema123";
            this.Process(command);
        }

        [TestMethod]
        public void CreateStream()
        {
            string eql = "cross " +
                                   "JOIN SourceParaPruebas as t1 WHERE t1.PrimaryAccountNumber == \"9999941616073663_1\" " +
                                   "WITH SourceParaPruebas as t2 WHERE t2.PrimaryAccountNumber == \"9999941616073663_2\" " +
                                   "ON t1.AcquiringInstitutionIdentificationCode == t2.AcquiringInstitutionIdentificationCode " +
                                   "TIMEOUT '00:00:02' " +
                                   //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                   "SELECT (string)t1.PrimaryAccountNumber as c1, t2.PrimaryAccountNumber as c2, 1 as numeroXXX into SourceXYZ ";

            string command = $"create stream stream123 {{ {eql} }}";
            this.Process(command);
        }

        [TestMethod]
        public void CreateStreamWithStatusOn()
        {
            string eql = "cross " +
                                   "JOIN SourceParaPruebas as t1 WHERE PrimaryAccountNumber == \"9999941616073663_1\" " +
                                   "WITH SourceParaPruebas as t2 WHERE PrimaryAccountNumber == \"9999941616073663_2\" " +
                                   "ON AcquiringInstitutionIdentificationCode == AcquiringInstitutionIdentificationCode " +
                                   "TIMEOUT '00:00:02' " +
                                   //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                   "SELECT (string)PrimaryAccountNumber as c1, PrimaryAccountNumber as c2, 1 as numeroXXX into SourceXYZ ";

            string command = $"create stream stream123 {{ {eql} }} with status = on";
            this.Process(command);
        }

        [TestMethod]
        public void CreateStreamWithStatusOff()
        {
            string eql = "cross " +
                                   "JOIN SourceParaPruebas as t1 WHERE PrimaryAccountNumber == \"9999941616073663_1\" " +
                                   "WITH SourceParaPruebas as t2 WHERE PrimaryAccountNumber == \"9999941616073663_2\" " +
                                   "ON AcquiringInstitutionIdentificationCode == AcquiringInstitutionIdentificationCode " +
                                   "TIMEOUT '00:00:02' " +
                                   //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                   "SELECT (string)PrimaryAccountNumber as c1, PrimaryAccountNumber as c2, 1 as numeroXXX into SourceXYZ ";

            string command = $"create stream stream123 {{ {eql} }} with status = off";
            this.Process(command);
        }

        [TestMethod]
        public void CreateSource()
        {
            string command = "create source source123 (column1 string, column2 int, column3 decimal)";
            this.Process(command);
        }

        [TestMethod]
        public void CreateSourceWithStatusOn()
        {
            string command = "create source source123 (column1 string, column2 int, column3 decimal) with status = on";
            this.Process(command);
        }

        [TestMethod]
        public void CreateSourceWithStatusOff()
        {
            string command = "create source source123 (column1 string, column2 int, column3 decimal) with status = off";
            this.Process(command);
        }

        [TestMethod]
        public void CreateSourceWithCacheDurability()
        {
            string command = "create source source123 (column1 string, column2 int, column3 decimal) with cache_durability = 1";
            this.Process(command);
        }

        [TestMethod]
        public void CreateSourceWithCacheSize()
        {
            string command = "create source source123 (column1 string, column2 int, column3 decimal) with cache_size = 1";
            this.Process(command);
        }

        [TestMethod]
        public void CreateSourceWithCacheSizeCacheDurability()
        {
            string command = "create source source123 (column1 string, column2 int, column3 decimal) with cache_size = 1, cache_durability = 1";
            this.Process(command);
        }

        [TestMethod]
        public void CreateSourceWithPersistentOn()
        {
            string command = "create source source123 (column1 string, column2 int, column3 decimal) with persistent = on";
            this.Process(command);
        }

        [TestMethod]
        public void CreateSourceWithPersistentOff()
        {
            string command = "create source source123 (column1 string, column2 int, column3 decimal) with persistent = off";
            this.Process(command);
        }

        #endregion create

        #region alter


        #region alter

        [TestMethod]
        public void AlterEndpoint()
        {
            string command = "alter endpoint endpoint1";
            this.Process(command);
        }

        [TestMethod]
        public void AlterEndpointWithStatusOn()
        {
            string command = "alter endpoint endpoint1 with status = on";
            this.Process(command);
        }

        [TestMethod]
        public void AlterEndpointWithStatusOff()
        {
            string command = "alter endpoint endpoint1 with status = off";
            this.Process(command);
        }

        [TestMethod]
        public void AlterLogin()
        {
            string command = "alter login endpoint1 with password = \"pass123\"";
            this.Process(command);
        }

        [TestMethod]
        public void AlterLoginWithStatusOn()
        {
            string command = "alter login login1 with password = \"pass123\", status = on";
            this.Process(command);
        }

        [TestMethod]
        public void AlterLoginWithStatusOff()
        {
            string command = "alter login login2 with password = \"pass123\", status = off";
            this.Process(command);
        }

        [TestMethod]
        public void AlterLoginWithDefaultDatabase()
        {
            string command = "alter login login2 with password = \"pass123\", default_database = db1";
            this.Process(command);
        }

        [TestMethod]
        public void AlterLoginWithDefaultDatabaseAndStatusOn()
        {
            string command = "alter login login2 with password = \"pass123\", default_database = db1, status = on";
            this.Process(command);
        }

        [TestMethod]
        public void AlterLoginWithDefaultDatabaseAndStatusOff()
        {
            string command = "alter login login2 with password = \"pass123\", default_database = db1, status = off";
            this.Process(command);
        }

        [TestMethod]
        public void AlterDatabaseName()
        {
            string command = "use Databasexxxxxx; alter database databaese1 with name = database2";
            this.Process(command);
        }

        [TestMethod]
        public void AlterDatabaseWithStatusOn()
        {
            string command = "alter database databaese1 with status = on";
            this.Process(command);
        }

        [TestMethod]
        public void AlterDatabaseWithStatusOff()
        {
            string command = "alter database databaese1 with status = off";
            this.Process(command);
        }

        [TestMethod]
        public void AlterUserName()
        {
            string command = "alter user user1 with name = user3";
            this.Process(command);
        }

        [TestMethod]
        public void AlterUserWithDefaultSchema()
        {
            string command = "alter user user1 with default_schema = schema1";
            this.Process(command);
        }

        [TestMethod]
        public void AlterUserWithLogin()
        {
            string command = "alter user user1 with login = login1";
            this.Process(command);
        }

        [TestMethod]
        public void AlterUserWithStatusOn()
        {
            string command = "alter user user1 with status = on";
            this.Process(command);
        }

        [TestMethod]
        public void AlterUserWithStatusOff()
        {
            string command = "alter user user1 with status = off";
            this.Process(command);
        }

        [TestMethod]
        public void AlterUserWithDefaultSchemaLoginStatusOff()
        {
            string command = "alter user user1 with default_schema = schema1, login = login1, status = off";
            this.Process(command);
        }

        [TestMethod]
        public void AlterUserWithDefaultSchemaLoginStatusOon()
        {
            string command = "alter user user1 with default_schema = schema1, login = login1, status = on";
            this.Process(command);
        }

        [TestMethod]
        public void AlterRoleName()
        {
            string command = "alter role role1 with name = role2";
            this.Process(command);
        }

        [TestMethod]
        public void AlterRoleWithStatusOn()
        {
            string command = "alter role role1 with status = on";
            this.Process(command);
        }

        [TestMethod]
        public void AlterRoleWithStatusOff()
        {
            string command = "alter role role1 with status = off";
            this.Process(command);
        }

        [TestMethod]
        public void AlterRoleAddUser()
        {
            string command = $"Alter role role1 with add = User1";
            this.Process(command);
        }

        [TestMethod]
        public void AlterRoleAddUsers()
        {
            string command = $"Alter role role1 with add = User1 User2 user3";
            this.Process(command);
        }
        
        [TestMethod]
        public void AlterSchemaName()
        {
            string command = "alter schema schema123 with name = schema456";
            this.Process(command);
        }

        [TestMethod]
        public void AlterStreamWithStatusOn()
        {
            string command = $"alter stream stream123 with status = on";
            this.Process(command);
        }

        [TestMethod]
        public void AlterStreamWithStatusOff()
        {
            string command = $"alter stream stream123 with status = off";
            this.Process(command);
        }

        [TestMethod]
        public void AlterStreamWithQueryStatusOn()
        {
            string eql = "cross " +
                                   "JOIN SourceParaPruebas as t1 WHERE PrimaryAccountNumber == \"9999941616073663_1\" " +
                                   "WITH SourceParaPruebas as t2 WHERE PrimaryAccountNumber == \"9999941616073663_2\" " +
                                   "ON AcquiringInstitutionIdentificationCode == AcquiringInstitutionIdentificationCode " +
                                   "TIMEOUT '00:00:02' " +
                                   //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                   "SELECT (string)PrimaryAccountNumber as c1, PrimaryAccountNumber as c2, 1 as numeroXXX into SourceXYZ ";

            string command = $"alter stream stream123 with query = {{ {eql} }}, status = on";
            this.Process(command);
        }

        [TestMethod]
        public void AlterStreamWithQueryStatusOff()
        {
            string eql = "cross " +
                                   "JOIN SourceParaPruebas as t1 WHERE PrimaryAccountNumber == \"9999941616073663_1\" " +
                                   "WITH SourceParaPruebas as t2 WHERE PrimaryAccountNumber == \"9999941616073663_2\" " +
                                   "ON AcquiringInstitutionIdentificationCode == AcquiringInstitutionIdentificationCode " +
                                   "TIMEOUT '00:00:02' " +
                                   //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                   "SELECT (string)PrimaryAccountNumber as c1, PrimaryAccountNumber as c2, 1 as numeroXXX into SourceXYZ ";

            string command = $"alter stream stream123 with query = {{ {eql} }}, status = off";
            this.Process(command);
        }

        [TestMethod]
        public void AlterSourceName()
        {
            string command = "alter source source1 with name = source2";
            this.Process(command);
        }

        [TestMethod]
        public void AlterSourceWithStatusOn()
        {
            string command = "alter source source123 with status = on";
            this.Process(command);
        }

        [TestMethod]
        public void AlterSourceWithStatusOff()
        {
            string command = "alter source source123 with status = off";
            this.Process(command);
        }

        [TestMethod]
        public void AlterSourceWithCacheDurability()
        {
            string command = "alter source source123 with cache_durability = 1";
            this.Process(command);
        }

        [TestMethod]
        public void AlterSourceWithCacheSize()
        {
            string command = "alter source source123 with cache_size = 1";
            this.Process(command);
        }

        [TestMethod]
        public void AlterSourceWithCacheSizeCacheDurability()
        {
            string command = "alter source source123 with cache_size = 1, cache_durability = 1";
            this.Process(command);
        }

        [TestMethod]
        public void AlterSourceAddColumns()
        {
            string command = "alter source source1 add column1 string, column2 int, column3 decimal";
            this.Process(command);
        }

        [TestMethod]
        public void AlterSourceRemoveColumns()
        {
            string command = "alter source source1 remove column1 string, column2 int, column3 decimal";
            this.Process(command);
        }

        #endregion alter

        [TestMethod]
        public void AlterUserNameAndDefaultSchema()
        {
            string command = "alter user oscar with name = roberto, default_schema = Schema1";
            this.Process(command);
        }
        
        [TestMethod]
        public void AlterUserDefaultSchema()
        {
            string command = "alter user oscar with default_schema = Schema1";
            this.Process(command);
        }
                
        [TestMethod]
        public void AlterRoleRemoveUser()
        {
            string command = $"alter role role1 with remove = User1";
            this.Process(command);
        }

        [TestMethod]
        public void AlterRoleRemoveUsers()
        {
            string command = $"alter role role1 with remove = User1 User2 user3";
            this.Process(command);
        }

        [TestMethod]
        public void AlterRoleStatusOn()
        {
            string command = $"alter role role1 with status = on";
            this.Process(command);
        }

        [TestMethod]
        public void AlterRoleStatusOff()
        {
            string command = $"alter role role1 with status = off";
            this.Process(command);
        }
        
        [TestMethod]
        public void AlterStreamQueryAndName()
        {
            string eql = "cross " +
                                      "JOIN SourceParaPruebas as t1 WHERE PrimaryAccountNumber == \"9999941616073663_1\" " +
                                      "WITH SourceParaPruebas as t2 WHERE PrimaryAccountNumber == \"9999941616073663_2\" " +
                                      "ON AcquiringInstitutionIdentificationCode == AcquiringInstitutionIdentificationCode " +
                                      "TIMEOUT '00:00:02' " +
                                      //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                      "SELECT (string)PrimaryAccountNumber as c1, PrimaryAccountNumber as c2, 1 as numeroXXX into SourceXYZ ";

            string command = $"alter stream Stream1 with query = {{ {eql} }}, name = Stream2";
            this.Process(command);
        }

        [TestMethod]
        public void AlterStreamQuery()
        {
            string eql = "cross " +
                                      "JOIN SourceParaPruebas as t1 WHERE PrimaryAccountNumber == \"9999941616073663_1\" " +
                                      "WITH SourceParaPruebas as t2 WHERE PrimaryAccountNumber == \"9999941616073663_2\" " +
                                      "ON AcquiringInstitutionIdentificationCode == AcquiringInstitutionIdentificationCode " +
                                      "TIMEOUT '00:00:02' " +
                                      //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                      "SELECT (string)PrimaryAccountNumber as c1, PrimaryAccountNumber as c2, 1 as numeroXXX into SourceXYZ ";

            string command = $"alter stream Schema1.Stream1 with query = {{ {eql} }}";
            this.Process(command);
        }

        [TestMethod]
        public void AlterStreamName()
        {
            string command = "alter stream Stream1 with name = Stream2";
            this.Process(command);
        }

        #endregion alter

        #region grant

        [TestMethod]
        public void GrantAlterAnyRole()
        {
            string roleName = "roleAux";
            string databaseName = "newDatabase";
            string userName = "newUser";
            string otherLogin = "LoginAux";
            string command = $"create database {databaseName}; use {databaseName}; create role {roleName}; create user {userName} with login = {otherLogin}; grant alter any role to user {userName}";
            this.Process(command);
        }

        [TestMethod]
        public void GrantAlterAnyLoginToLogin()
        {
            string command = "grant connect on database Database1 to login Login123";
            command = $"create login Login12345 with password = \"abc\", default_database = Database1";
            this.Process(command);
        }

        #region lists of permissions

        [TestMethod]
        public void GrantPermissionsToUser1()
        {
            string command = "grant create source, alter on stream Stream1, create stream, read on source Source1 to user oscar";
            this.Process(command);
        }

        #endregion lists of permissions

        #region read

        #region to user

        [TestMethod]
        public void GrantReadPermissionToUser2()
        {
            string command = "grant read on source Source1 to user oscar";
            this.Process(command);
            
            
        }

        #endregion to user

        #region to role

        [TestMethod]
        public void GrantReadPermissionToRole2()
        {
            string command = "grant read on source Source1 to role oscar";
            this.Process(command);
            
            
        }

        #endregion to role

        #endregion read

        #region create

        #region to user

        [TestMethod]
        public void GrantCreateSourceToUser()
        {
            string command = "grant create source to user oscar";
            this.Process(command);
        }

        [TestMethod]
        public void GrantCreateStreamToUser()
        {
            string command = "grant create stream to user oscar";
            this.Process(command);
        }

        [TestMethod]
        public void GrantCreateUserToUser()
        {
            string command = "grant alter on user User1 to user oscar";
            this.Process(command);
        }

        [TestMethod]
        public void GrantCreateRoleToUser()
        {
            string command = "grant create role to user oscar";
            this.Process(command);
            
            
        }

        #endregion to user

        #region to role

        [TestMethod]
        public void GrantCreateSourceToRole()
        {
            string command = "grant create source to role RoleX";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void GrantCreateStreamToRole()
        {
            string command = "grant create stream to role RoleX";
            this.Process(command);
        }

        [TestMethod]
        public void GrantCreateUserToRole()
        {
            string command = "grant alter on user User1 to role RoleX";
            this.Process(command);
        }

        [TestMethod]
        public void GrantCreateRoleToRole()
        {
            string command = "grant create role to role RoleX";
            this.Process(command);
            
            
        }

        #endregion to role

        #endregion create
        
        #region alter

        #region to user

        [TestMethod]
        public void GrantAlterStreamToUser()
        {
            string command = "grant alter on stream Stream1 to user oscar";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void GrantAlterUserToUser()
        {
            string command = "grant alter on user User1 to user oscar";
            this.Process(command);
        }
        [TestMethod]
        public void GrantAlterAnyUserToUser()
        {
            string command = "grant alter any user to user oscar";
            this.Process(command);
        }

        #endregion to user

        #region to role

        [TestMethod]
        public void GrantAlterStreamToRole()
        {
            string command = "grant alter on stream Stream1 to role RoleX, role RoleY";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void GrantAlterUserToRole()
        {
            string command = "grant alter on user User1 to role RoleX";
            this.Process(command);
            
            
        }

        #endregion to role

        #endregion alter

        #endregion grant

        #region revoke

        #region lists of permissions

        [TestMethod]
        public void revokePermissionsToUser1()
        {
            string command = "revoke create source, alter on stream Stream1, create stream, read on source Source1 to user oscar";
            this.Process(command);
        }

        #endregion lists of permissions

        #region read

        #region to user

        [TestMethod]
        public void revokeReadPermissionToUser2()
        {
            string command = "revoke read on source Source1 to user oscar";
            this.Process(command);


        }

        #endregion to user

        #region to role

        [TestMethod]
        public void revokeReadPermissionToRole2()
        {
            string command = "revoke read on source Source1 to role oscar";
            this.Process(command);


        }

        #endregion to role

        #endregion read

        #region create

        #region to user

        [TestMethod]
        public void revokeCreateSourceToUser()
        {
            string command = "revoke create source to user oscar";
            this.Process(command);
        }

        [TestMethod]
        public void revokeCreateStreamToUser()
        {
            string command = "revoke create stream to user oscar";
            this.Process(command);
        }

        [TestMethod]
        public void revokeCreateUserToUser()
        {
            string command = "revoke alter on user User1 to user oscar";
            this.Process(command);
        }

        [TestMethod]
        public void revokeCreateRoleToUser()
        {
            string command = "revoke create role to user oscar";
            this.Process(command);


        }

        #endregion to user

        #region to role

        [TestMethod]
        public void revokeCreateSourceToRole()
        {
            string command = "revoke create source to role RoleX";
            this.Process(command);


        }

        [TestMethod]
        public void revokeCreateStreamToRole()
        {
            string command = "revoke create stream to role RoleX";
            this.Process(command);
        }

        [TestMethod]
        public void revokeCreateUserToRole()
        {
            string command = "revoke alter on user User1 to role RoleX";
            this.Process(command);
        }

        [TestMethod]
        public void revokeCreateRoleToRole()
        {
            string command = "revoke create role to role RoleX";
            this.Process(command);


        }

        #endregion to role

        #endregion create
        
        #region alter

        #region to user

        [TestMethod]
        public void revokeAlterStreamToUser()
        {
            string command = "revoke alter on stream Stream1 to user oscar";
            this.Process(command);


        }

        [TestMethod]
        public void revokeAlterUserToUser()
        {
            string command = "revoke alter on user User1 to user oscar";
            this.Process(command);


        }

        #endregion to user

        #region to role

        [TestMethod]
        public void revokeAlterStreamToRole()
        {
            string command = "revoke alter on stream Stream1 to role RoleX";
            this.Process(command);


        }

        [TestMethod]
        public void revokeAlterUserToRole()
        {
            string command = "revoke alter on user User1 to role RoleX";
            this.Process(command);


        }

        #endregion to role

        #endregion alter

        #endregion revoke

        #region deny

        #region lists of permissions

        [TestMethod]
        public void denyPermissionsToUser1()
        {
            string command = "deny create source, alter on stream Stream1, create stream, read on source Source1 to user oscar";
            this.Process(command);
        }

        #endregion lists of permissions

        #region read

        #region to user

        [TestMethod]
        public void denyReadPermissionToUser2()
        {
            string command = "deny read on source Source1 to user oscar";
            this.Process(command);


        }

        #endregion to user

        #region to role
        
        [TestMethod]
        public void denyReadPermissionToRole2()
        {
            string command = "deny read on source Source1 to role oscar";
            this.Process(command);


        }

        #endregion to role

        #endregion read

        #region create

        #region to user

        [TestMethod]
        public void denyCreateSourceToUser()
        {
            string command = "deny create source to user oscar";
            this.Process(command);
        }

        [TestMethod]
        public void denyCreateStreamToUser()
        {
            string command = "deny create stream to user oscar";
            this.Process(command);
        }

        [TestMethod]
        public void denyCreateUserToUser()
        {
            string command = "deny alter on user User1 to user oscar";
            this.Process(command);
        }

        [TestMethod]
        public void denyCreateRoleToUser()
        {
            string command = "deny create role to user oscar";
            this.Process(command);


        }

        #endregion to user

        #region to role

        [TestMethod]
        public void denyCreateSourceToRole()
        {
            string command = "deny create source to role RoleX";
            this.Process(command);


        }

        [TestMethod]
        public void denyCreateStreamToRole()
        {
            string command = "deny create stream to role RoleX";
            this.Process(command);
        }

        [TestMethod]
        public void denyCreateUserToRole()
        {
            string command = "deny alter on user User1 to role RoleX";
            this.Process(command);
        }

        [TestMethod]
        public void denyCreateRoleToRole()
        {
            string command = "deny create role to role RoleX";
            this.Process(command);


        }

        #endregion to role

        #endregion create
             
        #region alter

        #region to user

        [TestMethod]
        public void denyAlterStreamToUser()
        {
            string command = "deny alter on stream Stream1 to user oscar";
            this.Process(command);


        }

        [TestMethod]
        public void denyAlterUserToUser()
        {
            string command = "deny alter on user User1 to user oscar";
            this.Process(command);


        }

        #endregion to user

        #region to role

        [TestMethod]
        public void denyAlterStreamToRole()
        {
            string command = "deny alter on stream Stream1 to role RoleX";
            this.Process(command);
        }

        [TestMethod]
        public void denyAlterUserToRole()
        {
            string command = "deny alter on user User1 to role RoleX";
            this.Process(command);


        }

        #endregion to role

        #endregion alter

        #endregion deny

        #region add

        [TestMethod]
        public void AddUserToRole()
        {
            string command = "add User1 to Role1";
            this.Process(command);
        }

        [TestMethod]
        public void AddUserToRoles()
        {
            string command = "add User1 to Role1, role2, role3";
            this.Process(command);
        }

        [TestMethod]
        public void AddUserListToRole()
        {
            string command = "add User1, User2, User3, User4 to Role1";
            this.Process(command);
        }

        [TestMethod]
        public void AddUserListToRoles()
        {
            string command = "add User1, User2, User3, User4 to Role1, role2, role3";
            this.Process(command);
        }

        #endregion add

        #region remove

        [TestMethod]
        public void RemoveUserToRole()
        {
            string command = "remove User1 to Role1";
            this.Process(command);
        }

        [TestMethod]
        public void RemoveUserToRoles()
        {
            string command = "remove User1 to Role1, role2, role3";
            this.Process(command);
        }

        [TestMethod]
        public void RemoveUserListToRole()
        {
            string command = "remove User1, User2, User3, User4 to Role1";
            this.Process(command);
        }

        [TestMethod]
        public void RemoveUserListToRoles()
        {
            string command = "remove User1, User2, User3, User4 to Role1, role2, role3";
            this.Process(command);
        }

        #endregion remove

        #region

        [TestMethod]
        public void TruncateCommand1()
        {
            string command = "truncate source source1";
            this.Process(command);
        }

        [TestMethod]
        public void TruncateCommand2()
        {
            string command = "truncate source schema1.source1";
            this.Process(command);
        }

        [TestMethod]
        public void TruncateCommand3()
        {
            string command = "truncate source database1.schema1.source1";
            this.Process(command);
        }
        #endregion
    }
}
