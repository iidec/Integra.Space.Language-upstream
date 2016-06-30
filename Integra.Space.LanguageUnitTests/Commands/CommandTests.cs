using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Integra.Space.Language;
using Integra.Space.Language.CommandContext;

namespace Integra.Space.LanguageUnitTests.Commands
{
    [TestClass]
    public class CommandTests
    {
        public void Process(string command)
        {
            CommandParser cp = new CommandParser(command);
            PipelineCommandContext context = cp.Evaluate();
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
            string command = "create user oscar password 'pass1234' status enable";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void CreateUserPass()
        {
            string command = "create user oscar password 'pass1234'";
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
            string command = "alter user oscar password 'passABC' status disable";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void AlterUserPass()
        {
            string command = "alter user oscar password 'passABC'";
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
            string command = "grant create source, read stream Stream1, alter source, drop source, create stream, read source Source1 to user oscar";
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
            string command = "grant create source to user oscar";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void GrantCreateUserToUser()
        {
            string command = "grant create source to user oscar";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void GrantCreateRoleToUser()
        {
            string command = "grant create source to user oscar";
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
            string command = "grant create source to role RoleX";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void GrantCreateUserToRole()
        {
            string command = "grant create source to role RoleX";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void GrantCreateRoleToRole()
        {
            string command = "grant create source to role RoleX";
            this.Process(command);
            
            
        }

        #endregion to role

        #endregion create

        #region drop

        #region to user

        [TestMethod]
        public void GrantDropSourceToUser()
        {
            string command = "grant Drop source to user oscar";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void GrantDropStreamToUser()
        {
            string command = "grant Drop source to user oscar";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void GrantDropUserToUser()
        {
            string command = "grant Drop source to user oscar";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void GrantDropRoleToUser()
        {
            string command = "grant Drop source to user oscar";
            this.Process(command);
            
            
        }

        #endregion to user

        #region to role

        [TestMethod]
        public void GrantDropSourceToRole()
        {
            string command = "grant Drop source to role RoleX";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void GrantDropStreamToRole()
        {
            string command = "grant Drop source to role RoleX";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void GrantDropUserToRole()
        {
            string command = "grant Drop source to role RoleX";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void GrantDropRoleToRole()
        {
            string command = "grant Drop source to role RoleX";
            this.Process(command);
            
            
        }

        #endregion to role

        #endregion drop

        #region alter

        #region to user

        [TestMethod]
        public void GrantAlterSourceToUser()
        {
            string command = "grant Alter source to user oscar";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void GrantAlterStreamToUser()
        {
            string command = "grant Alter source to user oscar";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void GrantAlterUserToUser()
        {
            string command = "grant Alter source to user oscar";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void GrantAlterRoleToUser()
        {
            string command = "grant Alter source to user oscar";
            this.Process(command);
            
            
        }

        #endregion to user

        #region to role

        [TestMethod]
        public void GrantAlterSourceToRole()
        {
            string command = "grant Alter source to role RoleX";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void GrantAlterStreamToRole()
        {
            string command = "grant Alter source to role RoleX";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void GrantAlterUserToRole()
        {
            string command = "grant Alter source to role RoleX";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void GrantAlterRoleToRole()
        {
            string command = "grant Alter source to role RoleX";
            this.Process(command);
            
            
        }

        #endregion to role

        #endregion alter

        #endregion grant

        #region revoke

        #region lists of permissions

        [TestMethod]
        public void RevokePermissionsToUser1()
        {
            string command = "Revoke create source, read stream Stream1, alter source, drop source, create stream, read source Source1 to user oscar";
            this.Process(command);
            
            
        }

        #endregion lists of permissions

        #region read

        #region to user
        [TestMethod]
        public void RevokeReadPermissionToUser1()
        {
            string command = "Revoke read stream Stream1 to user oscar";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void RevokeReadPermissionToUser2()
        {
            string command = "Revoke read source Source1 to user oscar";
            this.Process(command);
            
            
        }

        #endregion to user

        #region to role

        [TestMethod]
        public void RevokeReadPermissionToRole1()
        {
            string command = "Revoke read stream Stream1 to role oscar";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void RevokeReadPermissionToRole2()
        {
            string command = "Revoke read source Source1 to role oscar";
            this.Process(command);
            
            
        }

        #endregion to role

        #endregion read

        #region create

        #region to user

        [TestMethod]
        public void RevokeCreateSourceToUser()
        {
            string command = "Revoke create source to user oscar";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void RevokeCreateStreamToUser()
        {
            string command = "Revoke create source to user oscar";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void RevokeCreateUserToUser()
        {
            string command = "Revoke create source to user oscar";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void RevokeCreateRoleToUser()
        {
            string command = "Revoke create source to user oscar";
            this.Process(command);
            
            
        }

        #endregion to user

        #region to role

        [TestMethod]
        public void RevokeCreateSourceToRole()
        {
            string command = "Revoke create source to role RoleX";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void RevokeCreateStreamToRole()
        {
            string command = "Revoke create source to role RoleX";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void RevokeCreateUserToRole()
        {
            string command = "Revoke create source to role RoleX";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void RevokeCreateRoleToRole()
        {
            string command = "Revoke create source to role RoleX";
            this.Process(command);
            
            
        }

        #endregion to role

        #endregion create

        #region drop

        #region to user

        [TestMethod]
        public void RevokeDropSourceToUser()
        {
            string command = "Revoke Drop source to user oscar";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void RevokeDropStreamToUser()
        {
            string command = "Revoke Drop source to user oscar";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void RevokeDropUserToUser()
        {
            string command = "Revoke Drop source to user oscar";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void RevokeDropRoleToUser()
        {
            string command = "Revoke Drop source to user oscar";
            this.Process(command);
            
            
        }

        #endregion to user

        #region to role

        [TestMethod]
        public void RevokeDropSourceToRole()
        {
            string command = "Revoke Drop source to role RoleX";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void RevokeDropStreamToRole()
        {
            string command = "Revoke Drop source to role RoleX";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void RevokeDropUserToRole()
        {
            string command = "Revoke Drop source to role RoleX";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void RevokeDropRoleToRole()
        {
            string command = "Revoke Drop source to role RoleX";
            this.Process(command);
            
            
        }

        #endregion to role

        #endregion drop

        #region alter

        #region to user

        [TestMethod]
        public void RevokeAlterSourceToUser()
        {
            string command = "Revoke Alter source to user oscar";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void RevokeAlterStreamToUser()
        {
            string command = "Revoke Alter source to user oscar";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void RevokeAlterUserToUser()
        {
            string command = "Revoke Alter source to user oscar";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void RevokeAlterRoleToUser()
        {
            string command = "Revoke Alter source to user oscar";
            this.Process(command);
            
            
        }

        #endregion to user

        #region to role

        [TestMethod]
        public void RevokeAlterSourceToRole()
        {
            string command = "Revoke Alter source to role RoleX";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void RevokeAlterStreamToRole()
        {
            string command = "Revoke Alter source to role RoleX";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void RevokeAlterUserToRole()
        {
            string command = "Revoke Alter source to role RoleX";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void RevokeAlterRoleToRole()
        {
            string command = "Revoke Alter source to role RoleX";
            this.Process(command);
            
            
        }

        #endregion to role

        #endregion alter

        #endregion revoke

        #region deny

        #region lists of permissions

        [TestMethod]
        public void DenyPermissionsToUser1()
        {
            string command = "Deny create source, read stream Stream1, alter source, drop source, create stream, read source Source1 to user oscar";
            this.Process(command);
            
            
        }

        #endregion lists of permissions

        #region read

        #region to user
        [TestMethod]
        public void DenyReadPermissionToUser1()
        {
            string command = "Deny read stream Stream1 to user oscar";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void DenyReadPermissionToUser2()
        {
            string command = "Deny read source Source1 to user oscar";
            this.Process(command);
            
            
        }

        #endregion to user

        #region to role

        [TestMethod]
        public void DenyReadPermissionToRole1()
        {
            string command = "Deny read stream Stream1 to role oscar";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void DenyReadPermissionToRole2()
        {
            string command = "Deny read source Source1 to role oscar";
            this.Process(command);
            
            
        }

        #endregion to role

        #endregion read

        #region create

        #region to user

        [TestMethod]
        public void DenyCreateSourceToUser()
        {
            string command = "Deny create source to user oscar";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void DenyCreateStreamToUser()
        {
            string command = "Deny create source to user oscar";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void DenyCreateUserToUser()
        {
            string command = "Deny create source to user oscar";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void DenyCreateRoleToUser()
        {
            string command = "Deny create source to user oscar";
            this.Process(command);
            
            
        }

        #endregion to user

        #region to role

        [TestMethod]
        public void DenyCreateSourceToRole()
        {
            string command = "Deny create source to role RoleX";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void DenyCreateStreamToRole()
        {
            string command = "Deny create source to role RoleX";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void DenyCreateUserToRole()
        {
            string command = "Deny create source to role RoleX";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void DenyCreateRoleToRole()
        {
            string command = "Deny create source to role RoleX";
            this.Process(command);
            
            
        }

        #endregion to role

        #endregion create

        #region drop

        #region to user

        [TestMethod]
        public void DenyDropSourceToUser()
        {
            string command = "Deny Drop source to user oscar";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void DenyDropStreamToUser()
        {
            string command = "Deny Drop source to user oscar";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void DenyDropUserToUser()
        {
            string command = "Deny Drop source to user oscar";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void DenyDropRoleToUser()
        {
            string command = "Deny Drop source to user oscar";
            this.Process(command);
            
            
        }

        #endregion to user

        #region to role

        [TestMethod]
        public void DenyDropSourceToRole()
        {
            string command = "Deny Drop source to role RoleX";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void DenyDropStreamToRole()
        {
            string command = "Deny Drop source to role RoleX";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void DenyDropUserToRole()
        {
            string command = "Deny Drop source to role RoleX";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void DenyDropRoleToRole()
        {
            string command = "Deny Drop source to role RoleX";
            this.Process(command);
            
            
        }

        #endregion to role

        #endregion drop

        #region alter

        #region to user

        [TestMethod]
        public void DenyAlterSourceToUser()
        {
            string command = "Deny Alter source to user oscar";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void DenyAlterStreamToUser()
        {
            string command = "Deny Alter source to user oscar";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void DenyAlterUserToUser()
        {
            string command = "Deny Alter source to user oscar";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void DenyAlterRoleToUser()
        {
            string command = "Deny Alter source to user oscar";
            this.Process(command);
            
            
        }

        #endregion to user

        #region to role

        [TestMethod]
        public void DenyAlterSourceToRole()
        {
            string command = "Deny Alter source to role RoleX";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void DenyAlterStreamToRole()
        {
            string command = "Deny Alter source to role RoleX";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void DenyAlterUserToRole()
        {
            string command = "Deny Alter source to role RoleX";
            this.Process(command);
            
            
        }

        [TestMethod]
        public void DenyAlterRoleToRole()
        {
            string command = "Deny Alter source to role RoleX";
            this.Process(command);
            
            
        }

        #endregion to role

        #endregion alter

        #endregion deny
    }
}
