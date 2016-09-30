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

        #region take ownership

        [TestMethod]
        public void TakeOwnershipOnDbRole()
        {
            string command = "take ownership on role Role1";
            this.Process(command);
        }

        [TestMethod]
        public void TakeOwnershipOnDatabase()
        {
            string command = "take ownership on database Database1";
            this.Process(command);
        }

        [TestMethod]
        public void TakeOwnershipOnEndpoint()
        {
            string command = "take ownership on endpoint Endpoint1";
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

        [TestMethod]
        public void UseCommandTest2()
        {
            string command = "use Database1; create source source1";
            this.Process(command);
        }

        [TestMethod]
        public void UseCommandTest3()
        {
            string command = "use Database1; create source source123; use Database2; alter stream StreamXXX with query = { adsifjalkjdfalkdfalkjdhfaljdkhfadjkfhajkdsfhl }; use Database3; drop source source1";
            this.Process(command);
        }

        #endregion use

        #region Status
        #region Start
        [TestMethod]
        public void StartSource()
        {
            string command = "start source fuente1";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void StartStream()
        {
            string command = "start stream fuente1";
            this.Process(command);
            
            
        }

        #endregion Start

        #region Stop
        [TestMethod]
        public void StopSource()
        {
            string command = "stop source fuente1";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void StopStream()
        {
            string command = "stop stream fuente1";
            this.Process(command);
            
            
        }

        #endregion Stop
        #endregion Status

        #region drop
        [TestMethod]
        public void DropSource()
        {
            string command = "drop source Source1";
            this.Process(command);
        }

        [TestMethod]
        public void DropStream()
        {
            string command = "drop stream Source1";
            this.Process(command);
        }

        [TestMethod]
        public void DropUser()
        {
            string command = "drop user Usuario1";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void DropRole()
        {
            string command = "drop role myRole";
            this.Process(command);
            
            
        }

        #endregion role

        #region create

        [TestMethod]
        public void DropSchema()
        {
            string command = "drop schema schema1, schema2, schema3";
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
                                   "JOIN SpaceObservable1 as t1 WHERE t1.@event.Message.#1.#2 == \"9999941616073663_1\" " +
                                   "WITH SpaceObservable1 as t2 WHERE t2.@event.Message.#1.#2 == \"9999941616073663_2\" " +
                                   "ON t1.@event.Message.#1.#32 == t2.@event.Message.#1.#32 " +
                                   "TIMEOUT '00:00:02' " +
                                   //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                   "SELECT (string)t1.@event.Message.#1.#2 as c1, t2.@event.Message.#1.#2 as c2, 1 as numeroXXX ";

            string command = $"create stream stream123 {{ {eql} }}";
            this.Process(command);
        }

        [TestMethod]
        public void CreateSource()
        {
            string command = "create source source123";
            this.Process(command);
        }

        [TestMethod]
        public void CreateRole()
        {
            string command = "create role role123";
            this.Process(command);
        }

        [TestMethod]
        public void CreateUser()
        {
            string command = "create user user123";
            this.Process(command);
        }

        [TestMethod]
        public void CreateUserWithDefaultSchema()
        {
            string command = "Create user user123 with default_schema = Schema1";
            this.Process(command);
        }

        [TestMethod]
        public void CreateDatabase()
        {
            string command = "create database database123";
            this.Process(command);
        }

        [TestMethod]
        public void CreateLogin()
        {
            string command = "create login Login123 with password = \"abc\"";
            this.Process(command);
        }

        [TestMethod]
        public void CreateLoginWithDefaultDatabase()
        {
            string command = "create login Login123 with password = \"abc\", default_database = Database123";
            this.Process(command);
        }
                
        [TestMethod]
        public void CreateUserDefaultSchema()
        {
            string command = "create user User1 with default_schema = Schema1";
            this.Process(command);
        }

        #endregion create

        #region alter

        [TestMethod]
        public void AlterLogin()
        {
            string command = "alter login LoginXXX with default_database = Database1, name = LoginYYY";
            this.Process(command);
        }

        [TestMethod]
        public void AlterUserNameAndDefaultSchema()
        {
            string command = "alter user oscar with name = roberto, default_schema = Schema1";
            this.Process(command);
        }

        [TestMethod]
        public void AlterUserName()
        {
            string command = "alter user oscar with name = roberto";
            this.Process(command);
        }

        [TestMethod]
        public void AlterUserDefaultSchema()
        {
            string command = "alter user oscar with default_schema = Schema1";
            this.Process(command);
        }

        [TestMethod]
        public void AlterDatabaseName()
        {
            string command = "alter database db1 with name = db2";
            this.Process(command);
        }

        [TestMethod]
        public void AlterRoleName()
        {
            string command = "alter role role1 with name = role2";
            this.Process(command);
        }

        [TestMethod]
        public void AlterSchemaName()
        {
            string command = "alter schema schema1 with name = schema2";
            this.Process(command);
        }

        [TestMethod]
        public void AlterSourceName()
        {
            string command = "alter source source1 with name = source2";
            this.Process(command);
        }

        [TestMethod]
        public void AlterStreamQueryAndName()
        {
            string command = "alter stream Stream1 with query = { 0123456789abcdefghijklmnñopqrstuvwxyz°!\"#$%&/()=?¡´+[],.-;:_¨*¬^`~ }, name = Stream2";
            this.Process(command);
        }

        [TestMethod]
        public void AlterStreamQuery()
        {
            string command = "alter stream Stream1 with query = { 0123456789abcdefghijklmnñopqrstuvwxyz°!\"#$%&/()=?¡´+[],.-;:_¨*¬^`~ }";
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

        #region lists of permissions

        [TestMethod]
        public void GrantPermissionsToUser1()
        {
            string command = "grant create source, read on stream Stream1, alter on stream Stream1, create stream, read on source Source1 to user oscar";
            this.Process(command);
        }

        #endregion lists of permissions

        #region read

        #region to user
        [TestMethod]
        public void GrantReadPermissionToUser1()
        {
            string command = "grant read on stream Stream1 to user oscar";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void GrantReadPermissionToUser2()
        {
            string command = "grant read on source Source1 to user oscar";
            this.Process(command);
            
            
        }

        #endregion to user

        #region to role

        [TestMethod]
        public void GrantReadPermissionToRole1()
        {
            string command = "grant read on stream Stream1 to role oscar";
            this.Process(command);
            
            
        }

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
            string command = "revoke create source, read on stream Stream1, alter on stream Stream1, create stream, read on source Source1 to user oscar";
            this.Process(command);
        }

        #endregion lists of permissions

        #region read

        #region to user
        [TestMethod]
        public void revokeReadPermissionToUser1()
        {
            string command = "revoke read on stream Stream1 to user oscar";
            this.Process(command);


        }

        [TestMethod]
        public void revokeReadPermissionToUser2()
        {
            string command = "revoke read on source Source1 to user oscar";
            this.Process(command);


        }

        #endregion to user

        #region to role

        [TestMethod]
        public void revokeReadPermissionToRole1()
        {
            string command = "revoke read on stream Stream1 to role oscar";
            this.Process(command);


        }

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
            string command = "deny create source, read on stream Stream1, alter on stream Stream1, create stream, read on source Source1 to user oscar";
            this.Process(command);
        }

        #endregion lists of permissions

        #region read

        #region to user
        [TestMethod]
        public void denyReadPermissionToUser1()
        {
            string command = "deny read on stream Stream1 to user oscar";
            this.Process(command);


        }

        [TestMethod]
        public void denyReadPermissionToUser2()
        {
            string command = "deny read on source Source1 to user oscar";
            this.Process(command);


        }

        #endregion to user

        #region to role

        [TestMethod]
        public void denyReadPermissionToRole1()
        {
            string command = "deny read on stream Stream1 to role oscar";
            this.Process(command);


        }

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
        public void AddUserListToRole()
        {
            string command = "add User1, User2, User3, User4 to Role1";
            this.Process(command);
        }

        #endregion add
    }
}
