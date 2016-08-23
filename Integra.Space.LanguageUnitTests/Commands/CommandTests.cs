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
            SystemCommand spaceCommand = cp.Evaluate();

            Console.WriteLine();
        }

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
        public void CreateSource()
        {
            string command = "create source Source1";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void CreateStream()
        {
            string command = "create stream Stream1 { 0123456789abcdefghijklmnñopqrstuvwxyz°!\"#$%&/()=?¡´+[],.-;:_¨*¬^`~ }";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void CreateRole()
        {
            string command = "create role RoleX";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void CreateUserPassAndStatus()
        {
            string command = "create user oscar password \"pass1234\" status enable";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void CreateUserPass()
        {
            string command = "create user oscar password \"pass1234\"";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void CreateUserStatus()
        {
            string command = "create user oscar status enable";
            this.Process(command);
            
            
        }

        #endregion create

        #region alter

        [TestMethod]
        public void AlterStream()
        {
            string command = "alter stream Stream1 { 0123456789abcdefghijklmnñopqrstuvwxyz°!\"#$%&/()=?¡´+[],.-;:_¨*¬^`~ }";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void AlterUserPassAndStatus()
        {
            string command = "alter user oscar password \"passABC\" status disable";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void AlterUserPass()
        {
            string command = "alter user oscar password \"passABC\"";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void AlterUserStatus()
        {
            string command = "alter user oscar status disable";
            this.Process(command);
            
            
        }

        #endregion alter

        #region grant

        #region lists of permissions

        [TestMethod]
        public void GrantPermissionsToUser1()
        {
            string command = "grant create source, read stream Stream1, alter stream Stream1, create stream, read source Source1 to user oscar";
            this.Process(command);
        }

        #endregion lists of permissions

        #region read

        #region to user
        [TestMethod]
        public void GrantReadPermissionToUser1()
        {
            string command = "grant read stream Stream1 to user oscar";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void GrantReadPermissionToUser2()
        {
            string command = "grant read source Source1 to user oscar";
            this.Process(command);
            
            
        }

        #endregion to user

        #region to role

        [TestMethod]
        public void GrantReadPermissionToRole1()
        {
            string command = "grant read stream Stream1 to role oscar";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void GrantReadPermissionToRole2()
        {
            string command = "grant read source Source1 to role oscar";
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
            string command = "grant create user to user oscar";
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
            string command = "grant create user to role RoleX";
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
            string command = "grant Alter stream Stream1 to user oscar";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void GrantAlterUserToUser()
        {
            string command = "grant Alter user User1 to user oscar";
            this.Process(command);
            
            
        }
        
        #endregion to user

        #region to role
        
        [TestMethod]
        public void GrantAlterStreamToRole()
        {
            string command = "grant Alter stream Stream1 to role RoleX";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void GrantAlterUserToRole()
        {
            string command = "grant Alter user User1 to role RoleX";
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
            string command = "revoke create source, read stream Stream1, alter stream Stream1, create stream, read source Source1 to user oscar";
            this.Process(command);
        }

        #endregion lists of permissions

        #region read

        #region to user
        [TestMethod]
        public void revokeReadPermissionToUser1()
        {
            string command = "revoke read stream Stream1 to user oscar";
            this.Process(command);


        }

        [TestMethod]
        public void revokeReadPermissionToUser2()
        {
            string command = "revoke read source Source1 to user oscar";
            this.Process(command);


        }

        #endregion to user

        #region to role

        [TestMethod]
        public void revokeReadPermissionToRole1()
        {
            string command = "revoke read stream Stream1 to role oscar";
            this.Process(command);


        }

        [TestMethod]
        public void revokeReadPermissionToRole2()
        {
            string command = "revoke read source Source1 to role oscar";
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
            string command = "revoke create user to user oscar";
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
            string command = "revoke create user to role RoleX";
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
            string command = "revoke Alter stream Stream1 to user oscar";
            this.Process(command);


        }

        [TestMethod]
        public void revokeAlterUserToUser()
        {
            string command = "revoke Alter user User1 to user oscar";
            this.Process(command);


        }

        #endregion to user

        #region to role

        [TestMethod]
        public void revokeAlterStreamToRole()
        {
            string command = "revoke Alter stream Stream1 to role RoleX";
            this.Process(command);


        }

        [TestMethod]
        public void revokeAlterUserToRole()
        {
            string command = "revoke Alter user User1 to role RoleX";
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
            string command = "deny create source, read stream Stream1, alter stream Stream1, create stream, read source Source1 to user oscar";
            this.Process(command);
        }

        #endregion lists of permissions

        #region read

        #region to user
        [TestMethod]
        public void denyReadPermissionToUser1()
        {
            string command = "deny read stream Stream1 to user oscar";
            this.Process(command);


        }

        [TestMethod]
        public void denyReadPermissionToUser2()
        {
            string command = "deny read source Source1 to user oscar";
            this.Process(command);


        }

        #endregion to user

        #region to role

        [TestMethod]
        public void denyReadPermissionToRole1()
        {
            string command = "deny read stream Stream1 to role oscar";
            this.Process(command);


        }

        [TestMethod]
        public void denyReadPermissionToRole2()
        {
            string command = "deny read source Source1 to role oscar";
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
            string command = "deny create user to user oscar";
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
            string command = "deny create user to role RoleX";
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
            string command = "deny Alter stream Stream1 to user oscar";
            this.Process(command);


        }

        [TestMethod]
        public void denyAlterUserToUser()
        {
            string command = "deny Alter user User1 to user oscar";
            this.Process(command);


        }

        #endregion to user

        #region to role

        [TestMethod]
        public void denyAlterStreamToRole()
        {
            string command = "deny Alter stream Stream1 to role RoleX";
            this.Process(command);


        }

        [TestMethod]
        public void denyAlterUserToRole()
        {
            string command = "deny Alter user User1 to role RoleX";
            this.Process(command);


        }

        #endregion to role

        #endregion alter

        #endregion deny

        #region add

        [TestMethod]
        public void AddUserToRole()
        {
            string command = "add user User1 to role Role1";
            this.Process(command);
        }

        [TestMethod]
        public void AddUserListToRole()
        {
            string command = "add user User1, user User2, user User3, user User4 to role Role1";
            this.Process(command);
        }

        #endregion add
    }
}
