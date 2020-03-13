using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleClub.BusinessLogic.Utils
{
    public class Password
    {
        private string clearPassword = String.Empty;
        private int saltKey = 0;
        private string saltedPassword = String.Empty;

        public Password(string clearPassword, int saltKey)
        {
            this.clearPassword = clearPassword;
            this.saltKey = saltKey;
            this.saltedPassword = EncryptionHelper.ComputeSaltedHash(clearPassword, saltKey);
        }

        public Password(string clearPassword)
        {
            this.clearPassword = clearPassword;
            this.saltKey = EncryptionHelper.CreateRandomSalt();
            this.saltedPassword = EncryptionHelper.ComputeSaltedHash(clearPassword, saltKey);
        }

        public Password()
        {
            this.clearPassword = EncryptionHelper.CreateRandomPassword(8);
            this.saltKey = EncryptionHelper.CreateRandomSalt();
            this.saltedPassword = EncryptionHelper.ComputeSaltedHash(clearPassword, saltKey);
        }

        public string ClearPassword
        {
            get { return clearPassword; }
        }

        public int SaltKey
        {
            get { return saltKey; }
        }

        public string SaltedPassword
        {
            get { return saltedPassword; }
        }
    }
}
