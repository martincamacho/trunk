using CastleClub.BusinessLogic.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleClub.Tool.ModifyPassword
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("Welcome to manager for password of GCD, write the password for access:");
            if (System.Console.ReadLine()=="atcartsba")
            {

                System.Console.WriteLine("Select a command (change - change random), exit");
                string command = System.Console.ReadLine();

                while (command.ToLower() != "exit")
                {
                    switch (command.ToLower())
                    {
                        case "change":
                            #region Password change
                            System.Console.WriteLine("\tCustomer ID:");
                            command = System.Console.ReadLine();
                            System.Console.WriteLine("\tNew Password:");
                            string newPassword = System.Console.ReadLine();


                            using (Data.CastleClubEntities1 entities = new Data.CastleClubEntities1())
                            {
                                int id = int.Parse(command);
                                var customerDTChangePassword = entities.Customers.FirstOrDefault(x => x.Id == id);

                                if (customerDTChangePassword != null)
                                {
                                    if (!string.IsNullOrEmpty(newPassword))
                                    {
                                        Password password = new Password(newPassword);
                                        customerDTChangePassword.Password = password.SaltedPassword;
                                        customerDTChangePassword.SaltKey = password.SaltKey;
                                        entities.SaveChanges();
                                    }
                                    else
                                    {
                                        Console.WriteLine("\tThe new password is empty. Is necessary enter a password not empty.");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("\tNot exist some customer in the database with customerID: "+id);
                                }
                            }
                            #endregion
                            break;
                        case "change random":
                            #region Pasword change random
                            System.Console.WriteLine("\tCustomer ID:");
                            command = System.Console.ReadLine();

                            using (Data.CastleClubEntities1 entities2 = new Data.CastleClubEntities1())
                            {
                                int id = int.Parse(command);
                                var customerDTChangePassword = entities2.Customers.FirstOrDefault(x => x.Id == id);
                                if (customerDTChangePassword != null)
                                {
                                    Password password = new Password();
                                    customerDTChangePassword.Password = password.SaltedPassword;
                                    customerDTChangePassword.SaltKey = password.SaltKey;
                                    entities2.SaveChanges();

                                    System.Console.WriteLine("\tThe new password is: " + password.ClearPassword);
                                }
                                else
                                {
                                    System.Console.WriteLine("\tNot exist some customer in the database with this customerID: " + id);
                                }
                            }
                            #endregion
                            break;
                        default:
                            System.Console.WriteLine("\tCommand not match, try again.");
                            break;
                    }

                    System.Console.WriteLine("Select a command (change - change random), exit");
                    command = System.Console.ReadLine();
                }
            }
        }
    }
}
